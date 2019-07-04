using System.Xml;
using System.Xml.Serialization;

namespace WSQoSAPI.OfferBrokerWebService
{

    [XmlType(Namespace = "http://wsqos.org/")]
    public class RemoteOffer
    {

        public XmlNode Offer;
        public string ServiceAccessPoint;
        public string ServiceDescriptionUrl;
        public string ServiceName;

        public RemoteOffer()
        {
        }

    } // class RemoteOffer

}

