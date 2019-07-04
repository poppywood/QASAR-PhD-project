using System;

namespace WSQoSAPI
{

    public class NoSuchCurrencyCodeException : Exception
    {

        private string _m;

        public override string Message
        {
            get
            {
                return _m;
            }
        }

        public NoSuchCurrencyCodeException()
        {
            _m = "An invalid currency encoding has been detected. No currency could be found for code \uFFFD";
        }

        public NoSuchCurrencyCodeException(string s)
        {
            _m = "An invalid currency encoding has been detected. No currency could be found for code \uFFFD";
            _m = _m + "\"\uFFFD" + s + "\" \uFFFD";
        }

    } // class NoSuchCurrencyCodeException

}

