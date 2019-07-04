///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;

using com.espertech.esper.dispatch;
using com.espertech.esper.timer;

using log4net;

namespace com.espertech.esper.core
{
    /// <summary>
    /// UpdateDispatchFutureSpin can be added to a dispatch queue that is thread-local. It represents
    /// is a stand-in for a future dispatching of a statement result to statement listeners.
    /// <para>
    /// UpdateDispatchFutureSpin is aware of future and past dispatches:
    /// (newest) DF3   &lt;--&gt;   DF2  &lt;--&gt;  DF1  (oldest), and uses a spin lock to block if required
    /// </para>
    /// </summary>
	public class UpdateDispatchFutureSpin : Dispatchable
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	    private readonly UpdateDispatchViewBlockingSpin view;
	    private UpdateDispatchFutureSpin earlier;
	    private volatile bool isCompleted;
	    private readonly long msecTimeout;
        private readonly TimeSourceService timeSourceService;

        /// <summary>Ctor.</summary>
        /// <param name="view">is the blocking dispatch view through which to execute a dispatch</param>
        /// <param name="earlier">is the older future</param>
        /// <param name="msecTimeout">is the timeout period to wait for listeners to complete a prior dispatch</param>
        /// <param name="timeSourceService">time source provider</param>
        public UpdateDispatchFutureSpin(UpdateDispatchViewBlockingSpin view, UpdateDispatchFutureSpin earlier, long msecTimeout, TimeSourceService timeSourceService)
        {
            this.view = view;
            this.earlier = earlier;
            this.msecTimeout = msecTimeout;
            this.timeSourceService = timeSourceService;
        }

        /// <summary>
        /// Ctor - use for the first future to indicate completion.
        /// </summary>
        /// <param name="timeSourceService">time source provider</param>
        public UpdateDispatchFutureSpin(TimeSourceService timeSourceService)
        {
            isCompleted = true;
            this.timeSourceService = timeSourceService;
        }

	    /// <summary>Returns true if the dispatch completed for this future.</summary>
	    /// <returns>true for completed, false if not</returns>
	    public bool IsCompleted
	    {
            get { return isCompleted; }
	    }

	    public void Execute()
	    {
	        if (!earlier.isCompleted)
	        {
                bool useSpinWait = Environment.ProcessorCount > 1;
	            long spinStartTime = timeSourceService.GetTimeMillis();

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

                    long spinDelta = timeSourceService.GetTimeMillis() - spinStartTime;
	                if (spinDelta > msecTimeout)
	                {
	                    log.Info("Spin wait timeout exceeded in listener dispatch");
	                    break;
	                }
	            }
	        }

	        view.Execute();
	        isCompleted = true;

	        earlier = null;
	    }
	}
} // End of namespace
