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
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.util;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.epl.variable
{
    /// <summary>
    /// A view that handles the setting of variables upon receipt of a triggering event.
    /// <para>
    /// Variables are updated atomically and thus a separate commit actually updates the
    /// new variable values, or a rollback if an exception occured during validation.
    /// </para>
    /// </summary>
	public class OnSetVariableView : ViewSupport
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	    private readonly OnTriggerSetDesc desc;
	    private readonly EventAdapterService eventAdapterService;
	    private readonly VariableService variableService;
	    private readonly EventType eventType;
	    private readonly VariableReader[] readers;
	    private readonly EventBean[] eventsPerStream = new EventBean[1];
	    private readonly bool[] mustCoerce;
        private readonly StatementResultService statementResultService;

        /// <summary>Ctor.</summary>
        /// <param name="desc">specification for the on-set statement</param>
        /// <param name="eventAdapterService">for creating statements</param>
        /// <param name="variableService">for setting variables</param>
        /// <param name="statementResultService">for coordinating on whether insert and remove stream events should be posted</param>
        /// <throws>ExprValidationException if the assignment expressions are invalid</throws>

        public OnSetVariableView(OnTriggerSetDesc desc,
                                 EventAdapterService eventAdapterService,
                                 VariableService variableService,
                                 StatementResultService statementResultService)
        {
            this.desc = desc;
            this.eventAdapterService = eventAdapterService;
            this.variableService = variableService;
            this.statementResultService = statementResultService;

            Map<String, Object> variableTypes = new HashMap<String, Object>();
            readers = new VariableReader[desc.Assignments.Count];
            mustCoerce = new bool[desc.Assignments.Count];

            int count = 0;
            foreach (OnTriggerSetAssignment assignment in desc.Assignments) {
                String variableName = assignment.VariableName;
                readers[count] = variableService.GetReader(variableName);
                if (readers[count] == null) {
                    throw new ExprValidationException("Variable by name '" + variableName +
                                                      "' has not been created or configured");
                }

                // determine types
                Type variableType = readers[count].VariableType;
                Type expressionType = assignment.Expression.ReturnType;
                variableTypes.Put(variableName, variableType);

                // determine if the expression type can be assigned
                if ((TypeHelper.GetBoxedType(expressionType) != variableType) &&
                    (expressionType != null)) {
                    if ((!TypeHelper.IsNumeric(variableType)) ||
                        (!TypeHelper.IsNumeric(expressionType))) {
                        throw new ExprValidationException("Variable '" + variableName
                                                          + "' of declared type '" + variableType.FullName +
                                                          "' cannot be assigned a value of type '" +
                                                          expressionType.FullName + "'");
                    }

                    if (!(TypeHelper.CanCoerce(expressionType, variableType))) {
                        throw new ExprValidationException("Variable '" + variableName
                                                          + "' of declared type '" + variableType.FullName +
                                                          "' cannot be assigned a value of type '" +
                                                          expressionType.FullName + "'");
                    }

                    mustCoerce[count] = true;
                }
                count++;
            }
            eventType = eventAdapterService.CreateAnonymousMapType(variableTypes);
        }

        public override void Update(EventBean[] newData, EventBean[] oldData)
	    {
	        if (ExecutionPathDebugLog.isDebugEnabled && log.IsDebugEnabled)
	        {
	            log.Debug(".update Received update, " +
	                    "  newData.length==" + ((newData == null) ? 0 : newData.Length) +
	                    "  oldData.length==" + ((oldData == null) ? 0 : oldData.Length));
	        }

	        if ((newData == null) || (newData.Length == 0))
	        {
	            return;
	        }

	        Map<String, Object> values = null;
            bool produceOutputEvents = (statementResultService.IsMakeNatural || statementResultService.IsMakeSynthetic);
            if (produceOutputEvents)
            {
                values = new HashMap<String, Object>();
	        }

	        eventsPerStream[0] = newData[newData.Length - 1];
	        int count = 0;

	        // We obtain a write lock global to the variable space
	        // Since expressions can contain variables themselves, these need to be unchangeable for the duration
	        // as there could be multiple statements that do "var1 = var1 + 1".
            using( new WriterLock(variableService.ReadWriteLock))
            {
                try
                {
                    variableService.SetLocalVersion();

                    foreach (OnTriggerSetAssignment assignment in desc.Assignments)
                    {
                        VariableReader reader = readers[count];
                        Object value = assignment.Expression.Evaluate(eventsPerStream, true);
                        if ((value != null) && (mustCoerce[count]))
                        {
                            value = TypeHelper.CoerceBoxed(value, reader.VariableType);
                        }

                        variableService.Write(reader.VariableNumber, value);
                        count++;

                        if (values != null)
                        {
                            values.Put(assignment.VariableName, value);
                        }
                    }

                    variableService.Commit();
                }
                catch (Exception ex)
                {
                    log.Error("Error evaluating on-set variable expressions: " + ex.Message, ex);
                    variableService.Rollback();
                    throw;
                }
            }

	        if (values != null)
	        {
	            EventBean[] newDataOut = new EventBean[1];
	            newDataOut[0] = eventAdapterService.CreateMapFromValues(values, eventType);
	            this.UpdateChildren(newDataOut, null);
	        }
	    }

        public override EventType EventType
	    {
            get { return eventType; }
	    }

	    public override IEnumerator<EventBean> GetEnumerator()
	    {
            Map<String, Object> values = new HashMap<String, Object>();

	        int count = 0;
	        foreach (OnTriggerSetAssignment assignment in desc.Assignments)
	        {
	            Object value = readers[count].GetValue();
	            values.Put(assignment.VariableName, value);
	            count++;
	        }

	        EventBean @event = eventAdapterService.CreateMapFromValues(values, eventType);
            if ( @event != null )
            {
                yield return @event;
            }
	    }
	}
} // End of namespace
