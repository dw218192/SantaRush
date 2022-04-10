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
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] objs = scene.GetRootGameObjects();
        foreach(GameObject obj in objs)
            RegisterRecursive(obj.transform);
    }

    public void RegisterRecursive(Transform cur)
    {
        // check if any monobehaviors on this transform implements any of the events
        MonoBehaviour[] behaviours = cur.gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
            Register(behaviour);

        for (int i = 0; i < cur.childCount; ++i)
            RegisterRecursive(cur.GetChild(i));
    }

    public void Register(MonoBehaviour behaviour)
    {
        foreach (InterfaceMethodPair pair in EventStub.EventTypes)
        {
            if (pair.type.IsAssignableFrom(behaviour.GetType()))
            {
                List<EventTarget> l;
                if (!_eventMap.TryGetValue(pair.type, out l))
                {
                    l = new List<EventTarget>();
                    _eventMap.Add(pair.type, l);
                }

                // make sure no duplicate exists in event targets
                bool found = false;
                foreach(EventTarget target in l)
                {
                    if(ReferenceEquals(target.ins, behaviour))
                    {
                        found = true;
                        break;
                    }
                }
                if(!found)
                {
                    l.Add(new EventTarget
                    {
                        ins = behaviour,
                        method = behaviour.GetType().GetMethod(pair.methodName, BindingFlags.Instance | BindingFlags.Public)
                    });
                }
            }
        }
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
