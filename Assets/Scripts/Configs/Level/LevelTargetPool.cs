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
        float _timeLimit = 10;

        [SerializeField]
        [AllowNesting]
        [Label("阶段目标(礼物数)")]
        int _targetScore = 10;

        [SerializeField]
        [AllowNesting]
        [Label("得分之间时间限制(秒)")]
        float _timeBetweenScores = 3;

        [SerializeField]
        [AllowNesting]
        [Label("阶段倍数")]
        int _scoreMultiplier = 1;

        public float TimeLimit { get => _timeLimit; }
        public int TargetScore { get => _targetScore; }
        public float TimeBetweenScores { get => _timeBetweenScores; }
        public int ScoreMultiplier { get => _scoreMultiplier; }

        public int GetModifiedScoreDelta(int curScore)
        {
            return curScore * _scoreMultiplier;
        }
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

        [SerializeField]
        [AllowNesting]
        [Label("头槌使用时间")]
        float _superStatusTime;

        [SerializeField]
        [AllowNesting]
        [Label("头槌倍数")]
        float _superStatusScoreMultiplier;

        [SerializeField]
        [AllowNesting]
        [Label("头槌移速加成")]
        float _superStatusSpeedIncrease;

        public float BonusStartTimeLimit { get => _bonusStartTimeLimit; }
        public int BonusStartGiftNum { get => _bonusStartGiftNum; }
        public float SuperStatusSpeedIncrease { get => _superStatusSpeedIncrease; }
        public float SuperStatusScoreMultiplier { get => _superStatusScoreMultiplier; }
        public float SuperStatusTime { get => _superStatusTime; }
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
    int _bonusStageIdx = -1;
    Lottery _lottery = null;

    public LevelTargetDesc GetNextTarget()
    {
        if (_lottery == null)
            _lottery = new Lottery(_items);
        return (LevelTargetDesc)_lottery.NextItem();
    }

    public BonusStageDesc GetNextBonusStage()
    {
        _bonusStageIdx = Mathf.Min(_bonusStageIdx + 1, _bonusStages.Length - 1);
        return _bonusStages[_bonusStageIdx];
    }

    public BonusStageDesc PeekNextBonusStage()
    {
        return _bonusStages[Mathf.Min(_bonusStageIdx + 1, _bonusStages.Length - 1)];
    }

    public void ResetBonusStage()
    {
        _bonusStageIdx = -1;
    }

    public ScoreBonusConfig BonusConfig { get => _bonusConfig; }
    public BonusStageDesc[] BonusStages { get => _bonusStages; }
}