using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;

using Qasar.ESB.Helpers;

namespace Qasar.ESB.Filter
{

    /// <summary>
    /// Uses config to identify Xslt to use
    /// </summary>
    public class XsltFilter: FilterBase
    {
        /// <summary>
        /// Name
        /// </summary>
        public override string Name { get { return "XsltFilter"; } }
        /// <summary>
        /// Code
        /// </summary>
        public override string Code { get { return "XSL"; } }

        /// <summary>
        /// XsltFilter
        /// </summary>
        public XsltFilter()
        {
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        protected override void ProcessData(PipeData data)
        {
            string xslt = 
                data.Rule.Value(this.Code, "FIL", data.ProductCode, Convert.ToString(data.RequestType), data.SubAction, Convert.ToString(data.Source), data.UserId, data);
            XmlTextReader xr = new XmlTextReader(data.Request, XmlNodeType.Document, null);

            //Use XslTransform rather then XslCompiledTransform because XSLT requires xsi:type attributes
            //and these aren't supported by XslCompiledTransform (criminal).
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(data.Request);
            XPathNavigator xdNav = xd.CreateNavigator();
            XslTransform tr = new XslTransform();

            //Load the xslt from this assembly
            LoadResourceXsltIntoXslTransform(tr, xslt);

            StringWriter sw = new StringWriter();
            tr.Transform(xdNav,null,sw);
            string content = sw.ToString();
            content = content.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            data.Request = content;
        }

        /// <summary>
        /// Loads the named resource into the XslTransform object
        /// </summary>
        /// <param name="xlsTransform"></param>
        /// <param name="resourceName"></param>
        private void LoadResourceXsltIntoXslTransform(XslTransform xlsTransform, string resourceName)
        {
            System.Reflection.Assembly assembly =
                System.Reflection.Assembly.GetExecutingAssembly();

            using (System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(
                assembly.GetManifestResourceStream(
                    string.Format("{0}.{1}", assembly.GetName().Name, resourceName)
                )))
            {
                xlsTransform.Load(xmlReader);
            }
        }
    }
}
