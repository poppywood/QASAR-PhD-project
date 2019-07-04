///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.epl.expression;
using com.espertech.esper.events;
using com.espertech.esper.view;

namespace com.espertech.esper.epl.view
{
    /// <summary>
    /// Simple filter view filtering events using a filter expression tree.
    /// </summary>

    public class FilterExprView : ViewSupport
    {
        private readonly ExprEvaluator exprEvaluator;

        /// <summary> Ctor.</summary>
        /// <param name="exprEvaluator">Filter expression evaluation impl
        /// </param>

        public FilterExprView(ExprEvaluator exprEvaluator)
        {
            this.exprEvaluator = exprEvaluator;
        }

        /// <summary>
        /// Provides metadata information about the type of object the event collection contains.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// metadata for the objects in the collection
        /// </returns>
        public override EventType EventType
        {
            get { return parent.EventType; }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        private static IEnumerator<EventBean> GetEnumerator(IEnumerator<EventBean> source, ExprEvaluator filter)
        {
            EventBean[] evalEventArr = new EventBean[1];
            while(source.MoveNext())
            {
                EventBean candidate = source.Current;
                evalEventArr[0] = candidate;

                bool? pass = (bool?) filter.Evaluate(evalEventArr, true);
                if (pass ?? false)
                {
                    yield return candidate;
                }
            }    
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public override IEnumerator<EventBean> GetEnumerator()
        {
            return GetEnumerator(parent.GetEnumerator(), exprEvaluator);
        }

        /// <summary>
        /// Notify that data has been added or removed from the Viewable parent.
        /// The last object in the newData array of objects would be the newest object added to the parent view.
        /// The first object of the oldData array of objects would be the oldest object removed from the parent view.
        /// <para>
        /// If the call to update contains new (inserted) data, then the first argument will be a non-empty list and the
        /// second will be empty. Similarly, if the call is a notification of deleted data, then the first argument will be
        /// empty and the second will be non-empty. Either the newData or oldData will be non-null.
        /// This method won't be called with both arguments being null, but either one could be null.
        /// The same is true for zero-length arrays. Either newData or oldData will be non-empty.
        /// If both are non-empty, then the update is a modification notification.
        /// </para>
        /// 	<para>
        /// When update() is called on a view by the parent object, the data in newData will be in the collection of the
        /// parent, and its data structures will be arranged to reflect that.
        /// The data in oldData will not be in the parent's data structures, and any access to the parent will indicate that
        /// that data is no longer there.
        /// </para>
        /// </summary>
        /// <param name="newData">is the new data that has been added to the parent view</param>
        /// <param name="oldData">is the old data that has been removed from the parent view</param>
		
        public override void Update(EventBean[] newData, EventBean[] oldData)
        {
            EventBean[] filteredNewData = FilterEvents(exprEvaluator, newData, true);
            EventBean[] filteredOldData = FilterEvents(exprEvaluator, oldData, false);

            if ((filteredNewData != null) || (filteredOldData != null))
            {
                UpdateChildren(filteredNewData, filteredOldData);
            }
        }

        /// <summary> Filters events using the supplied evaluator.</summary>
        /// <param name="exprEvaluator">evaluator to use</param>
        /// <param name="events">events to filter</param>
		/// <param name="isNewData">true to indicate filter new data (istream) and not old data (rstream)</param>
        /// <returns> filtered events, or null if no events got through the filter 
        /// </returns>

        internal static EventBean[] FilterEvents(ExprEvaluator exprEvaluator, EventBean[] events, bool isNewData)
        {
            if (events == null)
            {
                return null;
            }

            EventBean[] evalEventArr = new EventBean[1];
            bool[] passResult = new bool[events.Length];
            int passCount = 0;

            for (int i = 0; i < events.Length; i++)
            {
                evalEventArr[0] = events[i];
                bool? pass = (bool?)exprEvaluator.Evaluate(evalEventArr, isNewData);
                if (pass ?? false)
                {
                    passResult[i] = true;
                    passCount++;
                }
            }

            if (passCount == 0)
            {
                return null;
            }
            if (passCount == events.Length)
            {
                return events;
            }

            EventBean[] resultArray = new EventBean[passCount];
            int count = 0;
            for (int i = 0; i < passResult.Length; i++)
            {
                if (passResult[i])
                {
                    resultArray[count] = events[i];
                    count++;
                }
            }
            return resultArray;
        }
    }
}
