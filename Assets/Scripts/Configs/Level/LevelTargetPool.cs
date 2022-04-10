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

    [SerializeField]
    [ReorderableList]
    [Label("关卡目标表")]
    LevelTargetDesc[] _items;

    // runtime
    Lottery _lottery = null;
    public LevelTargetDesc GetNextTarget()
    {
        if (_lottery == null)
            _lottery = new Lottery(_items);
        return (LevelTargetDesc)_lottery.NextItem();
    }
}