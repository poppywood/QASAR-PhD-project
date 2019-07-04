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
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.events
{
    [TestFixture]
	public class TestPropertyResolution
	{
	    private EPServiceProvider epService;

        [Test]
	    public void testCaseSensitive()
	    {
            Configuration configuration = SupportConfigFactory.Configuration;
            configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_SENSITIVE;
            epService = EPServiceProviderManager.GetDefaultProvider(configuration);

	        EPStatement stmt = epService.EPAdministrator.CreateEQL("select MYPROPERTY, myproperty, MyProperty from " + typeof(SupportBeanDupProperty).FullName);
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        epService.EPRuntime.SendEvent(new SupportBeanDupProperty("lowercamel", "uppercamel", "upper", "lower"));
	        EventBean result = listener.AssertOneGetNewAndReset();
	        Assert.AreEqual("upper", result.Get("MYPROPERTY"));
	        Assert.AreEqual("lower", result.Get("myproperty"));
	        Assert.AreEqual("uppercamel", result.Get("MyProperty"));

            // The following case was deprecated because properties in .NET have different
            // casing than their Java counterparts.  In particular, the case above was actually
            // the lowercamel case in Java. In .NET, the upper and lower camel cases are both
            // valid.

            //try
            //{
            //    epService.EPAdministrator.CreateEQL("select MyProperty from " + typeof(SupportBeanDupProperty).FullName);
            //    Assert.Fail();
            //}
            //catch (EPException ex)
            //{
            //    // expected
            //}
	    }

        [Test]
	    public void testCaseInsensitive()
	    {
	        Configuration configuration = SupportConfigFactory.Configuration;
	        configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;
	        epService = EPServiceProviderManager.GetDefaultProvider(configuration);
	        epService.Initialize();

	        EPStatement stmt = epService.EPAdministrator.CreateEQL("select MYPROPERTY, myproperty, myProperty, MyProperty from " + typeof(SupportBeanDupProperty).FullName);
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        epService.EPRuntime.SendEvent(new SupportBeanDupProperty("lowercamel", "uppercamel", "upper", "lower"));
	        EventBean result = listener.AssertOneGetNewAndReset();
	        Assert.AreEqual("upper", result.Get("MYPROPERTY"));
	        Assert.AreEqual("lower", result.Get("myproperty"));
	        Assert.AreEqual("lowercamel", result.Get("myProperty"));
	        Assert.AreEqual("uppercamel", result.Get("MyProperty"));

	        stmt = epService.EPAdministrator.CreateEQL("select " +
	                "NESTED.NESTEDVALUE as val1, " +
	                "ARRAYPROPERTY[0] as val2, " +
	                "MAPPED('keyOne') as val3, " +
	                "INDEXED[0] as val4 " +
	                " from " + typeof(SupportBeanComplexProps).FullName);
	        stmt.AddListener(listener);
	        epService.EPRuntime.SendEvent(SupportBeanComplexProps.MakeDefaultBean());
	        EventBean _event = listener.AssertOneGetNewAndReset();
	        Assert.AreEqual("nestedValue", _event.Get("val1"));
	        Assert.AreEqual(10, _event.Get("val2"));
	        Assert.AreEqual("valueOne", _event.Get("val3"));
	        Assert.AreEqual(1, _event.Get("val4"));
	    }

        [Test]
	    public void testCaseDistinctInsensitive()
	    {
	        Configuration configuration = SupportConfigFactory.Configuration;
	        configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = PropertyResolutionStyle.DISTINCT_CASE_INSENSITIVE;
	        epService = EPServiceProviderManager.GetDefaultProvider(configuration);
	        epService.Initialize();

	        EPStatement stmt = epService.EPAdministrator.CreateEQL("select MYPROPERTY, myproperty, myProperty, MyProperty from " + typeof(SupportBeanDupProperty).FullName);
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        epService.EPRuntime.SendEvent(new SupportBeanDupProperty("lowercamel", "uppercamel", "upper", "lower"));
	        EventBean result = listener.AssertOneGetNewAndReset();
	        Assert.AreEqual("upper", result.Get("MYPROPERTY"));
	        Assert.AreEqual("lower", result.Get("myproperty"));
	        Assert.AreEqual("lowercamel", result.Get("myProperty"));
            Assert.AreEqual("uppercamel", result.Get("MyProperty"));

	        try
	        {
	            epService.EPAdministrator.CreateEQL("select MyProPerty from " + typeof(SupportBeanDupProperty).FullName);
	            Assert.Fail();
	        }
	        catch (EPException ex)
	        {
	            Assert.AreEqual("Unable to determine which property to use for \"MyProPerty\" because more than one property matched", ex.Message);
	            // expected
	        }
	    }

        [Test]
	    public void testCaseInsensitiveEngineDefault()
	    {
	        Configuration configuration = SupportConfigFactory.Configuration;
	        configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;
	        configuration.AddEventTypeAlias("Bean", typeof(SupportBean).FullName);

	        TryCaseInsensitive(configuration, "select STRING, INTPRIMITIVE from Bean where STRING='A'", "STRING", "INTPRIMITIVE");
	        TryCaseInsensitive(configuration, "select sTrInG, INTprimitIVE from Bean where STRing='A'", "sTrInG", "INTprimitIVE");
	    }

        [Test]
	    public void testCaseInsensitiveTypeConfig()
	    {
	        Configuration configuration = SupportConfigFactory.Configuration;
	        ConfigurationEventTypeLegacy legacyDef = new ConfigurationEventTypeLegacy();
	        legacyDef.PropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;
	        configuration.AddEventTypeAlias("Bean", typeof(SupportBean).FullName, legacyDef);

	        TryCaseInsensitive(configuration, "select STRING, INTPRIMITIVE from Bean where STRING='A'", "STRING", "INTPRIMITIVE");
	        TryCaseInsensitive(configuration, "select sTrInG, INTprimitIVE from Bean where STRing='A'", "sTrInG", "INTprimitIVE");
	    }

	    private void TryCaseInsensitive(Configuration configuration, String stmtText, String propOneName, String propTwoName)
	    {
	        epService = EPServiceProviderManager.GetDefaultProvider(configuration);
	        epService.Initialize();

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        SupportUpdateListener listener = new SupportUpdateListener();
	        stmt.AddListener(listener);

	        epService.EPRuntime.SendEvent(new SupportBean("A", 10));
	        EventBean result = listener.AssertOneGetNewAndReset();
	        Assert.AreEqual("A", result.Get(propOneName));
	        Assert.AreEqual(10, result.Get(propTwoName));
	    }

	}
} // End of namespace
