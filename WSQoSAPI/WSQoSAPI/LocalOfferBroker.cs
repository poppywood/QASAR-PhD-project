using System.IO;
using System.Threading;
using System.Xml;
using Microsoft.Uddi;

namespace WSQoSAPI
{

    public class LocalOfferBroker : WSQoSAPI.WSQoSOfferBroker
    {

        private string ConfigFilePath;
        private System.Threading.Thread DeleteExpiredOffersThread;
        public int DeleteExpireOffersInterval;
        public int OfferUpdateInterval;
        public int ServiceUpdateInterval;
        public string TestID;
        private string[] TrustedProviders;
        private System.Threading.Thread UpdateOffersThread;
        public int UpdateProvidersInterval;
        private System.Threading.Thread UpdateProvidersThread;
        private System.Threading.Thread UpdateServicesThread;

        private static WSQoSAPI.NamedItemCollection OfferLists;
        private static WSQoSAPI.ServiceUpdateList ServicesToBeUpdated;

        public LocalOfferBroker()
        {
            ConfigFilePath = "./LocalBroker.config\uFFFD";
            OfferUpdateInterval = 60000;
            ServiceUpdateInterval = 300000;
            DeleteExpireOffersInterval = 1000;
            UpdateProvidersInterval = 300000;
            TestID = "unspecified\uFFFD";
            TestID = "not set\uFFFD";
            LoadConfig();
            StartThreads();
        }

        public LocalOfferBroker(string ConfigFileName)
        {
            ConfigFilePath = "./LocalBroker.config\uFFFD";
            OfferUpdateInterval = 60000;
            ServiceUpdateInterval = 300000;
            DeleteExpireOffersInterval = 1000;
            UpdateProvidersInterval = 300000;
            TestID = "unspecified\uFFFD";
            ConfigFilePath = ConfigFileName;
            TestID = "not set\uFFFD";
            LoadConfig();
            StartThreads();
        }

        static LocalOfferBroker()
        {
            WSQoSAPI.LocalOfferBroker.OfferLists = new WSQoSAPI.NamedItemCollection();
            WSQoSAPI.LocalOfferBroker.ServicesToBeUpdated = new WSQoSAPI.ServiceUpdateList();
        }

        public void AddService(WSQoSAPI.ServiceModel newService)
        {
            WSQoSAPI.OfferList offerList = WSQoSAPI.LocalOfferBroker.GetOfferList(newService.UddiTModelKey);
            if (offerList == null)
            {
                offerList = new WSQoSAPI.OfferList(newService.UddiTModelKey);
                WSQoSAPI.LocalOfferBroker.OfferLists.Add(offerList);
            }
            offerList.AddService(newService);
        }

        public void AutoUpdateProviders()
        {
            while (true)
            {
                UpdateProviders();
                System.Threading.Thread.Sleep(UpdateProvidersInterval);
            }
        }

        public void DeleteExpiredOffers()
        {
            // trial
        }

        public void Dispose()
        {
            UpdateProvidersThread.Abort();
            UpdateServicesThread.Abort();
            UpdateOffersThread.Abort();
            DeleteExpiredOffersThread.Abort();
        }

        public WSQoSAPI.QoSOffer GetBestOffer(WSQoSAPI.QoSRequirements Requirements, string UddiTModelKey)
        {
            // trial
            return null;
        }

        public WSQoSAPI.OfferList GetBrokersOfferList(string uddiTModelKey)
        {
            // trial
            return null;
        }

        public System.Xml.XmlNode GetProviderList()
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(ConfigFilePath);
            System.Xml.XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("TrustedProviders\uFFFD");
            if (xmlNodeList.Count > 0)
                return xmlNodeList[0];
            return null;
        }

        public string GetUddiReqistryLocation()
        {
            // trial
            return null;
        }

        private Microsoft.Uddi.ServiceList GetUddiServiceList(string uddiTModelKey)
        {
            // trial
            return null;
        }

