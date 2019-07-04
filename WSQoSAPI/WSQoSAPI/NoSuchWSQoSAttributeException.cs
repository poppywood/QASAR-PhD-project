using System;

namespace WSQoSAPI
{

    public class NoSuchWSQoSAttributeException : System.Exception
    {

        private string _m;

        public override string Message
        {
            get
            {
                return _m;
            }
        }

        public NoSuchWSQoSAttributeException()
        {
            _m = "A WS-QoS attribute has been attempted to be accessed from a WS-QoS attribute collection, which does not hold any WS-QoS attribute of the name \uFFFD";
        }

        public NoSuchWSQoSAttributeException(string s)
        {
            _m = "A WS-QoS attribute has been attempted to be accessed from a WS-QoS attribute collection, which does not hold any WS-QoS attribute of the name \uFFFD";
            _m = _m + "\"\uFFFD" + s + "\".\uFFFD";
        }

    } // class NoSuchWSQoSAttributeException

}

