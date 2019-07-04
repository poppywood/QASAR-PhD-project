///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;

using net.esper.dispatch;

using org.apache.commons.logging;

namespace net.esper.core
{
    /// <summary>
    /// DispatchFuture can be added to a dispatch queue that is thread-local. It represents
    /// is a stand-in for a future dispatching of a statement result to statement listeners.
    /// <para>
    /// DispatchFuture is aware of future and past dispatches:
    /// (newest) DF3   &lt;--&gt;   DF2  &lt;--&gt;  DF1  (oldest)
    /// </para>
    /// </summary>
    public class DispatchFuture : Dispatchable
    {
        private UpdateDispatchViewBlocking view;
        private DispatchFuture earlier;
        private DispatchFuture later;
        private int msecTimeout;
        private EventWaitHandle waitHandle;
        private volatile bool isCompleted;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="view">is the blocking dispatch view through which to execute a dispatch</param>
        /// <param name="earlier">is the older future</param>
        /// <param name="msecTimeout">is the timeout period to wait for listeners to complete a prior dispatch</param>
        public DispatchFuture(UpdateDispatchViewBlocking view, DispatchFuture earlier, int msecTimeout)
        {
            this.waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            this.view = view;
            this.earlier = earlier;
            this.msecTimeout = msecTimeout;
        }

        /// <summary>
        /// Ctor - use for the first future to indicate completion.
        /// </summary>
        public DispatchFuture()
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
        public void SetLater(DispatchFuture later)
        {
            this.later = later;
        }

        /// <summary>
        /// Execute dispatch.
        /// </summary>
        public void Execute()
        {
            if (!earlier.isCompleted)
            {
                waitHandle.WaitOne(msecTimeout, true);
            }

            //while(!earlier.isCompleted)
            //{
            //    Thread.Sleep(0); // yield
            //}

            view.Execute();
            isCompleted = true;

            if (later != null)
            {
                later.waitHandle.Set();
            }

            earlier = null;
            later = null;
        }
		
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
} // End of namespace
