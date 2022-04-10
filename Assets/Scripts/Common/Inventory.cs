using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{

}

public class Inventory
{
    private Dictionary<IInventoryItem, int> _dict;
    private int _total;

    public Inventory()
    {
        _dict = new Dictionary<IInventoryItem, int>();
        _total = 0;
    }

    public void Add(IInventoryItem type, int amount)
    {
        if (amount < 0)
        {
            Remove(type, -amount);
            return;
        }

        if (!_dict.ContainsKey(type))
            _dict[type] = amount;
        else
            _dict[type] += amount;

        _total += amount;
    }

    public void Remove(IInventoryItem type, int amount)
    {
        if (amount < 0)
        {
            Add(type, -amount);
            return;
        }

        if (_dict.TryGetValue(type, out int entry))
        {
            int delta = Mathf.Min(entry, amount);
            entry -= delta;

            if (entry == 0)
                _dict.Remove(type);
            else
                _dict[type] = entry;

            _total -= delta;
        }
    }

    public void Remove(IInventoryItem type)
    {
        if (_dict.TryGetValue(type, out int entry))
        {
            _total -= entry;
            _dict.Remove(type);
        }
    }

    public int Count(IInventoryItem type)
    {
        if(_dict.ContainsKey(type))
            return _dict[type];
        return 0;
    }

    public int TotalCount()
    {
        return _total;
    }
}
