using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

[Serializable]
public struct NPCPartDesc
{
    [AllowNesting]
    [Label("车厢种类")]
    public NPCPart prefab;

    [AllowNesting]
    [Label("数量")]
    public uint count;
}

[CreateAssetMenu(menuName = "策划/新NPC种类")]
public class NPCType : ScriptableObject, ILotteryItem
{
    [SerializeField]
    [GiftTypeDropdown("礼物种类")]
    [Required("不能为空")]
    GiftType _giftType;
    
    [SerializeField]
    [Tooltip("NPC第一个礼物落下后，第二个礼物间隔多少秒后落下")]
    [Label("礼物投放间隔(秒)")]
    [ValidateInput("IsGreaterThanZeroFloat", "必须大于0")]
    float _giftSpawnCooldown;

    [SerializeField]
    [ValidateInput("IsGreaterThanZeroFloat", "必须大于0")]
    float _speed;

    [SerializeField]
    [Range(0,1)]
    [Label("炸弹概率(百分比)")]
    float _bombProbability;

    [SerializeField]
    [Label("NPC出现权重")]
    [ValidateInput("IsGreaterThanZeroInt", "必须大于0")]
    int _npcWeight;

    [SerializeField]
    [Label("NPC雪橇部件")]
    [ReorderableList]
    [ValidateInput("IsNotEmpty", "不能为空")]
    NPCPartDesc[] _npcParts;

    [SerializeField]
    [Label("NPC部件间距")]
    [ValidateInput("IsGreaterThanZero", "必须大于0")]
    float _npcPartDist;

    public float BombProbability { get => _bombProbability; }
    public float Speed { get => _speed; }
    public float GiftSpawnCooldown { get => _giftSpawnCooldown; }
    public GiftType GiftType { get => _giftType; }
    public int NpcWeight { get => _npcWeight; }
    public NPCPartDesc[] NpcParts { get => _npcParts; }
    public float NpcPartDist { get => _npcPartDist; }

    public int GetWeight()
    {
        return _npcWeight;
    }

    #region EditorValidatorMethods
    private bool IsNotEmpty(NPCPartDesc[] parts)
    {
        return parts != null && parts.Length > 0;
    }

    private bool IsGreaterThanZeroFloat(float value)
    {
        return value > 0;
    }
    
    private bool IsGreaterThanZeroInt(int value)
    {
        return value > 0;
    }
    #endregion
}
