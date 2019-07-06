using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassBase
{
    public List<object> objectList = new List<object>();

    protected void AddMemberObject(object o)
    {
        objectList.Add(o);
    }
}
