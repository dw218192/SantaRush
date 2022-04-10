using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HurtboxGroup : MonoBehaviour
{
    [SerializeField] UnityEvent<Hitbox> _onHit = null;
    public UnityEvent<Hitbox> onHit { get => _onHit; }
}
