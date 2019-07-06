using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 仅用于判断是否为用户类
/// </summary>
public class ClassBase
{    
    public List<object> objectList = new List<object>();

    protected void AddMemberObject(object o)
    {
        objectList.Add(o);
    }
}
