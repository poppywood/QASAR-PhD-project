///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.events;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// A delegate that takes in an eventBean and returns the eventBean
    /// that should be used.  If no eventBean is selected, this method
    /// should return true.  This is primarily used for chaining filters
    /// on an eventBean.
    /// </summary>
    /// <param name="eventBean"></param>
    /// <returns></returns>
    public delegate EventBean BeanEvaluator(EventBean eventBean);
}
