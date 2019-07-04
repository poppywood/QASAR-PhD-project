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
using com.espertech.esper.epl.core;
using com.espertech.esper.events;
using com.espertech.esper.view.internals;

namespace com.espertech.esper.epl.join
{
    /// <summary>
    /// Implements a method for pre-loading (initializing) join indexes from a filled buffer.
    /// </summary>
	public class JoinPreloadMethodImpl : JoinPreloadMethod
	{
	    private readonly int numStreams;
	    private readonly BufferView[] bufferViews;
        private readonly JoinSetComposer joinSetComposer;

	    /// <summary>Ctor.</summary>
	    /// <param name="numStreams">number of streams</param>
	    /// <param name="joinSetComposer">the composer holding stream indexes</param>
	    public JoinPreloadMethodImpl(int numStreams, JoinSetComposer joinSetComposer)
	    {
	        this.numStreams = numStreams;
	        this.bufferViews = new BufferView[numStreams];
	        this.joinSetComposer = joinSetComposer;
	    }

	    /// <summary>Sets the buffer for a stream to preload events from.</summary>
	    /// <param name="view">buffer</param>
	    /// <param name="stream">the stream number for the buffer</param>
	    public void SetBuffer(BufferView view, int stream)
	    {
	        bufferViews[stream] = view;
	    }

	    public void PreloadFromBuffer(int stream)
	    {
	        EventBean[] preloadEvents = bufferViews[stream].NewDataBuffer.GetAndFlush();
	        EventBean[][] eventsPerStream = new EventBean[numStreams][];
	        eventsPerStream[stream] = preloadEvents;
	        joinSetComposer.Init(eventsPerStream);
	    }

        public void PreloadAggregation(ResultSetProcessor resultSetProcessor)
        {
            Set<MultiKey<EventBean>> newEvents = joinSetComposer.StaticJoin();
            Set<MultiKey<EventBean>> oldEvents = new HashSet<MultiKey<EventBean>>();
            resultSetProcessor.ProcessJoinResult(newEvents, oldEvents, false);
        }
	}
} // End of namespace
