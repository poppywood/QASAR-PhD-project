using System.Xml;

namespace WSQoSAPI
{

    public class OperationQoSInfo : QoSInfo
    {

        public OperationQoSInfo(string Name)
        {
            _name = Name;
        }

        public OperationQoSInfo(XmlNode src)
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

