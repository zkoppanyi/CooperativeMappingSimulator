using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.Communication
{
    [Serializable]
    public class NearbyCommunicationModel : CommunicationModel
    {
        public double Radius { get; set; }

        public NearbyCommunicationModel()
        {
            Radius = 50;
        }

        public override void Acquire(Platform platform, Enviroment enviroment)
        {
            foreach (Platform plt in enviroment.Platforms)
            {
                if (plt.Equals(platform)) continue;

                if (plt.CommunicationModel is NoCommunication) continue;

                double d = Math.Sqrt(Math.Pow(plt.Pose.X - platform.Pose.X, 2) + Math.Pow(plt.Pose.Y - platform.Pose.Y, 2));
                if (d > Radius) continue; 

                if (!platform.ObservedPlatforms.Exists(x => x == plt))
                {
                    platform.ObservedPlatforms.Add(plt);
                }

                platform.Map.UpdateFromAnotherMap(plt.Map);
            }
        }

        public override string ToString()
        {
            return "Nearby Communication Model";
        }
    }
}
