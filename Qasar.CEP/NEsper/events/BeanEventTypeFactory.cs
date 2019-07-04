///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Interface for a factory for obtaining <see cref="BeanEventType"/> instances.
    /// </summary>
	public interface BeanEventTypeFactory
	{
        /// <summary>
        /// Returns the bean event type for a given class assigning the given alias.
        /// </summary>
        /// <param name="alias">is the alias</param>
        /// <param name="type">is the type for which to generate an event type</param>
        /// <returns>is the event type for the class</returns>
	    BeanEventType CreateBeanType(String alias, Type type);

        /// <summary>
        /// Returns the default property resolution style.
        /// </summary>
        /// <value>The default property resolution style.</value>
        /// <returns>property resolution style</returns>
	    PropertyResolutionStyle DefaultPropertyResolutionStyle { get; }
	}
} // End of namespace
