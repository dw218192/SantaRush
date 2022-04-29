using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OutOfViewDestroyer), typeof(Scroller))]
[RequireComponent(typeof(Hitbox), typeof(SpriteRenderer))]
public class GiftInstance : MonoBehaviour
{
    public static GiftInstance Create(GiftType type, Vector2 pos)
    {
        var go = Instantiate(GameConsts.GetPrefab(GameConsts.k_ResourcesGiftPrefabPath));
        go.name = $"{type}_GiftInstance";

        go.SetActive(false); // required to do initialization before awake

        var ret = go.GetComponent<GiftInstance>();
        ret.GiftType = type;

        go.transform.position = pos;
        go.SetActive(true);

        return ret;
    }

    SpriteRenderer _rend;
    public SpriteRenderer rend
    {
        get
        {
            if (_rend == null)
                _rend = GetComponent<SpriteRenderer>();
            return _rend;
        }
    }

    GiftType _giftType;
    public GiftType GiftType 
    {
        get => _giftType;
        set
        {
            rend.color = value.GetColor();
            _giftType = value;
        }
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
        gameObject.layer = LayerMask.NameToLayer(GameConsts.k_GiftLayerName);
    }
}
