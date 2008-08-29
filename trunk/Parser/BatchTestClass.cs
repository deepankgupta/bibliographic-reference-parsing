using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Parser
{
    class BatchTestClass
    {
        internal static void Start()
        {
            string directory = Common.inputFilePath;
            if (!Directory.Exists(directory))
                return;
            foreach (string file in Directory.GetFiles(directory))
            {
                Common.inputFilePath = file;
                Common.Init();
                Program.Start();
            }
        }
    }
}
