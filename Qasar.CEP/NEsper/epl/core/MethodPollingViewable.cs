///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.epl.db;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.join;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.view;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Polling-data provider that calls a static method on a class and passed parameters, and wraps the
    /// results as object events.
    /// </summary>
	public class MethodPollingViewable : HistoricalEventViewable
	{
	    private readonly MethodStreamSpec methodStreamSpec;
	    private readonly int myStreamNumber;
	    private readonly PollExecStrategy pollExecStrategy;
	    private readonly IList<ExprNode> inputParameters;
	    private readonly DataCache dataCache;
	    private readonly EventType eventType;

	    private ExprNode[] validatedExprNodes;

	    /// <summary>Ctor.</summary>
	    /// <param name="methodStreamSpec">defines class and method names</param>
	    /// <param name="myStreamNumber">is the stream number</param>
	    /// <param name="inputParameters">the input parameter expressions</param>
	    /// <param name="pollExecStrategy">the execution strategy</param>
	    /// <param name="dataCache">the cache to use</param>
	    /// <param name="eventType">the type of event returned</param>
	    public MethodPollingViewable(
	                           MethodStreamSpec methodStreamSpec,
	                           int myStreamNumber,
	                           IList<ExprNode> inputParameters,
	                           PollExecStrategy pollExecStrategy,
	                           DataCache dataCache,
	                           EventType eventType)
	    {
	        this.methodStreamSpec = methodStreamSpec;
	        this.myStreamNumber = myStreamNumber;
	        this.inputParameters = inputParameters;
	        this.pollExecStrategy = pollExecStrategy;
	        this.dataCache = dataCache;
	        this.eventType = eventType;
	    }

	    public void Stop()
	    {
	        pollExecStrategy.Destroy();
	    }

	    public void Validate(StreamTypeService streamTypeService,
	                         MethodResolutionService methodResolutionService,
	                         TimeProvider timeProvider,
	                         VariableService variableService)
	    {
            Type[] paramTypes = new Type[inputParameters.Count];
	        int count = 0;
	        validatedExprNodes = new ExprNode[inputParameters.Count];

	        foreach (ExprNode exprNode in inputParameters)
	        {
	            ExprNode validated = exprNode.GetValidatedSubtree(streamTypeService, methodResolutionService, null, timeProvider, variableService);
	            validatedExprNodes[count] = validated;
	            paramTypes[count] = validated.ReturnType;
	            count++;
	        }

            // Try to resolve the method, also checking parameter types
            try
			{
				methodResolutionService.ResolveMethod(methodStreamSpec.ClassName, methodStreamSpec.MethodName, paramTypes);
			}
			catch(Exception e)
			{
	            if (inputParameters.Count == 0)
	            {
	                throw new ExprValidationException("Method footprint does not match the number or type of expression parameters, expecting no parameters in method: " + e.Message);
	            }
	            throw new ExprValidationException("Method footprint does not match the number or type of expression parameters, expecting a method where parameters are typed '" +
	                    CollectionHelper.Render(paramTypes) + "': " + e.Message);
			}
	    }

	    public EventTable[] Poll(EventBean[][] lookupEventsPerStream, PollResultIndexingStrategy indexingStrategy)
	    {
	        pollExecStrategy.Start();

	        EventTable[] resultPerInputRow = new EventTable[lookupEventsPerStream.Length];

	        // Get input parameters for each row
	        for (int row = 0; row < lookupEventsPerStream.Length; row++)
	        {
	            Object[] lookupValues = new Object[inputParameters.Count];

	            // Build lookup keys
	            for (int valueNum = 0; valueNum < inputParameters.Count; valueNum++)
	            {
	                Object parameterValue = validatedExprNodes[valueNum].Evaluate(lookupEventsPerStream[row], true);
	                lookupValues[valueNum] = parameterValue;
	            }

	            // Get the result from cache
	            EventTable result = dataCache.GetCached(lookupValues);
	            if (result != null)     // found in cache
	            {
	                resultPerInputRow[row] = result;
	            }
	            else        // not found in cache, get from actual polling (db query)
	            {
	                try
	                {
	                    // Poll using the polling execution strategy and lookup values
	                    IList<EventBean> pollResult = pollExecStrategy.Poll(lookupValues);

	                    // index the result, if required, using an indexing strategy
	                    EventTable indexTable = indexingStrategy.Index(pollResult, dataCache.IsActive);

	                    // assign to row
	                    resultPerInputRow[row] = indexTable;

	                    // save in cache
	                    dataCache.PutCached(lookupValues, indexTable);
	                }
	                catch (EPException)
	                {
	                    pollExecStrategy.Done();
	                    throw;
	                }
	            }
	        }

	        pollExecStrategy.Done();

	        return resultPerInputRow;
	    }

        /// <summary> Add a view to the viewable object.</summary>
        /// <param name="view">to add
        /// </param>
        /// <returns> view to add
        /// </returns>
        public View AddView(View view)
	    {
	        return view;
	    }

        /// <summary> Returns all added views.</summary>
        /// <returns> list of added views
        /// </returns>
        public IList<View> Views
	    {
            get { return new List<View>(); }
	    }

        /// <summary> Remove a view.</summary>
        /// <param name="view">to remove
        /// </param>
        /// <returns> true to indicate that the view to be removed existed within this view, false if the view to
        /// remove could not be found
        /// </returns>
        public bool RemoveView(View view)
	    {
	        throw new UnsupportedOperationException("Subviews not supported");
	    }

        /// <summary> Test is there are any views to the Viewable.</summary>
        /// <returns> true indicating there are child views, false indicating there are no child views
        /// </returns>
        public bool HasViews
	    {
            get { return false; }
	    }

        /// <summary> Provides metadata information about the type of object the event collection contains.</summary>
        /// <returns> metadata for the objects in the collection
        /// </returns>
        public EventType EventType
	    {
            get { return eventType; }
	    }

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        public IEnumerator<EventBean> GetEnumerator()
	    {
	        throw new UnsupportedOperationException("Enumerators not supported");
	    }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new UnsupportedOperationException("Enumerators not supported");
        }

        #endregion
    }
} // End of namespace
