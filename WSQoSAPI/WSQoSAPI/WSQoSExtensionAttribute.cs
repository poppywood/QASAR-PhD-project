using System;
using System.Xml;

namespace WSQoSAPI
{

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Module | System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Enum | System.AttributeTargets.Constructor | System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field | System.AttributeTargets.Event | System.AttributeTargets.Interface | System.AttributeTargets.Parameter | System.AttributeTargets.Delegate | System.AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class WSQoSExtensionAttribute : WSQoSAPI.WSQoSAttribute
    {

        private System.Xml.XmlNode _x;

        public System.Xml.XmlNode extensionElement
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

        public WSQoSExtensionAttribute(System.Xml.XmlNode src)
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

