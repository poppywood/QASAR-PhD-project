///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using NUnit.Framework;

using net.esper.compat;
using net.esper.support.bean;
using net.esper.support.events;

using org.apache.commons.logging;

namespace net.esper.events
{
	[TestFixture]
	public class TestMapEventBean
	{
	    private EDictionary<String, Type> testTypesMap;
        private DataDictionary testValuesMap;

	    private EventType eventType;
	    private MapEventBean _eventBean;

	    private SupportBeanComplexProps supportBean = SupportBeanComplexProps.MakeDefaultBean();

	    [SetUp]
	    public void SetUp()
	    {
	        testTypesMap = new HashDictionary<String, Type>();
	        testTypesMap.Put("aString", typeof(String));
	        testTypesMap.Put("anInt", typeof(int?));
	        testTypesMap.Put("myComplexBean", typeof(SupportBeanComplexProps));

	        testValuesMap = new DataDictionary();
	        testValuesMap.Put("aString", "test");
	        testValuesMap.Put("anInt", 10);
	        testValuesMap.Put("myComplexBean", supportBean);

	        eventType = new MapEventType("", testTypesMap, SupportEventAdapterService.GetService());
	        _eventBean = new MapEventBean(testValuesMap, eventType);
	    }

	    [Test]
	    public void testGet()
	    {
	        Assert.AreEqual(eventType, _eventBean.EventType);
	        Assert.AreEqual(testValuesMap, _eventBean.Underlying);

	        Assert.AreEqual("test", _eventBean["aString"]);
	        Assert.AreEqual(10, _eventBean["anInt"]);

	        Assert.AreEqual("nestedValue", _eventBean["myComplexBean.nested.nestedValue"]);

	        // test wrong property name
	        try
	        {
	            Object voidData = _eventBean["dummy"];
	            Assert.IsTrue(false);
	        }
	        catch (ArgumentException ex)
	        {
	            // Expected
	            log.Debug(".testGetter Expected exception, msg=" + ex.Message);
	        }
	    }

	    [Test]
	    public void testCreateUnderlying()
	    {
	        SupportBean beanOne = new SupportBean();
	        SupportBean_A beanTwo = new SupportBean_A("a");

	        EventBean eventOne = SupportEventBeanFactory.CreateObject(beanOne);
	        EventBean eventTwo = SupportEventBeanFactory.CreateObject(beanTwo);

	        // Set up event type
	        testTypesMap.Clear();
	        testTypesMap.Put("a", typeof(SupportBean));
	        testTypesMap.Put("b", typeof(SupportBean_A));
	        EventType eventType = SupportEventAdapterService.GetService().CreateAnonymousMapType(testTypesMap);

	        EDictionary<String, EventBean> events = new HashDictionary<String, EventBean>();
	        events.Put("a", eventOne);
	        events.Put("b", eventTwo);

	        MapEventBean _event = new MapEventBean(eventType, events);
	        Assert.IsTrue(_event["a"] == beanOne);
	        Assert.IsTrue(_event["b"] == beanTwo);
	    }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
