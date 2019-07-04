using System;

namespace WSQoSAPI
{

    public class NoUddiRegistryUrlException : Exception
    {

        private string _m;

        public override string Message
        {
            get
            {
                return _m;
            }
        }

        public NoUddiRegistryUrlException()
        {
            _m = "No URL for a UDDI registry service could be found in local broker config file. \uFFFD";
        }

        public NoUddiRegistryUrlException(string s)
        {
            _m = "No URL for a UDDI registry service could be found in local broker config file. \uFFFD";
            _m += s;
        }

    } // class NoUddiRegistryUrlException

}

