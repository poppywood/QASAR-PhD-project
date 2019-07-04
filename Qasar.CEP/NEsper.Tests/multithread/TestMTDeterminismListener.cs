///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using net.esper.client;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.util;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.multithread
{
/// <summary>
/// Test for multithread-safety and deterministic behavior when using insert-into.
/// </summary>
    [TestFixture]
    public class TestMTDeterminismListener
	{
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	    private EPServiceProvider engine;

        [TearDown]
	    public void TearDown()
	    {
	        engine.Initialize();
	    }

        [Test]
	    public void testOrderedDelivery()
	    {
	        engine = EPServiceProviderManager.GetDefaultProvider();
	        engine.Initialize();
	        TrySend(3, 1000);
	    }

	    public void ManualTestOrderedDeliveryFail()
	    {
	        /// <summary>Commented out as this is a manual test</summary>
	        Configuration config = new Configuration();
	        config.EngineDefaults.Threading.IsListenerDispatchPreserveOrder = false;
	        engine = EPServiceProviderManager.GetDefaultProvider(config);
	        engine.Initialize();
	        TrySend(3, 1000);
	    }

	    private void TrySend(int numThreads, int numEvents)
	    {
	        // setup statements
	        EPStatement stmtInsert =
	            engine.EPAdministrator.CreateEQL("select Count(*) as cnt from " + typeof (SupportBean).FullName);
	        SupportMTUpdateListener listener = new SupportMTUpdateListener();
	        stmtInsert.AddListener(listener);

	        // execute
	        ExecutorService threadPool = Executors.NewFixedThreadPool(numThreads);
	        Future[] future = new Future[numThreads];
	        for (int i = 0; i < numThreads; i++)
	        {
	            future[i] = threadPool.Submit(new SendEventCallable(i, engine, Generator.Create(numEvents)));
	        }

	        threadPool.Shutdown();
	        threadPool.AwaitTermination(new TimeSpan(0, 0, 0, 10));

	        for (int i = 0; i < numThreads; i++)
	        {
	            Assert.IsTrue((Boolean) future[i].Get());
	        }

	        EventBean[] events = listener.GetNewDataListFlattened();
	        long[] result = new long[events.Length];
	        for (int i = 0; i < events.Length; i++)
	        {
	            result[i] = (long) events[i].Get("cnt");
	        }
	        //log.Info(".trySend result=" + Arrays.ToString(result));

	        // assert result
	        Assert.AreEqual(numEvents * numThreads, events.Length);
	        for (int i = 0; i < numEvents * numThreads; i++)
	        {
	            Assert.AreEqual(result[i], (long) i + 1);
	        }
	    }
	}
} // End of namespace
