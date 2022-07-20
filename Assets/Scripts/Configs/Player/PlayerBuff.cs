using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerBuffType
{
    HP_REWARD,
    SUPER_STATUS
}

public class PlayerBuffDesc
{
    public PlayerBuffDesc(PlayerBuffType type, string name, float duration, AudioClip audioClip = null)
    {
        Type = type;
        Name = name;
        Duration = duration;
        AudioClip = audioClip;
    }

    public PlayerBuffType Type { get; private set; }
    public string Name { get; private set; }
    public float Duration { get; private set; }
    public AudioClip AudioClip { get; private set; }
}