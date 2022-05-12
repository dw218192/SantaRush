using System;
using UnityEngine.UI;

[Serializable]
public struct StringTextPair
{
    public LocalizedString str;
    public Text text;
    private string[] _appendStrings;

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
