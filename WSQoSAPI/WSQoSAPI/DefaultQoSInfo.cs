using System.Xml;

namespace WSQoSAPI
{

    public class DefaultQoSInfo : WSQoSAPI.QoSInfo
    {

        public DefaultQoSInfo()
        {
        }

        public DefaultQoSInfo(System.Xml.XmlNode src)
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

