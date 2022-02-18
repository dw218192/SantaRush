using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StickOnContact : MonoBehaviour
{
    Rigidbody2D _rb = null;
    ContactPoint2D[] _contacts = new ContactPoint2D[64];

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        bool vertical = false, horizontal = false;
        int num = collision.GetContacts(_contacts);

        Vector2 vert = _rb.velocity.y > 0 ? Vector2.down : Vector2.up;
        Vector2 hori = _rb.velocity.x > 0 ? Vector2.left : Vector2.right;

        for(int i=0; i<num; ++i)
        {
            if (Vector2.Dot(_contacts[i].normal, vert) > 0.9f)
                vertical = true;
            else if (Vector2.Dot(_contacts[i].normal, hori) > 0.9f)
                horizontal = true;
        }

        transform.parent = collision.collider.transform;
        if (vertical)
        {
            _rb.velocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Static;
        }
        else if(horizontal)
        {
            _rb.velocity.Set(0, _rb.velocity.y);
        }
    }
}
