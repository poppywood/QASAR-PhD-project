///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;

using com.espertech.esper.events;

namespace com.espertech.esper.core
{
    /// <summary>Query result. </summary>
    public class EPQueryResultImpl : EPQueryResult
    {
        private readonly EPPreparedQueryResult queryResult;
    
        /// <summary>Ctor. </summary>
        /// <param name="queryResult">is the prepared query</param>
        public EPQueryResultImpl(EPPreparedQueryResult queryResult)
        {
            this.queryResult = queryResult;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<EventBean>)queryResult.Result).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<EventBean> GetEnumerator()
        {
            return ((IEnumerable<EventBean>) queryResult.Result).GetEnumerator();
        }

        /// <summary>
        /// Returns an array representing query result rows.
        /// </summary>
        /// <returns>result array</returns>
        public EventBean[] GetArray()
        {
            return queryResult.Result;
        }

        /// <summary>
        /// Returns the event type of the result.
        /// </summary>
        /// <value></value>
        /// <returns>type</returns>
        public EventType EventType
        {
            get { return queryResult.EventType; }
        }
    }
}
