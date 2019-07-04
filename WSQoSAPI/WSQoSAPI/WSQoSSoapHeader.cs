using System.Web.Services.Protocols;
using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSSoapHeader : System.Web.Services.Protocols.SoapHeader
    {

        private string _selOff;
        private System.Xml.XmlElement _xml;

        public System.Xml.XmlElement qosInfo
        {
            get
            {
                return _xml;
            }
            set
            {
                _xml = value;
            }
        }

        public string selectedOffer
        {
            get
            {
                return _selOff;
            }
            set
            {
                _selOff = value;
            }
        }

        public WSQoSSoapHeader()
        {
            _selOff = "none\uFFFD";
            _xml = null;
        }

        public WSQoSSoapHeader(System.Xml.XmlElement qosinfo)
        {
            _selOff = "none\uFFFD";
            _xml = null;
            _xml = qosinfo;
        }

    } // class WSQoSSoapHeader

}

