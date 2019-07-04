using System.Xml;

namespace WSQoSAPI
{

    public class QoSRequirements : WSQoSAPI.QoSDefinition
    {

        public QoSRequirements()
        {
        }

        public QoSRequirements(string name)
        {
            _name = name;
        }

        public QoSRequirements(System.Xml.XmlNode src)
        {
            Update(src);
        }

        protected override string GetRootLabel()
        {
            // trial
            return null;
        }

    } // class QoSRequirements

}

