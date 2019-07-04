using System;

using net.esper.client;
using net.esper.events;

using org.apache.commons.logging;

namespace net.esper.example.autoid
{
	public class RFIDTagsPerSensorListener : UpdateListener
	{
	    public void Update(EventBean[] newEvents, EventBean[] oldEvents)
	    {
            if (newEvents != null)
            {
                LogRate(newEvents[0]);
            }
	    }
	
	    private void LogRate(EventBean eventBean)
	    {
	    	string sensorId = (string) eventBean["sensorId"];
	    	double numTags = (double) eventBean["numTagsPerSensor"];
	
	        log.Info("Sensor " + sensorId + " totals " + numTags + " tags");
	    }
	
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
}
