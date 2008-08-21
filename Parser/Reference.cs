﻿using System;
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
        #endregion
        
        #region Constructor
        public Reference(string reference)
        {
            this.referenceText = reference;
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
            file.AddReferenceTag();
            file.AddReferenceField("Text", referenceText);

            Common.sw.WriteLine("REFERENCE : " + referenceText);
            file.AddReferenceField("Authors", authors);
            Common.sw.WriteLine("AUTHORS : " + authors);
            file.AddReferenceField("Year", year.ToString());
            Common.sw.WriteLine("YEAR : " + year);
            file.AddReferenceField("Title", title);
            Common.sw.WriteLine("TITLE : " + title);
            if (this.seperatorBeforePublication != -1 && this.seperatorBeforePublication < referenceText.Length)
            {                
                Common.sw.WriteLine("SEPERATOR BEFORE PUBLICATION : "
                    + referenceText[seperatorBeforePublication]);
            }
            file.AddReferenceField("Publication", publication);
            Common.sw.WriteLine("PUBLICATION : " + publication);
            if (this.seperatorAfterPublication != -1 && this.seperatorAfterPublication < referenceText.Length)
            {
                Common.sw.WriteLine("SEPERATOR AFTER PUBLICATION : " +
                    referenceText[seperatorAfterPublication]);
            }
            file.AddReferenceField("PageNumbers", PageNos);
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
