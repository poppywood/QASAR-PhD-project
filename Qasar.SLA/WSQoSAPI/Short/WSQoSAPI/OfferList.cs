using System.Text;
using System.Threading;

namespace WSQoSAPI
{

    public class OfferList : WSQoSAttribute
    {

        private Mutex InsertingOffer;
        private Mutex InsertingService;
        public bool NoServicesOnLastCheck;
        private NamedItemCollection Offers;
        private NamedItemCollection Services;

        public int OfferCount
        {
            get
            {
                return Offers.Count;
            }
        }

        public int ServiceCount
        {
            get
            {
                return Services.Count;
            }
        }

        public string UddiTModelKey
        {
            get
            {
                return _name;
            }
        }

        public OfferList(string UddiTModelKey)
        {
            Offers = new NamedItemCollection();
            Services = new NamedItemCollection();
            NoServicesOnLastCheck = false;
            InsertingOffer = new Mutex();
            InsertingService = new Mutex();
            _name = UddiTModelKey;
        }

        public void AddOffer(QoSOffer newOffer)
        {
            int i;

            QoSOffer qoSOffer = GetOffer(newOffer.Name, newOffer.Service.Name);
            if (qoSOffer != null)
                RemoveOffer(qoSOffer);
            InsertingOffer.WaitOne();
            if (Offers.Count < 1)
            {
                Offers.Add(newOffer);
            }
            else
            {
                for (i = 0; i < Offers.Count; i++)
                {
                    if (newOffer.Price.IsCheaperThan(GetOffer(i).Price))
                    {
                        Offers.Insert(i, newOffer);
                        break;
                    }
                }
                if (i == Offers.Count)
                    Offers.Add(newOffer);
            }
            InsertingOffer.ReleaseMutex();
        }

        public void AddService(ServiceModel newService)
        {
            // trial
        }

        public QoSOffer GetBestOffer(QoSRequirements Requirements)
        {
            for (int i = 0; i < Offers.Count; i++)
            {
                if (GetOffer(i).Includes(Requirements))
                    return GetOffer(i);
            }
            return null;
        }

        public QoSOffer GetOffer(int index)
        {
            // trial
            return null;
        }

        public QoSOffer GetOffer(string Name)
        {
            return (QoSOffer)Offers.GetItemByName(Name);
        }

        public QoSOffer GetOffer(string OfferName, string ServiceName)
        {
            // trial
            return null;
        }

        public NamedItemCollection GetOffersForService(string UddiServiceKey)
        {
            NamedItemCollection namedItemCollection = new NamedItemCollection();
            for (int i = 0; i < Offers.Count; i++)
            {
                QoSOffer qoSOffer = GetOffer(i);
                if (qoSOffer.Service.UddiServiceKey == UddiServiceKey)
                    namedItemCollection.Add(qoSOffer);
            }
            return namedItemCollection;
        }

        public ServiceModel GetService(string name)
        {
            return (ServiceModel)Services.GetItemByName(name);
        }

        public ServiceModel GetService(int index)
        {
            // trial
            return null;
        }

        public void RemoveOffer(QoSOffer tobeRemoved)
        {
            // trial
        }

        public void RemoveService(ServiceModel tobeRemoved)
        {
            Services.Remove(tobeRemoved);
        }

        protected override string GetInnerXml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < OfferCount; i++)
            {
                stringBuilder.Append(GetOffer(i).Xml.OuterXml);
            }
            return stringBuilder.ToString();
        }

        protected override string GetRootLabel()
        {
            // trial
            return null;
        }

    } // class OfferList

}

