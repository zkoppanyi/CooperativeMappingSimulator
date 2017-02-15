using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    public static class Logger
    {
        private static StreamWriter logger = null;

        private static bool isOpen = false;
        public static bool IsOpen { get { return isOpen; } }

        public static void Open(String currentEnviromentLocation)
        {
            Close();

            String path = Path.GetDirectoryName(currentEnviromentLocation);
            String name = Path.GetFileNameWithoutExtension(currentEnviromentLocation);
            logger = new StreamWriter(path + "\\" + name + ".csv");
            isOpen = true;
        }

        public static void Log(String str)
        {
            if (isOpen)
            {
                logger.WriteLine(str);
                logger.Flush();
            }
        }

        public static void Close()
        {
            if ((logger != null) && (isOpen))
            {
                logger.Close();
                isOpen = false;
            }
        }
    }
}
