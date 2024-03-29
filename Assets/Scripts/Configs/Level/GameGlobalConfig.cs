﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(menuName = "My Assets/GameGlobalConfig")]
public class GameGlobalConfig : ScriptableObject
{
    [SerializeField]
    [OnValueChanged("SetPlayerVersion")]
    [Label("游戏版本")]
    string _gameVersion = "unknown";

    [SerializeField]
    [OnValueChanged("SetDefineSymbols")]
    [Label("调试模式")]
    bool _defineDebugSymbol = false;

    [Header("音乐")]
    [SerializeField]
    [Label("游戏音乐")]
    AudioClip _gameBGM;

    [SerializeField]
    [Label("菜单音乐")]
    AudioClip _menuBGM;

    public string GameVersion 
    {
        get => _gameVersion;
    }
    public AudioClip GameBGM { get => _gameBGM; }
    public AudioClip MenuBGM { get => _menuBGM; }

    private void OnEnable()
    {
#if UNITY_EDITOR
        _gameVersion = PlayerSettings.bundleVersion;
#endif
    }

    void SetPlayerVersion()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;
        PlayerSettings.bundleVersion = GameVersion;
#endif
    }

    void SetDefineSymbols()
    {
#if UNITY_EDITOR
        string defines = "";
        if (_defineDebugSymbol)
            defines = "DEBUG";

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), defines);
        AssetDatabase.Refresh();
#endif
    }
}
