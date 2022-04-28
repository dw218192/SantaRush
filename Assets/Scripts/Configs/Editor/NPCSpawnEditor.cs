// #define OldPreviewImpl

#if OldPreviewImpl
#undef NOT_OldPreviewImpl
#else
#define NOT_OldPreviewImpl
#endif

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

using Debug = UnityEngine.Debug;

[CustomEditor(typeof(NPCSpawnConfig))]
public class NPCSpawnEditor : Editor
{
    enum SupportedAspects
    {
        Aspect4by3 = 1,
        Aspect5by4 = 2,
        Aspect16by10 = 3,
        Aspect16by9 = 4
    };

    Texture _tex;

    PreviewRenderUtility _previewUtil;

    Scene _scene;
    // stuff inside the preview scene
    Camera _cam = null;

    // preview variables
    SupportedAspects _aspectChoiceIdx = SupportedAspects.Aspect16by10;
    float _curAspect;
    // world space (same as orthographicSize)
    float _worldScreenHeight = 5;
    float[] _spawnYs = new float[0];

    GameObject _backgroundObj = null;
    GameObject[] _spawnRef = null;
    int _renderTextureHeight = 1080;
    bool _useBackgroundPrefab = false;

    // editing
    List<(string, string)> _globalSpawnSettingProperties = new List<(string, string)>();
    
    // per-spawn point properties (must be arrays)
    SerializedProperty _perSpawnProperty;

    float ToFloat(SupportedAspects aspects)
    {
        switch(aspects)
        {
            case SupportedAspects.Aspect16by10:
                return 16 / 10f;
            case SupportedAspects.Aspect16by9:
                return 16 / 9f;
            case SupportedAspects.Aspect4by3:
                return 4 / 3f;
            case SupportedAspects.Aspect5by4:
                return 5 / 4f;
            default:
                throw new ArgumentException();
        }
    }

    #region PrevieDrawingUtils
    [Conditional("OldPreviewImpl")]
    void __DrawRefScene_OldPreviewImpl()
    {
        var save = RenderTexture.active;
        RenderTexture rt = new RenderTexture(Mathf.RoundToInt(_curAspect * _renderTextureHeight), _renderTextureHeight, 16);
        RenderTexture.active = rt;
        _cam.targetTexture = rt;

        _cam.Render();
        Texture2D tex2D;
        _tex = tex2D = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex2D.Apply(false);
        Graphics.CopyTexture(rt, _tex);

        RenderTexture.active = save;
    }

    [Conditional("NOT_OldPreviewImpl")]
    void __DrawRefScene_Impl(Rect r)
    {
        _previewUtil.BeginPreview(r, GUIStyle.none);
        _previewUtil.Render();
        _tex = _previewUtil.EndPreview();
    }

    [Conditional("OldPreviewImpl")]
    void __InstantiatePreviewGO_OldPreviewImpl(ref GameObject ret, Vector3 pos, string prefabPath = null)
    {
        if (prefabPath == null)
        {
            ret = new GameObject();
        }
        else
        {
            var prefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            ret = Instantiate(prefabObj);
        }
        ret.transform.position = pos;
        EditorSceneManager.MoveGameObjectToScene(ret, _scene);
    }

    [Conditional("NOT_OldPreviewImpl")]
    void __InstantiatePreviewGO_Impl(ref GameObject ret, Vector3 pos, string prefabPath = null)
    {
        if (prefabPath == null)
        {
            ret = new GameObject();
            _previewUtil.AddSingleGO(ret);
        }
        else
        {
            ret = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            ret = _previewUtil.InstantiatePrefabInScene(ret);
        }
        ret.transform.position = pos;
    }

    GameObject InstantiatePreviewGO(Vector3 pos, string prefabPath = null)
    {
        GameObject ret = null;
        __InstantiatePreviewGO_OldPreviewImpl(ref ret, pos, prefabPath);
        __InstantiatePreviewGO_Impl(ref ret, pos, prefabPath);
        return ret;
    }

