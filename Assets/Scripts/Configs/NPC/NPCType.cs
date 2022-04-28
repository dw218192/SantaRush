using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

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
    [ReorderableList]
    [Label("NPC雪橇部件")]
    [ValidateInput("IsNotEmpty", "不能为空")]
    PartDesc<NPCPart>[] _npcParts;

    [SerializeField]
    [Label("NPC部件间距")]
    [ValidateInput("IsGreaterThanZero", "必须大于0")]
    float _npcPartDist;

    [SerializeField]
    [Label("礼物投放位置(百分比)")]
    [Range(0,1)]
    float _npcGiftSpawnLerpPercentage;

    public float BombProbability { get => _bombProbability; }
    public float Speed { get => _speed; }
    public float GiftSpawnCooldown { get => _giftSpawnCooldown; }
    public GiftType GiftType { get => _giftType; }
    public int NpcWeight { get => _npcWeight; }

    public PartDesc<NPCPart>[] NpcParts { get => _npcParts; }
    
    public float NpcPartDist { get => _npcPartDist; }
    public float NpcGiftSpawnLerpPercentage { get => _npcGiftSpawnLerpPercentage; }

    public int GetWeight()
    {
        return _npcWeight;
    }

    #region EditorValidatorMethods
    private bool IsNotEmpty(PartDesc<NPCPart>[] parts)
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
