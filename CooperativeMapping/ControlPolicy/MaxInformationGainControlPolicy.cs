using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    [Serializable]
    public class MaxInformationGainControlPolicy : ControlPolicyAbstract
    {

        private const int maxDeep = 100;

        public MaxInformationGainControlPolicy()
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

                // this pose is not an obstacle
                if (platform.Map.GetPlace(p) < platform.OccupiedThreshold)
                {
                    nextPoses.Add(p);
                }
            }

            // Find closest undiscovered point
            double maxVal = Double.NegativeInfinity;
            Pose minPose = platform.Pose;

            foreach (Pose p in nextPoses)
            {
                double cmax = FindClosestFrontrierWithInformationGain(p, platform);
                if (cmax > maxVal)
                {
                    maxVal = cmax;
                    minPose = p;
                }
            }

            if (maxVal == int.MaxValue)
            {
                platform.SendLog("No undiscovered area!");
            }

            platform.Move(minPose.X - platform.Pose.X, minPose.Y - platform.Pose.Y);
        }

        private double FindClosestFrontrierWithInformationGain(Pose startPose, Platform platform)
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            double[,] distMap = Matrix.Create<double>(platform.Map.Rows, platform.Map.Columns, Double.NegativeInfinity);
            candidates.Add(startPose);

            double bestScore = Double.NegativeInfinity;
            Pose bestPose = null;
            int undiscoverNum = 0;

            distMap[startPose.X, startPose.Y] = 0;

            for (int k = 1; k < maxDeep; k++)
            {
                if (undiscoverNum > 100)
                {
                    break;
                }

                newCandidates.Clear();
                foreach (Pose cp in candidates)
                {
                    RegionLimits limits = platform.Map.CalculateLimits(cp.X, cp.Y, 1);
                    List<Pose> poses = limits.GetPosesWithinLimits();
                    double currentScore = distMap[cp.X, cp.Y];

                    foreach (Pose p in poses)
                    {
                        if ((p.X == cp.X) && (p.Y == cp.Y)) continue;

                        //if (platform.Map.GetPlace(p) == 0.5)
                        //{
                        
                        // calculate how many undiscovered places are around this pose
                        //List<Tuple<int, Pose>> neighp = platform.CalculateBinsInFOV(p);
                        //double info = neighp.Sum(x => platform.Map.MapMatrix[x.Item2.X, x.Item2.Y]);

                        double info = (0.5 - Math.Abs(platform.Map.MapMatrix[p.X, p.Y] - 0.5));
                        double newScore = (currentScore + info);

                        //int discoveredNeigbours = neighp.Count(x => platform.Map.MapMatrix[x.X, x.Y] == (int)MapPlaceIndicator.Discovered);
                        //double currentScoreWithModifiers = (currentScore + (discoveredNeigbours / 8)*2);

                        //currentScore = currentScoreWithModifiers;

                        if ((platform.Map.MapMatrix[p.X, p.Y] < platform.OccupiedThreshold) && (platform.Map.MapMatrix[p.X, p.Y] > platform.FreeThreshold))
                        {
                            undiscoverNum++;
                        }

                        if ((newScore > bestScore) && (platform.Map.MapMatrix[p.X, p.Y] < platform.OccupiedThreshold) && (platform.Map.MapMatrix[p.X, p.Y] > platform.FreeThreshold))
                        {
                            bestScore = newScore;
                            bestPose = p;
                        }
                        //}

                        // next steps
                        if (distMap[p.X, p.Y] < newScore) 
                        {
                            distMap[p.X, p.Y] = newScore;
                        }

                        if (platform.Map.MapMatrix[p.X, p.Y] < platform.OccupiedThreshold)
                        {
                            newCandidates.Add(p);
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
