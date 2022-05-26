using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct StringTextPair
{
    public LocalizedString str;
    public Text text;
    // if not null, will use this font instead of GameConsts.GetFont()
    public Font fontOverride;

    private string[] _appendStrings;

    public void Init()
    {
        if(fontOverride != null)
            text.font = fontOverride;
        else
            text.font = GameConsts.GetGameFont();

        Set();
    }

    public void Set(params string[] appendStrings)
    {
        _appendStrings = appendStrings;

        text.text = str;
        foreach (var s in appendStrings)
            text.text += s;
    }
    public void Refresh()
    {
        text.text = str;
        if (_appendStrings != null)
        {
            foreach (var s in _appendStrings)
                text.text += s;
        }
    }
    public void Clear()
    {
        text.text = "";
    }
}
