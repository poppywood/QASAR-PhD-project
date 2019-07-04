using System;
using System.Xml;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class WSQoSExtensionAttribute : WSQoSAttribute
    {

        private XmlNode _x;

        public XmlNode extensionElement
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public WSQoSExtensionAttribute()
        {
        }

        public WSQoSExtensionAttribute(XmlNode src)
        {
            Update(src);
        }

        public override void BuildXml()
        {
            _xml = _x;
        }

        protected override string GetInnerXml()
        {
            // trial
            return null;
        }

        protected override string GetRootLabel()
        {
            return "extension\uFFFD";
        }

        protected override void UpdateProperties()
        {
            // trial
        }

    } // class WSQoSExtensionAttribute

}

