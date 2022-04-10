using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

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

            if (Group == null)
                Debug.LogWarning("Hurtbox marked as using group but no group is found", this);
        }
    }
}
