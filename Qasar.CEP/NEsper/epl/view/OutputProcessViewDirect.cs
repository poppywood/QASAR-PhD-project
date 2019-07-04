///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.epl.core;
using com.espertech.esper.events;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.view
{
    /// <summary>
    /// Output process view that does not enforce any output policies and may simply
    /// hand over events to child views.
    /// </summary>
    public class OutputProcessViewDirect : OutputProcessView
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
        /// <summary>Ctor. </summary>
        /// <param name="resultSetProcessor">is processing the result set for publishing it out</param>
        /// <param name="outputStrategy">is the execution of output to sub-views or natively</param>
        /// <param name="isInsertInto">is true if the statement is a insert-into</param>
        /// <param name="statementResultService">service for managing listener/subscribers and result generation needs</param>
        public OutputProcessViewDirect(ResultSetProcessor resultSetProcessor, OutputStrategy outputStrategy, bool isInsertInto, StatementResultService statementResultService)
            : base(resultSetProcessor, outputStrategy, isInsertInto, statementResultService)
        {
    
            log.Debug(".ctor");
            if (resultSetProcessor == null)
            {
                throw new ArgumentException("Null result set processor, no output processor required");
            }
        }
    
        /// <summary>The update method is called if the view does not participate in a join. </summary>
        /// <param name="newData">new events</param>
        /// <param name="oldData">old events</param>
        public override void Update(EventBean[] newData, EventBean[] oldData)
        {
            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                log.Debug(".update Received update, " +
                        "  newData.Length==" + ((newData == null) ? 0 : newData.Length) +
                        "  oldData.Length==" + ((oldData == null) ? 0 : oldData.Length));
            }
    
            bool isGenerateSynthetic = statementResultService.IsMakeSynthetic;
            bool isGenerateNatural = statementResultService.IsMakeNatural;
    
            UniformPair<EventBean[]> newOldEvents = resultSetProcessor.ProcessViewResult(newData, oldData, isGenerateSynthetic);
    
            if ((!isGenerateSynthetic) && (!isGenerateNatural))
            {
                return;
            }
    
            bool forceOutput = false;
            if ((newData == null) && (oldData == null) &&
                    ((newOldEvents == null) || (newOldEvents.First == null && newOldEvents.Second == null)))
            {
                forceOutput = true;
            }
    
            // Child view can be null in replay from named window
            if (childView != null)
            {
                outputStrategy.Output(forceOutput, newOldEvents, childView);
            }
        }
    
        /// <summary>This process (update) method is for participation in a join. </summary>
        /// <param name="newEvents">new events</param>
        /// <param name="oldEvents">old events</param>
        public override void Process(Set<MultiKey<EventBean>> newEvents, Set<MultiKey<EventBean>> oldEvents)
        {
            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                log.Debug(".process Received update, " +
                        "  newData.Length==" + ((newEvents == null) ? 0 : newEvents.Count) +
                        "  oldData.Length==" + ((oldEvents == null) ? 0 : oldEvents.Count));
            }
    
            bool isGenerateSynthetic = statementResultService.IsMakeSynthetic;
            bool isGenerateNatural = statementResultService.IsMakeNatural;
    
            UniformPair<EventBean[]> newOldEvents = resultSetProcessor.ProcessJoinResult(newEvents, oldEvents, isGenerateSynthetic);
    
            if ((!isGenerateSynthetic) && (!isGenerateNatural))
            {
                return;
            }
    
            if (newOldEvents == null)
            {
                return;
            }
    
            // Child view can be null in replay from named window
            if (childView != null)
            {
                outputStrategy.Output(false, newOldEvents, childView);
            }
        }
    }
}
