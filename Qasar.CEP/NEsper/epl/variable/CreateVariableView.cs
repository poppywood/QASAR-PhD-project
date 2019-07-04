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
    /// View for handling create-variable syntax.
    /// <para/>
    /// The view posts to listeners when a variable changes, if it has subviews.
    /// <para/>
    /// The view returns the current variable value for the iterator.
    /// <para/>
    /// The event type for such posted events is a single field Map with the variable value.
    /// </summary>
	public class CreateVariableView : ViewSupport, VariableChangeCallback
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    private readonly EventAdapterService eventAdapterService;
	    private readonly VariableReader reader;
	    private readonly EventType eventType;
	    private readonly String variableName;
        private readonly StatementResultService statementResultService;

        /// <summary>Ctor.</summary>
        /// <param name="eventAdapterService">for creating events</param>
        /// <param name="variableService">for looking up variables</param>
        /// <param name="variableName">is the name of the variable to create</param>
        /// <param name="statementResultService">for coordinating on whether insert and remove stream events should be posted</param>
        public CreateVariableView(EventAdapterService eventAdapterService, VariableService variableService, String variableName, StatementResultService statementResultService)
        {
            this.eventAdapterService = eventAdapterService;
            this.variableName = variableName;
            this.statementResultService = statementResultService;
            reader = variableService.GetReader(variableName);

            Map<String, Object> variableTypes = new HashMap<String, Object>();
            variableTypes.Put(variableName, reader.VariableType);
            eventType = eventAdapterService.CreateAnonymousMapType(variableTypes);
        }

	    public void Update(Object newValue, Object oldValue)
	    {
            if (statementResultService.IsMakeNatural || statementResultService.IsMakeSynthetic)
            {
                Map<String, Object> valuesOld = new HashMap<String, Object>();
	            valuesOld.Put(variableName, oldValue);
	            EventBean eventOld = eventAdapterService.CreateMapFromValues(valuesOld, eventType);

                Map<String, Object> valuesNew = new HashMap<String, Object>();
	            valuesNew.Put(variableName, newValue);
	            EventBean eventNew = eventAdapterService.CreateMapFromValues(valuesNew, eventType);

	            this.UpdateChildren(new EventBean[] {eventNew}, new EventBean[] {eventOld});
	        }
	    }

	    public override void Update(EventBean[] newData, EventBean[] oldData)
	    {
	        throw new UnsupportedOperationException("Update not supported");
	    }

	    public override EventType EventType
	    {
            get { return eventType; }
	    }

	    public override IEnumerator<EventBean> GetEnumerator()
	    {
	        Object value = reader.GetValue();
            Map<String, Object> values = new HashMap<String, Object>();
	        values.Put(variableName, value);
	        EventBean @event = eventAdapterService.CreateMapFromValues(values, eventType);
            if ( @event != null )
            {
                yield return @event;
            }
	    }
	}
} // End of namespace
