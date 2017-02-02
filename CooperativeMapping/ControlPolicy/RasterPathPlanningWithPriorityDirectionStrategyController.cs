using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    [Serializable]
    public class RasterPathPlanningWithPriorityDirectionStrategyController : ControlPolicyAbstract
    {

        public int PriorityDirection { get; set; }
        public double DirectionWeight { get; set; }

        public RasterPathPlanningWithPriorityDirectionStrategyController()
        {
            PriorityDirection = 1;
            DirectionWeight = 0.8;
        }

        public RasterPathPlanningWithPriorityDirectionStrategyController(int priorityDirection)
        {
            this.PriorityDirection = priorityDirection;
        }

        public override void Next(Platform platform)
        {
            if (platform.Map.IsDiscovered()) return;

            platform.Measure();
            platform.Communicate();

            RegionLimits limits = platform.Map.CalculateLimits(platform.Pose, 1);
            List<Pose> poses = limits.GetPosesWithinLimits();

            // Find closest undiscovered point
            double minVal = Double.PositiveInfinity;
            Pose minPose = platform.Pose;

            int i = 0;
            foreach (Pose p in poses)
            {
                i++;

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

                if (platform.Map.GetPlace(p) == 1)
                {
                    continue;
                }

                double cmin = FindClosestUndiscovered(p, platform);

                if (i == PriorityDirection)
                {
                    cmin = cmin - DirectionWeight;
                }

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

                        if (platform.Map.GetPlace(p) == 0.5)
                        {
                            undiscoverNum++;

                            // calculate how many undiscovered places are around this pose
                            RegionLimits limitsp = platform.Map.CalculateLimits(p, platform.FieldOfViewRadius);
                            List<Pose> neighp = limitsp.GetPosesWithinLimits();
                            int undiscoveredNeigbours = neighp.Count(x => platform.Map.MapMatrix[x.X, x.Y] == 0.5);
                            double currentScoreWithModifiers = currentScore + (1.0 - undiscoveredNeigbours / (double)neighp.Count);
                            currentScore = currentScoreWithModifiers;

                            if (currentScoreWithModifiers < bestScore)
                            {
                                bestScore = currentScoreWithModifiers;
                                bestPose = p;
                            }
                        }

                        // next steps
                        if ((platform.Map.GetPlace(p) !=1))
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
            return "Raster Path Planning Strategy With Priority Direction Controller";
        }
    }
}
