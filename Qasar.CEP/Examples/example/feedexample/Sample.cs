///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using NUnit.Framework;

using net.esper.client;

namespace net.esper.example.feedexample
{
	[TestFixture]
	public class Sample
	{
        [Test]
	    public void TestIt()
	    {
	        String stmtText =
	                "insert into ThroughputPerFeed " +
	                " select feed, count(*) as cnt " +
	                "from " + typeof(FeedEvent).FullName + ".win:time_batch(1 sec) " +
	                "group by feed";

            Configuration configuration = new Configuration();
            configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;

            EPServiceProviderManager.PurgeDefaultProvider();
	        EPServiceProvider engine = EPServiceProviderManager.GetDefaultProvider(configuration);
	        EPStatement stmt = engine.EPAdministrator.CreateEQL(stmtText);

	        UpdateListener listener = new MyListener();
	        stmt.AddListener(listener);

	        /*
	        while(true)
	        {
	            FeedEvent event;
	            event = new FeedEvent(FeedEnum.FEED_A, "IBM", 70);
	            engine.GetEPRuntime().SendEvent(event);
	            event = new FeedEvent(FeedEnum.FEED_B, "IBM", 70);
	            engine.GetEPRuntime().SendEvent(event);
	        }
	        */
	    }
	}

} // End of namespace
