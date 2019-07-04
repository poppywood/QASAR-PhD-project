///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.generated;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.pattern;
using com.espertech.esper.type;

using Antlr.Runtime.Tree;

using log4net;

namespace com.espertech.esper.epl.parse
{
    /// <summary>
    /// Called during the walks of a EPL expression AST tree as specified in the grammar file.
    /// Constructs filter and view specifications etc.
    /// </summary>
    public class EPLTreeWalker : EsperEPL2Ast
    {
        // private holding areas for accumulated info
        private Map<ITree, ExprNode> astExprNodeMap = new HashMap<ITree, ExprNode>();
        private readonly Stack<Map<ITree, ExprNode>> astExprNodeMapStack;
    
        private readonly Map<ITree, EvalNode> astPatternNodeMap = new HashMap<ITree, EvalNode>();
    
        private FilterSpecRaw filterSpec;
        private readonly List<ViewSpec> viewSpecs = new List<ViewSpec>();
    
        // Pattern indicator dictates behavior for some AST nodes
        private bool isProcessingPattern;
    
        // AST Walk result
        private List<ExprSubstitutionNode> substitutionParamNodes = new List<ExprSubstitutionNode>();
        private StatementSpecRaw statementSpec;
        private readonly Stack<StatementSpecRaw> statementSpecStack;
    
        private readonly EngineImportService engineImportService;
        private readonly VariableService variableService;
        private readonly long engineTime;
        private readonly SelectClauseStreamSelectorEnum defaultStreamSelector;
    
        /// <summary>Ctor. </summary>
        /// <param name="engineImportService">is required to resolve lib-calls into static methods or configured aggregation functions</param>
        /// <param name="variableService">for variable access</param>
        /// <param name="input">is the tree nodes to walk</param>
        /// <param name="engineTime">is the current engine time</param>
        /// <param name="defaultStreamSelector">the configuration for which insert or remove streams (or both) to produce</param>
        public EPLTreeWalker(ITreeNodeStream input,
                             EngineImportService engineImportService,
                             VariableService variableService,
                             long engineTime,
                             SelectClauseStreamSelectorEnum defaultStreamSelector)
            : base(input)
        {
            this.engineImportService = engineImportService;
            this.variableService = variableService;
            this.engineTime = engineTime;
            this.defaultStreamSelector = defaultStreamSelector;
    
            statementSpec = new StatementSpecRaw(defaultStreamSelector);
            statementSpecStack = new Stack<StatementSpecRaw>();
            astExprNodeMapStack = new Stack<Map<ITree, ExprNode>>();
        }
    
        /// <summary>Pushes a statement into the stack, creating a new empty statement to fill in. The leave node method for lookup statements pops from the stack. </summary>
        protected override void PushStmtContext() {
            if (log.IsDebugEnabled)
            {
                log.Debug(".PushStmtContext");
            }
            statementSpecStack.Push(statementSpec);
            astExprNodeMapStack.Push(astExprNodeMap);
    
            statementSpec = new StatementSpecRaw(defaultStreamSelector);
            astExprNodeMap = new HashMap<ITree, ExprNode>();
        }

        /// <summary>
        /// Gets the statement specification.
        /// </summary>
        public StatementSpecRaw StatementSpec
        {
            get { return statementSpec; }
        }

        /// <summary>Returns statement specification. </summary>
        /// <returns>statement spec.</returns>
        public StatementSpecRaw GetStatementSpec()
        {
            return statementSpec;
        }
    
