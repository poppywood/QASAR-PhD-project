using System;
using System.Xml;

namespace WSQoSAPI
{

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Module | System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Enum | System.AttributeTargets.Constructor | System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field | System.AttributeTargets.Event | System.AttributeTargets.Interface | System.AttributeTargets.Parameter | System.AttributeTargets.Delegate | System.AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class CustomTransportQoSPriorityAttribute : WSQoSAPI.WSQoSAttribute
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

        public CustomTransportQoSPriorityAttribute(System.Xml.XmlNode src)
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
                _val = System.Int32.Parse(_xml.InnerText);
            }
            catch (System.NullReferenceException e1)
            {
                //e1.Message;
            }
            try
            {
                _ontUrl = _xml.Attributes.GetNamedItem("ontology\uFFFD").Value;
            }
            catch (System.NullReferenceException e2)
            {
                //e2.Message;
            }
        }

    } // class CustomTransportQoSPriorityAttribute

}

