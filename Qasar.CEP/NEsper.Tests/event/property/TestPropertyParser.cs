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

using net.esper.client;
using net.esper.compat;
using net.esper.events;

using org.apache.commons.logging;

namespace net.esper.events.property
{
	[TestFixture]
	public class TestPropertyParser
	{
        private BeanEventTypeFactory beanEventTypeFactory;

	    [SetUp]
	    public void SetUp()
	    {
            beanEventTypeFactory = new BeanEventAdapter(new HashDictionary<Type, BeanEventType>());
        }

	    [Test]
	    public void testParse()
	    {
            Property property = PropertyParser.Parse("i[1]", beanEventTypeFactory);
	        Assert.AreEqual("i", ((IndexedProperty)property).PropertyName);
	        Assert.AreEqual(1, ((IndexedProperty)property).Index);

            property = PropertyParser.Parse("m('key')", beanEventTypeFactory);
	        Assert.AreEqual("m", ((MappedProperty)property).PropertyName);
	        Assert.AreEqual("key", ((MappedProperty)property).Key);

            property = PropertyParser.Parse("a", beanEventTypeFactory);
	        Assert.AreEqual("a", ((SimpleProperty)property).PropertyName);

            property = PropertyParser.Parse("a.b[2].c('m')", beanEventTypeFactory);
            IList<Property> nested = ((NestedProperty)property).Properties;
	        Assert.AreEqual(3, nested.Count);
	        Assert.AreEqual("a", ((SimpleProperty)nested[0]).PropertyName);
	        Assert.AreEqual("b", ((IndexedProperty)nested[1]).PropertyName);
	        Assert.AreEqual(2, ((IndexedProperty)nested[1]).Index);
	        Assert.AreEqual("c", ((MappedProperty)nested[2]).PropertyName);
	        Assert.AreEqual("m", ((MappedProperty)nested[2]).Key);
	    }

	    [Test]
	    public void testParseMapKey()
	    {
	        Assert.AreEqual("a", TryKey("a"));
	    }

	    private String TryKey(String key)
	    {
	        String propertyName = "m(\"" + key + "\")";
	        log.Debug(".tryKey propertyName=" + propertyName + " key=" + key);
            Property property = PropertyParser.Parse(propertyName, beanEventTypeFactory);
	        return ((MappedProperty)property).Key;
	    }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace