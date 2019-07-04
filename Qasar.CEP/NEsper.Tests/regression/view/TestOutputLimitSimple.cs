///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using net.esper.client;
using net.esper.client.time;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.view
{
    [TestFixture]
    public class TestOutputLimitSimple
    {
        private const String JOIN_KEY = "KEY";

        private EPServiceProvider epService;
        private long currentTime;

        [SetUp]
        public void SetUp()
        {
            epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
            epService.Initialize();
        }

        [Test]
        public void testLimitEventJoin()
        {
            String eventName1 = typeof(SupportBean).FullName;
            String eventName2 = typeof(SupportBean_A).FullName;
            String joinStatement =
                "select * from " +
                    eventName1 + ".win:length(5) as event1," +
                    eventName2 + ".win:length(5) as event2" +
                " where event1.string = event2.id";
            String outputStmt1 = joinStatement + " output every 1 events";
            String outputStmt3 = joinStatement + " output every 3 events";

            EPStatement fireEvery1 = epService.EPAdministrator.CreateEQL(outputStmt1);
            EPStatement fireEvery3 = epService.EPAdministrator.CreateEQL(outputStmt3);

            SupportUpdateListener updateListener1 = new SupportUpdateListener();
            fireEvery1.AddListener(updateListener1);
            SupportUpdateListener updateListener3 = new SupportUpdateListener();
            fireEvery3.AddListener(updateListener3);

            // send event 1
            SendJoinEvents("s1");

            Assert.IsTrue(updateListener1.GetAndClearIsInvoked());
            Assert.AreEqual(1, updateListener1.LastNewData.Length);
            Assert.IsNull(updateListener1.LastOldData);

            Assert.IsFalse(updateListener3.GetAndClearIsInvoked());
            Assert.IsNull(updateListener3.LastNewData);
            Assert.IsNull(updateListener3.LastOldData);

            // send event 2
            SendJoinEvents("s2");

            Assert.IsTrue(updateListener1.GetAndClearIsInvoked());
            Assert.AreEqual(1, updateListener1.LastNewData.Length);
            Assert.IsNull(updateListener1.LastOldData);

            Assert.IsFalse(updateListener3.GetAndClearIsInvoked());
            Assert.IsNull(updateListener3.LastNewData);
            Assert.IsNull(updateListener3.LastOldData);

            // send event 3
            SendJoinEvents("s3");

            Assert.IsTrue(updateListener1.GetAndClearIsInvoked());
            Assert.AreEqual(1, updateListener1.LastNewData.Length);
            Assert.IsNull(updateListener1.LastOldData);

            Assert.IsTrue(updateListener3.GetAndClearIsInvoked());
            Assert.AreEqual(3, updateListener3.LastNewData.Length);
            Assert.IsNull(updateListener3.LastOldData);
        }

        [Test]
        public void testLimitTime()
        {
            String eventName = typeof(SupportBean).FullName;
            String selectStatement = "select * from " + eventName + ".win:length(5)";

            // test integer seconds
            String statementString1 = selectStatement +
                " output every 3 seconds";
            TimeCallback(statementString1, 3000);

            // test fractional seconds
            String statementString2 = selectStatement +
            " output every 3.3 seconds";
            TimeCallback(statementString2, 3300);

            // test integer minutes
            String statementString3 = selectStatement +
            " output every 2 minutes";
            TimeCallback(statementString3, 120000);

            // test fractional minutes
            String statementString4 =
                "select * from " +
                    eventName + ".win:length(5)" +
                " output every .05 minutes";
            TimeCallback(statementString4, 3000);
        }

        [Test]
        public void testTimeBatchOutputEvents()
        {
            epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));

            String stmtText = "select * from " + typeof(SupportBean).FullName + ".win:time_batch(10 seconds) output every 10 seconds";
            EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
            SupportUpdateListener listener = new SupportUpdateListener();
            stmt.AddListener(listener);

            SendTimer(0);
            SendTimer(10000);
            Assert.IsFalse(listener.IsInvoked);
            SendTimer(20000);
            Assert.IsFalse(listener.IsInvoked);

            SendEvent("e1");
            SendTimer(30000);
            Assert.IsFalse(listener.IsInvoked);
            SendTimer(40000);
            EventBean[] newEvents = listener.GetAndResetLastNewData();
            Assert.AreEqual(1, newEvents.Length);
            Assert.AreEqual("e1", newEvents[0].Get("string"));
            listener.Reset();

            SendTimer(50000);
            Assert.IsTrue(listener.IsInvoked);
            listener.Reset();

            SendTimer(60000);
            Assert.IsTrue(listener.IsInvoked);
            listener.Reset();

            SendTimer(70000);
            Assert.IsTrue(listener.IsInvoked);
            listener.Reset();

            SendEvent("e2");
            SendEvent("e3");
            SendTimer(80000);
            newEvents = listener.GetAndResetLastNewData();
            Assert.AreEqual(2, newEvents.Length);
            Assert.AreEqual("e2", newEvents[0].Get("string"));
            Assert.AreEqual("e3", newEvents[1].Get("string"));

            SendTimer(90000);
            Assert.IsTrue(listener.IsInvoked);
            listener.Reset();
        }

        [Test]
        public void testSimpleNoJoinAll()
        {
            String viewExpr = "select longBoxed " +
            "from " + typeof(SupportBean).FullName + ".win:length(3) " +
            "output all every 2 events";

            RunAssertAll(CreateStmtAndListenerNoJoin(viewExpr));

            viewExpr = "select longBoxed " +
            "from " + typeof(SupportBean).FullName + ".win:length(3) " +
            "output every 2 events";

            RunAssertAll(CreateStmtAndListenerNoJoin(viewExpr));

            viewExpr = "select * " +
            "from " + typeof(SupportBean).FullName + ".win:length(3) " +
            "output every 2 events";

            RunAssertAll(CreateStmtAndListenerNoJoin(viewExpr));
        }

        [Test]
        public void testSimpleNoJoinLast()
        {
            String viewExpr = "select longBoxed " +
            "from " + typeof(SupportBean).FullName + ".win:length(3) " +
            "output last every 2 events";

            RunAssertLast(CreateStmtAndListenerNoJoin(viewExpr));

            viewExpr = "select * " +
            "from " + typeof(SupportBean).FullName + ".win:length(3) " +
            "output last every 2 events";

            RunAssertLast(CreateStmtAndListenerNoJoin(viewExpr));
        }

        [Test]
        public void testSimpleJoinAll()
        {
            String viewExpr = "select longBoxed  " +
            "from " + typeof(SupportBeanString).FullName + ".win:length(3) as one, " +
            typeof(SupportBean).FullName + ".win:length(3) as two " +
            "output all every 2 events";

            RunAssertAll(CreateStmtAndListenerJoin(viewExpr));
        }

        private SupportUpdateListener CreateStmtAndListenerNoJoin(String viewExpr)
        {
            epService.Initialize();
            SupportUpdateListener updateListener = new SupportUpdateListener();
            EPStatement view = epService.EPAdministrator.CreateEQL(viewExpr);
            view.AddListener(updateListener);

            return updateListener;
        }

        private void RunAssertAll(SupportUpdateListener updateListener)
        {
            // send an event
            SendEvent(1);

            // check no update
            Assert.IsFalse(updateListener.GetAndClearIsInvoked());

            // send another event
            SendEvent(2);

            // check update, all events present
            Assert.IsTrue(updateListener.GetAndClearIsInvoked());
            Assert.AreEqual(2, updateListener.LastNewData.Length);
            Assert.AreEqual(1L, updateListener.LastNewData[0].Get("longBoxed"));
            Assert.AreEqual(2L, updateListener.LastNewData[1].Get("longBoxed"));
            Assert.IsNull(updateListener.LastOldData);
        }

        private void SendEvent(long longBoxed, int intBoxed, short shortBoxed)
        {
            SupportBean bean = new SupportBean();
            bean.SetString(JOIN_KEY);
            bean.SetLongBoxed(longBoxed);
            bean.SetIntBoxed(intBoxed);
            bean.SetShortBoxed(shortBoxed);
            epService.EPRuntime.SendEvent(bean);
        }

        private void SendEvent(long longBoxed)
        {
            SendEvent(longBoxed, 0, (short)0);
        }

        [Test]
        public void testSimpleJoinLast()
        {
            String viewExpr = "select longBoxed " +
            "from " + typeof(SupportBeanString).FullName + ".win:length(3) as one, " +
            typeof(SupportBean).FullName + ".win:length(3) as two " +
            "output last every 2 events";

            RunAssertLast(CreateStmtAndListenerJoin(viewExpr));
        }

        [Test]
        public void testLimitEventSimple()
        {
            SupportUpdateListener updateListener1 = new SupportUpdateListener();
            SupportUpdateListener updateListener2 = new SupportUpdateListener();
            SupportUpdateListener updateListener3 = new SupportUpdateListener();

            String eventName = typeof(SupportBean).FullName;
            String selectStmt = "select * from " + eventName + ".win:length(5)";
            String statement1 = selectStmt +
                " output every 1 events";
            String statement2 = selectStmt +
                " output every 2 events";
            String statement3 = selectStmt +
                " output every 3 events";

            EPStatement rateLimitStmt1 = epService.EPAdministrator.CreateEQL(statement1);
            rateLimitStmt1.AddListener(updateListener1);
            EPStatement rateLimitStmt2 = epService.EPAdministrator.CreateEQL(statement2);
            rateLimitStmt2.AddListener(updateListener2);
            EPStatement rateLimitStmt3 = epService.EPAdministrator.CreateEQL(statement3);
            rateLimitStmt3.AddListener(updateListener3);

            // send event 1
            SendEvent("s1");

            Assert.IsTrue(updateListener1.GetAndClearIsInvoked());
            Assert.AreEqual(1, updateListener1.LastNewData.Length);
            Assert.IsNull(updateListener1.LastOldData);

            Assert.IsFalse(updateListener2.GetAndClearIsInvoked());
            Assert.IsNull(updateListener2.LastNewData);
            Assert.IsNull(updateListener2.LastOldData);

            Assert.IsFalse(updateListener3.GetAndClearIsInvoked());
            Assert.IsNull(updateListener3.LastNewData);
            Assert.IsNull(updateListener3.LastOldData);

            // send event 2
            SendEvent("s2");

            Assert.IsTrue(updateListener1.GetAndClearIsInvoked());
            Assert.AreEqual(1, updateListener1.LastNewData.Length);
            Assert.IsNull(updateListener1.LastOldData);

            Assert.IsTrue(updateListener2.GetAndClearIsInvoked());
            Assert.AreEqual(2, updateListener2.LastNewData.Length);
            Assert.IsNull(updateListener2.LastOldData);

            Assert.IsFalse(updateListener3.GetAndClearIsInvoked());

            // send event 3
            SendEvent("s3");

            Assert.IsTrue(updateListener1.GetAndClearIsInvoked());
            Assert.AreEqual(1, updateListener1.LastNewData.Length);
            Assert.IsNull(updateListener1.LastOldData);

            Assert.IsFalse(updateListener2.GetAndClearIsInvoked());

            Assert.IsTrue(updateListener3.GetAndClearIsInvoked());
            Assert.AreEqual(3, updateListener3.LastNewData.Length);
            Assert.IsNull(updateListener3.LastOldData);
        }

        private SupportUpdateListener CreateStmtAndListenerJoin(String viewExpr)
        {
            epService.Initialize();

            SupportUpdateListener updateListener = new SupportUpdateListener();
            EPStatement view = epService.EPAdministrator.CreateEQL(viewExpr);
            view.AddListener(updateListener);

            epService.EPRuntime.SendEvent(new SupportBeanString(JOIN_KEY));

            return updateListener;
        }

        private void RunAssertLast(SupportUpdateListener updateListener)
        {
            // send an event
            SendEvent(1);

            // check no update
            Assert.IsFalse(updateListener.GetAndClearIsInvoked());

            // send another event
            SendEvent(2);

            // check update, only the last event present
            Assert.IsTrue(updateListener.GetAndClearIsInvoked());
            Assert.AreEqual(1, updateListener.LastNewData.Length);
            Assert.AreEqual(2L, updateListener.LastNewData[0].Get("longBoxed"));
            Assert.IsNull(updateListener.LastOldData);
        }

        private void SendTimer(long time)
        {
            CurrentTimeEvent _event = new CurrentTimeEvent(time);
            EPRuntime runtime = epService.EPRuntime;
            runtime.SendEvent(_event);
        }

        private void SendEvent(String s)
        {
            SupportBean bean = new SupportBean();
            bean.SetString(s);
            bean.SetDoubleBoxed(0.0);
            bean.SetIntPrimitive(0);
            bean.SetIntBoxed(0);
            epService.EPRuntime.SendEvent(bean);
        }

        private void TimeCallback(String statementString, int timeToCallback)
        {
            // clear any old events
            epService.Initialize();

            // turn off external clocking
            epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));

            // set the clock to 0
            currentTime = 0;
            SendTimeEvent(0);

            // create the eql statement and add a listener
            EPStatement statement = epService.EPAdministrator.CreateEQL(statementString);
            SupportUpdateListener updateListener = new SupportUpdateListener();
            statement.AddListener(updateListener);
            updateListener.Reset();

            // send an event
            SendEvent("s1");

            // check that the listener hasn't been updated
            Assert.IsFalse(updateListener.GetAndClearIsInvoked());

            // update the clock
            SendTimeEvent(timeToCallback);

            // check that the listener has been updated
            Assert.IsTrue(updateListener.GetAndClearIsInvoked());
            Assert.AreEqual(1, updateListener.LastNewData.Length);
            Assert.IsNull(updateListener.LastOldData);

            // send another event
            SendEvent("s2");

            // check that the listener hasn't been updated
            Assert.IsFalse(updateListener.GetAndClearIsInvoked());

            // update the clock
            SendTimeEvent(timeToCallback);

            // check that the listener has been updated
            Assert.IsTrue(updateListener.GetAndClearIsInvoked());
            Assert.AreEqual(1, updateListener.LastNewData.Length);
            Assert.IsNull(updateListener.LastOldData);

            // don't send an event
            // check that the listener hasn't been updated
            Assert.IsFalse(updateListener.GetAndClearIsInvoked());

            // update the clock
            SendTimeEvent(timeToCallback);

            // check that the listener has been updated
            Assert.IsTrue(updateListener.GetAndClearIsInvoked());
            Assert.IsNull(updateListener.LastNewData);
            Assert.IsNull(updateListener.LastOldData);

            // don't send an event
            // check that the listener hasn't been updated
            Assert.IsFalse(updateListener.GetAndClearIsInvoked());

            // update the clock
            SendTimeEvent(timeToCallback);

            // check that the listener has been updated
            Assert.IsTrue(updateListener.GetAndClearIsInvoked());
            Assert.IsNull(updateListener.LastNewData);
            Assert.IsNull(updateListener.LastOldData);

            // send several events
            SendEvent("s3");
            SendEvent("s4");
            SendEvent("s5");

            // check that the listener hasn't been updated
            Assert.IsFalse(updateListener.GetAndClearIsInvoked());

            // update the clock
            SendTimeEvent(timeToCallback);

            // check that the listener has been updated
            Assert.IsTrue(updateListener.GetAndClearIsInvoked());
            Assert.AreEqual(3, updateListener.LastNewData.Length);
            Assert.IsNull(updateListener.LastOldData);
        }

        private void SendTimeEvent(int timeIncrement)
        {
            currentTime += timeIncrement;
            CurrentTimeEvent _event = new CurrentTimeEvent(currentTime);
            epService.EPRuntime.SendEvent(_event);
        }

        private void SendJoinEvents(String s)
        {
            SupportBean event1 = new SupportBean();
            event1.SetString(s);
            event1.SetDoubleBoxed(0.0);
            event1.SetIntPrimitive(0);
            event1.SetIntBoxed(0);


            SupportBean_A event2 = new SupportBean_A(s);

            epService.EPRuntime.SendEvent(event1);
            epService.EPRuntime.SendEvent(event2);
        }
    }
} // End of namespace