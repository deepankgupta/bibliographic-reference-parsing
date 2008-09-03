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
