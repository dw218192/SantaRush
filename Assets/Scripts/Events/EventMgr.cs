using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventMgr : MonoBehaviour
{
    struct EventTarget
    {
        public MonoBehaviour ins;
        public MethodInfo method;
    }

    // maps event type to list of monobehavior instances that listen to this event
    Dictionary<Type, List<EventTarget>> _eventMap = new Dictionary<Type, List<EventTarget>>();

    void Awake()
    {
        if (!GameConsts.eventManager)
        {
            GameConsts.eventManager = this;
            BuildEventMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void BuildEventMap()
    {
        void dfs(Transform cur)
        {
            // check if any monobehaviors on this transform implements any of the events
            MonoBehaviour[] behaviours = cur.gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour behaviour in behaviours)
            {
                foreach (InterfaceMethodPair pair in EventStub.EventTypes)
                {
                    if(pair.type.IsAssignableFrom(behaviour.GetType()))
                    {
                        List<EventTarget> l;
                        if (!_eventMap.TryGetValue(pair.type, out l))
                        {
                            l = new List<EventTarget>();
                            _eventMap.Add(pair.type, l);
                        }

                        l.Add(new EventTarget 
                        { 
                            ins = behaviour, 
                            method = behaviour.GetType().GetMethod(pair.methodName, BindingFlags.Instance | BindingFlags.Public) 
                        });
                    }
                }
            }

            for(int i=0; i<cur.childCount; ++i)
            {
                dfs(cur.GetChild(i));
            }
        }

        Scene scene = SceneManager.GetActiveScene();
        GameObject[] objs = scene.GetRootGameObjects();
        foreach(GameObject obj in objs)
            dfs(obj.transform);
    }

    public void InvokeEvent(Type eventType, GameEventData eventData)
    {
        if(_eventMap.ContainsKey(eventType))
        {
            foreach(EventTarget target in _eventMap[eventType])
            {
                target.method.Invoke(target.ins, new object[] { eventData });
            }
        }
        else
        {
            Debug.LogWarning($"no listener found for event or event does not exist: {eventType}");
        }
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
