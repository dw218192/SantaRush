using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] NPCSpawnConfig _spawnConfig;
    Lottery _npcSpawnLottery;
    NPCType[] _npcTypes;
    float[] _spawnYs;

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

    IEnumerator _SpawnPointRoutine(float interval, float posY)
    {
        Vector2 pos = transform.position;
        pos.y = posY;
        while(true)
        {
            yield return new WaitForSeconds(interval);
            NPCType type = (NPCType)_npcSpawnLottery.NextItem();
            NPCInstance.Create(type, pos);
        }
    }

    void Start()
    {
        // grab all NPC types
        _npcTypes = Resources.LoadAll<NPCType>(GameConsts.k_ResourcesGameDataPath);
        _npcSpawnLottery = new Lottery(_npcTypes);
        _spawnYs = GenSpawnPointYs(GameConsts.worldCameraMin.y, GameConsts.worldCameraMax.y, _spawnConfig);

        // to outside the screen
        transform.position = GameConsts.worldCameraMax;
        transform.Translate(new Vector2(1, 0));

        // start spawning
        for (int i = 0; i < _spawnYs.Length; ++i)
            StartCoroutine(_SpawnPointRoutine(_spawnConfig.NpcSpawnInterval[i], _spawnYs[i]));
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
