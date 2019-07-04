///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Represents an stream selector that returns the streams underlying event, or null if undefined.
    /// </summary>
	public class ExprStreamUnderlyingNode : ExprNode
	{
	    private readonly String streamName;
	    private int streamNum = -1;
	    private Type type;

	    /// <summary>Ctor.</summary>
	    /// <param name="streamName">
	    /// is the name of the stream for which to return the underlying event
	    /// </param>
	    public ExprStreamUnderlyingNode(String streamName)
	    {
	        if (streamName == null)
	        {
	            throw new ArgumentException("Stream name is null");
	        }
	        this.streamName = streamName;
	    }

	    /// <summary>Returns the stream name.</summary>
	    /// <returns>stream name</returns>
	    public String StreamName
	    {
	        get { return streamName; }
	    }

	    public override void Validate(StreamTypeService streamTypeService, MethodResolutionService methodResolutionService, ViewResourceDelegate viewResourceDelegate, TimeProvider timeProvider, VariableService variableService)
	    {
	        String[] streams = streamTypeService.StreamNames;
	        for (int i = 0; i < streams.Length; i++)
	        {
	            if ((streams[i] != null) && (streams[i] == streamName))
	            {
	                streamNum = i;
	                break;
	            }
	        }

	        if (streamNum == -1)
	        {
	            throw new ExprValidationException("Stream by name '" + streamName + "' could not be found among all streams");
	        }

	        EventType eventType = streamTypeService.EventTypes[streamNum];
	        type = eventType.UnderlyingType;
	    }

	    public override Type ReturnType
	    {
            get
            {
                if (streamNum == -1)
                {
                    throw new IllegalStateException("Stream underlying node has not been validated");
                }
                return type;
            }
	    }

	    public override bool IsConstantResult
	    {
	        get { return false; }
	    }

	    /// <summary>Returns stream id supplying the property value.</summary>
	    /// <returns>stream number</returns>
	    public int StreamId
	    {
            get
            {
                if (streamNum == -1)
                {
                    throw new IllegalStateException("Stream underlying node has not been validated");
                }
                return streamNum;
            }
	    }

	    public override String ToString()
	    {
	        return "streamName=" + streamName +
	                " streamNum=" + streamNum;
	    }

	    public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
	    {
	        EventBean @event = eventsPerStream[streamNum];
	        if (@event == null)
	        {
	            return null;
	        }
	        return @event.Underlying;
	    }

	    public override String ExpressionString
	    {
            get
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append(streamName);
                return buffer.ToString();
            }
	    }

	    public override bool EqualsNode(ExprNode node)
	    {
	        if (!(node is ExprStreamUnderlyingNode))
	        {
	            return false;
	        }

	        ExprStreamUnderlyingNode other = (ExprStreamUnderlyingNode) node;

	        return (this.streamName == other.streamName);
	    }
	}
} // End of namespace
