using System;
using System.Collections.Generic;

using net.esper.eql.expression;
using net.esper.eql.join.exec;
using net.esper.eql.join.plan;
using net.esper.eql.join.table;
using net.esper.eql.spec;
using net.esper.events;
using net.esper.type;
using net.esper.view;

using org.apache.commons.logging;

namespace net.esper.eql.join
{
	/// <summary>
    /// Factory for building a <seealso cref="JoinSetComposer"/> implementations from analyzing filter nodes, for
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
	    /// <returns>composer implementation</returns>
	    /// <throws>  ExprValidationException is thrown to indicate that </throws>
	    JoinSetComposer MakeComposer(IList<OuterJoinDesc> outerJoinDescList, ExprNode optionalFilterNode,
	                                 EventType[] streamTypes, String[] streamNames, Viewable[] streamViews,
	                                 SelectClauseStreamSelectorEnum selectStreamSelectorEnum);
	}
}
