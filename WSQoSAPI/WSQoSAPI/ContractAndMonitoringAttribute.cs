using System;
using System.Xml;

namespace WSQoSAPI
{

    public class ContractAndMonitoringAttribute : WSQoSAPI.WSQoSAttribute
    {

        private WSQoSAPI.NamedItemCollection _Protocols;
        private string _requires;
        private WSQoSAPI.NamedItemCollection _TrustedParties;

        public int ProtocolCount
        {
            get
            {
                return _Protocols.Count;
            }
        }

        public string Requires
        {
            get
            {
                return _requires;
            }
            set
            {
                _requires = value;
            }
        }

        public int TrustedPartyCount
        {
            get
            {
                return _TrustedParties.Count;
            }
        }

        public ContractAndMonitoringAttribute()
        {
            _Protocols = new WSQoSAPI.NamedItemCollection();
            _TrustedParties = new WSQoSAPI.NamedItemCollection();
            _requires = "none\uFFFD";
        }

        public ContractAndMonitoringAttribute(System.Xml.XmlNode src)
        {
            _Protocols = new WSQoSAPI.NamedItemCollection();
            _TrustedParties = new WSQoSAPI.NamedItemCollection();
            _requires = "none\uFFFD";
            Update(src);
        }

        public void AddProtocol(WSQoSAPI.ConAndMonSupportAttribute newProtocol)
        {
            _Protocols.Add(newProtocol);
        }

        public void AddTrustedParty(WSQoSAPI.TrustedParty newParty)
        {
            _Protocols.Add(newParty);
        }

        public WSQoSAPI.ConAndMonSupportAttribute GetProtocol(string Name, string Ontology)
        {
            // trial
            return null;
        }

        public WSQoSAPI.ConAndMonSupportAttribute GetProtocol(int index)
        {
            _Protocols.ItemAt(index).GetType();
            return (WSQoSAPI.ConAndMonSupportAttribute)_Protocols.ItemAt(index);
        }

        public WSQoSAPI.TrustedParty GetTrustedParty(string Name, string Url)
        {
            // trial
            return null;
        }

        public WSQoSAPI.TrustedParty GetTrustedParty(int index)
        {
            return (WSQoSAPI.TrustedParty)_TrustedParties.ItemAt(index);
        }

        public void RemoveProtocol(WSQoSAPI.ConAndMonSupportAttribute toBeRemoved)
        {
            // trial
        }

        public void RemoveTrustedParty(WSQoSAPI.TrustedParty toBeRemoved)
        {
            // trial
        }

        protected override string GetAttributes()
        {
            return " requires=\"\uFFFD" + _requires + "\"\uFFFD";
        }

        protected override string GetInnerXml()
        {
            // trial
            return null;
        }

        protected override string GetRootLabel()
        {
            // trial
            return null;
        }

        protected override void UpdateProperties()
        {
            try
            {
                _requires = _xml.Attributes.GetNamedItem("requires\uFFFD").Value;
            }
            catch (System.NullReferenceException e)
            {
                //e.Message;
            }
            _Protocols.Clear();
            System.Xml.XmlNodeList xmlNodeList1 = ((System.Xml.XmlElement)_xml).GetElementsByTagName("protocol\uFFFD");
            for (int i1 = 0; i1 < xmlNodeList1.Count; i1++)
            {
                WSQoSAPI.ConAndMonSupportAttribute conAndMonSupportAttribute = new WSQoSAPI.ConAndMonSupportAttribute(xmlNodeList1[i1]);
                _Protocols.Add(conAndMonSupportAttribute);
            }
            _TrustedParties.Clear();
            System.Xml.XmlNodeList xmlNodeList2 = ((System.Xml.XmlElement)_xml).GetElementsByTagName("trustedParty\uFFFD");
            for (int i2 = 0; i2 < xmlNodeList2.Count; i2++)
            {
                WSQoSAPI.TrustedParty trustedParty = new WSQoSAPI.TrustedParty(xmlNodeList2[i2]);
                _TrustedParties.Add(trustedParty);
            }
            BuildXml();
        }

    } // class ContractAndMonitoringAttribute

}

