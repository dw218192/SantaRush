using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "My Assets/TilePool")]
public class TilePool : ScriptableObject
{
    [Serializable]
    public class TileDesc : ILotteryItem
    {
        [SerializeField]
        [AllowNesting]
        [Label("地图块种类")]
        GameObject _prefab;

        [SerializeField]
        [AllowNesting]
        [Tooltip("该数字越大概率越大, 例子: 1,7,100, 权重为100的物件更容易被抽到")]
        [Label("权重 (必须为非负数)")]
        int _weight;

        public GameObject prefab { get => _prefab; }

        public int GetWeight()
        {
            return _weight;
        }
    };

    [SerializeField]
    [ReorderableList]
    [Label("地图块表")]
    TileDesc[] _items;

    public TileDesc[] Items { get => _items; }

    // runtime
    Lottery _lottery = null;
    public TileDesc GetNextTile()
    {
        if (_lottery == null)
            _lottery = new Lottery(_items);
        return (TileDesc)_lottery.NextItem();
    }
}
