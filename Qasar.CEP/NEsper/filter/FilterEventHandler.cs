///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using com.espertech.esper.events;

namespace com.espertech.esper.filter
{
    /// <summary>
    /// Indicate that an event was evaluated by the <see cref="com.espertech.esper.filter.FilterService"/>
    /// which matches the filter specification <see cref="com.espertech.esper.filter.FilterSpecCompiled"/>
    /// associated with this callback.
    /// </summary>
    /// <param name="_event">the event received that matches the filter specification</param>

    public delegate void FilterEventHandler(EventBean _event);
}
