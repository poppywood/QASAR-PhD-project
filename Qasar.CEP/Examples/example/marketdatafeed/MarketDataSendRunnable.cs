///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using System.Threading;
using net.esper.client;
using net.esper.support.util;

using org.apache.commons.logging;

namespace net.esper.example.marketdatafeed
{
	public class MarketDataSendRunnable : Runnable
	{
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    private readonly EPServiceProvider engine;

	    private FeedEnum? rateDropOffFeed;

	    private volatile bool isShutdown;
	    private readonly Random random = new Random();

	    public MarketDataSendRunnable(EPServiceProvider engine)
	    {
	        this.engine = engine;
	    }

	    public void Run()
	    {
	        log.Info(".call Thread " + Thread.CurrentThread + " starting");

	        try
	        {
                Array enumValues = Enum.GetValues(typeof(FeedEnum));

                while (!isShutdown)
	            {

	                int nextFeed = Math.Abs(random.Next() % 2);
	                FeedEnum feed = (FeedEnum) enumValues.GetValue(nextFeed);
	                if (rateDropOffFeed != feed)
	                {
	                    engine.EPRuntime.SendEvent(new MarketDataEvent("SYM", feed));
	                }
	            }
	        }
	        catch (Exception ex)
	        {
	            log.Error("Error in send loop", ex);
	        }

	        log.Info(".call Thread " + Thread.CurrentThread + " done");
	    }

	    public void SetRateDropOffFeed(FeedEnum? feedToDrop)
	    {
	        rateDropOffFeed = feedToDrop;
	    }

	    public void SetShutdown()
	    {
	        isShutdown = true;
	    }
	}
} // End of namespace
