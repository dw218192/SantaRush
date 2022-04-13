using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HurtboxGroup : MonoBehaviour
{
    [SerializeField] UnityEvent<Hitbox> _onHit = null;
    public UnityEvent<Hitbox> OnHit { get => _onHit; }
    public List<Hurtbox> Hurtboxes { get; private set; } = new List<Hurtbox>();

    public void Register(Hurtbox hurtbox)
    {
        Hurtboxes.Add(hurtbox);
    }
}
