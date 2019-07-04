using System.Xml;

namespace WSQoSAPI
{

    public class DefaultQoSInfo : QoSInfo
    {

        public DefaultQoSInfo()
        {
        }

        public DefaultQoSInfo(XmlNode src)
        {
            _xml = src;
            UpdateProperties();
        }

        protected override string GetRootLabel()
        {
            // trial
            return null;
        }

    } // class DefaultQoSInfo

}

