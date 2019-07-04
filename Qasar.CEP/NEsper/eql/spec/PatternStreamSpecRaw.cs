///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.eql.core;
using net.esper.eql.expression;
using net.esper.eql.spec;
using net.esper.events;
using net.esper.filter;
using net.esper.pattern;
using net.esper.pattern.guard;
using net.esper.pattern.observer;
using net.esper.util;

namespace net.esper.eql.spec
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
	    public PatternStreamSpecRaw(EvalNode evalNode, List<ViewSpec> viewSpecs, String optionalStreamName)
	        : base(optionalStreamName, viewSpecs)
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
        /// Compiles a raw stream specification consisting event type information and filter expressions
        /// to an validated, optimized form for use with filter service
        /// </summary>
        /// <param name="eventAdapterService">supplies type information</param>
        /// <param name="methodResolutionService">for resolving imports</param>
        /// <returns>compiled stream</returns>
        /// <throws>ExprValidationException to indicate validation errors</throws>
	    public StreamSpecCompiled Compile(EventAdapterService eventAdapterService,
                                      MethodResolutionService methodResolutionService,
                                      PatternObjectResolutionService patternObjectResolutionService)
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
	        LinkedDictionary<String, EventType> taggedEventTypes = new LinkedDictionary<String, EventType>();
            foreach (EvalFilterNode filterNode in evalNodeAnalysisResult.FilterNodes)
	        {
	            String eventName = filterNode.RawFilterSpec.EventTypeAlias;
	            EventType eventType = FilterStreamSpecRaw.ResolveType(eventName, eventAdapterService);
	            String optionalTag = filterNode.EventAsName;

	            // If a tag was supplied for the type, the tags must stay with this type, i.e. a=BeanA -> b=BeanA -> a=BeanB is a no
	            if (optionalTag != null)
	            {
	                EventType existingType = taggedEventTypes.Get(optionalTag);
	                if ((existingType != null) && (existingType != eventType))
	                {
	                    throw new ArgumentException("Tag '" + optionalTag + "' for event '" + eventName +
	                            "' has already been declared for events of type " + existingType.UnderlyingType.Name);
	                }
	                taggedEventTypes.Put(optionalTag, eventType);
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
	            List<KeyValuePair<String, EventType>> filterTypes = new List<KeyValuePair<string, EventType>>();
	            filterTypes.Add(new KeyValuePair<string, EventType>(selfStreamName, eventType));
                foreach (KeyValuePair<string, EventType> entry in taggedEventTypes)
                {
                    filterTypes.Add(new KeyValuePair<string, EventType>(entry.Key, entry.Value));
                }

	            StreamTypeService streamTypeService = new StreamTypeServiceImpl(filterTypes, true, false);

	            IList<ExprNode> exprNodes = filterNode.RawFilterSpec.FilterExpressions;
	            FilterSpecCompiled spec = FilterSpecCompiler.MakeFilterSpec(eventType, exprNodes, taggedEventTypes, streamTypeService, methodResolutionService);
	            filterNode.FilterSpec = spec;
	        }

	        return new PatternStreamSpecCompiled(evalNode, taggedEventTypes, this.ViewSpecs, this.OptionalStreamName);
	    }
	}
} // End of namespace