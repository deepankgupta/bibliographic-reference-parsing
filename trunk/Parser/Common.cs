using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Parser
{
    class Common
    {
        //The list of seperators on which the words will be seperated in a reference. 
        public static char[] seperators = { ' ', ',', '.', ';', ':', '\t', '\n', '?', '(', ')',
                                       '{', '}', '[', ']', '+', '\n', '\r' };
        private static FileStream fw;
        public static StreamWriter sw;
        
        internal static void Init()
        {
            fw = new FileStream("output.txt", FileMode.Create, FileAccess.Write);
            sw = new StreamWriter(fw, Encoding.Unicode);
        }
    }
}
