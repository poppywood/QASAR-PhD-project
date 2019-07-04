using System;
using System.Xml;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class SecAndTASupportAttribute : ProtocolSupportAttribute
    {

        public SecAndTASupportAttribute()
        {
        }

        public SecAndTASupportAttribute(string Name, string OntologyUrl)
        {
            _name = Name;
            _ontUrl = OntologyUrl;
            BuildXml();
        }

        public SecAndTASupportAttribute(XmlNode src)
        {
            Update(src);
        }

    } // class SecAndTASupportAttribute

}

