using System.Xml;
using System.Xml.Serialization;

namespace WSQoSAPI.OfferBrokerWebService
{

    [System.Xml.Serialization.XmlType(Namespace = "http://wsqos.org/")]
    public class RemoteOffer
    {

        public System.Xml.XmlNode Offer;
        public string ServiceAccessPoint;
        public string ServiceDescriptionUrl;
        public string ServiceName;

        public RemoteOffer()
        {
        }

    } // class RemoteOffer

}

