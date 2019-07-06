using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventSystem
{
    private static EventSystem m_Instance;
    public static EventSystem Instance
    {
        get
        {
            if (m_Instance == null)
            {
                if (m_Instance == null)
                {
                    m_Instance = new EventSystem();
                }
            }
            return m_Instance;
        }
    }
    private Dictionary<ProtocolEventType, Action<ProtocolData>> eventDict = new Dictionary<ProtocolEventType, Action<ProtocolData>>();

    public void Dispatch(ProtocolEventType eventType, ProtocolData data)
    {
        Action<ProtocolData> protoEvent;
        eventDict.TryGetValue(eventType, out protoEvent);
        if (protoEvent != null)
        {
            Debug.Log("派发事件:" + eventType + "成功!");
            protoEvent(data);            
        }
        else
        {
            Debug.LogWarning("Event Dispatch失败, 事件系统没人监听该事件\nEventType=[" + eventType + "]");
        }
    }

    public void AddListener(ProtocolEventType eventType, Action<ProtocolData> action)
    {
        //***注意：这里有个小坑,用TryGetValue取出的Action<>其实是全新的，你对它进行添加action，并不会对字典里面的那个Acition<>进行改动
        //所以一定要用这种方式来进行追加，或者是你用TryGetValue拿出Action<>后，对它进行添加后，将这个Action<>放回到字典对应的key内容中
        if (eventDict.ContainsKey(eventType))
        {
            Debug.Log("追加订阅事件,名为:" + eventType);
            eventDict[eventType] += action;
        }
        else
        {
            Debug.Log("添加第一个订阅事件,名为:" + eventType);
            eventDict.Add(eventType, action);
        }
    }

    public void RemoveListener(ProtocolEventType eventType, Action<ProtocolData> action)
    {
        if (eventDict.ContainsKey(eventType))
        {
            eventDict[eventType] -= action;
        }
        else
        {
            Debug.LogError("事件系统没有该事件\nEventType=[" + eventType + "],Action<ProtocolData>=[" + action + "]");
        }
    }
}
