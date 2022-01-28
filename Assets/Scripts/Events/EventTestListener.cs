using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTestListener : MonoBehaviour, ITestEvent
{
    public void OnEvent(GameEventData data)
    {
        Debug.Log("Invoked");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
