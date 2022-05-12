/// @creator: Slipp Douglas Thompson
/// @license: Public Domain per The Unlicense.  See <http://unlicense.org/>.
/// @purpose: Genericized Unity3D SerializedProperty value access.
/// @why: Because this functionality should be built-into Unity.
/// @usage: Use as you would a native SerializedProperty method;
/// 	e.g. `Debug.Log(mySerializedProperty.Value<Color>());`
/// @intended project path: Assets/Plugins/Editor/UnityEditor Extensions/SerializedPropertyValueExtension.cs
/// @interwebsouce: https://gist.github.com/capnslipp/8516384

using System;
using System.Reflection;
using System.Collections.Generic; // for: KeyValuePair<,>
using UnityEngine;
using UnityEditor;



public static class SerializedPropertyValueExtension
{
    static PropertyInfo GradientProperty;
    static MethodInfo LookupInstanceByIdInternalMethod, GetManagedReferenceIdInternalMethod;

    static SerializedPropertyValueExtension()
    {
        BindingFlags instanceAnyPrivacyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        GradientProperty = typeof(SerializedProperty).GetProperty(
            "gradientValue",
            instanceAnyPrivacyBindingFlags,
            null,
            typeof(Gradient),
            new Type[0],
            null
        );

        LookupInstanceByIdInternalMethod = typeof(SerializedProperty).GetMethod(
            "LookupInstanceByIdInternal",
            instanceAnyPrivacyBindingFlags
        );

        GetManagedReferenceIdInternalMethod = typeof(SerializedProperty).GetMethod(
            "GetManagedReferenceIdInternal",
            instanceAnyPrivacyBindingFlags
        );
    }

    public static void SetValue(this SerializedProperty thisSP, Type valueType, object val)
    {
        // First, do special Type checks
        if (valueType.IsEnum)
            thisSP.enumValueIndex = (int)Enum.ToObject(valueType, val);

        if (typeof(Color).IsAssignableFrom(valueType))
            thisSP.colorValue = (Color)(object)val;
        else if (typeof(LayerMask).IsAssignableFrom(valueType))
            thisSP.intValue = (LayerMask)(object)val;
        else if (typeof(Vector2).IsAssignableFrom(valueType))
            thisSP.vector2Value = (Vector2)(object)val;
        else if (typeof(Vector3).IsAssignableFrom(valueType))
            thisSP.vector3Value = (Vector3)(object)val;
        else if (typeof(Rect).IsAssignableFrom(valueType))
            thisSP.rectValue = (Rect)(object)val;
        else if (typeof(AnimationCurve).IsAssignableFrom(valueType))
            thisSP.animationCurveValue = (AnimationCurve)(object)val;
        else if (typeof(Bounds).IsAssignableFrom(valueType))
            thisSP.boundsValue = (Bounds)(object)val;
        else if (typeof(Gradient).IsAssignableFrom(valueType))
            SafeSetGradientValue(thisSP, (Gradient)(object)val);
        else if (typeof(Quaternion).IsAssignableFrom(valueType))
            thisSP.quaternionValue = (Quaternion)(object)val;

        // Next, check if derived from UnityEngine.Object base class
        if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
            thisSP.objectReferenceValue = (UnityEngine.Object)(object)val;

        // Finally, check for native type-families
        if (typeof(int).IsAssignableFrom(valueType))
            thisSP.intValue = (int)(object)val;
        else if (typeof(bool).IsAssignableFrom(valueType))
            thisSP.boolValue = (bool)(object)val;
        else if (typeof(float).IsAssignableFrom(valueType))
            thisSP.floatValue = (float)(object)val;
        else if (typeof(string).IsAssignableFrom(valueType))
            thisSP.stringValue = (string)(object)val;
        else if (typeof(char).IsAssignableFrom(valueType))
            thisSP.intValue = (char)(object)val;


        if (typeof(object).IsAssignableFrom(valueType))
            thisSP.managedReferenceValue = val;

        // And if all fails, throw an exception.
        throw new NotImplementedException("Unimplemented propertyType " + thisSP.propertyType + ".");
    }
    /// @note: switch/case derived from the decompilation of SerializedProperty's internal SetToValueOfTarget() method.
    public static object Value(this SerializedProperty thisSP, Type valueType)
    {
        // First, do special Type checks
        if (valueType.IsEnum)
            return Enum.ToObject(valueType, thisSP.enumValueIndex);

