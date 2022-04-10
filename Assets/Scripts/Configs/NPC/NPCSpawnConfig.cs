using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region EDITOR
public class NPCHeightAttribute : PropertyAttribute
{
    public string label;
    public NPCHeightAttribute(string label)
    {
        this.label = label;
    }
}
#endregion

[Serializable]
public struct NPCHeight
{
    public float val;
    public static implicit operator NPCHeight(float value) => new NPCHeight { val = value };
    public static implicit operator float(NPCHeight h) => h.val;
}

[CreateAssetMenu(menuName = "My Assets/NPCSpawnConfig")]
public class NPCSpawnConfig : ScriptableObject
{
    [SerializeField]
    [Range(0,100)]
    float _bottomOffset;

    [SerializeField]
    [Range(0,100)]
    float _topOffset;

    [SerializeField]
    [Range(0.5f, 100)]
    float _ySpawnPointDist;

    [SerializeField]
    [NPCHeight("NPC高度")]
    NPCHeight _npcHeight = 1f;

    [SerializeField]
    float[] _npcSpawnInterval; // how often npc is spawned for each spawn point

    public float YSpawnPointDist { get => _ySpawnPointDist; }
    public float NpcHeight { get => _npcHeight; }
    public float BottomOffset { get => _bottomOffset; }
    public float TopOffset { get => _topOffset; }
    public float[] NpcSpawnInterval { get => _npcSpawnInterval;  }
}
