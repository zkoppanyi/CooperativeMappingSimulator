using Accord.Math;
using CooperativeMapping.Controllers;
using System;
using System.Collections.Generic;
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

    [Serializable]
    public class Platform
    {
        public MapObject Map { get; set; }
        public Pose Pose { get; set; }
        public int FieldOfViewRadius { get; set; }
        public int ID { get; }
        public Controller Controller { get; }

        public List<Platform> ObservedPlatforms = new List<Platform>();

        [field: NonSerialized]
        public event PlatformLogHandler PlatformLogEvent;

        private static int IDs = 0;

        private Enviroment enviroment;

        public Platform(Enviroment enviroment, Controller controller)
        {
            Pose = new Pose();
            Map = new MapObject(enviroment.Map.Rows, enviroment.Map.Columns);
            enviroment.Platforms.Add(this);
            this.enviroment = enviroment;
            this.Controller = controller;
            FieldOfViewRadius = 5;
            IDs++;
            this.ID = IDs;
        }

        public void Next()
        {
            Controller.Next(this);
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

            // add platforms to the map
            ObservedPlatforms.Clear();
            foreach (Platform p in enviroment.Platforms)
            {
                ObservedPlatforms.Add(p);
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
            Pose.X = Pose.X + dx;
            Pose.Y = Pose.Y + dy;
        }

        public override string ToString()
        {
            return "Platform " + this.ID + " Map " + this.Map.ID;
        }

    }
}
