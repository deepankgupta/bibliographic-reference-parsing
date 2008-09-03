//    This file is part of bibliographic-reference-parsing. 
//    Bibliographic-Reference-Parsing is free software; you can redistribute it
//    and/or modify it under the terms of the GNU General Public License as 
//    published by the Free Software Foundation; either version 3 of the License,
//    or (at your option) any later version.

//    Bibliographic-Reference-Parsing is distributed in the hope that it will be 
//    useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//    
//    Author : Deepank Gupta  (deepankgupta AT gmail DOT com)
//    Date   : 18/08/08

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;

namespace Parser
{
    /// <summary>
    /// This is the main class containing all the 4 stages of the program. 
    /// </summary>
    class Program
    {
        #region Variables
        /// <summary>
        /// It is used to store the publications string array from the publication text file. 
        /// </summary>
        static string[] publicationData;
        /// <summary>
        /// This is used to store the references array extracted from reference.txt file. 
        /// </summary>
        static Reference[] references;
        /// <summary>
        /// Pointer to file : reference.xml
        /// </summary>
        static XmlCreator referenceXml;
        /// <summary>
        /// Pointer to file : statistics.xml
        /// </summary>
        static XmlCreator statisticsXml;
        /// <summary>
        /// Pointer to file : citation.xml
        /// </summary>
        static XmlCreator citationXml;
        /// <summary>
        /// Instance of the GUI Class. 
        /// </summary>
        static InputForm form;
        /// <summary>
        /// This stream is used to read the reference.txt file. 
        /// </summary>
        static StreamReader referenceFileStream;

        #endregion

        #region Stage1
        /// <summary>
        /// This is the initialisation and Reference Block identification stage. 
        /// </summary>
        internal static void Stage1()
        {
            referenceXml = new XmlCreator("reference.xml", "References");
            statisticsXml = new XmlCreator("statistics.xml", "Statistics");
            citationXml = new XmlCreator("citations.xml", "Citations");
            ReferenceExtractor refExt = new ReferenceExtractor(Common.inputFilePath, Common.referenceFilePath);
            refExt.Main();
        }
        #endregion

