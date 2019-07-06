using System.Collections;
using System.Collections.Generic;

public class ProtocolData : System.ICloneable
{
    public virtual object Clone()
    {
        return MemberwiseClone();
    }

    public virtual void SetData(List<object> objectList)
    {

    }
}
