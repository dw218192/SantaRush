﻿using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class GiftType : IComparable<GiftType>, IEquatable<GiftType>, IInventoryItem
{
    public static readonly GiftType INVALID = new GiftType(-1);
    public static readonly GiftType GREEN = new GiftType(0);
    public static readonly GiftType BLUE = new GiftType(1);
    public static readonly GiftType RED = new GiftType(2);
    public static readonly GiftType PURPLE = new GiftType(3);
    public static readonly GiftType ORANGE = new GiftType(4);
    static Sprite[] _giftSprites = null;
    static AudioClip _giftSound = null;


    public int val;
    GiftType(int val)
    {
        this.val = val;
    }

    public int CompareTo(GiftType other)
    {
        return val.CompareTo(other.val);
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as GiftType);
    }
    public bool Equals(GiftType other)
    {
        if (ReferenceEquals(other, null))
            return false;
        return val == other.val;
    }

    public static bool operator==(GiftType g1, GiftType g2)
    {
        return g1.Equals(g2);
    }
    public static bool operator!=(GiftType g1, GiftType g2)
    {
        return !(g1==g2);
    }

    public override int GetHashCode()
    {
        return 1835847388 + val.GetHashCode();
    }

    [Obsolete("gift now uses different sprites", true)]
    public Color GetColor()
    {
        if (this == GREEN)
            return Color.green;
        else if (this == BLUE)
            return Color.blue;
        else if (this == RED)
            return Color.red;
        else if (this == PURPLE)
            return new Color(128 / 255f, 0, 128 / 255f);
        else if (this == ORANGE)
            return new Color(1, 165 / 255f, 0);
        else
            return Color.white;
    }

    public Sprite GetSprite()
    {
        if (_giftSprites == null)
            _giftSprites = Resources.LoadAll<Sprite>(GameConsts.k_ResourcesGiftSpritePath);
        Debug.Assert(_giftSprites != null && _giftSprites.Length > 0);
        Debug.Assert(val != GiftType.INVALID.val);
        
        return _giftSprites[val];
    }

    public AudioClip GetSound()
    {
        if (_giftSound == null)
            _giftSound = Resources.Load<AudioClip>(GameConsts.k_ResourcesGiftSoundPath);
        return _giftSound;
    }

    public int GetScore()
    {
        return val + 1;
    }

}

[Serializable]
public class GiftDesc
{
    [GiftTypeDropdown("礼物种类")]
    public GiftType type;

    [AllowNesting]
    [Label("数量")]
    public int count;
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
    PartDesc<WagonPart>[] _partDescs = null;

    [Header("游戏参数")]
    [SerializeField]
    [Range(0, 50)]
    [Label("碰撞后无敌时间")]
    float _invicibleTimeOnCollision = 3;

    [SerializeField]
    [ReorderableList]
    [Label("初始礼物")]
    GiftDesc[] _giftDescs = null;

    [Header("控制参数")]
    [Range(0, 1)]
    [Tooltip("minimum delta/screen_height percentage above which the wagon starts to accept input")]
    [SerializeField]
    [Label("拖动输入灵敏度")]
    float _tolerance = 0.1f;

    [SerializeField]
    [Label("移动按键绑定")]
    InputAction _moveUp = null;
    
    [SerializeField]
    [Label("礼物投放按键绑定")]
    InputAction _deployGift = null;

    [SerializeField]
    [Label("切换礼物按键绑定")]
    InputAction _switchGift = null;

    [SerializeField]
    [Label("雪橇拖动移动速度")]
    float _speed = 1f;

    [SerializeField]
    [Label("礼物投放冷却时间(秒)")]
    float _deplpyCooldown = 1f;

    [SerializeField]
    [Label("特效")]
    Animator _deathEffectPrefab = null;

    public float HingeDist { get => _hingeDist; }
    public float HingeAngularDrag { get => _hingeAngularDrag; }
    public float PartMass { get => _partMass; }
    public float AngleRange { get => _angleRange; }
    public PartDesc<WagonPart>[] PartDescs { get => _partDescs; }
    public float InvicibleTimeOnCollision { get => _invicibleTimeOnCollision; }
    public GiftDesc[] InitGiftDescs { get => _giftDescs; }
    public float Tolerance { get => _tolerance; }
    public InputAction MoveUp { get => _moveUp; set => _moveUp = value; }
    
    [Obsolete("deploying gift is a removed game mechanism")]
    public InputAction DeployGift { get => _deployGift; set => _deployGift = value; }
    [Obsolete("switching gift is a removed game mecahnism")]
    public InputAction SwitchGift { get => _switchGift; set => _switchGift = value; }

    public float Speed { get => _speed; }
    public float DeplpyCooldown { get => _deplpyCooldown; }
    public Animator DeathEffectPrefab { get => _deathEffectPrefab; }
}
