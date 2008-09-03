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
//    Author : Deepank Gupta  (deepankgupta AT gmail DOT com)
//    Date   : 18/08/08

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    internal class Statistics
    {
        #region Variables
        public static int stat = 0;
        public static int statYear = 0;
        public static int statAuthor = 0;
        public static int statPublication = 0;
        public static int statTitle = 0;
        public static int statParsed = 0;
        public static int statPredictedPublication = 0;
        public static int wronglyPredictedReference = 0;

        public static double avgPublicationLength = 0.0;
        public static double avgReferenceLength = 0.0;
        public static double avgPublicationStart = 0.0;
        public static double avgPublicationEnd = 0.0;

        #endregion

        #region DisplayMethod
        /// <summary>
        /// This function is used to display the statistics in an xml and output text file. 
        /// </summary>
        /// <param name="statisticsXml">XmlCreator to write to xml file. </param>
        public static void DisplayStatistics(XmlCreator statisticsXml)
        {
            Common.sw.WriteLine("STATISTICS");
            Common.sw.WriteLine("Total : " + stat);
            Common.sw.WriteLine("Wrong : " + wronglyPredictedReference);
            statisticsXml.AddFirstLevelTag("Reference", "");
            statisticsXml.AddSecondLevelTag("Total", stat.ToString());
            statisticsXml.AddSecondLevelTag("Wrong", wronglyPredictedReference.ToString());
            Common.sw.WriteLine("Avg Reference Length : " + avgReferenceLength);
            statisticsXml.AddSecondLevelTag("Length", avgReferenceLength.ToString());
            Common.sw.WriteLine("Author : " + statAuthor);
            statisticsXml.AddFirstLevelTag("Author", statAuthor.ToString());
            Common.sw.WriteLine("Year : " + statYear);
            statisticsXml.AddFirstLevelTag("Year", statYear.ToString());
            Common.sw.WriteLine("Title : " + statTitle);
            statisticsXml.AddFirstLevelTag("Titles", statTitle.ToString());
            Common.sw.WriteLine("Publication : " + statPublication);
            Common.sw.WriteLine("Predicted Publication : " + statPredictedPublication);
            statisticsXml.AddFirstLevelTag("Publications", "");
            statisticsXml.AddSecondLevelTag("DomainBased", statPublication.ToString());
            statisticsXml.AddSecondLevelTag("Predicted", statPredictedPublication.ToString());
            Common.sw.WriteLine("Avg Publication Length : " + avgPublicationLength);
            statisticsXml.AddSecondLevelTag("Length", statTitle.ToString());
            Common.sw.WriteLine("Avg Publication start : " + avgPublicationStart);
            statisticsXml.AddSecondLevelTag("StartIndex", avgPublicationStart.ToString());
            Common.sw.WriteLine("Avg Publication end : " + avgPublicationEnd);
            statisticsXml.AddSecondLevelTag("EndIndex", avgPublicationEnd.ToString());
        }
        #endregion

        #region UpdateMethods

        internal static void UpdateYearAuthor()
        {
            statYear = statYear + 1;
            statAuthor = statAuthor + 1;
        }

        internal static void UpdatePublication()
        {
            statPublication = statPublication + 1;
        }

        internal static void UpdatePredictedPublication()
        {
            statPredictedPublication = statPredictedPublication + 1;
        }

        internal static void UpdateTitle()
        {
            statTitle = statTitle + 1;
        }

        internal static void UpdateReference()
        {
            stat = stat + 1;
        }

        /// <summary>
        /// Updation of the statistics at every turn. 
        /// </summary>
        /// <param name="parsedReference">Reference object to be passed. </param>
        internal static void UpdateStatistics(Reference parsedReference)
        {
            if (parsedReference.IsPredictionNeeded())
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
        }
        #endregion
    }
}