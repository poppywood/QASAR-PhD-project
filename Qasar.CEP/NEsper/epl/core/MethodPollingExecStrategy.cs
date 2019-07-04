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
using com.espertech.esper.compat;
using com.espertech.esper.epl.db;
using com.espertech.esper.events;

using CGLib;

using log4net;

namespace com.espertech.esper.epl.core
{
    /// <summary>Viewable providing historical data from a database.</summary>
	public class MethodPollingExecStrategy : PollExecStrategy
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly EventAdapterService eventAdapterService;
	    private readonly FastMethod method;
	    private readonly bool isArray;
        private readonly bool useMapType;
        private readonly EventType eventType;

        /// <summary>Ctor.</summary>
        /// <param name="eventAdapterService">for generating event beans</param>
        /// <param name="method">the method to invoke</param>
        /// <param name="useMapType">is true to indicate that Map-events are generated</param>
        /// <param name="eventType">is the event type to use</param>
        public MethodPollingExecStrategy(EventAdapterService eventAdapterService,
                                         FastMethod method,
                                         bool useMapType,
                                         EventType eventType)
        {
            this.eventAdapterService = eventAdapterService;
            this.method = method;
            this.isArray = method.ReturnType.IsArray;
            this.useMapType = useMapType;
            this.eventType = eventType;
        }

        /// <summary>
        /// Start the poll, called before any poll operation.
        /// </summary>
	    public void Start()
	    {
	    }

        /// <summary>
        /// Indicate we are done polling and can release resources.
        /// </summary>
	    public void Done()
	    {
	    }

        /// <summary>
        /// Indicate we are no going to use this object again.
        /// </summary>
	    public void Destroy()
	    {
	    }

        /// <summary>
        /// Poll events using the keys provided.
        /// </summary>
        /// <param name="lookupValues">is keys for exeuting a query or such</param>
        /// <returns>a list of events for the keys</returns>
	    public IList<EventBean> Poll(Object[] lookupValues)
	    {
	        IList<EventBean> rowResult = null;
            try
            {
                Object invocationResult = method.Invoke(null, lookupValues);
                if (invocationResult != null)
                {
                    if (isArray)
                    {
                        Array tempArray = (Array)invocationResult;
                        int length = tempArray.Length;
                        if (length > 0)
                        {
                            rowResult = new List<EventBean>();
                            for (int i = 0; i < length; i++)
                            {
                                Object value = tempArray.GetValue(i);
                                if (value == null)
                                {
                                    log.Warn("Expected non-null return result from method '" + method.Name +
                                             "', but received null value");
                                    continue;
                                }

                                EventBean @event;
                                if (useMapType)
                                {
                                    if (!(value is Map<string, object>))
                                    {
                                        log.Warn("Expected Map-type return result from method '" + method.Name +
                                                 "', but received type '" + value.GetType() + "'");
                                        continue;
                                    }
                                    Map<string, object> mapValues = (Map<string, object>)value;
                                    @event = eventAdapterService.CreateMapFromValues(mapValues, eventType);
                                }
                                else
                                {
                                    @event = eventAdapterService.AdapterForBean(value);
                                }
                                rowResult.Add(@event);
                            }
                        }
                    }
                    else
                    {
                        rowResult = new List<EventBean>();
                        EventBean @event;
                        if (useMapType)
                        {
                            if (!(invocationResult is Map<string, object>))
                            {
                                log.Warn("Expected Map-type return result from method '" + method.Name +
                                         "', but received type '" + invocationResult.GetType() + "'");
                            }
                            else
                            {
                                Map<string, object> mapValues = (Map<string, object>)invocationResult;
                                @event = eventAdapterService.CreateMapFromValues(mapValues, eventType);
                                rowResult.Add(@event);
                            }
                        }
                        else
                        {
                            @event = eventAdapterService.AdapterForBean(invocationResult);
                            rowResult.Add(@event);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new EPException("Method '" + method.Name + "' of class '" + method.Target.DeclaringType.FullName +
                                      "' reported an exception: " + ex.GetType() + ": " + ex.Message,
                                      ex);
            }

	        return rowResult;
	    }
	}
} // End of namespace
