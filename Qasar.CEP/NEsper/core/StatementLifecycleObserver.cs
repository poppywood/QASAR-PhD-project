///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.core
{
    /// <summary>Observer statement management events. </summary>
    public interface StatementLifecycleObserver
    {
        /// <summary>Observer statement state changes. </summary>
        /// <param name="event">indicates statement changed</param>
        void Observe(StatementLifecycleEvent @event); 
    }
}
