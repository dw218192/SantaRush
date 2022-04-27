using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HingeJoint2D), typeof(Hurtbox))]
public class WagonPart : SpriteObject
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

    Collider2D _collider = null;
    public Collider2D Collider
    {
        get
        {
            if (_collider == null) _collider = GetComponent<Collider2D>();
            return _collider;
        }
    }

    Hurtbox _hurtBox = null;
    public Hurtbox Hurtbox
    {
        get
        {
            if (_hurtBox == null) _hurtBox = GetComponent<Hurtbox>();
            return _hurtBox;
        }
    }
    UnityEvent<Hitbox> _uevent = null;
    public UnityEvent<Hitbox> OnPartHurt
    {
        get
        {
            if (_uevent == null) _uevent = GetComponent<Hurtbox>().onHit;
            return _uevent;
        }
    }
}
