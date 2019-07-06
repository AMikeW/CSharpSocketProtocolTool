using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProtocol : MonoBehaviour
{

    private void Start()
    {
        EventSystem.Instance.AddListener(ProtocolEventType.LoginProtocolData, OnLoginResult);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProtocolManager.Instance.SendRequest(ProtocolNo.LoginProtocolData, ProtocolEventType.LoginProtocolData, 
                new Student("milk", "男人", "喝牛奶", 100), 666666);
        }
    }
    private void OnLoginResult(ProtocolData data)
    {
        LoginProtocolData tdata = (LoginProtocolData)data;
        Debug.Log("LoginProtocol:userID=" + tdata.user.userID + ",userName="
            + tdata.user.userName + ",userPassword="
            + tdata.user.userPassword + ",photo="
            + tdata.user.photo);
    }
    private void OnDestroy()
    {
        EventSystem.Instance.RemoveListener(ProtocolEventType.LoginProtocolData, OnLoginResult);
    }
}
