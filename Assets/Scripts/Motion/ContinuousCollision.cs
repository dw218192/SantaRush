using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class ContinuousCollision : MonoBehaviour
{
    [SerializeField] LayerMask _includeLayers;

    Collider2D _col;
    Rigidbody2D _rg;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        _rg = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_rg.velocity.sqrMagnitude > 0)
        {
            Vector2 dir = _rg.velocity.normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, _includeLayers);
            float t1 = (hit.point.x - transform.position.x) / dir.x;
            if (t1 < Time.fixedDeltaTime)
            {
                Vector2 comp = Vector2.Dot(_rg.velocity, -hit.normal) * (-hit.normal);
                _rg.velocity -= comp;
            }
        }
    }
}