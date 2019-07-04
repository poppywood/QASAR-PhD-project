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
    /// <summary>
    /// Specification object representing a complete EPL statement including all EPL constructs.
    /// </summary>
    public class StatementSpecCompiled
    {
        private readonly OnTriggerDesc onTriggerDesc;
        private readonly CreateWindowDesc createWindowDesc;
        private readonly CreateVariableDesc createVariableDesc;
        private readonly InsertIntoDesc insertIntoDesc;
        private SelectClauseStreamSelectorEnum selectStreamDirEnum;
        private readonly SelectClauseSpecCompiled selectClauseSpec;
        private readonly IList<StreamSpecCompiled> streamSpecs;
        private readonly IList<OuterJoinDesc> outerJoinDescList;
        private ExprNode filterExprRootNode;
        private readonly IList<ExprNode> groupByExpressions;
        private readonly ExprNode havingExprRootNode;
        private readonly OutputLimitSpec outputLimitSpec;
        private readonly IList<OrderByItem> orderByList;
        private readonly IList<ExprSubselectNode> subSelectExpressions;
        private readonly bool hasVariables;

        /// <summary>Ctor.</summary>
        /// <param name="insertIntoDesc">insert into def</param>
        /// <param name="selectClauseStreamSelectorEnum">stream selection</param>
        /// <param name="selectClauseSpec">select clause</param>
        /// <param name="streamSpecs">specs for streams</param>
        /// <param name="outerJoinDescList">outer join def</param>
        /// <param name="filterExprRootNode">where filter expr nodes</param>
        /// <param name="groupByExpressions">group by expression</param>
        /// <param name="havingExprRootNode">having expression</param>
        /// <param name="outputLimitSpec">output limit</param>
        /// <param name="orderByList">order by</param>
        /// <param name="subSelectExpressions">list of subqueries</param>
        /// <param name="onTriggerDesc">describes on-delete statements</param>
        /// <param name="createWindowDesc">describes create-window statements</param>
        /// <param name="createVariableDesc">describes create-variable statements</param>
        /// <param name="hasVariables">indicator whether the statement uses variables</param>
        public StatementSpecCompiled(OnTriggerDesc onTriggerDesc,
                                     CreateWindowDesc createWindowDesc,
                                     CreateVariableDesc createVariableDesc,
                                     InsertIntoDesc insertIntoDesc,
                                     SelectClauseStreamSelectorEnum selectClauseStreamSelectorEnum,
                                     SelectClauseSpecCompiled selectClauseSpec,
                                     IList<StreamSpecCompiled> streamSpecs,
                                     IList<OuterJoinDesc> outerJoinDescList,
                                     ExprNode filterExprRootNode,
                                     IList<ExprNode> groupByExpressions,
                                     ExprNode havingExprRootNode,
                                     OutputLimitSpec outputLimitSpec,
                                     IList<OrderByItem> orderByList,
                                     IList<ExprSubselectNode> subSelectExpressions,
                                     bool hasVariables)
        {
            this.onTriggerDesc = onTriggerDesc;
            this.createWindowDesc = createWindowDesc;
            this.createVariableDesc = createVariableDesc;
            this.insertIntoDesc = insertIntoDesc;
            this.selectStreamDirEnum = selectClauseStreamSelectorEnum;
            this.selectClauseSpec = selectClauseSpec;
            this.streamSpecs = streamSpecs;
            this.outerJoinDescList = outerJoinDescList;
            this.filterExprRootNode = filterExprRootNode;
            this.groupByExpressions = groupByExpressions;
            this.havingExprRootNode = havingExprRootNode;
            this.outputLimitSpec = outputLimitSpec;
            this.orderByList = orderByList;
            this.subSelectExpressions = subSelectExpressions;
            this.hasVariables = hasVariables;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public StatementSpecCompiled()
        {
            onTriggerDesc = null;
            createWindowDesc = null;
            createVariableDesc = null;
            insertIntoDesc = null;
            selectStreamDirEnum = SelectClauseStreamSelectorEnum.RSTREAM_ISTREAM_BOTH;
            selectClauseSpec = new SelectClauseSpecCompiled();
            streamSpecs = new List<StreamSpecCompiled>();
            outerJoinDescList = new List<OuterJoinDesc>();
            filterExprRootNode = null;
            groupByExpressions = new List<ExprNode>();
            havingExprRootNode = null;
            outputLimitSpec = null;
            orderByList = new List<OrderByItem>();
            subSelectExpressions = new List<ExprSubselectNode>();
            hasVariables = false;
        }

        /// <summary>Returns the specification for an create-window statement.</summary>
        /// <returns>create-window spec, or null if not such a statement</returns>
        public CreateWindowDesc CreateWindowDesc
        {
            get { return createWindowDesc; }
        }

        /// <summary>Returns the create-variable statement descriptor.</summary>
        /// <returns>create-variable spec</returns>
        public CreateVariableDesc CreateVariableDesc
        {
            get { return createVariableDesc; }
        }

        /// <summary>Returns the FROM-clause stream definitions.</summary>
        /// <returns>list of stream specifications</returns>
        public IList<StreamSpecCompiled> StreamSpecs
        {
            get { return streamSpecs; }
        }

        /// <summary>Returns SELECT-clause list of expressions.</summary>
        /// <returns>list of expressions and optional name</returns>
        public SelectClauseSpecCompiled SelectClauseSpec
        {
            get { return selectClauseSpec; }
        }

        /// <summary>Gets or sets the WHERE-clause root node of filter expression.</summary>
        /// <returns>filter expression root node</returns>
        public ExprNode FilterExprRootNode
        {
            get { return filterExprRootNode; }
            set { filterExprRootNode = value; }
        }

        /// <summary>Gets or sets the WHERE-clause root node of filter expression.</summary>
        /// <returns>filter expression root node</returns>
        public ExprNode FilterRootNode
        {
            get { return filterExprRootNode; }
            set { filterExprRootNode = value; }
        }

        /// <summary>
        /// Returns the LEFT/RIGHT/FULL OUTER JOIN-type and property name descriptor, if applicable. Returns null if regular join.
        /// </summary>
        /// <returns>outer join type, stream names and property names</returns>
        public IList<OuterJoinDesc> OuterJoinDescList
        {
            get { return outerJoinDescList; }
        }

        /// <summary>Returns list of group-by expressions.</summary>
        /// <returns>group-by expression nodes as specified in group-by clause</returns>
        public IList<ExprNode> GroupByExpressions
        {
            get { return groupByExpressions; }
        }

        /// <summary>
        /// Returns expression root node representing the having-clause, if present, or null if no having clause was supplied.
        /// </summary>
        /// <returns>having-clause expression top node</returns>
        public ExprNode HavingExprRootNode
        {
            get { return havingExprRootNode; }
        }

        /// <summary>Returns the output limit definition, if any.</summary>
        /// <returns>output limit spec</returns>
        public OutputLimitSpec OutputLimitSpec
        {
            get { return outputLimitSpec; }
        }

        /// <summary>
        /// Return a descriptor with the insert-into event name and optional list of columns.
        /// </summary>
        /// <returns>insert into specification</returns>
        public InsertIntoDesc InsertIntoDesc
        {
            get { return insertIntoDesc; }
        }

        /// <summary>
        /// Returns the list of order-by expression as specified in the ORDER BY clause.
        /// </summary>
        /// <returns>Returns the orderByList.</returns>
        public IList<OrderByItem> OrderByList
        {
            get { return orderByList; }
        }

        /// <summary>Returns the stream selector (rstream/istream).</summary>
        /// <returns>stream selector</returns>
        public SelectClauseStreamSelectorEnum SelectStreamSelectorEnum
        {
            get { return selectStreamDirEnum; }
        }

        /// <summary>Returns the list of lookup expression nodes.</summary>
        /// <returns>lookup nodes</returns>
        public IList<ExprSubselectNode> SubSelectExpressions
        {
            get { return subSelectExpressions; }
        }

        /// <summary>Returns the specification for an on-delete or on-select statement.</summary>
        /// <returns>on-trigger spec, or null if not such a statement</returns>
        public OnTriggerDesc OnTriggerDesc
        {
            get { return onTriggerDesc; }
        }

        /// <summary>Returns true to indicate the statement has vaiables.</summary>
        /// <returns>true for statements that use variables</returns>
        public bool HasVariables
        {
            get { return hasVariables; }
        }

        /// <summary>
        /// Gets or sets the stream selection.
        /// </summary>
        /// <value>The select stream dir enum.</value>
        public SelectClauseStreamSelectorEnum SelectStreamDirEnum
        {
            get { return this.selectStreamDirEnum; }
            set { this.selectStreamDirEnum = value; }
        }
    }
} // End of namespace