        if (typeof(Color).IsAssignableFrom(valueType))
            return (object)thisSP.colorValue;
        else if (typeof(LayerMask).IsAssignableFrom(valueType))
            return (object)thisSP.intValue;
        else if (typeof(Vector2).IsAssignableFrom(valueType))
            return (object)thisSP.vector2Value;
        else if (typeof(Vector3).IsAssignableFrom(valueType))
            return (object)thisSP.vector3Value;
        else if (typeof(Rect).IsAssignableFrom(valueType))
            return (object)thisSP.rectValue;
        else if (typeof(AnimationCurve).IsAssignableFrom(valueType))
            return (object)thisSP.animationCurveValue;
        else if (typeof(Bounds).IsAssignableFrom(valueType))
            return (object)thisSP.boundsValue;
        else if (typeof(Gradient).IsAssignableFrom(valueType))
            return (object)SafeGradientValue(thisSP);
        else if (typeof(Quaternion).IsAssignableFrom(valueType))
            return (object)thisSP.quaternionValue;

        // Next, check if derived from UnityEngine.Object base class
        if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
            return (object)thisSP.objectReferenceValue;

        // Finally, check for native type-families
        if (typeof(int).IsAssignableFrom(valueType))
            return (object)thisSP.intValue;
        else if (typeof(bool).IsAssignableFrom(valueType))
            return (object)thisSP.boolValue;
        else if (typeof(float).IsAssignableFrom(valueType))
            return (object)thisSP.floatValue;
        else if (typeof(string).IsAssignableFrom(valueType))
            return (object)thisSP.stringValue;
        else if (typeof(char).IsAssignableFrom(valueType))
            return (object)thisSP.intValue;


        if (typeof(object).IsAssignableFrom(valueType))
        {
            return thisSP.objectReferenceValue;
/*            long id = (long)GetManagedReferenceIdInternalMethod.Invoke(thisSP, new object[] { });
            return (object)LookupInstanceByIdInternalMethod.Invoke(thisSP, new object[] { id });*/
        }
        // And if all fails, throw an exception.
        throw new NotImplementedException("Unimplemented propertyType " + thisSP.propertyType + ".");
    }

    public static dynamic Value(this SerializedProperty thisSP)
    {
        switch (thisSP.propertyType)
        {
            case SerializedPropertyType.Integer:
                return thisSP.intValue;
            case SerializedPropertyType.Boolean:
                return thisSP.boolValue;
            case SerializedPropertyType.Float:
                return thisSP.floatValue;
            case SerializedPropertyType.String:
                return thisSP.stringValue;
            case SerializedPropertyType.Color:
                return thisSP.colorValue;
            case SerializedPropertyType.ObjectReference:
                return thisSP.objectReferenceValue;
            case SerializedPropertyType.LayerMask:
                return thisSP.intValue;
            case SerializedPropertyType.Enum:
                int enumI = thisSP.enumValueIndex;
                return new KeyValuePair<int, string>(enumI, thisSP.enumNames[enumI]);
            case SerializedPropertyType.Vector2:
                return thisSP.vector2Value;
            case SerializedPropertyType.Vector3:
                return thisSP.vector3Value;
            case SerializedPropertyType.Rect:
                return thisSP.rectValue;
            case SerializedPropertyType.ArraySize:
                return thisSP.intValue;
            case SerializedPropertyType.Character:
                return (char)thisSP.intValue;
            case SerializedPropertyType.AnimationCurve:
                return thisSP.animationCurveValue;
            case SerializedPropertyType.Bounds:
                return thisSP.boundsValue;
            case SerializedPropertyType.Gradient:
                return SafeGradientValue(thisSP);
            case SerializedPropertyType.Quaternion:
                return thisSP.quaternionValue;

            default:
                throw new NotImplementedException("Unimplemented propertyType " + thisSP.propertyType + ".");
        }
    }

    static void SafeSetGradientValue(SerializedProperty sp, Gradient g)
    {
        GradientProperty.SetValue(sp, g);
    }

    /// Access to SerializedProperty's internal gradientValue property getter, in a manner that'll only soft break (returning null) if the property changes or disappears in future Unity revs.
    static Gradient SafeGradientValue(SerializedProperty sp)
    {
        return GradientProperty.GetValue(sp, null) as Gradient;
    }
}