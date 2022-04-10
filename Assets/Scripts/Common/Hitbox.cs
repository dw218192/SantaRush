using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    const int COLLISION_BUF_SIZE = 20;

    [SerializeField] bool _destroyOnContact = false;
    [SerializeField] ContactFilter2D _filter;

    Collider2D _collider;

    HashSet<Collider2D> _lastOverlap = new HashSet<Collider2D>();
    Collider2D[] _resultBuffer = new Collider2D[COLLISION_BUF_SIZE];

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        int n = _collider.OverlapCollider(_filter, _resultBuffer);

        // get valid hurtboxes, further filtering
        HashSet<Collider2D> curOverlap = new HashSet<Collider2D>();
        HashSet<UnityEvent<Hitbox>> calls = new HashSet<UnityEvent<Hitbox>>();

        for (int i = 0; i < n; ++i)
        {
            Hurtbox hurtbox = _resultBuffer[i].GetComponent<Hurtbox>();

            if (hurtbox != null && !ReferenceEquals(_resultBuffer[i].gameObject, gameObject))
            {
                // valid
                curOverlap.Add(_resultBuffer[i]);

                // on enter
                if (!_lastOverlap.Contains(_resultBuffer[i]))
                {
                    if (hurtbox.UseGroup && hurtbox.Group != null)
                        calls.Add(hurtbox.Group.onHit);
                    else
                        calls.Add(hurtbox.onHit);
                }
            }
        }

        foreach(var call in calls)
        {
            call.Invoke(this);
        }

        if (calls.Count > 0 && _destroyOnContact)
            Destroy(gameObject);

        _lastOverlap = curOverlap;
    }
}