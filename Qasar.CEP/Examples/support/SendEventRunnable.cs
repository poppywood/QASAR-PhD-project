using System;

using net.esper.client;
using net.esper.support.util;

using org.apache.commons.logging;

namespace net.esper.support
{
	public class SendEventRunnable : Runnable
	{
	    private readonly Object eventToSend;
        private readonly EPServiceProvider epService;
	
	    public SendEventRunnable(EPServiceProvider epService, Object eventToSend)
	    {
	        this.epService = epService;
	        this.eventToSend = eventToSend;
	    }
	
	    public void Run()
	    {
	        try
	        {
	            epService.EPRuntime.SendEvent(eventToSend);
	        }
	        catch (Exception ex)
	        {
	            log.Fatal(ex);
	        }
	    }
	
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
}
