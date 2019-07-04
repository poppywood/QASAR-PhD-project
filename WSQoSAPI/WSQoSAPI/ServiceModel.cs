using Microsoft.Uddi;

namespace WSQoSAPI
{

    public class ServiceModel : WSQoSAPI.NamedItem
    {

        private string _accessPointUrl;
        private string _descriptionUrl;
        private string _name;
        private string _uddiServiceKey;
        private string _uddiTModelKey;
        public bool NoOffersOnLastCheck;

        public string AccessPointUrl
        {
            get
            {
                return _accessPointUrl;
            }
        }

        public string DescriptionUrl
        {
            get
            {
                return _descriptionUrl;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string UddiServiceKey
        {
            get
            {
                return _uddiServiceKey;
            }
        }

        public string UddiTModelKey
        {
            get
            {
                return _uddiTModelKey;
            }
        }

        public ServiceModel(string Name, string AccessPointUrl, string DescriptionUrl)
        {
            NoOffersOnLastCheck = false;
            _name = Name;
            _accessPointUrl = AccessPointUrl;
            _descriptionUrl = DescriptionUrl;
        }

        public ServiceModel(string UddiServiceKey, string UddiTModelKey)
        {
            NoOffersOnLastCheck = false;
            _uddiServiceKey = UddiServiceKey;
            _uddiTModelKey = UddiTModelKey;
            Microsoft.Uddi.GetServiceDetail getServiceDetail = new Microsoft.Uddi.GetServiceDetail();
            getServiceDetail.ServiceKeys.Add(UddiServiceKey);
            Microsoft.Uddi.ServiceDetail serviceDetail = new Microsoft.Uddi.ServiceDetail();
            serviceDetail = getServiceDetail.Send();
            _name = serviceDetail.BusinessServices[0].Names[0].Text;
            _accessPointUrl = serviceDetail.BusinessServices[0].BindingTemplates[0].AccessPoint.Text;
            for (int i = 0; i < serviceDetail.BusinessServices[0].BindingTemplates[0].TModelInstanceDetail.TModelInstanceInfos.Count; i++)
            {
                if (serviceDetail.BusinessServices[0].BindingTemplates[0].TModelInstanceDetail.TModelInstanceInfos[i].TModelKey == UddiTModelKey)
                    _descriptionUrl = serviceDetail.BusinessServices[0].BindingTemplates[0].TModelInstanceDetail.TModelInstanceInfos[i].InstanceDetail.OverviewDoc.OverviewURL.ToString();
            }
        }

        public string GetName()
        {
            // trial
            return null;
        }

    } // class ServiceModel

}

