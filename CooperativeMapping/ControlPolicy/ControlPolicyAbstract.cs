using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    [Serializable]
    public abstract class ControlPolicyAbstract
    {

        public ControlPolicyAbstract()
        {
            commandSequence = new Stack<Pose>();
            trajectory = new Stack<Pose>();
        }

        public abstract void Next(Platform platform);

        protected Stack<Pose> commandSequence;
        public Stack<Pose> CommandSequence { get { return commandSequence; } }

        protected Stack<Pose> trajectory;
        public Stack<Pose> Trajectory { get { return trajectory; } }

        protected Pose bestFronterier;
        public Pose BestFronterier { get { return bestFronterier; } }

    }
}
