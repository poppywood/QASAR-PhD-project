using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSProtocolDef : WSQoSAPI.WSQoSAttribute
    {

        private string _desc;
        private string _specUrl;

        public string Description
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
            }
        }

        public string SpecificationUrl
        {
            get
            {
                return _specUrl;
            }
            set
            {
                _specUrl = value;
            }
        }

        public WSQoSProtocolDef()
        {
            _desc = "\uFFFD";
            _specUrl = "\uFFFD";
        }

        public WSQoSProtocolDef(System.Xml.XmlNode src)
        {
            _desc = "\uFFFD";
            _specUrl = "\uFFFD";
            Update(src);
        }

        protected override string GetRootLabel()
        {
            return "protocolDefinition\uFFFD";
        }

        protected override void UpdateProperties()
        {
            // trial
        }

    } // class WSQoSProtocolDef

}

