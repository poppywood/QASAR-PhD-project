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
using net.esper.client.time;
using net.esper.compat;
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
	public class TestMTDeterminismInsertInto
	{
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	    private EPServiceProvider engine;

        [SetUp]
	    public void SetUp()
	    {
	        Configuration config = new Configuration();
	        // This should fail all test in this class
	        // config.GetEngineDefaults().GetThreading().SetInsertIntoDispatchPreserveOrder(false);

	        engine = EPServiceProviderManager.GetDefaultProvider(config);
	        engine.Initialize();
	        engine.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	    }

        [TearDown]
	    public void TearDown()
	    {
	        engine.Initialize();
	    }

        [Test]
	    public void testSceneOne()
	    {
	        TrySendCountFollowedBy(4, 10000);
	    }

        [Test]
        public void testSceneTwo()
	    {
	        TryChainedCountSum(3, 10000);
	    }

        [Test]
        public void testSceneThree()
	    {
	        TryMultiInsertGroup(3, 10, 1000);
	    }

	    private void TryMultiInsertGroup(int numThreads, int numStatements, int numEvents)
	    {
	        // setup statements
	        EPStatement[] insertIntoStmts = new EPStatement[numStatements];
	        for (int i = 0; i < numStatements; i++)
	        {
	            insertIntoStmts[i] = engine.EPAdministrator.CreateEQL("insert into MyStream select '" + i + "'" + " as ident,count(*) as cnt from " + typeof(SupportBean).FullName);
	        }
	        EPStatement stmtInsertTwo = engine.EPAdministrator.CreateEQL("select ident, Sum(cnt) as mysum from MyStream group by ident");
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmtInsertTwo.AddListener(listener);

	        // execute
	        ExecutorService threadPool = Executors.NewFixedThreadPool(numThreads);
	        Future[] future = new Future[numThreads];
	        FastReaderWriterLock sharedStartLock = new FastReaderWriterLock();
            using(new WriterLock(sharedStartLock))
            {
                for (int i = 0; i < numThreads; i++)
                {
                    future[i] =
                        threadPool.Submit(
                            new SendEventRWLockCallable(i, sharedStartLock, engine, Generator.Create(numEvents)));
                }
                Thread.Sleep(100);
            }

	        threadPool.Shutdown();
	        threadPool.AwaitTermination(new TimeSpan(0, 0, 0, 10));

	        for (int i = 0; i < numThreads; i++)
	        {
	            Assert.IsTrue((bool) future[i].Get());
	        }

	        // assert result
	        EventBean[] newEvents = listener.GetNewDataListFlattened();
	        int count = 0;
	        for (int i = 0; i < numEvents - 1; i++)
	        {
	            long expected = Total(i + 1);
	            for (int j = 0; j < numStatements; j++)
	            {
	                String ident = (String) newEvents[count].Get("ident");
	                long mysum = (long) newEvents[count].Get("mysum");
	                count++;

	                Assert.AreEqual(Convert.ToString(j), ident);
	                Assert.AreEqual(expected, mysum);
	            }
	        }

	        // destroy
	        for (int i = 0; i < numStatements; i++)
	        {
	            insertIntoStmts[i].Destroy();
	        }
	        stmtInsertTwo.Destroy();
	    }

	    private void TryChainedCountSum(int numThreads, int numEvents)
	    {
	        // setup statements
	        EPStatement stmtInsertOne =
	            engine.EPAdministrator.CreateEQL("insert into MyStreamOne select count(*) as cnt from " +
	                                                  typeof (SupportBean).FullName);
	        EPStatement stmtInsertTwo = engine.EPAdministrator.CreateEQL("insert into MyStreamTwo select sum(cnt) as mysum from MyStreamOne");
	        EPStatement stmtInsertThree = engine.EPAdministrator.CreateEQL("select * from MyStreamTwo");
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmtInsertThree.AddListener(listener);

	        // execute
	        ExecutorService threadPool = Executors.NewFixedThreadPool(numThreads);
	        Future[] future = new Future[numThreads];
	        FastReaderWriterLock sharedStartLock = new FastReaderWriterLock();
            using(new WriterLock(sharedStartLock))
            {
                for (int i = 0; i < numThreads; i++)
                {
                    future[i] = threadPool.Submit(
                            new SendEventRWLockCallable(i, sharedStartLock, engine, Generator.Create(numEvents)));
                }
                Thread.Sleep(100);
            }

	        threadPool.Shutdown();
	        threadPool.AwaitTermination(new TimeSpan(0, 0, 0, 10));

	        for (int i = 0; i < numThreads; i++)
	        {
	            Assert.IsTrue((bool) future[i].Get());
	        }

	        // assert result
	        EventBean[] newEvents = listener.GetNewDataListFlattened();
	        for (int i = 0; i < numEvents - 1; i++)
	        {
	            long expected = Total(i + 1);
	            Assert.AreEqual(expected, newEvents[i].Get("mysum"));
	        }

	        stmtInsertOne.Destroy();
	        stmtInsertTwo.Destroy();
	        stmtInsertThree.Destroy();
	    }

	    private long Total(int num)
	    {
	        long total = 0;
	        for (int i = 1; i < num + 1; i++)
	        {
	            total += i;
	        }
	        return total;
	    }

	    private void TrySendCountFollowedBy(int numThreads, int numEvents)
	    {
	        // setup statements
            EPStatement stmtInsert = engine.EPAdministrator.CreateEQL("insert into MyStream select Count(*) as cnt from " + typeof(SupportBean).FullName);
            stmtInsert.AddListener(
                new ProxyUpdateListener(
                    delegate(EventBean[] newEvents, EventBean[] oldEvents)
                        {
                            log.Debug(".update cnt=" + newEvents[0].Get("cnt"));
                        }));

            SupportUpdateListener[] listeners = new SupportUpdateListener[numEvents];
            for (int i = 0; i < numEvents; i++)
            {
                String text = "select * from pattern [MyStream(cnt=" + (i + 1) + ") -> MyStream(cnt=" + (i + 2) + ")]";
                EPStatement stmt = engine.EPAdministrator.CreateEQL(text);
                listeners[i] = new SupportUpdateListener(i);
                stmt.AddListener(listeners[i]);
            }

	        // execute
	        ExecutorService threadPool = Executors.NewFixedThreadPool(numThreads);
	        Future[] future = new Future[numThreads];
	        FastReaderWriterLock sharedStartLock = new FastReaderWriterLock();

            using (new WriterLock(sharedStartLock))
            {
                for (int i = 0; i < numThreads; i++)
                {
                    future[i] =
                        threadPool.Submit(
                            new SendEventRWLockCallable(i, sharedStartLock, engine, Generator.Create(numEvents)));
                }
                Thread.Sleep(100);
            }

	        threadPool.Shutdown();
	        threadPool.AwaitTermination(new TimeSpan(0, 0, 1000));

	        for (int i = 0; i < numThreads; i++)
	        {
	            Assert.IsTrue((bool) future[i].Get());
	        }

	        // assert result
            for (int i = 0; i < numEvents - 1; i++)
            {
                Assert.AreEqual(1, listeners[i].NewDataList.Count, "Listener not invoked: #" + i + "/" + numEvents);
            }
	    }
	}
} // End of namespace
