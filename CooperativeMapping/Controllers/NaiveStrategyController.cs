using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.Controllers
{
    [Serializable]
    public class NaiveStrategyController : Controller
    {
        public NaiveStrategyController() 
        {

        }      
        
        public override void Next(Platform platform)
        {
            platform.Measure();
            platform.Communicate();

            RegionLimits limits = platform.Map.CalculateLimits(platform.Pose, 1);
            List<Pose> poses = limits.GetPosesWithinLimits();

            //Remove those possible poses that are obstacles 
            List<Pose> possiblePoses = new List<Pose>();
            foreach (Pose p in poses)
            {
                // is there another platform on this pose?
                bool find = false;
                foreach (Platform plt in platform.ObservedPlatforms)
                {
                    if ((plt.Pose.X == p.X) && (plt.Pose.Y == p.Y))
                    {
                        find = true;
                        break;
                    }
                }
                if (find) continue;

                if ((platform.Map.GetPlace(p) != MapPlaceIndicator.Obstacle) && (platform.Map.GetPlace(p) != MapPlaceIndicator.NoBackVisist))
                {
                    possiblePoses.Add(p);
                }
            }

            if (platform.Map.GetPlace(platform.Pose) != MapPlaceIndicator.NoBackVisist)
            {
                possiblePoses.Add(platform.Pose);
            }

            // Find closest undiscovered point
            double minVal = Double.PositiveInfinity;
            Pose minPose = platform.Pose;
            for (int i = 0; i < platform.Map.Rows; i++)
            {
                for (int j = 0; j < platform.Map.Columns; j++)
                {
                    if (platform.Map.MapMatrix[i, j] == (int)MapPlaceIndicator.Undiscovered)
                    {
                        // Check whether the cell has discovered neighbor
                        limits = platform.Map.CalculateLimits(i, j, 1);
                        poses = limits.GetPosesWithinLimits();
                        Pose discoveredPlace = poses.Find(p => platform.Map.GetPlace(p) == MapPlaceIndicator.Discovered);

                        // if it does not have discovered neigbor, then skip it
                        if (discoveredPlace != null)
                        {
                            // Calculate the closest next pose
                            foreach (Pose p in possiblePoses)
                            {
                                double d = Distance.Euclidean(p.X, p.Y, i, j);
                                if (d < minVal)
                                {
                                    minVal = d;
                                    minPose = p;
                                }
                            }
                        }
                    }
                }
            }

            if ((minPose.X == platform.Pose.X) && (minPose.Y == platform.Pose.Y))
            {
                platform.Map.MapMatrix[platform.Pose.X, platform.Pose.Y] = (int)MapPlaceIndicator.NoBackVisist;
            }

            platform.Move(minPose.X - platform.Pose.X, minPose.Y - platform.Pose.Y);
        }

        public override string ToString()
        {
            return "Naive Strategy Controller";
        }
    }
}
