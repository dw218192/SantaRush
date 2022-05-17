﻿using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public abstract class UIObject : MonoBehaviour, IGameSettingHandler, IResolutionScaleHandler
{
    CanvasScaler _scaler = null;
    StringTextPair[] _localizedTexts = null;

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



        _scaler = GetComponent<CanvasScaler>();
        if (_scaler == null)
            _scaler = gameObject.AddComponent<CanvasScaler>();
        _scaler.referencePixelsPerUnit = 100;
        _scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
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

    public void OnResolutionScale(ResolutionScaleEventData data)
    {
        _scaler.scaleFactor = data.scaleFactor;
    }
}
