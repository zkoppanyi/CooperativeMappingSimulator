using Accord.Math;
using EpPathFinding.cs;
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

        private int lastBestDepth = 0;

        public double[,] DistMap { get { return distMap; } }
        public double MinDistMap { get { return minDistMap; } }
        public double MaxDistMap { get { return maxDistMap; } }

        private const int maxDeep = 100;

        public ClosestFronterierControlPolicy()
        {

        }

        public override void NextInit(Platform platform)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform">Platform object</param>
        /// <param name="searchRadius">Serach radius of the local path planner, if it is -1 then don't worry about the manuever constraints</param>
        public override void ReplanLocal(Platform platform, int searchRadius)
        {
            // Find closest undiscovered point
            GraphNode res = FindTrack(platform.Pose, platform, searchRadius);

            if (res == null)
            {
                commandSequence.Clear();
                return;
            }

            double minVal = res.Score;
            GraphNode bestTraj = res;
            bestFronterier = res.Pose;
            lastBestDepth = res.Depth;

            //Convert graph nodes to control commands
            GraphNode ptr = bestTraj;
            if (ptr != null) // if no solution this is null
            {
                commandSequence = new Stack<Pose>();
                //trajectory.Push(ptr.Pose);
                while (ptr.ParentNode != null)
                {
                    commandSequence.Push(ptr.Pose);
                    ptr = ptr.ParentNode;
                }
            }
        }

        private GraphNode FindTrack(Pose startPose, Platform platform, int searchRadius)
        {
            Queue<GraphNode> candidates = new Queue<GraphNode>();
            distMap = Matrix.Create<double>(platform.Map.Rows, platform.Map.Columns, Double.PositiveInfinity);
            candidates.Enqueue(new GraphNode(startPose, null, 0, 0));

            GraphNode bestFronterier = null;
            int fronterierNum = 0;
            distMap[startPose.X, startPose.Y] = 0;
            minDistMap = 0;
            maxDistMap = 0;

            // calculate safe zones around platforms
            List<Pose> safeZone = new List<Pose>();
            foreach (Platform plt in platform.ObservedPlatforms)
            {
                RegionLimits limits = platform.Map.CalculateLimits(plt.Pose.X, plt.Pose.Y, 1);
                List<Pose> poses = limits.GetPosesWithinLimits();
                foreach(Pose p in poses)
                {
                    safeZone.Add(p);
                }
            }

            //graph search
            while (candidates.Count != 0)
            {
                GraphNode cp = candidates.Dequeue();

                int k = cp.Depth + 1;
                if ((k > searchRadius) && (searchRadius != -1)) break;
                if (k > maxDeep) break;

                RegionLimits limits = platform.Map.CalculateLimits(cp.Pose.X, cp.Pose.Y, 1);
                List<Pose> poses = limits.GetPosesWithinLimits();

                foreach (Pose p in poses)
                {
                    // is there any other platform on this bin?
                    if (safeZone.Exists(pt => pt.Equals(p)))
                    {
                        continue;
                    }

                    double score = cp.Score + 1;

                    double dalpha = Math.Abs(p.GetHeadingTo(cp.Pose)) / 45.0;
                    score = score + dalpha;

                    // we found a solution if it is not discovered yet
                    if ((platform.Map.MapMatrix[p.X, p.Y] > platform.FreeThreshold) && (platform.Map.MapMatrix[p.X, p.Y] < platform.OccupiedThreshold))
                    {
                        fronterierNum++;

                        if ((bestFronterier == null) || (bestFronterier.Score > score))
                        {
                            bestFronterier = new GraphNode(p, cp, k, score);
                            candidates.Enqueue(bestFronterier);

                            // if search radius is -1, then give back the first fronterier that we found, neverthless the score
                            if (searchRadius == -1)
                            {
                                return bestFronterier;
                            }

                        }
                    }

                    // this pose is not occupied and has a higher score than the pervious, so expend it
                    if ((platform.Map.MapMatrix[p.X, p.Y] < platform.OccupiedThreshold) && (distMap[p.X, p.Y] > score))
                    {
                        candidates.Enqueue(new GraphNode(p, cp, k, score));

                        // maintain distance map
                        distMap[p.X, p.Y] = (double)score;
                        if (minDistMap > score) minDistMap = (double)score;
                        if (maxDistMap < score) maxDistMap = (double)score;
                    }
                }
            }

            return bestFronterier;
        }           

        public override string ToString()
        {
            return "Closest Fronterier Control Policy";
        }
    }
}
