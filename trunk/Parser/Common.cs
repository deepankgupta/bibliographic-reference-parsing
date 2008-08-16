using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;

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
        public static string[] paragraphs;
        internal static void Init()
        {
            fw = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
            sw = new StreamWriter(fw, Encoding.Unicode);
        }

        /// <summary>
        /// This function is used to check if the current paragraph contains an year in it or not. 
        /// </summary>
        /// <param name="paragraph">The strong specifying the paragraph</param>
        /// <returns>True or False</returns>
        public static int CheckForYear(string paragraph)
        {
            //Contains all unicode characters followed by 4 digit numbers
            //then again any characters can be present.             
            string pattern = @"(\p{Nd}\p{Nd}\p{Nd}\p{Nd})";
            MatchCollection mc;
            //Year stored in an integer value. 
            int year = 0;
            mc = Regex.Matches(paragraph, pattern);
            if (mc.Count == 0)
            {
                return -1;
            }
            for (int i = 0; i < mc.Count; i++)
            {
                string q = mc[i].Value;
                year = Convert.ToInt32(q);
                //Valid set of years is between 1800 and 2008
                if (year > 1800 && year < 2008)
                {
                    return mc[i].Index;
                }
            }
            return -1;
        }

    }
}
