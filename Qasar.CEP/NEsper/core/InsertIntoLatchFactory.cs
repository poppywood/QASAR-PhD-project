///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.timer;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Class to hold a current latch per statement that uses an insert-into stream (per statement and insert-into stream
    /// relationship).
    /// </summary>
	public class InsertIntoLatchFactory
	{
	    private readonly String name;
	    private readonly bool useSpin;
        private readonly TimeSourceService timeSourceService;

	    private InsertIntoLatchSpin currentLatchSpin;
	    private InsertIntoLatchWait currentLatchWait;
	    private readonly long msecWait;

        /// <summary>Ctor.</summary>
        /// <param name="name">the factory name</param>
        /// <param name="msecWait">the number of milliseconds latches will await maximually</param>
        /// <param name="locking">the blocking strategy to employ</param>
        /// <param name="timeSourceService">time source provider</param>
        public InsertIntoLatchFactory(String name,
                                      long msecWait,
                                      ConfigurationEngineDefaults.Locking locking,
                                      TimeSourceService timeSourceService)
        {
	        this.name = name;
	        this.msecWait = msecWait;
            this.timeSourceService = timeSourceService;

	        useSpin = (locking == ConfigurationEngineDefaults.Locking.SPIN);

	        // construct a completed latch as an initial root latch
	        if (useSpin)
	        {
                currentLatchSpin = new InsertIntoLatchSpin(timeSourceService);
	        }
	        else
	        {
	            currentLatchWait = new InsertIntoLatchWait();
	        }
	    }

        /// <summary>
        /// Returns a new latch.
        /// <para>
        /// Need not be synchronized as there is one per statement and execution is during statement lock.
        /// </para>
        /// </summary>
        /// <param name="payload">is the object returned by the await.</param>
        /// <returns>latch</returns>
	    public Object NewLatch(Object payload)
	    {
	        if (useSpin)
	        {
                InsertIntoLatchSpin nextLatch = new InsertIntoLatchSpin(currentLatchSpin, msecWait, payload, timeSourceService);
                currentLatchSpin = nextLatch;
	            return nextLatch;
	        }
	        else
	        {
	            InsertIntoLatchWait nextLatch = new InsertIntoLatchWait(currentLatchWait, msecWait, payload);
	            currentLatchWait.SetLater(nextLatch);
	            currentLatchWait = nextLatch;
	            return nextLatch;
	        }
	    }
	}
} // End of namespace
