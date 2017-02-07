using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    [Serializable]
    public class ClosestFronterierControlPolicy : ControlPolicyAbstract, IDistanceMap
    {
        private double[,] distMap;
        private double minDistMap;
        private double maxDistMap;

        public double[,] DistMap
        {
            get
            {
                return distMap;
            }
        }

        public double MinDistMap
        {
            get
            {
                return minDistMap;
            }
        }

        public double MaxDistMap
        {
            get
            {
                return maxDistMap;
            }
        }

        public ClosestFronterierControlPolicy()
        {

        }

        public override void Next(Platform platform)
        {
            platform.Measure();
            platform.Communicate();

            if (platform.Map.IsDiscovered(platform)) return;

            RegionLimits limits = platform.Map.CalculateLimits(platform.Pose, 1);
            List<Pose> poses = limits.GetPosesWithinLimits();

            //Remove those possible poses that are obstacles 
            List<Pose> nextPoses = new List<Pose>();
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

                // is this pose an obstacle
                if (platform.Map.GetPlace(p) < platform.OccupiedThreshold)
                {
                    nextPoses.Add(p);
                }
            }

            // Find closest undiscovered point
            double minVal = Double.PositiveInfinity;
            Pose minPose = platform.Pose;

            foreach(Pose p in nextPoses)
            {
                int cmin = FindClosestFronterier(p, platform);               

                if (cmin < minVal)
                {
                    minVal = cmin;
                    minPose = p;
                }
            }

            if (minVal == Double.NegativeInfinity)
            {
                platform.SendLog("No undiscovered area!");
            }

            FindClosestFronterier(minPose, platform);
            platform.Move(minPose.X - platform.Pose.X, minPose.Y - platform.Pose.Y);
        }

        private int FindClosestFronterier(Pose startPose, Platform platform)
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            distMap = Matrix.Create<double>(platform.Map.Rows, platform.Map.Columns, Double.PositiveInfinity);
            candidates.Add(startPose);
            int maxDeep = 1000;

            distMap[startPose.X, startPose.Y] = 0;
            minDistMap = 0;
            maxDistMap = 0;

            for (int k = 1; k < maxDeep; k++)
            {
                newCandidates.Clear();
                foreach (Pose cp in candidates)
                {                    
                    RegionLimits limits = platform.Map.CalculateLimits(cp.X, cp.Y, 1);
                    List<Pose> poses = limits.GetPosesWithinLimits();
                                       
                    foreach (Pose p in poses)
                    {
                        if ((p.X == cp.X) && (p.Y == cp.Y)) continue;

                        if ((platform.Map.GetPlace(p) > platform.FreeThreshold) && (platform.Map.GetPlace(p) < platform.OccupiedThreshold))
                        //if (platform.Map.GetPlace(p) == 0.5)
                        {
                            return k;
                        }

                        //if ((Platform.Map.GetPlace(p) == MapPlaceIndicator.Discovered) || (Platform.Map.GetPlace(p) == MapPlaceIndicator.Platform))
                        if ((platform.Map.GetPlace(p) < platform.OccupiedThreshold) && (distMap[p.X, p.Y] == Double.PositiveInfinity))
                        {
                            distMap[p.X, p.Y] = (double)k;
                            newCandidates.Add(p);

                            if (minDistMap > k) minDistMap = (double)k;
                            if (maxDistMap < k) maxDistMap = (double)k;
                        }
                    }
                }
                candidates = new List<Pose>(newCandidates);
            }

            return int.MaxValue;
        }

        public override string ToString()
        {
            return "Closest Fronterier Control Policy";
        }

    }
}
