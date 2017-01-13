using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.Controllers
{
    public class RasterPathPlanningWithPriorityStrategy : Controller
    {
        public double[,] PriorityMap = null;

        public RasterPathPlanningWithPriorityStrategy()
        {
            
        }

        public RasterPathPlanningWithPriorityStrategy(double[,] priorityMap)
        {
            this.PriorityMap = priorityMap;
        }

        public override void Next(Platform platform)
        {
            if (PriorityMap == null)
            {
                PriorityMap = Matrix.Create<double>(platform.Map.Rows, platform.Map.Columns, 1);
            }

            platform.Measure();

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

                if ((platform.Map.GetPlace(p) != MapPlaceIndicator.Obstacle) && (platform.Map.GetPlace(p) != MapPlaceIndicator.NoBackVisist))
                {
                    nextPoses.Add(p);
                }
            }

            // Find closest undiscovered point
            double minVal = Double.PositiveInfinity;
            Pose minPose = platform.Pose;

            foreach (Pose p in nextPoses)
            {
                double cmin = FindClosesUndiscovered(p, platform);

                if (cmin < minVal)
                {
                    minVal = cmin;
                    minPose = p;
                }
            }

            if (minVal == int.MaxValue)
            {
                platform.SendLog("No undiscovered area!");
            }

            platform.Move(minPose.X - platform.Pose.X, minPose.Y - platform.Pose.Y);
        }

        private double FindClosesUndiscovered(Pose startPose, Platform platform)
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            double[,] distMap = Matrix.Create<double>(platform.Map.Rows, platform.Map.Columns, int.MaxValue);
            candidates.Add(startPose);
            int maxDeep = 100;

            double bestScore = Double.PositiveInfinity;
            Pose bestPose = null;
            int undiscoverNum = 0;

            distMap[startPose.X, startPose.Y] = 0;

            for (int k = 1; k < maxDeep; k++)
            {
                if (undiscoverNum > 100) break;

                newCandidates.Clear();
                foreach (Pose cp in candidates)
                {
                    RegionLimits limits = platform.Map.CalculateLimits(cp.X, cp.Y, 1);
                    List<Pose> poses = limits.GetPosesWithinLimits();

                    foreach (Pose p in poses)
                    {
                        if ((p.X == cp.X) && (p.Y == cp.Y)) continue;
                        double currentScore = distMap[cp.X, cp.Y] + 1;

                        if (platform.Map.GetPlace(p) == MapPlaceIndicator.Undiscovered)
                        {
                            undiscoverNum++;

                            if (currentScore*PriorityMap[p.X, p.Y] < bestScore)
                            {
                                bestScore = currentScore * PriorityMap[p.X, p.Y];
                                bestPose = p;
                            }
                        }

                        //if ((Platform.Map.GetPlace(p) == MapPlaceIndicator.Discovered) || (Platform.Map.GetPlace(p) == MapPlaceIndicator.Platform))
                        if ((platform.Map.GetPlace(p) != MapPlaceIndicator.Obstacle))
                        {
                            if (distMap[p.X, p.Y] > currentScore)
                            {
                                distMap[p.X, p.Y] = currentScore;
                                newCandidates.Add(p);
                            }
                        }
                    }
                }
                candidates = new List<Pose>(newCandidates);
            }

            return bestScore;
        }

        // depricated
        private int CalcualteDistanceMap(Pose startPose, Pose endPose, Platform platform)
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            int[,] distMap = Matrix.Create<int>(platform.Map.Rows, platform.Map.Columns, int.MaxValue);
            distMap[startPose.X, startPose.Y] = 0;
            candidates.Add(startPose);

            for (int k = 0; k < 100; k++)
            {
                foreach (Pose cp in candidates)
                {
                    int val = distMap[cp.X, cp.Y];
                    RegionLimits limits = platform.Map.CalculateLimits(cp.X, cp.Y, 1);
                    List<Pose> poses = limits.GetPosesWithinLimits();

                    newCandidates.Clear();
                    foreach (Pose p in poses)
                    {
                        //if ((Platform.Map.GetPlace(p) == MapPlaceIndicator.Discovered) || (Platform.Map.GetPlace(p) == MapPlaceIndicator.Platform))
                        if ((platform.Map.GetPlace(p) != MapPlaceIndicator.Obstacle))
                        {
                            distMap[p.X, p.Y] = val + 1;
                            newCandidates.Add(p);
                        }

                        if ((endPose.X == p.X) && (endPose.Y == p.Y))
                        {
                            return val + 1;
                        }
                    }
                }
                candidates = new List<Pose>(newCandidates);
            }

            return int.MaxValue;
        }
    }
}
