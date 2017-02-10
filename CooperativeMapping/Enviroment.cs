using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{

    [Serializable]
    public enum PlatformState
    {
        Healthy = 1,
        Destroy = 2,
        OutOfBounderies = 3
    }

    [Serializable]
    public class Enviroment
    {
        public MapObject Map { get; set; }
        public List<Platform> Platforms = new List<Platform>();
        public MapDrawer Drawer { get; set; }        

        public Enviroment(int Rows, int Columns)
        {
            Map = new MapObject(Rows, Columns);
            Drawer = new MapDrawer();
        }

        public PlatformState CheckPlatformState(Platform platform)
        {
            if ((platform.Pose.X < 0) || (platform.Pose.X >= Map.Rows) || (platform.Pose.Y < 0) || (platform.Pose.Y >= Map.Columns))
            {
                return PlatformState.OutOfBounderies;
            }

            foreach (Platform p in Platforms)
            {
                if ( p.Pose.Equals(platform.Pose) && (!p.Equals(platform))) 
                {
                    return PlatformState.Destroy;
                }
            }

            double val = Map.MapMatrix[platform.Pose.X, platform.Pose.Y];

            if (Map.MapMatrix[platform.Pose.X, platform.Pose.Y] != 1)
            {
                return PlatformState.Healthy;
            }

            return PlatformState.Destroy;

        }
    }
}
