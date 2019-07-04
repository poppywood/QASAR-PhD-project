///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using com.espertech.esper.timer;
using log4net;

namespace com.espertech.esper.core
{
    /// <summary>
    /// A spin-locking implementation of a latch for use in guaranteeing delivery between
    /// a single event produced by a single statement and consumable by another statement.
    /// </summary>
	public class InsertIntoLatchSpin
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    // The earlier latch is the latch generated before this latch
	    private InsertIntoLatchSpin earlier;
	    private readonly long msecTimeout;
	    private readonly Object payload;
        private readonly TimeSourceService timeSourceService;

	    private volatile bool isCompleted;

        /// <summary>Ctor.</summary>
        /// <param name="earlier">the latch before this latch that this latch should be waiting for</param>
        /// <param name="msecTimeout">the timeout after which delivery occurs</param>
        /// <param name="payload">the payload is an event to deliver</param>
        /// <param name="timeSourceService">time source provider</param>
        public InsertIntoLatchSpin(InsertIntoLatchSpin earlier,
                                   long msecTimeout,
                                   Object payload,
                                   TimeSourceService timeSourceService)
        {
            this.earlier = earlier;
            this.msecTimeout = msecTimeout;
            this.payload = payload;
            this.timeSourceService = timeSourceService;
        }

        /// <summary>Ctor - use for the first and unused latch to indicate completion.</summary>
        /// <param name="timeSourceService">time source provider</param>
        public InsertIntoLatchSpin(TimeSourceService timeSourceService)
        {
            isCompleted = true;
            earlier = null;
            msecTimeout = 0;
        }

	    /// <summary>Returns true if the dispatch completed for this future.</summary>
	    /// <returns>true for completed, false if not</returns>
	    public bool IsCompleted
	    {
            get { return isCompleted; }
	    }

	    /// <summary>Blocking call that returns only when the earlier latch completed.</summary>
	    /// <returns>payload of the latch</returns>
	    public Object Await()
	    {
	        if (!earlier.isCompleted)
	        {
	            bool useSpinWait = Environment.ProcessorCount > 1;
                long spinStartTime = timeSourceService.GetTimeMillis();
	            long spinMaxTime = spinStartTime + msecTimeout;

                for (int ii = 0; !earlier.isCompleted; ii++)
                {
                    if (ii < 3 && useSpinWait)
                    {
                        Thread.SpinWait(20);    // Wait a few dozen instructions to let another processor release lock.
                    }
                    else
                    {
                        Thread.Sleep(0);        // Give up my quantum.
                    }

                    if ( timeSourceService.GetTimeMillis() > spinMaxTime )
	                {
	                    log.Info("Spin wait timeout exceeded in insert-into dispatch");
	                    break;
	                }
	            }
	        }

	        return payload;
	    }

	    /// <summary>
	    /// Called to indicate that the latch completed and a later latch can start.
	    /// </summary>
	    public void Done()
	    {
	        isCompleted = true;
	        earlier = null;
	    }
	}
} // End of namespace
