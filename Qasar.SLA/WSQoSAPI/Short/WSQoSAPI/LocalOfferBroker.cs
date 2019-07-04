using System.IO;
using System.Threading;
using System.Xml;
using Microsoft.Uddi;

namespace WSQoSAPI
{

    public class LocalOfferBroker : WSQoSOfferBroker
    {

        private string ConfigFilePath;
        private Thread DeleteExpiredOffersThread;
        public int DeleteExpireOffersInterval;
        public int OfferUpdateInterval;
        public int ServiceUpdateInterval;
        public string TestID;
        private string[] TrustedProviders;
        private Thread UpdateOffersThread;
        public int UpdateProvidersInterval;
        private Thread UpdateProvidersThread;
        private Thread UpdateServicesThread;

        private static NamedItemCollection OfferLists;
        private static ServiceUpdateList ServicesToBeUpdated;

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
            LocalOfferBroker.OfferLists = new NamedItemCollection();
            LocalOfferBroker.ServicesToBeUpdated = new ServiceUpdateList();
        }

        public void AddService(ServiceModel newService)
        {
            OfferList offerList = LocalOfferBroker.GetOfferList(newService.UddiTModelKey);
            if (offerList == null)
            {
                offerList = new OfferList(newService.UddiTModelKey);
                LocalOfferBroker.OfferLists.Add(offerList);
            }
            offerList.AddService(newService);
        }

        public void AutoUpdateProviders()
        {
            while (true)
            {
                UpdateProviders();
                Thread.Sleep(UpdateProvidersInterval);
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

        public QoSOffer GetBestOffer(QoSRequirements Requirements, string UddiTModelKey)
        {
            // trial
            return null;
        }

        public OfferList GetBrokersOfferList(string uddiTModelKey)
        {
            // trial
            return null;
        }

        public XmlNode GetProviderList()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(ConfigFilePath);
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("TrustedProviders\uFFFD");
            if (xmlNodeList.Count > 0)
                return xmlNodeList[0];
            return null;
        }

        public string GetUddiReqistryLocation()
        {
            // trial
            return null;
        }

        private ServiceList GetUddiServiceList(string uddiTModelKey)
        {
            // trial
            return null;
        }

        private void LoadConfig()
        {
            XmlDocument xmlDocument = new XmlDocument();
            bool flag = true;
            try
            {
                xmlDocument.Load(ConfigFilePath);
            }
            catch (FileNotFoundException e1)
            {
                flag = false;
                throw new InvalidBrokerConfigurationException("The specified file '\uFFFD" + ConfigFilePath + "' could not be found. Details ------> \n \uFFFD" + e1.Message);
            }
            catch (XmlException e2)
            {
                flag = false;
                throw new InvalidBrokerConfigurationException("Could not read Xml of file '\uFFFD" + ConfigFilePath + "'. Details ------> \n \uFFFD" + e2.Message);
            }
            if (flag)
            {
                XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("UddiRegistryUrl\uFFFD");
                if (xmlNodeList.Count == 0)
                    throw new InvalidBrokerConfigurationException("No node 'UddiRegistryUrl' found in configuration file '\uFFFD" + ConfigFilePath + "'.\uFFFD");
                Inquire.Url = xmlNodeList[0].InnerText;
                try
                {
                    UpdateProviders();
                }
                catch (NoTrustedProvidersException e3)
                {
                    throw new InvalidBrokerConfigurationException(e3.Message);
                }
            }
        }

        public void SetUddiReqistryLocation(string url)
        {
            Inquire.Url = url;
        }

        private void StartThreads()
        {
            // trial
        }

        public void UpdateOffers()
        {
            // trial
        }

        public void UpdateOffersForOfferList(OfferList CurrentOfferList)
        {
            for (int i = 0; i < CurrentOfferList.ServiceCount; i++)
            {
                UpdateOffersForService(CurrentOfferList.GetService(i));
            }
        }

        public void UpdateOffersForService(ServiceModel CurrentService)
        {
            LocalOfferBroker.ServicesToBeUpdated.AddService(CurrentService);
            (new Thread(new ThreadStart(LocalOfferBroker.UpdateOffersForNextService))).Start();
        }

        public void UpdateProviders()
        {
            // trial
        }

        public void UpdateServices()
        {
            while (true)
            {
                for (int i = 0; i < LocalOfferBroker.OfferLists.Count; i++)
                {
                    UpdateServicesForOfferList(LocalOfferBroker.GetOfferList(i));
                }
                Thread.Sleep(ServiceUpdateInterval);
            }
        }

        public void UpdateServicesForOfferList(OfferList CurrentOfferList)
        {
            // trial
        }

        public static void AddOffer(QoSOffer newOffer)
        {
            LocalOfferBroker.GetOfferList(newOffer.Service.UddiTModelKey).AddOffer(newOffer);
        }

        public static void AddOfferList(OfferList newOfferList)
        {
            // trial
        }

        public static OfferList GetOfferList(int index)
        {
            return (OfferList)LocalOfferBroker.OfferLists.ItemAt(index);
        }

        public static OfferList GetOfferList(string uddiTModelKey)
        {
            return (OfferList)LocalOfferBroker.OfferLists.GetItemByName(uddiTModelKey);
        }

        public static void RemoveOffer(QoSOffer toBeRemoved)
        {
            // trial
        }

        public static void RemoveOfferList(OfferList toBeRemoved)
        {
            LocalOfferBroker.OfferLists.Remove(toBeRemoved);
        }

        public static void RemoveService(ServiceModel toBeRemoved)
        {
            // trial
        }

        public static void RetrieveOffers(ServiceModel Service, string Url)
        {
            // trial
        }

        public static void UpdateOffersForNextService()
        {
            ServiceModel serviceModel = null;
            try
            {
                serviceModel = LocalOfferBroker.ServicesToBeUpdated.GetNextService();
            }
            catch (NoServicesInListException)
            {
                return;
            }
            LocalOfferBroker.RetrieveOffers(serviceModel, serviceModel.DescriptionUrl);
            Thread.CurrentThread.Abort();
        }

    } // class LocalOfferBroker

}

