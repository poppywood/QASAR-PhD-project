using System;

namespace WSQoSAPI
{

    public class NoCurrencyConverterException : Exception
    {

        private string _m;

        public override string Message
        {
            get
            {
                return _m;
            }
        }

        public NoCurrencyConverterException()
        {
            _m = "Currency converter has not yet been instanciated. Cannot compare prices without unless the WSQoSAPI.WSQoSServiceBroker's property CurrencyConverter is set to an instance of the WSQoSAPI.CurrencyConverter.\uFFFD";
        }

    } // class NoCurrencyConverterException

}

