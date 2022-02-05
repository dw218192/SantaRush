﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


[Serializable]
public struct WagonPartDesc
{
    [AllowNesting]
    [Label("车厢种类")]
    public WagonPart prefab;

    [AllowNesting]
    [Label("数量")]
    public uint count;
}


public class GiftTypeDropdownAttribute : PropertyAttribute
{
    public string label;
    public GiftTypeDropdownAttribute(string label)
    {
        this.label = label;
    }
}

[Serializable]
public class GiftType : IComparable<GiftType>
{
    public static readonly GiftType INVALID = new GiftType(-1);
    public static readonly GiftType GREEN = new GiftType(0);
    public static readonly GiftType BLUE = new GiftType(1);
    public static readonly GiftType RED = new GiftType(2);
    public static readonly GiftType PURPLE = new GiftType(3);
    public static readonly GiftType ORANGE = new GiftType(4);

    public int val;
    GiftType(int val)
    {
        this.val = val;
    }

    public int CompareTo(GiftType other)
    {
        return val.CompareTo(other.val);
    }
}

[Serializable]
public class GiftDesc
{
    [GiftTypeDropdown("礼物种类")]
    public GiftType type;

    [AllowNesting]
    [Label("数量")]
    public uint count;
}

[CreateAssetMenu(menuName = "My Assets/Wagon Config")]
public class WagonConfig : ScriptableObject
{
    [Header("物理参数")]
    [SerializeField]
    [Range(0.2f, 10)]
    [Label("车厢间距")]
    float _hingeDist = 1f;
    
    [SerializeField]
    [Range(0, 500)]
    [Label("摆动阻力")]
    float _hingeAngularDrag = 5f;
    
    [SerializeField]
    [Range(1, 100)]
    [Label("车厢质量")]
    float _partMass = 1;
    

    [SerializeField]
    [Range(0, 90)]
    [Label("摆动角度限制 (°)")]
    float _angleRange = 10; // plus minus _angleRange degrees
    
    [SerializeField]
    [ReorderableList]
    [Label("车厢组件")]
    WagonPartDesc[] _partDescs = null;

    [Header("游戏参数")]
    [SerializeField]
    [Range(0, 50)]
    [Label("碰撞后无敌时间")]
    float _invicibleTimeOnCollision = 3;

    [SerializeField]
    [ReorderableList]
    [Label("初始礼物")]
    GiftDesc[] _giftDescs = null;

    public float HingeDist { get => _hingeDist; }
    public float HingeAngularDrag { get => _hingeAngularDrag; }
    public float PartMass { get => _partMass; }
    public float AngleRange { get => _angleRange; }
    public WagonPartDesc[] PartDescs { get => _partDescs; }
    public float InvicibleTimeOnCollision { get => _invicibleTimeOnCollision; }
}
