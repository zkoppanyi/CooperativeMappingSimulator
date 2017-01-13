using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.Communication
{
    public class GlobalCommunicationModel : CommunicationModel
    {
        public override void Acquire(Platform platform, Enviroment enviroment)
        {
            foreach(Platform plt in enviroment.Platforms)
            {
                if (plt.CommunicationModel is NoCommunication) continue;

                if (!platform.ObservedPlatforms.Exists(x => x == plt))
                {
                    platform.ObservedPlatforms.Add(plt);
                }

                platform.Map.UpdateFromAnotherMap(plt.Map);
            }
        }

        public override string ToString()
        {
            return "Global Communication Model";
        }

    }
}
