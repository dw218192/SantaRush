using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[CustomPropertyDrawer(typeof(ArrayElement<>))]
public class ArrayElementDrawer : PropertyDrawer
{
    bool _init = false;
    bool _hasArrayFields = false;

    int _arraySelection = 0;
    string[] _arrayFieldSelectionNames = null;
    List<string> _arrayFields = new List<string>();
    
    SerializedProperty _curArray = null;
    string[] _curArraySelectionNames = null;
    int _curArraySelection = 0;
    

    // my prop
    SerializedProperty _elementProp;

    void SetupArraySelection(SerializedProperty property)
    {
        _curArraySelection = 0;
        _curArray = property.serializedObject.FindProperty(_arrayFields[_arraySelection]);
        _curArraySelectionNames = new string[_curArray.arraySize];
        for (int i = 0; i < _curArraySelectionNames.Length; ++i)
            _curArraySelectionNames[i] = i.ToString();
    }

    void CheckInit(SerializedProperty property)
    {
        if (!_init)
        {
            _elementProp = property.FindPropertyRelative("_element");

            var propIt = property.serializedObject.GetIterator();

            List<string> displayNames = new List<string>();
            while (propIt.Next(true))
            {
                if (propIt.isArray && propIt.arrayElementType == _elementProp.type)
                {
                    _arrayFields.Add(propIt.propertyPath);
                    displayNames.Add(propIt.displayName);
                }
            }

            _hasArrayFields = _arrayFields.Count > 0;
            _arrayFieldSelectionNames = displayNames.ToArray();
            SetupArraySelection(property);

            _init = true;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CheckInit(property);
        
        position = EditorGUI.PrefixLabel(position, label);
        float dw = position.width / 2;
        if (_hasArrayFields)
        {
            position.width = dw;

            _arraySelection = EditorGUI.Popup(position, _arraySelection, _arrayFieldSelectionNames);
            SetupArraySelection(property);

            position.x += dw;

            if (_curArray.arraySize == 0)
            {
                EditorGUI.LabelField(position, "no element");
            }
            else
            {
                _curArraySelection = EditorGUI.Popup(position, _curArraySelection, _curArraySelectionNames);
                SerializedProperty targetProp = _curArray.GetArrayElementAtIndex(_curArraySelection);

                Type valueType = Type.GetType(_elementProp.type + ", Assembly-CSharp");
                _elementProp.SetValue(valueType, targetProp.Value(valueType));
            }
        }
        else
        {
            EditorGUI.HelpBox(position, "No Array found on this object but you're using an array element field", MessageType.Warning);
        }
    }
}
