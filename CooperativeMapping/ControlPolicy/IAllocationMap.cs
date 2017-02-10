using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.ControlPolicy
{
    public interface IAllocationMap
    {
        int[,] AllocationMap { get; }
    }
}
