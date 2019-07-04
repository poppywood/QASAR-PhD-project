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
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.pattern;
using com.espertech.esper.schedule;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// An uncompiled, unoptimize for of stream specification created by a parser.
	/// </summary>
    public interface StreamSpecRaw : StreamSpec
    {
        /// <summary>Compiles a raw stream specification consisting event type information and filter expressionsto an validated, optimized form for use with filter service</summary>
        /// <param name="eventAdapterService">supplies type information</param>
        /// <param name="methodResolutionService">for resolving imports</param>
        /// <param name="patternObjectResolutionService">for resolving pattern objects</param>
        /// <param name="timeProvider">provides engine current time</param>
        /// <param name="namedWindowService">is the service managing named windows</param>
        /// <param name="variableService">provides variable values</param>
        /// <param name="engineURI">the engine URI</param>
        /// <param name="optionalPlugInTypeResolutionURIS">is URIs for resolving the event name against plug-inn event representations, if any</param>
        /// <param name="valueAddEventService">service that handles update events</param>
        /// <returns>compiled stream</returns>
        /// <throws>ExprValidationException to indicate validation errors</throws>

        StreamSpecCompiled Compile(EventAdapterService eventAdapterService, MethodResolutionService methodResolutionService, PatternObjectResolutionService patternObjectResolutionService, TimeProvider timeProvider, NamedWindowService namedWindowService, ValueAddEventService valueAddEventService, VariableService variableService, string engineURI, IList<Uri> optionalPlugInTypeResolutionURIS);
    }
}
