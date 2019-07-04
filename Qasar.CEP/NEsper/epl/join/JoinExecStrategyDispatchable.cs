///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.events;
using com.espertech.esper.view.internals;

namespace com.espertech.esper.epl.join
{
    /// <summary>
    /// This class reacts to any new data buffered by registring with the dispatch service.
    /// When dispatched via execute, it takes the buffered events and hands these to the join
    /// execution strategy.
    /// </summary>

    public class JoinExecStrategyDispatchable
		: EPStatementDispatch
        , BufferObserver
    {
        private readonly JoinExecutionStrategy joinExecutionStrategy;
        private readonly Map<Int32, FlushedEventBuffer> oldStreamBuffer;
        private readonly Map<Int32, FlushedEventBuffer> newStreamBuffer;
        private readonly int numStreams;

        private bool hasNewData;

        /// <summary> CTor.</summary>
        /// <param name="joinExecutionStrategy">strategy for executing the join</param>
        /// <param name="numStreams">number of stream</param>

        public JoinExecStrategyDispatchable(JoinExecutionStrategy joinExecutionStrategy, int numStreams)
        {
            this.joinExecutionStrategy = joinExecutionStrategy;
            this.numStreams = numStreams;

            oldStreamBuffer = new HashMap<Int32, FlushedEventBuffer>();
            newStreamBuffer = new HashMap<Int32, FlushedEventBuffer>();
        }

        /// <summary>
        /// Execute pending dispatchable items.
        /// </summary>
        public virtual void Execute()
        {
            if (!hasNewData)
	        {
	            return;
	        }
	        hasNewData = false;

            EventBean[][] oldDataPerStream = new EventBean[numStreams][];
            EventBean[][] newDataPerStream = new EventBean[numStreams][];

            for (int i = 0; i < numStreams; i++)
            {
                oldDataPerStream[i] = GetBufferData(oldStreamBuffer.Get(i, null));
                newDataPerStream[i] = GetBufferData(newStreamBuffer.Get(i, null));
            }

            joinExecutionStrategy.Join(newDataPerStream, oldDataPerStream);
        }

        private static EventBean[] GetBufferData(FlushedEventBuffer buffer)
        {
            return
                (buffer != null) ?
                (buffer.GetAndFlush()) :
                (null);
        }

        /// <summary>
        /// Receive new and old events from a stream.
        /// </summary>
        /// <param name="streamId">the stream number sending the events</param>
        /// <param name="newEventBuffer">buffer for new events</param>
        /// <param name="oldEventBuffer">buffer for old events</param>
        public virtual void NewData(int streamId, FlushedEventBuffer newEventBuffer, FlushedEventBuffer oldEventBuffer)
        {
			hasNewData = true;
            newStreamBuffer[streamId] = newEventBuffer;
            oldStreamBuffer[streamId] = oldEventBuffer;
        }
    }
}
