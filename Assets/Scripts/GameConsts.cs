using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class GameConsts
{
    public const int k_MainSceneIndex = 1;

    public const string k_PlayerLayerName = "Player";
    public const string k_NPCLayerName = "NPC";
    public const string k_WorldLayerName = "World";
    public const string k_GiftLayerName = "Gift";
    public const string k_BombLayerName = "Bomb";

    public const string k_GameEventDispatcherPath = "Data/GameEvents";
    public const string k_ResourcesGameDataPath = "Data/";
    public const string k_ResourcesGiftPrefabPath = "Prefabs/Things/GiftInstance";
    public const string k_ResourcesBombPrefabPath = "Prefabs/Things/BombInstance";
    public const string k_ResourcesNPCPrefabPath = "Prefabs/NPC/NPCInstance";
    public const string k_ResourcesArtPath = "Art/";
    public const string k_GameFontPath = "Art/Font/Muyao-Softbrush-2";

    public const string k_PlayerPrefHighestScore = "HighestScore";
    public const string k_PlayerPrefTutorialSkip = "TutorialSkip";

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

    private static Font _gameFont = null;
    public static Font GetGameFont()
    {
        if (_gameFont == null)
            _gameFont = Resources.Load<Font>(k_GameFontPath);
        return _gameFont;
    }

    private static GameEventDispatcher _eventDispatcher = null;
    public static GameEventDispatcher GetDispatcher()
    {
        if (_eventDispatcher == null)
            _eventDispatcher = Resources.Load<GameEventDispatcher>(k_GameEventDispatcherPath);
        return _eventDispatcher;
    }

    public static GameMgr gameManager
    {
        get;
        set;
    }

    public static EventMgr eventManager
    {
        get;
        set;
    }

    public static UIMgr uiMgr
    {
        get;
        set;
    }

    private static Vector2? _worldCameraMin = null;
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
    private static Vector2? _worldCameraMax = null;
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

    private static Language? _curLanguage = null;
    public static Language curLanguage 
    {
        get
        {
            if(_curLanguage == null)
            {
                curLanguage = Language.CHN;
            }

            return _curLanguage.Value;
        }
        set
        {
            _curLanguage = value;
            eventManager.InvokeEvent(typeof(IGameSettingHandler), new GameSettingEventData(GameSettingEventData.Type.LANGUAGE_CHANGE));
        }
    }

    [System.Obsolete("no longer used for debugging", true)]
    public static DebugMgr debugMgr = null;

    [DllImport("__Internal")]
    private static extern bool IsMobile();

    public static bool IsOnMobile()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        return IsMobile();
#else
        return Application.isMobilePlatform;
#endif
    }
}