        #region Stage2
        #region GetPublicationData
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
        #endregion

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
            string pattern = @"(\p{Nd}\p{Nd}\p{Nd}\p{Nd}[^\p{Nd}])";
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
                q = q.Substring(0, q.Length - 1);

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
        /// Find punctuations which might be used as seperators in references. 
        /// </summary>
        /// <param name="p">Paragraph string</param>
        /// <param name="index">Index at which to start</param>
        /// <returns>Int array of indexes with seperators</returns>
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
        static bool FindPublication(ref Reference r)
        {
            if (r == null || r.ReferenceText == String.Empty)
                return false;
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
            if (r.Publication == null)
            {
                r.Publication = string.Empty;
                return false;
            }
            if (r.Publication.Length < (r.seperatorAfterPublication - r.seperatorBeforePublication))
            {
                bool changed = false;
                if (r.ReferenceText[r.seperatorBeforePublication] == '.')
                {
                    int prevSeperatorStart = r.seperatorBeforePublication;
                    string tempStr = r.ReferenceText.Substring(r.yearEnd + 2);
                    int offset = r.yearEnd + 2;
                    int i = tempStr.IndexOf('.');
                    if (i == 0)
                    {
                        tempStr = tempStr.Substring(1);
                        i = tempStr.IndexOf('.');
                        offset++;
                    }
                    if (i < 5 || tempStr[i - 2] == ' ')
                    {
                        if (i >= 5)
                        {
                        }
                        tempStr = tempStr.Substring(i + 1);
                        offset = offset + i + 1;
                        i = tempStr.IndexOf('.');
                    }
                    r.seperatorBeforePublication = offset + i;
                    if (r.seperatorBeforePublication != prevSeperatorStart)
                    {
                        string afterPublicationText = r.ReferenceText.Substring(r.seperatorBeforePublication);
                        int lastIndex;
                        //Find a number/seperator.
                        string pattern = @"(\p{Nd})";
                        MatchCollection mc;
                        mc = Regex.Matches(afterPublicationText, pattern);
                        if (mc.Count == 0)
                        {
                            r.Publication = r.ReferenceText.Substring(r.seperatorBeforePublication);
                        }
                        else
                        {
                            lastIndex = mc[0].Index + r.seperatorBeforePublication;
                            r.Publication = r.ReferenceText.Substring(r.seperatorBeforePublication, lastIndex
                                - r.seperatorBeforePublication);
                        }
                        changed = true;
                    }
                }
                if (!changed)
                {
                    r.Publication = r.ReferenceText.Substring(r.seperatorBeforePublication,
                        r.seperatorAfterPublication - r.seperatorBeforePublication);
                }
            }
            //Check for a valid publication match. More than 1 word matched
            //Check if the one word match is the only word in the matched publication. Then also 
            //it is a valid publication. 
            if (max >= 2 || (max == 1 && matchedPublication.IndexOf(" ") == -1))
            {
                Statistics.UpdatePublication();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This function is called upon by FindPublication in case it is unable
        /// to find publication using Domain Knowledge and prediction means have
        /// be applied in order to aid the publication matching. 
        /// </summary>
        /// <param name="parsedReference">Reference to the reference object. </param>
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
                parsedReference.InterchangeTitlePublication();
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
            Common.sw.WriteLine("Predicted Publication Start Value : " + seperatorStart);
            Common.sw.WriteLine("Predicted Publication End Value : " + seperatorEnd);
            if (seperatorStart > seperatorEnd)
                return;
            if (seperatorEnd > parsedReference.ReferenceText.Length)
                return;
            if (seperatorEnd == -1 || seperatorStart == -1)
                return;
            if (parsedReference.ReferenceText[seperatorStart] == '.')
            {
                int prevSeperatorStart = seperatorStart;
                string tempStr = parsedReference.ReferenceText.Substring(parsedReference.yearEnd + 2);
                int offset = parsedReference.yearEnd + 2;
                int i = tempStr.IndexOf('.');
                if (i == 0)
                {
                    tempStr = tempStr.Substring(1);
                    i = tempStr.IndexOf('.');
                    offset++;
                }
                if (i < 5 || tempStr[i - 2] == ' ')
                {
                    tempStr = tempStr.Substring(i);
                    offset += i;
                    i = tempStr.IndexOf('.');
                }
                seperatorStart = offset + i;
                if (seperatorStart != prevSeperatorStart)
                {

                }
            }
            Statistics.UpdatePredictedPublication();
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
            if (r.ReferenceText == string.Empty)
                return;
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
            if (r.ReferenceText == String.Empty || r.yearEnd == -1)
                return;
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

        #region ReferenceParser
        /// <summary>
        /// This function is used to parse a reference into Author, Year, Title, Publication fields. 
        /// </summary>
        /// <param name="reference">Reference String</param>
        /// <returns>True or false based on the success/failure of the process. </returns>
        static Reference ParseReference(string reference, int index)
        {
            //String used to store the author name. 
            Reference r = new Reference(reference, Common.referenceStartOffsets[index], Common.referenceEndOffsets[index]);
            r.ReferenceText = reference;
            if (!FindYearAndAuthor(ref r))
            {
                //TODO:Maybe it is not a reference. Do something with this informaiton in the Reference
                //identification block code. 
                return null;
            }
            FindPageNumbers(ref r);
            FindPublication(ref r);
            FindTitle(ref r);
            if (r.IsPredictionNeeded())
            {
                r.Display(referenceXml);
            }
            return r;
        }
        #endregion
        
        /// <summary>
        /// This function governs the stage 2 where the references are parsed 
        /// and stored in the references array.
        /// </summary>
        internal static void Stage2()
        {
            ArrayList referenceList = new ArrayList();
            referenceFileStream = new StreamReader(Common.referenceFilePath,
                Encoding.Unicode);
            //It specifies whether we need to read the new string or not. 
            //Takes in the current value of the reference from the file. 
            string reference = "";
            Reference parsedReference;

            GetPublicationData();
            int ind = 0;
            //Main loop for reading the references. 
            while (referenceFileStream.Peek() != -1)
            {
                reference = referenceFileStream.ReadLine();
                Statistics.UpdateReference();
                parsedReference = ParseReference(reference, ind);
                if (parsedReference == null)
                {
                    Statistics.wronglyPredictedReference++;
                    continue;
                }
                Statistics.UpdateStatistics(parsedReference);
                if (!parsedReference.IsPredictionNeeded())
                {
                    parsedReference.toBePredicted = true;
                }
                referenceList.Add(parsedReference);
                ind++;
            }
            references = referenceList.ToArray(typeof(Reference)) as Reference[];            
        }
#endregion

        #region Stage3
        #region CitationExtractor
        /// <summary>
        /// This function is used to find and extract a citation from the paragraph. 
        /// </summary>
        /// <param name="paragraph">String of paragraph</param>
        static private int ExtractCitation(string paragraph, long offset)
        {
            string[] stopwords = { "in", "since", "during", "until", "before" };
            if (paragraph == null || paragraph == String.Empty)
                return -1;
            int year = -1;
            int index = Common.CheckForYear(paragraph, ref year);
            int yearEndIndex = index + year.ToString().Length;
            if (index == -1)
                return -1;
            int newIndex = index;
            string subString = paragraph.Substring(0, index);
            string[] strs = subString.Split(Common.seperators, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length == 0)
                return yearEndIndex;
            //Check for stopword
            foreach (string stopword in stopwords)
            {
                if (stopword.Equals(strs[strs.Length - 1], StringComparison.CurrentCultureIgnoreCase))
                {
                    return yearEndIndex;
                }
            }
            int i;
            //Check for Author. 
            for (i = strs.Length - 1; i > -1; i--)
            {
                string stringToCheck = strs[i];
                //Check for Genetive, if present remove it and check. 
                if (stringToCheck.Length > 2 && stringToCheck[stringToCheck.Length - 1] == 's' &&
                    stringToCheck[stringToCheck.Length - 2] == '\'')
                {
                    stringToCheck = stringToCheck.Substring(0, stringToCheck.Length - 2);
                }
                bool flag = false;
                foreach (Reference reference in references)
                {
                    if (reference.Authors.IndexOf(stringToCheck, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        int ind = reference.Authors.IndexOf(stringToCheck, StringComparison.CurrentCultureIgnoreCase);
                        if (ind == 0)
                        {
                            flag = true;
                            break;
                        }
                        char tempChar = reference.Authors[ind - 1];
                        foreach (char ch in Common.seperators)
                        {
                            if (ch == tempChar)
                                flag = true;
                        }
                        if (flag)
                            break;
                    }
                }
                if (flag == false)
                    break;
            }
            //Put the citation in
            if (i == strs.Length - 1)
                return yearEndIndex;
            i = i + 1;
            int temp = subString.LastIndexOf(strs[i]);
            Citation citation = new Citation();
            citation.Name = paragraph.Substring(temp, index - temp);
            citation.Year = year;
            //Note that the 4 has been kept to include the year also in this. 
            citation.Paragraph = paragraph.Substring(0, yearEndIndex);
            citation.Offset = offset + yearEndIndex;
            citation.Display(citationXml);
            //Common.sw.WriteLine("Citation : " + citation.Name + " " + citation.Year.ToString());
            return yearEndIndex;
        }
        #endregion

        /// <summary>
        /// This stage deals with the extraction of citations from the 
        /// rest of the document matching it with the references obtained 
        /// from the document. 
        /// </summary>
        internal static void Stage3()
        {
            for (int i = 0; i < references.Length; i++)
            {
                if (references[i].toBePredicted)
                {
                    //Use Statistics to predict publication and then title. 
                    //Check if author and year are there. If they are not present
                    //then it is hopeless to do so. 
                    if (references[i].year == -1 || references[i].Authors == String.Empty)
                    {
                        Common.sw.WriteLine("Cannot Predict for this reference");
                    }
                    else
                    {
                        PredictPublication(ref references[i]);
                        if (references[i].Title == String.Empty && references[i].Publication != String.Empty)
                        {
                            references[i].InterchangeTitlePublication();
                        }
                        if ((references[i].Title == String.Empty && references[i].Publication == String.Empty) ||
                            (references[i].Authors == String.Empty && references[i].year == -1))
                        {
                            /*ArrayList newarr = references.ToList();
                            newarr.RemoveAt(i);
                            references = newarr.ToArray(typeof(Reference)) as Reference[];*/
                        }
                        else
                        {
                            references[i].Display(referenceXml);
                        }
                    }
                }
            }
            referenceFileStream.Close();
            Statistics.DisplayStatistics(statisticsXml);
        }
        #endregion

        #region Stage4
        /// <summary>
        /// Starting point for the program after getting information from the form. 
        /// </summary>
        internal static void Stage4()
        {
            for(int i = 0; i < Common.paragraphs.Length; i++)
            {
                string subParagraph = Common.paragraphs[i];
                long baseOffset = Common.startOffsetParagraphs[i];
                int yearEndIndex = 0;
                int incrOffset = 0;
                do
                {
                    subParagraph = subParagraph.Substring(yearEndIndex);
                    yearEndIndex = ExtractCitation(subParagraph, baseOffset + incrOffset);
                    incrOffset += yearEndIndex;
                } while (yearEndIndex != -1);
            }            
            Common.sw.Close();
            /*Process p = new Process();
            ProcessStartInfo pInfo = new ProcessStartInfo(@"c:\windows\System32\notepad.exe", Common.outputFilePath);
            p.StartInfo = pInfo;
            p.Start();*/
        }
        #endregion

        #region EntryPoint
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new InputForm();
            Application.Run(form);
        }
        #endregion
    }
}