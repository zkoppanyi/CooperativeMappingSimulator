using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    interface IDistanceMap
    {
        double[,] DistMap { get; }
        double MinDistMap { get; }
        double MaxDistMap { get; }

    }
}
