using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    public interface ITrajectory
    {
        Stack<Pose> Trajectory { get; }
        Stack<Pose> CommandSequence { get; }
        Pose BestFronterier { get;  }
    }
}
