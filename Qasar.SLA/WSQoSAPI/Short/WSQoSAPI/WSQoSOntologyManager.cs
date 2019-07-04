namespace WSQoSAPI
{

    public class WSQoSOntologyManager
    {

        private static NamedItemCollection Ontologies;

        public static int OntologyCount
        {
            get
            {
                return WSQoSOntologyManager.Ontologies.Count;
            }
        }

        static WSQoSOntologyManager()
        {
            WSQoSOntologyManager.Ontologies = new NamedItemCollection();
        }

        public WSQoSOntologyManager()
        {
        }

        public static bool AddOntology(string ontologyUrl)
        {
            WSQoSOntology wsqoSOntology = new WSQoSOntology(ontologyUrl);
            if ((wsqoSOntology != null) && (wsqoSOntology.Count > 0))
            {
                WSQoSOntologyManager.Ontologies.Add(wsqoSOntology);
                return true;
            }
            return false;
        }

        public static WSQoSMetricDef GetMetricDef(string type, string ontologyUrl)
        {
            // trial
            return null;
        }

        public static WSQoSOntology GetOntology(string ontologyUrl)
        {
            return (WSQoSOntology)WSQoSOntologyManager.Ontologies.GetItemByName(ontologyUrl);
        }

        public static WSQoSOntology GetOntology(int index)
        {
            // trial
            return null;
        }

        public static WSQoSPriorityDef GetPriorityDef(string type, string ontologyUrl)
        {
            WSQoSOntology wsqoSOntology = WSQoSOntologyManager.GetOntology(ontologyUrl);
            if (wsqoSOntology == null)
            {
                wsqoSOntology = new WSQoSOntology(ontologyUrl);
                WSQoSOntologyManager.Ontologies.Add(wsqoSOntology);
            }
            if (wsqoSOntology != null)
                return wsqoSOntology.GetPriorityDef(type);
            return null;
        }

        public static WSQoSProtocolDef GetProtocolDef(string type, string ontologyUrl)
        {
            // trial
            return null;
        }

        public static void RemoveOntology(string name)
        {
            WSQoSOntology wsqoSOntology = (WSQoSOntology)WSQoSOntologyManager.Ontologies.GetItemByName(name);
            if (wsqoSOntology != null)
                WSQoSOntologyManager.Ontologies.Remove(wsqoSOntology);
        }

        public static bool ValidateOntology(string ontologyUrl)
        {
            // trial
            return false;
        }

    } // class WSQoSOntologyManager

}

