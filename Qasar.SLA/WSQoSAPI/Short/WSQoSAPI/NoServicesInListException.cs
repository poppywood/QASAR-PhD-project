using System;

namespace WSQoSAPI
{

    public class NoServicesInListException : Exception
    {

        private string _m;

        public override string Message
        {
            get
            {
                return _m;
            }
        }

        public NoServicesInListException()
        {
            _m = "No service model for updating was found in service list. \uFFFD";
        }

        public NoServicesInListException(string s)
        {
            _m = "No service model for updating was found in service list. \uFFFD";
            _m += s;
        }

    } // class NoServicesInListException

}

