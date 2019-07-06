using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Protocol.Data;
using GameServer.Protocol;
public class LoginProtocolData : ProtocolData, ICloneable
{
    //
    public Student student;
    //默认备注
    public int number;
    public LoginProtocolData ()
    {
        List<object> unPackMemberList = new List<object>();
        unPackMemberList.Add(DefaultVar.CLASS);
        unPackMemberList.Add(DefaultVar.INT32);
        ProtocolMgr.Instance.AddProtocolUnPackMemberListToDict((ProtocolNo)1, unPackMemberList);

        List<object> packMemberList = new List<object>();
        packMemberList.Add(DefaultVar.CLASS);
        ProtocolMgr.Instance.AddProtocolPackMemberListToDict((ProtocolNo)1, packMemberList);
    }

    public override void SetData(List<object> objectList)
    {
        int index = 0;
        student = Student.ToClass(objectList, ref index);
        number = (int)objectList[index];
        index += 1;
    }

    public override object Clone()
    {
        return MemberwiseClone();
    }
}
