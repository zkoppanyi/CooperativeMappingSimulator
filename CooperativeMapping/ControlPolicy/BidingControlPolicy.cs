﻿using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    [Serializable]
    public class BidingControlPolicy : ControlPolicyAbstract, IDistanceMap, IAllocationMap
    {
        private double[,] distMap;

        // MaxValue = Not assigned
        // [1-n]= Assigned to the ith robot
        private int[,] allocationMap;

        private double minDistMap;
        private double maxDistMap;
        
        private int lastBestDepth = 0;
        public double[,] DistMap { get { return distMap; } }
        public int[,] AllocationMap { get { return allocationMap; } }

        public double MinDistMap { get { return minDistMap; } }
        public double MaxDistMap { get { return maxDistMap; } }

        private const int maxDeep = 100;

        public BidingControlPolicy() : base()
        {

        }

        public override void NextInit(Platform platform)
        {

            // init commandSequence stack if it is null (fix serialization)
            if (allocationMap == null)
            {
                if (GenerateAllocationMap(platform) != 0)
                {
                    commandSequence.Clear();
                    //ReplanLocal(platform, platform.FieldOfViewRadius * 2);
                }
            }
            else
            {
                bool foundUndiscovered = false;
                for (int i = 0; i < allocationMap.Rows(); i++)
                {
                    for (int j = 0; j < allocationMap.Columns(); j++)
                    {
                        Pose p = new Pose(i, j);
                        if ((allocationMap[i, j] == platform.ID) && (!platform.Map.IsPlaceDiscovered(p, platform)))
                        {
                            foundUndiscovered = true;
                            break;
                        }
                    }
                }

                if (!foundUndiscovered)
                {
                    if (GenerateAllocationMap(platform) != 0)
                    {
                        //commandSequence.Clear();
                        //ReplanLocal(platform, platform.FieldOfViewRadius * 2);
                    }
                }
            }
        }

        public override void ReplanLocal(Platform platform, int searchRadius)
        {
            commandSequence.Clear();

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
                while (ptr.ParentNode != null)
                {
                    commandSequence.Push(ptr.Pose);
                    ptr = ptr.ParentNode;
                }
            }
        }

        public override void ReplanGlobal(Platform platform)
        {
            base.ReplanGlobal(platform);

            if (GenerateAllocationMap(platform) != 0)
            {

            }

        }

        private GraphNode FindTrack(Pose startPose, Platform platform, int searchRadius)
        {
            Queue<GraphNode> candidates = new Queue<GraphNode>();
            distMap = Matrix.Create<double>(platform.Map.Rows, platform.Map.Columns, Double.PositiveInfinity);
            GraphNode startNode = new GraphNode(startPose, null, 0, 0);
            candidates.Enqueue(startNode);

            GraphNode bestFronterier = null;
            double bestFronterierScore = Double.PositiveInfinity;

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
                foreach (Pose p in poses)
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

                // 1. Generate map based on the previous actions
                MapObject infoMap = platform.Map;

                infoMap = (MapObject)platform.Map.Clone();
                foreach (Pose p in cp.DiscoverCells)
                {
                    // this is an approximation here
                    infoMap.MapMatrix[p.X, p.Y] = 0;
                }

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

                    // don't override "(distMap[p.X, p.Y] > score)", speed up calculation, this is why this is an approximation algorithm
                    // for this, a correct score/info weighting procedure is needed
                    if (distMap[p.X, p.Y] != Double.PositiveInfinity) continue;


                    // 2. Compute info that is gained by the next step 
                    // this is an approximation here
                    RegionLimits nlimits = platform.Map.CalculateLimits(p.X, p.Y, 1);
                    List<Pose> neighp = nlimits.GetPosesWithinLimits();
                    //double info = neighp.Sum(x => (1 - Math.Abs(infoMap.MapMatrix[x.X, x.Y] - 0.5)) * 2) / 9; // using for-loop instead, cause it's faster

                    // using for-loop instead, cause it's faster
                    double info = 0;
                    for(int i = 0; i < neighp.Count; i++)
                    {
                        info += (1 - Math.Abs(infoMap.MapMatrix[neighp[i].X, neighp[i].Y] - 0.5)) * 2;
                    }
                    info = info / 9;

                    /*List<Tuple<int, Pose>> neighp = platform.CalculateBinsInFOV(p, 2);
                    double info = neighp.Sum(x => (0.5 - Math.Abs(infoMap.MapMatrix[x.Item2.X, x.Item2.Y] - 0.5)) * 2) / 9;*/

                    score = score - info;

                    // we found a solution if it is not discovered yet
                    if ((platform.Map.MapMatrix[p.X, p.Y] > platform.FreeThreshold) && (platform.Map.MapMatrix[p.X, p.Y] < platform.OccupiedThreshold))
                    {
                        fronterierNum++;

                        if (((bestFronterierScore > score) && ((allocationMap[p.X, p.Y] == platform.ID))))
                        {
                            bestFronterier = new GraphNode(p, cp, k, score);
                            bestFronterierScore = score;
                            bestFronterier.DiscoverCells = new List<Pose>(cp.DiscoverCells);
                            foreach (Pose lp in neighp)
                            {
                                bestFronterier.DiscoverCells.Add(lp);
                            }
                            candidates.Enqueue(bestFronterier);

                            // if search radius is -1, then give back the first fronterier that we found, neverthless the score
                            if (searchRadius == -1)
                            {
                                return bestFronterier;
                            }

                            continue;
                        }
                    }

                    // this pose is not occupied and has a higher score than the pervious, so expend it
                    if ((platform.Map.MapMatrix[p.X, p.Y] < platform.OccupiedThreshold) && (distMap[p.X, p.Y] > score))
                    {
                        GraphNode newNode = new GraphNode(p, cp, k, score);
                        newNode.DiscoverCells = new List<Pose>(cp.DiscoverCells);
                        foreach (Pose lp in neighp)
                        {
                            newNode.DiscoverCells.Add(lp);
                        }
                        candidates.Enqueue(newNode);

                        // maintain distance map
                        distMap[p.X, p.Y] = (double)score;
                        if (minDistMap > score) minDistMap = (double)score;
                        if (maxDistMap < score) maxDistMap = (double)score;
                    }
                }
            }

            return bestFronterier;
        }

        private int GenerateAllocationMap(Platform platform)
        {
            allocationMap = Matrix.Create<int>(platform.Map.Rows, platform.Map.Columns, int.MaxValue);
            double[,] valuationMap = Matrix.Create<double>(platform.Map.Rows, platform.Map.Columns, Double.PositiveInfinity);

            /*List<Platform> platforms = new List<Platform>();
            foreach(Platform plt in platform.ObservedPlatforms)
            {
                double d = Math.Sqrt(Math.Pow(plt.Pose.X - platform.Pose.X, 2) + Math.Pow(plt.Pose.Y - platform.Pose.Y, 2));
                if ((d  < platform.FieldOfViewRadius * 1.5) || (d < plt.FieldOfViewRadius * 1.5))
                {
                    platforms.Add(plt);
                }
            }*/

            List<Platform> platforms = platform.ObservedPlatforms;

            platforms.Add(platform);
            platform.SendLog("Generate allocaiton map (" + platforms.Count + ") ...");

            int allocatedCellNum = 0;
            foreach (Platform plt in platforms)
            {

                Pose aroundPose = platform.ControlPolicy.BestFronterier;
                List<Tuple<int, Pose>> cells = platform.CalculateBinsInFOV(plt.Pose, plt.Map, (int)(plt.FieldOfViewRadius * 2));
                //RegionLimits nlimits = platform.Map.CalculateLimits(plt.Pose.X, plt.Pose.Y, platform.FieldOfViewRadius*2);
                //List<Pose> cells = nlimits.GetPosesWithinLimits();

                foreach (Tuple<int, Pose> p in cells)
                {
                    if ((!platform.Map.IsPlaceDiscovered(p.Item2, platform)) && (valuationMap[p.Item2.X, p.Item2.Y] > p.Item1))
                    {
                        if (plt.ID == platform.ID)
                        {
                            allocatedCellNum++;
                        }

                        if (allocationMap[p.Item2.X, p.Item2.Y] == platform.ID)
                        {
                            allocatedCellNum--;
                        }

                        allocationMap[p.Item2.X, p.Item2.Y] = plt.ID;
                        valuationMap[p.Item2.X, p.Item2.Y] = p.Item1;

                    }
                }
            }

            return allocatedCellNum;
        }



        public override string ToString()
        {
            return "Biding Control Policy";
        }

    }
}
