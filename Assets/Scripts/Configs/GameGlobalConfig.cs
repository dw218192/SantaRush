using System.Collections;
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

    public string GameVersion 
    {
        get => _gameVersion;
    }

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
}
