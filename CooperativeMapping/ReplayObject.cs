using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    [Serializable]
    public class ReplayObject
    {
        public String EnviromentPath { get; set; }
        public List<Platform> FinalPlatforms { get; set; }

    }
}
