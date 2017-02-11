using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    [Serializable]
    public class ClosestFronterierControlPolicy : ControlPolicyAbstract, IDistanceMap, ITrajectory
    {
        private double[,] distMap;
        private double minDistMap;
        private double maxDistMap;
        private Pose prevPose;
        private Stack<Pose> trajectory;
        private Pose bestFronterier;
        private int lastBestDepth = 0;
        private double prevFronterierValue = 0;

        private Stack<Pose> commandSequence;
        public Stack<Pose> CommandSequence { get { return commandSequence; }  }

        public double[,] DistMap { get { return distMap; } }
        public double MinDistMap { get { return minDistMap; } }
        public double MaxDistMap { get { return maxDistMap; } }

        public Stack<Pose> Trajectory { get { return trajectory; } }
        public Pose BestFronterier { get { return bestFronterier; } }

        private const int maxDeep = 100;

        public ClosestFronterierControlPolicy()
        {

        }

        public override void Next(Platform platform)
        {
            platform.Measure();
            platform.Communicate();

            // init breadCumbers stack if it is null (fix serialization)
            if (trajectory == null)
            {
                trajectory = new Stack<Pose>();
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

            if ((commandSequence!= null) && (commandSequence.Count > 0))
            {
                nextPose = commandSequence.Pop();

                // ok, on the next step, there is another guy, plan the track again
                if (platform.ObservedPlatforms.Find(pt => nextPose.Equals(pt.Pose) ) != null)
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
                if (platform.Map.MapMatrix[bestFronterier.X, bestFronterier.Y] != prevFronterierValue)
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
                    trajectory.Push(prevPose);
                }
                prevPose = new Pose(nextPose.X, nextPose.Y, nextPose.Heading);

                // do action
                double dx = nextPose.X - platform.Pose.X;
                double dy = nextPose.Y - platform.Pose.Y;
                double goalAlpha = Utililty.ConvertAngleTo360(Math.Atan2(dy, dx) / Math.PI * 180);

                // choose the angle that is closer to the target heading
                double dalpha1 = Utililty.ConvertAngleTo360(goalAlpha  - platform.Pose.Heading);
                double dalpha2 = Utililty.ConvertAngleTo360((platform.Pose.Heading - goalAlpha) + 360.0);
                double dalpha = Math.Abs(dalpha1) < Math.Abs(dalpha2) ? dalpha1 : -dalpha2;

                if (dalpha == 0)
                {
                    prevFronterierValue = platform.Map.MapMatrix[bestFronterier.X, bestFronterier.Y];
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
