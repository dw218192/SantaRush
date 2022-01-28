using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        bool shouldStick = false;
        int num = collision.GetContacts(_contacts);
        for(int i=0; i<num && !shouldStick; ++i)
        {
            if (Vector2.Dot(_contacts[i].normal, Vector2.up) > 0.9f)
                shouldStick = true;
        }

        if(shouldStick)
        {
            if (_rb != null)
            {
                _rb.bodyType = RigidbodyType2D.Static;
            }

            transform.parent = collision.transform;
        }
    }
}
