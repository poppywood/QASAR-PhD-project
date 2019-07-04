///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.collection;
using com.espertech.esper.dispatch;
using com.espertech.esper.events;
using com.espertech.esper.timer;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Convenience view for dispatching view updates received from a parent view to update listeners
    /// via the dispatch service.
    /// </summary>
    public class UpdateDispatchViewBlockingSpin : UpdateDispatchViewBase
    {
        private UpdateDispatchFutureSpin currentFutureSpin;
        private readonly long msecTimeout;
        private readonly TimeSourceService timeSourceService;

        /// <summary>Ctor.</summary>
        /// <param name="dispatchService">for performing the dispatch</param>
        /// <param name="msecTimeout">timeout for preserving dispatch order through blocking</param>
        /// <param name="statementResultService">handles result delivery</param>
        /// <param name="timeSourceService">time source provider</param>
        public UpdateDispatchViewBlockingSpin(StatementResultService statementResultService, DispatchService dispatchService, long msecTimeout, TimeSourceService timeSourceService)
            : base(statementResultService, dispatchService)
        {
            this.currentFutureSpin = new UpdateDispatchFutureSpin(timeSourceService); // use a completed future as a start
            this.msecTimeout = msecTimeout;
            this.timeSourceService = timeSourceService;
        }

        public override void Update(EventBean[] newData, EventBean[] oldData)
        {
            NewResult(new UniformPair<EventBean[]>(newData, oldData));
        }

        public override void NewResult(UniformPair<EventBean[]> result)
        {
            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                DumpUpdateParams(".update for view " + this, result);
            }
            statementResultServiceImpl.Indicate(result);

            if (!IsDispatchWaiting)
            {
                UpdateDispatchFutureSpin nextFutureSpin;
                lock (this)
                {
                    nextFutureSpin = new UpdateDispatchFutureSpin(this, currentFutureSpin, msecTimeout, timeSourceService);
                    currentFutureSpin = nextFutureSpin;
                }
                dispatchService.AddExternal(nextFutureSpin);
                IsDispatchWaiting = true;
            }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
} // End of namespace
