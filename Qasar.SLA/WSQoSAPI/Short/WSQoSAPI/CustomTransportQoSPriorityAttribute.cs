using System;
using System.Xml;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class CustomTransportQoSPriorityAttribute : WSQoSAttribute
    {

        private string _ontUrl;
        private int _val;

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

        public int Value
        {
            get
            {
                return _val;
            }
            set
            {
                _val = value;
                BuildXml();
            }
        }

        public CustomTransportQoSPriorityAttribute()
        {
            _val = 0;
            _ontUrl = "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos\uFFFD";
        }

        public CustomTransportQoSPriorityAttribute(string Name, string OntologyUrl)
        {
            _val = 0;
            _ontUrl = "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos\uFFFD";
            _name = Name;
            _ontUrl = OntologyUrl;
            BuildXml();
        }

        public CustomTransportQoSPriorityAttribute(XmlNode src)
        {
            _val = 0;
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
                _val = Int32.Parse(_xml.InnerText);
            }
            catch (NullReferenceException e1)
            {
                e1.Message;
            }
            try
            {
                _ontUrl = _xml.Attributes.GetNamedItem("ontology\uFFFD").Value;
            }
            catch (NullReferenceException e2)
            {
                e2.Message;
            }
        }

    } // class CustomTransportQoSPriorityAttribute

}

