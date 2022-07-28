using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ResponsiveText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Text _text;
    [SerializeField] AudioClip _soundEffect;
    [SerializeField] Color _selectedTextColor;
    [SerializeField] Color _normalTextColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = _selectedTextColor;
        if(GameConsts.audioMgr && _soundEffect)
        {
            GameConsts.audioMgr.PlayEffect(_soundEffect);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = _normalTextColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<Text>();
        Debug.Assert(_text);

        _text.color = _normalTextColor;
    }
}
