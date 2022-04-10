using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    #region Event Manager Related
    // we need to register with event manager whenever a gameobject/component gets added to the scene dynamically
    public static T AddComponentEx<T>(this GameObject obj) where T : MonoBehaviour
    {
        T ret = obj.AddComponent<T>();
        GameConsts.eventManager.Register(ret);

        return ret;
    }

    public static GameObject InstantiateEx(GameObject prefab)
    {
        GameObject ret = GameObject.Instantiate(prefab);
        GameConsts.eventManager.RegisterRecursive(ret.transform);
        return ret;
    }
    #endregion
}