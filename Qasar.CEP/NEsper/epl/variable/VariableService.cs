///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;

using com.espertech.esper.core;
using com.espertech.esper.compat;

namespace com.espertech.esper.epl.variable
{
    /// <summary>
    /// Variables service for reading and writing variables, and for setting a version number for the current thread to
    /// consider variables for.
    /// <para>
    /// See implementation class for further details.
    /// </para>
    /// </summary>
	public interface VariableService
	{
	    /// <summary>Sets the variable version that subsequent reads consider.</summary>
	    void SetLocalVersion();

	    /// <summary>Lock for use in atomic writes to the variable space.</summary>
	    /// <returns>read write lock for external coordinated write</returns>
	    FastReaderWriterLock ReadWriteLock { get; }

        /// <summary>Creates a new variable.</summary>
        /// <param name="variableName">name of the variable</param>
        /// <param name="type">variable type</param>
        /// <param name="value">
        /// initialization value; String values are allowed and parsed according to type
        /// </param>
        /// <param name="extensionServicesContext">
        /// is extensions for implementing resilience attributes of variables
        /// </param>
        /// <throws>VariableExistsException if the variable name is already in use</throws>
        /// <throws>VariableTypeException if the variable type cannot be recognized</throws>
        void CreateNewVariable(String variableName, Type type, Object value,
                               StatementExtensionSvcContext extensionServicesContext);

	    /// <summary>
	    /// Returns a reader that provides access to variable values. The reader considers the
	    /// version currently set via setLocalVersion.
	    /// </summary>
	    /// <param name="variableName">the variable that the reader should read</param>
	    /// <returns>reader</returns>
	    VariableReader GetReader(String variableName);

	    /// <summary>
	    /// Registers a callback invoked when the variable is written with a new value.
	    /// </summary>
	    /// <param name="variableNumber">the variable index number</param>
	    /// <param name="variableChangeCallback">a callback</param>
	    void RegisterCallback(int variableNumber, VariableChangeCallback variableChangeCallback);

        /// <summary>
        /// Check type of the value supplied and writes the new variable value.
        /// <para/>
        /// Must be followed by either a commit or rollback.
        /// </summary>
        /// <param name="variableNumber">the index number of the variable to write (from VariableReader)</param>
        /// <param name="newValue">the new value</param>
        void CheckAndWrite(int variableNumber, Object newValue);

	    /// <summary>
	    /// Writes a new variable value.
	    /// <para>
	    /// Must be followed by either a commit or rollback.
        /// </para>
	    /// </summary>
	    /// <param name="variableNumber">
	    /// the index number of the variable to write (from VariableReader)
	    /// </param>
	    /// <param name="newValue">the new value</param>
	    void Write(int variableNumber, Object newValue);

	    /// <summary>Commits the variable outstanding changes.</summary>
	    void Commit();

	    /// <summary>Rolls back the variable outstanding changes.</summary>
	    void Rollback();

        /// <summary>Returns a map of variable name and reader, for thread-safe iteration.</summary>
        /// <returns>variable names and readers</returns>
        Map<String, VariableReader> Variables { get; }
	}
} // End of namespace
