using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomPropertyDrawer(typeof(GiftTypeDropdownAttribute))]
public class GiftTypeDrawer : PropertyDrawer
{
    SortedDictionary<GiftType, string> _giftTypeDict = new SortedDictionary<GiftType, string>
    {
        { GiftType.GREEN , "绿色" },
        { GiftType.BLUE  , "蓝色" },
        { GiftType.RED   , "红色" },
        { GiftType.PURPLE, "紫色" },
        { GiftType.ORANGE, "橙色" },
    };

    int _idx;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GiftTypeDropdownAttribute giftTypeAttrib = attribute as GiftTypeDropdownAttribute;
        SerializedProperty prop = property.FindPropertyRelative("val");

        EditorGUI.BeginChangeCheck();
        {
            _idx = EditorGUI.Popup(position, giftTypeAttrib.label, prop.intValue, _giftTypeDict.Values.ToArray());
        }
        if(EditorGUI.EndChangeCheck())
        {
            prop.intValue = _idx;
        }
    }
}
