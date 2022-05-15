using System;
using UnityEngine;

[CreateAssetMenu(menuName = "策划/字符串")]
public class LocalizedString : ScriptableObject
{
    [SerializeField]
    private string[] _values;

    private void OnEnable()
    {
        if (_values == null)
            _values = new string[0];

        int numLanguages = Enum.GetValues(typeof(Language)).Length;
        if (_values.Length < numLanguages)
        {
            string[] newValues = new string[numLanguages];
            _values.CopyTo(newValues, 0);
            for(int i = _values.Length; i<numLanguages; ++i)
            {
                newValues[i] = "unknown";
            }
            _values = newValues;
        }
    }


    public string Get()
    {
        return _values[(int)GameConsts.curLanguage];
    }
    public string Format(params object[] args)
    {
        return string.Format(Get(), args);
    }

    public static implicit operator string (LocalizedString locstr)
    {
        return locstr.Get();
    }
}
