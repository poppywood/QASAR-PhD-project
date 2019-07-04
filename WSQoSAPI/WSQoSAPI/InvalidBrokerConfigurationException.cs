using System;

namespace WSQoSAPI
{

    public class InvalidBrokerConfigurationException : System.Exception
    {

        private string _m;

        public override string Message
        {
            get
            {
                return _m;
            }
        }

        public InvalidBrokerConfigurationException()
        {
            _m = "The configuration of the offer broker is invalid. \uFFFD";
        }

        public InvalidBrokerConfigurationException(string s)
        {
            _m = "The configuration of the offer broker is invalid. \uFFFD";
            _m += s;
        }

    } // class InvalidBrokerConfigurationException

}

