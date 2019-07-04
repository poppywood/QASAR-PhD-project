using System;
using System.Xml;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class CustomServerQoSMetricAttribute : WSQoSAttribute
    {

        private string _ontUrl;
        private double _val;

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

        public double Value
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

        public CustomServerQoSMetricAttribute()
        {
            _ontUrl = "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos\uFFFD";
        }

        public CustomServerQoSMetricAttribute(string Name, string OntologyUrl)
        {
            _ontUrl = "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos\uFFFD";
            _name = Name;
            _ontUrl = OntologyUrl;
            BuildXml();
        }

        public CustomServerQoSMetricAttribute(XmlNode src)
        {
            _ontUrl = "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos\uFFFD";
            Update(src);
        }

        protected override string GetAttributes()
        {
            // trial
            return null;
        }

        protected override string GetInnerXml()
        {
            if (_val > 0.0)
                return String.Concat("\uFFFD", _val);
            return null;
        }

        protected override string GetRootLabel()
        {
            return "customMetric\uFFFD";
        }

        protected override void UpdateProperties()
        {
            // trial
        }

    } // class CustomServerQoSMetricAttribute

}