        /// <summary>Set to indicate that we are walking a pattern. </summary>
        /// <param name="isPatternWalk">is true if walking a pattern</param>
        protected override void SetIsPatternWalk(bool isPatternWalk)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".SetIsPatternWalk " + isPatternWalk);
            }
            isProcessingPattern = isPatternWalk;
        }
    
        /// <summary>Leave AST node and process it's type and child nodes. </summary>
        /// <param name="node">is the node to complete</param>
        /// <throws>ASTWalkException if the node tree walk operation failed</throws>
        protected override void LeaveNode(ITree node)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".LeaveNode " + node);
            }
    
            switch (node.Type)
            {
                case STREAM_EXPR:
                    LeaveStreamExpr(node);
                    break;
                case EVENT_FILTER_EXPR:
                    LeaveFilter(node);
                    break;
                case PATTERN_INCL_EXPR:
                    return;
                case VIEW_EXPR:
                    LeaveView(node);
                    break;
                case SELECTION_EXPR:
                    LeaveSelectClause(node);
                    break;
                case WILDCARD_SELECT:
                	LeaveWildcardSelect();
                	break;
                case SELECTION_ELEMENT_EXPR:
                    LeaveSelectionElement(node);
                    break;
                case SELECTION_STREAM:
                    LeaveSelectionStream(node);
                    break;
                case EVENT_PROP_EXPR:
                    LeaveEventPropertyExpr(node);
                    break;
                case EVAL_AND_EXPR:
                    LeaveJoinAndExpr(node);
                    break;
                case EVAL_OR_EXPR:
                    LeaveJoinOrExpr(node);
                    break;
                case EVAL_EQUALS_EXPR:
                case EVAL_NOTEQUALS_EXPR:
                    LeaveEqualsExpr(node);
                    break;
                case WHERE_EXPR:
                    LeaveWhereClause();
                    break;
                case NUM_INT:
                case INT_TYPE:
                case LONG_TYPE:
                case BOOL_TYPE:
                case FLOAT_TYPE:
                case DOUBLE_TYPE:
                case STRING_TYPE:
                case NULL_TYPE:
                    LeaveConstant(node);
                    break;
                case SUBSTITUTION:
                    LeaveSubstitution(node);
                    break;
                case STAR:
                case MINUS:
                case PLUS:
                case DIV:
                case MOD:
                    LeaveMath(node);
                    break;
                case BAND:
                case BOR:
                case BXOR:
                	LeaveBitWise(node);
                	break;
                 case LT:
                case GT:
                case LE:
                case GE:
                    LeaveRelationalOp(node);
                    break;
                case COALESCE:
                    LeaveCoalesce(node);
                    break;
                case NOT_EXPR:
                    LeaveNot(node);
                    break;
                case SUM:
                case AVG:
                case COUNT:
                case MEDIAN:
                case STDDEV:
                case AVEDEV:
                    LeaveAggregate(node);
                    break;
                case LIB_FUNCTION:
                	LeaveLibFunction(node);
                	break;
                case LEFT_OUTERJOIN_EXPR:
                case RIGHT_OUTERJOIN_EXPR:
                case FULL_OUTERJOIN_EXPR:
                    LeaveOuterJoin(node);
                    break;
                case GROUP_BY_EXPR:
                    LeaveGroupBy(node);
                    break;
                case HAVING_EXPR:
                    LeaveHavingClause();
                    break;
                case ORDER_BY_EXPR:
                	break;
                case ORDER_ELEMENT_EXPR:
                	LeaveOrderByElement(node);
                	break;
                case EVENT_LIMIT_EXPR:
                case SEC_LIMIT_EXPR:
                case MIN_LIMIT_EXPR:
                case TIMEPERIOD_LIMIT_EXPR:
                	LeaveOutputLimit(node);
                	break;
                case INSERTINTO_EXPR:
                	LeaveInsertInto(node);
                	break;
                case CONCAT:
                	LeaveConcat(node);
                	break;
                case CASE:
                    LeaveCaseNode(node, false);
                    break;
                case CASE2:
                    LeaveCaseNode(node, true);
                    break;
                case EVERY_EXPR:
                    LeaveEvery(node);
                    break;
                case FOLLOWED_BY_EXPR:
                    LeaveFollowedBy(node);
                    break;
                case OR_EXPR:
                    LeaveOr(node);
                    break;
                case AND_EXPR:
                    LeaveAnd(node);
                    break;
                case GUARD_EXPR:
                    LeaveGuard(node);
                    break;
                case OBSERVER_EXPR:
                    LeaveObserver(node);
                    break;
                case IN_SET:
                case NOT_IN_SET:
                    LeaveInSet(node);
                    break;
                case IN_RANGE:
                case NOT_IN_RANGE:
                    LeaveInRange(node);
                    break;
                case BETWEEN:
                case NOT_BETWEEN:
                    LeaveBetween(node);
                    break;
                case LIKE:
                case NOT_LIKE:
                    LeaveLike(node);
                    break;
                case REGEXP:
                case NOT_REGEXP:
                    LeaveRegexp(node);
                    break;
                case PREVIOUS:
                    LeavePrevious(node);
                    break;
                case PRIOR:
                    LeavePrior(node);
                    break;
                case ARRAY_EXPR:
                    LeaveArray(node);
                    break;
                case SUBSELECT_EXPR:
                    LeaveSubselectRow(node);
                    break;
                case EXISTS_SUBSELECT_EXPR:
                    LeaveSubselectExists(node);
                    break;
                case IN_SUBSELECT_EXPR:
                case NOT_IN_SUBSELECT_EXPR:
                    LeaveSubselectIn(node);
                    break;
                case IN_SUBSELECT_QUERY_EXPR:
                    LeaveSubselectQueryIn(node);
                    break;
                case INSTANCEOF:
                    LeaveInstanceOf(node);
                    break;
                case EXISTS:
                    LeaveExists(node);
                    break;
                case CAST:
                    LeaveCast(node);
                    break;
                case CURRENT_TIMESTAMP:
                    LeaveTimestamp(node);
                    break;
                case CREATE_WINDOW_EXPR:
                    LeaveCreateWindow(node);
                    break;
                case CREATE_WINDOW_SELECT_EXPR:
                    LeaveCreateWindowSelect(node);
                    break;
                case CREATE_VARIABLE_EXPR:
                    LeaveCreateVariable(node);
                    break;
                case ON_EXPR:
                    LeaveOnExpr(node);
                    break;
                default:
                    throw new ASTWalkException("Unhandled node type encountered, type '" + node.Type +
                            "' with text '" + node.Text + '\'');
            }
    
            // For each AST child node of this AST node that generated an ExprNode add the child node to the expression node.
            // This is for automatic expression tree building.
            ExprNode thisEvalNode = astExprNodeMap.Get(node);
    
            // Loop over all child nodes for this node.
            for (int i = 0; i < node.ChildCount; i++)
            {
                ITree childNode = node.GetChild(i);
    
                ExprNode childEvalNode = astExprNodeMap.Get(childNode);
                // If there was an expression node generated for the child node, and there is a current expression node,
                // add it to the current expression node (thisEvalNode)
                if ((childEvalNode != null) && (thisEvalNode != null))
                {
                    thisEvalNode.AddChildNode(childEvalNode);
                    astExprNodeMap.Remove(childNode);
                }
            }
    
            // For each AST child node of this AST node that generated an EvalNode add the EvalNode as a child
            EvalNode thisPatternNode = astPatternNodeMap.Get(node);
            for (int i = 0; i < node.ChildCount; i++)
            {
                ITree childNode = node.GetChild(i);
                EvalNode childEvalNode = astPatternNodeMap.Get(childNode);
                if (childEvalNode != null)
                {
                    thisPatternNode.AddChildNode(childEvalNode);
                    astPatternNodeMap.Remove(childNode);
                }
            }
        }
    
        private void LeaveCreateWindow(ITree node)
        {
            log.Debug(".leaveCreateWindow");
    
            String windowName = node.GetChild(0).Text;
    
            String eventName = null;
            for (int i = 0; i < node.ChildCount; i++)
            {
                ITree child = node.GetChild(i);
                if (child.Type == CLASS_IDENT) // the event type
                {
                    eventName = child.Text;
                }
            }
            if (eventName == null)
            {
                throw new ASTWalkException("Event type AST not found");
            }
    
            CreateWindowDesc desc = new CreateWindowDesc(windowName, viewSpecs);
            statementSpec.CreateWindowDesc = desc;
    
            FilterSpecRaw rawFilterSpec = new FilterSpecRaw(eventName, new List<ExprNode>());
            FilterStreamSpecRaw streamSpec = new FilterStreamSpecRaw(rawFilterSpec, new List<ViewSpec>(), null, false);
            statementSpec.StreamSpecs.Add(streamSpec);
        }
    
        private void LeaveCreateVariable(ITree node)
        {
            log.Debug(".leaveCreateVariable");

            ITree child = node.GetChild(0);
            String variableType = child.Text;
            child = node.GetChild(1);
            String variableName = child.Text;
    
            ExprNode assignment = null;
            if (node.ChildCount > 2)
            {
                child = node.GetChild(2);
                assignment = astExprNodeMap.Get(child);
                astExprNodeMap.Remove(child);
            }
    
            CreateVariableDesc desc = new CreateVariableDesc(variableType, variableName, assignment);
            statementSpec.CreateVariableDesc = desc;
        }
    
        private void LeaveCreateWindowSelect(ITree node)
        {
            log.Debug(".leaveCreateWindowSelect");
        }
    
        private void LeaveOnExpr(ITree node)
        {
            log.Debug(".leaveOnExpr");
    
            // determine on-delete or on-select
            bool isOnDelete = false;
            ITree typeChildNode = null;
            ITree childNode;
            for (int i = 0; i < node.ChildCount; i++)
            {
                childNode = node.GetChild(i);
    
                if (childNode.Type == ON_DELETE_EXPR)
                {
                    typeChildNode = childNode;
                    isOnDelete = true;
                    break;
                }
                if (childNode.Type == ON_SELECT_EXPR)
                {
                    typeChildNode = childNode;
                    break;
                }
                if (childNode.Type == ON_SET_EXPR)
                {
                    typeChildNode = childNode;
                    break;
                }
            }
            if (typeChildNode == null)
            {
                throw new IllegalStateException("Could not determine on-expr type");
            }
    
            // get optional filter stream as-name
            childNode = node.GetChild(1);
            String streamAsName = null;
            if (childNode.Type == IDENT)
            {
                streamAsName = childNode.Text;
            }
    
            // get stream to use (pattern or filter)
            StreamSpecRaw streamSpec;
            if (node.GetChild(0).Type == EVENT_FILTER_EXPR)
            {
                streamSpec = new FilterStreamSpecRaw(filterSpec, new List<ViewSpec>(), streamAsName, false);
            }
            else if (node.GetChild(0).Type == PATTERN_INCL_EXPR)
            {
                if ((astPatternNodeMap.Count > 1) || ((astPatternNodeMap.Count == 0)))
                {
                    throw new ASTWalkException("Unexpected AST tree contains zero or more then 1 child elements for root");
                }
                // Get expression node sub-tree from the AST nodes placed so far
                EvalNode evalNode = CollectionHelper.First(astPatternNodeMap.Values);
                streamSpec = new PatternStreamSpecRaw(evalNode, viewSpecs, streamAsName, false);
                astPatternNodeMap.Clear();
            }
            else
            {
                throw new IllegalStateException("Invalid AST type node, cannot map to stream specification");
            }
    
            if (typeChildNode.Type != ON_SET_EXPR)
            {
                // The ON_EXPR_FROM contains the window name
                UniformPair<String> windowName = GetWindowName(typeChildNode);
                statementSpec.OnTriggerDesc = new OnTriggerWindowDesc(windowName.First, windowName.Second, isOnDelete);
            }
            else
            {
                OnTriggerSetDesc setDesc = GetOnTriggerSet(typeChildNode);
                statementSpec.OnTriggerDesc = setDesc;
            }
            statementSpec.StreamSpecs.Add(streamSpec);
        }
    
        private OnTriggerSetDesc GetOnTriggerSet(ITree typeChildNode)
        {
            OnTriggerSetDesc desc = new OnTriggerSetDesc();
    
            int count = 0;
            ITree child = typeChildNode.GetChild(count);
            do
            {
                // get variable name
                if (child.Type != IDENT)
                {
                    throw new IllegalStateException("Expected identifier but received type '" + child.Type + "'");
                }
                String variableName = child.Text;
    
                // get expression
                child = typeChildNode.GetChild(++count);
                ExprNode childEvalNode = astExprNodeMap.Get(child);
                astExprNodeMap.Remove(child);
    
                desc.AddAssignment(new OnTriggerSetAssignment(variableName, childEvalNode));
                child = typeChildNode.GetChild(++count);
            }
            while (count < typeChildNode.ChildCount);
    
            return desc;
        }
    
        private static UniformPair<String> GetWindowName(ITree typeChildNode)
        {
            String windowName = null;
            String windowStreamName = null;
    
            for (int i = 0; i < typeChildNode.ChildCount; i++)
            {
            	ITree child = typeChildNode.GetChild(i);
                if (child.Type == ON_EXPR_FROM)
                {
                    windowName = child.GetChild(0).Text;
                    if (child.ChildCount > 1)
                    {
                        windowStreamName = child.GetChild(1).Text;
                    }
                    break;
                }
            }
            if (windowName == null)
            {
                throw new IllegalStateException("Could not determine on-expr from-clause named window name");
            }
            return new UniformPair<String>(windowName, windowStreamName);
        }
    
    
        private void LeavePrevious(ITree node)
        {
            log.Debug(".leavePrevious");
    
            ExprPreviousNode previousNode = new ExprPreviousNode();
            astExprNodeMap.Put(node, previousNode);
        }
    
        private void LeavePrior(ITree node)
        {
            log.Debug(".leavePrior");
    
            ExprPriorNode priorNode = new ExprPriorNode();
            astExprNodeMap.Put(node, priorNode);
        }
    
        private void LeaveInstanceOf(ITree node)
        {
            log.Debug(".leaveInstanceOf");
    
            // get class identifiers
            List<String> classes = new List<String>();
            for (int i = 1; i < node.ChildCount; i++)
            {
                ITree classIdent = node.GetChild(i);
                classes.Add(classIdent.Text);
            }

            String[] idents = classes.ToArray();
            ExprInstanceofNode isNode = new ExprInstanceofNode(idents);
            astExprNodeMap.Put(node, isNode);
        }
    
        private void LeaveExists(ITree node)
        {
            log.Debug(".leaveExists");
    
            ExprPropertyExistsNode isNode = new ExprPropertyExistsNode();
            astExprNodeMap.Put(node, isNode);
        }
    
        private void LeaveCast(ITree node)
        {
            log.Debug(".leaveCast");
    
            String classIdent = node.GetChild(1).Text;
            ExprCastNode castNode = new ExprCastNode(classIdent);
            astExprNodeMap.Put(node, castNode);
        }
    
        private void LeaveTimestamp(ITree node)
        {
            log.Debug(".leaveTimestamp");
    
            ExprTimestampNode timeNode = new ExprTimestampNode();
            astExprNodeMap.Put(node, timeNode);
        }
    
        private void LeaveArray(ITree node)
        {
            log.Debug(".leaveArray");
    
            ExprArrayNode arrayNode = new ExprArrayNode();
            astExprNodeMap.Put(node, arrayNode);
        }
    
        private void LeaveSubselectRow(ITree node)
        {
            log.Debug(".leaveSubselectRow");
    
            StatementSpecRaw currentSpec = PopStacks();
            ExprSubselectRowNode subselectNode = new ExprSubselectRowNode(currentSpec);
            astExprNodeMap.Put(node, subselectNode);
        }
    
        private void LeaveSubselectExists(ITree node)
        {
            log.Debug(".leaveSubselectExists");
    
            StatementSpecRaw currentSpec = PopStacks();
            ExprSubselectNode subselectNode = new ExprSubselectExistsNode(currentSpec);
            astExprNodeMap.Put(node, subselectNode);
        }
    
        private void LeaveSubselectIn(ITree node)
        {
            log.Debug(".leaveSubselectIn");
    
            ITree nodeSubquery = node.GetChild(1);
    
            bool isNot = false;
            if (node.Type == NOT_IN_SUBSELECT_EXPR)
            {
                isNot = true;
            }
    
            ExprSubselectInNode subqueryNode = (ExprSubselectInNode) astExprNodeMap.RemoveAndReturn(nodeSubquery);
            subqueryNode.SetNotIn(isNot);
    
            astExprNodeMap.Put(node, subqueryNode);
        }
    
        private void LeaveSubselectQueryIn(ITree node)
        {
            log.Debug(".leaveSubselectQueryIn");
    
            StatementSpecRaw currentSpec = PopStacks();
            ExprSubselectNode subselectNode = new ExprSubselectInNode(currentSpec);
            astExprNodeMap.Put(node, subselectNode);
        }
    
        private StatementSpecRaw PopStacks()
        {
            log.Debug(".popStacks");
    
            StatementSpecRaw currentSpec = statementSpec;
            statementSpec = statementSpecStack.Pop();
    
            if (currentSpec.HasVariables)
            {
                statementSpec.HasVariables = true;
            }
    
            astExprNodeMap = astExprNodeMapStack.Pop();
    
            return currentSpec;
        }
    
        /// <summary>End processing of the AST tree for stand-alone pattern expressions. </summary>
        /// <throws>ASTWalkException is the walk failed</throws>
        protected override void EndPattern()
        {
            log.Debug(".EndPattern");

            if ((astPatternNodeMap.Count > 1) || ((astPatternNodeMap.Count == 0)))
            {
                throw new ASTWalkException("Unexpected AST tree contains zero or more then 1 child elements for root");
            }
    
            // Get expression node sub-tree from the AST nodes placed so far
            EvalNode evalNode = CollectionHelper.First(astPatternNodeMap.Values);
    
            PatternStreamSpecRaw streamSpec = new PatternStreamSpecRaw(evalNode, new List<ViewSpec>(), null, false);
            statementSpec.StreamSpecs.Add(streamSpec);
            statementSpec.IsExistsSubstitutionParameters = substitutionParamNodes.Count > 0;
    
            astPatternNodeMap.Clear();
        }
    
        /// <summary>End processing of the AST tree, check that expression nodes found their homes. </summary>
        /// <throws>ASTWalkException is the walk failed</throws>
        protected override void End()
        {
            log.Debug(".end");
    
            if (astExprNodeMap.Count > 1)
            {
                throw new ASTWalkException("Unexpected AST tree contains left over child elements," +
                        " not all expression nodes have been removed from AST-to-expression nodes map");
            }
            if (astPatternNodeMap.Count > 1)
            {
                throw new ASTWalkException("Unexpected AST tree contains left over child elements," +
                        " not all pattern nodes have been removed from AST-to-pattern nodes map");
            }
    
            statementSpec.IsExistsSubstitutionParameters = substitutionParamNodes.Count > 0;
        }
    
        private void LeaveSelectionElement(ITree node)
        {
            log.Debug(".leaveSelectionElement");

            if ((astExprNodeMap.Count > 1) || ((astExprNodeMap.Count == 0)))
            {
                throw new ASTWalkException("Unexpected AST tree contains zero or more then 1 child element for root");
            }
    
            // Get expression node sub-tree from the AST nodes placed so far
            ExprNode exprNode = CollectionHelper.First(astExprNodeMap.Values);
            astExprNodeMap.Clear();
    
            // Get list element name
            String optionalName = null;
            if (node.ChildCount > 1)
            {
                optionalName = node.GetChild(1).Text;
            }
    
            // Add as selection element
            statementSpec.SelectClauseSpec.Add(new SelectClauseExprRawSpec(exprNode, optionalName));
        }
    
        private void LeaveSelectionStream(ITree node)
        {
            log.Debug(".leaveSelectionStream");
    
            String streamName = node.GetChild(0).Text;
    
            // Get alias element name
            String optionalName = null;
            if (node.ChildCount > 1)
            {
                optionalName = node.GetChild(1).Text;
            }
    
            // Add as selection element
            statementSpec.SelectClauseSpec.Add(new SelectClauseStreamRawSpec(streamName, optionalName));
        }
    
        private void LeaveWildcardSelect()
        {
        	log.Debug(".leaveWildcardSelect");
            statementSpec.SelectClauseSpec.Add(new SelectClauseElementWildcard());
        }
    
        private void LeaveView(ITree node)
        {
            log.Debug(".leaveView");
            String objectNamespace = node.GetChild(0).Text;
            String objectName = node.GetChild(1).Text;
    
            List<Object> objectParams = new List<Object>();
    
            for (int i = 2; i < node.ChildCount; i++)
            {
                ITree child = node.GetChild(i);
    
                // if there is an expression for this parameter, add the expression to the parameter list
                if (this.astExprNodeMap.ContainsKey(child))
                {
                    ExprNode expr = astExprNodeMap.Get(child);
                    if (expr is ExprIdentNode)
                    {
                        ExprIdentNode property = (ExprIdentNode) expr;
                        objectParams.Add(property.FullUnresolvedName);
                    }
                    else
                    {
                        objectParams.Add(expr);
                    }
                    astExprNodeMap.Remove(child);
                }
                else
                {
                    Object @object = ASTParameterHelper.MakeParameter(child, engineTime);
                    objectParams.Add(@object);
                }
            }
    
            viewSpecs.Add(new ViewSpec(objectNamespace, objectName, objectParams));
        }
    
        private void LeaveStreamExpr(ITree node)
        {
            log.Debug(".leaveStreamExpr");
    
            // Determine the optional stream name
            // Search for identifier node that carries the stream name in an "from Class.win:time().std:doit() as StreamName"
            ITree streamNameNode = null;
            for (int i = 1; i < node.ChildCount; i++)
            {
                ITree child = node.GetChild(i);
                if (child.Type == IDENT)
                {
                    streamNameNode = child;
                    break;
                }
            }
            String streamName = null;
            if (streamNameNode != null)
            {
                streamName = streamNameNode.Text;
            }
    
            // The first child node may be a "stream" keyword
            bool isUnidirectional = false;
            for (int i = 0; i < node.ChildCount; i++)
            {
                if (node.GetChild(i).Type == UNIDIRECTIONAL)
                {
                    isUnidirectional = true;
                    break;
                }
            }
    
            // Convert to a stream specification instance
            StreamSpecRaw streamSpec;
    
            // If the first subnode is a filter node, we have a filter stream specification
            if (node.GetChild(0).Type == EVENT_FILTER_EXPR)
            {
                streamSpec = new FilterStreamSpecRaw(filterSpec, viewSpecs, streamName, isUnidirectional);
            }
            else if (node.GetChild(0).Type == PATTERN_INCL_EXPR)
            {
                if ((astPatternNodeMap.Count > 1) || ((astPatternNodeMap.Count == 0)))
                {
                    throw new ASTWalkException("Unexpected AST tree contains zero or more then 1 child elements for root");
                }
    
                // Get expression node sub-tree from the AST nodes placed so far
                EvalNode evalNode = CollectionHelper.First(astPatternNodeMap.Values);
    
                streamSpec = new PatternStreamSpecRaw(evalNode, viewSpecs, streamName, isUnidirectional);
                astPatternNodeMap.Clear();
            }
            else if (node.GetChild(0).Type == DATABASE_JOIN_EXPR)
            {
                ITree dbrootNode = node.GetChild(0);
                String dbName = dbrootNode.GetChild(0).Text;
                String sqlWithParams = StringValue.ParseString(dbrootNode.GetChild(1).Text.Trim());
    
                String sampleSQL = null;
                if (dbrootNode.ChildCount > 2)
                {
                    sampleSQL = dbrootNode.GetChild(2).Text;
                    sampleSQL = StringValue.ParseString(sampleSQL.Trim());
                }
    
                streamSpec = new DBStatementStreamSpec(streamName, viewSpecs, dbName, sqlWithParams, sampleSQL);
            }
            else if (node.GetChild(0).Type == METHOD_JOIN_EXPR)
            {
                ITree methodRootNode = node.GetChild(0);
                String prefixIdent = methodRootNode.GetChild(0).Text;
                String className = methodRootNode.GetChild(1).Text;
    
                int indexDot = className.LastIndexOf('.');
                String classNamePart;
                String methodNamePart;
                if (indexDot == -1)
                {
                    classNamePart = className;
                    methodNamePart = null;
                }
                else
                {
                    classNamePart = className.Substring(0, indexDot);
                    methodNamePart = className.Substring(indexDot + 1);
                }
                List<ExprNode> exprNodes = GetExprNodes(methodRootNode, 2);
    
                streamSpec = new MethodStreamSpec(streamName, viewSpecs, prefixIdent, classNamePart, methodNamePart, exprNodes);
            }
            else
            {
                throw new ASTWalkException("Unexpected AST child node to stream expression, type=" + node.GetChild(0).Type);
            }
            viewSpecs.Clear();
            statementSpec.StreamSpecs.Add(streamSpec);
        }
    
        private void LeaveEventPropertyExpr(ITree node)
        {
            log.Debug(".leaveEventPropertyExpr");
    
            if (node.ChildCount == 0)
            {
                throw new IllegalStateException("Empty event property expression encountered");
            }
    
            ExprNode exprNode;
            String propertyName;
    
            // The stream name may precede the event property name, but cannot be told apart from the property name:
            //      s0.p1 could be a nested property, or could be stream 's0' and property 'p1'
    
            // A single entry means this must be the property name.
            // And a non-simple property means that it cannot be a stream name.
            if ((node.ChildCount == 1) || (node.GetChild(0).Type != EVENT_PROP_SIMPLE))
            {
                propertyName = ASTFilterSpecHelper.GetPropertyName(node, 0);
                exprNode = new ExprIdentNode(propertyName);
            }
            // --> this is more then one child node, and the first child node is a simple property
            // we may have a stream name in the first simple property, or a nested property
            // i.e. 's0.p0' could mean that the event has a nested property to 's0' of name 'p0', or 's0' is the stream name
            else
            {
                String leadingIdentifier = node.GetChild(0).GetChild(0).Text;
                String streamOrNestedPropertyName = ASTFilterSpecHelper.EscapeDot(leadingIdentifier);
                propertyName = ASTFilterSpecHelper.GetPropertyName(node, 1);
                exprNode = new ExprIdentNode(propertyName, streamOrNestedPropertyName);
            }
    
            if (variableService.GetReader(propertyName) != null)
            {
                exprNode = new ExprVariableNode(propertyName);
                statementSpec.HasVariables = true;
            }
    
            astExprNodeMap.Put(node, exprNode);
        }
    
        private void LeaveLibFunction(ITree node)
        {
        	log.Debug(".leaveLibFunction");
    
            String childNodeText = node.GetChild(0).Text;
            if ((childNodeText.Equals("max")) || (childNodeText.Equals("min")))
            {
                HandleMinMax(node);
                return;
            }
    
            if (node.GetChild(0).Type == CLASS_IDENT)
            {
                String className = node.GetChild(0).Text;
                String methodName = node.GetChild(1).Text;
                astExprNodeMap.Put(node, new ExprStaticMethodNode(className, methodName));
                return;
            }
    
            try
            {
                AggregationSupport aggregation = engineImportService.ResolveAggregation(childNodeText);
    
                bool isDistinct = false;
                if ((node.GetChild(1) != null) && (node.GetChild(1).Type == DISTINCT))
                {
                    isDistinct = true;
                }
    
                astExprNodeMap.Put(node, new ExprPlugInAggFunctionNode(isDistinct, aggregation, childNodeText));
                return;
            }
            catch (EngineImportUndefinedException)
            {
                // Not an aggretaion function
            }
            catch (EngineImportException e)
            {
                throw new IllegalStateException("Error resolving aggregation: " + e.Message, e);
            }
    
            throw new IllegalStateException("Unknown method named '" + childNodeText + "' could not be resolved");
        }
    
        private void LeaveEqualsExpr(ITree node)
        {
            log.Debug(".leaveEqualsExpr");
    
            bool isNot = false;
            if (node.Type == EVAL_NOTEQUALS_EXPR)
            {
                isNot = true;
            }
    
            ExprEqualsNode identNode = new ExprEqualsNode(isNot);
            astExprNodeMap.Put(node, identNode);
        }
    
        private void LeaveJoinAndExpr(ITree node)
        {
            log.Debug(".leaveJoinAndExpr");
            ExprAndNode identNode = new ExprAndNode();
            astExprNodeMap.Put(node, identNode);
        }
    
        private void LeaveJoinOrExpr(ITree node)
        {
            log.Debug(".leaveJoinOrExpr");
            ExprOrNode identNode = new ExprOrNode();
            astExprNodeMap.Put(node, identNode);
        }
    
        private void LeaveConstant(ITree node)
        {
            log.Debug(".leaveConstant");
            ExprConstantNode constantNode = new ExprConstantNode(ASTConstantHelper.Parse(node));
            astExprNodeMap.Put(node, constantNode);
        }
    
        private void LeaveSubstitution(ITree node)
        {
            log.Debug(".leaveSubstitution");
    
            // Add the substitution parameter node, for later replacement
            int currentSize = this.substitutionParamNodes.Count;
            ExprSubstitutionNode substitutionNode = new ExprSubstitutionNode(currentSize + 1);
            substitutionParamNodes.Add(substitutionNode);
    
            astExprNodeMap.Put(node, substitutionNode);
        }
    
        private void LeaveMath(ITree node)
        {
            log.Debug(".leaveMath");
    
            MathArithTypeEnum mathArithTypeEnum;
    
            switch (node.Type)
            {
                case DIV :
                    mathArithTypeEnum = MathArithTypeEnum.DIVIDE;
                    break;
                case STAR :
                    mathArithTypeEnum = MathArithTypeEnum.MULTIPLY;
                    break;
                case PLUS :
                    mathArithTypeEnum = MathArithTypeEnum.ADD;
                    break;
                case MINUS :
                    mathArithTypeEnum = MathArithTypeEnum.SUBTRACT;
                    break;
                case MOD :
                    mathArithTypeEnum = MathArithTypeEnum.MODULO;
                    break;
                default :
                    throw new ArgumentException("Node type " + node.Type + " not a recognized math node type");
            }
    
            ExprMathNode mathNode = new ExprMathNode(mathArithTypeEnum);
            astExprNodeMap.Put(node, mathNode);
        }
    
        // Min/Max nodes can be either an aggregate or a per-row function depending on the number or arguments
        private void HandleMinMax(ITree libNode)
        {
            log.Debug(".handleMinMax");
    
            // Determine min or max
            ITree childNode = libNode.GetChild(0);
            MinMaxTypeEnum minMaxTypeEnum;
            if (childNode.Text.Equals("min"))
            {
                minMaxTypeEnum = MinMaxTypeEnum.MIN;
            }
            else if (childNode.Text.Equals("max"))
            {
                minMaxTypeEnum = MinMaxTypeEnum.MAX;
            }
            else
            {
                throw new ArgumentException("Node type " + childNode.Type + ' ' + childNode.Text + " not a recognized min max node");
            }
    
            // Determine distinct or not
            ITree nextNode = libNode.GetChild(1);
            bool isDistinct = false;
            if (nextNode.Type == DISTINCT)
            {
                isDistinct = true;
            }
    
            // Error if more then 3 nodes with distinct since it's an aggregate function
            if ((libNode.ChildCount > 3) && (isDistinct))
            {
                throw new ASTWalkException("The distinct keyword is not valid in per-row min and max " +
                        "functions with multiple sub-expressions");
            }
    
            ExprNode minMaxNode;
            if ((!isDistinct) && (libNode.ChildCount > 2))
            {
                // use the row function
                minMaxNode = new ExprMinMaxRowNode(minMaxTypeEnum);
            }
            else
            {
                // use the aggregation function
                minMaxNode = new ExprMinMaxAggrNode(isDistinct, minMaxTypeEnum);
            }
            astExprNodeMap.Put(libNode, minMaxNode);
        }
    
        private void LeaveCoalesce(ITree node)
        {
            log.Debug(".leaveCoalesce");
    
            ExprNode coalesceNode = new ExprCoalesceNode();
            astExprNodeMap.Put(node, coalesceNode);
        }
    
        private void LeaveAggregate(ITree node)
        {
            log.Debug(".leaveAggregate");
    
            bool isDistinct = false;
            if ((node.GetChild(0) != null) && (node.GetChild(0).Type == DISTINCT))
            {
                isDistinct = true;
            }
    
            ExprAggregateNode aggregateNode;
    
            switch (node.Type)
            {
                case AVG:
                    aggregateNode = new ExprAvgNode(isDistinct);
                    break;
                case SUM:
                    aggregateNode = new ExprSumNode(isDistinct);
                    break;
                case COUNT:
                    aggregateNode = new ExprCountNode(isDistinct);
                    break;
                case MEDIAN:
                    aggregateNode = new ExprMedianNode(isDistinct);
                    break;
                case STDDEV:
                    aggregateNode = new ExprStddevNode(isDistinct);
                    break;
                case AVEDEV:
                    aggregateNode = new ExprAvedevNode(isDistinct);
                    break;
                default:
                    throw new ArgumentException("Node type " + node.Type + " not a recognized aggregate node type");
            }
    
            astExprNodeMap.Put(node, aggregateNode);
        }
    
        private void LeaveRelationalOp(ITree node)
        {
            log.Debug(".leaveRelationalOp");
    
            RelationalOpEnum relationalOpEnum;
    
            switch (node.Type)
            {
                case LT :
                    relationalOpEnum = RelationalOpEnum.LT;
                    break;
                case GT :
                    relationalOpEnum = RelationalOpEnum.GT;
                    break;
                case LE :
                    relationalOpEnum = RelationalOpEnum.LE;
                    break;
                case GE :
                    relationalOpEnum = RelationalOpEnum.GE;
                    break;
                default :
                    throw new ArgumentException("Node type " + node.Type + " not a recognized relational op node type");
            }
    
            ExprRelationalOpNode mathNode = new ExprRelationalOpNode(relationalOpEnum);
            astExprNodeMap.Put(node, mathNode);
        }

        private void LeaveBitWise(ITree node)
        {
            log.Debug(".leaveBitWise");
    
            BitWiseOpEnum bitWiseOpEnum;
            switch (node.Type)
            {
    	        case BAND :
    	        	bitWiseOpEnum = BitWiseOpEnum.BAND;
    	            break;
    	        case BOR :
    	        	bitWiseOpEnum = BitWiseOpEnum.BOR;
    	            break;
    	        case BXOR :
    	        	bitWiseOpEnum = BitWiseOpEnum.BXOR;
    	            break;
    	        default :
    	            throw new ArgumentException("Node type " + node.Type + " not a recognized bit wise node type");
            }
    
    	    ExprBitWiseNode bwNode = new ExprBitWiseNode(bitWiseOpEnum);
    	    astExprNodeMap.Put(node, bwNode);
        }
    
        private void LeaveWhereClause()
        {
            log.Debug(".leaveWhereClause");
    
            if (astExprNodeMap.Count != 1)
            {
                throw new IllegalStateException("Where clause generated zero or more then one expression nodes");
            }
    
            // Just assign the single root ExprNode not consumed yet
            statementSpec.FilterRootNode = CollectionHelper.First(astExprNodeMap.Values);
            astExprNodeMap.Clear();
        }
    
        private void LeaveHavingClause()
        {
            log.Debug(".leaveHavingClause");
    
            if (astExprNodeMap.Count != 1)
            {
                throw new IllegalStateException("Having clause generated zero or more then one expression nodes");
            }
    
            // Just assign the single root ExprNode not consumed yet
            statementSpec.HavingExprRootNode = CollectionHelper.First(astExprNodeMap.Values);
            astExprNodeMap.Clear();
        }

        private void LeaveOutputLimit(ITree node)
        {
            log.Debug(".leaveOutputLimit");
    
            OutputLimitSpec spec = ASTOutputLimitHelper.BuildOutputLimitSpec(node);
            statementSpec.OutputLimitSpec = spec;
    
            if (spec.VariableName != null)
            {
                statementSpec.HasVariables = true;
            }
        }

        private void LeaveOuterJoin(ITree node)
        {
            log.Debug(".leaveOuterJoin");
    
            OuterJoinType joinType;
            switch (node.Type)
            {
                case LEFT_OUTERJOIN_EXPR:
                    joinType = OuterJoinType.LEFT;
                    break;
                case RIGHT_OUTERJOIN_EXPR:
                    joinType = OuterJoinType.RIGHT;
                    break;
                case FULL_OUTERJOIN_EXPR:
                    joinType = OuterJoinType.FULL;
                    break;
                default:
                    throw new ArgumentException("Node type " + node.Type + " not a recognized outer join node type");
            }
    
            // get subnodes representing the expression
            ExprIdentNode left = (ExprIdentNode) astExprNodeMap.Get(node.GetChild(0));
            ExprIdentNode right = (ExprIdentNode) astExprNodeMap.Get(node.GetChild(1));
    
            // remove from AST-to-expression node map
            astExprNodeMap.Remove(node.GetChild(0));
            astExprNodeMap.Remove(node.GetChild(1));
    
            // get optional additional
            ExprIdentNode[] addLeftArr = null;
            ExprIdentNode[] addRightArr = null;
            if (node.ChildCount > 2)
            {
                List<ExprIdentNode> addLeft = new List<ExprIdentNode>();
                List<ExprIdentNode> addRight = new List<ExprIdentNode>();
                for (int i = 2; i < node.ChildCount; i+=2)
                {
                    ITree child = node.GetChild(i);
                    addLeft.Add((ExprIdentNode)astExprNodeMap.RemoveAndReturn(child));
                    addRight.Add((ExprIdentNode)astExprNodeMap.RemoveAndReturn(node.GetChild(i + 1)));
                }
                addLeftArr = addLeft.ToArray();
                addRightArr = addRight.ToArray();
            }
    
            OuterJoinDesc outerJoinDesc = new OuterJoinDesc(joinType, left, right, addLeftArr, addRightArr);
            statementSpec.OuterJoinDescList.Add(outerJoinDesc);
        }

        private void LeaveGroupBy(ITree node)
        {
            log.Debug(".leaveGroupBy");
    
            // there must be some expressions under the group by in our map
            if (astExprNodeMap.Count < 1)
            {
                throw new IllegalStateException("Group-by clause generated no expression nodes");
            }
    
            // For each child to the group-by AST node there must be a generated ExprNode
            for (int i = 0; i < node.ChildCount; i++)
            {
                ITree child = node.GetChild(i);
                // get top expression node for the child node
                ExprNode exprNode = astExprNodeMap.Get(child);
    
                if (exprNode == null)
                {
                    throw new IllegalStateException("Expression node as a result of group-by child node not found in collection");
                }
    
                statementSpec.GroupByExpressions.Add(exprNode);
            }
    
            // Clear the map - all expression node should be gone
            astExprNodeMap.Clear();
        }

        private void LeaveInsertInto(ITree node)
        {
            log.Debug(".leaveInsertInto");
    
            int count = 0;
            ITree child = node.GetChild(count);
    
            // istream or rstream
            bool isIStream = true;
            if (child.Type == RSTREAM)
            {
                isIStream = false;
                child = node.GetChild(++count);
            }
            if (child.Type == ISTREAM)
            {
                child = node.GetChild(++count);
            }
    
            // alias
            String eventAliasName = child.Text;
            InsertIntoDesc insertIntoDesc = new InsertIntoDesc(isIStream, eventAliasName);
    
            // optional columns
            child = node.GetChild(++count);
            if ((child != null) && (child.Type == INSERTINTO_EXPRCOL))
            {
                // Each child to the insert-into AST node represents a column name
                for (int i = 0; i < child.ChildCount; i++)
                {
                    ITree childNode = child.GetChild(i);
                    insertIntoDesc.Add(childNode.Text);
                }
            }
    
            statementSpec.InsertIntoDesc = insertIntoDesc;
        }

        private void LeaveOrderByElement(ITree node)
        {
            log.Debug(".leaveOrderByElement");
            if ((astExprNodeMap.Count > 1) || ((astExprNodeMap.Count == 0)))
            {
                throw new ASTWalkException("Unexpected AST tree contains zero or more then 1 child element for root");
            }
    
            // Get expression node sub-tree from the AST nodes placed so far
            ExprNode exprNode = CollectionHelper.First(astExprNodeMap.Values);
            astExprNodeMap.Clear();
    
            // Get optional ascending or descending qualifier
            bool descending = false;
            if (node.ChildCount > 1)
            {
                descending = node.GetChild(1).Type == DESC;
            }
    
            // Add as order-by element
            statementSpec.OrderByList.Add(new OrderByItem(exprNode, descending));
        }

        private void LeaveConcat(ITree node)
        {
            ExprConcatNode concatNode = new ExprConcatNode();
            astExprNodeMap.Put(node, concatNode);
        }

        private void LeaveEvery(ITree node)
        {
            log.Debug(".leaveEvery");
            EvalEveryNode everyNode = new EvalEveryNode();
            astPatternNodeMap.Put(node, everyNode);
        }

        private void LeaveFilter(ITree node)
        {
            log.Debug(".leaveFilter");
    
            int count = 0;
            ITree startNode = node.GetChild(0);
            String optionalPatternTagName = null;
            if (startNode.Type == IDENT)
            {
                optionalPatternTagName = startNode.Text;
                startNode = node.GetChild(++count);
            }
    
            // Determine event type
            String eventName = startNode.Text;

            ITree currentNode = node.GetChild(++count);
            List<ExprNode> exprNodes = GetExprNodes(node, count);
    
            FilterSpecRaw rawFilterSpec = new FilterSpecRaw(eventName, exprNodes);
            if (isProcessingPattern)
            {
                EvalFilterNode filterNode = new EvalFilterNode(rawFilterSpec, optionalPatternTagName);
                astPatternNodeMap.Put(node, filterNode);
            }
            else
            {
                // for event streams we keep the filter spec around for use when the stream definition is completed
                filterSpec = rawFilterSpec;
    
                // clear the sub-nodes for the filter since the event property expressions have been processed
                // by building the spec
                astExprNodeMap.Clear();
            }
        }

        private void LeaveFollowedBy(ITree node)
        {
            log.Debug(".leaveFollowedBy");
            EvalFollowedByNode fbNode = new EvalFollowedByNode();
            astPatternNodeMap.Put(node, fbNode);
        }

        private void LeaveAnd(ITree node)
        {
            log.Debug(".leaveAnd");
            EvalAndNode andNode = new EvalAndNode();
            astPatternNodeMap.Put(node, andNode);
        }

        private void LeaveOr(ITree node)
        {
            log.Debug(".leaveOr");
            EvalOrNode orNode = new EvalOrNode();
            astPatternNodeMap.Put(node, orNode);
        }

        private void LeaveInSet(ITree node)
        {
            log.Debug(".leaveInSet");
    
            ExprInNode inNode = new ExprInNode(node.Type == NOT_IN_SET);
            astExprNodeMap.Put(node, inNode);
        }

        private void LeaveInRange(ITree node)
        {
            log.Debug(".leaveInRange");
    
            // The second node must be braces
            ITree bracesNode = node.GetChild(1);
            if ((bracesNode.Type != LBRACK) && ((bracesNode.Type != LPAREN)))
            {
                throw new IllegalStateException("Invalid in-range syntax, no braces but type '" + bracesNode.Type + "'");
            }
            bool isLowInclude = bracesNode.Type == LBRACK;
    
            // The fifth node must be braces
            bracesNode = node.GetChild(4);
            if ((bracesNode.Type != RBRACK) && ((bracesNode.Type != RPAREN)))
            {
                throw new IllegalStateException("Invalid in-range syntax, no braces but type '" + bracesNode.Type + "'");
            }
            bool isHighInclude = bracesNode.Type == RBRACK;
    
            ExprBetweenNode betweenNode = new ExprBetweenNode(isLowInclude, isHighInclude, node.Type == NOT_IN_RANGE);
            astExprNodeMap.Put(node, betweenNode);
        }

        private void LeaveBetween(ITree node)
        {
            log.Debug(".leaveBetween");
    
            ExprBetweenNode betweenNode = new ExprBetweenNode(true, true, node.Type == NOT_BETWEEN);
            astExprNodeMap.Put(node, betweenNode);
        }

        private void LeaveLike(ITree node)
        {
            log.Debug(".leaveLike");
    
            bool isNot = node.Type == NOT_LIKE;
            ExprLikeNode likeNode = new ExprLikeNode(isNot);
            astExprNodeMap.Put(node, likeNode);
        }
    
        private void LeaveRegexp(ITree node)
        {
            log.Debug(".leaveRegexp");
    
            bool isNot = node.Type == NOT_REGEXP;
            ExprRegexpNode regExpNode = new ExprRegexpNode(isNot);
            astExprNodeMap.Put(node, regExpNode);
        }

        private void LeaveNot(ITree node)
        {
            log.Debug(".leaveNot");
    
            if (isProcessingPattern)
            {
                EvalNotNode notNode = new EvalNotNode();
                astPatternNodeMap.Put(node, notNode);
            }
            else
            {
                ExprNotNode notNode = new ExprNotNode();
                astExprNodeMap.Put(node, notNode);
            }
        }
    
        private void LeaveGuard(ITree node)
        {
            log.Debug(".leaveGuard");
    
            // Get the object information from AST
            ITree startGuard = node.GetChild(1);
            String objectNamespace = startGuard.Text;
            String objectName = node.GetChild(2).Text;
    
            List<Object> objectParams = new List<Object>();
            for (int i = 3; i < node.ChildCount; i++)
            {
            	ITree childNode = node.GetChild(i);
                Object @object = ASTParameterHelper.MakeParameter(childNode, engineTime);
                objectParams.Add(@object);
            }
    
            PatternGuardSpec guardSpec = new PatternGuardSpec(objectNamespace, objectName, objectParams);
            EvalGuardNode guardNode = new EvalGuardNode(guardSpec);
            astPatternNodeMap.Put(node, guardNode);
        }

        private void LeaveCaseNode(ITree node, bool inCase2)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".leaveCase2Node inCase2=" + inCase2);
            }
    
            if (astExprNodeMap.Count == 0)
            {
                throw new ASTWalkException("Unexpected AST tree contains zero child element for case node");
            }
            ITree childNode = node.GetChild(0);
            if (astExprNodeMap.Count == 1)
            {
                throw new ASTWalkException("AST tree doesn not contain at least when node for case node");
            }
    
            ExprCaseNode caseNode = new ExprCaseNode(inCase2);
            astExprNodeMap.Put(node, caseNode);
        }
    
        private void LeaveObserver(ITree node)
        {
            log.Debug(".leaveObserver");
    
            // Get the object information from AST
            String objectNamespace = node.GetChild(0).Text;
            String objectName = node.GetChild(1).Text;
    
            List<Object> objectParams = new List<Object>();
            for (int i = 2; i < node.ChildCount; i++)
            {
            	ITree child = node.GetChild(i);
                Object @object = ASTParameterHelper.MakeParameter(child, engineTime);
                objectParams.Add(@object);
            }
    
            PatternObserverSpec observerSpec = new PatternObserverSpec(objectNamespace, objectName, objectParams);
            EvalObserverNode observerNode = new EvalObserverNode(observerSpec);
            astPatternNodeMap.Put(node, observerNode);
        }

        private void LeaveSelectClause(ITree node)
        {
            log.Debug(".leaveSelectClause");
    
            int nodeType = node.GetChild(0).Type;
            if (nodeType == RSTREAM)
            {
                statementSpec.SelectStreamDirEnum = SelectClauseStreamSelectorEnum.RSTREAM_ONLY;
            }
            if (nodeType == ISTREAM)
            {
                statementSpec.SelectStreamDirEnum = SelectClauseStreamSelectorEnum.ISTREAM_ONLY;
            }
            if (nodeType == IRSTREAM)
            {
                statementSpec.SelectStreamDirEnum = SelectClauseStreamSelectorEnum.RSTREAM_ISTREAM_BOTH;
            }
        }

        private List<ExprNode> GetExprNodes(ITree parentNode, int startIndex)
        {
            List<ExprNode> exprNodes = new List<ExprNode>();
    
            for (int i = startIndex; i < parentNode.ChildCount; i++)
            {
                ITree currentNode = parentNode.GetChild(i);
                ExprNode exprNode = astExprNodeMap.Get(currentNode);
                if (exprNode == null)
                {
                    throw new IllegalStateException("Expression node for AST node not found for type " + currentNode.Type);
                }
                exprNodes.Add(exprNode);
                astExprNodeMap.Remove(currentNode);
            }
            return exprNodes;
        }
    
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
