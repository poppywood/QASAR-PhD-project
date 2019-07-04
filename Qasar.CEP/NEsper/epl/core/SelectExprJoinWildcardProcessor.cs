using System;

using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Processor for select-clause expressions that handles wildcards. Computes results based on matching events.
    /// </summary>

    public class SelectExprJoinWildcardProcessor : SelectExprProcessor
    {
        /// <summary>
        /// Returns the event type that represents the select-clause items.
        /// </summary>
        /// <value></value>
        /// <returns> event type representing select-clause items
        /// </returns>
        virtual public EventType ResultEventType
        {
            get { return resultEventType; }
        }

        private readonly String[] streamNames;
        private readonly EventType resultEventType;
        private readonly EventAdapterService eventAdapterService;

        /// <summary>Ctor.</summary>
        /// <param name="streamNames">name of each stream</param>
        /// <param name="streamTypes">type of each stream</param>
        /// <param name="eventAdapterService">
        /// service for generating events and handling event types
        /// </param>
        /// <param name="insertIntoDesc">describes the insert-into clause</param>
        /// <throws>ExprValidationException if the expression validation failed</throws>
        public SelectExprJoinWildcardProcessor(String[] streamNames,
                                               EventType[] streamTypes,
                                               EventAdapterService eventAdapterService,
                                               InsertIntoDesc insertIntoDesc)
        {
            if ((streamNames.Length < 2) || (streamTypes.Length < 2) || (streamNames.Length != streamTypes.Length))
            {
                throw new ArgumentException("Stream names and types parameter length is invalid, expected use of this class is for join statements");
            }

            this.streamNames = streamNames;
            this.eventAdapterService = eventAdapterService;

            // Create EventType of result join events
            Map<String, Object> eventTypeMap = new HashMap<String, Object>();
            for (int i = 0; i < streamTypes.Length; i++)
            {
                eventTypeMap[streamNames[i]] = streamTypes[i].UnderlyingType;
            }

	        // If we have an alias for this type, add it
	        if (insertIntoDesc != null)
	        {
	        	try
	            {
                    resultEventType = eventAdapterService.AddNestableMapType(insertIntoDesc.EventTypeAlias, eventTypeMap);
	            }
	            catch (EventAdapterException ex)
	            {
	                throw new ExprValidationException(ex.Message);
	            }
	        }
	        else
	        {
	            resultEventType = eventAdapterService.CreateAnonymousMapType(eventTypeMap);
	        }
        }

        /// <summary>
        /// Computes the select-clause results and returns an event of the result event type that contains, in it's
        /// properties, the selected items.
        /// </summary>
        /// <param name="eventsPerStream">is per stream the event</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <param name="isSynthesize">set to true to indicate that synthetic events are required for an iterator result set</param>
        /// <returns>
        /// event with properties containing selected items
        /// </returns>
        public virtual EventBean Process(EventBean[] eventsPerStream, bool isNewData, bool isSynthesize)
        {
        	DataMap tuple = new HashMap<string,object>() ;
            for (int i = 0; i < streamNames.Length; i++)
            {
                if (streamNames[i] == null)
	            {
	                throw new IllegalStateException("Event name for stream " + i + " is null");
	            }

                if (eventsPerStream[i] != null)
                {
                    tuple[streamNames[i]] = eventsPerStream[i].Underlying;
                }
                else
                {
                    tuple[streamNames[i]] = null;
                }
            }

            return eventAdapterService.CreateMapFromValues(tuple, resultEventType);
        }
    }
}
