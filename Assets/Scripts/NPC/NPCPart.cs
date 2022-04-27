using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Hitbox), typeof(Hurtbox))]
public class NPCPart : SpriteObject
{
    Hitbox _hitbox; // to interact the player's hurtbox
    Hurtbox _hurtbox; // to interact with gift

    public NPCInstance Owner
    {
        get; set;
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
