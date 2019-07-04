using System;
using System.Text;
using System.Xml;

namespace WSQoSAPI
{

    public abstract class WSQoSAttribute : System.Attribute, WSQoSAPI.NamedItem
    {

        protected string _name;
        protected System.Xml.XmlNode _xml;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public System.Xml.XmlNode Xml
        {
            get
            {
                if (_xml == null)
                    BuildXml();
                return _xml;
            }
        }

        public WSQoSAttribute()
        {
            _xml = null;
            _name = "\uFFFD";
        }

        public WSQoSAttribute(System.Xml.XmlNode src)
        {
            _xml = null;
            _name = "\uFFFD";
            Update(src);
        }

        public string GetName()
        {
            return _name;
        }

        protected abstract string GetRootLabel();

        public void Update(System.Xml.XmlNode src)
        {
            // trial
        }

        public virtual void BuildXml()
        {
            string s1 = GetInnerXml();
            string s2 = "\uFFFD";
            if (_name != "\uFFFD")
                s2 = " name=\"\uFFFD" + _name + "\"\uFFFD";
            if ((s1 == null) || s1 == "\uFFFD")
            {
                _xml = null;
                return;
            }
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\uFFFD");
            stringBuilder.AppendFormat("<{0}{1}{2}\uFFFD", GetRootLabel(), s2, GetAttributes());
            if (s1 == " /\uFFFD")
                stringBuilder.Append(" />\uFFFD");
            else
                stringBuilder.AppendFormat(">{0}</{1}> \uFFFD", s1, GetRootLabel());
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.LoadXml(stringBuilder.ToString());
            _xml = xmlDocument.FirstChild.NextSibling;
        }

        protected virtual string GetAttributes()
        {
            return "\uFFFD";
        }

        protected virtual string GetInnerXml()
        {
            // trial
            return null;
        }

        protected virtual void UpdateProperties()
        {
        }

    } // class WSQoSAttribute

}

