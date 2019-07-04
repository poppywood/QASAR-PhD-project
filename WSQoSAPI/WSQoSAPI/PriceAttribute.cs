using System;
using System.Xml;

namespace WSQoSAPI
{

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class PriceAttribute : WSQoSAPI.WSQoSAttribute
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

        public PriceAttribute(System.Xml.XmlNode src)
        {
            _val = 0.0;
            _cur = "EUR\uFFFD";
            Update(src);
        }

        public bool IsCheaperThan(WSQoSAPI.PriceAttribute otherPrice)
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
                _val = System.Double.Parse(_xml.InnerText);
            }
            catch (System.NullReferenceException e)
            {
                //e.Message;
            }
            if (((System.Xml.XmlElement)_xml).HasAttribute("currency\uFFFD"))
            {
                _cur = _xml.Attributes.GetNamedItem("currency\uFFFD").Value;
                return;
            }
            _cur = "EUR\uFFFD";
        }

    } // class PriceAttribute

}

