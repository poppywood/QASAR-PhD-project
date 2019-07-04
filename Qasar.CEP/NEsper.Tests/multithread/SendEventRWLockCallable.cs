///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using net.esper.client;
using net.esper.compat;
using net.esper.support.util;

using org.apache.commons.logging;

namespace net.esper.multithread
{
	public class SendEventRWLockCallable : Callable
	{
	    private readonly int threadNum;
	    private readonly EPServiceProvider engine;
	    private readonly IEnumerator<Object> events;
	    private readonly FastReaderWriterLock sharedStartLock;

	    public SendEventRWLockCallable(int threadNum, FastReaderWriterLock sharedStartLock, EPServiceProvider engine, IEnumerator<Object> events)
	    {
	        this.threadNum = threadNum;
	        this.engine = engine;
	        this.events = events;
	        this.sharedStartLock = sharedStartLock;
	    }

	    public Object Call()
	    {
            using (new ReaderLock(sharedStartLock))
            {
                log.Info(".call Thread " + Thread.CurrentThread.ManagedThreadId + " starting");
                try
                {
                    EPRuntime epRuntime = engine.EPRuntime;

                    while (events.MoveNext())
                    {
                        log.Info(".call -- sending event");
                        epRuntime.SendEvent(events.Current);
                    }
                }
                catch (Exception ex)
                {
                    log.Fatal("Error in thread " + threadNum, ex);
                    return false;
                }
                log.Info(".call Thread " + Thread.CurrentThread.ManagedThreadId + " done");
            }
	        return true;
	    }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
