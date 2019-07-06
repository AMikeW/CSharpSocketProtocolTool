using System.Collections.Generic;
using GameServer.Handle;
namespace Assets.Script.Protocol
{
    public static class InitSystem
    {
        public static void Init(GameServer.Protocol.ProtocolMgr protocolManager)
        {
            protocolManager.AddProtocolObjectToDict((ProtocolNo)1, new LoginProtocolData());
            new LoginProtocolDataHandler();
            protocolManager.AddUnPackClassStructToDict(Student.DATAID, new List<object> { DefaultVar.STRING,DefaultVar.STRING,DefaultVar.STRING,DefaultVar.FLOAT });
        }
    }
}
