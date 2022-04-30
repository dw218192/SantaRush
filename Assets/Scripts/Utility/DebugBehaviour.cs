using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * an additional guarantee to make sure no development script leaks into builds
 */
public class DebugBehaviour : MonoBehaviour
{
    void Awake()
    {
#if !DEVELOPMENT_BUILD
        if (!Application.isEditor)
            Destroy(gameObject);
#endif
    }
}
