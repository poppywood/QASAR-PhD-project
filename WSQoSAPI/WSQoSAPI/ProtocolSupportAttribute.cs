using System;
using System.Xml;

namespace WSQoSAPI
{

    public class ProtocolSupportAttribute : WSQoSAPI.WSQoSAttribute
    {

        protected string _ontUrl;

        public string OntologyUrl
        {
            get
            {
                return _ontUrl;
            }
            set
            {
                _ontUrl = value;
                BuildXml();
            }
        }

        public ProtocolSupportAttribute()
        {
            _ontUrl = "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos\uFFFD";
        }

        public ProtocolSupportAttribute(string Name, string OntologyUrl)
        {
            _ontUrl = "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos\uFFFD";
            _name = Name;
            _ontUrl = OntologyUrl;
            BuildXml();
        }

        public ProtocolSupportAttribute(System.Xml.XmlNode src)
        {
            _ontUrl = "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos\uFFFD";
            Update(src);
        }

        protected override string GetAttributes()
        {
            if (_ontUrl != "\uFFFD" && _ontUrl != "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos\uFFFD")
                return " ontology=\"\uFFFD" + _ontUrl + "\"\uFFFD";
            return "\uFFFD";
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
                _ontUrl = _xml.Attributes.GetNamedItem("ontology\uFFFD").Value;
            }
            catch (System.NullReferenceException e)
            {
                //e.Message;
            }
        }

    } // class ProtocolSupportAttribute

}

