using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteObject : MonoBehaviour
{
    protected SpriteRenderer _rend = null;
    public float Width
    {
        get
        {
            if (_rend == null) _rend = GetComponent<SpriteRenderer>();
            return _rend.bounds.size.x;
        }
    }

    public SpriteRenderer Renderer
    {
        get
        {
            if (_rend == null) _rend = GetComponent<SpriteRenderer>();
            return _rend;
        }
    }
}
