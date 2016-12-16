using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    public class RasterPathPlanningStrategy : Controller
    {
        public RasterPathPlanningStrategy(Platform platform) : base(platform)
        {

        }

        public override void Next()
        {
            Platform.Measure();

            RegionLimits limits = Platform.Map.CalculateLimits(Platform.Pose, 1);
            List<Pose> poses = limits.GetPosesWithinLimits();

            //Remove those possible poses that are obstacles 
            List<Pose> nextPoses = new List<Pose>();
            foreach (Pose p in poses)
            {
                if ((Platform.Map.GetPlace(p) != MapPlaceIndicator.Obstacle) && (Platform.Map.GetPlace(p) != MapPlaceIndicator.Platform) && (Platform.Map.GetPlace(p) != MapPlaceIndicator.NoBackVisist))
                {
                    nextPoses.Add(p);
                }
            }

            // Find closest undiscovered point
            double minVal = Double.PositiveInfinity;
            Pose minPose = Platform.Pose;

            foreach(Pose p in nextPoses)
            {
                int cmin = FindClosesUndiscovered(p);               

                if (cmin < minVal)
                {
                    minVal = cmin;
                    minPose = p;
                }
            }

            if (minVal == int.MaxValue)
            {
                Platform.SendLog("No undiscovered area!");
            }

            Platform.Move(minPose.X - Platform.Pose.X, minPose.Y - Platform.Pose.Y);
        }

        private int FindClosesUndiscovered(Pose startPose)
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            int[,] distMap = Matrix.Create<int>(Platform.Map.Rows, Platform.Map.Columns, int.MaxValue);
            candidates.Add(startPose);
            int maxDeep = 1000;

            distMap[startPose.X, startPose.Y] = 0;

            for (int k = 1; k < maxDeep; k++)
            {
                newCandidates.Clear();
                foreach (Pose cp in candidates)
                {                    
                    RegionLimits limits = Platform.Map.CalculateLimits(cp.X, cp.Y, 1);
                    List<Pose> poses = limits.GetPosesWithinLimits();
                                       
                    foreach (Pose p in poses)
                    {
                        if ((p.X == cp.X) && (p.Y == cp.Y)) continue;

                        if (Platform.Map.GetPlace(p) == MapPlaceIndicator.Undiscovered)
                        {
                            return k;
                        }

                        //if ((Platform.Map.GetPlace(p) == MapPlaceIndicator.Discovered) || (Platform.Map.GetPlace(p) == MapPlaceIndicator.Platform))
                        if ((Platform.Map.GetPlace(p) != MapPlaceIndicator.Obstacle) && (distMap[p.X, p.Y] == int.MaxValue))
                        {
                            distMap[p.X, p.Y] = k;
                            newCandidates.Add(p);
                        }
                    }
                }
                candidates = new List<Pose>(newCandidates);
            }

            return int.MaxValue;
        }

        // depricated
        private int CalcualteDistanceMap(Pose startPose, Pose endPose)
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            int[,] distMap = Matrix.Create<int>(Platform.Map.Rows, Platform.Map.Columns, int.MaxValue);
            distMap[startPose.X, startPose.Y] = 0;
            candidates.Add(startPose);

            for (int k = 0; k < 100; k++)
            {
                foreach (Pose cp in candidates)
                {
                    int val = distMap[cp.X, cp.Y];
                    RegionLimits limits = Platform.Map.CalculateLimits(cp.X, cp.Y, 1);
                    List<Pose> poses = limits.GetPosesWithinLimits();

                    newCandidates.Clear();
                    foreach (Pose p in poses)
                    {
                        //if ((Platform.Map.GetPlace(p) == MapPlaceIndicator.Discovered) || (Platform.Map.GetPlace(p) == MapPlaceIndicator.Platform))
                        if ((Platform.Map.GetPlace(p) != MapPlaceIndicator.Obstacle))
                        {
                            distMap[p.X, p.Y] = val + 1;
                            newCandidates.Add(p);
                        }

                        if ((endPose.X == p.X) && (endPose.Y == p.Y))
                        {
                            return val + 1;
                        }
                    }
                }
                candidates = new List<Pose>(newCandidates);
            }
            
            return int.MaxValue;
        }
    }
}
