using System;
using System.Xml;

namespace WSQoSAPI
{

    public class QoSOffer : QoSDefinition
    {

        private DateTime _expires;
        private ServiceModel _service;

        public DateTime Expires
        {
            get
            {
                return _expires;
            }
        }

        public ServiceModel Service
        {
            get
            {
                return _service;
            }
        }

        public QoSOffer()
        {
            _expires = DateTime.Now;
            _service = null;
        }

        public QoSOffer(XmlNode src)
        {
            _expires = DateTime.Now;
            _service = null;
            try
            {
                _expires = DateTime.Parse(src.Attributes.GetNamedItem("expires\uFFFD").Value);
            }
            catch (Exception e)
            {
                e.Message;
            }
            Update(src);
        }

        public QoSOffer(XmlNode Src, DateTime Expires)
        {
            _expires = DateTime.Now;
            _service = null;
            _expires = Expires;
            Update(Src);
        }

        public QoSOffer(XmlNode Src, ServiceModel Service)
        {
            _expires = DateTime.Now;
            _service = null;
            try
            {
                _expires = DateTime.Parse(Src.Attributes.GetNamedItem("expires\uFFFD").Value);
            }
            catch (Exception e)
            {
                e.Message;
            }
            _service = Service;
            Update(Src);
        }

        protected override string GetAttributes()
        {
            return String.Concat(" expires=\"\uFFFD", _expires, "\"\uFFFD");
        }

        protected override string GetRootLabel()
        {
            // trial
            return null;
        }

        public override bool Includes(QoSRequirements RequireDef)
        {
            if (Expires > DateTime.Now)
                return base.Includes(RequireDef);
            return false;
        }

    } // class QoSOffer

}

