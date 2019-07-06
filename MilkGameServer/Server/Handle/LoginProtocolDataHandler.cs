using GameServer.Protocol;
using GameServer.SocketFile;
using System;
namespace GameServer.Handle
{
    class LoginProtocolDataHandler
    {
        public LoginProtocolDataHandler()
        {
            EventSystem.Instance.AddListener(ProtocolEventType.LoginProtocolData, OnRequest);
        }
        ~LoginProtocolDataHandler()
        {
            RemoveAllListener();
        }
        private void OnRequest(object data, Client client)
        {
            LoginProtocolData tempData = (LoginProtocolData)data;
            Console.Write("LoginProtocolData:number=" + tempData.number + ",student.studentName=" + tempData.student.studentName + ",student.sex="
                + tempData.student.sex + ",student.hoby=" + tempData.student.hoby + ",student.score=" + tempData.student.score);
            ProtocolMgr.Instance.SendRequest(ProtocolNo.LoginProtocolData, ProtocolEventType.LoginProtocolData, client, new User(9527, "milk", "666666", "000000"));
        }
        public void RemoveAllListener()
        {
            if (EventSystem.Instance != null)
            {
                EventSystem.Instance.RemoveListener(ProtocolEventType.LoginProtocolData, OnRequest);
            }
        }
    }
}
