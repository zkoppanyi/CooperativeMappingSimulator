using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class Pose
    {
        [ReadOnly(false)]
        [Description("X coordinate of the pose")]
        [DisplayName("X")]
        public int X { get; set; }

        [ReadOnly(false)]
        [Description("Y coordinate of the pose")]
        [DisplayName("Y")]
        public int Y { get; set; }

        [ReadOnly(false)]
        [Description("Heading of the platform")]
        [DisplayName("Heading")]
        public double Heading
        {

            get
            {
                return heading;
            }

            set
            {
                heading = Utililty.ConvertAngleTo360(value);
            }

        }
        private double heading;

        public Pose(int x = 0, int y = 0, double heading = 0) 
        {
            this.X = x;
            this.Y = y;
            this.Heading = heading;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var pose = obj as Pose;
            if (pose == null) return false;

            return (pose.X == this.X) && (pose.Y == this.Y);
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double GetHeadingTo(Pose goalPose)
        {
            double dxp = goalPose.X - this.X;
            double dyp = goalPose.Y - this.Y;
            return Math.Atan2(dyp, dxp) / Math.PI * 180.0;
        }

        public Pose MoveOne()
        {
            double dx = Math.Cos(this.Heading / 180.0 * Math.PI);
            double dy = Math.Sin(this.Heading / 180.0 * Math.PI);
            return new Pose(this.X + (int)dx, this.Y + (int)dy, this.Heading);
        }

    }
}
