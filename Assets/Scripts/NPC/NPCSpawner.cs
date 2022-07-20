using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    struct SpawnDesc
    {
        public NPCType type;
        public Vector2 pos;
        public int spawnPointIndex;

        public SpawnDesc(NPCType type, Vector2 pos, int spawnPointIndex)
        {
            this.type = type;
            this.pos = pos;
            this.spawnPointIndex = spawnPointIndex;
        }
    }

    [SerializeField] NPCSpawnConfig _spawnConfig;
    const int _maxSimultaneousSpawn = 3;
    const int _maxReadyQueueSize = 1024;
    float[] _spawnYs;

    // RUNTIME -------
    Lottery _npcSpawnLottery;
    NPCType[] _npcTypes;
    float[] _spawnTimers; //spawn timer per spawn point
    int[] _activeNpcs;    //number of active npcs spawned that are on the screen per spawn point
    Queue<SpawnDesc> _ready = new Queue<SpawnDesc>();

    public static float[] GenSpawnPointYs(float viewYMin, float viewYMax, NPCSpawnConfig config)
    {
        List<float> ret = new List<float>();
        float halfHeight = config.NpcHeight / 2;
        float minY = viewYMin + config.BottomOffset + halfHeight;
        float maxY = viewYMax - config.TopOffset - halfHeight;
        float curY = minY;
        while(curY < maxY)
        {
            ret.Add(curY);
            curY += config.NpcHeight + config.YSpawnPointDist;
        }
        return ret.ToArray();
    }

    void Start()
    {
        // grab all NPC types
        _npcTypes = Resources.LoadAll<NPCType>(GameConsts.k_ResourcesGameDataPath);
        _npcSpawnLottery = new Lottery(_npcTypes);
        _spawnYs = GenSpawnPointYs(GameConsts.worldCameraMin.y, GameConsts.worldCameraMax.y, _spawnConfig);
        _spawnTimers = new float[_spawnYs.Length];
        _activeNpcs = new int[_spawnYs.Length];

        // to outside the screen
        transform.position = GameConsts.worldCameraMax;
        transform.Translate(new Vector2(1, 0));

        // start spawning
        for (int i = 0; i < _spawnYs.Length; ++i)
            _spawnTimers[i] = _spawnConfig.NpcSpawnInterval[i];
    }

    void Update()
    {
        for (int i = 0; i < _spawnYs.Length; ++i)
        {
            _spawnTimers[i] = Mathf.Max(_spawnTimers[i] - Time.deltaTime, 0);
            if(Mathf.Approximately(_spawnTimers[i], 0) && _ready.Count < _maxReadyQueueSize)
            {
                _ready.Enqueue(new SpawnDesc((NPCType)_npcSpawnLottery.NextItem(), new Vector2(transform.position.x, _spawnYs[i]), i));
                _spawnTimers[i] = _spawnConfig.NpcSpawnInterval[i];
            }
        }

        for (int i = 0; i < _maxSimultaneousSpawn && _ready.Count > 0; ++i)
        {
            SpawnDesc desc = _ready.Dequeue();
            if (_activeNpcs[desc.spawnPointIndex] > 0)
                continue;
            
            ++_activeNpcs[desc.spawnPointIndex];
            NPCInstance.Create(desc.type, desc.pos, ()=> {
                -- _activeNpcs[desc.spawnPointIndex];
            });
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
