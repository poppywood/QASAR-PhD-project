///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.epl.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.pattern;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
    /// <summary>
    /// Specification object for historical data poll via database SQL statement.
    /// </summary>
	public class MethodStreamSpec
        : StreamSpecBase
        , StreamSpecRaw
        , StreamSpecCompiled
        , MetaDefItem
	{
	    private readonly String ident;
	    private readonly String className;
	    private readonly String methodName;
	    private readonly IList<ExprNode> expressions;

        /// <summary>Ctor.</summary>
        /// <param name="optionalStreamName">is the stream name or null if none defined</param>
        /// <param name="viewSpecs">is an list of view specifications</param>
        /// <param name="ident">the prefix in the clause</param>
        /// <param name="className">the class name</param>
        /// <param name="methodName">the method name</param>
        /// <param name="expressions">the parameter expressions</param>
        public MethodStreamSpec(String optionalStreamName,
                                IEnumerable<ViewSpec> viewSpecs,
                                String ident,
                                String className,
                                String methodName,
                                IList<ExprNode> expressions)
	        : base(optionalStreamName, viewSpecs, false)
	    {
	        this.ident = ident;
	        this.className = className;
	        this.methodName = methodName;
	        this.expressions = expressions;
	    }

	    /// <summary>Returns the prefix (method) for the method invocation syntax.</summary>
	    /// <returns>identifier</returns>
	    public String Ident
	    {
            get { return ident; }
	    }

	    /// <summary>Returns the class name.</summary>
	    /// <returns>class name</returns>
	    public String ClassName
	    {
	        get {return className;}
	    }

	    /// <summary>Returns the method name.</summary>
	    /// <returns>method name</returns>
	    public String MethodName
	    {
	        get {return methodName;}
	    }

	    /// <summary>Returns the parameter expressions.</summary>
	    /// <returns>parameter expressions</returns>
	    public IList<ExprNode> Expressions
	    {
	        get { return expressions; }
	    }


        /// <summary>
        /// Compiles a raw stream specification consisting event type information and filter expressionsto an validated, optimized form for use with filter service
        /// </summary>
        /// <param name="eventAdapterService">supplies type information</param>
        /// <param name="methodResolutionService">for resolving imports</param>
        /// <param name="patternObjectResolutionService">for resolving pattern objects</param>
        /// <param name="timeProvider">provides engine current time</param>
        /// <param name="namedWindowService">is the service managing named windows</param>
        /// <param name="valueAddEventService">The value add event service.</param>
        /// <param name="variableService">provides variable values</param>
        /// <param name="engineURI">The engine URI.</param>
        /// <param name="plugInTypeResolutionURIs">The plug in type resolution UR is.</param>
        /// <returns>compiled stream</returns>
        /// <throws>ExprValidationException to indicate validation errors</throws>
        public StreamSpecCompiled Compile(EventAdapterService eventAdapterService, MethodResolutionService methodResolutionService, PatternObjectResolutionService patternObjectResolutionService, TimeProvider timeProvider, NamedWindowService namedWindowService, ValueAddEventService valueAddEventService, VariableService variableService, string engineURI, IList<Uri> plugInTypeResolutionURIs)
        {
	        if (ident != "method")
	        {
	            throw new ExprValidationException("Expecting keyword 'method', found '" + ident + "'");
	        }
	        if (methodName == null)
	        {
	            throw new ExprValidationException("No method name specified for method-based join");
	        }
	        return this;
	    }
	}
} // End of namespace
