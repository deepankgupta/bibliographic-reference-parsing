using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace DataCollector
{
    class Program
    {
        private static string[] GetAttachments()
        {
            ArrayList arr = new ArrayList();
            string destLocation = "C:\\";
            if (Directory.Exists(destLocation))
            {
                if (File.Exists(destLocation + "results.log"))
                {
                    //this will be true when some GUI Testing commands were 
                    //given and there results were stored in a result.log file
                    //in the FLATRELEASEDIR
                    foreach (string s in Directory.GetFiles(destLocation))
                    {
                        if (s.IndexOf("results") != -1 && s.IndexOf("log") != -1)
                        {
                            string assembly = GetAssemblyFromResultLog(s);
                            if (assembly == null)
                                continue;
                            if (ParseResultLog(s) == false)
                            {

                                Console.WriteLine("BVT Testing of the assembly : "
                                    + assembly + " was unsuccesful. Attaching log");
                                arr.Add(s);
                            }
                            else
                            {
                                Console.WriteLine("BVT Testing of the assembly : "
                                    + assembly + " was successful. ");
                            }
                        }
                    }
                }
            }
            string[] strArray = arr.ToArray(typeof(string)) as string[];
            return strArray;
        }

        private static string GetAssemblyFromResultLog(string s)
        {
            StreamReader fs = new StreamReader(s);
            string str;
            string assembly;
            while (fs.Peek() != -1)
            {
                str = fs.ReadLine();
                if (str.IndexOf("assembly = ") != -1)
                {
                    assembly = str.Substring(str.IndexOf("assembly = ") + 11);
                    return assembly;
                }
            }
            return null;
        }

        private static bool ParseResultLog(string s)
        {
            StreamReader fs = new StreamReader(s);
            string str;
            string pattern = @"(.*)(\p{Nd}+)(.*)";
            int passed = 0, failed = 0, aborted = 0, skipped = 0;
            MatchCollection mc;
            while (fs.Peek() != -1)
            {
                str = fs.ReadLine();
                if (str.IndexOf("Aborted:") != -1)
                {
                    mc = Regex.Matches(str, pattern);
                    if (mc.Count != 0)
                    {
                        GroupCollection gc = mc[0].Groups;
                        aborted = Convert.ToInt32((string)gc[2].Value);
                    }
                }
                if (str.IndexOf("Passed:") != -1)
                {
                    mc = Regex.Matches(str, pattern);
                    if (mc.Count != 0)
                    {
                        GroupCollection gc = mc[0].Groups;
                        string temp = (string)gc[2].Value;
                        passed = Convert.ToInt32(temp);
                    }
                }
                if (str.IndexOf("Failed:") != -1)
                {
                    mc = Regex.Matches(str, pattern);
                    if (mc.Count != 0)
                    {
                        GroupCollection gc = mc[0].Groups;
                        failed = Convert.ToInt32((string)gc[2].Value);
                    }
                }
                if (str.IndexOf("Skipped:") != -1)
                {
                    mc = Regex.Matches(str, pattern);
                    if (mc.Count != 0)
                    {
                        GroupCollection gc = mc[0].Groups;
                        skipped = Convert.ToInt32((string)gc[2].Value);
                    }
                }
            }
            if (aborted > 0 || failed > 0 || skipped > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        static void Main(string[] args)
        {

            string[] str = GetAttachments();
            foreach (string s in str)
                Console.WriteLine(s);
            Console.WriteLine(ParseResultLog("C:\\results.log"));
            Console.WriteLine(GetAssemblyFromResultLog("C:\\results.log"));
            Console.ReadKey();
        }
    }
}
