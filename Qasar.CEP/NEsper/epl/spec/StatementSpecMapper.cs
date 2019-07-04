///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.client.soda;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.parse;
using com.espertech.esper.epl.variable;
using com.espertech.esper.pattern;
using com.espertech.esper.type;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Helper for mapping internal representations of a statement to the SODA object model for statements.
	/// </summary>
    public class StatementSpecMapper
    {
        /// <summary>Maps the SODA object model to a statement specification.</summary>
        /// <param name="sodaStatement">is the object model to map</param>
        /// <param name="engineImportService">for resolving imports such as plug-in aggregations</param>
        /// <param name="variableService">provides variable values</param>
        /// <returns>statement specification, and internal representation of a statement</returns>
        public static StatementSpecRaw Map(EPStatementObjectModel sodaStatement, EngineImportService engineImportService, VariableService variableService)
        {
            StatementSpecMapContext mapContext = new StatementSpecMapContext(engineImportService, variableService);

            StatementSpecRaw raw = Map(sodaStatement, mapContext);
            if (mapContext.HasVariables)
            {
                raw.HasVariables = true;
            }
            return raw;
        }

        private static StatementSpecRaw Map(EPStatementObjectModel sodaStatement, StatementSpecMapContext mapContext)
        {
            StatementSpecRaw raw = new StatementSpecRaw(SelectClauseStreamSelectorEnum.ISTREAM_ONLY);
            MapCreateWindow(sodaStatement.CreateWindow, raw);
            MapCreateVariable(sodaStatement.CreateVariable, raw, mapContext);
            MapOnTrigger(sodaStatement.OnExpr, raw, mapContext);
            MapInsertInto(sodaStatement.InsertInto, raw);
            MapSelect(sodaStatement.SelectClause, raw, mapContext);
            MapFrom(sodaStatement.FromClause, raw, mapContext);
            MapWhere(sodaStatement.WhereClause, raw, mapContext);
            MapGroupBy(sodaStatement.GroupByClause, raw, mapContext);
            MapHaving(sodaStatement.HavingClause, raw, mapContext);
            MapOutputLimit(sodaStatement.OutputLimitClause, raw, mapContext);
            MapOrderBy(sodaStatement.OrderByClause, raw, mapContext);
            return raw;
        }

        /// <summary>Maps the internal representation of a statement to the SODA object model.</summary>
        /// <param name="statementSpec">is the internal representation</param>
        /// <returns>object model of statement</returns>
        public static StatementSpecUnMapResult Unmap(StatementSpecRaw statementSpec)
        {
            StatementSpecUnMapContext unmapContext = new StatementSpecUnMapContext();

            EPStatementObjectModel model = new EPStatementObjectModel();
            UnmapCreateWindow(statementSpec.CreateWindowDesc, model);
            UnmapCreateVariable(statementSpec.CreateVariableDesc, model, unmapContext);
            UnmapOnClause(statementSpec.OnTriggerDesc, model, unmapContext);
            UnmapInsertInto(statementSpec.InsertIntoDesc, model);
            UnmapSelect(statementSpec.SelectClauseSpec, statementSpec.SelectStreamSelectorEnum, model, unmapContext);
            UnmapFrom(statementSpec.StreamSpecs, statementSpec.OuterJoinDescList, model, unmapContext);
            UnmapWhere(statementSpec.FilterExprRootNode, model, unmapContext);
            UnmapGroupBy(statementSpec.GroupByExpressions, model, unmapContext);
            UnmapHaving(statementSpec.HavingExprRootNode, model, unmapContext);
            UnmapOutputLimit(statementSpec.OutputLimitSpec, model);
            UnmapOrderBy(statementSpec.OrderByList, model, unmapContext);

            return new StatementSpecUnMapResult(model, unmapContext.IndexedParams);
        }

        private static void UnmapOnClause(OnTriggerDesc onTriggerDesc, EPStatementObjectModel model, StatementSpecUnMapContext unmapContext)
        {
            if (onTriggerDesc == null)
            {
                return;
            }
            if (onTriggerDesc.OnTriggerType == OnTriggerType.ON_DELETE)
            {
                OnTriggerWindowDesc window = (OnTriggerWindowDesc)onTriggerDesc;
                model.OnExpr = new OnDeleteClause(window.WindowName, window.OptionalAsName);
            }
            if (onTriggerDesc.OnTriggerType == OnTriggerType.ON_SELECT)
            {
                OnTriggerWindowDesc window = (OnTriggerWindowDesc)onTriggerDesc;
                model.OnExpr = new OnSelectClause(window.WindowName, window.OptionalAsName);
            }
            if (onTriggerDesc.OnTriggerType == OnTriggerType.ON_SET)
            {
                OnTriggerSetDesc trigger = (OnTriggerSetDesc)onTriggerDesc;
                OnSetClause clause = new OnSetClause();
                foreach (OnTriggerSetAssignment assignment in trigger.Assignments)
                {
                    Expression expr = UnmapExpressionDeep(assignment.Expression, unmapContext);
                    clause.AddAssignment(assignment.VariableName, expr);
                }
                model.OnExpr = clause;
            }
        }

        private static void UnmapCreateWindow(CreateWindowDesc createWindowDesc, EPStatementObjectModel model)
        {
            if (createWindowDesc == null)
            {
                return;
            }
            model.CreateWindow = new CreateWindowClause(createWindowDesc.WindowName, UnmapViews(createWindowDesc.ViewSpecs));
        }

        private static void UnmapCreateVariable(CreateVariableDesc createVariableDesc, EPStatementObjectModel model, StatementSpecUnMapContext unmapContext)
        {
            if (createVariableDesc == null)
            {
                return;
            }
            Expression assignment = null;
            if (createVariableDesc.Assignment != null)
            {
                assignment = UnmapExpressionDeep(createVariableDesc.Assignment, unmapContext);
            }
            model.CreateVariable = new CreateVariableClause(createVariableDesc.VariableType, createVariableDesc.VariableName, assignment);
        }

        private static void UnmapOrderBy(ICollection<OrderByItem> orderByList, EPStatementObjectModel model, StatementSpecUnMapContext unmapContext)
        {
            if ((orderByList == null) || (orderByList.Count == 0))
            {
                return;
            }

            OrderByClause clause = new OrderByClause();
            foreach (OrderByItem item in orderByList)
            {
                Expression expr = UnmapExpressionDeep(item.ExprNode, unmapContext);
                clause.Add(expr, item.IsDescending);
            }
            model.OrderByClause = clause;
        }

        private static void UnmapOutputLimit(OutputLimitSpec outputLimitSpec, EPStatementObjectModel model)
        {
            if (outputLimitSpec == null)
            {
                return;
            }

            OutputLimitSelector selector = OutputLimitSelector.DEFAULT;
            if (outputLimitSpec.DisplayLimit == OutputLimitLimitType.FIRST)
            {
                selector = OutputLimitSelector.FIRST;
            }

            switch (outputLimitSpec.DisplayLimit) {
                case OutputLimitLimitType.LAST:
                    selector = OutputLimitSelector.LAST;
                    break;
                case OutputLimitLimitType.SNAPSHOT:
                    selector = OutputLimitSelector.SNAPSHOT;
                    break;
                case OutputLimitLimitType.ALL:
                    selector = OutputLimitSelector.ALL;
                    break;
            }

            OutputLimitUnit unit = OutputLimitUnit.EVENTS;
            if (outputLimitSpec.RateType == OutputLimitRateType.TIME_MIN)
            {
                unit = OutputLimitUnit.MINUTES;
            }
            if (outputLimitSpec.RateType == OutputLimitRateType.TIME_SEC)
            {
                unit = OutputLimitUnit.SECONDS;
            }

            OutputLimitClause clause = new OutputLimitClause(selector, outputLimitSpec.Rate, outputLimitSpec.VariableName, unit);
            model.OutputLimitClause = clause;
        }

        private static void MapOrderBy(OrderByClause orderByClause, StatementSpecRaw raw, StatementSpecMapContext mapContext)
        {
            if (orderByClause == null)
            {
                return;
            }
            foreach (OrderByElement element in orderByClause.OrderByExpressions)
            {
                ExprNode orderExpr = MapExpressionDeep(element.Expression, mapContext);
                OrderByItem item = new OrderByItem(orderExpr, element.IsDescending);
                raw.OrderByList.Add(item);
            }
        }

        private static void MapOutputLimit(OutputLimitClause outputLimitClause, StatementSpecRaw raw, StatementSpecMapContext mapContext)
        {
            if (outputLimitClause == null)
            {
                return;
            }

            OutputLimitLimitType displayLimit = EnumHelper.Parse<OutputLimitLimitType>(
                outputLimitClause.Selector.ToString().ToLower());

            OutputLimitRateType rateType = OutputLimitRateType.TIME_SEC;
            if (outputLimitClause.Unit == OutputLimitUnit.EVENTS)
            {
                rateType = OutputLimitRateType.EVENTS;
            }
            else if (outputLimitClause.Unit == OutputLimitUnit.MINUTES)
            {
                rateType = OutputLimitRateType.TIME_MIN;
            }
            else if (outputLimitClause.Unit == OutputLimitUnit.SECONDS)
            {
                rateType = OutputLimitRateType.TIME_SEC;
            }

            Double? frequency = outputLimitClause.Frequency;
            String frequencyVariable = outputLimitClause.FrequencyVariable;

            if (frequencyVariable != null)
            {
                mapContext.HasVariables = true;
            }

            OutputLimitSpec spec = new OutputLimitSpec(frequency, frequencyVariable, rateType, displayLimit);
            raw.OutputLimitSpec = spec;
        }

        private static void MapOnTrigger(OnClause onExpr, StatementSpecRaw raw, StatementSpecMapContext mapContext)
        {
            if (onExpr == null)
            {
                return;
            }

            if (onExpr is OnDeleteClause)
            {
                OnDeleteClause onDeleteClause = (OnDeleteClause)onExpr;
                raw.OnTriggerDesc = new OnTriggerWindowDesc(onDeleteClause.WindowName, onDeleteClause.OptionalAsName, true);
            }
            else if (onExpr is OnSelectClause)
            {
                OnSelectClause onSelectClause = (OnSelectClause)onExpr;
                raw.OnTriggerDesc = new OnTriggerWindowDesc(onSelectClause.WindowName, onSelectClause.OptionalAsName, true);
            }
            else if (onExpr is OnSetClause)
            {
                OnSetClause setClause = (OnSetClause)onExpr;
                OnTriggerSetDesc desc = new OnTriggerSetDesc();
                mapContext.HasVariables = true;
                foreach (Pair<String, Expression> pair in setClause.Assignments)
                {
                    ExprNode expr = MapExpressionDeep(pair.Second, mapContext);
                    desc.AddAssignment(new OnTriggerSetAssignment(pair.First, expr));
                }
                raw.OnTriggerDesc = desc;
            }
            else
            {
                throw new ArgumentException("Cannot map on-clause expression type : " + onExpr);
            }
        }

        private static void MapHaving(Expression havingClause, StatementSpecRaw raw, StatementSpecMapContext mapContext)
        {
            if (havingClause == null)
            {
                return;
            }
            ExprNode node = MapExpressionDeep(havingClause, mapContext);
            raw.HavingExprRootNode = node;
        }

        private static void UnmapHaving(ExprNode havingExprRootNode, EPStatementObjectModel model, StatementSpecUnMapContext unmapContext)
        {
            if (havingExprRootNode == null)
            {
                return;
            }
            Expression expr = UnmapExpressionDeep(havingExprRootNode, unmapContext);
            model.HavingClause = expr;
        }

        private static void MapGroupBy(GroupByClause groupByClause, StatementSpecRaw raw, StatementSpecMapContext mapContext)
        {
            if (groupByClause == null)
            {
                return;
            }
            foreach (Expression expr in groupByClause.GroupByExpressions)
            {
                ExprNode node = MapExpressionDeep(expr, mapContext);
                raw.GroupByExpressions.Add(node);
            }
        }

        private static void UnmapGroupBy(IList<ExprNode> groupByExpressions, EPStatementObjectModel model, StatementSpecUnMapContext unmapContext)
        {
            if (groupByExpressions.Count == 0)
            {
                return;
            }
            GroupByClause clause = new GroupByClause();
            foreach (ExprNode node in groupByExpressions)
            {
                Expression expr = UnmapExpressionDeep(node, unmapContext);
                clause.GroupByExpressions.Add(expr);
            }
            model.GroupByClause = clause;
        }

        private static void MapWhere(Expression whereClause, StatementSpecRaw raw, StatementSpecMapContext mapContext)
        {
            if (whereClause == null)
            {
                return;
            }
            ExprNode node = MapExpressionDeep(whereClause, mapContext);
            raw.FilterRootNode = node;
        }

        private static void UnmapWhere(ExprNode filterRootNode, EPStatementObjectModel model, StatementSpecUnMapContext unmapContext)
        {
            if (filterRootNode == null)
            {
                return;
            }
            Expression expr = UnmapExpressionDeep(filterRootNode, unmapContext);
            model.WhereClause = expr;
        }

	    private static void UnmapFrom(IEnumerable<StreamSpecRaw> streamSpecs,
	                                  IEnumerable<OuterJoinDesc> outerJoinDescList,
	                                  EPStatementObjectModel model,
	                                  StatementSpecUnMapContext unmapContext)
        {
            FromClause from = new FromClause();
            model.FromClause = from;

            foreach (StreamSpecRaw stream in streamSpecs)
            {
                Stream targetStream;
                if (stream is FilterStreamSpecRaw)
                {
                    FilterStreamSpecRaw filterStreamSpec = (FilterStreamSpecRaw)stream;
                    Filter filter = UnmapFilter(filterStreamSpec.RawFilterSpec, unmapContext);
                    FilterStream filterStream = new FilterStream(filter, filterStreamSpec.OptionalStreamName);
                    filterStream.IsUnidirectional = stream.IsUnidirectional;
                    targetStream = filterStream;
                }
                else if (stream is DBStatementStreamSpec)
                {
                    DBStatementStreamSpec db = (DBStatementStreamSpec)stream;
                    targetStream = new SQLStream(db.DatabaseName, db.SqlWithSubsParams, db.OptionalStreamName, db.MetadataSQL);
                }
                else if (stream is PatternStreamSpecRaw)
                {
                    PatternStreamSpecRaw pattern = (PatternStreamSpecRaw)stream;
                    PatternExpr patternExpr = UnmapPatternEvalDeep(pattern.EvalNode, unmapContext);
                    PatternStream patternStream = new PatternStream(patternExpr, pattern.OptionalStreamName);
                    patternStream.IsUnidirectional = stream.IsUnidirectional;
                    targetStream = patternStream;
                }
                else if (stream is MethodStreamSpec)
                {
                    MethodStreamSpec method = (MethodStreamSpec)stream;
                    MethodInvocationStream methodStream = new MethodInvocationStream(method.ClassName, method.MethodName, method.OptionalStreamName);
                    foreach (ExprNode exprNode in method.Expressions)
                    {
                        Expression expr = UnmapExpressionDeep(exprNode, unmapContext);
                        methodStream.AddParameter(expr);
                    }
                    targetStream = methodStream;
                }
                else
                {
                    throw new ArgumentException("Stream modelled by " + stream.GetType() + " cannot be unmapped");
                }

                if (targetStream is ProjectedStream)
                {
                    ProjectedStream projStream = (ProjectedStream)targetStream;
                    foreach (ViewSpec viewSpec in stream.ViewSpecs)
                    {
                        projStream.AddView(View.Create(viewSpec.ObjectNamespace, viewSpec.ObjectName, viewSpec.ObjectParameters));
                    }
                }
                from.Add(targetStream);
            }

            foreach (OuterJoinDesc desc in outerJoinDescList)
            {
                PropertyValueExpression left = (PropertyValueExpression)UnmapExpressionFlat(desc.LeftNode, unmapContext);
                PropertyValueExpression right = (PropertyValueExpression)UnmapExpressionFlat(desc.RightNode, unmapContext);

                List<Pair<PropertyValueExpression, PropertyValueExpression>> additionalProperties = new List<Pair<PropertyValueExpression, PropertyValueExpression>>();
                if (desc.AdditionalLeftNodes != null)
                {
                    for (int i = 0; i < desc.AdditionalLeftNodes.Length; i++)
                    {
                        ExprIdentNode leftNode = desc.AdditionalLeftNodes[i];
                        ExprIdentNode rightNode = desc.AdditionalRightNodes[i];
                        PropertyValueExpression propLeft = (PropertyValueExpression)UnmapExpressionFlat(leftNode, unmapContext);
                        PropertyValueExpression propRight = (PropertyValueExpression)UnmapExpressionFlat(rightNode, unmapContext);
                        additionalProperties.Add(new Pair<PropertyValueExpression, PropertyValueExpression>(propLeft, propRight));
                    }
                }
                from.Add(new OuterJoinQualifier(desc.OuterJoinType, left, right, additionalProperties));
            }

        }

        private static void UnmapSelect(SelectClauseSpecRaw selectClauseSpec,
                                        SelectClauseStreamSelectorEnum selectStreamSelectorEnum,
                                        EPStatementObjectModel model,
                                        StatementSpecUnMapContext unmapContext)
        {
            SelectClause clause = SelectClause.Create();
            clause.StreamSelector = SelectClauseStreamSelectorHelper.MapFromSODA(selectStreamSelectorEnum);
            foreach (SelectClauseElementRaw raw in selectClauseSpec.SelectExprList) {
                if (raw is SelectClauseStreamRawSpec) {
                    SelectClauseStreamRawSpec streamSpec = (SelectClauseStreamRawSpec) raw;
                    clause.AddStreamWildcard(streamSpec.StreamAliasName, streamSpec.OptionalAsName);
                } else if (raw is SelectClauseElementWildcard) {
                    clause.AddWildcard();
                } else if (raw is SelectClauseExprRawSpec) {
                    SelectClauseExprRawSpec rawSpec = (SelectClauseExprRawSpec) raw;
                    Expression expression = UnmapExpressionDeep(rawSpec.SelectExpression, unmapContext);
                    clause.Add(expression, rawSpec.OptionalAsName);
                } else {
                    throw new IllegalStateException("Unexpected select clause element type " + raw.GetType().FullName);
                }
            }
            model.SelectClause = clause;
        }

	    private static void UnmapInsertInto(InsertIntoDesc insertIntoDesc, EPStatementObjectModel model)
        {
            StreamSelector s = StreamSelector.ISTREAM_ONLY;
            if (insertIntoDesc == null)
            {
                return;
            }
            if (!insertIntoDesc.IsIStream)
            {
                s = StreamSelector.RSTREAM_ONLY;
            }
            model.InsertInto = InsertIntoClause.Create(
                insertIntoDesc.EventTypeAlias,
                CollectionHelper.ToArray(insertIntoDesc.ColumnNames),
                s);
        }

        private static void MapCreateWindow(CreateWindowClause createWindow, StatementSpecRaw raw)
        {
            if (createWindow == null)
            {
                return;
            }

            raw.CreateWindowDesc = new CreateWindowDesc(createWindow.WindowName, MapViews(createWindow.Views));
        }

        private static void MapCreateVariable(CreateVariableClause createVariable, StatementSpecRaw raw, StatementSpecMapContext mapContext)
        {
            if (createVariable == null)
            {
                return;
            }

            ExprNode assignment = null;
            if (createVariable.OptionalAssignment != null)
            {
                assignment = MapExpressionDeep(createVariable.OptionalAssignment, mapContext);
            }

            raw.CreateVariableDesc = new CreateVariableDesc(createVariable.VariableType, createVariable.VariableName, assignment);
        }

        private static void MapInsertInto(InsertIntoClause insertInto, StatementSpecRaw raw)
        {
            if (insertInto == null)
            {
                return;
            }

            bool isIStream = insertInto.IsIStream;
            String eventTypeAlias = insertInto.StreamName;
            InsertIntoDesc desc = new InsertIntoDesc(isIStream, eventTypeAlias);

            foreach (String name in insertInto.ColumnNames)
            {
                desc.Add(name);
            }

            raw.InsertIntoDesc = desc;
        }

        private static void MapSelect(SelectClause selectClause, StatementSpecRaw raw, StatementSpecMapContext mapContext)
        {
            if (selectClause == null) {
                return;
            }
            SelectClauseSpecRaw spec = new SelectClauseSpecRaw();
            raw.SelectStreamDirEnum = SelectClauseStreamSelectorHelper.MapFromSODA(selectClause.StreamSelector);
            raw.SelectClauseSpec = spec;

            foreach (SelectClauseElement element in selectClause.SelectList) {
                if (element is SelectClauseWildcard) {
                    spec.Add(new SelectClauseElementWildcard());
                } else if (element is SelectClauseExpression) {
                    SelectClauseExpression selectExpr = (SelectClauseExpression) element;
                    Expression expr = selectExpr.Expression;
                    ExprNode exprNode = MapExpressionDeep(expr, mapContext);
                    SelectClauseExprRawSpec rawElement = new SelectClauseExprRawSpec(exprNode, selectExpr.GetAsName());
                    spec.Add(rawElement);
                } else if (element is SelectClauseStreamWildcard) {
                    SelectClauseStreamWildcard streamWild = (SelectClauseStreamWildcard) element;
                    SelectClauseStreamRawSpec rawElement =
                        new SelectClauseStreamRawSpec(streamWild.StreamAliasName, streamWild.OptionalColumnAlias);
                    spec.Add(rawElement);
                }
            }
        }

	    private static Expression UnmapExpressionDeep(ExprNode exprNode, StatementSpecUnMapContext unmapContext)
        {
            Expression parent = UnmapExpressionFlat(exprNode, unmapContext);
            UnmapExpressionRecursive(parent, exprNode, unmapContext);
            return parent;
        }

        private static ExprNode MapExpressionDeep(Expression expr, StatementSpecMapContext mapContext)
        {
            ExprNode parent = MapExpressionFlat(expr, mapContext);
            MapExpressionRecursive(parent, expr, mapContext);
            return parent;
        }

        private static ExprNode MapExpressionFlat(Expression expr, StatementSpecMapContext mapContext)
        {
            if (expr == null)
            {
                throw new ArgumentException("Null expression parameter");
            }
            if (expr is ArithmaticExpression)
            {
                ArithmaticExpression arith = (ArithmaticExpression)expr;
                return new ExprMathNode(MathArithTypeEnum.ParseOperator(arith.Operator));
            }
            else if (expr is PropertyValueExpression)
            {
                PropertyValueExpression prop = (PropertyValueExpression)expr;
                int indexDot = ASTFilterSpecHelper.UnescapedIndexOfDot(prop.PropertyName);
                if (indexDot != -1)
                {
                    String stream = prop.PropertyName.Substring(0, indexDot);
                    String property = prop.PropertyName.Substring(indexDot + 1);
                    return new ExprIdentNode(property, stream);
                }

                if (mapContext.VariableService.GetReader(prop.PropertyName) != null)
                {
                    mapContext.HasVariables = true;
                    return new ExprVariableNode(prop.PropertyName);
                }
                return new ExprIdentNode(prop.PropertyName);
            }
            else if (expr is Conjunction)
            {
                return new ExprAndNode();
            }
            else if (expr is Disjunction)
            {
                return new ExprOrNode();
            }
            else if (expr is RelationalOpExpression)
            {
                RelationalOpExpression op = (RelationalOpExpression)expr;
                if (op.Operator.Equals("="))
                {
                    return new ExprEqualsNode(false);
                }
                if (op.Operator.Equals("!="))
                {
                    return new ExprEqualsNode(true);
                }
                else
                {
                    return new ExprRelationalOpNode(RelationalOpEnum.Parse(op.Operator));
                }
            }
            else if (expr is ConstantExpression)
            {
                ConstantExpression op = (ConstantExpression)expr;
                return new ExprConstantNode(op.Constant);
            }
            else if (expr is ConcatExpression)
            {
                return new ExprConcatNode();
            }
            else if (expr is SubqueryExpression)
            {
                SubqueryExpression sub = (SubqueryExpression)expr;
                StatementSpecRaw rawSubselect = Map(sub.Model, mapContext);
                return new ExprSubselectRowNode(rawSubselect);
            }
            else if (expr is SubqueryInExpression)
            {
                SubqueryInExpression sub = (SubqueryInExpression)expr;
                StatementSpecRaw rawSubselect = Map(sub.Model, mapContext);
                ExprSubselectInNode inSub = new ExprSubselectInNode(rawSubselect);
                inSub.IsNotIn = sub.IsNotIn;
                return inSub;
            }
            else if (expr is SubqueryExistsExpression)
            {
                SubqueryExistsExpression sub = (SubqueryExistsExpression)expr;
                StatementSpecRaw rawSubselect = Map(sub.Model, mapContext);
                return new ExprSubselectExistsNode(rawSubselect);
            }
            else if (expr is CountStarProjectionExpression)
            {
                return new ExprCountNode(false);
            }
            else if (expr is CountProjectionExpression)
            {
                CountProjectionExpression count = (CountProjectionExpression)expr;
                return new ExprCountNode(count.IsDistinct);
            }
            else if (expr is AvgProjectionExpression)
            {
                AvgProjectionExpression avg = (AvgProjectionExpression)expr;
                return new ExprAvgNode(avg.IsDistinct);
            }
            else if (expr is SumProjectionExpression)
            {
                SumProjectionExpression avg = (SumProjectionExpression)expr;
                return new ExprSumNode(avg.IsDistinct);
            }
            else if (expr is BetweenExpression)
            {
                BetweenExpression between = (BetweenExpression)expr;
                return new ExprBetweenNode(between.IsLowEndpointIncluded, between.IsHighEndpointIncluded, between.IsNotBetween);
            }
            else if (expr is PriorExpression)
            {
                return new ExprPriorNode();
            }
            else if (expr is PreviousExpression)
            {
                return new ExprPreviousNode();
            }
            else if (expr is StaticMethodExpression)
            {
                StaticMethodExpression method = (StaticMethodExpression)expr;
                return new ExprStaticMethodNode(method.ClassName, method.Method);
            }
            else if (expr is MinProjectionExpression)
            {
                MinProjectionExpression method = (MinProjectionExpression)expr;
                return new ExprMinMaxAggrNode(method.IsDistinct, MinMaxTypeEnum.MIN);
            }
            else if (expr is MaxProjectionExpression)
            {
                MaxProjectionExpression method = (MaxProjectionExpression)expr;
                return new ExprMinMaxAggrNode(method.IsDistinct, MinMaxTypeEnum.MAX);
            }
            else if (expr is NotExpression)
            {
                return new ExprNotNode();
            }
            else if (expr is InExpression)
            {
                InExpression @in = (InExpression)expr;
                return new ExprInNode(@in.IsNotIn);
            }
            else if (expr is CoalesceExpression)
            {
                return new ExprCoalesceNode();
            }
            else if (expr is CaseWhenThenExpression)
            {
                return new ExprCaseNode(false);
            }
            else if (expr is CaseSwitchExpression)
            {
                return new ExprCaseNode(true);
            }
            else if (expr is MaxRowExpression)
            {
                return new ExprMinMaxRowNode(MinMaxTypeEnum.MAX);
            }
            else if (expr is MinRowExpression)
            {
                return new ExprMinMaxRowNode(MinMaxTypeEnum.MIN);
            }
            else if (expr is BitwiseOpExpression)
            {
                BitwiseOpExpression bit = (BitwiseOpExpression)expr;
                return new ExprBitWiseNode(bit.BinaryOp);
            }
            else if (expr is ArrayExpression)
            {
                return new ExprArrayNode();
            }
            else if (expr is LikeExpression)
            {
                return new ExprLikeNode(false);
            }
            else if (expr is RegExpExpression)
            {
                return new ExprRegexpNode(false);
            }
            else if (expr is MedianProjectionExpression)
            {
                MedianProjectionExpression median = (MedianProjectionExpression)expr;
                return new ExprMedianNode(median.IsDistinct);
            }
            else if (expr is AvedevProjectionExpression)
            {
                AvedevProjectionExpression node = (AvedevProjectionExpression)expr;
                return new ExprAvedevNode(node.IsDistinct);
            }
            else if (expr is StddevProjectionExpression)
            {
                StddevProjectionExpression node = (StddevProjectionExpression)expr;
                return new ExprStddevNode(node.IsDistinct);
            }
            else if (expr is InstanceOfExpression)
            {
                InstanceOfExpression node = (InstanceOfExpression)expr;
                return new ExprInstanceofNode(node.TypeNames);
            }
            else if (expr is CastExpression)
            {
                CastExpression node = (CastExpression)expr;
                return new ExprCastNode(node.TypeName);
            }
            else if (expr is PropertyExistsExpression)
            {
                return new ExprPropertyExistsNode();
            }
            else if (expr is CurrentTimestampExpression)
            {
                return new ExprTimestampNode();
            }
            else if (expr is SubstitutionParameterExpression)
            {
                SubstitutionParameterExpression node = (SubstitutionParameterExpression)expr;
                if (!(node.IsSatisfied))
                {
                    throw new EPException("Substitution parameter value for index " + node.Index + " not set, please provide a value for this parameter");
                }
                return new ExprConstantNode(node.Constant);
            }
            else if (expr is PlugInProjectionExpression)
            {
                PlugInProjectionExpression node = (PlugInProjectionExpression)expr;
                try
                {
                    AggregationSupport aggregation = mapContext.EngineImportService.ResolveAggregation(node.FunctionName);
                    return new ExprPlugInAggFunctionNode(node.IsDistinct, aggregation, node.FunctionName);
                }
                catch (EngineImportUndefinedException e)
                {
                    throw new EPException("Error resolving aggregation: " + e.Message, e);
                }
                catch (EngineImportException e)
                {
                    throw new EPException("Error resolving aggregation: " + e.Message, e);
                }
            }
            throw new ArgumentException("Could not map expression node of type " + expr.GetType().FullName);
        }

        private static Expression UnmapExpressionFlat(ExprNode expr, StatementSpecUnMapContext unmapContext)
        {
            if (expr is ExprMathNode)
            {
                ExprMathNode math = (ExprMathNode)expr;
                return new ArithmaticExpression(math.MathArithTypeEnum.ExpressionText);
            }
            else if (expr is ExprIdentNode)
            {
                ExprIdentNode prop = (ExprIdentNode)expr;
                String propertyName = prop.UnresolvedPropertyName;
                if (prop.StreamOrPropertyName != null)
                {
                    propertyName = prop.StreamOrPropertyName + "." + prop.UnresolvedPropertyName;
                }
                return new PropertyValueExpression(propertyName);
            }
            else if (expr is ExprVariableNode)
            {
                ExprVariableNode prop = (ExprVariableNode)expr;
                String propertyName = prop.VariableName;
                return new PropertyValueExpression(propertyName);
            }
            else if (expr is ExprEqualsNode)
            {
                ExprEqualsNode equals = (ExprEqualsNode)expr;
                String @operator = "=";
                if (equals.IsNotEquals)
                {
                    @operator = "!=";
                }
                return new RelationalOpExpression(@operator);
            }
            else if (expr is ExprRelationalOpNode)
            {
                ExprRelationalOpNode rel = (ExprRelationalOpNode)expr;
                return new RelationalOpExpression(rel.RelationalOpEnum.ExpressionText);
            }
            else if (expr is ExprAndNode)
            {
                return new Conjunction();
            }
            else if (expr is ExprOrNode)
            {
                return new Disjunction();
            }
            else if (expr is ExprConstantNode)
            {
                ExprConstantNode constNode = (ExprConstantNode)expr;
                return new ConstantExpression(constNode.Value);
            }
            else if (expr is ExprConcatNode)
            {
                return new ConcatExpression();
            }
            else if (expr is ExprSubselectRowNode)
            {
                ExprSubselectRowNode sub = (ExprSubselectRowNode)expr;
                StatementSpecUnMapResult unmapped = Unmap(sub.StatementSpecRaw);
                unmapContext.AddAll(unmapped.IndexedParams);
                return new SubqueryExpression(unmapped.ObjectModel);
            }
            else if (expr is ExprSubselectInNode)
            {
                ExprSubselectInNode sub = (ExprSubselectInNode)expr;
                StatementSpecUnMapResult unmapped = Unmap(sub.StatementSpecRaw);
                unmapContext.AddAll(unmapped.IndexedParams);
                return new SubqueryInExpression(unmapped.ObjectModel, sub.IsNotIn);
            }
            else if (expr is ExprSubselectExistsNode)
            {
                ExprSubselectExistsNode sub = (ExprSubselectExistsNode)expr;
                StatementSpecUnMapResult unmapped = Unmap(sub.StatementSpecRaw);
                unmapContext.AddAll(unmapped.IndexedParams);
                return new SubqueryExistsExpression(unmapped.ObjectModel);
            }
            else if (expr is ExprCountNode)
            {
                ExprCountNode sub = (ExprCountNode)expr;
                if (sub.ChildNodes.Count == 0)
                {
                    return new CountStarProjectionExpression();
                }
                else
                {
                    return new CountProjectionExpression(sub.IsDistinct);
                }
            }
            else if (expr is ExprAvgNode)
            {
                ExprAvgNode sub = (ExprAvgNode)expr;
                return new AvgProjectionExpression(sub.IsDistinct);
            }
            else if (expr is ExprSumNode)
            {
                ExprSumNode sub = (ExprSumNode)expr;
                return new SumProjectionExpression(sub.IsDistinct);
            }
            else if (expr is ExprBetweenNode)
            {
                ExprBetweenNode between = (ExprBetweenNode)expr;
                return new BetweenExpression(between.IsLowEndpointIncluded, between.IsHighEndpointIncluded, between.IsNotBetween);
            }
            else if (expr is ExprPriorNode)
            {
                return new PriorExpression();
            }
            else if (expr is ExprPreviousNode)
            {
                return new PreviousExpression();
            }
            else if (expr is ExprStaticMethodNode)
            {
                ExprStaticMethodNode node = (ExprStaticMethodNode)expr;
                return new StaticMethodExpression(node.ClassName, node.MethodName);
            }
            else if (expr is ExprMinMaxAggrNode)
            {
                ExprMinMaxAggrNode node = (ExprMinMaxAggrNode)expr;
                if (node.MinMaxTypeEnum == MinMaxTypeEnum.MIN)
                {
                    return new MinProjectionExpression(node.IsDistinct);
                }
                else
                {
                    return new MaxProjectionExpression(node.IsDistinct);
                }
            }
            else if (expr is ExprNotNode)
            {
                return new NotExpression();
            }
            else if (expr is ExprInNode)
            {
                ExprInNode @in = (ExprInNode)expr;
                return new InExpression(@in.IsNotIn);
            }
            else if (expr is ExprCoalesceNode)
            {
                return new CoalesceExpression();
            }
            else if (expr is ExprCaseNode)
            {
                ExprCaseNode mycase = (ExprCaseNode)expr;
                if (mycase.IsCase2)
                {
                    return new CaseSwitchExpression();
                }
                else
                {
                    return new CaseWhenThenExpression();
                }
            }
            else if (expr is ExprMinMaxRowNode)
            {
                ExprMinMaxRowNode node = (ExprMinMaxRowNode)expr;
                if (node.MinMaxTypeEnum == MinMaxTypeEnum.MAX)
                {
                    return new MaxRowExpression();
                }
                return new MinRowExpression();
            }
            else if (expr is ExprBitWiseNode)
            {
                ExprBitWiseNode node = (ExprBitWiseNode)expr;
                return new BitwiseOpExpression(node.BitWiseOpEnum);
            }
            else if (expr is ExprArrayNode)
            {
                return new ArrayExpression();
            }
            else if (expr is ExprLikeNode)
            {
                return new LikeExpression();
            }
            else if (expr is ExprRegexpNode)
            {
                return new RegExpExpression();
            }
            else if (expr is ExprMedianNode)
            {
                ExprMedianNode median = (ExprMedianNode)expr;
                return new MedianProjectionExpression(median.IsDistinct);
            }
            else if (expr is ExprAvedevNode)
            {
                ExprAvedevNode node = (ExprAvedevNode)expr;
                return new AvedevProjectionExpression(node.IsDistinct);
            }
            else if (expr is ExprStddevNode)
            {
                ExprStddevNode node = (ExprStddevNode)expr;
                return new StddevProjectionExpression(node.IsDistinct);
            }
            else if (expr is ExprPlugInAggFunctionNode)
            {
                ExprPlugInAggFunctionNode node = (ExprPlugInAggFunctionNode)expr;
                return new PlugInProjectionExpression(node.AggregationFunctionName, node.IsDistinct);
            }
            else if (expr is ExprInstanceofNode)
            {
                ExprInstanceofNode node = (ExprInstanceofNode)expr;
                return new InstanceOfExpression(node.TypeIdentifiers);
            }
            else if (expr is ExprCastNode)
            {
                ExprCastNode node = (ExprCastNode)expr;
                return new CastExpression(node.ClassIdentifier);
            }
            else if (expr is ExprPropertyExistsNode)
            {
                return new PropertyExistsExpression();
            }
            else if (expr is ExprTimestampNode)
            {
                return new CurrentTimestampExpression();
            }
            else if (expr is ExprSubstitutionNode)
            {
                ExprSubstitutionNode node = (ExprSubstitutionNode)expr;
                SubstitutionParameterExpression subParam = new SubstitutionParameterExpression(node.Index);
                unmapContext.Add(node.Index, subParam);
                return subParam;
            }
            throw new ArgumentException("Could not map expression node of type " + expr.GetType().FullName);
        }

        private static void UnmapExpressionRecursive(Expression parent, ExprNode expr, StatementSpecUnMapContext unmapContext)
        {
            foreach (ExprNode child in expr.ChildNodes)
            {
                Expression result = UnmapExpressionFlat(child, unmapContext);
                parent.Children.Add(result);
                UnmapExpressionRecursive(result, child, unmapContext);
            }
        }

        private static void MapExpressionRecursive(ExprNode parent, Expression expr, StatementSpecMapContext mapContext)
        {
            foreach (Expression child in expr.Children)
            {
                ExprNode result = MapExpressionFlat(child, mapContext);
                parent.AddChildNode(result);
                MapExpressionRecursive(result, child, mapContext);
            }
        }

        private static void MapFrom(FromClause fromClause, StatementSpecRaw raw, StatementSpecMapContext mapContext)
        {
            if (fromClause == null)
            {
                return;
            }

            foreach (Stream stream in fromClause.Streams)
            {
                StreamSpecRaw spec;

                if (stream is FilterStream)
                {
                    FilterStream filterStream = (FilterStream)stream;
                    FilterSpecRaw filterSpecRaw = MapFilter(filterStream.Filter, mapContext);
                    spec = new FilterStreamSpecRaw(filterSpecRaw,
                                                   new List<ViewSpec>(),
                                                   filterStream.StreamName,
                                                   filterStream.IsUnidirectional);
                }
                else if (stream is SQLStream)
                {
                    SQLStream sqlStream = (SQLStream)stream;
                    spec = new DBStatementStreamSpec(sqlStream.StreamName,
                                                     new List<ViewSpec>(),
                                                     sqlStream.DatabaseName,
                                                     sqlStream.SqlWithSubsParams,
                                                     sqlStream.OptionalMetadataSQL);
                }
                else if (stream is PatternStream)
                {
                    PatternStream patternStream = (PatternStream)stream;
                    EvalNode child = MapPatternEvalDeep(patternStream.Expression, mapContext);
                    spec = new PatternStreamSpecRaw(child,
                                                    new List<ViewSpec>(),
                                                    patternStream.StreamName,
                                                    patternStream.IsUnidirectional);
                }
                else if (stream is MethodInvocationStream)
                {
                    MethodInvocationStream methodStream = (MethodInvocationStream)stream;
                    IList<ExprNode> expressions = new List<ExprNode>();
                    foreach (Expression expr in methodStream.ParameterExpressions)
                    {
                        ExprNode exprNode = MapExpressionDeep(expr, mapContext);
                        expressions.Add(exprNode);
                    }

                    spec = new MethodStreamSpec(methodStream.StreamName, new List<ViewSpec>(), "method",
                            methodStream.ClassName, methodStream.MethodName, expressions);
                }
                else
                {
                    throw new ArgumentException("Could not map from stream " + stream + " to an internal representation");
                }

                raw.StreamSpecs.Add(spec);

                if (stream is ProjectedStream)
                {
                    ProjectedStream projectedStream = (ProjectedStream)stream;
                    CollectionHelper.AddAll(spec.ViewSpecs, MapViews(projectedStream.Views));
                }
            }

            foreach (OuterJoinQualifier qualifier in fromClause.OuterJoinQualifiers)
            {
                ExprIdentNode left = (ExprIdentNode)MapExpressionFlat(qualifier.Left, mapContext);
                ExprIdentNode right = (ExprIdentNode)MapExpressionFlat(qualifier.Right, mapContext);

                ExprIdentNode[] additionalLeft = null;
                ExprIdentNode[] additionalRight = null;
                if (qualifier.AdditionalProperties.Count != 0)
                {
                    additionalLeft = new ExprIdentNode[qualifier.AdditionalProperties.Count];
                    additionalRight = new ExprIdentNode[qualifier.AdditionalProperties.Count];
                    int count = 0;
                    foreach (Pair<PropertyValueExpression, PropertyValueExpression> pair in qualifier.AdditionalProperties)
                    {
                        additionalLeft[count] = (ExprIdentNode)MapExpressionFlat(pair.First, mapContext);
                        additionalRight[count] = (ExprIdentNode)MapExpressionFlat(pair.Second, mapContext);
                        count++;
                    }
                }
                raw.OuterJoinDescList.Add(new OuterJoinDesc(qualifier.JoinType, left, right, additionalLeft, additionalRight));
            }
        }

        private static IList<ViewSpec> MapViews(IList<View> views)
        {
            IList<ViewSpec> viewSpecs = new List<ViewSpec>();
            foreach (View view in views)
            {
                viewSpecs.Add(new ViewSpec(view.Namespace, view.Name, view.Parameters));
            }
            return viewSpecs;
        }

        private static IList<View> UnmapViews(IList<ViewSpec> viewSpecs)
        {
            IList<View> views = new List<View>();
            foreach (ViewSpec viewSpec in viewSpecs)
            {
                views.Add(View.Create(viewSpec.ObjectNamespace, viewSpec.ObjectName, viewSpec.ObjectParameters));
            }
            return views;
        }

        private static EvalNode MapPatternEvalFlat(PatternExpr eval, StatementSpecMapContext mapContext)
        {
            if (eval == null)
            {
                throw new ArgumentException("Null expression parameter");
            }
            if (eval is PatternAndExpr)
            {
                return new EvalAndNode();
            }
            else if (eval is PatternOrExpr)
            {
                return new EvalOrNode();
            }
            else if (eval is PatternFollowedByExpr)
            {
                return new EvalFollowedByNode();
            }
            else if (eval is PatternEveryExpr)
            {
                return new EvalEveryNode();
            }
            else if (eval is PatternFilterExpr)
            {
                PatternFilterExpr filterExpr = (PatternFilterExpr)eval;
                FilterSpecRaw filterSpec = MapFilter(filterExpr.Filter, mapContext);
                return new EvalFilterNode(filterSpec, filterExpr.TagName);
            }
            else if (eval is PatternObserverExpr)
            {
                PatternObserverExpr observer = (PatternObserverExpr)eval;
                return new EvalObserverNode(new PatternObserverSpec(observer.Namespace, observer.Name, observer.Parameters));
            }
            else if (eval is PatternGuardExpr)
            {
                PatternGuardExpr guard = (PatternGuardExpr)eval;
                return new EvalGuardNode(new PatternGuardSpec(guard.Namespace, guard.Name, guard.Parameters));
            }
            else if (eval is PatternNotExpr)
            {
                return new EvalNotNode();
            }
            throw new ArgumentException("Could not map pattern expression node of type " + eval.GetType().FullName);
        }

        private static PatternExpr UnmapPatternEvalFlat(EvalNode eval, StatementSpecUnMapContext unmapContext)
        {
            if (eval is EvalAndNode)
            {
                return new PatternAndExpr();
            }
            else if (eval is EvalOrNode)
            {
                return new PatternOrExpr();
            }
            else if (eval is EvalFollowedByNode)
            {
                return new PatternFollowedByExpr();
            }
            else if (eval is EvalEveryNode)
            {
                return new PatternEveryExpr();
            }
            else if (eval is EvalNotNode)
            {
                return new PatternNotExpr();
            }
            else if (eval is EvalFilterNode)
            {
                EvalFilterNode filterNode = (EvalFilterNode)eval;
                Filter filter = UnmapFilter(filterNode.RawFilterSpec, unmapContext);
                return new PatternFilterExpr(filter, filterNode.EventAsName);
            }
            else if (eval is EvalObserverNode)
            {
                EvalObserverNode observerNode = (EvalObserverNode)eval;
                return new PatternObserverExpr(observerNode.PatternObserverSpec.ObjectNamespace,
                        observerNode.PatternObserverSpec.ObjectName, observerNode.PatternObserverSpec.ObjectParameters);
            }
            else if (eval is EvalGuardNode)
            {
                EvalGuardNode guardNode = (EvalGuardNode)eval;
                return new PatternGuardExpr(guardNode.PatternGuardSpec.ObjectNamespace,
                        guardNode.PatternGuardSpec.ObjectName, guardNode.PatternGuardSpec.ObjectParameters);
            }
            throw new ArgumentException("Could not map pattern expression node of type " + eval.GetType().FullName);
        }

        private static void UnmapPatternEvalRecursive(PatternExpr parent, EvalNode eval, StatementSpecUnMapContext unmapContext)
        {
            foreach (EvalNode child in eval.ChildNodes)
            {
                PatternExpr result = UnmapPatternEvalFlat(child, unmapContext);
                parent.Children.Add(result);
                UnmapPatternEvalRecursive(result, child, unmapContext);
            }
        }

        private static void MapPatternEvalRecursive(EvalNode parent, PatternExpr expr, StatementSpecMapContext mapContext)
        {
            foreach (PatternExpr child in expr.Children)
            {
                EvalNode result = MapPatternEvalFlat(child, mapContext);
                parent.AddChildNode(result);
                MapPatternEvalRecursive(result, child, mapContext);
            }
        }

        private static PatternExpr UnmapPatternEvalDeep(EvalNode exprNode, StatementSpecUnMapContext unmapContext)
        {
            PatternExpr parent = UnmapPatternEvalFlat(exprNode, unmapContext);
            UnmapPatternEvalRecursive(parent, exprNode, unmapContext);
            return parent;
        }

        private static EvalNode MapPatternEvalDeep(PatternExpr expr, StatementSpecMapContext mapContext)
        {
            EvalNode parent = MapPatternEvalFlat(expr, mapContext);
            MapPatternEvalRecursive(parent, expr, mapContext);
            return parent;
        }

        private static FilterSpecRaw MapFilter(Filter filter, StatementSpecMapContext mapContext)
        {
            IList<ExprNode> expr = new List<ExprNode>();
            if (filter.FilterExpression != null)
            {
                ExprNode exprNode = MapExpressionDeep(filter.FilterExpression, mapContext);
                expr.Add(exprNode);
            }

            return new FilterSpecRaw(filter.EventTypeAlias, expr);
        }

        private static Filter UnmapFilter(FilterSpecRaw filter, StatementSpecUnMapContext unmapContext)
        {
            Expression expr = null;
            if (filter.FilterExpressions.Count > 1)
            {
                expr = new Conjunction();
                foreach (ExprNode exprNode in filter.FilterExpressions)
                {
                    Expression expression = UnmapExpressionDeep(exprNode, unmapContext);
                    expr.Children.Add(expression);
                }
            }
            else if (filter.FilterExpressions.Count == 1)
            {
                expr = UnmapExpressionDeep(filter.FilterExpressions[0], unmapContext);
            }

            return new Filter(filter.EventTypeAlias, expr);
        }
    }
} // End of namespace
