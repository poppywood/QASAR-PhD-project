using System;
using System.Collections.Generic;

using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.dispatch
{
    /// <summary>
    /// Implements dispatch service using a thread-local linked list of Dispatchable instances.
    /// </summary>

    public class DispatchServiceImpl : DispatchService
    {
        [ThreadStatic]
        private static Queue<Dispatchable> threadDispatchQueue = new Queue<Dispatchable>();

        private static Queue<Dispatchable> ThreadDispatchQueue
        {
            get
            {
                if (threadDispatchQueue == null)
                {
                    threadDispatchQueue = new Queue<Dispatchable>();
                }
                return threadDispatchQueue;
            }
        }

        /// <summary>
        /// Dispatches events in the queue.
        /// </summary>

        public void Dispatch()
        {
            DispatchFromQueue(ThreadDispatchQueue);
        }

        /// <summary>
        /// Add an item to be dispatched.  The item is added to
        /// the external dispatch queue.
        /// </summary>
        /// <param name="dispatchable">to execute later</param>
        public void AddExternal(Dispatchable dispatchable)
        {
            Queue<Dispatchable> dispatchQueue = ThreadDispatchQueue;
            dispatchQueue.Enqueue(dispatchable);
        }

        private static readonly bool isDebugEnabled;

        static DispatchServiceImpl()
        {
            isDebugEnabled = ExecutionPathDebugLog.isDebugEnabled && log.IsDebugEnabled;
        }

        private static void DispatchFromQueue(Queue<Dispatchable> dispatchQueue)
        {
            if (isDebugEnabled)
            {
                log.Debug(".dispatchFromQueue Dispatch queue is " + dispatchQueue.Count + " elements");
            }

            try
            {
                while (dispatchQueue.Count > 0)
                {
                    dispatchQueue.Dequeue().Execute();
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
