///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using net.esper.client.time;
using net.esper.events;
using net.esper.support.util;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.example.transaction
{
    [TestFixture]
	public class TestFindMissingEventStmt : TestStmtBase
	{
	    private const int TIME_WINDOW_SIZE_MSEC = 1800 * 1000;

	    private SupportUpdateListener listener;

	    [SetUp]
	    public override void SetUp()
	    {
	        base.SetUp();

	        listener = new SupportUpdateListener();
	        FindMissingEventStmt stmt = new FindMissingEventStmt(epService.EPAdministrator);
	        stmt.AddListener(listener);

	        // Use external clocking for the test
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	    }

	    [Test]
	    public void TestFlow()
	    {
	        TxnEventA[] a = new TxnEventA[20];
	        TxnEventB[] b = new TxnEventB[20];
	        TxnEventC[] c = new TxnEventC[20];


	        int seconds = 1 * 60;   // after 1 minutes
	        SendEvent(new CurrentTimeEvent(seconds * 1000));
	        SendEvent(a[0]=new TxnEventA("id0", seconds * 1000, "c1"));
	        SendEvent(b[0]=new TxnEventB("id0", seconds * 1000));
	        SendEvent(c[0]=new TxnEventC("id0", seconds * 1000, "s1"));

	        seconds = 2 * 60;
	        SendEvent(new CurrentTimeEvent(seconds * 1000));
	        SendEvent(a[1]=new TxnEventA("id1", seconds * 1000, "c1"));

	        seconds = 3 * 60;
	        SendEvent(new CurrentTimeEvent(seconds * 1000));
	        SendEvent(b[2]=new TxnEventB("id2", seconds * 1000));

	        seconds = 4 * 60;
	        SendEvent(new CurrentTimeEvent(seconds * 1000));
	        SendEvent(b[3]=new TxnEventB("id3", seconds * 1000));
	        SendEvent(c[3]=new TxnEventC("id3", seconds * 1000, "s1"));

	        seconds = 5 * 60;
	        SendEvent(new CurrentTimeEvent(seconds * 1000));
	        SendEvent(a[4]=new TxnEventA("id4", seconds * 1000, "c1"));
	        SendEvent(c[4]=new TxnEventC("id4", seconds * 1000, "s1"));

	        seconds = 6 * 60;
	        SendEvent(new CurrentTimeEvent(seconds * 1000));
	        SendEvent(a[5]=new TxnEventA("id5", seconds * 1000, "c1"));
	        SendEvent(b[5]=new TxnEventB("id5", seconds * 1000));

	        listener.Reset();

	        SendEvent(new CurrentTimeEvent(TIME_WINDOW_SIZE_MSEC + 1 * 60 * 1000)); // Expire "id0" from window
	        Assert.IsFalse(listener.IsInvoked);

	        SendEvent(new CurrentTimeEvent(TIME_WINDOW_SIZE_MSEC + 2 * 60 * 1000)); // Expire "id1" from window
	        AssertReceivedEvent(a[1], null);

	        SendEvent(new CurrentTimeEvent(TIME_WINDOW_SIZE_MSEC + 3 * 60 * 1000));
	        AssertReceivedEvent(null, b[2]);

	        SendEvent(new CurrentTimeEvent(TIME_WINDOW_SIZE_MSEC + 4 * 60 * 1000));
	        Assert.IsFalse(listener.IsInvoked);

	        SendEvent(new CurrentTimeEvent(TIME_WINDOW_SIZE_MSEC + 5 * 60 * 1000));
	        Assert.IsFalse(listener.IsInvoked);

	        SendEvent(new CurrentTimeEvent(TIME_WINDOW_SIZE_MSEC + 6 * 60 * 1000));
	        AssertReceivedTwoEvents(a[5], b[5]);
	    }

	    private void AssertReceivedEvent(TxnEventA expectedA, TxnEventB expectedB)
	    {
	        Assert.AreEqual(1, listener.OldDataList.Count);
	        Assert.AreEqual(1, listener.LastOldData.Length);
	        EventBean combinedEvent = listener.LastOldData[0];
	        Compare(combinedEvent, expectedA, expectedB);
	        listener.Reset();
	    }

	    private void AssertReceivedTwoEvents(TxnEventA expectedA, TxnEventB expectedB)
	    {
	        Assert.AreEqual(1, listener.OldDataList.Count);
	        Assert.AreEqual(2, listener.LastOldData.Length);

	        // The order is not guaranteed
	        if (listener.LastOldData[0].Get("A") == expectedA)
	        {
	            Compare(listener.LastOldData[0], expectedA, null);
	            Compare(listener.LastOldData[1], null, expectedB);
	        }
	        else
	        {
	            Compare(listener.LastOldData[1], expectedA, null);
	            Compare(listener.LastOldData[0], null, expectedB);
	        }

	        listener.Reset();
	    }

	    private static void Compare(EventBean combinedEvent, TxnEventA expectedA, TxnEventB expectedB)
	    {
	        Assert.AreSame(expectedA, combinedEvent.Get("A"));
	        Assert.AreSame(expectedB, combinedEvent.Get("B"));
	        Assert.IsNull(combinedEvent.Get("C"));
	    }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
