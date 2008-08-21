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
            citationXml.AddSecondLevelTag("Name", name);
            citationXml.AddSecondLevelTag("Year", year.ToString());
        }
    }
}
