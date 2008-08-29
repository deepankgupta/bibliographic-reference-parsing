#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Collections;
#endregion

namespace Parser
{
    class ReferenceExtractor
    {
        #region Constants
        int MaxAuthorLength = 100;
        int MaxReferenceLength = 400;
        double YearFound = 0.2;
        double AuthorFound = 0.2;
        double AuthorMightFound = 0.1;
        double EachPreviousReference = 0.1;
        double MaxPreviousReference = 0.2;
        double KeywordPresent = 0.2;
        double SuccessorFound = 0.1;
        double ReferenceLengthLess = 0.1;        
        string keywordFilePath = @"..\data\keywords.txt";
        #endregion

        #region Variables
        private string filePath;
        private string referenceFilePath;
        private XmlTextReader reader;
        private string[] references;        
        private int noPreviousParagraphReference;
        /// <summary>
        /// This variable is for internal use of the private function 
        /// </summary>
        static int prevIndex = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="path">FilePath of the file from which references are to be extracted</param>
        public ReferenceExtractor(string path, string outputPath)
        {
            filePath = path;
            referenceFilePath = outputPath;
            noPreviousParagraphReference = -1;
        }
        #endregion

        /// <summary>
        /// This function will define the streamreader from the filePath
        /// This will also define an XmlReader which will facilitate the reading of the file. 
        /// </summary>
        private void Init()
        {
            //Get objects
            reader = new XmlTextReader(filePath);
            GetParagraphs();
        }

        /// <summary>
        /// This function is used to get the string of all values related to a particular node. 
        /// </summary>
        /// <param name="p">The path to the node</param>
        /// <returns>The string array of all the values of all nodes with path p</returns>
        private string[] GetNodeValues(string p, XmlNamespaceManager nm)
        {
            XPathDocument doc = new XPathDocument(filePath);
            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression _configExpr = nav.Compile("tax:taxonx");
            _configExpr.SetContext(nm);
            XPathNodeIterator itr = nav.Select(_configExpr);            
            string[] ret = new string[itr.Count];
            int i = 0;
            while (itr.MoveNext())
            {
                ret[i] = itr.Current.Value;
                i = i + 1;
            }
            if (itr.Count == 0)
                return null;
            else
                return ret;
        }
        
        /// <summary>
        /// This function will get all the paragraphs from the streamreader which match as 
        /// tax:taxonxBody/tax:p
        /// </summary>
        private void GetParagraphs()
        {
            ArrayList strList = new ArrayList();
            ArrayList intList = new ArrayList();            
            while (reader.Read())
            {
                if (reader.Name == "tax:p")
                {
                    StringBuilder b = new StringBuilder();
                    intList.Add(reader.LineNumber);
                    do
                    {
                        do
                        {
                            reader.Read();
                            if (reader.Value != String.Empty)
                            {                                
                                b.Append(reader.Value);
                            }
                        } while (reader.Name == String.Empty);
                    } while (reader.Name != "tax:p");
                    strList.Add(b.ToString());
                }
            }
            reader.Close();
            ArrayList offsetArray = new ArrayList();
            foreach (int index in intList)
            {
                long offset = FindIndexOfLine(index);
                offsetArray.Add(offset);
            }            
            Common.paragraphs = strList.ToArray(typeof(string)) as string[];
            Common.offsetParagraphs = offsetArray.ToArray(typeof(long)) as long[];
        }

        private long FindIndexOfLine(int index)
        {
            FileStream fileReader = new FileStream(Common.inputFilePath, FileMode.Open);
            int count = 1;
            while (count < index)
            {
                if (fileReader.ReadByte() == '\n')
                    count++;
            }
            char test = (char)fileReader.ReadByte();
            long temp = fileReader.Position;
            fileReader.Close();
            return temp;
        }
        
        /// <summary>
        /// Checks for Year and Author presence and gives the probablitiy associated with it. 
        /// </summary>
        /// <param name="paragraph">String specifying the paragrpah to be checked. </param>
        /// <returns>Probablity</returns>
        private double CheckForYearAndAuthor(string paragraph)
        {
            double prob = 0.0;
            int year = -1;
            int index = Common.CheckForYear(paragraph, ref year);
            if (index == -1)
            {
                if (noPreviousParagraphReference >= 0)
                    noPreviousParagraphReference = 0;
                else
                    noPreviousParagraphReference = -1;
                return prob;
            }
            else
            {
                prob += YearFound;
            }
            //This if statement is a very prelimnary check on the fact that an author name can never extend to be 
            //more than 100 characters. It is not foolproof and very prelimnary thing to rule out more than 90%
            //cases. 
            if (index > MaxAuthorLength)
            {
                //Check if there are single character words. If there are no single character words, it is unlikely
                //to be an author. If they are there and all are capitals, it might just be an author. 
                string predictedAuthor = paragraph.Substring(0, index);
                string[] predictedAuthorWords = predictedAuthor.Split(Common.seperators,
                    StringSplitOptions.RemoveEmptyEntries);
                bool singleCharacterWords = false;
                bool areTheyCapitalized = false;
                foreach (string word in predictedAuthorWords)
                {
                    if (word.Length == 1 && (word.ToUpper() == word))
                    {
                        singleCharacterWords = true;
                        areTheyCapitalized = true;
                    }
                    else if (word.Length == 1)
                    {
                        singleCharacterWords = true;
                        areTheyCapitalized = false;
                    }
                }
                if (!singleCharacterWords || !areTheyCapitalized)
                {
                    noPreviousParagraphReference = -1;
                    return prob;
                }
                else
                {
                    //else given a second chance. 
                    prob += AuthorMightFound;
                }
            }
            else
            {
                prob += AuthorFound;
            }
            return prob;
        }

