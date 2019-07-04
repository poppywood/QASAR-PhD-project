///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.epl.expression;

namespace com.espertech.esper.epl.spec
{
    /// <summary>Specification for use of an existing named window.</summary>
	public class NamedWindowConsumerStreamSpec
        : StreamSpecBase
        , StreamSpecCompiled
	{
	    private readonly String windowName;
	    private readonly IList<ExprNode> filterExpressions;

        /// <summary>Ctor.</summary>
        /// <param name="windowName">specifies the name of the named window</param>
        /// <param name="optionalAsName">an alias or null if none defined</param>
        /// <param name="viewSpecs">is the view specifications</param>
        /// <param name="filterExpressions">the named window filters</param>
        /// <param name="isUnidirectional">true to indicate a unidirectional stream in a join, applicable for joins</param>
        public NamedWindowConsumerStreamSpec(String windowName,
                                             String optionalAsName,
                                             IList<ViewSpec> viewSpecs,
                                             IList<ExprNode> filterExpressions,
                                             bool isUnidirectional)
            : base(optionalAsName, viewSpecs, isUnidirectional)
        {
            this.windowName = windowName;
            this.filterExpressions = filterExpressions;
        }

	    /// <summary>Returns the window name.</summary>
	    /// <returns>window name</returns>
        public String WindowName
        {
            get { return windowName; }
        }

	    /// <summary>
	    /// Returns list of filter expressions onto the named window, or no filter expressions if none defined.
	    /// </summary>
	    /// <returns>list of filter expressions</returns>
        public IList<ExprNode> FilterExpressions
        {
            get { return filterExpressions; }
        }
	}
} // End of namespace
