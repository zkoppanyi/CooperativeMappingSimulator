using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    [Serializable]
    public class Pose
    {
        public int X { get; set; }
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
