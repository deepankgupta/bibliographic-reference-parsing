﻿//    This file is part of bibliographic-reference-parsing. 
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
using System.IO;
using System.Xml;

namespace Parser
{
    class XmlCreator
    {
        #region Variables
        XmlDocument xmldoc;
        string xmlFile;
        #endregion

        #region Create

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        private XmlElement Create(string name, string text)
        {            
            XmlElement xmlelem;
            XmlText xmltext;
            xmlelem = xmldoc.CreateElement("", name, "");
            xmltext = xmldoc.CreateTextNode(text);
            xmlelem.AppendChild(xmltext);
            return xmlelem;
        }

        /// <summary>
        /// Add a reference Field Tag
        /// </summary>
        /// <param name="name">String specifying the tag name. </param>
        /// <param name="text">String specifying the inner text. </param>
        internal void AddSecondLevelTag(string tag, string value)
        {
            if (tag == String.Empty || tag == null)
                return;
            XmlElement xmlelem = Create(tag, value);             
            xmldoc.ChildNodes.Item(1).LastChild.AppendChild(xmlelem);            
        }

        /// <summary>
        /// A function to add a Reference tag
        /// </summary>
        internal void AddFirstLevelTag(string tag, string value)
        {
            XmlElement xmlelem = Create(tag, value);
            xmldoc.ChildNodes.Item(1).AppendChild(xmlelem);
        }

        #endregion 

        #region ConstructorDestructor
        /// <summary>
        /// Constructor creates the XmlCreator object. 
        /// </summary>
        /// <param name="filename">Filename which will be used to store the xml file. </param>
        /// <param name="rootTag">The roottag of the xml. </param>
        public XmlCreator(string filename, string rootTag)
        {
            xmlFile = Path.GetDirectoryName(Common.inputFilePath) +
                    @"\" + Path.GetFileNameWithoutExtension(Common.inputFilePath) + @"_" + filename;
            xmldoc = new XmlDocument();
            if (File.Exists(xmlFile))
            {
                File.Delete(xmlFile);
            }
            //let's add the XML declaration section
            XmlNode xmlnode = xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            xmldoc.AppendChild(xmlnode);
            //let's add the root element
            XmlElement xmlelem = Create(rootTag, "");
            xmldoc.AppendChild(xmlelem);
        }

        /// <summary>
        /// Destructor: Saves the xmlFile before exiting. 
        /// </summary>
        ~XmlCreator()
        {
            xmldoc.Save(xmlFile);
        }
        #endregion
    }
}