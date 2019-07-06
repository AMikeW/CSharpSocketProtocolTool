using System.Collections.Generic;
namespace Assets.Script.Protocol
{
    public static class InitSystem
    {
        public static void Init(ProtocolManager protocolManager)
        {
            protocolManager.AddProtocolObjectToDict((ProtocolNo)1, new LoginProtocolData());
            protocolManager.AddUnPackClassStructToDict(User.DATAID, new List<object> { DefaultVar.INT32,DefaultVar.STRING,DefaultVar.STRING,DefaultVar.STRING });
        }
    }
}
