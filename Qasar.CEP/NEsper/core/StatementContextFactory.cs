///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.compat;
using com.espertech.esper.epl.spec;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Interface for a factory class that makes statement context specific to a statement.
    /// </summary>
	public interface StatementContextFactory
	{
        /// <summary>Create a new statement context consisting of statement-level services.</summary>
        /// <param name="statementId">is the statement is</param>
        /// <param name="statementName">is the statement name</param>
        /// <param name="expression">is the statement expression</param>
        /// <param name="engineServices">is engine services</param>
        /// <param name="optAdditionalContext">addtional context to pass to the statement</param>
        /// <param name="optOnTriggerDesc">the on-delete statement descriptor for named window context creation</param>
        /// <param name="optCreateWindowDesc">the create-window statement descriptor for named window context creation</param>
        /// <param name="hasVariables">indicator whether the statement uses variables anywhere in the statement</param>
        /// <returns>statement context</returns>
        StatementContext MakeContext(String statementId,
                                     String statementName,
                                     String expression,
                                     bool hasVariables,
                                     EPServicesContext engineServices,
                                     Map<String, Object> optAdditionalContext,
                                     OnTriggerDesc optOnTriggerDesc,
                                     CreateWindowDesc optCreateWindowDesc);

	}
} // End of namespace
