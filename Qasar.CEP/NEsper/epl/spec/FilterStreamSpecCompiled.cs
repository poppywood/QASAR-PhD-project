///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.epl.spec;
using com.espertech.esper.filter;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Specification for building an event stream out of a filter for events (supplying type and basic filter criteria)
	/// and views onto these events which are staggered onto each other to supply a final stream of events.
	/// </summary>
	public class FilterStreamSpecCompiled : StreamSpecBase, StreamSpecCompiled
	{
	    private FilterSpecCompiled filterSpec;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="filterSpec">specifies what events we are interested in.</param>
        /// <param name="viewSpecs">specifies what view to use to derive data</param>
        /// <param name="optionalStreamName">stream name, or null if none supplied</param>
        /// <param name="isUnidirectional">if set to <c>true</c> [is unidirectional].</param>
	    public FilterStreamSpecCompiled(FilterSpecCompiled filterSpec,
	                                    IList<ViewSpec> viewSpecs,
	                                    String optionalStreamName,
	                                    bool isUnidirectional)
	        : base(optionalStreamName, viewSpecs, isUnidirectional)
	    {
	        this.filterSpec = filterSpec;
	    }

	    /// <summary>
	    /// Gets or sets the filter specification for which events the stream will getSelectListEvents.
	    /// </summary>
	    /// <returns>filter spec</returns>
	    public FilterSpecCompiled FilterSpec
	    {
	    	get { return filterSpec; }
            set { filterSpec = value; }
	    }
	}
} // End of namespace
