using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] UnityEvent<Hitbox> _onHit = null;
    [SerializeField] LayerMask _excludeLayers = 0;
    public LayerMask ExcludeLayers { get => _excludeLayers; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Handle(GameObject other)
    {
        Hitbox hitbox = other.GetComponent<Hitbox>();
        if (hitbox && other != gameObject && (_excludeLayers.value & (1 << other.layer)) == 0)
        {
            _onHit.Invoke(hitbox);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Handle(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Handle(collision.gameObject);
    }
}
