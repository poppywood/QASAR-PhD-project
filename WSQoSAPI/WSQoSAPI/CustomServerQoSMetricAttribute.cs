using System;
using System.Xml;

namespace WSQoSAPI
{

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Module | System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Enum | System.AttributeTargets.Constructor | System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field | System.AttributeTargets.Event | System.AttributeTargets.Interface | System.AttributeTargets.Parameter | System.AttributeTargets.Delegate | System.AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class CustomServerQoSMetricAttribute : WSQoSAPI.WSQoSAttribute
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

        public CustomServerQoSMetricAttribute(System.Xml.XmlNode src)
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
                return System.String.Concat("\uFFFD", _val);
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

