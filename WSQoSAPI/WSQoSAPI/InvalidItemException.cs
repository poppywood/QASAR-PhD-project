using System;

namespace WSQoSAPI
{

    public class InvalidItemException : System.Exception
    {

        public override string Message
        {
            get
            {
                return "The attempt to add an object to a named item collection has failed, because the object is not of the type NamedItem.\uFFFD";
            }
        }

        public InvalidItemException()
        {
        }

    } // class InvalidItemException

}

