using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ResponsiveText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Text _text;

    [SerializeField] Color _selectedTextColor;
    [SerializeField] Color _normalTextColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = _selectedTextColor;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
