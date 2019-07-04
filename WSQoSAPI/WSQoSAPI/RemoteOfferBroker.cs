using System;
using WSQoSAPI.OfferBrokerWebService;

namespace WSQoSAPI
{

    public class RemoteOfferBroker : WSQoSAPI.WSQoSOfferBroker
    {

        public const string RemoteBrokerConfigFile = "./RemoteBroker.config";

        private WSQoSAPI.OfferBrokerWebService.OfferBroker BrokerService;

        public string Location
        {
            get
            {
                return BrokerService.Url;
            }
            set
            {
                BrokerService.Url = value;
            }
        }

        public RemoteOfferBroker()
        {
            BrokerService = new WSQoSAPI.OfferBrokerWebService.OfferBroker();
            LoadConfig();
        }

        public RemoteOfferBroker(string BrokerLocation)
        {
            BrokerService = new WSQoSAPI.OfferBrokerWebService.OfferBroker();
            BrokerService.Url = BrokerLocation;
        }

        public void Dispose()
        {
        }

        public WSQoSAPI.QoSOffer GetBestOffer(WSQoSAPI.QoSRequirements Requirements, string uddiTModelKey)
        {
            if (Requirements == null)
                Requirements = new WSQoSAPI.QoSRequirements();
            WSQoSAPI.OfferBrokerWebService.RemoteOffer remoteOffer = null;
            try
            {
                remoteOffer = BrokerService.GetBestOffer(Requirements.Xml, uddiTModelKey);
            }
            catch (System.Exception e)
            {
             //   e.Message;
            }
            if (remoteOffer == null)
                return null;
            WSQoSAPI.ServiceModel serviceModel = new WSQoSAPI.ServiceModel(remoteOffer.ServiceName, remoteOffer.ServiceAccessPoint, remoteOffer.ServiceDescriptionUrl);
            return new WSQoSAPI.QoSOffer(remoteOffer.Offer, serviceModel);
        }

        public string GetUddiReqistryLocation()
        {
            // trial
            return null;
        }

        private void LoadConfig()
        {
            // trial
        }

    } // class RemoteOfferBroker

}

