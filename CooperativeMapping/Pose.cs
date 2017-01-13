using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public Pose()
        {
            X = 0;
            Y = 0;
        }

        public Pose(int x, int y) 
        {
            this.X = x;
            this.Y = y;
        }
    }
}
