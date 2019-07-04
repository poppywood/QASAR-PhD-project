using System;

using net.esper.client;
using net.esper.events;

using org.apache.commons.logging;

namespace net.esper.example.qos_sla.monitor
{
    public class AverageLatencyListener : UpdateListener
    {
        private long alertThreshold;

        public AverageLatencyListener(long alertThreshold)
        {
            this.alertThreshold = alertThreshold;
        }

        public void Update(EventBean[] newEvents, EventBean[] oldEvents)
        {
            long count = (long)newEvents[0]["count"];
            double avg = (double)newEvents[0]["average"];

            if ((count < 100) || (avg < alertThreshold))
            {
                return;
            }

            String operation = (String)newEvents[0]["operationName"];
            String customer = (String)newEvents[0]["customerId"];

            log.Debug("Alert, for operation '" + operation +
                    "' and customer '" + customer + "'" +
                    " average latency was " + avg);
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}