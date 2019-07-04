///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.core;
using com.espertech.esper.compat;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// Holds a unit of dispatch that is a result of a named window processing incoming or timer events.
    /// </summary>
	public class NamedWindowConsumerDispatchUnit
	{
	    private NamedWindowDeltaData deltaData;
	    private readonly Map<EPStatementHandle, IList<NamedWindowConsumerView>> dispatchTo;

	    /// <summary>Ctor.</summary>
	    /// <param name="deltaData">
	    /// the insert and remove stream posted by the named window
	    /// </param>
	    /// <param name="dispatchTo">
	    /// the list of consuming statements, and for each the list of consumer views
	    /// </param>
        public NamedWindowConsumerDispatchUnit(NamedWindowDeltaData deltaData, Map<EPStatementHandle, IList<NamedWindowConsumerView>> dispatchTo)
	    {
	        this.deltaData = deltaData;
	        this.dispatchTo = dispatchTo;
	    }

	    /// <summary>Returns the data to dispatch.</summary>
	    /// <returns>dispatch insert and remove stream events</returns>
        public NamedWindowDeltaData DeltaData
        {
            get { return deltaData; }
        }

	    /// <summary>
	    /// Returns the destination of the dispatch: a map of statements and their consuming views (one or multiple)
	    /// </summary>
	    /// <returns>map of statement to consumer views</returns>
        public Map<EPStatementHandle, IList<NamedWindowConsumerView>> DispatchTo
	    {
            get { return dispatchTo; }
	    }
	}
} // End of namespace
