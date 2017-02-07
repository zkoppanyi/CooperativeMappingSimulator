using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    [Serializable]
    public class MaxInformationGainControlPolicy : ControlPolicyAbstract, IDistanceMap
    {
        public Stack<Pose> CumberBreads;
        private Pose prevPose;
        private double[,] distMap;
        private double minDistMap;
        private double maxDistMap;
        public Pose MinPose;
        public Pose BestFronterier;

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


        public MaxInformationGainControlPolicy()
        {
            CumberBreads = new Stack<Pose>(100);
        }

        public override void Next(Platform platform)
        {
            platform.Measure();
            platform.Communicate();

            if (platform.Map.IsDiscovered(platform)) return;

            if (this.CumberBreads == null)
            {
                this.CumberBreads = new Stack<Pose>(100);
            }

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

            Tuple<double, Pose> result = FindNextStep(platform, nextPoses, (int)((double)platform.FieldOfViewRadius*1.2));
            double maxVal = result.Item1;
            MinPose = result.Item2;

            bool extensiveSearchIsNeeded = false;
            if (this.CumberBreads.Count > 0)
            {
                if (maxVal == Double.NegativeInfinity)
                {
                    //platform.SendLog("No undiscovered area!");
                    MinPose = this.CumberBreads.Pop();

                    Pose res = nextPoses.Find(p => (p.X == MinPose.X) && (p.Y == MinPose.Y));

                    if (res != null)
                    {
                        platform.Move(MinPose.X - platform.Pose.X, MinPose.Y - platform.Pose.Y);
                    }
                    else
                    {
                        extensiveSearchIsNeeded = true;
                    }

                }
                else
                {
                    if (prevPose != null)
                    {
                        this.CumberBreads.Push(prevPose);
                    }
                    prevPose = new Pose(MinPose.X, MinPose.Y);
                    platform.Move(MinPose.X - platform.Pose.X, MinPose.Y - platform.Pose.Y);
                }
            }
            else
            {
                extensiveSearchIsNeeded = true;
            }


            if (extensiveSearchIsNeeded)
            {
                result = FindNextStep(platform, nextPoses, 100);
                maxVal = result.Item1;
                Pose minPose = result.Item2;

                if (maxVal == Double.NegativeInfinity)
                {
                    platform.SendLog("No undiscovered area!");
                }
                else
                {
                    if (prevPose != null)
                    {
                        this.CumberBreads.Push(prevPose);
                    }
                    prevPose = new Pose(minPose.X, minPose.Y);
                    platform.Move(minPose.X - platform.Pose.X, minPose.Y - platform.Pose.Y);
                }
            }
        }

        private Tuple<double, Pose> FindNextStep(Platform platform, List<Pose> nextPoses, int maxDeep)
        {
            // Find closest undiscovered point
            double maxVal = Double.NegativeInfinity;
            Pose minPose = platform.Pose;
            foreach (Pose p in nextPoses)
            {
                Tuple<double, Pose> res = FindClosestFrontrierWithInformationGain(p, platform, maxDeep);
                double cmax = res.Item1;

                /*double r = 0;
                foreach (Platform plt in platform.ObservedPlatforms)
                {
                    if ((plt.Pose.X != p.X) && (plt.Pose.Y != p.Y))
                    {
                         r += Math.Sqrt(Math.Pow((p.X - plt.Pose.X), 2) + Math.Pow((p.Y - plt.Pose.Y), 2));
                    }
                }*/
                //cmax -= r / platform.ObservedPlatforms.Count;

                if (cmax > maxVal)
                {
                    maxVal = cmax;
                    minPose = p;
                    BestFronterier = res.Item2;
                }
            }

            FindClosestFrontrierWithInformationGain(minPose, platform, maxDeep);
            return new Tuple<double, Pose>(maxVal, minPose);
        }

        private Tuple<double, Pose> FindClosestFrontrierWithInformationGain(Pose startPose, Platform platform, int maxDeep)
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            distMap = Matrix.Create<double>(platform.Map.Rows, platform.Map.Columns, Double.NegativeInfinity);
            candidates.Add(startPose);

            double bestScore = Double.NegativeInfinity;
            Pose bestPose = null;
            int undiscoverNum = 0;

            distMap[startPose.X, startPose.Y] = 0;
            minDistMap = 0;
            maxDistMap = 0;

            int k = 1;
            for (k = 1; k < maxDeep; k++)
            {

                if (candidates.Count == 0)
                {
                    break;
                }

                if ((undiscoverNum > 0) && (maxDeep > platform.FieldOfViewRadius*2))
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

                        RegionLimits nlimits = platform.Map.CalculateLimits(cp.X, cp.Y, 0);
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
                        if ((distMap[p.X, p.Y] < newScore) && (platform.Map.MapMatrix[p.X, p.Y] <= platform.OccupiedThreshold))
                        {
                            distMap[p.X, p.Y] = newScore;
                            newCandidates.Add(p);

                            if (minDistMap > newScore) minDistMap = newScore;
                            if (maxDistMap < newScore) maxDistMap = newScore;
                        }
                    }
                }
                candidates = new List<Pose>(newCandidates);
            }

            return new Tuple<double, Pose>(bestScore, bestPose);
        }
        public override string ToString()
        {
            return "Maximum Information Gain";
        }

    }
}
