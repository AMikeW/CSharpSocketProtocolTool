using System.Collections;
using System.Collections.Generic;
using System;
using GameServer.Protocol.Data;
using GameServer.SocketFile;

//public enum ProtocolEventType
//{
//    Null,
//    LoginProtocol,
//    RegisterProtocol,
//    RLUserProtocolData,
//    MiniProtocolData,
//}

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
    private Dictionary<ProtocolEventType, Action<object, Client>> eventDict = new Dictionary<ProtocolEventType, Action<object, Client>>();

    public void Dispatch(ProtocolEventType eventType, object data, Client client)
    {
        Action<object, Client> protoEvent;
        eventDict.TryGetValue(eventType, out protoEvent);
        if (protoEvent != null)
        {
            protoEvent(data, client);
            //Debug.Log("派发事件:" + eventType + "成功!");
        }
        else
        {
            //Debug.LogWarning("Event Dispatch失败, 事件系统没人监听该事件\nEventType=[" + eventType + "]");
        }
    }

    public void AddListener(ProtocolEventType eventType, Action<object, Client> action)
    {
        //***注意：这里有个小坑,用TryGetValue取出的Action<>其实是全新的，你对它进行添加action，并不会对字典里面的那个Acition<>进行改动
        //所以一定要用这种方式来进行追加，或者是你用TryGetValue拿出Action<>后，对它进行添加后，将这个Action<>放回到字典对应的key内容中
        if (eventDict.ContainsKey(eventType))
        {
            //Debug.Log("追加订阅事件,名为:" + eventType);
            eventDict[eventType] += action;
        }
        else
        {
            //Debug.Log("添加第一个订阅事件,名为:" + eventType);
            eventDict.Add(eventType, action);
        }
    }

    public void RemoveListener(ProtocolEventType eventType, Action<object, Client> action)
    {
        if (eventDict.ContainsKey(eventType))
        {
            eventDict[eventType] -= action;
        }
        else
        {
            throw new Exception("事件系统没有该事件\nEventType=[" + eventType + "],Action<ProtocolData>=[" + action + "]");
        }
    }
}