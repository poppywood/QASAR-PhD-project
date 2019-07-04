///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using NUnit.Framework;

using net.esper.compat;
using net.esper.support.bean;
using net.esper.support.util;

namespace net.esper.events
{
	[TestFixture]
	public class TestBeanEventAdapter
	{
	    private BeanEventTypeFactory beanEventTypeFactory;

	    [SetUp]
	    public void SetUp()
	    {
            beanEventTypeFactory = new BeanEventAdapter(new HashDictionary<Type, BeanEventType>());
	    }

        [Test]
        public void testCreateBeanType()
        {
            BeanEventType eventType = beanEventTypeFactory.CreateBeanType("a", typeof (SupportBeanSimple));

            Assert.AreEqual(typeof (SupportBeanSimple), eventType.UnderlyingType);
            Assert.AreEqual(2, eventType.PropertyNames.Count);

            // Second call to create the event type, should be the same instance as the first
            EventType eventTypeTwo = beanEventTypeFactory.CreateBeanType("b", typeof (SupportBeanSimple));
            Assert.IsTrue(eventTypeTwo == eventType);

            // Third call to create the event type, getting a given event type id
            EventType eventTypeThree = beanEventTypeFactory.CreateBeanType("c", typeof (SupportBeanSimple));
            Assert.IsTrue(eventTypeThree == eventType);
        }

	    [Test]
	    public void testInterfaceProperty()
	    {
	        // Assert implementations have full set of properties
	        ISupportDImpl _event = new ISupportDImpl("D", "BaseD", "BaseDBase");
	        EventType typeBean = beanEventTypeFactory.CreateBeanType(_event.GetType().FullName, _event.GetType());
            EventBean bean = new BeanEventBean(_event, typeBean);
	        Assert.AreEqual("D", bean["d"]);
	        Assert.AreEqual("BaseD", bean["baseD"]);
	        Assert.AreEqual("BaseDBase", bean["baseDBase"]);
	        Assert.AreEqual(3, bean.EventType.PropertyNames.Count);
	        ArrayAssertionUtil.AreEqualAnyOrder(bean.EventType.PropertyNames,
	                new String[] {"D", "BaseD", "BaseDBase"});

	        // Assert intermediate interfaces have full set of fields
            EventType interfaceType = beanEventTypeFactory.CreateBeanType("d", typeof(ISupportD));
            ArrayAssertionUtil.AreEqualAnyOrder(interfaceType.PropertyNames,
	                new String[] {"D", "BaseD", "BaseDBase"});
	    }

	    [Test]
	    public void testMappedIndexedNestedProperty()
	    {
    	    EventType eventType = beanEventTypeFactory.CreateBeanType("e", typeof(SupportBeanComplexProps));

	        Assert.AreEqual(typeof(Properties), eventType.GetPropertyType("mapProperty"));
	        Assert.AreEqual(typeof(String), eventType.GetPropertyType("mapped('x')"));
	        Assert.AreEqual(typeof(int), eventType.GetPropertyType("indexed[1]"));
	        Assert.AreEqual(typeof(SupportBeanComplexProps.SupportBeanSpecialGetterNested), eventType.GetPropertyType("nested"));
	        Assert.AreEqual(typeof(int[]), eventType.GetPropertyType("arrayProperty"));
	    }
	}
} // End of namespace
