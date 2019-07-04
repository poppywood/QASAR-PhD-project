using System.Xml;

namespace WSQoSAPI
{

    public class OperationQoSInfo : WSQoSAPI.QoSInfo
    {

        public OperationQoSInfo(string Name)
        {
            _name = Name;
        }

        public OperationQoSInfo(System.Xml.XmlNode src)
        {
            _xml = src;
            UpdateProperties();
        }

        protected override string GetRootLabel()
        {
            return "operationQoSInfo\uFFFD";
        }

    } // class OperationQoSInfo

}

