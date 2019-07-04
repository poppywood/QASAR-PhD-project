///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.core;
using com.espertech.esper.events;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Provides prepared query functionality.
    /// </summary>
    public class EPPreparedQueryImpl : EPPreparedQuery
    {
        private readonly EPPreparedExecuteMethod executeMethod;
        private readonly String epl;
    
        /// <summary>Ctor. </summary>
        /// <param name="executeMethod">used at execution time to obtain query results</param>
        /// <param name="epl">is the EPL to execute</param>
        public EPPreparedQueryImpl(EPPreparedExecuteMethod executeMethod, String epl)
        {
            this.executeMethod = executeMethod;
            this.epl = epl;
        }
    
        public EPQueryResult Execute()
        {
            try
            {
                EPPreparedQueryResult result = executeMethod.Execute();
                return new EPQueryResultImpl(result);
            }
            catch (EPStatementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                String message = "Error executing statement: " + ex.Message;
                throw new EPStatementException(message, epl);
            }
        }

        public EventType EventType
        {
            get { return executeMethod.EventType; }
        }
    }
}
