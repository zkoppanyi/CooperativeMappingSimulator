using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    public abstract class Controller
    {
        public abstract void Next();
        public Platform Platform { get; set; }

        public Controller(Platform platform)
        {
            this.Platform = platform;
        }

    }
}
