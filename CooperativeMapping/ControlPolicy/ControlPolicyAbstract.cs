using EpPathFinding.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    public enum SolutionType
    {
        GlobalPlanner,
        LocalPlanner
    }

    [Serializable]
    public abstract class ControlPolicyAbstract
    {

        protected Pose prevPose;
        protected double prevFronterierValue = 0;

        protected Stack<Pose> commandSequence;
        public Stack<Pose> CommandSequence { get { return commandSequence; } }

        protected Stack<Pose> trajectory;
        public Stack<Pose> Trajectory { get { return trajectory; } }

        protected Pose bestFronterier;
        public Pose BestFronterier { get { return bestFronterier; } }

        protected bool hasFeasablePath;
        public bool HasFeasablePath { get { return hasFeasablePath; } }

        protected SolutionType solutionType;
        public SolutionType SolutionType { get; }

        public ControlPolicyAbstract()
        {
            commandSequence = new Stack<Pose>();
            trajectory = new Stack<Pose>();
        }

        public abstract void ReplanLocal(Platform platform, int searchRadius);
        public abstract void NextInit(Platform platform);

        public void Next(Platform platform)
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

            this.NextInit(platform);

            // if robot is stopped then return
            if (platform.IsStopped) return;

            // if the map is discovered we are done
            if (platform.Map.IsDiscovered(platform))
            {
                platform.Stop();
                return;
            }

            Pose nextPose = null;
            if (!(platform.ControlPolicy is ReplayPolicy))
            {
                // get the next pose and decide whether a replan is needed                
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
                    if ((bestFronterier != null) && (platform.Map.MapMatrix[bestFronterier.X, bestFronterier.Y] != prevFronterierValue))
                    {
                        isReplan = true;
                    }

                }
                else
                {
                    isReplan = true;
                }

                //isReplan = true;

                // *** need to replan
                if (isReplan)
                {
                    // this is the search radius around the platform
                    // if it is -1, then look for the first fronterier and don't worry about the manuevers
                    int rad = (int)((double)platform.FieldOfViewRadius * 1.5);
                    ReplanLocal(platform, rad);

                    // get the next pose, if it is available
                    if (commandSequence.Count > 0)
                    {
                        nextPose = commandSequence.Pop();
                        solutionType = SolutionType.LocalPlanner;
                    }

                    if ((nextPose == null) || (commandSequence.Count == 0))
                    {
                        ReplanGlobal(platform);

                        if (commandSequence.Count > 0)
                        {
                            nextPose = commandSequence.Pop();
                            solutionType = SolutionType.GlobalPlanner;
                        }
                    }
                }
            }
            else
            {
                nextPose = commandSequence.Pop();
            }

            // *** need to replan
            if ((platform.ControlPolicy is ReplayPolicy) || ((nextPose != null) && (platform.Map.MapMatrix[nextPose.X, nextPose.Y] < platform.OccupiedThreshold) && ((platform.ObservedPlatforms.Find(pt => nextPose.Equals(pt.Pose)) == null))))
            {
                // maintain breadcumbers
                prevPose = new Pose(nextPose.X, nextPose.Y, nextPose.Heading);

                // do action
                double dx = nextPose.X - platform.Pose.X;
                double dy = nextPose.Y - platform.Pose.Y;
                int goalAlpha = Utililty.ConvertAngleTo360(Math.Atan2(dy, dx) / Math.PI * 180);

                // choose the angle that is closer to the target heading
                int dalpha1 = Utililty.ConvertAngleTo360(goalAlpha - platform.Pose.Heading);
                int dalpha2 = Utililty.ConvertAngleTo360((platform.Pose.Heading - goalAlpha) + 360.0);
                double dalpha = Math.Abs(dalpha1) < Math.Abs(dalpha2) ? dalpha1 : -dalpha2;

                if (dalpha == 0)
                {
                   if (bestFronterier != null)
                    {
                        prevFronterierValue = platform.Map.MapMatrix[bestFronterier.X, bestFronterier.Y];
                    }

                    trajectory.Push(nextPose);
                    platform.Move((int)dx, (int)dy);
                }
                else // rotatation is needed, let's rotate
                {

                    double rot = Math.Sign(dalpha) * 45;
                    platform.Rotate(rot);
                    commandSequence.Push(nextPose);
                }

                hasFeasablePath = true;
            }
            else
            {
                platform.SendLog("No feasible solution");
                hasFeasablePath = false;
                commandSequence.Clear();
            }

        }

        public virtual void ReplanGlobal(Platform platform)
        {
            // create searchGrid data structure for the EpPathFinding
            BaseGrid searchGrid = new StaticGrid(platform.Map.Rows, platform.Map.Columns);
            List<Tuple<double, GridPos>> searchPoses = new List<Tuple<double, GridPos>> ();
            for (int i = 0; i < platform.Map.Rows; i++)
            {
                for (int j = 0; j < platform.Map.Columns; j++)
                {
                    if (platform.Map.MapMatrix[i, j] < platform.OccupiedThreshold)
                    {
                        searchGrid.SetWalkableAt(i, j, true);
                    }

                    if ((platform.Map.MapMatrix[i, j] >= platform.FreeThreshold) && (platform.Map.MapMatrix[i, j] <= platform.OccupiedThreshold))
                    {
                        RegionLimits limits = platform.Map.CalculateLimits(i, j, 1);
                        List<Pose> posesl = limits.GetPosesWithinLimits();
                        foreach (Pose p in posesl)
                        {
                            if (platform.Map.MapMatrix[p.X, p.Y] < platform.FreeThreshold)
                            {
                                double d = Math.Sqrt(Math.Pow(p.X - platform.Pose.X, 2) + Math.Pow(p.Y - platform.Pose.Y, 2));
                                searchPoses.Add(new Tuple<double, GridPos>(d, new GridPos(i, j)));
                                break;
                            }
                        }
                    }
                }
            }

            foreach (Platform plt in platform.ObservedPlatforms)
            {
                if (plt.Equals(platform)) continue;
                RegionLimits limits = platform.Map.CalculateLimits(plt.Pose.X, plt.Pose.Y, 1);
                List<Pose> posesl = limits.GetPosesWithinLimits();
                foreach (Pose p in posesl)
                {
                    searchGrid.SetWalkableAt(p.X, p.Y, false);
                }
            }

            // bound the search to avoid large computation
            int maxNumOfSearchPoses = 50;
            searchPoses.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
            if (searchPoses.Count > maxNumOfSearchPoses)
            {
                searchPoses.RemoveRange(maxNumOfSearchPoses, searchPoses.Count - maxNumOfSearchPoses);
            }

            // init search
            GridPos startPos = new GridPos(platform.Pose.X, platform.Pose.Y);
            GridPos endPos = new GridPos(20, 10);
            JumpPointParam jpParam = new JumpPointParam(searchGrid, startPos, endPos, false, true, true);

            // find the best path
            double bestPathScore = Double.PositiveInfinity;
            List<GridPos> bestPath = null;
            foreach (Tuple<double, GridPos> p in searchPoses)
            {
                //jpParam.Reset(startPos, p);
                jpParam.Reset(new GridPos(platform.Pose.X, platform.Pose.Y), p.Item2);
                List<GridPos> resultPathList = JumpPointFinder.FindPath(jpParam);

                if (resultPathList.Count > 2)
                {
                    double score = 0;
                    for (int i = 1; i < resultPathList.Count; i++)
                    {
                        score += Math.Sqrt(Math.Pow(resultPathList[i].x - resultPathList[i - 1].x, 2) + Math.Pow(resultPathList[i].y - resultPathList[i - 1].y, 2));
                    }

                    if (score < bestPathScore)
                    {
                        bestPathScore = score;
                        bestPath = resultPathList;
                        bestFronterier = new Pose(resultPathList.Last().x, resultPathList.Last().y);
                    }
                }
            }


            // convert the best path to command sequence
            if ((bestPath != null) && (bestPath.Count > 2))
            {
                List<Pose> bestPathConv = new List<Pose>();
                bestPathConv.Add(platform.Pose);

                for (int i = 1; i < bestPath.Count; i++)
                {
                    Pose prevPose = bestPathConv.Last();
                    Pose goalPose = new Pose(bestPath[i].x, bestPath[i].y);

                    int dxl = Math.Sign(goalPose.X - prevPose.X);
                    int dyl = Math.Sign(goalPose.Y - prevPose.Y);

                    while (!prevPose.Equals(goalPose)) // it's a bit dangerous here
                    {
                        Pose newPose = new Pose(prevPose.X + dxl, prevPose.Y + dyl);
                        prevPose = newPose;
                        bestPathConv.Add(newPose);
                    }
                }

                for (int i = bestPathConv.Count - 2; i > 0; i--)
                {
                    int dx = bestPathConv[i + 1].X - bestPathConv[i].X;
                    int dy = bestPathConv[i + 1].Y - bestPathConv[i].Y;
                    double dalpha = Math.Atan2(dy, dx);

                    Pose newPose = new Pose(bestPathConv[i].X, bestPathConv[i].Y, dalpha);
                    commandSequence.Push(newPose);
                }
            }
        }

    }
}
