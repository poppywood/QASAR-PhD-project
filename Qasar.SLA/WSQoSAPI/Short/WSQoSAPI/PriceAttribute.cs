using System;
using System.Xml;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class PriceAttribute : WSQoSAttribute
    {

        private string _cur;
        private double _val;

        public string Currency
        {
            get
            {
                return _cur;
            }
            set
            {
                _cur = value;
                BuildXml();
            }
        }

        public double Value
        {
            get
            {
                return _val;
            }
            set
            {
                _val = value;
                BuildXml();
            }
        }

        public PriceAttribute()
        {
            _val = 0.0;
            _cur = "EUR\uFFFD";
        }

        public PriceAttribute(XmlNode src)
        {
            _val = 0.0;
            _cur = "EUR\uFFFD";
            Update(src);
        }

        public bool IsCheaperThan(PriceAttribute otherPrice)
        {
            // trial
            return false;
        }

        protected override string GetAttributes()
        {
            return " currency=\"\uFFFD" + _cur + "\"\uFFFD";
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
                _val = Double.Parse(_xml.InnerText);
            }
            catch (NullReferenceException e)
            {
                e.Message;
            }
            if (((XmlElement)_xml).HasAttribute("currency\uFFFD"))
            {
                _cur = _xml.Attributes.GetNamedItem("currency\uFFFD").Value;
                return;
            }
            _cur = "EUR\uFFFD";
        }

    } // class PriceAttribute

}