        /// <summary>
        /// This function will predict whether a given paragraph is a potential reference or not. 
        /// </summary>
        /// <returns>True in case it is a potential reference
        /// False in case it is not. </returns>
        private bool PredictPotentialReference(int i)
        {
            double probability = 0.0;
            string paragraph = CleanUpReference(Common.paragraphs[i]);
            if (paragraph == String.Empty)
            {
                return false;
            }
            //0. Check for the presence of a keyword. 
            if (CheckForKeyword(paragraph))
            {
                noPreviousParagraphReference = 0;
                return false;
            }
            
            //1. Check for an author name or an year name,  --> Most Probability. If not found, it is not a reference. 
            probability += CheckForYearAndAuthor(paragraph);
            
            //2. Is it preceded by a keyword or a reference. --> Given weightage, 
            if (noPreviousParagraphReference >= 0)
            {
                probability += KeywordPresent;
                double max = (MaxPreviousReference < (noPreviousParagraphReference * EachPreviousReference)) ? MaxPreviousReference : (noPreviousParagraphReference * EachPreviousReference);
                probability += max;
            }
            else
            {
                //TODO: It might be that the previous line might be a keyword which we might have missed. 

            }

            //3. Length of the paragraph --> Prelimnary check which checks with the maximum length of the reference.             
            if (paragraph.Length < MaxReferenceLength)
            {
                probability += ReferenceLengthLess;
            }

            //4. Check for the paragraph suceeding it prelimnary as well as author name check. If that is also a reference
            // then it is high probability to be a reference.
            if (i != Common.paragraphs.Length - 1)
            {
                probability += (SuccessorFound * (CheckForYearAndAuthor(Common.paragraphs[i + 1]) / (YearFound + AuthorFound)));
            }
            if (probability >= 0.7)
            {
                noPreviousParagraphReference++;
                return true;
            }
            else
            {
                if (noPreviousParagraphReference >= 0)
                    noPreviousParagraphReference = 0;
                else
                    noPreviousParagraphReference = -1;
                return false;
            }
        }

        /// <summary>
        /// This function is used to check whether the paragraph is a keyword starting the block of references or not. 
        /// </summary>
        /// <param name="paragraph">Paragraph input string</param>
        /// <returns>True or False</returns>
        private bool CheckForKeyword(string paragraph)
        {
            StreamReader keywordReader = new StreamReader(keywordFilePath);
            ArrayList keywordList = new ArrayList();
            while(keywordReader.Peek()!= -1)
            {
                keywordList.Add(keywordReader.ReadLine());
            }
            string[] keywordArray = keywordList.ToArray(typeof(string)) as string[];
            string[] paragraphWords = paragraph.Split(Common.seperators, StringSplitOptions.RemoveEmptyEntries);
            int minimum = Int32.MaxValue;
            foreach (string keyword in keywordArray)
            {
                string[] keywordParts = keyword.Split(Common.seperators, StringSplitOptions.RemoveEmptyEntries);
                if (keywordParts.Length < minimum)
                    minimum = keywordParts.Length;
                if (keywordParts.Length != paragraphWords.Length)
                    continue;
                int i;
                for(i = 0; i < keywordParts.Length;i++)
                {
                    if (!keywordParts[i].Equals(paragraphWords[i], StringComparison.CurrentCultureIgnoreCase))
                        break;
                }
                if (i == keywordParts.Length)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This function is used to remove the starting noise from a reference ie. 1,2,3 or 1a, 2a, etc. 
        /// This can also be in the form of roman numbers like [i] or alphabetical like [A] 
        /// This will always be a number, character or roman numeral followed by a seperator. 
        /// </summary>
        private string CleanUpReference(string input)
        {
            //TODO: Implement CleanUp
            int i;
            bool flag = false;
            string output = "";
            for (i = 0; i < input.Length; i++)
            {
                flag = false;
                foreach (char ch in Common.seperators)
                {
                    if (ch == input[i])
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    break;
            }
            output = input.Substring(i);
            output = output.Replace("\r", "");
            output = output.Replace("\n", "");
            return output;
        }

        /// <summary>
        /// This function is used to define a write stream which will store all the references in a references.txt file. 
        /// </summary>
        private void StoreReferences()
        {
            FileStream fw = new FileStream(referenceFilePath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fw, Encoding.Unicode);
            //Write all references in it.             
            foreach (string reference in references)
            {
                sw.WriteLine(reference);
            }
            sw.Close();
        }

        /// <summary>
        /// Release resources. 
        /// </summary>
        private void Exit()
        {
            //Close the XmlReader  
            reader.Close();
        }

        /// <summary>
        /// The starting point of the class and the place where all the work is done. 
        /// </summary>
        public void Main()
        {
            Init();
            ArrayList referenceList = new ArrayList();
            ArrayList referenceOffsetList = new ArrayList();
            int i = 0;
            foreach (string paragraph in Common.paragraphs)
            {
                // all paragraphs are examined.
                if (PredictPotentialReference(i))
                {
                    referenceList.Add(CleanUpReference(paragraph));
                    referenceOffsetList.Add(Common.offsetParagraphs[i]);
                    //Delete it from paragraph list
                    Common.paragraphs[i] = "";
                }
                i = i + 1;
            }
            references = referenceList.ToArray(typeof(string)) as string[];
            Common.referenceOffsets = referenceOffsetList.ToArray(typeof(long)) as long[];
            StoreReferences();
            Exit();
        }
    }
}