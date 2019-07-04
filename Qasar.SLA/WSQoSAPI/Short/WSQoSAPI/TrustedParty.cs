using System;
using System.Xml;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class TrustedParty : WSQoSAttribute
    {

        private string _url;

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
                BuildXml();
            }
        }

        public TrustedParty()
        {
        }

        public TrustedParty(string Name, string Url)
        {
            _name = Name;
            _url = Url;
            BuildXml();
        }

        public TrustedParty(XmlNode src)
        {
            Update(src);
        }

        protected override string GetAttributes()
        {
            if (_url != "\uFFFD")
                return " url=\"\uFFFD" + _url + "\"\uFFFD";
            return "\uFFFD";
        }

        protected override string GetInnerXml()
        {
            // trial
            return null;
        }

        protected override string GetRootLabel()
        {
            // trial
            return null;
        }

        protected override void UpdateProperties()
        {
            try
            {
                _url = _xml.Attributes.GetNamedItem("url\uFFFD").Value;
            }
            catch (NullReferenceException e)
            {
                e.Message;
            }
        }

    } // class TrustedParty

}

