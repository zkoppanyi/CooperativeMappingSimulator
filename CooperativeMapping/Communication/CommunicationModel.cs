using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.Communication
{
    [Serializable]
    public abstract class CommunicationModel
    {
        public abstract void Acquire(Platform platform, Enviroment enviroment);
    }
}
