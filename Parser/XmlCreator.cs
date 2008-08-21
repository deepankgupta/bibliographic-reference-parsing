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
        internal void AddReferenceField(string tag, string value)
        {
            if (tag == String.Empty || tag == null)
                return;
            XmlElement xmlelem = Create(tag, value);             
            xmldoc.ChildNodes.Item(1).LastChild.AppendChild(xmlelem);            
        }

        /// <summary>
        /// A function to add a Reference tag
        /// </summary>
        internal void AddReferenceTag()
        {
            XmlElement xmlelem = Create("Reference", "");
            xmldoc.ChildNodes.Item(1).AppendChild(xmlelem);
        }


        #endregion 

        public XmlCreator(string filename, string rootTag)
        {
            xmlFile = Path.GetDirectoryName(Common.outputFilePath) + @"\" + filename;
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

        ~XmlCreator()
        {
            xmldoc.Save(xmlFile);
        }            
    }
}