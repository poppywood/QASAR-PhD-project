///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using NUnit.Framework;

using net.esper.client;

namespace net.esper.example.transaction
{
    [TestFixture]
	public abstract class TestStmtBase
	{
	    protected EPServiceProvider epService;

	    public virtual void SetUp()
	    {
	        Configuration configuration = new Configuration();
            configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = PropertyResolutionStyle.CASE_INSENSITIVE;
            configuration.AddEventTypeAlias("TxnEventA", typeof(TxnEventA).FullName);
	        configuration.AddEventTypeAlias("TxnEventB", typeof(TxnEventB).FullName);
	        configuration.AddEventTypeAlias("TxnEventC", typeof(TxnEventC).FullName);

	        epService = EPServiceProviderManager.GetProvider("TestStmtBase", configuration);
	        epService.Initialize();
	    }

	    protected void SendEvent(Object @event)
	    {
	        epService.EPRuntime.SendEvent(@event);
	    }

	}
} // End of namespace
