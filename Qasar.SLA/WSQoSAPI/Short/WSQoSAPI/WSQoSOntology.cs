using System;

namespace WSQoSAPI
{

    public class WSQoSOntology : WSQoSAttribute
    {

        public const string LOCAL_WSQOS_STANDARD_ONTOLOGY_URL = "./WSQoSStandardOntology.wsqos";
        public const string WSQOS_STANDARD_ONTOLOGY_URL = "http://www.inf.fu-berlin.de/inst/ag-tech/wsqos/ontologies/WSQoSStandardOntology.wsqos";

        private Uri _url;
        private NamedItemCollection Metrics;
        private NamedItemCollection Priorities;
        private NamedItemCollection Protocols;

        public int Count
        {
            get
            {
                return Metrics.Count + Priorities.Count + Protocols.Count;
            }
        }

        public NamedItemCollection MetricDefs
        {
            get
            {
                return Metrics;
            }
        }

        public NamedItemCollection PriorityDefs
        {
            get
            {
                return Priorities;
            }
        }

        public NamedItemCollection ProtocolDefs
        {
            get
            {
                return Protocols;
            }
        }

        public Uri Url
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
            Metrics = new NamedItemCollection();
            Priorities = new NamedItemCollection();
            Protocols = new NamedItemCollection();
        }

        public WSQoSOntology(string url)
        {
            Metrics = new NamedItemCollection();
            Priorities = new NamedItemCollection();
            Protocols = new NamedItemCollection();
            _url = new Uri(url);
            base.UpdateProperties();
        }

        public void BuildOntology()
        {
            // trial
        }

        public WSQoSMetricDef GetMetricDef(string name)
        {
            // trial
            return null;
        }

        public WSQoSMetricDef GetMetricDef(int i)
        {
            return (WSQoSMetricDef)Metrics.ItemAt(i);
        }

        public WSQoSPriorityDef GetPriorityDef(string name)
        {
            // trial
            return null;
        }

        public WSQoSPriorityDef GetPriorityDef(int i)
        {
            return (WSQoSPriorityDef)Priorities.ItemAt(i);
        }

        public WSQoSProtocolDef GetProtocolDef(string name)
        {
            // trial
            return null;
        }

        public WSQoSProtocolDef GetProtocolDef(int i)
        {
            return (WSQoSProtocolDef)Protocols.ItemAt(i);
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

