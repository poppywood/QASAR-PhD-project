namespace WSQoSAPI
{

    public class WSQoSOntologyManager
    {

        private static WSQoSAPI.NamedItemCollection Ontologies;

        public static int OntologyCount
        {
            get
            {
                return WSQoSAPI.WSQoSOntologyManager.Ontologies.Count;
            }
        }

        static WSQoSOntologyManager()
        {
            WSQoSAPI.WSQoSOntologyManager.Ontologies = new WSQoSAPI.NamedItemCollection();
        }

        public WSQoSOntologyManager()
        {
        }

        public static bool AddOntology(string ontologyUrl)
        {
            WSQoSAPI.WSQoSOntology wsqoSOntology = new WSQoSAPI.WSQoSOntology(ontologyUrl);
            if ((wsqoSOntology != null) && (wsqoSOntology.Count > 0))
            {
                WSQoSAPI.WSQoSOntologyManager.Ontologies.Add(wsqoSOntology);
                return true;
            }
            return false;
        }

        public static WSQoSAPI.WSQoSMetricDef GetMetricDef(string type, string ontologyUrl)
        {
            // trial
            return null;
        }

        public static WSQoSAPI.WSQoSOntology GetOntology(string ontologyUrl)
        {
            return (WSQoSAPI.WSQoSOntology)WSQoSAPI.WSQoSOntologyManager.Ontologies.GetItemByName(ontologyUrl);
        }

        public static WSQoSAPI.WSQoSOntology GetOntology(int index)
        {
            // trial
            return null;
        }

        public static WSQoSAPI.WSQoSPriorityDef GetPriorityDef(string type, string ontologyUrl)
        {
            WSQoSAPI.WSQoSOntology wsqoSOntology = WSQoSAPI.WSQoSOntologyManager.GetOntology(ontologyUrl);
            if (wsqoSOntology == null)
            {
                wsqoSOntology = new WSQoSAPI.WSQoSOntology(ontologyUrl);
                WSQoSAPI.WSQoSOntologyManager.Ontologies.Add(wsqoSOntology);
            }
            if (wsqoSOntology != null)
                return wsqoSOntology.GetPriorityDef(type);
            return null;
        }

        public static WSQoSAPI.WSQoSProtocolDef GetProtocolDef(string type, string ontologyUrl)
        {
            // trial
            return null;
        }

        public static void RemoveOntology(string name)
        {
            WSQoSAPI.WSQoSOntology wsqoSOntology = (WSQoSAPI.WSQoSOntology)WSQoSAPI.WSQoSOntologyManager.Ontologies.GetItemByName(name);
            if (wsqoSOntology != null)
                WSQoSAPI.WSQoSOntologyManager.Ontologies.Remove(wsqoSOntology);
        }

        public static bool ValidateOntology(string ontologyUrl)
        {
            // trial
            return false;
        }

    } // class WSQoSOntologyManager

}

