﻿using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.Controllers
{
    [Serializable]
    public class RasterPathPlanningStrategy2Controller : Controller
    {

        private const int maxDeep = 100;

        public RasterPathPlanningStrategy2Controller()
        {

        }

        public override void Next(Platform platform)
        {
            platform.Measure();
            platform.Communicate();

            if (platform.Map.IsDiscovered()) return;

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
                double cmin = FindClosestUndiscovered(p, platform);
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

        private double FindClosestUndiscovered(Pose startPose, Platform platform)
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            double[,] distMap = Matrix.Create<double>(platform.Map.Rows, platform.Map.Columns, int.MaxValue);
            candidates.Add(startPose);

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

                            // calculate how many undiscovered places are around this pose
                            RegionLimits limitsp = platform.Map.CalculateLimits(p, platform.FieldOfViewRadius);
                            List<Pose> neighp = limitsp.GetPosesWithinLimits();
                            int undiscoveredNeigbours = neighp.Count(x => platform.Map.MapMatrix[x.X, x.Y] == (int)MapPlaceIndicator.Undiscovered);
                            double currentScoreWithModifiers = (currentScore + (1.0 - undiscoveredNeigbours / (double)neighp.Count));
                            //currentScore = currentScoreWithModifiers;

                            if (currentScoreWithModifiers < bestScore)
                            {
                                bestScore = currentScoreWithModifiers;
                                bestPose = p;
                            }
                        }

                        // next steps
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
        public override string ToString()
        {
            return "Raster Path Planning Strategy Controller With Undiscovered Counts";
        }

    }
}
