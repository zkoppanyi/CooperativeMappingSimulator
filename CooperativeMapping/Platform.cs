using Accord.Math;
using CooperativeMapping.Communication;
using CooperativeMapping.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    [Serializable]
    public class Platform
    {
            
        [ReadOnly(false)]
        [Description("Pose of the platform")]
        [Category("Platform")]
        [DisplayName("Pose")]
        public Pose Pose { get; set; }

        [ReadOnly(false)]
        [Description("Radius of field of view")]
        [Category("Field of view")]
        [DisplayName("Radius")]
        public int FieldOfViewRadius { get; set; }

        [ReadOnly(true)]
        [Description("Unique ID of the platform")]
        [Category("Platform")]
        [DisplayName("ID")]
        public int ID { get; }

        [ReadOnly(false)]
        [Description("Display color of the platform")]
        [Category("Display")]
        [DisplayName("Color")]
        public Color Color { get; set; }

        [Browsable(false)]
        public MapObject Map { get; set; }

        [Browsable(false)]
        public List<Platform> ObservedPlatforms = new List<Platform>();

        [field: NonSerialized]
        public event PlatformLogHandler PlatformLogEvent;

        [ReadOnly(true)]
        public int Step { get { return step; } }
        private int step = 0;

        private static int IDs = 0;

        private Enviroment enviroment;

        [Browsable(false)]
        public Controller Controller { get; set; }

        [Browsable(false)]
        public CommunicationModel CommunicationModel { get; set; }


        public Platform(Enviroment enviroment, Controller controller, CommunicationModel commModel)
        {
            Pose = new Pose();
            Map = new MapObject(enviroment.Map.Rows, enviroment.Map.Columns);
            enviroment.Platforms.Add(this);
            this.enviroment = enviroment;
            FieldOfViewRadius = 2;
            this.Controller = controller;
            this.CommunicationModel = commModel;
            IDs++;
            this.ID = IDs;
            this.Color = Color.Blue;
        }

        public void Next()
        {
            Controller.Next(this);
        }

        public void Measure()
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            ObservedPlatforms.Clear();
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

                        foreach (Platform pe in enviroment.Platforms)
                        {
                            if ((pe.Pose.X == p.X) && (pe.Pose.Y == p.Y))
                            {
                                if (!ObservedPlatforms.Exists(x => x == pe))
                                {
                                    ObservedPlatforms.Add(pe);
                                }
                            }
                        }

                    }
                }
                candidates = new List<Pose>(newCandidates);
            }
        }

        public void Communicate()
        {
            CommunicationModel.Acquire(this, this.enviroment);
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

        public override string ToString()
        {
            return "Platform #" + this.ID;
        }

    }
}
