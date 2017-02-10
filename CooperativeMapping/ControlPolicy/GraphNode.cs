using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    public class GraphNode
    {
        public Pose Pose { get; set; }
        public GraphNode ParentNode { get; set; }
        public int Depth { get; set; }
        public double Score { get; set; }

        public GraphNode(Pose pose, GraphNode parentNode, int depth, double score)
        {
            this.Pose = pose;
            this.ParentNode = parentNode;
            this.Depth = depth;
            this.Score = score;
        }

    }
}
