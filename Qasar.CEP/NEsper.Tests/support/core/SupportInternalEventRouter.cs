using System;
using System.Collections.Generic;

using net.esper.core;
using net.esper.events;

namespace net.esper.support.core
{
    public class SupportInternalEventRouter : InternalEventRouter
    {
        private readonly IList<EventBean> routed = new List<EventBean>();

        public void Route(EventBean[] events, EPStatementHandle epStatementHandle)
        {
            for (int i = 0; i < events.Length; i++)
            {
                routed.Add(events[i]);
            }
        }

        public IList<EventBean> Routed
        {
            get { return routed; }
        }

        public IList<EventBean> GetRouted()
        {
            return routed;
        }

        public virtual void Reset()
        {
            routed.Clear();
        }
    }
}
