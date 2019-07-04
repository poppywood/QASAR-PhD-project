///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.compat;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.client
{
    [TestFixture]
	public class TestStatementAwareListener
	{
	    private EPServiceProvider epService;
	    private SupportStmtAwareUpdateListener listener;

        [SetUp]
	    public void SetUp()
	    {
	        Configuration configuration = SupportConfigFactory.Configuration;
	        configuration.AddEventTypeAlias("Bean", typeof(SupportBean).FullName);
	        epService = EPServiceProviderManager.GetDefaultProvider(configuration);
	        epService.Initialize();

	        listener = new SupportStmtAwareUpdateListener();
	    }

        [Test]
	    public void testStmtAware()
	    {
	        String stmtText = "select * from Bean";
	        EPStatement statement = epService.EPAdministrator.CreateEQL(stmtText);
	        statement.AddListener(listener);

	        epService.EPRuntime.SendEvent(new SupportBean());
	        Assert.IsTrue(listener.IsInvoked);
	        Assert.AreEqual(1, listener.StatementList.Count);
	        Assert.AreEqual(statement, listener.StatementList[0]);
	        Assert.AreEqual(1, listener.SvcProviderList.Count);
	        Assert.AreEqual(epService, listener.SvcProviderList[0]);
	    }

        [Test]
	    public void testInvalid()
	    {
	        String stmtText = "select * from Bean";
	        EPStatement statement = epService.EPAdministrator.CreateEQL(stmtText);
	        StatementAwareUpdateListener listener = null;
	        try
	        {
	            statement.AddListener(listener);
	            Assert.Fail();
	        }
	        catch (ArgumentException ex)
	        {
	            // expected
	        }
	    }

        [Test]
	    public void testBothListeners()
	    {
	        String stmtText = "select * from Bean";
	        EPStatement statement = epService.EPAdministrator.CreateEQL(stmtText);

	        SupportStmtAwareUpdateListener[] awareListeners = new SupportStmtAwareUpdateListener[3];
	        SupportUpdateListener[] updateListeners = new SupportUpdateListener[awareListeners.Length];
	        for (int i = 0; i < awareListeners.Length; i++)
	        {
	            awareListeners[i] = new SupportStmtAwareUpdateListener();
	            statement.AddListener(awareListeners[i]);
	            updateListeners[i] = new SupportUpdateListener();
	            statement.AddListener(updateListeners[i]);
	        }

	        Object _event = new SupportBean();
	        epService.EPRuntime.SendEvent(_event);
	        for (int i = 0; i < awareListeners.Length; i++)
	        {
	            Assert.AreSame(_event, updateListeners[i].AssertOneGetNewAndReset().Underlying);
	            Assert.AreSame(_event, awareListeners[i].AssertOneGetNewAndReset().Underlying);
	        }

	        statement.RemoveListener(awareListeners[1]);
	        _event = new SupportBean();
	        epService.EPRuntime.SendEvent(_event);
	        for (int i = 0; i < awareListeners.Length; i++)
	        {
	            if(i == 1)
	            {
	                Assert.AreSame(_event, updateListeners[i].AssertOneGetNewAndReset().Underlying);
	                Assert.IsFalse(awareListeners[i].IsInvoked);
	            }
	            else
	            {
	                Assert.AreSame(_event, updateListeners[i].AssertOneGetNewAndReset().Underlying);
	                Assert.AreSame(_event, awareListeners[i].AssertOneGetNewAndReset().Underlying);
	            }
	        }

	        statement.RemoveListener(updateListeners[1]);
	        _event = new SupportBean();
	        epService.EPRuntime.SendEvent(_event);
	        for (int i = 0; i < awareListeners.Length; i++)
	        {
	            if(i == 1)
	            {
	                Assert.IsFalse(updateListeners[i].IsInvoked);
	                Assert.IsFalse(awareListeners[i].IsInvoked);
	            }
	            else
	            {
	                Assert.AreSame(_event, updateListeners[i].AssertOneGetNewAndReset().Underlying);
	                Assert.AreSame(_event, awareListeners[i].AssertOneGetNewAndReset().Underlying);
	            }
	        }

	        statement.AddListener(updateListeners[1]);
	        statement.AddListener(awareListeners[1]);
	        _event = new SupportBean();
	        epService.EPRuntime.SendEvent(_event);
	        for (int i = 0; i < awareListeners.Length; i++)
	        {
	            Assert.AreSame(_event, updateListeners[i].AssertOneGetNewAndReset().Underlying);
	            Assert.AreSame(_event, awareListeners[i].AssertOneGetNewAndReset().Underlying);
	        }

	        statement.RemoveAllListeners();
	        for (int i = 0; i < awareListeners.Length; i++)
	        {
	            Assert.IsFalse(updateListeners[i].IsInvoked);
	            Assert.IsFalse(awareListeners[i].IsInvoked);
	        }
	    }

        [Test]
	    public void testUseOnMultipleStmts()
	    {
	        EPStatement statementOne = epService.EPAdministrator.CreateEQL("select * from Bean(string='A' or string='C')");
	        EPStatement statementTwo = epService.EPAdministrator.CreateEQL("select * from Bean(string='B' or string='C')");

	        SupportStmtAwareUpdateListener awareListener = new SupportStmtAwareUpdateListener();
	        statementOne.AddListener(awareListener);
	        statementTwo.AddListener(awareListener);

	        epService.EPRuntime.SendEvent(new SupportBean("B", 1));
	        Assert.AreEqual("B", awareListener.AssertOneGetNewAndReset().Get("string"));

	        epService.EPRuntime.SendEvent(new SupportBean("A", 1));
	        Assert.AreEqual("A", awareListener.AssertOneGetNewAndReset().Get("string"));

	        epService.EPRuntime.SendEvent(new SupportBean("C", 1));
	        Assert.AreEqual(2, awareListener.NewDataList.Count);
	        Assert.AreEqual("C", awareListener.NewDataList[0][0].Get("string"));
	        Assert.AreEqual("C", awareListener.NewDataList[1][0].Get("string"));
	        EPStatement[] stmts = CollectionHelper.ToArray(awareListener.StatementList);
	        ArrayAssertionUtil.AreEqualAnyOrder(stmts, new Object[] {statementOne, statementTwo});
	    }

        [Test]
	    public void testOrderOfInvocation()
	    {
	        String stmtText = "select * from Bean";
	        EPStatement statement = epService.EPAdministrator.CreateEQL(stmtText);

	        MyStmtAwareUpdateListener[] awareListeners = new MyStmtAwareUpdateListener[2];
	        MyUpdateListener[] updateListeners = new MyUpdateListener[awareListeners.Length];
	        List<Object> invoked = new List<Object>();
	        for (int i = 0; i < awareListeners.Length; i++)
	        {
	            awareListeners[i] = new MyStmtAwareUpdateListener(invoked);
	            updateListeners[i] = new MyUpdateListener(invoked);
	        }

	        statement.AddListener(awareListeners[0]);
	        statement.AddListener(updateListeners[1]);
	        statement.AddListener(updateListeners[0]);
	        statement.AddListener(awareListeners[1]);

	        epService.EPRuntime.SendEvent(new SupportBean());

	        Assert.AreEqual(updateListeners[1], invoked[0]);
	        Assert.AreEqual(updateListeners[0], invoked[1]);
	        Assert.AreEqual(awareListeners[0], invoked[2]);
	        Assert.AreEqual(awareListeners[1], invoked[3]);

	        do
	        {
	            IEnumerator<UpdateListener> itOne = statement.UpdateListeners.GetEnumerator();
	            Assert.IsTrue(itOne.MoveNext());
	        } while (false);

            do
            {
                IEnumerator<StatementAwareUpdateListener> itOne = statement.StatementAwareListeners.GetEnumerator();
                Assert.IsTrue(itOne.MoveNext());
            } while (false);

            do
            {
                IEnumerator<UpdateListener> itOne = statement.UpdateListeners.GetEnumerator();
                Assert.IsTrue(itOne.MoveNext());
                Assert.AreEqual(updateListeners[1], itOne.Current);
                Assert.IsTrue(itOne.MoveNext());
                Assert.AreEqual(updateListeners[0], itOne.Current);
                Assert.IsFalse(itOne.MoveNext());
            } while (false);

	        do
	        {
                IEnumerator<StatementAwareUpdateListener> itTwo = statement.StatementAwareListeners.GetEnumerator();
                Assert.IsTrue(itTwo.MoveNext());
                Assert.AreEqual(awareListeners[0], itTwo.Current);
                Assert.IsTrue(itTwo.MoveNext());
                Assert.AreEqual(awareListeners[1], itTwo.Current);
                Assert.IsFalse(itTwo.MoveNext());
	        } while (false);

	        statement.RemoveAllListeners();
	        Assert.IsFalse(statement.StatementAwareListeners.GetEnumerator().MoveNext());
	        Assert.IsFalse(statement.UpdateListeners.GetEnumerator().MoveNext());
	    }

	    public class MyUpdateListener : UpdateListener
	    {
	        private IList<Object> invoked;

	        public MyUpdateListener(IList<Object> invoked)
	        {
	            this.invoked = invoked;
	        }

	        public void Update(EventBean[] newEvents, EventBean[] oldEvents)
	        {
	            invoked.Add(this);
	        }
	    }

	    public class MyStmtAwareUpdateListener : StatementAwareUpdateListener
	    {
	        private IList<Object> invoked;

	        public MyStmtAwareUpdateListener(IList<Object> invoked)
	        {
	            this.invoked = invoked;
	        }

	        public void Update(EventBean[] newEvents, EventBean[] oldEvents, EPStatement statement, EPServiceProvider epServiceProvider)
	        {
	            invoked.Add(this);
	        }
	    }
	}
} // End of namespace
