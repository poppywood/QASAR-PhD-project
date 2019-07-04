using System;

namespace WSQoSAPI
{

    public class NoTrustedProvidersException : System.Exception
    {

        private string _m;

        public override string Message
        {
            get
            {
                return _m;
            }
        }

        public NoTrustedProvidersException()
        {
            _m = "No entry for trusted providers could be found. \uFFFD";
        }

        public NoTrustedProvidersException(string s)
        {
            _m = "No entry for trusted providers could be found. \uFFFD";
            _m += s;
        }

    } // class NoTrustedProvidersException

}

