///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using com.espertech.esper.compat;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.core
{
    /// <summary>Works in conjunction with <see cref="SelectExprResultProcessor"/> to present a result as an object array for 'natural' delivery. </summary>
    public class BindProcessor
    {
        private readonly ExprEvaluator[] expressionNodes;
        private readonly Type[] expressionTypes;
        private readonly String[] columnNamesAssigned;
    
        /// <summary>Ctor. </summary>
        /// <param name="selectionList">the select clause</param>
        /// <param name="typesPerStream">the event types per stream</param>
        /// <param name="streamNames">the stream names</param>
        /// <throws>ExprValidationException when the validation of the select clause failed</throws>
        public BindProcessor(IEnumerable<SelectClauseElementCompiled> selectionList,
                             EventType[] typesPerStream,
                             String[] streamNames)
        {
            List<ExprEvaluator> expressions = new List<ExprEvaluator>();
            List<Type> types = new List<Type>();
            List<String> columnNames = new List<String>();
    
            foreach (SelectClauseElementCompiled element in selectionList)
            {
                // handle wildcards by outputting each stream's underlying event
                if (element is SelectClauseElementWildcard)
                {
                    for (int i = 0; i < typesPerStream.Length; i++)
                    {
                        int streamNum = i;
                        expressions.Add(
                            new ProxyExprEvaluator(
                                delegate(EventBean[] eventsPerStream, bool isNewData) {
                                    EventBean @event = eventsPerStream[streamNum];
                                    if (@event != null) {
                                        return @event.Underlying;
                                    }
                                    else {
                                        return null;
                                    }
                                }));

                        types.Add(typesPerStream[streamNum].UnderlyingType);
                        columnNames.Add(streamNames[streamNum]);
                    }
                }
                // handle stream wildcards by outputting the stream underlying event
                else if (element is SelectClauseStreamCompiledSpec)
                {
                    SelectClauseStreamCompiledSpec streamSpec = (SelectClauseStreamCompiledSpec) element;
                    expressions.Add(
                        new ProxyExprEvaluator(
                            delegate(EventBean[] eventsPerStream, bool isNewData) {
                                EventBean @event = eventsPerStream[streamSpec.StreamNumber];
                                if (@event != null) {
                                    return @event.Underlying;
                                }
                                else {
                                    return null;
                                }
                            }));

                    types.Add(typesPerStream[streamSpec.StreamNumber].UnderlyingType);
                    columnNames.Add(streamNames[streamSpec.StreamNumber]);
                }
    
                // handle expressions
                else if (element is SelectClauseExprCompiledSpec)
                {
                    SelectClauseExprCompiledSpec expr = (SelectClauseExprCompiledSpec) element;
                    expressions.Add(expr.SelectExpression);
                    types.Add(expr.SelectExpression.ReturnType);
                    if (expr.AssignedName != null)
                    {
                        columnNames.Add(expr.AssignedName);
                    }
                    else
                    {
                        columnNames.Add(expr.SelectExpression.ExpressionString);
                    }
                }
                else
                {
                    throw new IllegalStateException("Unrecognized select expression element of type " + element.GetType());
                }
            }
    
            expressionNodes = expressions.ToArray();
            expressionTypes = types.ToArray();
            columnNamesAssigned = columnNames.ToArray();
        }
    
        /// <summary>Process select expressions into columns for native dispatch. </summary>
        /// <param name="eventsPerStream">each stream's events</param>
        /// <param name="isNewData">true for new events</param>
        /// <returns>object array with select-clause results</returns>
        public Object[] Process(EventBean[] eventsPerStream, bool isNewData)
        {
            Object[] parameters = new Object[expressionNodes.Length];
    
            for (int i = 0; i < parameters.Length; i++)
            {
                Object result = expressionNodes[i].Evaluate(eventsPerStream, isNewData);
                parameters[i] = result;
            }
    
            return parameters;
        }
    
        /// <summary>Returns the expression types generated by the select-clause expressions. </summary>
        /// <returns>types</returns>
        public Type[] ExpressionTypes
        {
            get { return expressionTypes; }
        }
    
        /// <summary>Returns the column names of select-clause expressions. </summary>
        /// <returns>column names</returns>
        public String[] ColumnNamesAssigned {
            get { return columnNamesAssigned; }
        }
    }
}
