using System;
using System.Collections.Generic;

using net.esper.client;

namespace net.esper.support.emit
{
    public class SupportEmittedListener : EmittedListener
    {
        readonly IList<Object> emittedObjects = new List<Object>();

        public virtual void Emitted(Object _object)
        {
            emittedObjects.Add(_object);
        }

        public IList<Object> getEmittedObjects()
        {
            return emittedObjects;
        }
    }
}
