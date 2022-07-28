using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Scroller), typeof(HurtboxGroup))]
public class NPCInstance : MonoBehaviour
{
    public static NPCInstance Create(NPCType type, Vector2 pos, Action onDeath = null)
    {
        var go = Instantiate(GameConsts.GetPrefab(GameConsts.k_ResourcesNPCPrefabPath));
        go.name = $"{type}_NPCInstance";

        go.SetActive(false); // required to do initialization before awake

        var ret = go.GetComponent<NPCInstance>();
        ret.NpcType = type;
        ret.OnDeath = onDeath;

        go.transform.position = pos;
        go.SetActive(true);
        return ret;
    }
    static AudioClip s_npcDeathSound = null;
    
    List<NPCPart> _parts = new List<NPCPart>();
    Transform _partParent = null;
    bool _isTailInBound = false;
    float _totalWidth = 0;
    Scroller _scroller = null;
    float _giftSpawnTimer = 0;

    NPCType _npcType;
    public NPCType NpcType { get => _npcType; set => _npcType = value; }
    public Action OnDeath { get; private set; } = null;

    public bool NoBomb { get => Mathf.Approximately(_npcType.BombProbability, 0f); }
    public bool AlwaysBomb { get => Mathf.Approximately(_npcType.BombProbability, 1f); }

    public Vector2 GiftSpawnPos 
    { 
        get
        {
            return _partParent.TransformPoint(new Vector2(_totalWidth * NpcType.NpcGiftSpawnLerpPercentage, 0));
        }
    }

    public float TotalWidth { get => _totalWidth; }

    void SetupParts()
    {
        _partParent = new GameObject("parts").transform;
        _partParent.parent = transform;
        _partParent.localPosition = new Vector2(-NpcType.NpcPartDist, 0);

        foreach (var desc in NpcType.NpcParts)
        {
            for (int i = 0; i < desc.count; ++i)
                AddPart(desc.prefab);
        }
    }

    void AddPart(NPCPart prefab)
    {
        NPCPart last = _parts.Count > 0 ? _parts[_parts.Count - 1] : null;
        NPCPart new_part = Instantiate(prefab, _partParent);

        Vector2 local_pos;
        if (last != null)
        {
            local_pos = last.transform.localPosition;
            float delta_x = last.Width / 2 + NpcType.NpcPartDist + new_part.Width / 2;
            local_pos.x += delta_x;

            _totalWidth += new_part.Width + NpcType.NpcPartDist;
        }
        else
        {
            local_pos = Vector2.zero;

            _totalWidth += new_part.Width;
        }

        
        new_part.transform.localPosition = local_pos;
        new_part.gameObject.layer = gameObject.layer;
        new_part.Owner = this;

        _parts.Add(new_part);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(GameConsts.k_NPCLayerName);
        SetupParts();

        _scroller = GetComponent<Scroller>();
        _scroller.BaseSpeed = -(NpcType.Speed + GameConsts.gameManager.StageTable.InitScrollSpeed);

        if (s_npcDeathSound == null)
        {
            s_npcDeathSound = Resources.Load<AudioClip>(GameConsts.k_ResourcesNPCDeathSoundPath);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Bounds camBound = new Bounds();
        camBound.min = GameConsts.worldCameraMin;
        camBound.max = GameConsts.worldCameraMax;

        Vector3 tailPos = transform.TransformPoint(new Vector3(_totalWidth, 0));
        Vector3 headPos = transform.TransformPoint(Vector3.zero);
        bool tailInBound = camBound.Contains(tailPos);
        if (_isTailInBound && !tailInBound)  // when the tail of NPC first leaves the screen
            Die(false);
        
        _isTailInBound = tailInBound;

        
        //shrink the camera bound a bit
        var tmp = camBound.min;
        tmp.x += 0.1f * camBound.size.x;
        camBound.min = tmp;

        if (_giftSpawnTimer >= NpcType.GiftSpawnCooldown)
        {
            bool headInBound = camBound.Contains(headPos);
            // only spawn if the head of the NPC is still visible on the screen
            if (headInBound)
                SpawnGiftOrBomb();
            
            _giftSpawnTimer = 0f;
        }

        _giftSpawnTimer += Time.deltaTime;

        // draw debug NPC bound
        Debug.DrawLine(camBound.min, camBound.min + new Vector3(camBound.size.x, 0), Color.red);
        Debug.DrawLine(camBound.min + new Vector3(camBound.size.x, 0), camBound.max, Color.red);
        Debug.DrawLine(camBound.max, camBound.max - new Vector3(camBound.size.x, 0), Color.red);
        Debug.DrawLine(camBound.max - new Vector3(camBound.size.x, 0), camBound.min, Color.red);
    }

    void SpawnGiftOrBomb()
    {
        if (AlwaysBomb || (!NoBomb && Random.Range(0f, 1f) <= NpcType.BombProbability))
        {
            Debug.Assert(!NoBomb, this);

            var bomb = BombInstance.Create(GiftSpawnPos);
            foreach (var part in _parts)
                bomb.Hitbox.AddExcludeList(part.Hurtbox);
        }
        else
        {
            Debug.Assert(!AlwaysBomb, this);

            var gift = GiftInstance.Create(NpcType.GiftType, GiftSpawnPos);
            foreach (var part in _parts)
                gift.Hitbox.AddExcludeList(part.Hurtbox);
        }
    }

    public void Die(bool useEffect)
    {
        if(useEffect)
        {
            GameConsts.audioMgr.PlayEffect(s_npcDeathSound);
        }
        
        Destroy(gameObject);
        OnDeath?.Invoke();
    }

    [HurtboxHandler]
    public void OnNPCCollide(Hitbox inflictor)
    {
        if (inflictor.IsInLayer(GameConsts.k_WorldLayerName) ||
            inflictor.IsInLayer(GameConsts.k_PlayerLayerName))
            Die(true);
    }
}
