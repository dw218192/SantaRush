using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIObject : MonoBehaviour
{
    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        Text[] texts = GetComponentsInChildren<Text>();
        foreach (var text in texts)
            text.font = GameConsts.GetGameFont();
    }
}
