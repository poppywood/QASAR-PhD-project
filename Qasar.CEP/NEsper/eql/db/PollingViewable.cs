using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.compat;
using net.esper.eql.core;
using net.esper.eql.expression;
using net.esper.events;
using net.esper.view;

namespace net.esper.eql.db
{
    /// <summary>
    /// Implements a poller viewable that uses a polling strategy, a cache and
    /// some input parameters extracted from event streams to perform the polling.
    /// </summary>

    public class PollingViewable : HistoricalEventViewable
    {
        private readonly int myStreamNumber;
        private readonly PollExecStrategy pollExecStrategy;
        private readonly IList<String> inputParameters;
        private readonly DataCache dataCache;
        private readonly EventType eventType;

        private EventPropertyGetter[] getters;
        private int[] getterStreamNumbers;

        /// <summary> Ctor.</summary>
        /// <param name="myStreamNumber">is the stream number of the view
        /// </param>
        /// <param name="inputParameters">are the event property names providing input parameter keys
        /// </param>
        /// <param name="pollExecStrategy">is the strategy to use for retrieving results
        /// </param>
        /// <param name="dataCache">is looked up before using the strategy
        /// </param>
        /// <param name="eventType">is the type of events generated by the view
        /// </param>

        public PollingViewable(
            int myStreamNumber,
            IList<String> inputParameters,
            PollExecStrategy pollExecStrategy,
            DataCache dataCache,
            EventType eventType)
        {
            this.myStreamNumber = myStreamNumber;
            this.inputParameters = inputParameters;
            this.pollExecStrategy = pollExecStrategy;
            this.dataCache = dataCache;
            this.eventType = eventType;
        }

        /// <summary>
        /// Stops the view
        /// </summary>
        public virtual void Stop()
        {
            pollExecStrategy.Destroy();
        }

        /// <summary>
        /// Validate the view.
        /// </summary>
        /// <param name="streamTypeService">supplies the types of streams against which to validate</param>
        /// <throws>  ExprValidationException is thrown to indicate an exception in validating the view </throws>
        public virtual void Validate(StreamTypeService streamTypeService)
        {
            getters = new EventPropertyGetter[inputParameters.Count];
            getterStreamNumbers = new int[inputParameters.Count];

            int count = 0;
            foreach (String inputParam in inputParameters)
            {
                PropertyResolutionDescriptor desc = null;

                // try to resolve the property name alone
                try
                {
                    desc = streamTypeService.ResolveByStreamAndPropName(inputParam);
                }
                catch (StreamTypesException ex)
                {
                    throw new ExprValidationException("Property '" + inputParam + "' failed to resolve, reason: " + ex.Message);
                }

                // hold on to getter and stream number for each stream
                int streamId = desc.StreamNum;
                if (streamId == myStreamNumber)
                {
                    throw new ExprValidationException("Invalid property '" + inputParam + "' resolves to the historical data itself");
                }
                String propName = desc.PropertyName;
                getters[count] = streamTypeService.EventTypes[streamId].GetGetter(propName);
                getterStreamNumbers[count] = streamId;

                count++;
            }
        }

        /// <summary>
        /// Poll for stored historical or reference data using events per stream and
        /// returing for each event-per-stream row a separate list with events
        /// representing the poll result.
        /// </summary>
        /// <param name="lookupEventsPerStream">is the events per stream where the
        /// first dimension is a number of rows (often 1 depending on windows used) and
        /// the second dimension is the number of streams participating in a join.</param>
        /// <returns>
        /// array of lists with one list for each event-per-stream row
        /// </returns>
        public IList<EventBean>[] Poll(EventBean[][] lookupEventsPerStream)
        {
            pollExecStrategy.Start();

            IList<EventBean>[] resultPerInputRow = new List<EventBean>[lookupEventsPerStream.Length];

            // Get input parameters for each row
            for (int row = 0; row < lookupEventsPerStream.Length; row++)
            {
                Object[] lookupValues = new Object[inputParameters.Count];

                // Build lookup keys
                for (int valueNum = 0; valueNum < inputParameters.Count; valueNum++)
                {
                    int streamNum = getterStreamNumbers[valueNum];
                    EventBean streamEvent = lookupEventsPerStream[row][streamNum];
                    Object lookupValue = getters[valueNum].GetValue(streamEvent);
                    lookupValues[valueNum] = lookupValue;
                }

                // Get the result from cache
				IList<EventBean> result = dataCache.GetCached( lookupValues );
                if (result != null)
                // found in cache
                {
                    resultPerInputRow[row] = result;
                }
                // not found in cache, get from actual polling (db query)
                else
                {
                    try
                    {
                        result = pollExecStrategy.Poll(lookupValues);
                        resultPerInputRow[row] = result;
                        dataCache.PutCached(lookupValues, result);
                    }
                    catch (EPException ex)
                    {
                        pollExecStrategy.Done();
                        throw;
                    }
                }
            }

            pollExecStrategy.Done();

            return resultPerInputRow;
        }

        /// <summary>
        /// Add a view to the viewable object.
        /// </summary>
        /// <param name="view">to add</param>
        /// <returns>view to add</returns>
        public virtual View AddView(View view)
        {
            return view;
        }

        /// <summary>
        /// Returns all added views.
        /// </summary>
        /// <returns>list of added views</returns>
        public IList<View> Views
        {
        	get { return new List<View>() ; }
        }

        /// <summary>
        /// Remove a view.
        /// </summary>
        /// <param name="view">to remove</param>
        /// <returns>
        /// true to indicate that the view to be removed existed within this view, false if the view to
        /// remove could not be found
        /// </returns>
        public virtual bool RemoveView(View view)
        {
            throw new NotSupportedException("Subviews not supported");
        }

        /// <summary>
        /// Test is there are any views to the Viewable.
        /// </summary>
        /// <value></value>
        /// <returns> true indicating there are child views, false indicating there are no child views
        /// </returns>
        public virtual bool HasViews
        {
            get { return false; }
        }

        /// <summary>
        /// Provides metadata information about the type of object the event collection contains.
        /// </summary>
        /// <value></value>
        /// <returns> metadata for the objects in the collection
        /// </returns>
        public virtual EventType EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<EventBean> GetEnumerator()
        {
            throw new NotSupportedException("Iterator not supported");
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}