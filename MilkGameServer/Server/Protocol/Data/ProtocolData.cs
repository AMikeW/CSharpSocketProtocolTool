using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Protocol.Data
{
    public class ProtocolData:ICloneable
    {
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual void SetData(List<object> objectList)
        {

        }
    }
}
