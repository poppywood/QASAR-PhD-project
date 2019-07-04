using System;
using System.ComponentModel;
using System.Web.Services;
using System.Xml;

namespace WSQoSAPI
{

    [System.Web.Services.WebService(Description = "A WS-QoS Offer Broker Web Service.", Namespace = "http://wsqos.org/")]
    public abstract class WSQoSOfferBrokerWebService : System.Web.Services.WebService
    {

        public class RemoteOffer
        {

            public System.Xml.XmlNode Offer;
            public string ServiceAccessPoint;
            public string ServiceDescriptionUrl;
            public string ServiceName;

            public RemoteOffer()
            {
            }

            public RemoteOffer(WSQoSAPI.QoSOffer offer)
            {
                if (offer == null)
                    throw new System.NullReferenceException("offer is null.\uFFFD");
                if (offer.Xml == null)
                    throw new System.NullReferenceException("offer has no entries.\uFFFD");
                Offer = offer.Xml;
                ServiceName = offer.Service.Name;
                ServiceAccessPoint = offer.Service.AccessPointUrl;
                ServiceDescriptionUrl = offer.Service.DescriptionUrl;
            }

        } // class RemoteOffer

        private System.ComponentModel.IContainer components;

        public static WSQoSAPI.LocalOfferBroker Broker;
        private static int callcount;

        public WSQoSOfferBrokerWebService()
        {
            components = null;
            InitializeComponent();
            if (WSQoSAPI.CurrencyConverter.ActiveCurrencyConverter == null)
                WSQoSAPI.CurrencyConverter.ActiveCurrencyConverter = CreateCurrencyConverter();
            if (WSQoSAPI.WSQoSOfferBrokerWebService.Broker == null)
                WSQoSAPI.WSQoSOfferBrokerWebService.Broker = new WSQoSAPI.LocalOfferBroker(GetConfigFilePath());
        }

        static WSQoSOfferBrokerWebService()
        {
            WSQoSAPI.WSQoSOfferBrokerWebService.callcount = 0;
        }

        [System.Web.Services.WebMethod(Description = " Returns a string indicating status information on how many offers and services are currently available. ")]
        public string AvailableOffersInfo(string UddiTModelKey)
        {
            WSQoSAPI.WSQoSOfferBrokerWebService.callcount++;
            string s = "BrokerID: \uFFFD" + WSQoSAPI.WSQoSOfferBrokerWebService.Broker.TestID + ".  \uFFFD";
            WSQoSAPI.WSQoSOfferBrokerWebService.Broker.TestID = "MyBroker\uFFFD";
            WSQoSAPI.OfferList offerList = WSQoSAPI.LocalOfferBroker.GetOfferList(UddiTModelKey);
            if (offerList == null)
            {
                offerList = new WSQoSAPI.OfferList(UddiTModelKey);
                WSQoSAPI.LocalOfferBroker.AddOfferList(offerList);
                WSQoSAPI.WSQoSOfferBrokerWebService.Broker.UpdateServicesForOfferList(offerList);
                WSQoSAPI.WSQoSOfferBrokerWebService.Broker.UpdateOffersForOfferList(offerList);
                object obj = s;
                s = obj + "no services available for tModel with key \uFFFD" + UddiTModelKey + " - (call no.\uFFFD" + WSQoSAPI.WSQoSOfferBrokerWebService.callcount + ").\uFFFD";
            }
            else
            {
                s = s + offerList.OfferCount + " offers available from \uFFFD" + offerList.ServiceCount + " services for tModel with key \uFFFD" + UddiTModelKey + " - (call no.\uFFFD" + WSQoSAPI.WSQoSOfferBrokerWebService.callcount + ").\uFFFD";
            }
            return s;
        }

        protected abstract WSQoSAPI.CurrencyConverter CreateCurrencyConverter();

        [System.Web.Services.WebMethod(Description = "Finds the cheapest of all offers for services that implement the tModel identified by the uddiServiceKey and fulfill the WS-QoS requirements Requirements.")]
        public WSQoSAPI.WSQoSOfferBrokerWebService.RemoteOffer GetBestOffer(System.Xml.XmlNode Requirements, string UddiTModelKey)
        {
            // trial
            return null;
        }

        protected abstract string GetConfigFilePath();

        [System.Web.Services.WebMethod(Description = "Returns the Url of the uddi registry to be inquired.")]
        public System.Xml.XmlNode GetTrustedProviders()
        {
            return WSQoSAPI.WSQoSOfferBrokerWebService.Broker.GetProviderList();
        }

        [System.Web.Services.WebMethod(Description = "Returns the Url of the uddi registry to be inquired.")]
        public string GetUddiReqistryLocation()
        {
            // trial
            return null;
        }

        private void InitializeComponent()
        {
        }

        public void SetUddiRegistryLocation(string url)
        {
            // trial
        }

        protected override void Dispose(bool disposing)
        {
            // trial
        }

    } // class WSQoSOfferBrokerWebService

}

