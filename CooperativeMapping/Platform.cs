using Accord.Math;
using CooperativeMapping.Communication;
using CooperativeMapping.ControlPolicy;
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

        [ReadOnly(false)]
        [Description("Display color of the platform")]
        [Category("Bins")]
        [DisplayName("Occupied Threshold")]
        public double OccupiedThreshold { get { return 0.9; } }

        [ReadOnly(false)]
        [Description("Display color of the platform")]
        [Category("Bins")]
        [DisplayName("Free Threshold")]
        public double FreeThreshold { get { return 0.3; } }

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
        public ControlPolicyAbstract ControlPolicy { get; set; }

        [Browsable(false)]
        public CommunicationModel CommunicationModel { get; set; }


        public Platform(Enviroment enviroment, ControlPolicy.ControlPolicyAbstract controller, CommunicationModel commModel)
        {
            Pose = new Pose();
            Map = new MapObject(enviroment.Map.Rows, enviroment.Map.Columns);
            enviroment.Platforms.Add(this);
            this.enviroment = enviroment;
            FieldOfViewRadius = 2;
            this.ControlPolicy = controller;
            this.CommunicationModel = commModel;
            IDs++;
            this.ID = IDs;
            this.Color = Color.Blue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var plt = obj as Platform;
            if (plt == null) return false;

            return plt.ID == this.ID;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public void Next()
        {
            ControlPolicy.Next(this);
        }

        public void Measure()
        {
            List<Pose> candidates = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            ObservedPlatforms.Clear();
            candidates.Add(this.Pose);

            List<Tuple<int, Pose>> bins = this.CalculateBinsInFOV(this.Pose);

            foreach (Tuple<int, Pose> tp in bins)
            {
                int k = tp.Item1;
                Pose p = tp.Item2;

                double val_env = enviroment.Map.MapMatrix[p.X, p.Y];
                double val_curr = Map.MapMatrix[p.X, p.Y];
                double val_new = val_curr;
                double val_cand = val_env;

                if (val_env == 0)
                {
                    val_cand = 0.5 * ((double)k / (double)FieldOfViewRadius);
                    //val_cand = 0;
                }
                else if (val_env == 1)
                {
                    val_cand = 1 - 0.5 * ((double)k / (double)FieldOfViewRadius);
                    //val_cand = 1;

                }

                if (Math.Abs(0.5 - val_cand) > Math.Abs(0.5 - val_curr)) // avoid overwrite the better solution
                {
                    val_new = val_cand;
                }

                //double d = Math.Sqrt(Math.Pow(p.X - this.Pose.X, 2) + Math.Pow(p.Y - this.Pose.Y, 2));
                Map.MapMatrix[p.X, p.Y] = val_new;

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

        /// <summary>
        /// Graph search for the bins that is within the FOV limits
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, Pose>> CalculateBinsInFOV(Pose pcent, double FOV = -1)
        {
            if (FOV < 0)
            {
                FOV = this.FieldOfViewRadius;
            }

            List<Pose> candidates = new List<Pose>();
            List<Pose> visited = new List<Pose>();
            List<Pose> newCandidates = new List<Pose>();
            List< Tuple<int, Pose> > ret = new List<Tuple<int, Pose>>();

            candidates.Add(pcent);
            ret.Add(new Tuple<int, Pose>(0, pcent));

            for (int k = 0; k < FOV; k++)
            {
                newCandidates.Clear();
                foreach (Pose cp in candidates)
                {
                    if (visited.Find(p => (p.X == cp.X) && (p.Y == cp.Y)) != null)
                    {
                        continue;
                    }

                    RegionLimits limits = this.Map.CalculateLimits(cp.X, cp.Y, 1);
                    List<Pose> poses = limits.GetPosesWithinLimits();

                    foreach (Pose p in poses)
                    {
                        if ((p.X == cp.X) && (p.Y == cp.Y)) continue;

                        ret.Add(new Tuple<int, Pose>(k, p));
                        if (enviroment.Map.GetPlace(p) < this.OccupiedThreshold)
                        {
                            if (visited.Find(ps => ((ps.X == p.X) && (ps.Y == p.Y))) == null)
                            {
                                visited.Add(cp);
                                newCandidates.Add(p);
                            }
                        }
                    }
                }
                candidates = new List<Pose>(newCandidates);
            }

            return ret;
        }

        /// <summary>
        /// LOS calculation to select bins that is within the FOV limits
        /// </summary>
        /// <returns></returns>
        public List<Tuple<int, Pose>> CalculateBinsInFOVWithLOS(Pose pcent, double FOV = -1)
        {
            if (FOV < 0)
            {
                FOV = this.FieldOfViewRadius;
            }

            List<Tuple<int, Pose>> ret = new List<Tuple<int, Pose>>();
                        

            return ret;
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
            if ((this.Pose.Heading == Utililty.ConvertAngleTo360(Math.Atan2(dy, dx) / Math.PI * 180)) && (Math.Abs(dx) <= 1) && (Math.Abs(dy) <= 1))
            //if ((Math.Abs(dx) <= 1) && (Math.Abs(dy) <= 1))
            {
                step++;
                Pose.X = Pose.X + dx;
                Pose.Y = Pose.Y + dy;
            }
            else
            {
                throw new Exception("Illegal movement.");
            }
        }

        public void Rotate(double dalpha)
        {
            if (Math.Abs(dalpha) == 45)
            {
                step++;
                this.Pose.Heading += dalpha;
            }
            else
            {
                throw new Exception("Illegal movement.");
            }
        }

        public override string ToString()
        {
            return "Platform #" + this.ID;
        }

    }
}
