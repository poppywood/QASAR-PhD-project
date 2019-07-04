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

namespace net.esper.example.feedexample
{
	public class MyListener : UpdateListener
	{
	    public void Update(EventBean[] newEvents, EventBean[] oldEvents)
	    {
	        foreach (EventBean @event in newEvents)
	        {
	            Console.WriteLine("feed " + @event.Get("feed") +
	             " is count " + @event.Get("cnt"));
	        }
	    }
	}
} // End of namespace
