using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LoginProtocolData : ProtocolData, ICloneable
{
    //默认备注
    public User user;
    public LoginProtocolData ()
    {
        List<object> unPackMemberList = new List<object>();
        unPackMemberList.Add(DefaultVar.CLASS);
        ProtocolManager.Instance.AddProtocolUnPackMemberListToDict((ProtocolNo)1, unPackMemberList);

        List<object> packMemberList = new List<object>();
        packMemberList.Add(DefaultVar.CLASS);
        packMemberList.Add(DefaultVar.INT32);
        ProtocolManager.Instance.AddProtocolPackMemberListToDict((ProtocolNo)1, packMemberList);
    }

    public override void SetData(List<object> objectList)
    {
        int index = 0;
        user = User.ToClass(objectList, ref index);
    }

    public override object Clone()
    {
        return MemberwiseClone();
    }
}
