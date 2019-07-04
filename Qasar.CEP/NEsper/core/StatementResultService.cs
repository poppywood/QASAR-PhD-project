///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;

using com.espertech.esper.client;
using com.espertech.esper.collection;
using com.espertech.esper.events;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Interface for a statement-level service for coordinating the insert/remove stream generation,
    /// native deliver to subscribers and the presence/absence of listener or subscribers to a statement.
    /// </summary>
    public interface StatementResultService
    {
        /// <summary>For initialization of the service to provide statement context. </summary>
        /// <param name="epStatement">the statement</param>
        /// <param name="epServiceProvider">the engine instance</param>
        /// <param name="isInsertInto">true if this is insert into</param>
        /// <param name="isPattern">true if this is a pattern statement</param>
        void SetContext(EPStatementSPI epStatement,
                        EPServiceProvider epServiceProvider,
                        bool isInsertInto,
                        bool isPattern);

        /// <summary>For initialize of the service providing select clause column types and names. </summary>
        /// <param name="selectClauseTypes">types of columns in the select clause</param>
        /// <param name="selectClauseColumnNames">column names</param>
        void SetSelectClause(Type[] selectClauseTypes, String[] selectClauseColumnNames);
        
        /// <summary>Returns true to indicate that synthetic events should be produced, for use in select expression processing. </summary>
        /// <returns>true to produce synthetic events</returns>
        bool IsMakeSynthetic { get; }
    
        /// <summary>Returns true to indicate that natural events should be produced, for use in select expression processing. </summary>
        /// <returns>true to produce natural (object[] column) events</returns>
        bool IsMakeNatural { get; }
    
        /// <summary>Dispatch the remaining results, if any, to listeners as the statement is about to be stopped. </summary>
        void DispatchOnStop();
    
        /// <summary>Returns the last iterable event, for use by patterns since these are not iterable. </summary>
        /// <returns>last event</returns>
        EventBean LastIterableEvent { get; }
    
        /// <summary>Indicate a change in update listener. </summary>
        /// <param name="updateListeners">is the new listeners and subscriber</param>
        void SetUpdateListeners(EPStatementListenerSet updateListeners);
    
        /// <summary>Stores for dispatching the statement results. </summary>
        /// <param name="results">is the insert and remove stream data</param>
        void Indicate(UniformPair<EventBean[]> results);
    
        /// <summary>Execution of result indication. </summary>
        void Execute();
    }
}
