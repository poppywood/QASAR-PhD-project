///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;

namespace com.espertech.esper.epl.spec
{
    /// <summary>
    /// Context for mapping a SODA statement to a statement specification, or multiple for subqueries,
    /// and obtaining certain optimization information from a statement.
    /// </summary>
    public class StatementSpecMapContext
    {
        private readonly EngineImportService engineImportService;
        private readonly VariableService variableService;

        private bool hasVariables;

        /// <summary>Ctor.</summary>
        /// <param name="engineImportService">engine imports</param>
        /// <param name="variableService">variable names</param>
        public StatementSpecMapContext(EngineImportService engineImportService, VariableService variableService)
        {
            this.engineImportService = engineImportService;
            this.variableService = variableService;
        }

        /// <summary>Returns the engine import service.</summary>
        /// <returns>service</returns>
        public EngineImportService EngineImportService
        {
            get { return engineImportService; }
        }

        /// <summary>Returns the variable service.</summary>
        /// <returns>service</returns>
        public VariableService VariableService
        {
            get { return variableService; }
        }

        /// <summary>Returns true if a statement has variables.</summary>
        /// <returns>true for variables found</returns>
        public bool HasVariables
        {
            get { return hasVariables; }
            set { hasVariables = value; }
        }
    }
} // End of namespace
