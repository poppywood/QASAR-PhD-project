using System;
using System.Xml;

using com.espertech.esper.compat;

namespace com.espertech.esper.events.xml
{
    /// <summary>
    /// Provides the namespace context information for compiling XPath expressions.
	/// Not sure if this has any relevance for the .NET implementation -- Aaron
    /// </summary>
    
    public class XPathNamespaceContext
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public static XmlNamespaceManager Create()
        {
            XmlNameTable nsTable = new NameTable() ;
            XmlNamespaceManager nsManager = new XmlNamespaceManager( nsTable ) ;
            nsManager.AddNamespace(
                XMLConstants.XML_NS_PREFIX,
                XMLConstants.XML_NS_URI ) ;
            //nsManager.AddNamespace(
            //    XMLConstants.XMLNS_ATTRIBUTE,
            //    XMLConstants.XMLNS_ATTRIBUTE_NS_URI ) ;

            return nsManager ;
        }
    }
}
