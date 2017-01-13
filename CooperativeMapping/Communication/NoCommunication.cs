using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.Communication
{
    public class NoCommunication : CommunicationModel
    {
        public override void Acquire(Platform platform, Enviroment enviroment)
        {
            
        }

        public override string ToString()
        {
            return "No Communication";
        }
    }
}
