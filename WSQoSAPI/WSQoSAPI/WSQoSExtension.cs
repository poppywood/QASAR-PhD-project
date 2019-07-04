using System.Web.Services.Configuration;
using System.Web.Services.Description;
using System.Xml;

namespace WSQoSAPI
{

    [System.Web.Services.Configuration.XmlFormatExtension("wsqos", "http://wsqos.org", typeof(System.Web.Services.Description.Port))]
    public class WSQoSExtension : System.Web.Services.Description.ServiceDescriptionFormatExtension
    {

        public System.Xml.XmlElement offers;

        public WSQoSExtension()
        {
        }

    } // class WSQoSExtension

}

