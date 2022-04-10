using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConsts
{
    public const int k_MainSceneIndex = 1;

    public const string k_PlayerLayerName = "Player";
    public const string k_NPCLayerName = "NPC";
    public const string k_WorldLayerName = "World";

    public const string k_GameEventDispatcherPath = "Data/GameEvents";
    public const string k_ResourcesGameDataPath = "Data/";
    public const string k_ResourcesGiftPrefabPath = "Prefabs/Things/GiftInstance";
    public const string k_ResourcesNPCPrefabPath = "Prefabs/NPC/NPCInstance";

    private static Dictionary<string, GameObject> _loadedPrefabs = new Dictionary<string, GameObject>();
    public static GameObject GetPrefab(string prefabPath)
    {
        GameObject ret;
        if (!_loadedPrefabs.TryGetValue(prefabPath, out ret))
        {
            ret = Resources.Load<GameObject>(prefabPath);
            _loadedPrefabs[prefabPath] = ret;
        }
        return ret;
    }

    private static GameEventDispatcher _eventDispatcher = null;
    public static GameEventDispatcher GetDispatcher()
    {
        if (_eventDispatcher == null)
            _eventDispatcher = Resources.Load<GameEventDispatcher>(k_GameEventDispatcherPath);
        return _eventDispatcher;
    }

    public static GameMgr gameManager = null;
    public static EventMgr eventManager = null;
    public static UIMgr uiMgr = null;

    private static Vector2? _worldCameraMin;
    public static Vector2 worldCameraMin
    {
        get
        {
            if(_worldCameraMin == null)
            {
                Camera cam = Camera.main;
                float h = cam.orthographicSize, w = cam.orthographicSize * cam.aspect;
                Vector2 localMin = new Vector2(-w, -h);
                _worldCameraMin = cam.transform.TransformPoint(localMin);
            }
            return _worldCameraMin.Value;
        }
    }
    private static Vector2? _worldCameraMax;
    public static Vector2 worldCameraMax
    {
        get
        {
            if (_worldCameraMax == null)
            {
                Camera cam = Camera.main;
                float h = cam.orthographicSize, w = cam.orthographicSize * cam.aspect;
                Vector2 localMax = new Vector2(w, h);
                _worldCameraMax = cam.transform.TransformPoint(localMax);
            }
            return _worldCameraMax.Value;
        }
    }

    public static DebugMgr debugMgr = null;
}