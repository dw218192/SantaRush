using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Scroller), typeof(HurtboxGroup))]
public class NPCInstance : MonoBehaviour
{
    public static NPCInstance Create(NPCType type, Vector2 pos)
    {
        var go = Instantiate(GameConsts.GetPrefab(GameConsts.k_ResourcesNPCPrefabPath));
        go.name = $"{type}_NPCInstance";

        go.SetActive(false); // required to do initialization before awake

        var ret = go.GetComponent<NPCInstance>();
        ret.NpcType = type;

        go.transform.position = pos;
        go.SetActive(true);
        return ret;
    }

    
    List<NPCPart> _parts = new List<NPCPart>();
    Transform _partParent = null;
    bool _isTailInBound = false;
    float _totalWidth = 0;
    Scroller _scroller = null;
    float _giftSpawnTimer = 0;

    NPCType _npcType;
    public NPCType NpcType { get => _npcType; set => _npcType = value; }
    public Vector2 PartCenter 
    { 
        get
        {
            Vector2 center = Vector2.zero;
            foreach (NPCPart part in _parts)
                center += new Vector2(part.transform.position.x, transform.position.y);
            return center / _parts.Count;
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
        _parts.Add(new_part);
    }

    // Start is called before the first frame update
    void Start()
    {
        _partParent = new GameObject("parts").transform;
        _partParent.parent = transform;
        _partParent.localPosition = new Vector2(-NpcType.NpcPartDist, 0);

        foreach (NPCPartDesc desc in NpcType.NpcParts)
        {
            for (int i = 0; i < desc.count; ++i)
                AddPart(desc.prefab);
        }

        _scroller = GetComponent<Scroller>();
        _scroller.BaseSpeed = -(NpcType.Speed + GameConsts.gameManager.StageTable.InitScrollSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Bounds camBound = new Bounds();
        camBound.min = GameConsts.worldCameraMin;
        camBound.max = GameConsts.worldCameraMax;
        Vector3 tailPos = transform.TransformPoint(new Vector3(_totalWidth, 0));
        bool tailInBound = camBound.Contains(tailPos);
        if (_isTailInBound && !tailInBound)  // when the tail of NPC first leaves the screen
            Destroy(gameObject);
        _isTailInBound = tailInBound;


        if(_giftSpawnTimer >= NpcType.GiftSpawnCooldown)
        {
            SpawnGift();
            _giftSpawnTimer = 0f;
        }

        _giftSpawnTimer += Time.deltaTime;
    }

    void SpawnGift()
    {
        var gift = GiftInstance.Create(NpcType.GiftType, PartCenter);
        foreach(var part in _parts)
            gift.Hitbox.AddExcludeList(part.Hurtbox);
    }

    [HurtboxHandler]
    public void OnNPCCollide(Hitbox inflictor)
    {
        if (inflictor.gameObject.layer == LayerMask.NameToLayer(GameConsts.k_WorldLayerName) || 
            inflictor.gameObject.layer == LayerMask.NameToLayer(GameConsts.k_PlayerLayerName))
            Destroy(gameObject);
    }
}
