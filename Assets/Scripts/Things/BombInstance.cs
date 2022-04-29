using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OutOfViewDestroyer), typeof(Scroller))]
[RequireComponent(typeof(Hitbox), typeof(SpriteRenderer))]
public class BombInstance : MonoBehaviour
{
    public static BombInstance Create(Vector2 pos)
    {
        var go = Instantiate(GameConsts.GetPrefab(GameConsts.k_ResourcesBombPrefabPath));
        go.transform.position = pos;
        var ret = go.GetComponent<BombInstance>();
        return ret;
    }

    Hitbox _hitbox;
    public Hitbox Hitbox
    {
        get
        {
            if (_hitbox == null)
                _hitbox = GetComponent<Hitbox>();
            return _hitbox;
        }
    }
    
    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(GameConsts.k_BombLayerName);
    }
}
