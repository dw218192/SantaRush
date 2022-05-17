using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameConsts
{
    public const float k_AspectRatio = 1.33333f;
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
    public const string k_ResourcesUIPrefabPath = "Prefabs/UI";

    public const string k_GameFontPath = "Art/Font/Muyao-Softbrush-2";

    public const string k_PlayerPrefHighestScore = "HighestScore";
    public const string k_PlayerPrefTutorialViewed = "TutorialView";

#region Resolution Related
    private static float _baseUIHeight = Screen.height;
    private static Vector2Int? _lastScreenSize = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitGame()
    {
        if (IsOnMobile())
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = Screen.autorotateToLandscapeRight = false;
        }

        _baseUIHeight = Screen.height;
        OrderedUpdate.AddUpdateReceiver(UpdateGame);
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    static void FixResolution()
    {
        float aspect = Screen.width / (float)Screen.height;
        // get rid of FUCKING portrait, fucking annoying as shit having to manually do this
        if (aspect < k_AspectRatio)
        {
            float heightRatio = Screen.width / k_AspectRatio / Screen.height;
            Debug.Assert(heightRatio > 0 && heightRatio < 1);

            Rect r = Camera.main.rect;

            float margin = (1 - heightRatio) / 2;
            // left bottom
            r.x = 0;
            r.y = margin;
            r.width = 1;
            r.height = heightRatio;

            Camera.main.rect = r;

            eventManager.InvokeEvent(typeof(IResolutionScaleHandler), new ResolutionScaleEventData(heightRatio));
        }
        // ok resolution
        else
        {
            eventManager.InvokeEvent(typeof(IResolutionScaleHandler), new ResolutionScaleEventData(Screen.height / _baseUIHeight));
        }
    }

    static void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        _lastScreenSize = null;
    }

    static void UpdateGame()
    {
        Debug.Assert(uiMgr != null && eventManager != null);

        // force normal aspect ratio
        Vector2Int curRes = new Vector2Int(Screen.width, Screen.height);

        // resolution change or first update
        if (_lastScreenSize == null || _lastScreenSize.Value != curRes)
        {
            FixResolution();
        }
        _lastScreenSize = curRes;
    }

#endregion

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

    // private static Vector2? _worldCameraMin = null;
    public static Vector2 worldCameraMin
    {
        get
        {
            /*
            if(_worldCameraMin == null)
            {
                Camera cam = Camera.main;
                float h = cam.orthographicSize, w = cam.orthographicSize * cam.aspect;
                Vector2 localMin = new Vector2(-w, -h);
                _worldCameraMin = cam.transform.TransformPoint(localMin);
            }
            return _worldCameraMin.Value;
            */
            return Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        }
    }
    // private static Vector2? _worldCameraMax = null;
    public static Vector2 worldCameraMax
    {
        get
        {
            /*
            if (_worldCameraMax == null)
            {
                Camera cam = Camera.main;
                float h = cam.orthographicSize, w = cam.orthographicSize * cam.aspect;
                Vector2 localMax = new Vector2(w, h);
                _worldCameraMax = cam.transform.TransformPoint(localMax);
            }
            return _worldCameraMax.Value;
            */
            return Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        }
    }

    public static Vector2 worldCameraCenter
    {
        get
        {
            return Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
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
            Debug.Assert(eventManager != null);

            _curLanguage = value;
            eventManager.InvokeEvent(typeof(IGameSettingHandler), new GameSettingEventData(GameSettingEventData.Type.LANGUAGE_CHANGE));
        }
    }

    [System.Obsolete("no longer used for debugging", true)]
    public static DebugMgr debugMgr = null;

#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern bool IsMobile();
#endif

    public static bool IsOnMobile()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        return IsMobile();
#else
        return Application.isMobilePlatform;
#endif
    }
}