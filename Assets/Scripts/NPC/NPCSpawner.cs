using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] NPCSpawnConfig _spawnConfig;
    const int _maxSimultaneousSpawn = 3;
    const int _maxReadyQueueSize = 1024;
    Lottery _npcSpawnLottery;
    NPCType[] _npcTypes;
    float[] _spawnYs;
    float[] _spawnTimers;
    Queue<(NPCType, Vector2)> _ready = new Queue<(NPCType, Vector2)>();

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
                _ready.Enqueue(((NPCType)_npcSpawnLottery.NextItem(), new Vector2(transform.position.x, _spawnYs[i])));
                _spawnTimers[i] = _spawnConfig.NpcSpawnInterval[i];
            }
        }

        for (int i = 0; i < _maxSimultaneousSpawn && _ready.Count > 0; ++i)
        {
            var desc = _ready.Dequeue();
            NPCType type = desc.Item1;
            Vector2 pos = desc.Item2;

            NPCInstance.Create(type, pos);
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
