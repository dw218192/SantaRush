using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(LocalizedString))]
public class LocalizedStringEditor : Editor
{
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty prop = serializedObject.FindProperty("_values");

        Language[] languages = (Language[])Enum.GetValues(typeof(Language));
        if (prop.arraySize != languages.Length)
            prop.arraySize = languages.Length;

        int i = 0;
        foreach (Language lan in languages)
        {
            GUILayout.Label(lan.ToPrettyString());
            prop.GetArrayElementAtIndex(i).stringValue = GUILayout.TextArea(prop.GetArrayElementAtIndex(i).stringValue, 256);
            ++i;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
