///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.core;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.filter;
using com.espertech.esper.pattern;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Unvalided filter-based stream specification.
	/// </summary>
	public class FilterStreamSpecRaw
        : StreamSpecBase
        , StreamSpecRaw
        , MetaDefItem
	{
	    private readonly FilterSpecRaw rawFilterSpec;

        /// <summary>Ctor.</summary>
        /// <param name="rawFilterSpec">is unvalidated filter specification</param>
        /// <param name="viewSpecs">is the view definition</param>
        /// <param name="optionalStreamName">is the stream name if supplied, or null if not supplied</param>
        /// <param name="isUnidirectional">true to indicate a unidirectional stream in a join, applicable for joins</param>
        public FilterStreamSpecRaw(FilterSpecRaw rawFilterSpec, IEnumerable<ViewSpec> viewSpecs, String optionalStreamName, bool isUnidirectional)
            : base(optionalStreamName, viewSpecs, isUnidirectional)
        {
            this.rawFilterSpec = rawFilterSpec;
        }

	    /// <summary>Default ctor.</summary>
	    public FilterStreamSpecRaw()
	    {
	    }

	    /// <summary>Returns the unvalided filter spec.</summary>
	    /// <returns>filter def</returns>
	    public FilterSpecRaw RawFilterSpec
	    {
            get { return rawFilterSpec; }
	    }

	    public StreamSpecCompiled Compile(EventAdapterService eventAdapterService,
	                                      MethodResolutionService methodResolutionService,
	                                      PatternObjectResolutionService patternObjectResolutionService,
	                                      TimeProvider timeProvider,
	                                      NamedWindowService namedWindowService,
	                                      ValueAddEventService valueAddEventService,
	                                      VariableService variableService,
	                                      string engineURI,
	                                      IList<Uri> optionalPlugInTypeResolutionURIS)
        {
            StreamTypeService streamTypeService;
            // Determine the event type
            String eventName = rawFilterSpec.EventTypeAlias;
            // Could be a named window
            if (namedWindowService.IsNamedWindow(eventName))
            {
                EventType namedWindowType = namedWindowService.GetProcessor(eventName).TailView.EventType;
                streamTypeService = new StreamTypeServiceImpl(
                    new EventType[] {namedWindowType},
                    new String[] {"s0"},
                    engineURI,
                    new String[] {eventName});

                IList<ExprNode> validatedNodes =
                    FilterSpecCompiler.ValidateDisallowSubquery(rawFilterSpec.FilterExpressions,
                                                                streamTypeService, methodResolutionService, timeProvider,
                                                                variableService);

                return
                    new NamedWindowConsumerStreamSpec(eventName,
                                                      this.OptionalStreamName,
                                                      this.ViewSpecs,
                                                      validatedNodes,
                                                      this.IsUnidirectional);
            }


            EventType eventType = null;

            if (valueAddEventService.IsRevisionTypeAlias(eventName)) {
                eventType = valueAddEventService.GetValueAddUnderlyingType(eventName);
            }

            if (eventType == null) {
                eventType = ResolveType(engineURI, eventName, eventAdapterService, optionalPlugInTypeResolutionURIS);
            }

            // Validate all nodes, make sure each returns a boolean and types are good;
            // Also decompose all AND super nodes into individual expressions
            streamTypeService = new StreamTypeServiceImpl(
                new EventType[] { eventType },
                new String[] { "s0" }, engineURI,
                new String[] { eventName });

            FilterSpecCompiled spec = FilterSpecCompiler.MakeFilterSpec(
                eventType,
                eventName,
                rawFilterSpec.FilterExpressions,
                null,
                streamTypeService,
                methodResolutionService,
                timeProvider,
                variableService);

            return new FilterStreamSpecCompiled(spec, this.ViewSpecs, this.OptionalStreamName, this.IsUnidirectional);
        }

	    /// <summary>Resolves a given event alias to an event type.</summary>
	    /// <param name="eventName">is the alias to resolve</param>
	    /// <param name="eventAdapterService">for resolving event types</param>
	    /// <param name="engineURI">the provider URI</param>
	    /// <param name="optionalResolutionURIs">is URIs for resolving the event name against plug-inn event representations, if any</param>
	    /// <returns>event type</returns>
	    /// <throws>ExprValidationException if the info cannot be resolved</throws>
	    protected internal static EventType ResolveType(String engineURI,
	                                           String eventName,
	                                           EventAdapterService eventAdapterService,
	                                           IList<Uri> optionalResolutionURIs)
        {
            EventType eventType = eventAdapterService.GetEventTypeByAlias(eventName);

            // may already be known
            if (eventType != null) {
                return eventType;
            }

            String engineURIQualifier = engineURI;
            if (engineURI == null) {
                engineURIQualifier = EPServiceProviderConstants.DEFAULT_ENGINE_URI__QUALIFIER;
            }

            // The event name can be prefixed by the engine URI, i.e. "select * from default.MyEvent"
            if (eventName.StartsWith(engineURIQualifier)) {
                int indexDot = eventName.IndexOf(".");
                if (indexDot > 0) {
                    String eventNameURI = eventName.Substring(0, indexDot);
                    String eventNameRemainder = eventName.Substring(indexDot + 1);

                    if (engineURIQualifier == eventNameURI) {
                        eventType = eventAdapterService.GetEventTypeByAlias(eventNameRemainder);
                    }
                }
            }

            // may now be known
            if (eventType != null) {
                return eventType;
            }

            // The type is not known yet, attempt to add as a type with the same alias
            String message = null;
            try {
                eventType = eventAdapterService.AddBeanType(eventName, eventName, true);
            } catch (EventAdapterException ex) {
                log.Info(".resolveType Event type alias '" + eventName + "' not resolved as object event");
                message = "Failed to resolve event type: " + ex.Message;
            }

            // Attempt to use plug-in event types
            try {
                eventType = eventAdapterService.AddPlugInEventType(eventName, optionalResolutionURIs, null);
            } catch (EventAdapterException) {
                log.Debug(".resolveType Event type alias '" + eventName +
                          "' not resolved by plug-in event representations");
                // remains unresolved
            }

            if (eventType == null) {
                throw new ExprValidationException(message);
            }
            return eventType;
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
} // End of namespace
