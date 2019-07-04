using System;
using System.Web.Services.Protocols;

namespace WSQoSAPI
{

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class WSQoSHeaderLogExtensionAttribute : System.Web.Services.Protocols.SoapExtensionAttribute
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

        public override System.Type ExtensionType
        {
            get
            {
                return typeof(WSQoSAPI.WSQoSHeaderLogExtension);
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

