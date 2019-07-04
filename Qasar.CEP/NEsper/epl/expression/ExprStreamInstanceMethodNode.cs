///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;

using CGLib;
using com.espertech.esper.util;
using log4net;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Represents an invocation of a instance method on an event of a given stream in the expression tree.
    /// </summary>
	public class ExprStreamInstanceMethodNode : ExprNode
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly String streamName;
		private readonly String methodName;

	    private int streamNum = -1;
	    private Type[] paramTypes;
		private FastMethod instanceMethod;

	    /// <summary>Ctor.</summary>
	    /// <param name="streamName">
	    /// the declaring class for the method that this node will invoke
	    /// </param>
	    /// <param name="methodName">the name of the method that this node will invoke</param>
		public ExprStreamInstanceMethodNode(String streamName, String methodName)
		{
			if(streamName == null)
			{
				throw new ArgumentException("Stream name is null");
			}
			if(methodName == null)
			{
				throw new ArgumentException("Method name is null");
			}

			this.streamName = streamName;
			this.methodName = methodName;
		}

	    public override bool IsConstantResult
	    {
	        get { return false; }
	    }

		/// <summary>Returns the stream name.</summary>
		/// <returns>the stream that provides events that provide the instance method</returns>
		public String StreamName {
			get {return streamName;}
		}

		/// <summary>Returns the method name.</summary>
		/// <returns>the name of the method</returns>
		public String MethodName {
			get {return methodName;}
		}

		/// <summary>Returns parameter descriptor.</summary>
		/// <returns>the types of the child nodes of this node</returns>
		public Type[] ParamTypes {
			get { return paramTypes; }
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

	    public override String ExpressionString
		{
            get
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append(streamName);
                buffer.Append('.');
                buffer.Append(methodName);

                buffer.Append('(');
                String appendString = "";
                foreach (ExprNode child in ChildNodes)
                {
                    buffer.Append(appendString);
                    buffer.Append(child.ExpressionString);
                    appendString = ", ";
                }
                buffer.Append(')');

                return buffer.ToString();
            }
		}

		public override bool EqualsNode(ExprNode node)
		{
			if(!(node is ExprStreamInstanceMethodNode))
			{
				return false;
			}

			if(instanceMethod == null)
			{
				throw new IllegalStateException("ExprStreamInstanceMethodNode has not been validated");
			}
			else
			{
				ExprStreamInstanceMethodNode otherNode = (ExprStreamInstanceMethodNode) node;
				return streamName.Equals(otherNode.streamName) && instanceMethod.Equals(otherNode.instanceMethod);
			}
		}

		public override void Validate(StreamTypeService streamTypeService, MethodResolutionService methodResolutionService, ViewResourceDelegate viewResourceDelegate, TimeProvider timeProvider, VariableService variableService)
		{
			// Get the types of the childNodes
			IList<ExprNode> childNodes = this.ChildNodes;
			paramTypes = new Type[childNodes.Count];
			int count = 0;

	        foreach (ExprNode childNode in childNodes)
			{
			    paramTypes[count++] = childNode.ReturnType;
	        }

	        String[] streams = streamTypeService.StreamNames;
	        for (int i = 0; i < streams.Length; i++)
	        {
	            if ((streams[i] != null) && (streams[i].Equals(streamName)))
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
	        Type type = eventType.UnderlyingType;

	        // Try to resolve the method
			try
			{
	            MethodInfo method = methodResolutionService.ResolveMethod(type, methodName, paramTypes);
				FastClass declaringClass = FastClass.Create(method.DeclaringType);
				instanceMethod = declaringClass.GetMethod(method);
			}
			catch(Exception e)
			{
	            log.Debug("Error resolving method for instance", e);
	            throw new ExprValidationException(e.Message, e);
			}
		}

		public override Type ReturnType
		{
            get
            {
                if (instanceMethod == null)
                {
                    throw new IllegalStateException("ExprStaticMethodNode has not been validated");
                }
                return TypeHelper.GetBoxedType(instanceMethod.ReturnType);
            }
		}

		public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
		{
	        // get underlying event
	        EventBean @event = eventsPerStream[streamNum];
	        if (@event == null)
	        {
	            return null;
	        }
	        Object underlying = @event.Underlying;

	        // get parameters
	        IList<ExprNode> childNodes = this.ChildNodes;
			Object[] args = new Object[childNodes.Count];
			int count = 0;
			foreach (ExprNode childNode in childNodes)
			{
				args[count++] = childNode.Evaluate(eventsPerStream, isNewData);
			}

			try
			{
	            return instanceMethod.Invoke(underlying, args);
			}
			catch (TargetInvocationException e)
			{
	            log.Warn("Error evaluating instance method by name '" + instanceMethod.Name + "': " + e.Message, e);
	            return null;
			}
		}
	}
} // End of namespace
