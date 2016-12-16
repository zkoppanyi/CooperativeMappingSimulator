using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    public enum PlatformState
    {
        Healthy = 1,
        Destroy = 2,
        OutOfBounderies = 3
    }

    public class Enviroment
    {
        public MapObject Map { get; set; }
        public List<Platform> Platforms = new List<Platform>();

        public Enviroment(int Rows, int Columns)
        {
            Map = new MapObject(Rows, Columns);
        }

        public PlatformState CheckPlatformState(Platform platform)
        {
            if ((platform.Pose.X < 0) || (platform.Pose.X >= Map.Rows) || (platform.Pose.Y < 0) || (platform.Pose.Y >= Map.Columns))
            {
                return PlatformState.OutOfBounderies;
            }

            foreach (Platform p in Platforms)
            {
                if ((p.Pose.X == platform.Pose.X) && (p.Pose.Y == platform.Pose.Y) && (p!=platform)) 
                {
                    return PlatformState.Destroy;
                }
            }

            if ((Map.MapMatrix[platform.Pose.X, platform.Pose.Y] == (int)MapPlaceIndicator.Discovered) ||
                  (Map.MapMatrix[platform.Pose.X, platform.Pose.Y] == (int)MapPlaceIndicator.Undiscovered))
            {
                return PlatformState.Healthy;
            }

            return PlatformState.Destroy;

        }
    }
}
