using System;
using System.Xml;

namespace WSQoSAPI
{

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Module | System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Enum | System.AttributeTargets.Constructor | System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field | System.AttributeTargets.Event | System.AttributeTargets.Interface | System.AttributeTargets.Parameter | System.AttributeTargets.Delegate | System.AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class TrustedParty : WSQoSAPI.WSQoSAttribute
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

        public TrustedParty(System.Xml.XmlNode src)
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
            catch (System.NullReferenceException e)
            {
                //e.Message;
            }
        }

    } // class TrustedParty

}

