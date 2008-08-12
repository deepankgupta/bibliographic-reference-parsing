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
        public static string inputFilePath = @"..\data\paper.xml";
        public static string referenceFilePath = @"..\data\references.txt";
        public static string outputFilePath = @"..\data\output.txt";
        internal static void Init()
        {
            fw = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
            sw = new StreamWriter(fw, Encoding.Unicode);
        }
    }
}
