using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArrayElement<T>
{
    [SerializeField]
    T _element;

    public T Value { get => _element; }

    public static implicit operator T(ArrayElement<T> element)
    {
        return element._element;
    }
}
