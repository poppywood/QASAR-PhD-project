///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Statement information for use to perform statement lifecycle management.
    /// </summary>
    public class EPStatementDesc
    {
        private EPStatementSPI epStatement;
        private EPStatementStartMethod startMethod;
        private EPStatementStopMethod stopMethod;
        private String optInsertIntoStream;

        /// <summary>Ctor.</summary>
        /// <param name="epStatement">the statement</param>
        /// <param name="startMethod">the start method</param>
        /// <param name="stopMethod">the stop method</param>
        /// <param name="optInsertIntoStream">
        /// is the insert-into stream name, or null if not using insert-into
        /// </param>
        public EPStatementDesc(EPStatementSPI epStatement, EPStatementStartMethod startMethod, EPStatementStopMethod stopMethod, String optInsertIntoStream)
        {
            this.epStatement = epStatement;
            this.startMethod = startMethod;
            this.stopMethod = stopMethod;
            this.optInsertIntoStream = optInsertIntoStream;
        }

        /// <summary>Returns the statement.</summary>
        /// <returns>statement.</returns>
        public EPStatementSPI EpStatement
        {
            get {return epStatement;}
        }

        /// <summary>Returns the start method.</summary>
        /// <returns>start method</returns>
        public EPStatementStartMethod StartMethod
        {
            get {return startMethod;}
        }

        /// <summary>Returns the stop method.</summary>
        /// <returns>stop method</returns>
        public EPStatementStopMethod StopMethod
        {
            get {return stopMethod;}
            set { this.stopMethod = value; }
        }

        /// <summary>Return the insert-into stream name, or null if no insert-into</summary>
        /// <returns>stream name</returns>
        public String OptInsertIntoStream
        {
            get {return optInsertIntoStream;}
        }
    }
} // End of namespace
