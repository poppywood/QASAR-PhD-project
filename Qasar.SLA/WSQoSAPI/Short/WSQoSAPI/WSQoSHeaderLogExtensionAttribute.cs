using System;
using System.Web.Services.Protocols;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Method)]
    public class WSQoSHeaderLogExtensionAttribute : SoapExtensionAttribute
    {

        private int priority;
        private string serviceName;

        public string ServiceName
        {
            get
            {
                return serviceName;
            }
            set
            {
                serviceName = value;
            }
        }

        public override Type ExtensionType
        {
            get
            {
                return typeof(WSQoSHeaderLogExtension);
            }
        }

        public override int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }

        public WSQoSHeaderLogExtensionAttribute()
        {
            serviceName = "\uFFFD";
        }

    } // class WSQoSHeaderLogExtensionAttribute

}

