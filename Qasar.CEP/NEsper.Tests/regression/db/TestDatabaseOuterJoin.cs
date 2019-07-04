// ---------------------------------------------------------------------------------- /
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
// ---------------------------------------------------------------------------------- /

using System;

using net.esper.client;
using net.esper.client.time;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.eql;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.db
{
	[TestFixture]
	public class TestDatabaseOuterJoin
	{
	    private readonly static String ALL_FIELDS = "mybigint, myint, myvarchar, mychar, mybool, mynumeric, mydecimal, mydouble, myreal";

	    private EPServiceProvider epService;
	    private SupportUpdateListener listener;

	    [SetUp]
	    public void SetUp()
	    {
	        ConfigurationDBRef configDB = new ConfigurationDBRef();
            configDB.SetDatabaseProviderConnection(SupportDatabaseService.DB1_SETTINGS);
            configDB.ParameterStyle = SupportDatabaseService.DB1_PARAMETER_STYLE;
            configDB.ParameterPrefix = SupportDatabaseService.DB1_PARAMETER_PREFIX;
	        configDB.ConnectionLifecycle = ConnectionLifecycleEnum.RETAIN;

            Configuration configuration = SupportConfigFactory.Configuration; 
	        configuration.AddDatabaseReference("MyDB", configDB);

	        epService = EPServiceProviderManager.GetProvider("TestDatabaseJoinRetained", configuration);
	        epService.Initialize();
	        epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	    }

	    [Test]
	    public void testOuterJoinLeftS0()
	    {
	        String stmtText = "select s0.intPrimitive as MyInt, " + TestDatabaseOuterJoin.ALL_FIELDS + " from " +
	                typeof(SupportBean).FullName + " as s0 left outer join " +
	                " sql:MyDB ['select " + TestDatabaseOuterJoin.ALL_FIELDS + " from mytesttable where ${s0.intPrimitive} = mytesttable.mybigint'] as s1 on intPrimitive = mybigint";
	        TryOuterJoinResult(stmtText);
	    }

	    [Test]
	    public void testOuterJoinRightS1()
	    {
	        String stmtText = "select s0.intPrimitive as MyInt, " + TestDatabaseOuterJoin.ALL_FIELDS + " from " +
	                " sql:MyDB ['select " + TestDatabaseOuterJoin.ALL_FIELDS + " from mytesttable where ${s0.intPrimitive} = mytesttable.mybigint'] as s1 right outer join " +
	                typeof(SupportBean).FullName + " as s0 on intPrimitive = mybigint";
	        TryOuterJoinResult(stmtText);
	    }

	    [Test]
	    public void testOuterJoinFullS0()
	    {
	        String stmtText = "select s0.intPrimitive as MyInt, " + TestDatabaseOuterJoin.ALL_FIELDS + " from " +
	                " sql:MyDB ['select " + TestDatabaseOuterJoin.ALL_FIELDS + " from mytesttable where ${s0.intPrimitive} = mytesttable.mybigint'] as s1 full outer join " +
	                typeof(SupportBean).FullName + " as s0 on intPrimitive = mybigint";
	        TryOuterJoinResult(stmtText);
	    }

	    [Test]
	    public void testOuterJoinFullS1()
	    {
	        String stmtText = "select s0.intPrimitive as MyInt, " + TestDatabaseOuterJoin.ALL_FIELDS + " from " +
	                typeof(SupportBean).FullName + " as s0 full outer join " +
	                " sql:MyDB ['select " + TestDatabaseOuterJoin.ALL_FIELDS + " from mytesttable where ${s0.intPrimitive} = mytesttable.mybigint'] as s1 on intPrimitive = mybigint";
	        TryOuterJoinResult(stmtText);
	    }

	    [Test]
	    public void testOuterJoinRightS0()
	    {
	        String stmtText = "select s0.intPrimitive as MyInt, " + TestDatabaseOuterJoin.ALL_FIELDS + " from " +
	                typeof(SupportBean).FullName + " as s0 right outer join " +
	                " sql:MyDB ['select " + TestDatabaseOuterJoin.ALL_FIELDS + " from mytesttable where ${s0.intPrimitive} = mytesttable.mybigint'] as s1 on intPrimitive = mybigint";
	        TryOuterJoinNoResult(stmtText);
	    }

	    [Test]
	    public void testOuterJoinLeftS1()
	    {
	        String stmtText = "select s0.intPrimitive as MyInt, " + TestDatabaseOuterJoin.ALL_FIELDS + " from " +
	                " sql:MyDB ['select " + TestDatabaseOuterJoin.ALL_FIELDS + " from mytesttable where ${s0.intPrimitive} = mytesttable.mybigint'] as s1 left outer join " +
	                typeof(SupportBean).FullName + " as s0 on intPrimitive = mybigint";
	        TryOuterJoinNoResult(stmtText);
	    }

	    [Test]
	    public void testOuterJoinOnFilter()
	    {
	        String stmtText = "select s0.intPrimitive as MyInt, " + TestDatabaseOuterJoin.ALL_FIELDS + " from " +
	                " sql:MyDB ['select " + TestDatabaseOuterJoin.ALL_FIELDS + " from mytesttable where ${s0.intPrimitive} = mytesttable.mybigint'] as s1 right outer join " +
	                typeof(SupportBean).FullName + " as s0 on string = myvarchar";

	        EPStatement statement = epService.EPAdministrator.CreateEQL(stmtText);
	        listener = new SupportUpdateListener();
	        statement.AddListener(listener);

	        SendEvent(1, "xxx");
	        Assert.IsFalse(listener.IsInvoked);

	        SendEvent(2, "B");
	        EventBean received = listener.AssertOneGetNewAndReset();
	        Assert.AreEqual(2, received["MyInt"]);
	        AssertReceived(received, 2l, 20, "B", "Y", false, 100.0m, 200.0m, 2.2d, 2.3d);
	    }

	    public void TryOuterJoinNoResult(String statementText)
	    {
	        EPStatement statement = epService.EPAdministrator.CreateEQL(statementText);
	        listener = new SupportUpdateListener();
	        statement.AddListener(listener);

	        SendEvent(2);
	        EventBean received = listener.AssertOneGetNewAndReset();
	        Assert.AreEqual(2, received["MyInt"]);
	        AssertReceived(received, 2l, 20, "B", "Y", false, 100.0m, 200.0m, 2.2d, 2.3d);

	        SendEvent(11);
	        Assert.IsFalse(listener.IsInvoked);
	    }

	    public void TryOuterJoinResult(String statementText)
	    {
	        EPStatement statement = epService.EPAdministrator.CreateEQL(statementText);
	        listener = new SupportUpdateListener();
	        statement.AddListener(listener);

	        SendEvent(1);
	        EventBean received = listener.AssertOneGetNewAndReset();
	        Assert.AreEqual(1, received["MyInt"]);
	        AssertReceived(received, 1l, 10, "A", "Z", true, 5000.0m, 100.0m, 1.2d, 1.3d);

	        SendEvent(11);
	        received = listener.AssertOneGetNewAndReset();
	        Assert.AreEqual(11, received["MyInt"]);
	        AssertReceived(received, null, null, null, null, null, null, null, null, null);
	    }

	    private void AssertReceived(EventBean _event, long? mybigint, int? myint, String myvarchar, String mychar, bool? mybool, decimal? mynumeric, decimal? mydecimal, double? mydouble, double? myreal)
	    {
	        Assert.AreEqual(mybigint, _event["mybigint"]);
	        Assert.AreEqual(myint, _event["myint"]);
	        Assert.AreEqual(myvarchar, _event["myvarchar"]);
	        Assert.AreEqual(mychar, _event["mychar"]);
	        Assert.AreEqual(mybool, _event["mybool"]);
	        Assert.AreEqual(mynumeric, _event["mynumeric"]);
	        Assert.AreEqual(mydecimal, _event["mydecimal"]);
	        Assert.AreEqual(mydouble, _event["mydouble"]);
	        Object r = _event["myreal"];
	        Assert.AreEqual(myreal, _event["myreal"]);
	    }

	    private void SendEvent(int intPrimitive)
	    {
	        SupportBean bean = new SupportBean();
	        bean.SetIntPrimitive(intPrimitive);
	        epService.EPRuntime.SendEvent(bean);
	    }

	    private void SendEvent(int intPrimitive, String _string)
	    {
	        SupportBean bean = new SupportBean();
	        bean.SetIntPrimitive(intPrimitive);
	        bean.SetString(_string);
	        epService.EPRuntime.SendEvent(bean);
	    }
	}
} // End of namespace
