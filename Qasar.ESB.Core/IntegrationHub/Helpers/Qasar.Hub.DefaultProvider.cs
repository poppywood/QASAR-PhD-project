using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;


namespace Qasar.ESB.Helpers
{
    /// <summary>
    /// Provides defaults from named Xml file
    /// Each file is assumed to have structure:
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <SomeDefaults>
    ///     <Default>
    ///         <node></node>
    ///         <value></value>    
    ///     </Default>
    /// </SomeDefaults>
    /// </summary>
    public class DefaultProvider
    {
        /// <summary>
        /// Appends default values into a single instance of the target node
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="defaultfile">The file from which to extract defaults</param>
        /// <param name="appendnode">The node into which to append the defaults</param>
        /// <param name="insertbefore">The insert before node</param>
        /// <param name="mgr">A namespace manager</param>
        /// <param name="insertmultiple">When true attempts to insert multiple copies of the defaults into multiple target nodes</param>
        /// <returns>The modified request</returns>
        public static string Insert(string request, string defaultfile, string appendnode, string insertbefore,  XmlNamespaceManager mgr, bool insertmultiple)
        {
            XmlDocument doc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader(System.AppDomain.CurrentDomain.BaseDirectory + "\\" + defaultfile);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            doc.LoadXml(request);
            string node = string.Empty;
            string value = string.Empty;
            reader.Read(); //skip the xml declaration
            reader.Read(); //read the root 
            while (!reader.EOF) 
            {
                reader.Read(); //get the next default node
                reader.Read(); //move onto node node
                if (reader.IsStartElement())
                {
                    if (reader.GetAttribute("type") != null)
                    //this is an attribute not an element
                    {
                        node = reader.ReadElementContentAsString();
                        value = reader.ReadElementContentAsString(); 
                        if (insertmultiple)
                            doc = XPathHelper.AddManyAttributes(doc, node, value, appendnode, mgr);
                        else
                            doc = XPathHelper.AddAttribute(doc, node, value, appendnode, mgr);
                    }
                    else
                    {
                        node = reader.ReadElementContentAsString();
                        value = reader.ReadElementContentAsString(); 
                        if (insertmultiple)
                            doc = XPathHelper.AddManyElements(doc, node, value, appendnode, insertbefore, mgr);
                        else
                            doc = XPathHelper.AddElement(doc, node, value, appendnode, insertbefore, mgr);
                    }
                }
            }
            reader.Close();
            return doc.OuterXml;
        }
    }
}
