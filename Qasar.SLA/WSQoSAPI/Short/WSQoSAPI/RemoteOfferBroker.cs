using System;
using WSQoSAPI.OfferBrokerWebService;

namespace WSQoSAPI
{

    public class RemoteOfferBroker : WSQoSOfferBroker
    {

        public const string RemoteBrokerConfigFile = "./RemoteBroker.config";

        private OfferBroker BrokerService;

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
            BrokerService = new OfferBroker();
            LoadConfig();
        }

        public RemoteOfferBroker(string BrokerLocation)
        {
            BrokerService = new OfferBroker();
            BrokerService.Url = BrokerLocation;
        }

        public void Dispose()
        {
        }

        public QoSOffer GetBestOffer(QoSRequirements Requirements, string uddiTModelKey)
        {
            if (Requirements == null)
                Requirements = new QoSRequirements();
            RemoteOffer remoteOffer = null;
            try
            {
                remoteOffer = BrokerService.GetBestOffer(Requirements.Xml, uddiTModelKey);
            }
            catch (Exception e)
            {
                e.Message;
            }
            if (remoteOffer == null)
                return null;
            ServiceModel serviceModel = new ServiceModel(remoteOffer.ServiceName, remoteOffer.ServiceAccessPoint, remoteOffer.ServiceDescriptionUrl);
            return new QoSOffer(remoteOffer.Offer, serviceModel);
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

