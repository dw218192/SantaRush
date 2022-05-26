using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class EventResponsiveText : MonoBehaviour
{
    [SerializeField] Color _eventColor;
    [SerializeField] Color _normalColor;

    Text _text;

    public void OnEnterEvent(BaseEventData data)
    {
        if (!_text)
            _text = GetComponent<Text>();
        _text.color = _eventColor;
    }
    public void OnExitEvent(BaseEventData data)
    {
        if (!_text)
            _text = GetComponent<Text>();
        _text.color = _normalColor;
    }
}
