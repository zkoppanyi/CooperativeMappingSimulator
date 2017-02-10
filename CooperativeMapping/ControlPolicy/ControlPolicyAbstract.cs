using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    [Serializable]
    public abstract class ControlPolicyAbstract
    {
        public abstract void Next(Platform platform);

    }
}
