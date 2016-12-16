using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    public class NaiveStrategy : Controller
    {
        public NaiveStrategy(Platform platform) : base(platform)
        {

        }

        public override void Next()
        {
            Platform.Measure();

            RegionLimits limits = Platform.Map.CalculateLimits(Platform.Pose, 1);
            List<Pose> poses = limits.GetPosesWithinLimits();

            //Remove those possible poses that are obstacles 
            List<Pose> possiblePoses = new List<Pose>();
            foreach (Pose p in poses)
            {
                if ((Platform.Map.GetPlace(p) != MapPlaceIndicator.Obstacle) && (Platform.Map.GetPlace(p) != MapPlaceIndicator.Platform) && (Platform.Map.GetPlace(p) != MapPlaceIndicator.NoBackVisist))
                //&& !((p.X == this.Pose.X) && (p.Y == this.Pose.Y)))
                {
                    possiblePoses.Add(p);
                }
            }
            if (Platform.Map.GetPlace(Platform.Pose) != MapPlaceIndicator.NoBackVisist)
            {
                possiblePoses.Add(Platform.Pose);
            }

            // Find closest undiscovered point
            double minVal = Double.PositiveInfinity;
            Pose minPose = Platform.Pose;
            for (int i = 0; i < Platform.Map.Rows; i++)
            {
                for (int j = 0; j < Platform.Map.Columns; j++)
                {
                    if (Platform.Map.MapMatrix[i, j] == (int)MapPlaceIndicator.Undiscovered)
                    {
                        // Check whether the cell has discovered neighbor
                        limits = Platform.Map.CalculateLimits(i, j, 1);
                        poses = limits.GetPosesWithinLimits();
                        Pose discoveredPlace = poses.Find(p => Platform.Map.GetPlace(p) == MapPlaceIndicator.Discovered);

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

            if ((minPose.X == Platform.Pose.X) && (minPose.Y == Platform.Pose.Y))
            {
                Platform.Map.MapMatrix[Platform.Pose.X, Platform.Pose.Y] = (int)MapPlaceIndicator.NoBackVisist;
            }

            Platform.Move(minPose.X - Platform.Pose.X, minPose.Y - Platform.Pose.Y);
        }
    }
}
