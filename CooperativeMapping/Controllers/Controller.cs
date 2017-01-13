using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping.Controllers
{
    [Serializable]
    public abstract class Controller
    {
        public abstract void Next(Platform platform);
    }
}
