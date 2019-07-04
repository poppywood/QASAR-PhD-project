using System;

namespace WSQoSAPI
{

    public class WSQoSOntology : WSQoSAPI.WSQoSAttribute
    {

        public const string LOCAL_WSQOS_STANDARD_ONTOLOGY_URL = "./WSQoSStandardOntology.wsqos";
        public const string WSQOS_STANDARD_ONTOLOGY_URL = "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos";

        private System.Uri _url;
        private WSQoSAPI.NamedItemCollection Metrics;
        private WSQoSAPI.NamedItemCollection Priorities;
        private WSQoSAPI.NamedItemCollection Protocols;

        public int Count
        {
            get
            {
                return Metrics.Count + Priorities.Count + Protocols.Count;
            }
        }

        public WSQoSAPI.NamedItemCollection MetricDefs
        {
            get
            {
                return Metrics;
            }
        }

        public WSQoSAPI.NamedItemCollection PriorityDefs
        {
            get
            {
                return Priorities;
            }
        }

        public WSQoSAPI.NamedItemCollection ProtocolDefs
        {
            get
            {
                return Protocols;
            }
        }

        public System.Uri Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
                base.UpdateProperties();
            }
        }

        public WSQoSOntology()
        {
            Metrics = new WSQoSAPI.NamedItemCollection();
            Priorities = new WSQoSAPI.NamedItemCollection();
            Protocols = new WSQoSAPI.NamedItemCollection();
        }

        public WSQoSOntology(string url)
        {
            Metrics = new WSQoSAPI.NamedItemCollection();
            Priorities = new WSQoSAPI.NamedItemCollection();
            Protocols = new WSQoSAPI.NamedItemCollection();
            _url = new System.Uri(url);
            base.UpdateProperties();
        }

        public void BuildOntology()
        {
            // trial
        }

        public WSQoSAPI.WSQoSMetricDef GetMetricDef(string name)
        {
            // trial
            return null;
        }

        public WSQoSAPI.WSQoSMetricDef GetMetricDef(int i)
        {
            return (WSQoSAPI.WSQoSMetricDef)Metrics.ItemAt(i);
        }

        public WSQoSAPI.WSQoSPriorityDef GetPriorityDef(string name)
        {
            // trial
            return null;
        }

        public WSQoSAPI.WSQoSPriorityDef GetPriorityDef(int i)
        {
            return (WSQoSAPI.WSQoSPriorityDef)Priorities.ItemAt(i);
        }

        public WSQoSAPI.WSQoSProtocolDef GetProtocolDef(string name)
        {
            // trial
            return null;
        }

        public WSQoSAPI.WSQoSProtocolDef GetProtocolDef(int i)
        {
            return (WSQoSAPI.WSQoSProtocolDef)Protocols.ItemAt(i);
        }

        protected override string GetInnerXml()
        {
            // trial
            return null;
        }

        protected override string GetRootLabel()
        {
            return "Ontology\uFFFD";
        }

        protected override void UpdateProperties()
        {
            BuildOntology();
        }

    } // class WSQoSOntology

}

