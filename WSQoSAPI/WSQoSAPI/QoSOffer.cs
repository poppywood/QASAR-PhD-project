using System;
using System.Xml;

namespace WSQoSAPI
{

    public class QoSOffer : WSQoSAPI.QoSDefinition
    {

        private System.DateTime _expires;
        private WSQoSAPI.ServiceModel _service;

        public System.DateTime Expires
        {
            get
            {
                return _expires;
            }
        }

        public WSQoSAPI.ServiceModel Service
        {
            get
            {
                return _service;
            }
        }

        public QoSOffer()
        {
            _expires = System.DateTime.Now;
            _service = null;
        }

        public QoSOffer(System.Xml.XmlNode src)
        {
            _expires = System.DateTime.Now;
            _service = null;
            try
            {
                _expires = System.DateTime.Parse(src.Attributes.GetNamedItem("expires\uFFFD").Value);
            }
            catch (System.Exception e)
            {
                //e.Message;
            }
            Update(src);
        }

        public QoSOffer(System.Xml.XmlNode Src, System.DateTime Expires)
        {
            _expires = System.DateTime.Now;
            _service = null;
            _expires = Expires;
            Update(Src);
        }

        public QoSOffer(System.Xml.XmlNode Src, WSQoSAPI.ServiceModel Service)
        {
            _expires = System.DateTime.Now;
            _service = null;
            try
            {
                _expires = System.DateTime.Parse(Src.Attributes.GetNamedItem("expires\uFFFD").Value);
            }
            catch (System.Exception e)
            {
                //e.Message;
            }
            _service = Service;
            Update(Src);
        }

        protected override string GetAttributes()
        {
            return System.String.Concat(" expires=\"\uFFFD", _expires, "\"\uFFFD");
        }

        protected override string GetRootLabel()
        {
            // trial
            return null;
        }

        public override bool Includes(WSQoSAPI.QoSRequirements RequireDef)
        {
            if (Expires > System.DateTime.Now)
                return base.Includes(RequireDef);
            return false;
        }

    } // class QoSOffer

}

