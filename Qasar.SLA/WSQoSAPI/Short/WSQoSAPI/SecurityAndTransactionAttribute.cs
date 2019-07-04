using System;
using System.Xml;

namespace WSQoSAPI
{

    public class SecurityAndTransactionAttribute : WSQoSAttribute
    {

        private NamedItemCollection _Protocols;
        private string _requires;

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

        public SecurityAndTransactionAttribute()
        {
            _Protocols = new NamedItemCollection();
            _requires = "none\uFFFD";
        }

        public SecurityAndTransactionAttribute(string Name)
        {
            _Protocols = new NamedItemCollection();
            _requires = "none\uFFFD";
            _name = Name;
        }

        public SecurityAndTransactionAttribute(XmlNode Src)
        {
            _Protocols = new NamedItemCollection();
            _requires = "none\uFFFD";
            Update(Src);
        }

        public void AddProtocol(SecAndTASupportAttribute newProtocol)
        {
            _Protocols.Add(newProtocol);
        }

        public SecAndTASupportAttribute GetProtocol(string Name, string Ontology)
        {
            // trial
            return null;
        }

        public SecAndTASupportAttribute GetProtocol(int index)
        {
            return (SecAndTASupportAttribute)_Protocols.ItemAt(index);
        }

        public void RemoveProtocol(SecAndTASupportAttribute toBeRemoved)
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
            catch (NullReferenceException)
            {
                _requires = "none\uFFFD";
            }
            _Protocols.Clear();
            XmlNodeList xmlNodeList = ((XmlElement)_xml).GetElementsByTagName("protocol\uFFFD");
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                SecAndTASupportAttribute secAndTASupportAttribute = new SecAndTASupportAttribute(xmlNodeList[i]);
                _Protocols.Add(secAndTASupportAttribute);
            }
            BuildXml();
        }

    } // class SecurityAndTransactionAttribute

}

