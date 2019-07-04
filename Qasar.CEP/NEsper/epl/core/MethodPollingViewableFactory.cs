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
using com.espertech.esper.core;
using com.espertech.esper.compat;
using com.espertech.esper.epl.db;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.util;
using com.espertech.esper.view;

using CGLib;

using log4net;

using DataMap = com.espertech.esper.compat.Map<string, object>;
using TypeMap = com.espertech.esper.compat.Map<string, System.Type>;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Factory for method-invocation data provider streams.
    /// </summary>
	public class MethodPollingViewableFactory
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
	    /// Creates a method-invocation polling view for use as a stream that calls a method, or pulls results from cache.
	    /// </summary>
	    /// <param name="streamNumber">the stream number</param>
	    /// <param name="methodStreamSpec">defines the class and method to call</param>
	    /// <param name="eventAdapterService">for creating event types and events</param>
	    /// <param name="epStatementHandle">for time-based callbacks</param>
	    /// <param name="methodResolutionService">for resolving classes and imports</param>
	    /// <param name="engineImportService">for resolving configurations</param>
	    /// <param name="schedulingService">
	    /// for scheduling callbacks in expiry-time based caches
	    /// </param>
	    /// <param name="scheduleBucket">for schedules within the statement</param>
	    /// <returns>pollable view</returns>
	    /// <throws>
	    /// ExprValidationException if the expressions cannot be validated or the method descriptor
	    /// has incorrect class and method names, or parameter number and types don't match
	    /// </throws>
	    public static HistoricalEventViewable CreatePollMethodView(int streamNumber,
	                                                               MethodStreamSpec methodStreamSpec,
	                                                               EventAdapterService eventAdapterService,
	                                                               EPStatementHandle epStatementHandle,
	                                                               MethodResolutionService methodResolutionService,
	                                                               EngineImportService engineImportService,
	                                                               SchedulingService schedulingService,
	                                                               ScheduleBucket scheduleBucket)
	    {
	        // Try to resolve the method
	        FastMethod staticMethod;
	        Type declaringClass;
	        try {
	            MethodInfo method =
	                methodResolutionService.ResolveMethod(methodStreamSpec.ClassName, methodStreamSpec.MethodName);
	            declaringClass = method.DeclaringType;
	            FastClass declaringFastClass = FastClass.Create(method.DeclaringType);
	            staticMethod = declaringFastClass.GetMethod(method);
	        } catch (Exception e) {
	            throw new ExprValidationException(e.Message);
	        }

	        // Determine object type returned by method
	        Type beanClass = staticMethod.ReturnType;
	        if ((beanClass == typeof (void)) ||
	            (TypeHelper.IsBuiltinDataType(beanClass))) {
	            throw new ExprValidationException("Invalid return type for static method '" + staticMethod.Target.Name +
	                                              "' of class '" + methodStreamSpec.ClassName + "', expecting a type");
	        }
	        if (staticMethod.ReturnType.IsArray) {
	            beanClass = staticMethod.ReturnType.GetElementType();
	        }

	        // If the method returns a Map, look up the map type
	        Map<String, Type> mapType = null;
	        String mapTypeName = null;
            if ((TypeHelper.IsImplementsInterface(staticMethod.ReturnType, typeof(DataMap))) ||
                (staticMethod.ReturnType.IsArray && TypeHelper.IsImplementsInterface(staticMethod.ReturnType.GetElementType(), typeof(DataMap))))
            {
	            MethodInfo typeGetterMethod = null;
	            String getterMethodName = methodStreamSpec.MethodName + "Metadata";
	            try {
	                typeGetterMethod =
	                    methodResolutionService.ResolveMethod(methodStreamSpec.ClassName, getterMethodName, new Type[] {});
	            } catch (Exception) {
	                log.Warn("Could not find getter method for Map-typed method invocation, expected a method by name '" +
	                         getterMethodName + "' accepting no parameters");
	            }
	            if ((typeGetterMethod != null) &&
	                (TypeHelper.IsImplementsInterface(typeGetterMethod.ReturnType, typeof(TypeMap)) ))
	            {
	                TypeMap resultType = null;
	                try {
	                    resultType = typeGetterMethod.Invoke(null, null) as TypeMap;
	                } catch (Exception) {
	                    log.Warn("Error invoking getter method for Map-typed method invocation, for method by name '" +
	                             getterMethodName + "' accepting no parameters");
	                }

	                if (resultType != null) {
	                    mapTypeName = methodStreamSpec.ClassName + "." + typeGetterMethod.Name;
	                    mapType = resultType;
	                }
	            }
	        }

	        // Determine event type from class and method name
	        EventType eventType;
	        if (mapType != null) {
	            eventType = eventAdapterService.AddMapType(mapTypeName, mapType);
	        } else {
	            eventType = eventAdapterService.AddBeanType(beanClass.Name, beanClass);
	        }

	        // Construct polling strategy as a method invocation
	        ConfigurationMethodRef configCache = engineImportService.GetConfigurationMethodRef(declaringClass.FullName);
	        if (configCache == null) {
	            configCache = engineImportService.GetConfigurationMethodRef(declaringClass.Name);
	        }

            ConfigurationDataCache dataCacheDesc = (configCache != null) ? configCache.DataCacheDesc : null;
            DataCache dataCache = DataCacheFactory.GetDataCache(dataCacheDesc, epStatementHandle, schedulingService, scheduleBucket);
            PollExecStrategy methodPollStrategy = new MethodPollingExecStrategy(eventAdapterService, staticMethod, mapTypeName != null, eventType);

	        return new MethodPollingViewable(methodStreamSpec,
	                                         streamNumber,
	                                         methodStreamSpec.Expressions,
	                                         methodPollStrategy,
	                                         dataCache,
	                                         eventType);
	    }
	}
} // End of namespace
