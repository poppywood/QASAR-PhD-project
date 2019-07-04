///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using net.esper.collection;
using net.esper.compat;
using net.esper.eql.spec;
using net.esper.pattern;
using net.esper.pattern.guard;
using net.esper.pattern.observer;

using org.apache.commons.logging;

namespace net.esper.pattern
{
	/// <summary>
	/// Resolves pattern object namespace and name to guard or observer factory class, using configuration.
	/// </summary>
	public class PatternObjectResolutionServiceImpl : PatternObjectResolutionService
	{
        private readonly PluggableObjectCollection patternObjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternObjectResolutionServiceImpl"/> class.
        /// </summary>
        /// <param name="patternObjects">the pattern plug-in objects configured</param>

        public PatternObjectResolutionServiceImpl(PluggableObjectCollection patternObjects)
        {
            this.patternObjects = patternObjects;
        }

    public ObserverFactory Create(PatternObserverSpec spec)
    {
        Object result = CreateFactory(spec, PluggableObjectType.PATTERN_OBSERVER);
        ObserverFactory factory;
        try
        {
            factory = (ObserverFactory) result;

            if (log.IsDebugEnabled)
            {
                log.Debug(".create Successfully instantiated observer");
            }
        }
        catch (InvalidCastException e)
        {
            String message = "Error casting observer factory instance to " + typeof (ObserverFactory).FullName +
                             " interface for observer '" + spec.ObjectName + "'";
            throw new PatternObjectException(message, e);
        }
        return factory;
    }

	    public GuardFactory Create(PatternGuardSpec spec)
    {
        Object result = CreateFactory(spec, PluggableObjectType.PATTERN_GUARD);
        GuardFactory factory;
        try
        {
            factory = (GuardFactory) result;

            if (log.IsDebugEnabled)
            {
                log.Debug(".create Successfully instantiated guard");
            }
        }
        catch (InvalidCastException e)
        {
            String message = "Error casting guard factory instance to " + typeof(GuardFactory).FullName + " interface for guard '" + spec.ObjectName + "'";
            throw new PatternObjectException(message, e);
        }
        return factory;
    }

        private Object CreateFactory(ObjectSpec spec, PluggableObjectType type)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".create Creating factory, spec=" + spec);
            }

            // Find the factory class for this pattern object
            Type factoryClass = null;

            EDictionary<String, Pair<Type, PluggableObjectType>> namespaceMap =
                patternObjects.Pluggables.Get(spec.ObjectNamespace);
            if (namespaceMap != null)
            {
                Pair<Type, PluggableObjectType> pair = namespaceMap.Get(spec.ObjectName);
                if (pair != null)
                {
                    if (pair.Second == type)
                    {
                        factoryClass = pair.First;
                    }
                    else
                    {
                        // invalid type: expecting observer, got guard
                        if (type == PluggableObjectType.PATTERN_GUARD)
                        {
                            throw new PatternObjectException("Pattern observer function '" + spec.ObjectName +
                                                             "' cannot be used as a pattern guard");
                        }
                        else
                        {
                            throw new PatternObjectException("Pattern guard function '" + spec.ObjectName +
                                                             "' cannot be used as a pattern observer");
                        }
                    }
                }
            }

            if (factoryClass == null)
            {
                if (type == PluggableObjectType.PATTERN_GUARD)
                {
                    String message = "Pattern guard name '" + spec.ObjectName + "' is not a known pattern object name";
                    throw new PatternObjectException(message);
                }
                else if (type == PluggableObjectType.PATTERN_OBSERVER)
                {
                    String message = "Pattern observer name '" + spec.ObjectName +
                                     "' is not a known pattern object name";
                    throw new PatternObjectException(message);
                }
                else
                {
                    throw new PatternObjectException("Pattern object type '" + type + "' not known");
                }
            }

            Object result;
            try
            {
                result = Activator.CreateInstance(factoryClass);
            }
            catch (MethodAccessException e)
            {
                String message = "Error invoking pattern object factory constructor for object '" + spec.ObjectName;
                message += "', no invocation access for Activator.CreateInstance";
                throw new PatternObjectException(message, e);
            }
            catch (MissingMethodException e)
            {
                String message = "Error invoking pattern object factory constructor for object '" + spec.ObjectName;
                message += "' using Activator.CreateInstance";
                throw new PatternObjectException(message, e);
            }

            return result;
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
}
