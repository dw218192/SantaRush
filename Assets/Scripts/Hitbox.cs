using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    [SerializeField] bool _destroyOnContact = false;

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
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if(hurtbox && other != gameObject && (hurtbox.ExcludeLayers & (1 << gameObject.layer)) == 0)
        {
            if (_destroyOnContact)
                Destroy(gameObject);
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
