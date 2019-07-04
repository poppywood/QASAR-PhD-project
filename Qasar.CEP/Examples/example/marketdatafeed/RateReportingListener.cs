using System;

using net.esper.client;
using net.esper.events;

using org.apache.commons.logging;

namespace net.esper.example.marketdatafeed
{
    public class RateReportingListener : UpdateListener
    {
        public void Update(EventBean[] newEvents, EventBean[] oldEvents)
        {
            if (newEvents.Length > 0)
            {
                LogRate(newEvents[0]);
            }
            if (newEvents.Length > 1)
            {
                LogRate(newEvents[1]);
            }
        }

        private static void LogRate(EventBean eventBean)
        {
            log.Info("Current rate for feed " + eventBean["feed"].ToString() +
                      " is " + eventBean["cnt"]);
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}