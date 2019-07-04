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
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Processor for select-clause expressions that handles a list of selection items 
    /// represented by expression nodes. Computes results based on matching events. 
    /// </summary>
    public class SelectExprEvalProcessorStreams : SelectExprProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
        private readonly EventAdapterService eventAdapterService;
        private readonly List<SelectClauseStreamCompiledSpec> aliasedStreams;
        private readonly List<SelectClauseStreamCompiledSpec> unaliasedStreams;
        private readonly bool singleStreamWrapper;
        private readonly bool isUsingWildcard;
    
        private ExprNode[] expressionNodes;
        private String[] columnNames;
        private EventType resultEventType;
        private readonly EventType underlyingEventType;
        private readonly int underlyingStreamNumber;
        private readonly bool underlyingIsTaggedEvent;
        private EventPropertyGetter underlyingPropertyEventGetter;

        /// <summary>Ctor. </summary>
        /// <param name="selectionList">list of select-clause items</param>
        /// <param name="insertIntoDesc">descriptor for insert-into clause contains column names overriding select clause names</param>
        /// <param name="isUsingWildcard">true if the wildcard (*) appears in the select clause</param>
        /// <param name="typeService">service for information about streams</param>
        /// <param name="eventAdapterService">service for generating events and handling event types</param>
        /// <param name="selectedStreams">list of stream selectors (e.g. select alias.* from Event as alias)</param>
        /// <throws>com.espertech.esper.epl.expression.ExprValidationException thrown if any of the expressions don't validate</throws>
        public SelectExprEvalProcessorStreams(IList<SelectClauseExprCompiledSpec> selectionList,
                                              IEnumerable<SelectClauseStreamCompiledSpec> selectedStreams,
                                              InsertIntoDesc insertIntoDesc,
                                              bool isUsingWildcard,
                                              StreamTypeService typeService,
                                              EventAdapterService eventAdapterService)
        {
            this.eventAdapterService = eventAdapterService;
            this.isUsingWildcard = isUsingWildcard;
    
            // Get the un-aliased stream selectors (i.e. select s0.* from S0 as s0)
            unaliasedStreams = new List<SelectClauseStreamCompiledSpec>();
            aliasedStreams = new List<SelectClauseStreamCompiledSpec>();
            foreach (SelectClauseStreamCompiledSpec spec in selectedStreams)
            {
                if (spec.OptionalAliasName == null)
                {
                    unaliasedStreams.Add(spec);
                }
                else
                {
                    aliasedStreams.Add(spec);
                    if (spec.IsProperty) {
                        throw new ExprValidationException("The property wildcard syntax must be used without alias");
                    }
                }
            }
    
            // Verify insert into clause
            if (insertIntoDesc != null)
            {
                VerifyInsertInto(insertIntoDesc, selectionList, aliasedStreams);
            }
    
            // Error if there are more then one un-aliased streams (i.e. select s0.*, s1.* from S0 as s0, S1 as s1)
            // Thus there is only 1 unaliased stream selector maximum.
            if (unaliasedStreams.Count > 1)
            {
                throw new ExprValidationException("A column alias must be supplied for all but one stream if multiple streams are selected via the stream.* notation");
            }
    
            // Resolve underlying event type in the case of wildcard or non-aliased stream select.
            // Determine if the we are considering a tagged event or a stream name.
            if((isUsingWildcard) || (CollectionHelper.IsNotEmpty(unaliasedStreams)))
            {
                if (CollectionHelper.IsNotEmpty(unaliasedStreams))
                {
                    // the tag.* syntax for :  select tag.* from pattern [tag = A]
                    underlyingStreamNumber = unaliasedStreams[0].StreamNumber;
                    if (unaliasedStreams[0].IsTaggedEvent)
                    {
                        TaggedCompositeEventType comp = (TaggedCompositeEventType) typeService.EventTypes[underlyingStreamNumber];
                        Pair<EventType, String> pair = comp.TaggedEventTypes.Get(unaliasedStreams[0].StreamAliasName);
                        if (pair != null)
                        {
                            underlyingEventType = pair.First;
                        }

                        underlyingEventType = comp.TaggedEventTypes.Get(unaliasedStreams[0].StreamAliasName).First;
                        underlyingIsTaggedEvent = true;
                    }
                    // the property.* syntax for :  select property.* from A
                    else if (unaliasedStreams[0].IsProperty)
                    {
                        String propertyName = unaliasedStreams[0].StreamAliasName;
                        Type propertyType = unaliasedStreams[0].PropertyType;
                        int streamNumber = unaliasedStreams[0].StreamNumber;

                        if (TypeHelper.IsBuiltinDataType(unaliasedStreams[0].PropertyType))
                        {
                            throw new ExprValidationException("The property wildcard syntax cannot be used on built-in types as returned by property '" + propertyName + "'");
                        }

                        // create or get an underlying type for that Class
                        underlyingEventType = eventAdapterService.AddBeanType(propertyType.Name, propertyType);
                        underlyingPropertyEventGetter = typeService.EventTypes[streamNumber].GetGetter(propertyName);
                        if (underlyingPropertyEventGetter == null)
                        {
                            throw new ExprValidationException("Unexpected error resolving property getter for property " + propertyName);
                        }
                    }
                    // the stream.* syntax for:  select a.* from A as a
                    else
                    {
                        underlyingEventType = typeService.EventTypes[underlyingStreamNumber];
                    }
                }
                else
                {
                    // no un-aliases stream selectors, but a wildcard was specified
                    if (typeService.EventTypes.Length == 1)
                    {
                        // not a join, we are using the selected event
                        underlyingEventType = typeService.EventTypes[0];
                        if(underlyingEventType is WrapperEventType)
                        {
                            singleStreamWrapper = true;
                        }
                    }
                    else
                    {
                        // For joins, all results are placed in a map with properties for each stream
                        underlyingEventType = null;
                    }
                }
            }
    
            Init(selectionList, aliasedStreams, insertIntoDesc, eventAdapterService, typeService);
        }
    
        private void Init(IList<SelectClauseExprCompiledSpec> selectionList,
                          ICollection<SelectClauseStreamCompiledSpec> aliasedStreams,
                          InsertIntoDesc insertIntoDesc,
                          EventAdapterService eventAdapterService,
                          StreamTypeService typeService)
        {
            // Get expression nodes
            expressionNodes = new ExprNode[selectionList.Count];
            for (int i = 0; i < selectionList.Count; i++)
            {
                expressionNodes[i] = selectionList[i].SelectExpression;
            }
    
            // Get column names
            int count;
            if ((insertIntoDesc != null) && CollectionHelper.IsNotEmpty(insertIntoDesc.ColumnNames))
            {
                columnNames = CollectionHelper.ToArray(insertIntoDesc.ColumnNames);
            }
            else
            {
                int numStreamColumnsJoin = 0;
                if (isUsingWildcard && typeService.EventTypes.Length > 1)
                {
                    numStreamColumnsJoin = typeService.EventTypes.Length;
                }
                columnNames = new String[selectionList.Count + aliasedStreams.Count + numStreamColumnsJoin];
                count = 0;
                foreach (SelectClauseExprCompiledSpec aSelectionList in selectionList)
                {
                    columnNames[count] = aSelectionList.AssignedName;
                    count++;
                }
                foreach (SelectClauseStreamCompiledSpec aSelectionList in aliasedStreams)
                {
                    columnNames[count] = aSelectionList.OptionalAliasName;
                    count++;
                }
                // for wildcard joins, add the streams themselves
                if (isUsingWildcard && typeService.EventTypes.Length > 1)
                {
                    foreach (String streamName in typeService.StreamNames)
                    {
                        columnNames[count] = streamName;
                        count++;
                    }
                }
            }
    
            // Build event type that reflects all selected properties
            Map<String, Object> selPropertyTypes = new HashMap<String, Object>();
            count = 0;
            foreach (ExprNode expressionNode in expressionNodes)
            {
                Type expressionReturnType = expressionNode.ReturnType;
                selPropertyTypes.Put(columnNames[count], expressionReturnType);
                count++;
            }
            foreach (SelectClauseStreamCompiledSpec element in aliasedStreams)
            {
                EventType eventTypeStream = typeService.EventTypes[element.StreamNumber];
                Type expressionReturnType = eventTypeStream.UnderlyingType;
                selPropertyTypes.Put(columnNames[count], expressionReturnType);
                count++;
            }
            if (isUsingWildcard && typeService.EventTypes.Length > 1)
            {
                for (int i = 0; i < typeService.EventTypes.Length; i++)
                {
                    EventType eventTypeStream = typeService.EventTypes[i];
                    Type expressionReturnType = eventTypeStream.UnderlyingType;
                    selPropertyTypes.Put(columnNames[count], expressionReturnType);
                    count++;
                }
            }
    
            // If we have an alias for this type, add it
            if (insertIntoDesc != null)
            {
                try
                {
                    if (underlyingEventType != null)
                    {
                        resultEventType = eventAdapterService.AddWrapperType(insertIntoDesc.EventTypeAlias, underlyingEventType, selPropertyTypes);
                    }
                    else
                    {
                        resultEventType = eventAdapterService.AddNestableMapType(insertIntoDesc.EventTypeAlias, selPropertyTypes);
                    }
                }
                catch (EventAdapterException ex)
                {
                    throw new ExprValidationException(ex.Message);
                }
            }
            else
            {
                if (underlyingEventType != null)
                {
            	    resultEventType = eventAdapterService.CreateAnonymousWrapperType(underlyingEventType, selPropertyTypes);
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
            int count = 0;
            foreach (ExprNode expressionNode in expressionNodes)
            {
                Object evalResult = expressionNode.Evaluate(eventsPerStream, isNewData);
                props.Put(columnNames[count], evalResult);
                count++;
            }
            foreach (SelectClauseStreamCompiledSpec element in aliasedStreams)
            {
                Object value = eventsPerStream[element.StreamNumber].Underlying;
                props.Put(columnNames[count], value);
                count++;
            }
            if (isUsingWildcard && eventsPerStream.Length > 1)
            {
                foreach (EventBean anEventsPerStream in eventsPerStream)
                {
                    Object value = anEventsPerStream.Underlying;
                    props.Put(columnNames[count], value);
                    count++;
                }
            }
    
            if (underlyingEventType != null)
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

                EventBean @event = null;
                if (underlyingIsTaggedEvent)
                {
                    TaggedCompositeEventBean eventBean = (TaggedCompositeEventBean) eventsPerStream[underlyingStreamNumber];
                    @event = eventBean.GetEventBean(unaliasedStreams[0].StreamAliasName);
                }
                else if (underlyingPropertyEventGetter != null)
                {
                    Object value = underlyingPropertyEventGetter.GetValue(eventsPerStream[underlyingStreamNumber]);
                    if (value != null) {
                        @event = eventAdapterService.AdapterForBean(value);
                    }
                }
                else
                {
                    @event = eventsPerStream[underlyingStreamNumber];
                }
    
                // Using a wrapper bean since we cannot use the same event type else same-type filters match.
                // Wrapping it even when not adding properties is very inexpensive.
                return eventAdapterService.CreateWrapper(@event, props, resultEventType);
            }
            else
            {
            	return eventAdapterService.CreateMapFromValues(props, resultEventType);
            }
        }
    
        public EventType ResultEventType
        {
            get { return resultEventType; }
        }
    
        private static void VerifyInsertInto(InsertIntoDesc insertIntoDesc,
                                             ICollection<SelectClauseExprCompiledSpec> selectionList,
                                             ICollection<SelectClauseStreamCompiledSpec> aliasedStreams)
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
            if ( CollectionHelper.IsNotEmpty(insertIntoDesc.ColumnNames) &&
                 (insertIntoDesc.ColumnNames.Count != (selectionList.Count + aliasedStreams.Count)) )
            {
                throw new ExprValidationException("Number of supplied values in the select clause does not match insert-into clause");
            }
        }
    }
}
