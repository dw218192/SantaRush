using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "My Assets/LevelStageTable")]
public class LevelStageTable : ScriptableObject
{
    [Serializable]
    public class SpeedChangeStat
    {
        [SerializeField]
        [AllowNesting]
        [Tooltip("速度从初始到最终之间总共被分成了多少个区间")]
        [Label("区间数量")]
        int _numIntervals;

        [SerializeField]
        [AllowNesting]
        [Label("每个区间时长(秒)")]
        float _intervalLength;

        [SerializeField]
        [AllowNesting]
        [Tooltip("是距离游戏开始的秒数，经过这么多时间后会开始第一个加速区间")]
        [Label("速度变化开始时间(秒)")]
        float _intervalStartTime;

        [SerializeField]
        [AllowNesting]
        [Tooltip("初始的速度倍率，例子：1.2的意思就是1.2倍初始速度")]
        [Label("初始速度")]
        float _initialMultiplier;

        [SerializeField]
        [AllowNesting]
        [Tooltip("最终的速度倍率，例子：1.2的意思就是1.2倍初始速度")]
        [Label("最终速度")]
        float _finalMultiplier;

        public int numIntervals { get => _numIntervals; }
        public float intervalLength { get => _intervalLength; }
        public float intervalStartTime { get => _intervalStartTime; }
        public float initialMultiplier { get => _initialMultiplier; }
        public float finalMultiplier { get => _finalMultiplier; }
    };

    [SerializeField] 
    [Label("雪橇速度变化表")]
    SpeedChangeStat _speedChangeStat;

    [SerializeField]
    [Label("绝对初始速度")]
    float _initScrollSpeed;

    public SpeedChangeStat SpeedChangeConfig { get => _speedChangeStat; }
    public float InitScrollSpeed { get => _initScrollSpeed; }
}