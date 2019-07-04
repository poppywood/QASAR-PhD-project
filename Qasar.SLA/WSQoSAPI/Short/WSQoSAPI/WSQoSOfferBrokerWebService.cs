using System;
using System.ComponentModel;
using System.Web.Services;
using System.Xml;

namespace WSQoSAPI
{

    [WebService(Description = "A WS-QoS Offer Broker Web Service.", Namespace = "http://wsqos.org/")]
    public abstract class WSQoSOfferBrokerWebService : WebService
    {

        public class RemoteOffer
        {

            public XmlNode Offer;
            public string ServiceAccessPoint;
            public string ServiceDescriptionUrl;
            public string ServiceName;

            public RemoteOffer()
            {
            }

            public RemoteOffer(QoSOffer offer)
            {
                if (offer == null)
                    throw new NullReferenceException("offer is null.\uFFFD");
                if (offer.Xml == null)
                    throw new NullReferenceException("offer has no entries.\uFFFD");
                Offer = offer.Xml;
                ServiceName = offer.Service.Name;
                ServiceAccessPoint = offer.Service.AccessPointUrl;
                ServiceDescriptionUrl = offer.Service.DescriptionUrl;
            }

        } // class RemoteOffer

        private IContainer components;

        public static LocalOfferBroker Broker;
        private static int callcount;

        public WSQoSOfferBrokerWebService()
        {
            components = null;
            InitializeComponent();
            if (CurrencyConverter.ActiveCurrencyConverter == null)
                CurrencyConverter.ActiveCurrencyConverter = CreateCurrencyConverter();
            if (WSQoSOfferBrokerWebService.Broker == null)
                WSQoSOfferBrokerWebService.Broker = new LocalOfferBroker(GetConfigFilePath());
        }

        static WSQoSOfferBrokerWebService()
        {
            WSQoSOfferBrokerWebService.callcount = 0;
        }

        [WebMethod(Description = " Returns a string indicating status information on how many offers and services are currently available. ")]
        public string AvailableOffersInfo(string UddiTModelKey)
        {
            WSQoSOfferBrokerWebService.callcount++;
            string s = "BrokerID: \uFFFD" + WSQoSOfferBrokerWebService.Broker.TestID + ".  \uFFFD";
            WSQoSOfferBrokerWebService.Broker.TestID = "MyBroker\uFFFD";
            OfferList offerList = LocalOfferBroker.GetOfferList(UddiTModelKey);
            if (offerList == null)
            {
                offerList = new OfferList(UddiTModelKey);
                LocalOfferBroker.AddOfferList(offerList);
                WSQoSOfferBrokerWebService.Broker.UpdateServicesForOfferList(offerList);
                WSQoSOfferBrokerWebService.Broker.UpdateOffersForOfferList(offerList);
                object obj = s;
                s = obj + "no services available for tModel with key \uFFFD" + UddiTModelKey + " - (call no.\uFFFD" + WSQoSOfferBrokerWebService.callcount + ").\uFFFD";
            }
            else
            {
                s = s + offerList.OfferCount + " offers available from \uFFFD" + offerList.ServiceCount + " services for tModel with key \uFFFD" + UddiTModelKey + " - (call no.\uFFFD" + WSQoSOfferBrokerWebService.callcount + ").\uFFFD";
            }
            return s;
        }

        protected abstract CurrencyConverter CreateCurrencyConverter();

        [WebMethod(Description = "Finds the cheapest of all offers for services that implement the tModel identified by the uddiServiceKey and fulfill the WS-QoS requirements Requirements.")]
        public WSQoSOfferBrokerWebService.RemoteOffer GetBestOffer(XmlNode Requirements, string UddiTModelKey)
        {
            // trial
            return null;
        }

        protected abstract string GetConfigFilePath();

        [WebMethod(Description = "Returns the Url of the uddi registry to be inquired.")]
        public XmlNode GetTrustedProviders()
        {
            return WSQoSOfferBrokerWebService.Broker.GetProviderList();
        }

        [WebMethod(Description = "Returns the Url of the uddi registry to be inquired.")]
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

