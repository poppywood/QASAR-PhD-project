///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

using com.espertech.esper.client;
using com.espertech.esper.epl.agg;

namespace com.espertech.esper.epl.core
{
	/// <summary>
	/// Service for engine-level resolution of static methods and aggregation methods.
	/// </summary>
	public interface EngineImportService
	{
        /// <summary>Returns the method invocation caches for the from-clause for a class.</summary>
        /// <param name="className">the class name providing the method</param>
        /// <returns>cache configs</returns>
        ConfigurationMethodRef GetConfigurationMethodRef(String className);

	    /// <summary>
	    /// Add an import, such as "com.mypackage" or "com.mypackage.MyClass".
	    /// </summary>
	    /// <param name="importName">is the import to add</param>
	    /// <throws>EngineImportException if the information or format is invalid</throws>
	    void AddImport(String importName);

	    /// <summary>Add an aggregation function.</summary>
	    /// <param name="functionName">is the name of the function to make known.</param>
	    /// <param name="aggregationClass">is the class that provides the aggregator</param>
	    /// <throws>EngineImportException throw if format or information is invalid</throws>
	    void AddAggregation(String functionName, String aggregationClass);

	    /// <summary>
	    /// Used at statement compile-time to try and resolve a given function name into an
	    /// aggregation method. Matches function name case-neutral.
	    /// </summary>
	    /// <param name="functionName">is the function name</param>
	    /// <returns>aggregation provider</returns>
	    /// <throws>
	    /// EngineImportUndefinedException if the function is not a configured aggregation function
	    /// </throws>
	    /// <throws>
	    /// EngineImportException if the aggregation providing class could not be loaded or doesn't match
	    /// </throws>
	    AggregationSupport ResolveAggregation(String functionName);

	    /// <summary>
	    /// Resolves a given class, method and list of parameter types to a static method.
	    /// </summary>
        /// <param name="typeNameAlias">is the class name to use</param>
	    /// <param name="methodName">is the method name</param>
	    /// <param name="paramTypes">is parameter types match expression sub-nodes</param>
	    /// <returns>method this resolves to</returns>
	    /// <throws>
	    /// EngineImportException if the method cannot be resolved to a visible static method
	    /// </throws>
        MethodInfo ResolveMethod(String typeNameAlias, String methodName, Type[] paramTypes);

        /// <summary>Resolves a given class name, either fully qualified and simple and imported to a class.</summary>
        /// <param name="typeNameAlias">is the class name to use</param>
        /// <returns>class this resolves to</returns>
        /// <throws>EngineImportException if there was an error resolving the class</throws>
        Type ResolveType(String typeNameAlias);

        /// <summary>Resolves a given class and method name to a static method, expecting the method to existexactly once and not be overloaded, with any parameters.</summary>
        /// <param name="typeNameAlias">is the class name to use</param>
        /// <param name="methodName">is the method name</param>
        /// <returns>method this resolves to</returns>
        /// <throws>EngineImportException if the method cannot be resolved to a visible static method, orif the method is overloaded</throws>
	    MethodInfo ResolveMethod(String typeNameAlias, String methodName);

        /// <summary>Resolves a given method name and list of parameter types to an instance or static method exposed by the given class.</summary>
        /// <param name="type">is the class to look for a fitting method</param>
        /// <param name="methodName">is the method name</param>
        /// <param name="paramTypes">is parameter types match expression sub-nodes</param>
        /// <returns>method this resolves to</returns>
        /// <throws>EngineImportException if the method cannot be resolved to a visible static or instance method</throws>
	    MethodInfo ResolveMethod(Type type, String methodName, Type[] paramTypes);

	}
} // End of namespace
