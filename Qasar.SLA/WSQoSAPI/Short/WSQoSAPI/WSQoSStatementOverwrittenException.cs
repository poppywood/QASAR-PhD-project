using System;

namespace WSQoSAPI
{

    public class WSQoSStatementOverwrittenException : Exception
    {

        private string _m;

        public override string Message
        {
            get
            {
                return _m;
            }
        }

        public WSQoSStatementOverwrittenException()
        {
            _m = "A WS-QoS statements has been overwritten due to multiple statement definition [server qos metrics, transport qos metrics and price statements must not be defined more than once for one service object].\uFFFD";
        }

    } // class WSQoSStatementOverwrittenException

}