    float WorldDistToGUIDist(float previewGUIWidth, float dist)
    {
        float camSizeWorldX = _worldScreenHeight * _curAspect;
        float scaleFactor = previewGUIWidth / camSizeWorldX;
        return scaleFactor * dist;
    }
    #endregion

#region Init
    [Conditional("OldPreviewImpl")]
    void __OnEnable_OldPreviewImpl()
    {
        _aspectChoiceIdx = SupportedAspects.Aspect16by10;

        _scene = EditorSceneManager.NewPreviewScene();

        _backgroundObj = PrefabUtility.LoadPrefabContents(EditorResources.kDemoBkgPrefab);
        EditorSceneManager.MoveGameObjectToScene(_backgroundObj, _scene);
        _backgroundObj.SetActive(_useBackgroundPrefab);

        _cam = InstantiatePreviewGO(Vector3.zero).AddComponent<Camera>();
        _cam.orthographic = true;
        _cam.cameraType = CameraType.Preview;
        _cam.scene = _scene;
    }

    [Conditional("NOT_OldPreviewImpl")]
    void __OnEnable_Impl()
    {
        _aspectChoiceIdx = SupportedAspects.Aspect16by10;

        _previewUtil = new PreviewRenderUtility();
        _cam = _previewUtil.camera;
        _cam.orthographic = true;

        _backgroundObj = InstantiatePreviewGO(Vector3.zero, EditorResources.kDemoBkgPrefab);
        _backgroundObj.SetActive(_useBackgroundPrefab);
    }

    void OnEnable()
    {
        __OnEnable_OldPreviewImpl();
        __OnEnable_Impl();

        _globalSpawnSettingProperties.Add(("_bottomOffset", "下部偏移量"));
        _globalSpawnSettingProperties.Add(("_topOffset", "上部偏移量"));
        _globalSpawnSettingProperties.Add(("_ySpawnPointDist", "Y间隔"));
        _globalSpawnSettingProperties.Add(("_npcHeight", ""));
        _perSpawnProperty = serializedObject.FindProperty("_npcSpawnInterval");

        OnCamSettingChange();
    }

    [Conditional("OldPreviewImpl")]
    void __OnDisable_OldPreviewImpl()
    {
        if (!EditorSceneManager.ClosePreviewScene(_scene))
            Debug.LogWarning("failed to close preview scene");
    }

    [Conditional("NOT_OldPreviewImpl")]
    void __OnDisable_Impl()
    {
        _previewUtil.Cleanup();
    }

    void OnDisable()
    {
        __OnDisable_OldPreviewImpl();
        __OnDisable_Impl();
    }

#endregion

    void OnCamSettingChange()
    {
        _curAspect = ToFloat(_aspectChoiceIdx);
        _cam.aspect = _curAspect;
        _cam.orthographicSize = _worldScreenHeight;

        OnSpawnSettingChange();
    }

    void OnSpawnSettingChange()
    {
        serializedObject.UpdateIfRequiredOrScript();

        Vector2 viewMin = _cam.transform.TransformPoint(
            new Vector2(-_cam.orthographicSize * _cam.aspect, -_cam.orthographicSize));
        Vector2 viewMax = _cam.transform.TransformPoint(
            new Vector2(_cam.orthographicSize * _cam.aspect, _cam.orthographicSize));

        // calculate spawn points
        NPCSpawnConfig configIns = serializedObject.targetObject as NPCSpawnConfig;

        _spawnYs = NPCSpawner.GenSpawnPointYs(
            viewMin.y,
            viewMax.y,
            configIns);

        // resize per-spawnpoint properties
        _perSpawnProperty.arraySize = _spawnYs.Length;

        // gen ref objects
        float npcHeight = configIns.NpcHeight;
        bool instantiateFlag = false;
        if (_spawnRef == null || _spawnYs.Length != _spawnRef.Length)
        {
            instantiateFlag = true;
            if(_spawnRef != null)
                foreach (var go in _spawnRef)
                    DestroyImmediate(go);
            _spawnRef = new GameObject[_spawnYs.Length];
        }

        for (int i = 0; i < _spawnYs.Length; ++i)
        {
            Vector3 pos = new Vector3(viewMax.x - 1f, _spawnYs[i], 10f);
            if (instantiateFlag)
                _spawnRef[i] = InstantiatePreviewGO(pos, EditorResources.kPlaceHolderSpritePrefab);
            else
                _spawnRef[i].transform.position = pos;

            // make spawn point reference same height as NPC
            float height = _spawnRef[i].GetComponentInChildren<SpriteRenderer>().bounds.size.y;
            float scaleFactor = npcHeight / height;
            _spawnRef[i].transform.localScale.Set(1, scaleFactor, 1);
        }

        // now transform spawn Ys to GUI space

        // transform coordinate system to be relative to bottom of the cam frame        
        for (int i = 0; i < _spawnYs.Length; ++i)
            _spawnYs[i] -= viewMin.y;

        float viewHeight = _cam.orthographicSize * 2;
        for (int i = 0; i < _spawnYs.Length; ++i)
            _spawnYs[i] /= viewHeight;

        foreach (var y in _spawnYs)
            Debug.Assert(y >= 0 && y < 1);

        serializedObject.ApplyModifiedProperties();
    }

