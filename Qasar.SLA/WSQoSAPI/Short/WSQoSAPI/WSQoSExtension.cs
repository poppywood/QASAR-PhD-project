using System.Web.Services.Configuration;
using System.Web.Services.Description;
using System.Xml;

namespace WSQoSAPI
{

    [XmlFormatExtension("wsqos", "http://wsqos.org", typeof(System.Web.Services.Description.Port))]
    public class WSQoSExtension : ServiceDescriptionFormatExtension
    {

        public XmlElement offers;

        public WSQoSExtension()
        {
        }

    } // class WSQoSExtension

}

