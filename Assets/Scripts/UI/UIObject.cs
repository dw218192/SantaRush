using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public abstract class UIObject : MonoBehaviour, IGameSettingHandler
{
    StringTextPair[] _localizedTexts;

    protected virtual void Awake()
    {
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        List<StringTextPair> pairs = new List<StringTextPair>();
        foreach (var field in fields)
            if (field.FieldType == typeof(StringTextPair))
                pairs.Add((StringTextPair)field.GetValue(this));
        _localizedTexts = pairs.ToArray();

        foreach (var txt in _localizedTexts)
        {
            txt.Set();
        }
    }

    protected virtual void Start()
    {
        Text[] texts = GetComponentsInChildren<Text>();
        foreach (var text in texts)
            text.font = GameConsts.GetGameFont();
    }

    public void OnGameSettingChange(GameSettingEventData eventData)
    {
        if(eventData.type == GameSettingEventData.Type.LANGUAGE_CHANGE)
        {
            if (_localizedTexts != null && _localizedTexts.Length > 0)
            {
                foreach(var txt in _localizedTexts)
                {
                    txt.Refresh();
                }
            }
        }
    }
}