    // GUI states
    class GUIControlStates
    {
        public bool foldout = false;
        public Vector3 camPos;
    };
    GUIControlStates _guiStates = new GUIControlStates();
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // serialized object edits
        using (var scope = new EditorGUI.ChangeCheckScope())
        {
            foreach(var prop in _globalSpawnSettingProperties)
            {
                if (prop.Item2 == null || prop.Item2.Length == 0)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.Item1));
                else
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.Item1), new GUIContent(prop.Item2));
            }

            if (scope.changed)
            {
                OnSpawnSettingChange();
                __DrawRefScene_OldPreviewImpl();
            }
        }

        // display options
        using (var scope = new EditorGUI.ChangeCheckScope())
        {
            _aspectChoiceIdx = (SupportedAspects)EditorGUILayout.EnumPopup("显示器宽高比", (Enum)_aspectChoiceIdx);
            if (scope.changed)
            {
                OnCamSettingChange();
                __DrawRefScene_OldPreviewImpl();
            }
        }

        _guiStates.foldout = EditorGUILayout.Foldout(_guiStates.foldout, "高级显示设置", true);
        if(_guiStates.foldout)
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                _worldScreenHeight = EditorGUILayout.FloatField("摄像机世界空间Y长度", _worldScreenHeight);
                _renderTextureHeight = EditorGUILayout.IntField("参考视图大小", _renderTextureHeight);

                if (scope.changed)
                {
                    OnCamSettingChange();
                    __DrawRefScene_OldPreviewImpl();
                }
            }

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                _guiStates.camPos = EditorGUILayout.Vector3Field("摄像机位置", _guiStates.camPos);

                if (scope.changed)
                {
                    _cam.transform.position = _guiStates.camPos;
                    __DrawRefScene_OldPreviewImpl();
                }
            }

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                _useBackgroundPrefab = EditorGUILayout.Toggle("开关背景", _useBackgroundPrefab);

                if (scope.changed)
                    _backgroundObj.SetActive(_useBackgroundPrefab);
            }
        }

        float previewWidth = EditorGUIUtility.currentViewWidth * 0.8f;
        float spawnEditRegionWidth = EditorGUIUtility.currentViewWidth * 0.2f;

        float previewHeight = WorldDistToGUIDist(previewWidth, _worldScreenHeight);
        Rect r = EditorGUILayout.GetControlRect(false,
            GUILayout.Height(previewHeight),
            GUILayout.Width(previewWidth),
            GUILayout.ExpandHeight(false),
            GUILayout.ExpandWidth(false));

        // left preview
        __DrawRefScene_OldPreviewImpl();
        __DrawRefScene_Impl(r);
        EditorGUI.DrawPreviewTexture(r, _tex);

        // right editing fields
        r.x += previewWidth + 0.2f;
        r.y += previewHeight;
        r.height = EditorGUIUtility.singleLineHeight;
        r.width = spawnEditRegionWidth * 0.5f;

        float base_y = r.y;
        // Debug.Assert(_spawnYs.Length == _perSpawnProperty.arraySize);
        for(int i=0; i<_spawnYs.Length; ++i)
        {
            r.y = base_y - previewHeight * _spawnYs[i] - EditorGUIUtility.singleLineHeight; // WorldDistToGUIDist(previewWidth, worldY);
            EditorGUI.LabelField(r, "NPC出生间隔");

            r.y += EditorGUIUtility.singleLineHeight;
            SerializedProperty elem = _perSpawnProperty.GetArrayElementAtIndex(i);
            elem.floatValue = EditorGUI.FloatField(r, elem.floatValue);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
