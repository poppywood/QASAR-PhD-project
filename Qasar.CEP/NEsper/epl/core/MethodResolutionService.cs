///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

using com.espertech.esper.collection;
using com.espertech.esper.epl.agg;
using com.espertech.esper.type;

namespace com.espertech.esper.epl.core
{
	/// <summary>
	/// Service for resolving methods and aggregation functions, and for creating managing aggregation instances.
	/// </summary>
	public interface MethodResolutionService
	{
        /// <summary>Resolves a given method name and list of parameter types to an instance or static method exposed by the given class.</summary>
        /// <param name="type">is the type to look for a fitting method</param>
        /// <param name="methodName">is the method name</param>
        /// <param name="paramTypes">is parameter types match expression sub-nodes</param>
        /// <returns>method this resolves to</returns>
        /// <throws>EngineImportException if the method cannot be resolved to a visible static or instance method</throws>
        MethodInfo ResolveMethod(Type type, String methodName, Type[] paramTypes);

        /// <summary>Resolves a given class, method and list of parameter types to a static method.</summary>
        /// <param name="typeNameAlias">is the class name to use</param>
        /// <param name="methodName">is the method name</param>
        /// <param name="paramTypes">is parameter types match expression sub-nodes</param>
        /// <returns>method this resolves to</returns>
        /// <throws>EngineImportException if the method cannot be resolved to a visible static method</throws>
        MethodInfo ResolveMethod(String typeNameAlias, String methodName, Type[] paramTypes);

        /// <summary>Resolves a given class and method name to a static method, not allowing overloaded methodsand expecting the method to be found exactly once with zero or more parameters.</summary>
        /// <param name="classNameAlias">is the class name to use</param>
        /// <param name="methodName">is the method name</param>
        /// <returns>method this resolves to</returns>
        /// <throws>EngineImportException if the method cannot be resolved to a visible static method, or if the method exists morethen once with different parameters</throws>
	    MethodInfo ResolveMethod(String classNameAlias, String methodName);

        /// <summary>Resolves a given class name, either fully qualified and simple and imported to a class.</summary>
        /// <param name="classNameAlias">is the class name to use</param>
        /// <returns>class this resolves to</returns>
        /// <throws>EngineImportException if there was an error resolving the class</throws>
	    Type ResolveClass(String classNameAlias);

	    /// <summary>
	    /// Returns a plug-in aggregation method for a given configured aggregation function name.
	    /// </summary>
	    /// <param name="functionName">is the aggregation function name</param>
	    /// <returns>aggregation-providing class</returns>
	    /// <throws>EngineImportUndefinedException is the function name cannot be found</throws>
	    /// <throws>
	    /// EngineImportException if there was an error resolving class information
	    /// </throws>
	    AggregationSupport ResolveAggregation(String functionName);

	    /// <summary>Makes a new plug-in aggregation instance by name.</summary>
	    /// <param name="name">is the plug-in aggregation function name</param>
	    /// <returns>new instance of plug-in aggregation method</returns>
	    AggregationSupport MakePlugInAggregator(String name);

	    /// <summary>Makes a new count-aggregator.</summary>
	    /// <param name="isIgnoreNull">is true to ignore nulls, or false to count nulls</param>
	    /// <returns>aggregator</returns>
	    AggregationMethod MakeCountAggregator(bool isIgnoreNull);

	    /// <summary>Makes a new sum-aggregator.</summary>
	    /// <param name="type">is the type to be summed up, i.e. float, long etc.</param>
	    /// <returns>aggregator</returns>
	    AggregationMethod MakeSumAggregator(Type type);

        /// <summary>
        /// Makes a new distinct-value-aggregator.
        /// </summary>
        /// <param name="aggregationMethod">is the inner aggregation method</param>
        /// <param name="childType">the return type of the inner expression to aggregate, if any</param>
        /// <returns>aggregator</returns>
	    AggregationMethod MakeDistinctAggregator(AggregationMethod aggregationMethod, Type childType);

	    /// <summary>Makes a new avg-aggregator.</summary>
	    /// <returns>aggregator</returns>
	    AggregationMethod MakeAvgAggregator();

	    /// <summary>Makes a new avedev-aggregator.</summary>
	    /// <returns>aggregator</returns>
	    AggregationMethod MakeAvedevAggregator();

	    /// <summary>Makes a new median-aggregator.</summary>
	    /// <returns>aggregator</returns>
	    AggregationMethod MakeMedianAggregator();

	    /// <summary>Makes a new min-max-aggregator.</summary>
	    /// <param name="minMaxType">dedicates whether to do min or max</param>
	    /// <param name="targetType">is the type to max or min</param>
	    /// <returns>aggregator</returns>
	    AggregationMethod MakeMinMaxAggregator(MinMaxTypeEnum minMaxType, Type targetType);

	    /// <summary>Makes a new stddev-aggregator.</summary>
	    /// <returns>aggregator</returns>
	    AggregationMethod MakeStddevAggregator();

	    /// <summary>
	    /// Returns a new set of aggregators given an existing prototype-set of aggregators for a given group key.
	    /// </summary>
	    /// <param name="prototypes">is the prototypes</param>
	    /// <param name="groupKey">is the key to group-by for</param>
	    /// <returns>new set of aggregators for this group</returns>
	    AggregationMethod[] NewAggregators(AggregationMethod[] prototypes, MultiKeyUntyped groupKey);
	}
} // End of namespace
