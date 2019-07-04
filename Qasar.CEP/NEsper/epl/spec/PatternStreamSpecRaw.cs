///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.filter;
using com.espertech.esper.pattern;
using com.espertech.esper.pattern.guard;
using com.espertech.esper.pattern.observer;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Pattern specification in unvalidated, unoptimized form.
	/// </summary>

    public class PatternStreamSpecRaw
        : StreamSpecBase
        , StreamSpecRaw
    {
        private readonly EvalNode evalNode;

        /// <summary>Ctor.</summary>
        /// <param name="evalNode">pattern evaluation node representing pattern statement</param>
        /// <param name="viewSpecs">specifies what view to use to derive data</param>
        /// <param name="optionalStreamName">stream name, or null if none supplied</param>
        /// <param name="isUnidirectional">true to indicate a unidirectional stream in a join, applicable for joins</param>
        public PatternStreamSpecRaw(EvalNode evalNode, IEnumerable<ViewSpec> viewSpecs, String optionalStreamName, bool isUnidirectional)
                : base(optionalStreamName, viewSpecs, isUnidirectional)
        {

            this.evalNode = evalNode;
        }

        /// <summary>
        /// Returns the pattern expression evaluation node for the top pattern operator.
        /// </summary>
        /// <returns>parent pattern expression node</returns>
        public EvalNode EvalNode
        {
            get { return evalNode; }
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
        /// <param name="plugInTypeResolutionURIs">The plugin type resolution Uri</param>
        /// <returns>compiled stream</returns>
        /// <throws>ExprValidationException to indicate validation errors</throws>
        public StreamSpecCompiled Compile(EventAdapterService eventAdapterService, MethodResolutionService methodResolutionService, PatternObjectResolutionService patternObjectResolutionService, TimeProvider timeProvider, NamedWindowService namedWindowService, ValueAddEventService valueAddEventService, VariableService variableService, string engineURI, IList<Uri> plugInTypeResolutionURIs)
        {
            // Determine all the filter nodes used in the pattern
            EvalNodeAnalysisResult evalNodeAnalysisResult = EvalNode.RecursiveAnalyzeChildNodes(evalNode);

            // Resolve guard and observers factories
            try
            {
                foreach (EvalGuardNode guardNode in evalNodeAnalysisResult.GuardNodes)
                {
                    GuardFactory guardFactory = patternObjectResolutionService.Create(guardNode.PatternGuardSpec);
                    guardFactory.GuardParameters = guardNode.PatternGuardSpec.ObjectParameters;
                    guardNode.GuardFactory = guardFactory;
                }
                foreach (EvalObserverNode observerNode in evalNodeAnalysisResult.ObserverNodes)
                {
                    ObserverFactory observerFactory = patternObjectResolutionService.Create(observerNode.PatternObserverSpec);
                    observerFactory.ObserverParameters = observerNode.PatternObserverSpec.ObjectParameters;
                    observerNode.ObserverFactory = observerFactory;
                }
            }
            catch (ObserverParameterException e)
            {
                throw new ExprValidationException("Invalid parameter for pattern observer: " + e.Message, e);
            }
            catch (GuardParameterException e)
            {
                throw new ExprValidationException("Invalid parameter for pattern guard: " + e.Message, e);
            }
            catch (PatternObjectException e)
            {
                throw new ExprValidationException("Failed to resolve pattern object: " + e.Message, e);
            }

            // Resolve all event types; some filters are tagged and we keep the order in which they are specified
            LinkedHashMap<String, Pair<EventType, String>> taggedEventTypes = new LinkedHashMap<String, Pair<EventType, String>>(); 
            foreach (EvalFilterNode filterNode in evalNodeAnalysisResult.FilterNodes)
            {
                String eventName = filterNode.RawFilterSpec.EventTypeAlias;
                EventType eventType = FilterStreamSpecRaw.ResolveType(engineURI, eventName, eventAdapterService, plugInTypeResolutionURIs);
                String optionalTag = filterNode.EventAsName;

                // If a tag was supplied for the type, the tags must stay with this type, i.e. a=BeanA -> b=BeanA -> a=BeanB is a no
                if (optionalTag != null)
                {
                    Pair<EventType, String> pair = taggedEventTypes.Get(optionalTag);
                    EventType existingType = null;
                    if (pair != null)
                    {
                        existingType = pair.First;
                    } 
                    if ((existingType != null) && (existingType != eventType))
                    {
                        throw new ArgumentException("Tag '" + optionalTag + "' for event '" + eventName +
                                "' has already been declared for events of type " + existingType.UnderlyingType.Name);
                    }
                    pair = new Pair<EventType, String>(eventType, eventName);
                    taggedEventTypes.Put(optionalTag, pair);
                }

                // For this filter, filter types are all known tags at this time,
                // and additionally stream 0 (self) is our event type.
                // Stream type service allows resolution by property name event if that name appears in other tags.
                // by defaulting to stream zero.
                // Stream zero is always the current event type, all others follow the order of the map (stream 1 to N).
                String selfStreamName = optionalTag;
                if (selfStreamName == null)
                {
                    selfStreamName = "s_" + UuidGenerator.Generate(filterNode);
                }
                LinkedHashMap<String, Pair<EventType, String>> filterTypes = new LinkedHashMap<String, Pair<EventType, String>>();
                Pair<EventType, String> typePair = new Pair<EventType, String>(eventType, eventName);
                filterTypes.Put(selfStreamName, typePair);
                filterTypes.PutAll(taggedEventTypes);
                StreamTypeService streamTypeService = new StreamTypeServiceImpl(filterTypes, engineURI, true, false);

                IList<ExprNode> exprNodes = filterNode.RawFilterSpec.FilterExpressions;
                FilterSpecCompiled spec = FilterSpecCompiler.MakeFilterSpec(eventType, eventName, exprNodes, taggedEventTypes, streamTypeService, methodResolutionService, timeProvider, variableService);
                filterNode.FilterSpec = spec;
            }

            return new PatternStreamSpecCompiled(evalNode, taggedEventTypes, this.ViewSpecs, this.OptionalStreamName, this.IsUnidirectional);
        }
    }
} // End of namespace
