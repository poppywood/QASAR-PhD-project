using System;
using System.Xml;

namespace WSQoSAPI
{

    public class ContractAndMonitoringAttribute : WSQoSAttribute
    {

        private NamedItemCollection _Protocols;
        private string _requires;
        private NamedItemCollection _TrustedParties;

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
            _Protocols = new NamedItemCollection();
            _TrustedParties = new NamedItemCollection();
            _requires = "none\uFFFD";
        }

        public ContractAndMonitoringAttribute(XmlNode src)
        {
            _Protocols = new NamedItemCollection();
            _TrustedParties = new NamedItemCollection();
            _requires = "none\uFFFD";
            Update(src);
        }

        public void AddProtocol(ConAndMonSupportAttribute newProtocol)
        {
            _Protocols.Add(newProtocol);
        }

        public void AddTrustedParty(TrustedParty newParty)
        {
            _Protocols.Add(newParty);
        }

        public ConAndMonSupportAttribute GetProtocol(string Name, string Ontology)
        {
            // trial
            return null;
        }

        public ConAndMonSupportAttribute GetProtocol(int index)
        {
            _Protocols.ItemAt(index).GetType();
            return (ConAndMonSupportAttribute)_Protocols.ItemAt(index);
        }

        public TrustedParty GetTrustedParty(string Name, string Url)
        {
            // trial
            return null;
        }

        public TrustedParty GetTrustedParty(int index)
        {
            return (TrustedParty)_TrustedParties.ItemAt(index);
        }

        public void RemoveProtocol(ConAndMonSupportAttribute toBeRemoved)
        {
            // trial
        }

        public void RemoveTrustedParty(TrustedParty toBeRemoved)
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
            catch (NullReferenceException e)
            {
                e.Message;
            }
            _Protocols.Clear();
            XmlNodeList xmlNodeList1 = ((XmlElement)_xml).GetElementsByTagName("protocol\uFFFD");
            for (int i1 = 0; i1 < xmlNodeList1.Count; i1++)
            {
                ConAndMonSupportAttribute conAndMonSupportAttribute = new ConAndMonSupportAttribute(xmlNodeList1[i1]);
                _Protocols.Add(conAndMonSupportAttribute);
            }
            _TrustedParties.Clear();
            XmlNodeList xmlNodeList2 = ((XmlElement)_xml).GetElementsByTagName("trustedParty\uFFFD");
            for (int i2 = 0; i2 < xmlNodeList2.Count; i2++)
            {
                TrustedParty trustedParty = new TrustedParty(xmlNodeList2[i2]);
                _TrustedParties.Add(trustedParty);
            }
            BuildXml();
        }

    } // class ContractAndMonitoringAttribute

}

