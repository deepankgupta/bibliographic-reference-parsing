using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Parser
{
    class Program
    {
        #region Variables
        /// <summary>
        /// It is used to store the publications string array from the publication text file. 
        /// </summary>
        static string[] publicationData;
        #endregion

        private static string AddEscapeChars(string s)
        {
            string news = "";
            news = s.Replace(".", "\\.");
            news = news.Replace("[", "\\[");
            news = news.Replace("]", "\\]");
            news = news.Replace("{", "\\{");
            news = news.Replace("}", "\\}");
            news = news.Replace("(", "\\(");
            news = news.Replace(")", "\\)");
            news = s.Replace("]", "\\]");
            /*if (news != s)
                Common.sw.WriteLine("Escape characters : " + news);            */
            return news;
        }

        #region FindYearAndAuthor
        /// <summary>
        /// This function is used to find year and author from a string reference
        /// </summary>
        /// <param name="reference">Reference String</param>
        /// <param name="author">Author String which will be filled by this function</param>
        /// <returns>Returns the year from the reference</returns>
        static bool FindYearAndAuthor(ref Reference r)
        {
            //Contains all unicode characters followed by 4 digit numbers
            //then again any characters can be present.             
            string pattern = @"(\p{Nd}\p{Nd}\p{Nd}\p{Nd})";
            MatchCollection mc;

            //Flag is set to true for all the wrong numbers caught and thought of as an year. 
            bool flag = true;

            //Year stored in an integer value. 
            int year = 0;

            //Keep on working with the same reference till we do not get a valid year.
            while (flag && r.ReferenceText.Length > 0)
            {
                mc = Regex.Matches(r.ReferenceText, pattern);
                if (mc.Count == 0)
                {
                    Common.sw.WriteLine("FAILED : " + r.ReferenceText);
                    return false;
                }
                GroupCollection gc = mc[0].Groups;
                string q = gc[1].Value; 
                

                year = Convert.ToInt32(q);
                //Valid set of years is between 1800 and 2008
                if (year < 1800 || year > 2008)
                {                    
                    r.ReferenceText = r.ReferenceText.Substring(gc[1].Index + gc[1].Length);
                    flag = true;
                    year = 0;
                }
                else
                {
                    flag = false;
                    r.Authors = r.ReferenceText.Substring(0, gc[1].Index);
                    r.yearEnd = gc[1].Index + gc[1].Length;
                    //Common.sw.WriteLine(r.Authors);
                    //Common.sw.WriteLine("\nYEAR : " + year.ToString());
                    Statistics.UpdateYearAuthor();                    
                }
            }
            r.year = year;
            if (year != 0)
                return true;
            else
            {
                Common.sw.WriteLine("FAILED : " + r.ReferenceText);
                return false;
            }
        }
        #endregion

        #region FindSeperators
        /// <summary>
        /// This function finds the seperator character before the publication. 
        /// </summary>
        /// <param name="reference">Reference String</param>
        /// <param name="firstWord">The first word of the matched publication. </param>
        /// <returns>The seperator character</returns>
        private static int FindSeperatorBeforePublication(string reference, string firstWord)
        {
            int min = reference.IndexOf(firstWord);
            int ans = -1, i = min;
            char sepChar = ' ';
            foreach (char ch in Common.seperators)
            {
                if (ch == ' ')
                    continue;
                for (i = min; i > 0; i--)
                {
                    if (reference[i] == ch)
                        break;
                }
                if (i > ans)
                {
                    ans = i;
                    sepChar = ch;
                }
            }
            return ans;
            //Common.sw.WriteLine("Character : " + sepChar);
        }


        /// <summary>
        /// Find the seperator character after the publication
        /// </summary>
        /// <param name="reference">Reference String</param>
        /// <param name="lastWord">Last word of the matched publication</param>
        /// <returns>The seperator character after the publication. </returns>
        private static int FindSeperatorAfterPublication(string reference, string lastWord)
        {
            int i = reference.IndexOf(lastWord) + lastWord.Length;
            int min = reference.Length;
            int temp;
            char sepChar = ' ';
            foreach (char ch in Common.seperators)
            {
                if (ch == ' ')
                    continue;
                temp = reference.IndexOf(ch, i);
                if (temp != -1 && temp < min)
                {
                    min = temp;
                    sepChar = ch;
                }
            }
            return min;
            //Common.sw.WriteLine("Character : " + sepChar);
        }
        #endregion

        #region FindPublication
        /// <summary>
        /// This function is used to get the publication from the reference. 
        /// </summary>
        /// <param name="r">The reference object is passed by reference as parameter. </param>
        static void FindPublication(ref Reference r)
        {
            //empty pattern2
            string pattern2 = "";
            MatchCollection mc2;
            string[] referenceWords = r.ReferenceText.Substring(r.yearEnd).Split(Common.seperators);
            int max = 0;
            string matchedPublication = "";
            r.seperatorAfterPublication = -1;
            r.seperatorBeforePublication = -1;
            //Common.sw.WriteLine(r.Reference);
            //For each string s in publication string, we do the following. 
            foreach (string s in publicationData)
            {
                int count = 0, tempCount = 0;
                ArrayList referencePublicationArrayList = new ArrayList();
                ArrayList temp = new ArrayList();
                foreach (string word in referenceWords)
                {
                    //We do not consider a word of length 2 or less to be no match word. 
                    if (word.Length > 2)
                    {
                        pattern2 = "(.*)" + word + "(.*)";
                        mc2 = Regex.Matches(s, word);
                        //Match found in publication and the word of the reference. 
                        if (mc2.Count != 0)
                        {
                            //TODO:We need to take in the redundant words from the file. 
                            if (word.Equals("the", StringComparison.InvariantCultureIgnoreCase) ||
                                                            word.Equals("from", StringComparison.InvariantCultureIgnoreCase) ||
                                                            word.Equals("for", StringComparison.InvariantCultureIgnoreCase) ||
                                                            word.Equals("and", StringComparison.InvariantCultureIgnoreCase))
                            {
                            }
                            else
                            {
                                tempCount++;
                                temp.Add(word);
                            }
                        }
                        else
                        {
                            //Remove the match found till now. Since, we are looking for a contigous match. 
                            tempCount = 0;
                            if (temp.Count != 0)
                            {
                                for (int i = temp.Count - 1; i >= 0; i--)
                                {
                                    temp.RemoveAt(i);
                                }
                            }
                        }
                        //If the count of the words matched in this publication is more than the previous one, 
                        //replace this with the previous publication as the matched publication. 
                        if (tempCount > count)
                        {
                            count = tempCount;
                            referencePublicationArrayList = (ArrayList)temp.Clone();
                        }
                    }
                }
                if (count >= max)
                {
                    r.Publication = "";
                    //Common.sw.WriteLine("Match with " + count + " words.");
                    max = count;
                    matchedPublication = s;
                    string[] ref_matched_pub_words =
                        referencePublicationArrayList.ToArray(typeof(string)) as string[];
                    foreach (string st in ref_matched_pub_words)
                    {
                        //Common.sw.WriteLine(st);
                        r.Publication += " " + st;
                    }
                    if (ref_matched_pub_words.Length != 0)
                    {
                        r.seperatorAfterPublication = FindSeperatorAfterPublication(r.ReferenceText,
                            ref_matched_pub_words[ref_matched_pub_words.Length - 1]);
                        r.seperatorBeforePublication = FindSeperatorBeforePublication(r.ReferenceText,
                            ref_matched_pub_words[0]);
                    }
                    //Common.sw.WriteLine(publication);
                    //Common.sw.WriteLine(matched_pub + " matches " + max.ToString() + " words");
                }
            }
            if (r.Publication.Length < (r.seperatorAfterPublication - r.seperatorBeforePublication))
            {
                r.Publication = r.ReferenceText.Substring(r.seperatorBeforePublication,
                    r.seperatorAfterPublication - r.seperatorBeforePublication);
            }
            //Check for a valid publication match. More than 1 word matched
            //Check if the one word match is the only word in the matched publication. Then also 
            //it is a valid publication. 
            if (max >= 2 || (max == 1 && matchedPublication.IndexOf(" ") == -1))
            {
                Statistics.UpdatePublication();                
            }

        }
        private static void PredictPublication(ref Reference parsedReference)
        {
            int[] arrSeperators = FindSeperatorsInReference(parsedReference.ReferenceText, parsedReference.yearEnd);
            double difference1 = parsedReference.ReferenceText.Length;
            double difference2 = parsedReference.ReferenceText.Length;
            double temp;
            int seperatorStart = -1;
            int seperatorEnd = -1;
            double thisPublicationStart = Statistics.avgPublicationStart / Statistics.avgReferenceLength
                * parsedReference.ReferenceText.Length;
            double thisPublicationEnd = Statistics.avgPublicationEnd / Statistics.avgReferenceLength
                * parsedReference.ReferenceText.Length;
            if (arrSeperators.Length < 4)
            {
                //SPECIAL CASE : This means that there is no publication and only title and we will 
                //make title as the previously predicted publication. 
                parsedReference.Title = parsedReference.Publication;
                parsedReference.Publication = "";
                return;
            }
            else
            {
                foreach (int position in arrSeperators)
                {
                    temp = thisPublicationStart - position;
                    if (temp < 0)
                        temp = -temp;
                    if (temp < difference1)
                    {
                        difference1 = temp;
                        seperatorStart = position;
                        continue;
                    }
                    temp = thisPublicationEnd - position;
                    if (temp < 0)
                        temp = -temp;
                    if (temp < difference2)
                    {
                        difference2 = temp;
                        seperatorEnd = position;
                    }
                }
            }
            Common.sw.WriteLine("Pridicted Publication Start Value : " + seperatorStart);
            Common.sw.WriteLine("Predicted Publication End Value : " + seperatorEnd);
            if (seperatorStart > seperatorEnd)
                return;
            if (seperatorEnd > parsedReference.ReferenceText.Length)
                return;
            if (seperatorEnd == -1 || seperatorStart == -1)
                return;
            parsedReference.Publication = parsedReference.ReferenceText.Substring(seperatorStart,
                seperatorEnd - seperatorStart);
            parsedReference.Title = parsedReference.ReferenceText.Substring(parsedReference.yearEnd,
                seperatorStart - parsedReference.yearEnd);
        }
        #endregion

        #region FindTitle
        /// <summary>
        /// This function is used to find the title of the reference. 
        /// </summary>
        /// <param name="r">The reference object is passed by reference. </param>
        static void FindTitle(ref Reference r)
        {
            ArrayList ref_title = new ArrayList();
            int t1 = r.ReferenceText.IndexOf(r.year.ToString()) + r.year.ToString().Length;
            int t2 = 0;
            string[] pub_words = r.Publication.Split(Common.seperators, StringSplitOptions.RemoveEmptyEntries);
            if (pub_words.Length == 0)
            {
                r.Title = r.ReferenceText.Substring(r.yearEnd);
                return;
            }
            if (pub_words[0].Length >= 2)
                t2 = r.ReferenceText.IndexOf(pub_words[0]);
            else if (pub_words.Length > 2)
                t2 = r.ReferenceText.IndexOf(pub_words[1]);
            r.Title = "";
            if (t2 < t1 || r.Publication.Length < 2)
            {
                //    Common.sw.WriteLine("Erroneous Decoding");
            }
            else
            {
                r.Title = r.ReferenceText.Substring(t1, t2 - t1);
                Statistics.UpdateTitle();
            }

        }
        #endregion

        #region FindPageNumbers
        /// <summary>
        /// This function is used to find the page numbers in a reference. 
        /// </summary>
        /// <param name="r">The reference object is passed as a parameter by reference. </param>
        private static void FindPageNumbers(ref Reference r)
        {
            string input = r.ReferenceText.Substring(r.yearEnd);
            string pattern = @"\p{Nd}+-\p{Nd}+";
            Regex re = new Regex(pattern);
            MatchCollection mc = re.Matches(input);
            if (mc.Count > 0)
            {
                Match pageNo = mc[mc.Count - 1];
                r.PageNos = pageNo.Value;
                return;
            }
            pattern = @"\bpp.\b|\bp.\b|\bpg.\b|\bpp\b|\bp\b|\bpg\b";
            Regex re2 = new Regex(pattern);
            mc = re2.Matches(input);
            if (mc.Count > 0)
            {
                Match pageNo = mc[mc.Count - 1];
                int index = pageNo.Index;
                //TODO:Refine this furthur
                r.PageNos = r.ReferenceText.Substring(index);
                return;
            }
        }
        #endregion

        private static int[] FindSeperatorsInReference(string p, int index)
        {
            ArrayList arr = new ArrayList();
            for (int i = index; i < p.Length; i++)
            {
                for (int j = 1; j < Common.seperators.Length; j++)
                {
                    if (p[i] == Common.seperators[j])
                    {
                        arr.Add(i);
                    }
                }
            }
            return arr.ToArray(typeof(int)) as int[];
        }

        #region ReferenceParser
        /// <summary>
        /// This function is used to parse a reference into Author, Year, Title, Publication fields. 
        /// </summary>
        /// <param name="reference">Reference String</param>
        /// <returns>True or false based on the success/failure of the process. </returns>
        static Reference ParseReference(string reference)
        {
            //String used to store the author name. 
            Reference r = new Reference(reference);
            r.ReferenceText = reference;
            if (!FindYearAndAuthor(ref r))
            {
                //TODO:Maybe it is not a reference. Do something with this informaiton in the Reference
                //identification block code. 
            }                        
            FindPageNumbers(ref r);
            FindPublication(ref r);
            FindTitle(ref r);
            if (r.IsValid())
            {
                r.Display();
            }
            return r;
        }

        
        #endregion
        
        /// <summary>
        /// Store the publications from the Publication file into the string array. 
        /// </summary>
        static private void GetPublicationData()
        {
            StreamReader pubFile = new StreamReader(@"..\data\publications.txt");
            ArrayList arr = new ArrayList();
            while (pubFile.Peek() != -1)
            {
                arr.Add(pubFile.ReadLine());
            }
            publicationData = (string[])arr.ToArray(typeof(string));
        }

        static void Main(string[] args)
        {
            Common.Init();
            string filePath = @"..\data\paper.xml";
            string referenceFilePath = @"..\data\references.txt";
            ReferenceExtractor refExt = new ReferenceExtractor(filePath, referenceFilePath);
            refExt.Main();
            StreamReader fs = new StreamReader(referenceFilePath, Encoding.Unicode);
            //It specifies whether we need to read the new string or not. 
            //Takes in the current value of the reference from the file. 
            string reference = "";
            Reference parsedReference;

            GetPublicationData();

            //Main loop for reading the references. 
            while (fs.Peek() != -1)
            {
                reference = fs.ReadLine();
                Statistics.UpdateReference();
                parsedReference = ParseReference(reference);
                Statistics.UpdateStatistics(parsedReference);
                if(!parsedReference.IsValid())
                {
                    //Use Statistics to predict publication and then title. 
                    //Check if author and year are there. If they are not present
                    //then it is hopeless to do so. 
                    if (parsedReference.year == -1 || parsedReference.Authors == String.Empty)
                    {
                        Common.sw.WriteLine("Cannot Predict for this reference");
                    }
                    else
                    {
                        PredictPublication(ref parsedReference);
                        parsedReference.Display();                   
                    }
                }
            }

            Statistics.DisplayStatistics();
            Common.sw.Close();
            Process p = new Process();
            ProcessStartInfo pInfo = new ProcessStartInfo(@"c:\windows\System32\notepad.exe", "output.txt");
            p.StartInfo = pInfo;
            p.Start();
        }

        
    }
}