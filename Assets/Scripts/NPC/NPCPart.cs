using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer), typeof(Hitbox), typeof(Hurtbox))]
public class NPCPart : MonoBehaviour
{
    SpriteRenderer _rend = null;

    Hitbox _hitbox; // to interact the player's hurtbox
    Hurtbox _hurtbox; // to interact with gift

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

    public Hurtbox Hurtbox 
    {
        get
        {
            if (_hurtbox == null) _hurtbox = GetComponent<Hurtbox>();
            return _hurtbox;
        }
    }
}
