using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Parser
{
    class Program
    {


        #region Variables
        static int stat = 0;
        static int statYear = 0;
        static int statAuthor = 0;
        static int statPublication = 0;
        static int statTitle = 0;
        static int statParsed = 0;

        static double avgPublicationLength = 0.0;
        static double avgReferenceLength = 0.0;
        static double avgPublicationStart = 0.0;
        static double avgPublicationEnd = 0.0;

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
                Console.WriteLine("Escape characters : " + news);            */
            return news;
        }

        #region FindYearAndAuthor
        /// <summary>
        /// This function is used to find year and author from a string reference
        /// </summary>
        /// <param name="reference">Reference String</param>
        /// <param name="author">Author String which will be filled by this function</param>
        /// <returns>Returns the year from the reference</returns>
        static int FindYearAndAuthor(ref Reference r)
        {
            //Contains all unicode characters followed by 4 digit numbers
            //then again any characters can be present. 
            string pattern = @"([\p{L}\p{N}\p{M}\p{P}\p{Z}\p{S}]*)(\p{Nd}\p{Nd}\p{Nd}\p{Nd})(.*)";
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
                    return 0;
                GroupCollection gc = mc[0].Groups;
                string q = gc[2].Value;

                year = Convert.ToInt32(q);
                //Valid set of years is between 1800 and 2008
                if (year < 1800 || year > 2008)
                {
                    r.ReferenceText = gc[3].Value;
                    flag = true;
                    year = 0;
                }
                else
                {
                    flag = false;
                    r.Authors = (string)gc[1].Value;
                    //Console.WriteLine(r.Authors);
                    //Console.WriteLine("\nYEAR : " + year.ToString());
                    statYear = statYear + 1;
                    statAuthor = statAuthor + 1;
                }
            }
            return year;
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
            //Console.WriteLine("Character : " + sepChar);
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
            //Console.WriteLine("Character : " + sepChar);
        }
        #endregion

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
            r.year = FindYearAndAuthor(ref r);
            //Could not find Year/Author
            if (r.year == 0)
            {
                Console.WriteLine("FAILED : " + reference);
                return r;
            }
            //empty pattern2
            string pattern2 = "";
            MatchCollection mc2;
            string[] referenceWords = reference.Split(Common.seperators);
            int max = 0;
            string matchedPublication = "";
            r.seperatorAfterPublication = -1;
            r.seperatorBeforePublication = -1;
            //Console.WriteLine(r.Reference);
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
                    //Console.WriteLine("Match with " + count + " words.");
                    max = count;
                    matchedPublication = s;
                    string[] ref_matched_pub_words =
                        referencePublicationArrayList.ToArray(typeof(string)) as string[];
                    foreach (string st in ref_matched_pub_words)
                    {
                        //Console.WriteLine(st);
                        r.Publication += " " + st;
                    }
                    if (ref_matched_pub_words.Length != 0)
                    {
                        r.seperatorAfterPublication = FindSeperatorAfterPublication(r.ReferenceText,
                            ref_matched_pub_words[ref_matched_pub_words.Length - 1]);
                        r.seperatorBeforePublication = FindSeperatorBeforePublication(r.ReferenceText,
                            ref_matched_pub_words[0]);
                    }
                    //Console.WriteLine(publication);
                    //Console.WriteLine(matched_pub + " matches " + max.ToString() + " words");
                }
            }

            if (r.seperatorBeforePublication != -1)
            {
                //    Console.WriteLine("Seperator Character before publication : "
                //        + reference[r.seperatorBeforePublication]);
            }
            if (r.seperatorAfterPublication != -1)
            {
                //    Console.WriteLine("Seperator Character after publication : "
                //        + reference[r.seperatorAfterPublication]);
            }

            //Check for a valid publication match. More than 1 word matched
            //Check if the one word match is the only word in the matched publication. Then also 
            //it is a valid publication. 
            if (max >= 2 || (max == 1 && matchedPublication.IndexOf(" ") == -1))
            {
                statPublication = statPublication + 1;
                if (r.Publication.Length < (r.seperatorAfterPublication - r.seperatorBeforePublication))
                {
                    r.Publication = reference.Substring(r.seperatorBeforePublication,
                        r.seperatorAfterPublication - r.seperatorBeforePublication);
                }
            }
            else
            {
                //TODO:Try to find publication by looking for the major and minor seperators. 
                //Also take into account the average publication length for estimating a publication 
                //Apart from this take the average publication starting character and ending 
                //character into account. 

            }

            ArrayList ref_title = new ArrayList();
            int t1 = r.ReferenceText.IndexOf(r.year.ToString()) + r.year.ToString().Length;
            int t2 = 0;
            string[] pub_words = r.Publication.Split(Common.seperators, StringSplitOptions.RemoveEmptyEntries);
            if (pub_words[0].Length >= 2)
                t2 = r.ReferenceText.IndexOf(pub_words[0]);
            else if (pub_words.Length > 2)
                t2 = r.ReferenceText.IndexOf(pub_words[1]);
            r.Title = "";
            if (t2 < t1 || r.Publication.Length < 2)
            {
                //    Console.WriteLine("Erroneous Decoding");
            }
            else
            {
                r.Title = r.ReferenceText.Substring(t1, t2 - t1);
                statTitle = statTitle + 1;
            }
            //Console.WriteLine("Publication : " + r.publication);
            //Console.WriteLine(matchedPublication + " matches " + max.ToString() + " words");
            //Console.WriteLine("TITLE : " + r.Title);
            ////Console.WriteLine(r.Reference);
            //Console.ReadKey();
            if (r.IsValid())
            {
                r.Display();
            }
            return r;
        }
        #endregion

        static void Main(string[] args)
        {
            StreamReader fs = new StreamReader(@"..\data\references.txt");
            //It specifies whether we need to read the new string or not. 
            bool flag = true;
            //Takes in the current value of the reference from the file. 
            string reference = "";
            Reference parsedReference;
            //Store the publications from the Publication file into the string array. 
            StreamReader pubFile = new StreamReader(@"..\data\publications.txt");
            ArrayList arr = new ArrayList();
            while (pubFile.Peek() != -1)
            {
                arr.Add(pubFile.ReadLine());
            }
            publicationData = (string[])arr.ToArray(typeof(string));


            //Main loop for reading the references. 
            while (fs.Peek() != -1)
            {
                if (flag)
                {
                    reference = fs.ReadLine();
                    stat = stat + 1;
                }
                else
                {
                    flag = true;
                }
                parsedReference = ParseReference(reference);
                if (parsedReference.IsValid())
                {
                    //Collect statistics
                    statParsed = statParsed + 1;
                    avgReferenceLength = ((avgReferenceLength * (statParsed - 1)) +
                        parsedReference.ReferenceText.Length) / statParsed;
                    avgPublicationLength = ((avgPublicationLength * (statParsed - 1)) +
                        parsedReference.Publication.Length) / statParsed;
                    avgPublicationStart = ((avgPublicationStart * (statParsed - 1)) +
                        parsedReference.seperatorBeforePublication) / statParsed;
                    avgPublicationEnd = ((avgPublicationEnd * (statParsed - 1)) +
                        parsedReference.seperatorAfterPublication) / statParsed;
                }
                else
                {
                    //TODO: Use Statistics to predict publication and then title. 
                    //Check if author and year are there. If they are not present
                    //then it is hopeless to do so. 
                    if (parsedReference.year == -1 || parsedReference.Authors == String.Empty)
                    {
                        Console.WriteLine("Cannot Predict for this reference");
                    }
                    else
                    {
                        PredictPublication(ref parsedReference);
                        parsedReference.Display();                   
                    }
                }
            }

            DisplayStatistics();
        }

        private static void PredictPublication(ref Reference parsedReference)
        {
            int[] arrSeperators = FindSeperatorsInReference(parsedReference.ReferenceText);
            double difference1 = parsedReference.ReferenceText.Length;
            double difference2 = parsedReference.ReferenceText.Length;
            double temp;
            int seperatorStart = -1;
            int seperatorEnd = -1; 
            foreach (int position in arrSeperators)
            {
                temp = avgPublicationStart - position;
                if (temp < 0)
                    temp = -temp;
                if (temp < difference1)
                {
                    difference1 = temp;
                    seperatorStart = position;
                }
                temp = avgPublicationEnd - position;
                if (temp < 0)
                    temp = -temp;
                if (temp < difference2)
                {
                    difference2 = temp;
                    seperatorEnd = position;
                }
            }
            Console.WriteLine("Pridicted Publication Start Value : " + seperatorStart);
            Console.WriteLine("Predicted Publication End Vale : " + seperatorEnd);
            if (seperatorStart > seperatorEnd)
                return;
            if (seperatorEnd > parsedReference.ReferenceText.Length)
                return;
            parsedReference.Publication = parsedReference.ReferenceText.Substring(seperatorStart, seperatorEnd - seperatorStart);
        }

        private static int[] FindSeperatorsInReference(string p)
        {
            ArrayList arr = new ArrayList();
            for (int i = 0; i < p.Length; i++)
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

        private static void DisplayStatistics()
        {
            Console.WriteLine("STATISTICS");
            Console.WriteLine("Total : " + stat);
            Console.WriteLine("Author : " + statAuthor);
            Console.WriteLine("Year : " + statYear);
            Console.WriteLine("Title : " + statTitle);
            Console.WriteLine("Publication : " + statPublication);
            Console.WriteLine("Avg Reference Length : " + avgReferenceLength);
            Console.WriteLine("Avg Publication Length : " + avgPublicationLength);
            Console.WriteLine("Avg Publication start : " + avgPublicationStart);
            Console.WriteLine("Avg Publication end : " + avgPublicationEnd);
            Console.ReadKey();
        }
    }
}