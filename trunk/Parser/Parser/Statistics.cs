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

        public static double avgPublicationLength = 0.0;
        public static double avgReferenceLength = 0.0;
        public static double avgPublicationStart = 0.0;
        public static double avgPublicationEnd = 0.0;
        #endregion

        public static void DisplayStatistics()
        {
            Common.sw.WriteLine("STATISTICS");
            Common.sw.WriteLine("Total : " + stat);
            Common.sw.WriteLine("Author : " + statAuthor);
            Common.sw.WriteLine("Year : " + statYear);
            Common.sw.WriteLine("Title : " + statTitle);
            Common.sw.WriteLine("Publication : " + statPublication);
            Common.sw.WriteLine("Avg Reference Length : " + avgReferenceLength);
            Common.sw.WriteLine("Avg Publication Length : " + avgPublicationLength);
            Common.sw.WriteLine("Avg Publication start : " + avgPublicationStart);
            Common.sw.WriteLine("Avg Publication end : " + avgPublicationEnd);
        }

        public static void UpdateStatistics(Reference parsedReference)
        {
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
        }

        internal static void UpdateYearAuthor()
        {
            statYear = statYear + 1;
            statAuthor = statAuthor + 1;
        }

        internal static void UpdatePublication()
        {
            statPublication = statPublication + 1;
        }

        internal static void UpdateTitle()
        {
            statTitle = statTitle + 1;
        }

        internal static void UpdateReference()
        {
            stat = stat + 1;
        }
    }
}
