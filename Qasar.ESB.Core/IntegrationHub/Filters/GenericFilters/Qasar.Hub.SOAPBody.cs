using System;
using Qasar.ESB.Helpers;
using System.Xml;

namespace Qasar.ESB.Filter
{
    /// <summary>
    /// Extract the body from the content. 
    /// Assumes the content is provided as a fully-formed
    /// SOAP envelope, and takes out whatever it
    /// finds between the Body tags.
    /// </summary>
    public class SOAPBody : FilterBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Name { get { return "SOAPBody"; } }
        /// <summary>
        /// 
        /// </summary>
        public override string Code { get { return "SPB"; } }
        /// <summary>
        /// 
        /// </summary>
        public SOAPBody()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessData(PipeData data)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data.Request);
            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            mgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
            mgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            data.Request = doc.SelectSingleNode("//soap:Envelope/soap:Body", mgr).InnerXml;              
        }
    }
}