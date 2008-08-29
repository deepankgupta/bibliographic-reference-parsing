using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    class Citation
    {
        string name;
        int year;
        string paragraph;
        long offset;

        #region GetAndSetMethods
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                name = Common.Strip(name);
            }
        }

        public long Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }            
        }

        public string Paragraph
        {
            get
            {
                return paragraph;
            }
            set
            {
                paragraph = value;
                paragraph = Common.Strip(paragraph);
            }
        }

        public int Year
        {
            get
            {
                return year;
            }
            set
            {
                year = value;
            }
        }
        #endregion

        internal void Display(XmlCreator citationXml)
        {
            citationXml.AddFirstLevelTag("Citation", "");
            citationXml.AddSecondLevelTag("Paragraph", paragraph);
            citationXml.AddSecondLevelTag("Name", name);
            citationXml.AddSecondLevelTag("Year", year.ToString());
            citationXml.AddSecondLevelTag("Offset", offset.ToString());
        }
    }
}
