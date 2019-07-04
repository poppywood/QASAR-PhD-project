using System.Text;
using System.Threading;

namespace WSQoSAPI
{

    public class OfferList : WSQoSAPI.WSQoSAttribute
    {

        private System.Threading.Mutex InsertingOffer;
        private System.Threading.Mutex InsertingService;
        public bool NoServicesOnLastCheck;
        private WSQoSAPI.NamedItemCollection Offers;
        private WSQoSAPI.NamedItemCollection Services;

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
            Offers = new WSQoSAPI.NamedItemCollection();
            Services = new WSQoSAPI.NamedItemCollection();
            NoServicesOnLastCheck = false;
            InsertingOffer = new System.Threading.Mutex();
            InsertingService = new System.Threading.Mutex();
            _name = UddiTModelKey;
        }

        public void AddOffer(WSQoSAPI.QoSOffer newOffer)
        {
            int i;

            WSQoSAPI.QoSOffer qoSOffer = GetOffer(newOffer.Name, newOffer.Service.Name);
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

        public void AddService(WSQoSAPI.ServiceModel newService)
        {
            // trial
        }

        public WSQoSAPI.QoSOffer GetBestOffer(WSQoSAPI.QoSRequirements Requirements)
        {
            for (int i = 0; i < Offers.Count; i++)
            {
                if (GetOffer(i).Includes(Requirements))
                    return GetOffer(i);
            }
            return null;
        }

        public WSQoSAPI.QoSOffer GetOffer(int index)
        {
            // trial
            return null;
        }

        public WSQoSAPI.QoSOffer GetOffer(string Name)
        {
            return (WSQoSAPI.QoSOffer)Offers.GetItemByName(Name);
        }

        public WSQoSAPI.QoSOffer GetOffer(string OfferName, string ServiceName)
        {
            // trial
            return null;
        }

        public WSQoSAPI.NamedItemCollection GetOffersForService(string UddiServiceKey)
        {
            WSQoSAPI.NamedItemCollection namedItemCollection = new WSQoSAPI.NamedItemCollection();
            for (int i = 0; i < Offers.Count; i++)
            {
                WSQoSAPI.QoSOffer qoSOffer = GetOffer(i);
                if (qoSOffer.Service.UddiServiceKey == UddiServiceKey)
                    namedItemCollection.Add(qoSOffer);
            }
            return namedItemCollection;
        }

        public WSQoSAPI.ServiceModel GetService(string name)
        {
            return (WSQoSAPI.ServiceModel)Services.GetItemByName(name);
        }

        public WSQoSAPI.ServiceModel GetService(int index)
        {
            // trial
            return null;
        }

        public void RemoveOffer(WSQoSAPI.QoSOffer tobeRemoved)
        {
            // trial
        }

        public void RemoveService(WSQoSAPI.ServiceModel tobeRemoved)
        {
            Services.Remove(tobeRemoved);
        }

        protected override string GetInnerXml()
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
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

