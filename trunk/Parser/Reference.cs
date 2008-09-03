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
    class Reference
    {
        #region Variables
        private string referenceText;
        private string authors;
        private string title;
        private string publication;
        private string pageNos;
        public int year;
        public int seperatorBeforePublication;
        public int seperatorAfterPublication;
        public int yearEnd;
        public bool toBePredicted;
        public long startOffset;
        public long endOffset;
        #endregion
        
        #region Constructor
        public Reference(string reference, long startOffset, long endOffset)
        {
            this.referenceText = reference;
            this.startOffset = startOffset;
            this.endOffset = endOffset;
            this.publication = "";
            this.seperatorAfterPublication = -1;
            this.seperatorBeforePublication = -1;
            this.title = "";
            this.year = -1;
            this.authors = "";
            this.yearEnd = -1;
            this.pageNos = "";
        }
        #endregion

        #region GetAndSetMethods

        public string ReferenceText
        {
            get
            {
                return referenceText;
            }
            set
            {
                referenceText = value;
                referenceText = Common.Strip(referenceText);
            }
        }

        public string Authors
        {
            get
            {
                return authors;
            }
            set
            {
                authors = value;
                authors = Common.Strip(authors);
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                title = Common.Strip(title);
            }
        }

        public string Publication
        {
            get
            {
                return publication;
            }
            set
            {
                publication = value;
                publication = Common.Strip(publication);
            }
        }

        public string PageNos
        {
            get
            {
                return pageNos;
            }
            set
            {
                pageNos = value;
                pageNos = Common.Strip(pageNos);
            }
        }
        #endregion

        public void Display(XmlCreator file)
        {
            file.AddFirstLevelTag("Reference", "");
            file.AddSecondLevelTag("Text", referenceText);
            file.AddSecondLevelTag("StartOffset", startOffset.ToString());
            file.AddSecondLevelTag("EndOffset", endOffset.ToString());
            Common.sw.WriteLine("REFERENCE : " + referenceText);
            file.AddSecondLevelTag("Authors", authors);
            Common.sw.WriteLine("AUTHORS : " + authors);
            file.AddSecondLevelTag("Year", year.ToString());
            Common.sw.WriteLine("YEAR : " + year);
            file.AddSecondLevelTag("Title", title);
            Common.sw.WriteLine("TITLE : " + title);
            if (this.seperatorBeforePublication != -1 && this.seperatorBeforePublication < referenceText.Length)
            {                
                Common.sw.WriteLine("SEPERATOR BEFORE PUBLICATION : "
                    + referenceText[seperatorBeforePublication]);
            }
            file.AddSecondLevelTag("Publication", publication);
            Common.sw.WriteLine("PUBLICATION : " + publication);
            if (this.seperatorAfterPublication != -1 && this.seperatorAfterPublication < referenceText.Length)
            {
                Common.sw.WriteLine("SEPERATOR AFTER PUBLICATION : " +
                    referenceText[seperatorAfterPublication]);
            }
            file.AddSecondLevelTag("PageNumbers", PageNos);
            Common.sw.WriteLine("PAGE NUMBERS : " + PageNos);
            Common.sw.WriteLine("\n\n");            
        }

        public bool IsPredictionNeeded()
        {
            if (authors == String.Empty || publication == string.Empty || title == string.Empty)
                return false;
            else
                return true;
        }

        public void InterchangeTitlePublication()
        {
            Title = Publication;
            int index = ReferenceText.IndexOf(Title) + Title.Length;
            Publication = ReferenceText.Substring(index,ReferenceText.Length - index);
        }
    }
}