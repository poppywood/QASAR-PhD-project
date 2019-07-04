///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.view;

namespace com.espertech.esper.epl.join
{
	/// <summary>
    /// Factory for building a <see cref="JoinSetComposer"/> implementations from analyzing filter nodes, for
	/// fast join tuple result set composition.
	/// </summary>

    public interface JoinSetComposerFactory
	{
        /// <summary>
        /// Builds join tuple composer.
        /// </summary>
        /// <param name="outerJoinDescList">list of descriptors for outer join criteria</param>
        /// <param name="optionalFilterNode">filter tree for analysis to build indexes for fast access</param>
        /// <param name="streamTypes">types of streams</param>
        /// <param name="streamNames">names of streams</param>
        /// <param name="streamViews">leaf view per stream</param>
        /// <param name="selectStreamSelectorEnum">indicator for rstream or istream-only, for optimization</param>
        /// <param name="isUnidirectional">an array of indicators for each stream set to true for a unidirectional stream in a join</param>
        /// <param name="hasChildViews">indicates if child views are declared for a stream</param>
        /// <param name="isNamedWindow">indicates whether the join is against named windows</param>
        /// <returns></returns>
	    JoinSetComposer MakeComposer(IList<OuterJoinDesc> outerJoinDescList,
	                                 ExprNode optionalFilterNode,
	                                 EventType[] streamTypes,
	                                 String[] streamNames,
	                                 Viewable[] streamViews,
	                                 SelectClauseStreamSelectorEnum selectStreamSelectorEnum,
	                                 bool[] isUnidirectional,
	                                 bool[] hasChildViews,
	                                 bool[] isNamedWindow);
	}
}
