using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class AutoResizeBoxCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D boxCollider = gameObject.GetComponent<BoxCollider2D>();
        SpriteRenderer rend = gameObject.GetComponent<SpriteRenderer>();
        if(rend != null)
        {
            Bounds bounds = rend.bounds;
            boxCollider.size = transform.InverseTransformVector(bounds.size);
            boxCollider.offset = transform.InverseTransformPoint(bounds.center);
        }
    }
}
