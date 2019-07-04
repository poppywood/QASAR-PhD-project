///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using com.espertech.esper.collection;
using com.espertech.esper.epl.expression;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Specification object representing a complete EPL statement including all EPL constructs.
	/// </summary>
    public class StatementSpecRaw : MetaDefItem
    {
        private OnTriggerDesc onTriggerDesc;
        private CreateWindowDesc createWindowDesc;
        private CreateVariableDesc createVariableDesc;
        private InsertIntoDesc insertIntoDesc;
        private SelectClauseStreamSelectorEnum selectStreamDirEnum;
        private SelectClauseSpecRaw selectClauseSpec = new SelectClauseSpecRaw();
        private readonly List<StreamSpecRaw> streamSpecs = new List<StreamSpecRaw>();
        private readonly List<OuterJoinDesc> outerJoinDescList = new List<OuterJoinDesc>();
        private ExprNode filterExprRootNode;
        private readonly List<ExprNode> groupByExpressions = new List<ExprNode>();
        private ExprNode havingExprRootNode;
        private OutputLimitSpec outputLimitSpec;
        private readonly List<OrderByItem> orderByList = new List<OrderByItem>();
        private bool existsSubstitutionParameters;
        private bool hasVariables;

        /// <summary>Ctor.</summary>
        /// <param name="defaultStreamSelector">stream selection for the statement</param>
        public StatementSpecRaw(SelectClauseStreamSelectorEnum defaultStreamSelector)
        {
            selectStreamDirEnum = defaultStreamSelector;
        }

        /// <summary>Returns the FROM-clause stream definitions.</summary>
        /// <returns>list of stream specifications</returns>
        public IList<StreamSpecRaw> StreamSpecs
        {
            get { return streamSpecs; }
        }

        /// <summary>Gets or sets the SELECT-clause list of expressions.</summary>
        /// <returns>list of expressions and optional name</returns>
        public SelectClauseSpecRaw SelectClauseSpec
        {
            get { return selectClauseSpec; }
            set { selectClauseSpec = value;  }
        }

        /// <summary>
        /// Gets or sets a value indicating if there are one or more substitution parameters
        /// in the statement of contained-within lookup statements
        /// </summary>
        public bool IsExistsSubstitutionParameters
        {
            get { return existsSubstitutionParameters; }
            set { this.existsSubstitutionParameters = value; }
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
        /// Gets or sets the expression root node representing the having-clause, if present,
        /// or null if no having clause was supplied.
        /// </summary>
        /// <returns>having-clause expression top node</returns>
        public ExprNode HavingExprRootNode
        {
            get { return havingExprRootNode; }
            set { havingExprRootNode = value; }
        }

        /// <summary>Gets or sets the output limit definition, if any.</summary>
        /// <returns>output limit spec</returns>
        public OutputLimitSpec OutputLimitSpec
        {
            get { return outputLimitSpec; }
            set { outputLimitSpec = value; }
        }

        /// <summary>
        /// Gets or sets a descriptor with the insert-into event name and optional list of columns.
        /// </summary>
        /// <returns>insert into specification</returns>
        public InsertIntoDesc InsertIntoDesc
        {
            get { return insertIntoDesc; }
            set { insertIntoDesc = value; }
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
        ///
        public SelectClauseStreamSelectorEnum SelectStreamSelectorEnum
        {
            get { return selectStreamDirEnum; }
        }

        /// <summary>Gets or sets the stream selector (rstream/istream/both etc).</summary>
        /// <returns>stream selector</returns>
        public SelectClauseStreamSelectorEnum SelectStreamDirEnum
        {
            get { return selectStreamDirEnum; }
            set { selectStreamDirEnum = value; }
        }

        /// <summary>Returns the create-window specification.</summary>
        /// <returns>descriptor for creating a named window</returns>
        public CreateWindowDesc CreateWindowDesc
        {
            get { return createWindowDesc; }
            set { createWindowDesc = value; }
        }

        /// <summary>Returns the on-delete statement specification.</summary>
        /// <returns>descriptor for creating a an on-delete statement</returns>
        public OnTriggerDesc OnTriggerDesc
        {
            get {return onTriggerDesc;}
            set { onTriggerDesc = value; }
        }

        /// <summary>Gets or sets the where clause.</summary>
        /// <returns>where clause or null if none</returns>
        public ExprNode FilterExprRootNode
        {
            get {return filterExprRootNode;}
            set { filterExprRootNode = value; }
        }

        /// <summary>Returns true if a statement (or subquery sub-statements) use variables.</summary>
        /// <returns>indicator if variables are used</returns>
        public bool HasVariables
        {
            get { return hasVariables; }
            set { hasVariables = value; }
        }

        /// <summary>Gets or sets the descriptor for create-variable statements.</summary>
        /// <returns>create-variable info</returns>
        public CreateVariableDesc CreateVariableDesc
        {
            get { return createVariableDesc; }
            set { createVariableDesc = value; }
        }
    }
} // End of namespace
