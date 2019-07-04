using System;
using System.Xml;

namespace WSQoSAPI
{

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Module | System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Enum | System.AttributeTargets.Constructor | System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field | System.AttributeTargets.Event | System.AttributeTargets.Interface | System.AttributeTargets.Parameter | System.AttributeTargets.Delegate | System.AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class SecAndTASupportAttribute : WSQoSAPI.ProtocolSupportAttribute
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

        public SecAndTASupportAttribute(System.Xml.XmlNode src)
        {
            Update(src);
        }

    } // class SecAndTASupportAttribute

}

