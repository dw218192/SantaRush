using System.Collections;
using System.Collections.Generic;
using System;

public interface ILotteryItem
{
    int GetWeight();
}

public class Lottery
{
    // array of pairs in the format of ( weight prefix sum, index )
    List<(int, ILotteryItem)> _prefix = new List<(int, ILotteryItem)>(); 
    Random _rand = new Random();

    public Lottery(IEnumerable<ILotteryItem> items)
    {
        foreach(ILotteryItem item in items)
        {
            if (item.GetWeight() == 0)
                continue;
            
            if (_prefix.Count == 0)
                _prefix.Add((item.GetWeight(), item));
            else
                _prefix.Add((_prefix[_prefix.Count - 1].Item1 + item.GetWeight(), item));
        }
    }

    public ILotteryItem NextItem()
    {
        if (_prefix.Count == 0)
            return null;

        int minVal = _prefix[0].Item1, maxVal = _prefix[_prefix.Count - 1].Item1;
        int randVal = _rand.Next(minVal, maxVal);

        int l = 0, r = _prefix.Count;
        while (l < r)
        {
            int mid = l + (r - l) / 2;
            if (_prefix[mid].Item1 < randVal)
                l = mid + 1;
            else
                r = mid;
        }
        return _prefix[l].Item2;
    }
}
