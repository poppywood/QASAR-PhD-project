///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.collection;
using net.esper.compat;
using net.esper.eql.spec;
using net.esper.util;

using org.apache.commons.logging;

namespace net.esper.view
{
	/// <summary>
	/// Resolves view namespace and name to view factory class, using configuration.
	/// </summary>
    public class ViewResolutionServiceImpl : ViewResolutionService
    {
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PluggableObjectCollection viewObjects;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="viewObjects">is the view objects to use for resolving views, can be both built-in and plug-in views.</param>
        public ViewResolutionServiceImpl(PluggableObjectCollection viewObjects)
        {
            this.viewObjects = viewObjects;
        }

        public ViewFactory Create(String nameSpace, String name)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".Create Creating view factory, namespace=" + nameSpace + " name=" + name);
            }

            Type viewFactoryClass = null;

            EDictionary<String, Pair<Type, PluggableObjectType>> namespaceMap = viewObjects.Pluggables.Get(nameSpace);
            if (namespaceMap != null)
            {
                Pair<Type, PluggableObjectType> pair = namespaceMap.Get(name);
                if (pair != null)
                {
                    if (pair.Second == PluggableObjectType.VIEW)
                    {
                        viewFactoryClass = pair.First;
                    }
                    else
                    {
                        throw new ViewProcessingException("Invalid object type '" + pair.Second + "' for view '" + name + "'");
                    }
                }
            }

            if (viewFactoryClass == null)
            {
                String message = "View name '" + nameSpace + ":" + name + "' is not a known view name";
                throw new ViewProcessingException(message);
            }

            ViewFactory viewFactory;
            try
            {
                viewFactory = (ViewFactory)Activator.CreateInstance(viewFactoryClass);

                if (log.IsDebugEnabled)
                {
                    log.Debug(".create Successfully instantiated view");
                }
            }
            catch (InvalidCastException e)
            {
                String message = "Error casting view factory instance to " + typeof(ViewFactory).FullName + " interface for view '" + name + "'";
                throw new ViewProcessingException(message, e);
            }
            catch (MethodAccessException e)
            {
                String message = "Error invoking view factory constructor for view '" + name;
                message += "', no invocation access for Activator.CreateInstance";
                throw new ViewProcessingException(message, e);
            }
            catch (MissingMethodException e)
            {
                String message = "Error invoking view factory constructor for view '" + name;
                message += "' using Activator.CreateInstance";
                throw new ViewProcessingException(message, e);
            }

            return viewFactory;
        }
    }
} // End of namespace
