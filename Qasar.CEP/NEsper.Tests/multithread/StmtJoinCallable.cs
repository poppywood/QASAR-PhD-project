// ---------------------------------------------------------------------------------- /
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
// ---------------------------------------------------------------------------------- /

using System;
using System.Threading;

using NUnit.Framework;

using net.esper.client;
using net.esper.compat;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.util;
using net.esper.util;

using org.apache.commons.logging;

namespace net.esper.multithread
{
	public class StmtJoinCallable : Callable
	{
	    private readonly int threadNum;
	    private readonly EPServiceProvider engine;
	    private readonly EPStatement stmt;
	    private readonly int numRepeats;

	    public StmtJoinCallable(int threadNum, EPServiceProvider engine, EPStatement stmt, int numRepeats)
	    {
	        this.threadNum = threadNum;
	        this.engine = engine;
	        this.stmt = stmt;
	        this.numRepeats = numRepeats;
	    }

	    public Object Call()
	    {
	        try
	        {
	            // Add assertListener
	            SupportMTUpdateListener assertListener = new SupportMTUpdateListener();
	            ThreadLogUtil.Trace("adding listeners ", assertListener);
	            stmt.AddListener(assertListener);
                using (new TimeTracer<StmtJoinCallable>("Iteration" + numRepeats))
                {
                    for (int loop = 0; loop < numRepeats; loop++)
                    {
                        long id = threadNum*100000000 + loop;
                        Object eventS0 = MakeEvent("s0", id);
                        Object eventS1 = MakeEvent("s1", id);

                        ThreadLogUtil.Trace("SENDING s0 event ", id, eventS0);
                        engine.EPRuntime.SendEvent(eventS0);
                        ThreadLogUtil.Trace("SENDING s1 event ", id, eventS1);
                        engine.EPRuntime.SendEvent(eventS1);

                        //ThreadLogUtil.Info("sent", eventS0, eventS1);
                        // Should have received one that's mine, possible multiple since the statement is used by other threads
                        bool found = false;
                        EventBean[] events = assertListener.GetNewDataListFlattened();
                        foreach (EventBean _event in events)
                        {
                            Object s0Received = _event["s0"];
                            Object s1Received = _event["s1"];
                            //ThreadLogUtil.Info("received", _event.Get("s0"), _event.Get("s1"));
                            if ((s0Received == eventS0) && (s1Received == eventS1))
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                        }
                        Assert.IsTrue(found);
                        assertListener.Reset();
                    }
                }
	        }
	        catch (AssertionException ex)
	        {
	            log.Fatal("Assertion error in thread " + Thread.CurrentThread.ManagedThreadId, ex);
	            return false;
	        }
	        catch (Exception ex)
	        {
	            log.Fatal("Error in thread " + Thread.CurrentThread.ManagedThreadId, ex);
	            return false;
	        }
	        return true;
	    }

	    private SupportBean MakeEvent(String _string, long longPrimitive)
	    {
	        SupportBean _event = new SupportBean();
	        _event.SetLongPrimitive(longPrimitive);
	        _event.SetString(_string);
	        return _event;
	    }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
