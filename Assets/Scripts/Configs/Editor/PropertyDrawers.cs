using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;

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

[CustomPropertyDrawer(typeof(NPCHeightAttribute))]
public class NPCHeightDrawer : PropertyDrawer
{
    readonly float kFieldHeight = EditorGUIUtility.singleLineHeight;
    const float kFieldPadding = 1;
    const int numFields = 3;

    SpriteRenderer _rend;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return numFields * kFieldHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect r = position;
        r.height = kFieldHeight;

        NPCHeightAttribute npcHeightAttrib = attribute as NPCHeightAttribute;
        SerializedProperty prop = property.FindPropertyRelative("val");

        EditorGUI.LabelField(r, npcHeightAttrib.label);
        ++ EditorGUI.indentLevel;
        
        {
            r.y += kFieldHeight + kFieldPadding;
            EditorGUI.BeginChangeCheck();
            {
                _rend = (SpriteRenderer)EditorGUI.ObjectField(r, "拖入物体计算高度", _rend, typeof(SpriteRenderer), false);
            }
            if (EditorGUI.EndChangeCheck())
            {
                prop.floatValue = _rend.bounds.size.y;
            }
            
            r.y += kFieldHeight + kFieldPadding;
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUI.FloatField(r, "当前高度", prop.floatValue);
            }
            EditorGUI.EndDisabledGroup();
        }
        
        --EditorGUI.indentLevel;

        property.serializedObject.ApplyModifiedProperties();
    }
}