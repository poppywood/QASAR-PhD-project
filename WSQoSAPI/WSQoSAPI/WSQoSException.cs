using System;

namespace WSQoSAPI
{

    public class WSQoSException : System.Exception
    {

        private string _m;

        public override string Message
        {
            get
            {
                return _m;
            }
        }

        public WSQoSException()
        {
            _m = "An unidentified problem has occured while processing WS-QoS attributes.\uFFFD";
        }

    } // class WSQoSException

}

