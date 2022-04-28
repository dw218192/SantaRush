using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using System.Reflection;
using NaughtyAttributes.Editor;

[CustomEditor(typeof(NPCType))]
public class NPCTypeEditor : NaughtyInspector
{
    PreviewRenderUtility _renderUtility;
    NPCInstance _npcIns;
    MethodInfo _npcSetupMethod;
    GameObject _giftSpawnRefGO;

    public override bool HasPreviewGUI()
    {
        return true;
    }
    
    void UpdateNPCIns()
    {
        if(_npcIns != null)
            DestroyImmediate(_npcIns.gameObject);

        serializedObject.Update();
        NPCType type = serializedObject.targetObject as NPCType;

        _npcIns = NPCInstance.Create(type, Vector2.zero);
        _renderUtility.AddSingleGO(_npcIns.gameObject);
        
        _npcIns.NpcType = type as NPCType;
        _npcSetupMethod.Invoke(_npcIns, new object[] { });

        _giftSpawnRefGO.transform.position = _npcIns.GiftSpawnPos;
        var pos = _npcIns.transform.TransformPoint(new Vector2(_npcIns.TotalWidth / 2, 0));
        pos.z = -2;
        _renderUtility.camera.transform.position = pos;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);

        _renderUtility.BeginPreview(r, background);
        if (EditorUtility.IsDirty(target))
            UpdateNPCIns();
        _renderUtility.Render();
        GUI.DrawTexture(r, _renderUtility.EndPreview());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _renderUtility = new PreviewRenderUtility();
        _renderUtility.camera.transform.position = new Vector3(0, 0, -2);
        _renderUtility.camera.orthographic = true;
        _renderUtility.camera.orthographicSize = 2;

        var giftSpawnRefPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorResources.kPlaceHolderSpritePrefab);
        _giftSpawnRefGO = _renderUtility.InstantiatePrefabInScene(giftSpawnRefPrefab);

        var quat = _giftSpawnRefGO.transform.rotation;
        quat.eulerAngles = new Vector3(0, 0, 90);
        _giftSpawnRefGO.transform.rotation = quat;
        
        _npcSetupMethod = typeof(NPCInstance).GetMethod("SetupParts", BindingFlags.Instance | BindingFlags.NonPublic);

        UpdateNPCIns();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _renderUtility.Cleanup();
    }
}
