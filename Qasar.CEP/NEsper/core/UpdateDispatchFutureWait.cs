///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;

using com.espertech.esper.compat;
using com.espertech.esper.dispatch;

using log4net;

namespace com.espertech.esper.core
{
    /// <summary>
    /// UpdateDispatchFutureWait can be added to a dispatch queue that is thread-local. It represents
    /// is a stand-in for a future dispatching of a statement result to statement listeners.
    /// <para>
    /// UpdateDispatchFutureWait is aware of future and past dispatches:
    /// (newest) DF3   &lt;--&gt;   DF2  &lt;--&gt;  DF1  (oldest)
    /// </para>
    /// </summary>
	public class UpdateDispatchFutureWait : Dispatchable
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	    private readonly UpdateDispatchViewBlockingWait view;
	    private UpdateDispatchFutureWait earlier;
	    private UpdateDispatchFutureWait later;
	    private volatile bool isCompleted;
	    private readonly long msecTimeout;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="view">is the blocking dispatch view through which to execute a dispatch</param>
        /// <param name="earlier">is the older future</param>
        /// <param name="msecTimeout">is the timeout period to wait for listeners to complete a prior dispatch</param>
	    public UpdateDispatchFutureWait(UpdateDispatchViewBlockingWait view, UpdateDispatchFutureWait earlier, long msecTimeout)
	    {
	        this.view = view;
	        this.earlier = earlier;
	        this.msecTimeout = msecTimeout;
	    }

        /// <summary>
        /// Ctor - use for the first future to indicate completion.
        /// </summary>
	    public UpdateDispatchFutureWait()
	    {
	        isCompleted = true;
	    }

        /// <summary>
        /// Returns true if the dispatch completed for this future.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is completed; otherwise, <c>false</c>.
        /// </value>
        /// <returns>true for completed, false if not</returns>
	    public bool IsCompleted
	    {
            get { return isCompleted; }
	    }

        /// <summary>
        /// Hand a later future to the dispatch to use for indicating completion via notify.
        /// </summary>
        /// <param name="later">is the later dispatch</param>
	    public void SetLater(UpdateDispatchFutureWait later)
	    {
	        this.later = later;
	    }

	    public void Execute()
	    {
	        if (!earlier.isCompleted)
	        {
	            lock(this)
	            {
	                if (!earlier.isCompleted)
	                {
                        Monitor.Wait(this, (int) msecTimeout);
	                }
	            }
	        }

	        view.Execute();
	        isCompleted = true;

	        if (later != null)
	        {
	            Monitor.Enter(later);
	            Monitor.Pulse(later);
	            Monitor.Exit(later);
	        }
	        earlier = null;
	        later = null;
	    }
	}
} // End of namespace
