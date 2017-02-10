using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    [Serializable]
    public class MaxInformationGainControlPolicy : ControlPolicyAbstract, IDistanceMap, IBreadCumbers
    {
        private double[,] distMap;
        private double minDistMap;
        private double maxDistMap;
        private Pose prevPose;
        private Stack<Pose> breadCumbers;
        private Pose bestFronterier;
        private int lastBestDepth = 0;
        private double prevFronterierValue = 0;

        private Stack<Pose> commandSequence;

        public double[,] DistMap { get { return distMap; } }
        public double MinDistMap { get { return minDistMap; } }
        public double MaxDistMap { get { return maxDistMap; } }

        public Stack<Pose> BreadCumbers { get { return breadCumbers; } }
        public Pose BestFronterier { get { return bestFronterier; } }

        private const int maxDeep = 100;

        public MaxInformationGainControlPolicy()
        {

        }

        public override void Next(Platform platform)
        {
            platform.Measure();
            platform.Communicate();

            // init breadCumbers stack if it is null (fix serialization)
            if (breadCumbers == null)
            {
                breadCumbers = new Stack<Pose>(100);
            }

            // init commandSequence stack if it is null (fix serialization)
            if (commandSequence == null)
            {
                commandSequence = new Stack<Pose>();
            }

            // if the map is discovered we are done
            if (platform.Map.IsDiscovered(platform)) return;

            // get the next pose
            Pose nextPose = null;
            bool isReplan = false;

            if ((commandSequence != null) && (commandSequence.Count > 0))
            {
                nextPose = commandSequence.Pop();

                // ok, on the next step, there is another guy, plan the track again
                if (platform.ObservedPlatforms.Find(pt => nextPose.Equals(pt.Pose)) != null)
                {
                    isReplan = true;
                }

                // ok, next step would be a wall, let's plan the track again
                if (platform.Map.MapMatrix[nextPose.X, nextPose.Y] >= platform.OccupiedThreshold)
                {
                    isReplan = true;
                }

                // if the goal is not changed, then keep the track
                // this is good, if needed time to plan the way back from an abonden area
                if (platform.Map.GetPlace(bestFronterier) != prevFronterierValue)
                {
                    isReplan = true;
                }

            }
            else
            {
                isReplan = true;
            }

            //isReplan = true;

            // need to replan
            if (isReplan)
            {
                // this is the search radius around the platform
                // if it is -1, then look for the first fronterier and don't worry about the manuevers
                //int[] searchRadiusList = new int[] { (int)((double)lastBestDepth * 1.5), (int)((double)platform.FieldOfViewRadius * 1.2), -1 };
                int rad = Math.Min((int)((double)lastBestDepth * 1.5), (int)((double)platform.FieldOfViewRadius * 1.5));
                int[] searchRadiusList = new int[] { rad, -1 };
                foreach (int searchRadius in searchRadiusList)
                {
                    Replan(platform, searchRadius);

                    // get the next pose
                    if (commandSequence.Count > 0)
                    {
                        nextPose = commandSequence.Pop();
                        break;
                    }
                }
            }

            if (nextPose != null)
            {
                // maintain breadcumbers
                if (prevPose != null)
                {
                    breadCumbers.Push(prevPose);
                }
                prevPose = new Pose(nextPose.X, nextPose.Y, nextPose.Heading);

                // do action
                double dx = nextPose.X - platform.Pose.X;
                double dy = nextPose.Y - platform.Pose.Y;
                double goalAlpha = Utililty.ConvertAngleTo360(Math.Atan2(dy, dx) / Math.PI * 180);

                // choose the angle that is closer to the target heading
                double dalpha1 = Utililty.ConvertAngleTo360(goalAlpha - platform.Pose.Heading);
                double dalpha2 = Utililty.ConvertAngleTo360((platform.Pose.Heading - goalAlpha) + 360.0);
                double dalpha = Math.Abs(dalpha1) < Math.Abs(dalpha2) ? dalpha1 : -dalpha2;

                if (dalpha == 0)
                {
                    prevFronterierValue = platform.Map.GetPlace(bestFronterier);
                    platform.Move((int)dx, (int)dy);
                }
                else // rotatation is needed, let's rotate
                {

                    double rot = Math.Sign(dalpha) * 45;
                    platform.Rotate(rot);
                    commandSequence.Push(nextPose);
                }
            }
            else
            {
                platform.SendLog("No feasible solution");
            }

        }

        public void Replan(Platform platform, int searchRadius)
        {
            //List<Pose> nextPoses = GetNextPossiblePoses(platform);

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
                    double score = cp.Score + 1;

                    double dalpha = Math.Abs(p.GetHeadingTo(cp.Pose)) / 45.0;
                    score = score + dalpha;

                    // don't override "(distMap[p.X, p.Y] > score)", speed up calculation, this is why this is an approximation algorithm
                    // for this, a correct score/info weighting procedure is needed
                    if (distMap[p.X, p.Y] != Double.PositiveInfinity) continue;
                                                            

                    // 2. Compute info that is gained by the next step 
                    // this is an approximation here
                    RegionLimits nlimits = platform.Map.CalculateLimits(p.X, p.Y, 4);
                    List<Pose> neighp = nlimits.GetPosesWithinLimits();
                    double info = neighp.Sum(x => 0.5 - Math.Abs(infoMap.MapMatrix[x.X, x.Y] - 0.5)) / neighp.Count;

                    /*List<Tuple<int, Pose>> neighp = platform.CalculateBinsInFOV(p, 2);
                    double info = neighp.Sum(x => (0.5 - Math.Abs(infoMap.MapMatrix[x.Item2.X, x.Item2.Y] - 0.5)) * 2) / 9;*/
                
                    score = score - info*2;


                    // is there any other platform on this bin?
                    if (platform.ObservedPlatforms.Find(pt => pt.Pose.Equals(p)) != null)
                    {
                        continue;
                    }

                    // we found a solution if it is not discovered yet
                    if ((platform.Map.GetPlace(p) > platform.FreeThreshold) && (platform.Map.GetPlace(p) < platform.OccupiedThreshold))
                    {
                        fronterierNum++;

                        if ((bestFronterier == null) || (bestFronterier.Score > score))
                        {
                            bestFronterier = new GraphNode(p, cp, k, score);
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
                    if ((platform.Map.GetPlace(p) < platform.OccupiedThreshold) && (distMap[p.X, p.Y] > score))
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

        public override string ToString()
        {
            return "Maximum Information Gain Control Policy";
        }

    }
}
