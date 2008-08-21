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
        internal static int CheckForYear(string paragraph, ref int year)
        {
            //Contains all unicode characters followed by 4 digit numbers
            //then again any characters can be present.             
            string pattern = @"(\p{Nd}\p{Nd}\p{Nd}\p{Nd}[^\p{Nd}])";
            MatchCollection mc;
            //Year stored in an integer value. 
            year = 0;
            mc = Regex.Matches(paragraph, pattern);
            if (mc.Count == 0)
            {
                return -1;
            }
            for (int i = 0; i < mc.Count; i++)
            {
                string q = mc[i].Value;
                q = q.Substring(0, q.Length - 1);
                year = Convert.ToInt32(q);
                //Valid set of years is between 1800 and 2008
                if (year > 1800 && year < 2008)
                {
                    return mc[i].Index;
                }
            }
            return -1;
        }


        /// <summary>
        /// Strips seperators from the beginning and end of the string. 
        /// </summary>
        /// <param name="input">Input String</param>
        /// <returns>Output stripped string</returns>
        internal static string Strip(string input)
        {
            int i;
            //Remove seperatos in front
            for (i = 0; i < input.Length; i++)
            {
                bool flag = false;
                foreach (char ch in Common.seperators)
                {
                    if (input[i] == ch)
                        flag = true;
                }
                if (!flag)
                    break;
            }
            string temp = input.Remove(0, i);
            if (temp == String.Empty)
                return String.Empty;
            //Remove seperators at back
            for (i = temp.Length - 1; i > 0; i--)
            {
                bool flag = false;
                foreach (char ch in Common.seperators)
                {
                    if (temp[i] == ch)
                        flag = true;
                }
                if (!flag)
                    break;
            }
            string output = temp.Remove(i + 1, temp.Length - i - 1);
            return output;
        }
    }
}
