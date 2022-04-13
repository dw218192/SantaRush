using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;


[CreateAssetMenu(menuName = "My Assets/LevelTargetPool")]
public class LevelTargetPool : ScriptableObject
{
    [Serializable]
    public class LevelTargetDesc : ILotteryItem
    {
        [SerializeField]
        [AllowNesting]
        [Label("目标礼物数量")]
        int _giftNum;

        [SerializeField] 
        [AllowNesting]
        [Label("目标时间限制(秒)")]
        float _duration;
        
        [SerializeField] 
        [AllowNesting]
        [Tooltip("该数字越大概率越大, 例子: 1,7,100, 权重为100的物件更容易被抽到")]
        [Label("权重 (必须为非负数)")]
        int _weight;

        public int giftNum { get => _giftNum; }
        public float duration { get => _duration; }

        public int GetWeight()
        {
            return _weight;
        }
    }

    [Serializable]
    public class BonusStageDesc
    {
        [SerializeField]
        [AllowNesting]
        [Label("时间限制(秒)")]
        float _timeLimit;

        [SerializeField]
        [AllowNesting]
        [Label("阶段目标")]
        int _targetScore;

        public float TimeLimit { get => _timeLimit; }
        public int TargetScore { get => _targetScore; }
    }

    [Serializable]
    public class ScoreBonusConfig
    {
        [SerializeField]
        [AllowNesting]
        [Tooltip("当玩家接中第一个礼物后,开始做判断,在如下这么多时间内连续接中礼物才能触发连续得分奖励阶段")]
        [Label("触发连续得分机制的时间限制(秒)")]
        float _bonusStartTimeLimit;

        [SerializeField]
        [AllowNesting]
        [Tooltip("在上面的时间限制内, 触发连续得分需要接住多少个礼物")]
        [Label("触发连续得分机制的礼物数量")]
        int _bonusStartGiftNum;

        public float BonusStartTimeLimit { get => _bonusStartTimeLimit; }
        public int BonusStartGiftNum { get => _bonusStartGiftNum; }
    }


    [SerializeField]
    [ReorderableList]
    [Label("关卡目标表")]
    LevelTargetDesc[] _items;

    [SerializeField]
    [ReorderableList]
    [Label("连续得分奖励阶段表")]
    BonusStageDesc[] _bonusStages;

    [SerializeField]
    [Label("触发连续得分的要求")]
    ScoreBonusConfig _bonusConfig;

    // runtime
    Lottery _lottery = null;

    public LevelTargetDesc GetNextTarget()
    {
        if (_lottery == null)
            _lottery = new Lottery(_items);
        return (LevelTargetDesc)_lottery.NextItem();
    }

    public BonusStageDesc[] BonusStages { get => _bonusStages; }
    public ScoreBonusConfig BonusConfig { get => _bonusConfig; }
}