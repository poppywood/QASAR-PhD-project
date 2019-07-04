///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.util;

using log4net;


namespace com.espertech.esper.epl.view
{
    /// <summary>
    /// A view that prepares output events, batching incoming events and invoking the result set processor as necessary.
    /// <para/>
    /// Handles output rate limiting or stabilizing.
    /// </summary>
    public class OutputProcessViewPolicy : OutputProcessView
    {
        private readonly OutputCondition outputCondition;
        private readonly OutputLimitLimitType outputLimitLimitType;

        // Posted events in ordered form (for applying to aggregates) and summarized per type
        // Using ArrayList as random access is a requirement.
        private readonly List<UniformPair<EventBean[]>> viewEventsList = new List<UniformPair<EventBean[]>>();
    	private readonly List<UniformPair<Set<MultiKey<EventBean>>>> joinEventsSet = new List<UniformPair<Set<MultiKey<EventBean>>>>();

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Ctor. </summary>
        /// <param name="resultSetProcessor">is processing the result set for publishing it out</param>
        /// <param name="streamCount">is the number of streams, indicates whether or not this view participates in a join</param>
        /// <param name="outputLimitSpec">is the specification for limiting output (the output condition and the result set processor)</param>
        /// <param name="statementContext">is the services the output condition may depend on</param>
        /// <param name="isInsertInto">is true if the statement is a insert-into</param>
        /// <param name="outputStrategy">is the method to use to produce output</param>
        public OutputProcessViewPolicy(ResultSetProcessor resultSetProcessor,
                              OutputStrategy outputStrategy,
                              bool isInsertInto,
                              int streamCount,
        					  OutputLimitSpec outputLimitSpec,
        					  StatementContext statementContext)
            : base(resultSetProcessor, outputStrategy, isInsertInto, statementContext.StatementResultService)
        {
            log.Debug(".ctor");

        	if(streamCount < 1)
        	{
        		throw new ArgumentException("Output process view is part of at least 1 stream");
        	}

        	OutputCallback outputCallback = GetCallbackToLocal(streamCount);
        	this.outputCondition = statementContext.OutputConditionFactory.CreateCondition(outputLimitSpec, statementContext, outputCallback);
            outputLimitLimitType = outputLimitSpec.DisplayLimit;
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

            int newDataLength = 0;
            int oldDataLength = 0;
            if(newData != null)
            {
            	newDataLength = newData.Length;
            }
            if(oldData != null)
            {
            	oldDataLength = oldData.Length;
            }

            // add the incoming events to the event batches
            viewEventsList.Add(new UniformPair<EventBean[]>(newData, oldData));

            outputCondition.UpdateOutputCondition(newDataLength, oldDataLength);
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

            int newEventsSize = 0;
            if (newEvents != null)
            {
                newEventsSize = newEvents.Count;
            }

            int oldEventsSize = 0;
            if (oldEvents != null)
            {
                oldEventsSize = oldEvents.Count;
            }

            // add the incoming events to the event batches
            Set<MultiKey<EventBean>> copyNew = new LinkedHashSet<MultiKey<EventBean>>(newEvents);
            Set<MultiKey<EventBean>> copyOld = new LinkedHashSet<MultiKey<EventBean>>(oldEvents);
            joinEventsSet.Add(new UniformPair<Set<MultiKey<EventBean>>>(copyNew, copyOld));

            outputCondition.UpdateOutputCondition(newEventsSize, oldEventsSize);
        }

        /// <summary>Called once the output condition has been met.Invokes the result set processor.Used for non-join event data.</summary>
        /// <param name="doOutput">true if the batched events should actually be output as well as processed, false if they should just be processed</param>
        /// <param name="forceUpdate">true if output should be made even when no updating events have arrived</param>
        protected void ContinueOutputProcessingView(bool doOutput, bool forceUpdate)
        {
            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                log.Debug(".continueOutputProcessingView");
            }

            bool isGenerateSynthetic = statementResultService.IsMakeSynthetic;
            bool isGenerateNatural = statementResultService.IsMakeNatural;

            // Process the events and get the result
            UniformPair<EventBean[]> newOldEvents =
                resultSetProcessor.ProcessOutputLimitedView(viewEventsList, isGenerateSynthetic, outputLimitLimitType);

            if ((!isGenerateSynthetic) && (!isGenerateNatural))
            {
                return;
            }

            if (doOutput)
            {
                Output(forceUpdate, newOldEvents);
            }
            ResetEventBatches();
        }

	    private void Output(bool forceUpdate, UniformPair<EventBean[]> results)
	    {
            // Child view can be null in replay from named window
            if (childView != null)
            {
                outputStrategy.Output(forceUpdate, results, childView);
            }
	    }

	    private void ResetEventBatches()
	    {
		    viewEventsList.Clear();
		    joinEventsSet.Clear();
        }

	    /// <summary>Called once the output condition has been met.Invokes the result set processor.Used for join event data.</summary>
	    /// <param name="doOutput">true if the batched events should actually be output as well as processed, false if they should just be processed</param>
	    /// <param name="forceUpdate">true if output should be made even when no updating events have arrived</param>
	    protected void ContinueOutputProcessingJoin(bool doOutput, bool forceUpdate)
	    {
		    if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                log.Debug(".continueOutputProcessingJoin");
            }

            bool isGenerateSynthetic = statementResultService.IsMakeSynthetic;
            bool isGenerateNatural = statementResultService.IsMakeNatural;

            // Process the events and get the result
	        UniformPair<EventBean[]> newOldEvents =
	            resultSetProcessor.ProcessOutputLimitedJoin(joinEventsSet, isGenerateSynthetic, outputLimitLimitType);

            if ((!isGenerateSynthetic) && (!isGenerateNatural))
            {
                return;
            }

		    if(doOutput)
		    {
			    Output(forceUpdate, newOldEvents);
		    }
		    ResetEventBatches();
	    }

        private OutputCallback GetCallbackToLocal(int streamCount)
        {
            // single stream means no join
            // multiple streams means a join
            if(streamCount == 1)
            {
                return delegate(bool doOutput, bool forceUpdate) {
                           ContinueOutputProcessingView(doOutput, forceUpdate);
                       };
            }
            else
            {
                return delegate(bool doOutput, bool forceUpdate) {
                           ContinueOutputProcessingJoin(doOutput, forceUpdate);
                       };
            }
        }
    }
}
