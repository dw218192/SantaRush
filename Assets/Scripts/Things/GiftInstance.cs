using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OutOfViewDestroyer), typeof(StickOnContact), typeof(Scroller))]
[RequireComponent(typeof(Hitbox))]
public class GiftInstance : MonoBehaviour
{
    public static GiftInstance Create(GiftType type, Vector2 pos)
    {
        var go = Extension.InstantiateEx(GameConsts.GetPrefab(GameConsts.k_ResourcesGiftPrefabPath));
        go.name = $"{type}_GiftInstance";

        go.SetActive(false); // required to do initialization before awake

        var ret = go.GetComponent<GiftInstance>();
        ret.GiftType = type;

        go.transform.position = pos;
        go.SetActive(true);

        return ret;
    }

    GiftType _giftType;
    public GiftType GiftType { get => _giftType; set => _giftType = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
