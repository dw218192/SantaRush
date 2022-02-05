using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint2D), typeof(SpriteRenderer), typeof(Hurtbox))]
public class WagonPart : MonoBehaviour
{
    HingeJoint2D _hingeJoint = null;
    public HingeJoint2D HingeJoint
    {
        get { if (_hingeJoint == null) _hingeJoint = GetComponent<HingeJoint2D>(); return _hingeJoint; }
    }
    
    Rigidbody2D _rgBody = null;
    public Rigidbody2D RigidBody
    {
        get { if (_rgBody == null) _rgBody = GetComponent<Rigidbody2D>(); return _rgBody; }
    }

    SpriteRenderer _rend = null;
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

    Collider2D _collider = null;
    public Collider2D Collider
    {
        get
        {
            if (_collider == null) _collider = GetComponent<Collider2D>();
            return _collider;
        }
    }

    public Wagon parent { get; set; }

    public void OnWagonCollision()
    {
        if(parent.RemoveEnd())
            GameConsts.eventManager.InvokeEvent(typeof(IWagonCollisionHandler), new WagonCollisionEventData(parent.PartCount()));
    }
}
