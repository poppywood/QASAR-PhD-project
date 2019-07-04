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
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Processor for select-clause expressions that handles a list of selection items
    /// represented by expression nodes. Computes results based on matching events.
    /// </summary>
    public class SelectExprEvalProcessor : SelectExprProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ExprEvaluator[] expressionNodes;
        private String[] columnNames;
        private EventType resultEventType;
        private EventType vaeInnerEventType;
        private readonly EventAdapterService eventAdapterService;
        private readonly bool isUsingWildcard;
        private readonly bool singleStreamWrapper;
        private bool singleColumnCoercion;
        private readonly SelectExprJoinWildcardProcessor joinWildcardProcessor;
        private bool isRevisionEvent;
        private ValueAddEventProcessor vaeProcessor;

        /// <summary>Ctor.</summary>
        /// <param name="selectionList">list of select-clause items</param>
        /// <param name="insertIntoDesc">descriptor for insert-into clause contains column names overriding select clause names</param>
        /// <param name="isUsingWildcard">true if the wildcard (*) appears in the select clause</param>
        /// <param name="typeService">service for information about streams</param>
        /// <param name="eventAdapterService">service for generating events and handling event types</param>
        /// <param name="revisionService">service that handles update events</param>
        /// <throws>com.espertech.esper.epl.expression.ExprValidationException thrown if any of the expressions don't validate</throws>
        public SelectExprEvalProcessor(IList<SelectClauseExprCompiledSpec> selectionList,
                                       InsertIntoDesc insertIntoDesc,
                                       bool isUsingWildcard,
                                       StreamTypeService typeService,
                                       EventAdapterService eventAdapterService,
                                       ValueAddEventService revisionService)
        {
            this.eventAdapterService = eventAdapterService;
            this.isUsingWildcard = isUsingWildcard;

            if (selectionList.Count == 0 && !isUsingWildcard)
            {
                throw new ArgumentException("Empty selection list not supported");
            }

            foreach (SelectClauseExprCompiledSpec entry in selectionList)
            {
                if (entry.AssignedName == null)
                {
                    throw new ArgumentException("Expected name for each expression has not been supplied");
                }
            }

            // Verify insert into clause
            if (insertIntoDesc != null)
            {
                VerifyInsertInto(insertIntoDesc, selectionList);
            }

            // Build a subordinate wildcard processor for joins
            if(typeService.StreamNames.Length > 1 && isUsingWildcard)
            {
            	joinWildcardProcessor = new SelectExprJoinWildcardProcessor(typeService.StreamNames, typeService.EventTypes, eventAdapterService, null);
            }

            // Resolve underlying event type in the case of wildcard select
            EventType underlyingType = null;
            if(isUsingWildcard)
            {
            	if(joinWildcardProcessor != null)
            	{
            		underlyingType = joinWildcardProcessor.ResultEventType;
            	}
            	else
            	{
            		underlyingType = typeService.EventTypes[0];
            		if(underlyingType is WrapperEventType)
            		{
            			singleStreamWrapper = true;
            		}
            	}
            }

            Init(selectionList, insertIntoDesc, underlyingType, eventAdapterService, typeService, revisionService);
        }

        private void Init(IList<SelectClauseExprCompiledSpec> selectionList,
                          InsertIntoDesc insertIntoDesc,
                          EventType eventType,
                          EventAdapterService eventAdapterService,
                          StreamTypeService typeService,
                          ValueAddEventService valueAddEventService)
        {
            // Get expression nodes
            expressionNodes = new ExprEvaluator[selectionList.Count];
            Object[] expressionReturnTypes = new Object[selectionList.Count];
            for (int i = 0; i < selectionList.Count; i++)
            {
                ExprNode expr = selectionList[i].SelectExpression;
                expressionNodes[i] = expr;
                expressionReturnTypes[i] = expr.ReturnType;
            }

            // Get column names
            if ((insertIntoDesc != null) && CollectionHelper.IsNotEmpty(insertIntoDesc.ColumnNames))
            {
                columnNames = CollectionHelper.ToArray(insertIntoDesc.ColumnNames);
            }
            else
            {
                columnNames = new String[selectionList.Count];
                for (int i = 0; i < selectionList.Count; i++)
                {
                    columnNames[i] = selectionList[i].AssignedName;
                }
            }

            // Find if there is any tagged event types:
            // This is a special case for patterns: select a, b from pattern [a=A -> b=B]
            // We'd like to maintain 'A' and 'B' EventType in the Map type, and 'a' and 'b' EventBeans in the event bean
            for (int i = 0; i < selectionList.Count; i++)
            {
                ExprIdentNode identNode = expressionNodes[i] as ExprIdentNode;
                if ( identNode == null ) {
                    continue;
                }

                String propertyName = identNode.ResolvedPropertyName;
                int streamNum = identNode.StreamId;

                EventType eventTypeStream = typeService.EventTypes[streamNum];
                TaggedCompositeEventType comp = eventTypeStream as TaggedCompositeEventType;
                if ( comp == null ) {
                    continue;
                }

                Pair<EventType, String> pair = comp.TaggedEventTypes.Get(propertyName);
                if (pair == null)
                {
                    continue;
                }

                // A match was found, we replace the expression
                String tagName = propertyName;
                ExprEvaluator evaluator =
                    new ProxyExprEvaluator(
                        delegate(EventBean[] eventsPerStream, bool isNewData) {
                            EventBean streamEvent = eventsPerStream[streamNum];
                            if (streamEvent == null) {
                                return null;
                            }
                            TaggedCompositeEventBean taggedComposite = (TaggedCompositeEventBean) streamEvent;
                            return taggedComposite.GetEventBean(tagName);
                        });

                expressionNodes[i] = evaluator;
                expressionReturnTypes[i] = pair.First;
            }

            // Find if there is any stream expression (ExprStreamNode) :
            // This is a special case for stream selection: select a, b from A as a, B as b
            // We'd like to maintain 'A' and 'B' EventType in the Map type, and 'a' and 'b' EventBeans in the event bean
            for (int i = 0; i < selectionList.Count; i++) {
                ExprStreamUnderlyingNode undNode = expressionNodes[i] as ExprStreamUnderlyingNode;
                if (undNode == null) {
                    continue;
                }

                int streamNum = undNode.StreamId;
                EventType eventTypeStream = typeService.EventTypes[streamNum];

                // A match was found, we replace the expression
                ExprEvaluator evaluator =
                    new ProxyExprEvaluator(
                        delegate(EventBean[] eventsPerStream, bool isNewData) {
                            return eventsPerStream[streamNum];
                        });

                expressionNodes[i] = evaluator;
                expressionReturnTypes[i] = eventTypeStream;
            }

            // Build event type
            Map<String, Object> selPropertyTypes = new HashMap<String, Object>();
            for (int i = 0; i < expressionNodes.Length; i++)
            {
                Object expressionReturnType = expressionReturnTypes[i];
                selPropertyTypes.Put(columnNames[i], expressionReturnType);
            }

            // If we have an alias for this type, add it
            if (insertIntoDesc != null)
            {
                try
                {
                    vaeProcessor = valueAddEventService.GetValueAddProcessor(insertIntoDesc.EventTypeAlias);
                    if (isUsingWildcard)
                    {
                        if (vaeProcessor != null)
                        {
                            resultEventType = vaeProcessor.ValueAddEventType;
                            isRevisionEvent = true;
                            vaeProcessor.ValidateEventType(eventType);
                        }
                        else
                        {
                            resultEventType = eventAdapterService.AddWrapperType(insertIntoDesc.EventTypeAlias, eventType, selPropertyTypes);
                        }
                    }
                    else
                    {
                        resultEventType = null;
                        if ((columnNames.Length == 1) && (insertIntoDesc.ColumnNames.Count == 0))
                        {
                            EventType existingType = eventAdapterService.GetEventTypeByAlias(insertIntoDesc.EventTypeAlias);
                            if (existingType != null)
                            {
                                // check if the existing type and new type are compatible
                                Object columnOneType = expressionReturnTypes[0];
                                if (existingType is WrapperEventType)
                                {
                                    WrapperEventType wrapperType = (WrapperEventType) existingType;
                                    // Map and Object both supported
                                    if (wrapperType.UnderlyingEventType.UnderlyingType == columnOneType)
                                    {
                                        singleColumnCoercion = true;
                                        resultEventType = existingType;
                                    }
                                }
                            }
                        }
                        if (resultEventType == null)
                        {
                            if (vaeProcessor != null)
                            {
                                resultEventType = eventAdapterService.CreateAnonymousMapType(selPropertyTypes);
                            }
                            else
                            {
                                resultEventType = eventAdapterService.AddNestableMapType(insertIntoDesc.EventTypeAlias, selPropertyTypes);
                            }
                        }

                        if (vaeProcessor != null)
                        {
                            vaeProcessor.ValidateEventType(resultEventType);
                            vaeInnerEventType = resultEventType;
                            resultEventType = vaeProcessor.ValueAddEventType;
                            isRevisionEvent = true;
                        }
                    }
                }
                catch (EventAdapterException ex)
                {
                    throw new ExprValidationException(ex.Message);
                }
            }
            else
            {
                if (isUsingWildcard)
                {
            	    resultEventType = eventAdapterService.CreateAnonymousWrapperType(eventType, selPropertyTypes);
                }
                else
                {
                    resultEventType = eventAdapterService.CreateAnonymousMapType(selPropertyTypes);
                }
            }

            if (log.IsDebugEnabled)
            {
                log.Debug(".init resultEventType=" + resultEventType);
            }
        }

        public EventBean Process(EventBean[] eventsPerStream, bool isNewData, bool isSynthesize)
        {
            // Evaluate all expressions and build a map of name-value pairs
            Map<String, Object> props = new HashMap<String, Object>();
            for (int i = 0; i < expressionNodes.Length; i++)
            {
                Object evalResult = expressionNodes[i].Evaluate(eventsPerStream, isNewData);
                props.Put(columnNames[i], evalResult);
            }

            if(isUsingWildcard)
            {
            	// In case of a wildcard and single stream that is itself a
            	// wrapper bean, we also need to add the map properties
            	if(singleStreamWrapper)
            	{
            		DecoratingEventBean wrapper = (DecoratingEventBean)eventsPerStream[0];
            		if(wrapper != null)
            		{
            			Map<String, Object> map = wrapper.DecoratingProperties;
                        if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
                        {
            			    log.Debug(".process additional properties=" + map);
                        }
                        props.PutAll(map);
            		}
            	}

                EventBean @event;
                if(joinWildcardProcessor != null) {
                    @event = joinWildcardProcessor.Process(eventsPerStream, isNewData, isSynthesize);
                } else {
                    @event = eventsPerStream[0];
                }

                if (isRevisionEvent) {
                    return vaeProcessor.GetValueAddEventBean(@event);
                } else {
                    // Using a wrapper bean since we cannot use the same event type else same-type filters match.
                    // Wrapping it even when not adding properties is very inexpensive.
                    return eventAdapterService.CreateWrapper(@event, props, resultEventType);
                }
            }
            else
            {
                if (singleColumnCoercion)
                {
                    Object result = props.Get(columnNames[0]);
                    EventBean wrappedEvent;
                    if (result is Map<string,object>)
                    {
                        wrappedEvent = eventAdapterService.CreateMapFromValues((Map<string,object>)result, resultEventType);
                    }
                    else
                    {
                        wrappedEvent = eventAdapterService.AdapterForBean(result);
                    }
                    props.Clear();
                    if (!isRevisionEvent)
                    {
                        return eventAdapterService.CreateWrapper(wrappedEvent, props, resultEventType);
                    }
                    else
                    {
                        return vaeProcessor.GetValueAddEventBean(eventAdapterService.CreateWrapper(wrappedEvent, props, vaeInnerEventType));
                    }
                }
                else
                {
                    if (!isRevisionEvent)
                    {
                        return eventAdapterService.CreateMapFromValues(props, resultEventType);
                    }
                    else
                    {
                        return vaeProcessor.GetValueAddEventBean(eventAdapterService.CreateMapFromValues(props, vaeInnerEventType));
                    }
                }
            }
        }

        public EventType ResultEventType
        {
            get { return resultEventType; }
        }

        private static void VerifyInsertInto(InsertIntoDesc insertIntoDesc,
                                             ICollection<SelectClauseExprCompiledSpec> selectionList)
        {
            // Verify all column names are unique
            Set<String> names = new HashSet<String>();
            foreach (String element in insertIntoDesc.ColumnNames)
            {
                if (names.Contains(element))
                {
                    throw new ExprValidationException("Property name '" + element + "' appears more then once in insert-into clause");
                }
                names.Add(element);
            }

            // Verify number of columns matches the select clause
            if ( (CollectionHelper.IsNotEmpty(insertIntoDesc.ColumnNames)) &&
                 (insertIntoDesc.ColumnNames.Count != selectionList.Count) )
            {
                throw new ExprValidationException("Number of supplied values in the select clause does not match insert-into clause");
            }
        }
    }
}
