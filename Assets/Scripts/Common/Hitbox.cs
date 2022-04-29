using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    const int COLLISION_BUF_SIZE = 20;

    [SerializeField] bool _destroyOnContact = false;
    [SerializeField] ContactFilter2D _filter;

    Collider2D _collider;

    HashSet<Collider2D> _lastOverlap = new HashSet<Collider2D>();
    Collider2D[] _resultBuffer = new Collider2D[COLLISION_BUF_SIZE];

    HashSet<Hurtbox> _excludeList = new HashSet<Hurtbox>();
    // set a list of game objects that will be ignore, should this hitbox ever collide with their hurtboxes
    public void AddExcludeList(params Hurtbox[] objs)
    {
        foreach(var obj in objs)
            _excludeList.Add(obj);
    }

    public void ClearExcludeList()
    {
        _excludeList.Clear();
    }

    public bool IsInLayer(string layerName)
    {
        return gameObject.layer == LayerMask.NameToLayer(layerName);
    }

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

            if (hurtbox != null &&  // not a non-hurtbox collider
                hurtbox.enabled &&  // hurtbox is enabled
                !ReferenceEquals(_resultBuffer[i].gameObject, gameObject) && // not our own part
                !_excludeList.Contains(hurtbox) // not in our exclude list
                )
            {
                // valid
                curOverlap.Add(_resultBuffer[i]);

                // on enter
                if (!_lastOverlap.Contains(_resultBuffer[i]))
                {
                    if (hurtbox.UseGroup && hurtbox.Group != null)
                        calls.Add(hurtbox.Group.OnHit);
                    else
                        calls.Add(hurtbox.onHit);
                }
            }
        }

        DEBUG_ShowContact(_lastOverlap, curOverlap);

        foreach(var call in calls)
        {
            call.Invoke(this);
        }

        if (calls.Count > 0 && _destroyOnContact)
            Destroy(gameObject);

        _lastOverlap = curOverlap;
    }

    [Conditional("DEBUG")]
    void DEBUG_ShowContact(HashSet<Collider2D> lastOverlap, HashSet<Collider2D> curOverlap)
    { 
        HashSet<Collider2D> tmp = new HashSet<Collider2D>(lastOverlap);
        tmp.IntersectWith(curOverlap);

        foreach (var collider in tmp)
            Debug.DrawLine(transform.position, collider.transform.position, Color.red);
    }
}