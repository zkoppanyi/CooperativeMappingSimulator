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


            //vectors
            double sx = 0;
            double sy = 0;
            foreach (Platform plt in platform.ObservedPlatforms)
            {
                if ((plt.Pose.X != platform.Pose.X) && (plt.Pose.Y != platform.Pose.Y))
                {
                    double dx = (platform.Pose.X - plt.Pose.X);
                    double dy = (platform.Pose.Y - plt.Pose.Y);
                    sx += dx;
                    sy += dy;
                }
            }
            double alpha = Math.Atan2(sy, sx);

            // Find closest undiscovered point
            double maxVal = Double.NegativeInfinity;
            Pose minPose = platform.Pose;

            foreach (Pose p in nextPoses)
            {
                double cmax = FindClosestFrontrierWithInformationGain(p, platform);
                /*double alphap = Math.Atan2(platform.Pose.Y - p.Y, platform.Pose.X - p.X);
                double forceCorrection = -Math.Abs(alpha - alphap)/50;
                //double forceCorrection = -Math.Abs(alpha - alphap);
                cmax += forceCorrection;*/

                double r = 0;
                foreach (Platform plt in platform.ObservedPlatforms)
                {
                    if ((plt.Pose.X != p.X) && (plt.Pose.Y != p.Y))
                    {
                         r += Math.Sqrt(Math.Pow((p.X - plt.Pose.X), 2) + Math.Pow((p.Y - plt.Pose.Y), 2));
                    }
                }
                cmax -= r / platform.ObservedPlatforms.Count;

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

            int k = 1;
            for (k = 1; k < maxDeep; k++)
            {

                if (candidates.Count == 0)
                {
                    break;
                }

                if (undiscoverNum > 5)
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

                        // calculate information gain considering FOV - it's too slow
                        /*List<Tuple<int, Pose>> neighp = platform.CalculateBinsInFOV(p, 2);
                        double info = neighp.Sum(x => 0.5 - Math.Abs(platform.Map.MapMatrix[x.Item2.X, x.Item2.Y] - 0.5)) / neighp.Count;*/

                        RegionLimits nlimits = platform.Map.CalculateLimits(cp.X, cp.Y, 1);
                        List<Pose> neighp = nlimits.GetPosesWithinLimits();
                        double info = neighp.Sum(x => 0.5 - Math.Abs(platform.Map.MapMatrix[x.X, x.Y] - 0.5)) / neighp.Count;

                        double newScore = (currentScore + info) - k;

                        if ((platform.Map.MapMatrix[p.X, p.Y] < platform.OccupiedThreshold) && (platform.Map.MapMatrix[p.X, p.Y] > platform.FreeThreshold))
                        {
                            undiscoverNum++;
                        }

                        if ((newScore > bestScore) && (platform.Map.MapMatrix[p.X, p.Y] < platform.OccupiedThreshold) && (platform.Map.MapMatrix[p.X, p.Y] > platform.FreeThreshold))
                        {
                            bestScore = newScore;
                            bestPose = p;
                        }

                        // next steps
                        if ((distMap[p.X, p.Y] < newScore) && (platform.Map.MapMatrix[p.X, p.Y] <= platform.FreeThreshold))
                        {
                            distMap[p.X, p.Y] = newScore;
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
