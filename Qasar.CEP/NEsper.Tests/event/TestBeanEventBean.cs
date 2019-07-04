///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using NUnit.Framework;

using net.esper.support.bean;
using net.esper.support.events;

using org.apache.commons.logging;

namespace net.esper.events
{
	[TestFixture]
	public class TestBeanEventBean
	{
	    SupportBean testEvent;

	    [SetUp]
	    public void SetUp()
	    {
	        testEvent = new SupportBean();
	        testEvent.SetIntPrimitive(10);
	    }

	    [Test]
	    public void testGet()
	    {
	        EventType eventType = SupportEventTypeFactory.CreateBeanType(typeof(SupportBean));
            BeanEventBean eventBean = new BeanEventBean(testEvent, eventType);

	        Assert.AreEqual(eventType, eventBean.EventType);
	        Assert.AreEqual(testEvent, eventBean.Underlying);

	        Assert.AreEqual(10, eventBean["intPrimitive"]);

	        // Test wrong property name
	        try
	        {
                Object theVoid = eventBean["dummy"];
	            Assert.IsTrue(false);
	        }
	        catch (PropertyAccessException ex)
	        {
	            // Expected
	            log.Debug(".testGetter Expected exception, msg=" + ex.Message);
	        }

	        // Test wrong event type - not possible to happen under normal use
	        try
	        {
	            eventType = SupportEventTypeFactory.CreateBeanType(typeof(SupportBeanSimple));
                eventBean = new BeanEventBean(testEvent, eventType);
                Object theVoid = eventBean["myString"];
	            Assert.IsTrue(false);
	        }
	        catch (PropertyAccessException ex)
	        {
	            // Expected
	            log.Debug(".testGetter Expected exception, msg=" + ex.Message);
	        }
	    }

	    [Test]
	    public void testGetComplexProperty()
	    {
	        SupportBeanCombinedProps _event = SupportBeanCombinedProps.MakeDefaultBean();
	        EventBean eventBean = SupportEventBeanFactory.CreateObject(_event);

	        Assert.AreEqual("0ma0", eventBean["indexed[0].mapped('0ma').value"]);
	        Assert.AreEqual("0ma1", eventBean["indexed[0].mapped('0mb').value"]);
	        Assert.AreEqual("1ma0", eventBean["indexed[1].mapped('1ma').value"]);
	        Assert.AreEqual("1ma1", eventBean["indexed[1].mapped('1mb').value"]);

	        Assert.AreEqual("0ma0", eventBean["array[0].mapped('0ma').value"]);
	        Assert.AreEqual("1ma1", eventBean["array[1].mapped('1mb').value"]);

	        TryInvalid(eventBean, "array[0].mapprop('0ma').value");
	        TryInvalid(eventBean, "dummy");
	        TryInvalid(eventBean, "dummy[1]");
	        TryInvalid(eventBean, "dummy('dd')");
	        TryInvalid(eventBean, "dummy.dummy1");
	    }

	    private static void TryInvalid(EventBean eventBean, String propName)
	    {
	        try
	        {
	            Object theVoid = eventBean[propName];
	            Assert.Fail();
	        }
	        catch (PropertyAccessException)
	        {
	            // expected
	        }
	    }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
