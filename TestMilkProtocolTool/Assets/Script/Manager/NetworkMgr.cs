using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMgr : MonoBehaviour {

    private static NetworkMgr m_Instance;
    public static NetworkMgr Instance
    {
        get
        {
            return m_Instance;
        }
    }
    private void Awake()
    {
        m_Instance = this;
    }
}