        private void LoadConfig()
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            bool flag = true;
            try
            {
                xmlDocument.Load(ConfigFilePath);
            }
            catch (System.IO.FileNotFoundException e1)
            {
                flag = false;
                throw new WSQoSAPI.InvalidBrokerConfigurationException("The specified file '\uFFFD" + ConfigFilePath + "' could not be found. Details ------> \n \uFFFD" + e1.Message);
            }
            catch (System.Xml.XmlException e2)
            {
                flag = false;
                throw new WSQoSAPI.InvalidBrokerConfigurationException("Could not read Xml of file '\uFFFD" + ConfigFilePath + "'. Details ------> \n \uFFFD" + e2.Message);
            }
            if (flag)
            {
                System.Xml.XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("UddiRegistryUrl\uFFFD");
                if (xmlNodeList.Count == 0)
                    throw new WSQoSAPI.InvalidBrokerConfigurationException("No node 'UddiRegistryUrl' found in configuration file '\uFFFD" + ConfigFilePath + "'.\uFFFD");
                Microsoft.Uddi.Inquire.Url = xmlNodeList[0].InnerText;
                try
                {
                    UpdateProviders();
                }
                catch (WSQoSAPI.NoTrustedProvidersException e3)
                {
                    throw new WSQoSAPI.InvalidBrokerConfigurationException(e3.Message);
                }
            }
        }

        public void SetUddiReqistryLocation(string url)
        {
            Microsoft.Uddi.Inquire.Url = url;
        }

        private void StartThreads()
        {
            // trial
        }

        public void UpdateOffers()
        {
            // trial
        }

        public void UpdateOffersForOfferList(WSQoSAPI.OfferList CurrentOfferList)
        {
            for (int i = 0; i < CurrentOfferList.ServiceCount; i++)
            {
                UpdateOffersForService(CurrentOfferList.GetService(i));
            }
        }

        public void UpdateOffersForService(WSQoSAPI.ServiceModel CurrentService)
        {
            WSQoSAPI.LocalOfferBroker.ServicesToBeUpdated.AddService(CurrentService);
            (new System.Threading.Thread(new System.Threading.ThreadStart(WSQoSAPI.LocalOfferBroker.UpdateOffersForNextService))).Start();
        }

        public void UpdateProviders()
        {
            // trial
        }

        public void UpdateServices()
        {
            while (true)
            {
                for (int i = 0; i < WSQoSAPI.LocalOfferBroker.OfferLists.Count; i++)
                {
                    UpdateServicesForOfferList(WSQoSAPI.LocalOfferBroker.GetOfferList(i));
                }
                System.Threading.Thread.Sleep(ServiceUpdateInterval);
            }
        }

        public void UpdateServicesForOfferList(WSQoSAPI.OfferList CurrentOfferList)
        {
            // trial
        }

        public static void AddOffer(WSQoSAPI.QoSOffer newOffer)
        {
            WSQoSAPI.LocalOfferBroker.GetOfferList(newOffer.Service.UddiTModelKey).AddOffer(newOffer);
        }

        public static void AddOfferList(WSQoSAPI.OfferList newOfferList)
        {
            // trial
        }

        public static WSQoSAPI.OfferList GetOfferList(int index)
        {
            return (WSQoSAPI.OfferList)WSQoSAPI.LocalOfferBroker.OfferLists.ItemAt(index);
        }

        public static WSQoSAPI.OfferList GetOfferList(string uddiTModelKey)
        {
            return (WSQoSAPI.OfferList)WSQoSAPI.LocalOfferBroker.OfferLists.GetItemByName(uddiTModelKey);
        }

        public static void RemoveOffer(WSQoSAPI.QoSOffer toBeRemoved)
        {
            // trial
        }

        public static void RemoveOfferList(WSQoSAPI.OfferList toBeRemoved)
        {
            WSQoSAPI.LocalOfferBroker.OfferLists.Remove(toBeRemoved);
        }

        public static void RemoveService(WSQoSAPI.ServiceModel toBeRemoved)
        {
            // trial
        }

        public static void RetrieveOffers(WSQoSAPI.ServiceModel Service, string Url)
        {
            // trial
        }

        public static void UpdateOffersForNextService()
        {
            WSQoSAPI.ServiceModel serviceModel = null;
            try
            {
                serviceModel = WSQoSAPI.LocalOfferBroker.ServicesToBeUpdated.GetNextService();
            }
            catch (WSQoSAPI.NoServicesInListException)
            {
                return;
            }
            WSQoSAPI.LocalOfferBroker.RetrieveOffers(serviceModel, serviceModel.DescriptionUrl);
            System.Threading.Thread.CurrentThread.Abort();
        }

    } // class LocalOfferBroker

}

