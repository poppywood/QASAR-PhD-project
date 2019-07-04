///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.epl.agg;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.core
{
	/// <summary>Implementation for engine-level imports.</summary>
	public class EngineImportServiceImpl : EngineImportService
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly List<String> imports;
	    private readonly Map<String, String> aggregationFunctions;
        private readonly Map<String, ConfigurationMethodRef> methodInvocationRef;

		/// <summary>Ctor.</summary>
		public EngineImportServiceImpl()
	    {
	        imports = new List<String>();
	        aggregationFunctions = new HashMap<String, String>();
            methodInvocationRef = new HashMap<String, ConfigurationMethodRef>();
	    }

        public ConfigurationMethodRef GetConfigurationMethodRef(String className)
        {
            return methodInvocationRef.Get(className);
        }

        /// <summary>Adds cache configs for method invocations for from-clause.</summary>
        /// <param name="configs">cache configs</param>
        public void AddMethodRefs(Map<String, ConfigurationMethodRef> configs)
        {
            methodInvocationRef.PutAll(configs);
        }

	    public void AddImport(String importName)
	    {
	        if(!IsTypeNameOrNamespace(importName))
	        {
	            throw new EngineImportException("Invalid import name '" + importName + "'");
	        }

	        imports.Add(importName);
	    }

	    public void AddAggregation(String functionName, String aggregationClass)
	    {
	        String existing = aggregationFunctions.Get(functionName);
	        if (existing != null)
	        {
	            throw new EngineImportException("Aggregation function by name '" + functionName + "' is already defined");
	        }
	        if(!IsFunctionName(functionName))
	        {
	            throw new EngineImportException("Invalid aggregation function name '" + functionName + "'");
	        }
	        if(!IsTypeNameOrNamespace(aggregationClass))
	        {
	            throw new EngineImportException("Invalid class name for aggregation '" + aggregationClass + "'");
	        }
	        aggregationFunctions.Put(functionName.ToLower(), aggregationClass);
	    }

	    public AggregationSupport ResolveAggregation(String name)
	    {
            String className = aggregationFunctions.Get(name);
	        if (className == null)
	        {
                className = aggregationFunctions.Get(name.ToLower());
	        }
	        if (className == null)
	        {
	            throw new EngineImportUndefinedException("Aggregation function named '" + name + "' is not defined");
	        }

	        Type type;

	        try
	        {
	            type = TypeHelper.ResolveType(className);
	        }
	        catch (TypeLoadException ex)
	        {
	            throw new EngineImportException("Could not load aggregation class by name '" + className + "'", ex);
	        }

            if (!typeof(AggregationSupport).IsAssignableFrom(type))
            {
                throw new EngineImportException("Aggregation class by name '" + className + "' does not subclass AggregationSupport");
            }

	        Object obj;
            try
            {
                obj = Activator.CreateInstance(type);
            }
            catch (TypeLoadException e)
            {
                throw new EngineImportException("Error instantiating aggregation class", e);
            }
            catch (MissingMethodException e)
            {
                throw new EngineImportException("Error instantiating aggregation class - Default constructor was not found", e);
            }
            catch (MethodAccessException e)
            {
                throw new EngineImportException("Error instantiating aggregation class - Caller does not have permission to use constructor", e);
            }
            catch (ArgumentException e)
            {
                throw new EngineImportException("Error instantiating aggregation class - Type is not a RuntimeType", e);
            }

	        return (AggregationSupport) obj;
	    }

	    public MethodInfo ResolveMethod(String classNameAlias, String methodName, Type[] paramTypes)
	    {
	        Type type;
	        try
	        {
	            type = ResolveTypeInternal(classNameAlias);
	        }
	        catch (TypeLoadException e)
	        {
	            throw new EngineImportException("Could not load class by name '" + classNameAlias + "' ", e);
	        }

	        try
	        {
                return MethodResolver.ResolveMethod(type, methodName, paramTypes, false);
	        }
	        catch (MissingMethodException e)
	        {
                throw new EngineImportException("Could not find static method named '" + methodName + "' in class '" + classNameAlias + "' ", e);
	        }
	    }

        public MethodInfo ResolveMethod(String classNameAlias, String methodName)
        {
            Type type;
            try
            {
                type = ResolveTypeInternal(classNameAlias);
            }
            catch (TypeLoadException e)
            {
                throw new EngineImportException("Could not load class by name '" + classNameAlias + "' ", e);
            }

            MethodInfo[] methods = type.GetMethods();
            MethodInfo methodByName = null;

            // check each method by name
            foreach (MethodInfo method in methods)
            {
                if (method.Name == methodName)
                {
                    if (methodByName != null)
                    {
                        throw new EngineImportException("Ambiguous method name: method by name '" + methodName +
                                                        "' is overloaded in class '" + classNameAlias + "'");
                    }

                    if (method.IsPublic && method.IsStatic)
                    {
                        methodByName = method;
                    }
                }
            }

            if (methodByName == null)
            {
                throw new EngineImportException("Could not find static method named '" + methodName + "' in class '" +
                                                classNameAlias + "'");
            }
            return methodByName;
        }

	    public Type ResolveType(String classNameAlias)
        {
            Type type;
            try
            {
                type = ResolveTypeInternal(classNameAlias);
            }
            catch (TypeLoadException e)
            {
                throw new EngineImportException("Could not load class by name '" + classNameAlias + "' ", e);
            }

            return type;
        }

	    /// <summary>
	    /// Finds a class by class name using the auto-import information provided.
	    /// </summary>
	    /// <param name="className">is the class name to find</param>
	    /// <returns>class</returns>
	    /// <throws>ClassNotFoundException if the class cannot be loaded</throws>
	    public Type ResolveTypeInternal(String className)
	    {
			// Attempt to retrieve the class with the name as-is
			try
			{
			    return TypeHelper.ResolveType(className);
			}
			catch(TypeLoadException){}

			// Try all the imports
			foreach (String importName in imports)
			{
				// Test as a class name
				if(importName.EndsWith(className))
				{
					Type type = TypeHelper.ResolveType(importName);
                    if ( type != null )
                    {
                        return type;
                    }
                    else if (log.IsDebugEnabled)
                    {
                        log.Debug("Type not found for resolving from name as-is:" + className);
                    }
                }
				else
				{
					// Import is a namespace
					String prefixedClassName = importName + '.' + className;
					try
					{
                        Type type = TypeHelper.ResolveType(prefixedClassName);
                        if ( type != null )
                        {
                            return type;
                        } 
                        else if (log.IsDebugEnabled)
                        {
                            log.Debug("Type not found for resolving from name as-is:" + className);
                        }
					}
					catch(TypeLoadException){}
				}
			}

			// No import worked, the class isn't resolved
			throw new TypeLoadException("Unknown class " + className);
		}

        public MethodInfo ResolveMethod(Type type, String methodName, Type[] paramTypes)
        {
            try
            {
                return MethodResolver.ResolveMethod(type, methodName, paramTypes, true);
            }
            catch (Exception e)
            {
                throw new EngineImportException(
                    "Could not find a method named '" + methodName + "' in class '" + type.Name +
                    "' and matching the required parameter types", e);
            }
        }

	    /// <summary>For testing, returns imports.</summary>
	    /// <returns>returns auto-import list as array</returns>
	    public String[] Imports
		{
	    	get { return imports.ToArray(); }
		}

        private static readonly Regex functionRegEx = new Regex(@"^\w+$", RegexOptions.None);

        private static bool IsFunctionName(String functionName)
	    {
            return functionRegEx.IsMatch(functionName);
	    }

        private static readonly Regex typeNameRegEx = new Regex(@"^(\w+\.)*\w+$", RegexOptions.None);

		private static bool IsTypeNameOrNamespace(String importName)
		{
            return typeNameRegEx.IsMatch(importName);
		}
	}
} // End of namespace
