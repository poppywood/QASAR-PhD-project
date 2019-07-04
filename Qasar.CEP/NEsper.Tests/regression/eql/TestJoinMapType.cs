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

namespace net.esper.regression.eql
{
    [TestFixture]
	public class TestJoinMapType
	{
	    private EPServiceProvider epService;
	    private SupportUpdateListener listener;

        [SetUp]
	    public void SetUp()
	    {
	        EDictionary<String, Type> typeInfo = new HashDictionary<String, Type>();
	        typeInfo.Put("id", typeof(String));
	        typeInfo.Put("p00", typeof(int));

	        Configuration config = SupportConfigFactory.Configuration;
	        config.AddEventTypeAlias("MapS0", typeInfo);
	        config.AddEventTypeAlias("MapS1", typeInfo);

	        epService = EPServiceProviderManager.GetDefaultProvider(config);
	        epService.Initialize();
	        listener = new SupportUpdateListener();
	    }

        [Test]
	    public void testJoinMapEvent()
	    {
	        String joinStatement = "select S0.id, S1.id, S0.p00, S1.p00 from MapS0 as S0, MapS1 as S1" +
	                " where S0.id = S1.id";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(joinStatement);
	        stmt.AddListener(listener);

	        SendMapEvent("MapS0", "a", 1);
	        Assert.IsFalse(listener.IsInvoked);

	        SendMapEvent("MapS1", "a", 2);
	        EventBean _event = listener.AssertOneGetNewAndReset();
	        Assert.AreEqual("a", _event.Get("S0.id"));
	        Assert.AreEqual("a", _event.Get("S1.id"));
	        Assert.AreEqual(1, _event.Get("S0.p00"));
	        Assert.AreEqual(2, _event.Get("S1.p00"));

	        SendMapEvent("MapS1", "b", 3);
	        SendMapEvent("MapS0", "c", 4);
	        Assert.IsFalse(listener.IsInvoked);
	    }

        [Test]
	    public void testJoinMapEventNotUnique()
	    {
	        // Test for Esper-122
	        String joinStatement = "select S0.id, S1.id, S0.p00, S1.p00 from MapS0 as S0, MapS1 as S1" +
	                " where S0.id = S1.id";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(joinStatement);
	        stmt.AddListener(listener);

	        for (int i = 0; i < 100; i++)
	        {
	            if (i % 2 == 1)
	            {
	                SendMapEvent("MapS0", "a", 1);
	            }
	            else
	            {
	                SendMapEvent("MapS1", "a", 1);
	            }
	        }
	    }

        [Test]
	    public void testJoinWrapperEventNotUnique()
	    {
	        // Test for Esper-122
	        epService.EPAdministrator.CreateEQL("insert into S0 select 's0' as stream, * from " + typeof(SupportBean).FullName);
	        epService.EPAdministrator.CreateEQL("insert into S1 select 's1' as stream, * from " + typeof(SupportBean).FullName);
	        String joinStatement = "select * from S0 as a, S1 as b where a.intBoxed = b.intBoxed";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(joinStatement);
	        stmt.AddListener(listener);

	        for (int i = 0; i < 100; i++)
	        {
	            epService.EPRuntime.SendEvent(new SupportBean());
	        }
	    }

	    private void SendMapEvent(String alias, String id, int p00)
	    {
            DataDictionary _event = new DataDictionary();
	        _event.Put("id", id);
	        _event.Put("p00", p00);
	        epService.EPRuntime.SendEvent(_event, alias);
	    }
	}
} // End of namespace
