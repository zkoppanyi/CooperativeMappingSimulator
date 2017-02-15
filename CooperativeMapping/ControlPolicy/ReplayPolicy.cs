using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    public class ReplayPolicy : ControlPolicyAbstract
    {
        public override void NextInit(Platform platform)
        {
            if (this.CommandSequence.Count == 0)
            {
                platform.Stop();
            }

        }


        public override void ReplanLocal(Platform platform, int searchRadius)
        {

        }


        public override void ReplanGlobal(Platform platform)
        {

        }
    }
}
