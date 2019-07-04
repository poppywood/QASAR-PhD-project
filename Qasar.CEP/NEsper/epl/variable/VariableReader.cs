///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using log4net;

namespace com.espertech.esper.epl.variable
{
    /// <summary>
    /// Reads and writes variable values.
    /// <para>
    /// Works closely with <see cref="VariableService"/> in determining the version to read.
    /// </para>
    /// </summary>
	public class VariableReader
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    private readonly String variableName;
	    private readonly int variableNumber;
	    private readonly VariableVersionThreadLocal versionThreadLocal;
	    private VersionedValueList<Object> versionsHigh;
	    private VersionedValueList<Object> versionsLow;
	    private readonly Type type;

	    /// <summary>Ctor.</summary>
	    /// <param name="versionThreadLocal">
	    /// service for returning the threads current version of variable
	    /// </param>
	    /// <param name="type">is the type of the variable returned</param>
	    /// <param name="variableName">variable name</param>
	    /// <param name="variableNumber">number of the variable</param>
	    /// <param name="versions">a list of versioned-values to ask for the version</param>
	    public VariableReader(VariableVersionThreadLocal versionThreadLocal, Type type, String variableName, int variableNumber, VersionedValueList<Object> versions)
	    {
	        this.variableName = variableName;
	        this.variableNumber = variableNumber;
	        this.versionThreadLocal = versionThreadLocal;
	        this.type = type;
	        this.versionsLow = versions;
	        this.versionsHigh = null;
	    }

        /// <summary>
        /// Returns the variable name.
        /// </summary>

        public string VariableName
        {
            get { return variableName; }
        }

        /// <summary>Returns the variable number.</summary>
	    /// <returns>variable index number</returns>
	    public int VariableNumber
	    {
	        get { return variableNumber; }
	    }

	    /// <summary>Returns the type of the variable.</summary>
	    /// <returns>type</returns>
	    public Type VariableType
	    {
            get { return type; }
	    }

	    /// <summary>
	    /// For roll-over (overflow) in version numbers, sets a new collection of versioned-values for the variable
	    /// to use when requests over the version rollover boundary are made.
	    /// </summary>
	    public VersionedValueList<Object> VersionsHigh
	    {
            get { return this.versionsHigh; }
            set { this.versionsHigh = value; }
	    }

	    /// <summary>
	    /// Sets a new list of versioned-values to inquire against, for use when version numbers roll-over.
	    /// </summary>
	    public VersionedValueList<Object> VersionsLow
	    {
            get { return this.versionsLow; }
            set { this.versionsLow = value; }
	    }

        public Object Value
        {
            get { return GetValue(); }
        }

	    /// <summary>
	    /// Returns the value of a variable.
	    /// <para>
	    /// Considers the version set via thread-local for the thread's atomic read of variable values.
	    /// </para>
	    /// </summary>
	    /// <returns>value of variable at the version applicable for the thead</returns>
	    public Object GetValue()
	    {
	        VariableVersionThreadEntry entry = versionThreadLocal.CurrentThread;
	        if (entry.Uncommitted != null)
	        {
	            // Check existance as null values are allowed
	            if (entry.Uncommitted.ContainsKey(variableNumber))
	            {
	                return entry.Uncommitted.Get(variableNumber);
	            }
	        }

	        int myVersion = entry.Version;
	        VersionedValueList<Object> versions = versionsLow;
	        if (myVersion >= VariableServiceImpl.ROLLOVER_READER_BOUNDARY)
	        {
	            if (versionsHigh != null)
	            {
	                versions = versionsHigh;
	            }
	        }
	        return versions.GetVersion(myVersion);
	    }
	}
} // End of namespace
