using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTestPublisher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameConsts.eventManager.InvokeEvent(typeof(ITestEvent), null);
    }
}