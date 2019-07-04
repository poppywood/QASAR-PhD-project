using System.Xml;

namespace WSQoSAPI
{

    public class QoSRequirements : QoSDefinition
    {

        public QoSRequirements()
        {
        }

        public QoSRequirements(string name)
        {
            _name = name;
        }

        public QoSRequirements(XmlNode src)
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

