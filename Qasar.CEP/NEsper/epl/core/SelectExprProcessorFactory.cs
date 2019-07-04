///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.core;
using com.espertech.esper.compat;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using log4net;

namespace com.espertech.esper.epl.core
{
    /// <summary>Factory for select expression processors. </summary>
    public class SelectExprProcessorFactory
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Returns the processor to use for a given select-clause.
        /// </summary>
        /// <param name="selectionList">the list of select clause elements/items, which are expected to have been validated</param>
        /// <param name="isUsingWildcard">true if the wildcard (*) occurs in the select clause</param>
        /// <param name="insertIntoDesc">contains column names for the optional insert-into clause (if supplied)</param>
        /// <param name="typeService">serves stream type information</param>
        /// <param name="eventAdapterService">for generating wrapper instances for events</param>
        /// <param name="statementResultService">handles listeners/subscriptions awareness to reduce output result generation</param>
        /// <param name="valueAddEventService">service that handles update events and variant events</param>
        /// <returns>select-clause expression processor</returns>
        /// <throws>com.espertech.esper.epl.expression.ExprValidationException to indicate the select expression cannot be validated</throws>
        public static SelectExprProcessor GetProcessor(IList<SelectClauseElementCompiled> selectionList,
                                                       bool isUsingWildcard,
                                                       InsertIntoDesc insertIntoDesc,
                                                       StreamTypeService typeService,
                                                       EventAdapterService eventAdapterService,
                                                       StatementResultService statementResultService,
                                                       ValueAddEventService valueAddEventService)
        {
            SelectExprProcessor synthetic = GetProcessorInternal(selectionList, isUsingWildcard, insertIntoDesc, typeService, eventAdapterService, valueAddEventService); 
            BindProcessor bindProcessor = new BindProcessor(selectionList, typeService.EventTypes, typeService.StreamNames);
            statementResultService.SetSelectClause(bindProcessor.ExpressionTypes, bindProcessor.ColumnNamesAssigned);
            
            return new SelectExprResultProcessor(statementResultService, synthetic, bindProcessor);        
        }

        private static SelectExprProcessor GetProcessorInternal(IList<SelectClauseElementCompiled> selectionList,
                                                                bool isUsingWildcard,
                                                                InsertIntoDesc insertIntoDesc,
                                                                StreamTypeService typeService,
                                                                EventAdapterService eventAdapterService,
                                                                ValueAddEventService valueAddEventService)
        {
            // Wildcard not allowed when insert into specifies column order
        	if(isUsingWildcard && insertIntoDesc != null && CollectionHelper.IsNotEmpty(insertIntoDesc.ColumnNames))
        	{
        		throw new ExprValidationException("Wildcard not allowed when insert-into specifies column order");
        	}
    
            // Determine wildcard processor (select *)
            if (IsWildcardsOnly(selectionList))
            {
                // For joins
                if (typeService.StreamNames.Length > 1)
                {
                    log.Debug(".getProcessor Using SelectExprJoinWildcardProcessor");
                    return new SelectExprJoinWildcardProcessor(typeService.StreamNames, typeService.EventTypes, eventAdapterService, insertIntoDesc);
                }
                // Single-table selects with no insert-into
                // don't need extra processing
                else if (insertIntoDesc == null)
                {
                	log.Debug(".getProcessor Using wildcard processor");
                    return new SelectExprWildcardProcessor(typeService.EventTypes[0]);
                }
            }
    
            // Verify the assigned or alias name used is unique
            VerifyNameUniqueness(selectionList);
    
            // Construct processor
            log.Debug(".getProcessor Using SelectExprEvalProcessor");
            List<SelectClauseExprCompiledSpec> expressionList = GetExpressions(selectionList);
            List<SelectClauseStreamCompiledSpec> streamWildcards = GetStreamWildcards(selectionList);
            if (streamWildcards.Count == 0)
            {
                // This one only deals with wildcards and expressions in the selection
                return new SelectExprEvalProcessor(expressionList, insertIntoDesc, isUsingWildcard, typeService, eventAdapterService, valueAddEventService);
            }
            else
            {
                // This one also deals with stream selectors (e.g. select *, p1, s0.* from S0 as s0)
                return new SelectExprEvalProcessorStreams(expressionList, streamWildcards, insertIntoDesc, isUsingWildcard, typeService, eventAdapterService);
            }
        }
    
        /// <summary>Verify that each given name occurs exactly one. </summary>
        /// <param name="selectionList">is the list of select items to verify names</param>
        /// <throws>com.espertech.esper.epl.expression.ExprValidationException thrown if a name occured more then once</throws>
        public static void VerifyNameUniqueness(IList<SelectClauseElementCompiled> selectionList)
        {
            Set<String> names = new HashSet<String>();
            foreach (SelectClauseElementCompiled element in selectionList)
            {
                if (element is SelectClauseExprCompiledSpec)
                {
                    SelectClauseExprCompiledSpec expr = (SelectClauseExprCompiledSpec) element;
                    if (names.Contains(expr.AssignedName))
                    {
                        throw new ExprValidationException("Property alias name '" + expr.AssignedName + "' appears more then once in select clause");
                    }
                    names.Add(expr.AssignedName);
                }
                else if (element is SelectClauseStreamCompiledSpec)
                {
                    SelectClauseStreamCompiledSpec stream = (SelectClauseStreamCompiledSpec) element;
                    if (stream.OptionalAliasName == null)
                    {
                        continue; // ignore no-alias stream selectors
                    }
                    if (names.Contains(stream.OptionalAliasName))
                    {
                        throw new ExprValidationException("Property alias name '" + stream.OptionalAliasName + "' appears more then once in select clause");
                    }
                    names.Add(stream.OptionalAliasName);
                }
            }
        }
    
        private static bool IsWildcardsOnly(IEnumerable<SelectClauseElementCompiled> elements)
        {
            foreach (SelectClauseElementCompiled element in elements)
            {
                if (!(element is SelectClauseElementWildcard))
                {
                    return false;
                }
            }
            return true;
        }
    
        private static List<SelectClauseExprCompiledSpec> GetExpressions(IEnumerable<SelectClauseElementCompiled> elements)
        {
            List<SelectClauseExprCompiledSpec> result = new List<SelectClauseExprCompiledSpec>();
            foreach (SelectClauseElementCompiled element in elements)
            {
                if (element is SelectClauseExprCompiledSpec)
                {
                    result.Add((SelectClauseExprCompiledSpec)element);
                }
            }
            return result;
        }
    
        private static List<SelectClauseStreamCompiledSpec> GetStreamWildcards(IEnumerable<SelectClauseElementCompiled> elements)
        {
            List<SelectClauseStreamCompiledSpec> result = new List<SelectClauseStreamCompiledSpec>();
            foreach (SelectClauseElementCompiled element in elements)
            {
                if (element is SelectClauseStreamCompiledSpec)
                {
                    result.Add((SelectClauseStreamCompiledSpec)element);
                }
            }
            return result;
        }
    }
}
