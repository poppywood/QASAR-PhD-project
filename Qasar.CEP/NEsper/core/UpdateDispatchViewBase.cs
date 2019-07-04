///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.dispatch;
using com.espertech.esper.events;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Convenience view for dispatching view updates received from a parent view to update listeners
    /// via the dispatch service.
    /// </summary>
    public abstract class UpdateDispatchViewBase
        : ViewSupport
        , Dispatchable
        , UpdateDispatchView
    {
        /// <summary>Handles result delivery</summary>
        protected readonly StatementResultService statementResultServiceImpl;

        /// <summary>Dispatches events to listeners.</summary>
        protected readonly DispatchService dispatchService;

        /// <summary>For iteration with patterns.</summary>
        protected EventBean lastIterableEvent;

        /// <summary>
        /// Thread local data
        /// </summary>
        protected internal class ThreadLocalData
        {
            public bool isDispatchWaiting = false;
        }

        private readonly ThreadLocal<ThreadLocalData> localData =
            new FastThreadLocal<ThreadLocalData>(CreateLocalData);

        /// <summary>
        /// Gets the local data.
        /// </summary>
        /// <value>The local data.</value>
        protected internal ThreadLocalData LocalData
        {
            get { return localData.GetOrCreate(); }
        }

        /// <summary>
        /// Creates the local data.
        /// </summary>
        /// <returns></returns>
        private static ThreadLocalData CreateLocalData()
        {
            return new ThreadLocalData();
        }

        /// <summary>Flag to indicate we have registered a dispatch.</summary>
        protected bool IsDispatchWaiting
        {
            get { return LocalData.isDispatchWaiting; }
            set { LocalData.isDispatchWaiting = value; }
        }

        /// <summary>Ctor.</summary>
        /// <param name="dispatchService">for performing the dispatch</param>
        /// <param name="statementResultServiceImpl">handles result delivery</param>
        public UpdateDispatchViewBase(StatementResultService statementResultServiceImpl, DispatchService dispatchService)
        {
            this.dispatchService = dispatchService;
            this.statementResultServiceImpl = statementResultServiceImpl;
        }

        /// <summary>
        /// Provides metadata information about the type of object the event collection contains.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// metadata for the objects in the collection
        /// </returns>
        public override EventType EventType
        {
            get { return null; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public override IEnumerator<EventBean> GetEnumerator()
        {
            throw new UnsupportedOperationException();
        }

        /// <summary>
        /// Execute dispatch.
        /// </summary>
        public void Execute()
        {
            ThreadLocalData threadLocal = LocalData;
            threadLocal.isDispatchWaiting = false;
            statementResultServiceImpl.Execute();
        }

        /// <summary>
        /// Remove event reference to last event.
        /// </summary>
        public void Clear()
        {
            lastIterableEvent = null;
        }

        /// <summary>
        /// Convenience method that accepts a pair of new and old data as this is the most treated unit.
        /// </summary>
        /// <param name="result">is new data (insert stream) and old data (remove stream)</param>
        public abstract void NewResult(UniformPair<EventBean[]> result);

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
} // End of namespace
