using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class HurtboxHandlerAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class StateMachineFunctionAttribute : Attribute
{
    
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class NPCHeightAttribute : PropertyAttribute
{
    public string label;
    public NPCHeightAttribute(string label)
    {
        this.label = label;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class GiftTypeDropdownAttribute : PropertyAttribute
{
    public string label;
    public GiftTypeDropdownAttribute(string label)
    {
        this.label = label;
    }
}