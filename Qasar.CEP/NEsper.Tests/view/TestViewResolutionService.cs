// ---------------------------------------------------------------------------------- /
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
// ---------------------------------------------------------------------------------- /

using System;
using System.Collections.Generic;

using NUnit.Framework;

using net.esper.client;
using net.esper.eql.spec;
using net.esper.support.view;
using net.esper.view.stat;

using org.apache.commons.logging;

namespace net.esper.view
{
	[TestFixture]
	public class TestViewResolutionService
	{
	    private ViewResolutionService service;

        [SetUp]
        public void SetUp()
        {
            service = new ViewResolutionServiceImpl(ViewEnumHelper.BuiltinViews);
        }

	    [Test]
	    public void testInitializeFromConfig()
	    {
	        service = CreateService(new String[] {"a", "b"}, new String[] {"v1", "v2"},
	                new String[] {typeof(SupportViewFactoryOne).FullName, typeof(SupportViewFactoryTwo).FullName});

            ViewFactory factory = service.Create("a", "v1");
	        Assert.IsTrue(factory is SupportViewFactoryOne);

            factory = service.Create("b", "v2");
	        Assert.IsTrue(factory is SupportViewFactoryTwo);

	        TryInvalid("a", "v3");
	        TryInvalid("c", "v1");

	        try
	        {
	            service = CreateService(new String[] {"a"}, new String[] {"v1"}, new String[] {"abc"});
	            Assert.Fail();
	        }
	        catch (ConfigurationException ex)
	        {
	            // expected
	        }
	    }

	    private void TryInvalid(String _namespace, String name)
	    {
	        try
	        {
                service.Create(_namespace, name);
	            Assert.Fail();
	        }
	        catch (ViewProcessingException ex)
	        {
	            // expected
	        }
	    }

	    [Test]
	    public void testCreate()
	    {
            ViewFactory viewFactory = service.Create(ViewEnum.UNIVARIATE_STATISTICS.Namespace, ViewEnum.UNIVARIATE_STATISTICS.Name);
	        Assert.IsTrue(viewFactory is UnivariateStatisticsViewFactory);
	    }

	    [Test]
	    public void testInvalidViewName()
	    {
	        try
	        {
                service.Create("dummy", "bumblebee");
	            Assert.IsFalse(true);
	        }
	        catch (ViewProcessingException ex)
	        {
	            log.Debug(".testInvalidViewName Expected exception caught, msg=" + ex.Message);
	        }
	    }

        private ViewResolutionService CreateService(String[] namespaces, String[] names, String[] classNames)
	    {
	        IList<ConfigurationPlugInView> configs = new List<ConfigurationPlugInView>();
	        for (int i = 0; i < namespaces.Length; i++)
	        {
	            ConfigurationPlugInView config = new ConfigurationPlugInView();
	            config.Namespace = namespaces[i];
	            config.Name = names[i];
	            config.FactoryClassName = classNames[i];
	            configs.Add(config);
	        }

            PluggableObjectCollection desc = new PluggableObjectCollection();
            desc.AddViews(configs);
            return new ViewResolutionServiceImpl(desc);
	    }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
