///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.dispatch;
using com.espertech.esper.events;
using com.espertech.esper.util;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Convenience view for dispatching view updates received from a parent view to update listeners
    /// via the dispatch service.
    /// </summary>
	public class UpdateDispatchViewNonBlocking : UpdateDispatchViewBase
	{
        /// <summary>Ctor.</summary>
        /// <param name="dispatchService">for performing the dispatch</param>
        /// <param name="statementResultServiceImpl">handles result delivery</param>

        public UpdateDispatchViewNonBlocking(StatementResultService statementResultServiceImpl,
                                             DispatchService dispatchService)
            : base(statementResultServiceImpl, dispatchService)
        {
        }

        public override void Update(EventBean[] newData, EventBean[] oldData)
        {
            NewResult(new UniformPair<EventBean[]>(newData, oldData));
        }

        public override void NewResult(UniformPair<EventBean[]> results)
        {
            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                ViewSupport.DumpUpdateParams(".update for view " + this, results);
            }

            statementResultServiceImpl.Indicate(results);
            if (!IsDispatchWaiting)
            {
                dispatchService.AddExternal(this);
                IsDispatchWaiting = true;
            }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
