using System;
using System.Xml;

namespace WSQoSAPI
{

    public class SecurityAndTransactionAttribute : WSQoSAPI.WSQoSAttribute
    {

        private WSQoSAPI.NamedItemCollection _Protocols;
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
            _Protocols = new WSQoSAPI.NamedItemCollection();
            _requires = "none\uFFFD";
        }

        public SecurityAndTransactionAttribute(string Name)
        {
            _Protocols = new WSQoSAPI.NamedItemCollection();
            _requires = "none\uFFFD";
            _name = Name;
        }

        public SecurityAndTransactionAttribute(System.Xml.XmlNode Src)
        {
            _Protocols = new WSQoSAPI.NamedItemCollection();
            _requires = "none\uFFFD";
            Update(Src);
        }

        public void AddProtocol(WSQoSAPI.SecAndTASupportAttribute newProtocol)
        {
            _Protocols.Add(newProtocol);
        }

        public WSQoSAPI.SecAndTASupportAttribute GetProtocol(string Name, string Ontology)
        {
            // trial
            return null;
        }

        public WSQoSAPI.SecAndTASupportAttribute GetProtocol(int index)
        {
            return (WSQoSAPI.SecAndTASupportAttribute)_Protocols.ItemAt(index);
        }

        public void RemoveProtocol(WSQoSAPI.SecAndTASupportAttribute toBeRemoved)
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
            catch (System.NullReferenceException)
            {
                _requires = "none\uFFFD";
            }
            _Protocols.Clear();
            System.Xml.XmlNodeList xmlNodeList = ((System.Xml.XmlElement)_xml).GetElementsByTagName("protocol\uFFFD");
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                WSQoSAPI.SecAndTASupportAttribute secAndTASupportAttribute = new WSQoSAPI.SecAndTASupportAttribute(xmlNodeList[i]);
                _Protocols.Add(secAndTASupportAttribute);
            }
            BuildXml();
        }

    } // class SecurityAndTransactionAttribute

}

