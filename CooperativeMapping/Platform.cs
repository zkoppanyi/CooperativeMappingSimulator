using Accord.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    public class PlatformLogEventArgs
    {
        public String Message { get; set; }
        public PlatformLogEventArgs(String msg)
        {
            this.Message = msg;
        }
    }

    public delegate void PlatformLogHandler(object sender, PlatformLogEventArgs e);

    public class Platform
    {
        public MapObject Map { get; set; }
        public Pose Pose { get; set; }
        public int FieldOfViewRadius { get; set; }
        public int ID { get; }
        public Color Color { get; set; }

        public event PlatformLogHandler PlatformLogEvent;

        public int step = 0;
        public int Step { get { return step; } }

        private static int IDs = 0;

        private Enviroment enviroment;

        public Platform(Enviroment enviroment)
        {
            Pose = new Pose();
            Map = new MapObject(enviroment.Map.Rows, enviroment.Map.Columns);
            enviroment.Platforms.Add(this);
            this.enviroment = enviroment;
            FieldOfViewRadius = 2;
            IDs++;
            this.ID = IDs;
            this.Color = Color.Blue;
        }


        /// <summary>
        /// Map update
        /// </summary>
        public void Measure2()
        {
            Map.RemovePlatforms();

            RegionLimits limits = enviroment.Map.CalculateLimits(this.Pose, FieldOfViewRadius);
            for (int i = limits.MinLimitX; i <= limits.MaxLimitX; ++i)
            {
                for (int j = limits.MinLimitY; j <= limits.MaxLimitY; ++j)
                {
                    if (this.Map.MapMatrix[i, j] == (int)MapPlaceIndicator.NoBackVisist)
                    {
                        continue;
                    }

                    int val = enviroment.Map.MapMatrix[i, j];

                    if (val == (int)MapPlaceIndicator.Obstacle)
                    {
                        Map.MapMatrix[i, j] = val;
                    }

                    if (val == (int)MapPlaceIndicator.Platform)
                    {
                        Map.MapMatrix[i, j] = val;
                    }

                    if ((val == (int)MapPlaceIndicator.Undiscovered) || (val == (int)MapPlaceIndicator.Discovered))
                    {
                        Map.MapMatrix[i, j] = (int)MapPlaceIndicator.Discovered;
                    }
                }
            }

            foreach (Platform p in enviroment.Platforms)
            {
                if (Map.GetPlace(p.Pose.X, p.Pose.Y) != MapPlaceIndicator.NoBackVisist)
                {
                    Map.MapMatrix[p.Pose.X, p.Pose.Y] = (int)MapPlaceIndicator.Platform;
                }
            }

        }

        public void Measure()
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            candidates.Add(this.Pose);

            for (int k = 0; k < FieldOfViewRadius; k++)
            {
                newCandidates.Clear();
                foreach (Pose cp in candidates)
                {
                    RegionLimits limits = this.Map.CalculateLimits(cp.X, cp.Y, 1);
                    List<Pose> poses = limits.GetPosesWithinLimits();

                    foreach (Pose p in poses)
                    {
                        if (this.Map.MapMatrix[p.X, p.Y] == (int)MapPlaceIndicator.NoBackVisist)
                        {
                            continue;
                        }

                        if ((p.X == cp.X) && (p.Y == cp.Y)) continue;

                        if (this.Map.GetPlace(p) != MapPlaceIndicator.Obstacle)
                        {
                            newCandidates.Add(p);
                        }

                        int val = enviroment.Map.MapMatrix[p.X, p.Y];

                        if (val == (int)MapPlaceIndicator.Obstacle)
                        {
                            Map.MapMatrix[p.X, p.Y] = val;
                        }

                        if (val == (int)MapPlaceIndicator.Platform)
                        {
                            Map.MapMatrix[p.X, p.Y] = val;
                        }

                        if ((val == (int)MapPlaceIndicator.Undiscovered) || (val == (int)MapPlaceIndicator.Discovered))
                        {
                            Map.MapMatrix[p.X, p.Y] = (int)MapPlaceIndicator.Discovered;
                        }

                    }
                }
                candidates = new List<Pose>(newCandidates);
            }

            foreach (Platform p in enviroment.Platforms)
            {
                if (Map.GetPlace(p.Pose.X, p.Pose.Y) != MapPlaceIndicator.NoBackVisist)
                {
                    Map.MapMatrix[p.Pose.X, p.Pose.Y] = (int)MapPlaceIndicator.Platform;
                }
            }

        }

        public void SendLog(String message)
        {
            PlatformLogEvent?.Invoke(this, new PlatformLogEventArgs(message));
        }

        /// <summary>
        /// Moving control
        /// </summary>
        /// <param name="dx">Displacement along X axis</param>
        /// <param name="dy">Displacement along Y axis</param>
        public void Move(int dx, int dy)
        {
            step++;
            Pose.X = Pose.X + dx;
            Pose.Y = Pose.Y + dy;
        }

    }
}
