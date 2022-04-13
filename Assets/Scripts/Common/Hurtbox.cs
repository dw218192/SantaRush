using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using Debug = UnityEngine.Debug;
[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour
{
    [DisableIf("_useGroup")]
    [SerializeField]
    UnityEvent<Hitbox> _onHit = null;
    
    [SerializeField]
    LayerMask _layerMask = 0;
    
    [SerializeField]
    bool _useGroup = false;

    // if group is used, this will be ignored

    public UnityEvent<Hitbox> onHit { get => _onHit; }
    public HurtboxGroup Group { get; set; } = null;
    public bool UseGroup { get => _useGroup; }

    public void Handle(Hitbox hitbox)
    {
        if (_useGroup && Group != null)
        {
            Debug.LogWarning("Hurtbox::Handle called on grouped hurtbox");
            return;
        }

        if ((_layerMask.value & (1 << hitbox.gameObject.layer)) != 0)
        {
            _onHit.Invoke(hitbox);
        }
    }

    void Awake()
    {
        if(_useGroup)
        {
            Transform trans = transform;
            // walk up parent chain to find a group
            while (trans != null)
            {
                Group = trans.GetComponent<HurtboxGroup>();
                if (Group != null)
                    break;
                trans = trans.parent;
            }

            Group.Register(this);

            if (Group == null)
                Debug.LogWarning("Hurtbox marked as using group but no group is found", this);
            else
                DEBUG_RegisterOnHit(Group.OnHit);
        }
        else
        {
            DEBUG_RegisterOnHit(_onHit);
        }
    }


    [Conditional("DEBUG")]
    void DEBUG_RegisterOnHit(UnityEvent<Hitbox> onhit)
    {
        IEnumerator _drawRoutine(Hitbox hitbox, float timer)
        {
            if (!enabled || Mathf.Approximately(timer, 5f))
                yield break;

            Vector3 offset = Vector3.right * 0.2f;
            Debug.DrawLine(transform.position + offset, hitbox.transform.position + offset, Color.blue);
            // Debug.Break();

            timer += Time.deltaTime;                
            yield return null;
        }

        onhit.AddListener((Hitbox hitbox)=>
        {
            StartCoroutine(_drawRoutine(hitbox, 0));
        });
    }
}
