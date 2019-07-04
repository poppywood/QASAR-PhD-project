///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.core;
using com.espertech.esper.compat;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.util;
using com.espertech.esper.view;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// This service hold for each named window a dedicated processor and a lock to the named window.
    /// This lock is shrared between the named window and on-delete statements.
    /// </summary>
	public class NamedWindowServiceImpl : NamedWindowServiceConstants, NamedWindowService
	{
	    private readonly Map<String, NamedWindowProcessor> processors;
	    private readonly Map<String, ManagedLock> windowStatementLocks;
	    private readonly StatementLockFactory statementLockFactory;
	    private readonly VariableService variableService;

        private ThreadLocal<List<NamedWindowConsumerDispatchUnit>> threadLocal;
	    private static List<NamedWindowConsumerDispatchUnit> CreateDispatchUnitList()
	    {
	        return new List<NamedWindowConsumerDispatchUnit>(100);
	    }

        private ThreadLocal<Map<EPStatementHandle, Object>> dispatchesPerStmtTL;
        private static Map<EPStatementHandle, Object> CreateDispatchTable()
        {
            return new HashMap<EPStatementHandle, Object>();
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="statementLockFactory">statement lock factory</param>
        /// <param name="variableService">is for variable access</param>
	    public NamedWindowServiceImpl(
            StatementLockFactory statementLockFactory, 
            VariableService variableService)
	    {
            this.threadLocal = new FastThreadLocal<List<NamedWindowConsumerDispatchUnit>>(CreateDispatchUnitList);
            this.dispatchesPerStmtTL = new FastThreadLocal<Map<EPStatementHandle, Object>>(CreateDispatchTable);

	        this.processors = new HashMap<String, NamedWindowProcessor>();
	        this.windowStatementLocks = new HashMap<String, ManagedLock>();
	        this.statementLockFactory = statementLockFactory;
	        this.variableService = variableService;
	    }

	    public void Destroy()
	    {
	        processors.Clear();
	        threadLocal = null;
	        dispatchesPerStmtTL = null;
	    }

        /// <summary>
        /// Returns the names of all named windows known.
        /// </summary>
        /// <value></value>
        /// <returns>named window names</returns>
        public ICollection<String> NamedWindows
        {
            get { return new List<string>(processors.Keys); }
        }
        
        public ManagedLock GetNamedWindowLock(String windowName)
	    {
	        return windowStatementLocks.Get(windowName);
	    }

	    public void AddNamedWindowLock(String windowName, ManagedLock statementResourceLock)
	    {
	        windowStatementLocks.Put(windowName, statementResourceLock);
	    }

	    public bool IsNamedWindow(String name)
	    {
	        return processors.ContainsKey(name);
	    }

	    public NamedWindowProcessor GetProcessor(String name)
	    {
	        NamedWindowProcessor processor = processors.Get(name);
	        if (processor == null)
	        {
	            throw new IllegalStateException("A named window by name '" + name + "' does not exist");
	        }
	        return processor;
	    }

        public NamedWindowProcessor AddProcessor(String name,
                                                 EventType eventType,
                                                 EPStatementHandle createWindowStmtHandle,
                                                 StatementResultService statementResultService,
                                                 ValueAddEventProcessor revisionProcessor)
	    {
	        if (processors.ContainsKey(name))
	        {
	            throw new ViewProcessingException("A named window by name '" + name + "' has already been created");
	        }

            NamedWindowProcessor processor = new NamedWindowProcessor(this, name, eventType, createWindowStmtHandle, statementResultService, revisionProcessor);
	        processors.Put(name, processor);

	        return processor;
	    }

	    public void RemoveProcessor(String name)
	    {
	        NamedWindowProcessor processor = processors.Get(name);
	        if (processor != null)
	        {
	            processor.Destroy();
	            processors.Remove(name);
	        }
	    }

	    public void AddDispatch(NamedWindowDeltaData delta, Map<EPStatementHandle, IList<NamedWindowConsumerView>> consumers)
	    {
	        NamedWindowConsumerDispatchUnit unit = new NamedWindowConsumerDispatchUnit(delta, consumers);
	        threadLocal.GetOrCreate().Add(unit);
	    }

	    public bool Dispatch()
	    {
            ThreadLocal<List<NamedWindowConsumerDispatchUnit>> lthreadLocal = threadLocal;
            if (lthreadLocal == null)
            {
                return false;
            }

            List<NamedWindowConsumerDispatchUnit> dispatches = lthreadLocal.GetOrCreate();
	        if (dispatches.Count == 0)
	        {
	            return false;
	        }

	        if (dispatches.Count == 1)
	        {
	            NamedWindowConsumerDispatchUnit unit = dispatches[0];
	            EventBean[] newData = unit.DeltaData.NewData;
	            EventBean[] oldData = unit.DeltaData.OldData;

	            foreach (KeyValuePair<EPStatementHandle, IList<NamedWindowConsumerView>> entry in unit.DispatchTo)
	            {
	                EPStatementHandle handle = entry.Key;
	                using(handle.StatementLock.AcquireLock(statementLockFactory))
	                {
	                    if (handle.HasVariables)
	                    {
	                        variableService.SetLocalVersion();
	                    }

	                    foreach (NamedWindowConsumerView consumerView in entry.Value)
	                    {
	                        consumerView.Update(newData, oldData);
	                    }

	                    // internal join processing, if applicable
	                    handle.InternalDispatch();
	                }
	            }

	            dispatches.Clear();
	            return true;
	        }

	        // Multiple different-result dispatches to same or different statements are needed in two situations:
	        // a) an event comes in, triggers two insert-into statements inserting into the same named window and the window produces 2 results
	        // b) a time batch is grouped in the named window, and a timer fires for both groups at the same time producing more then one result

	        // Most likely all dispatches go to different statements since most statements are not joins of
	        // named windows that produce results at the same time. Therefore sort by statement handle.
	        Map<EPStatementHandle, Object> dispatchesPerStmt = dispatchesPerStmtTL.GetOrCreate();
	        foreach (NamedWindowConsumerDispatchUnit unit in dispatches)
	        {
	            foreach (KeyValuePair<EPStatementHandle, IList<NamedWindowConsumerView>> entry in unit.DispatchTo)
	            {
	                EPStatementHandle handle = entry.Key;
	                Object perStmtObj = dispatchesPerStmt.Get(handle);
	                if (perStmtObj == null)
	                {
	                    dispatchesPerStmt.Put(handle, unit);
	                }
	                else if (perStmtObj is IList<NamedWindowConsumerDispatchUnit>)
	                {
	                    IList<NamedWindowConsumerDispatchUnit> list = (IList<NamedWindowConsumerDispatchUnit>) perStmtObj;
	                    list.Add(unit);
	                }
	                else    // convert from object to list
	                {
	                    NamedWindowConsumerDispatchUnit unitObj = (NamedWindowConsumerDispatchUnit) perStmtObj;
	                    List<NamedWindowConsumerDispatchUnit> list = new List<NamedWindowConsumerDispatchUnit>();
	                    list.Add(unitObj);
	                    list.Add(unit);
	                    dispatchesPerStmt.Put(handle, list);
	                }
	            }
	        }

	        // Dispatch
	        foreach (KeyValuePair<EPStatementHandle, Object> entry in dispatchesPerStmt)
	        {
	            EPStatementHandle handle = entry.Key;
	            Object perStmtObj = entry.Value;

	            // dispatch of a single result to the statement
                if (perStmtObj is NamedWindowConsumerDispatchUnit)
	            {
	                NamedWindowConsumerDispatchUnit unit = (NamedWindowConsumerDispatchUnit) perStmtObj;
	                EventBean[] newData = unit.DeltaData.NewData;
	                EventBean[] oldData = unit.DeltaData.OldData;

	                using(handle.StatementLock.AcquireLock(statementLockFactory))
	                {
	                    if (handle.HasVariables)
	                    {
	                        variableService.SetLocalVersion();
	                    }

	                    foreach (NamedWindowConsumerView consumerView in unit.DispatchTo.Get(handle))
	                    {
	                        consumerView.Update(newData, oldData);
	                    }

	                    // internal join processing, if applicable
	                    handle.InternalDispatch();
	                }

	                continue;
	            }

	            // dispatch of multiple results to a the same statement, need to aggregate per consumer view
	            IList<NamedWindowConsumerDispatchUnit> list = (IList<NamedWindowConsumerDispatchUnit>) perStmtObj;
	            Map<NamedWindowConsumerView, NamedWindowDeltaData> deltaPerConsumer = new LinkedHashMap<NamedWindowConsumerView, NamedWindowDeltaData>();
	            foreach (NamedWindowConsumerDispatchUnit unit in list)   // for each unit
	            {
	                foreach (NamedWindowConsumerView consumerView in unit.DispatchTo.Get(handle))   // each consumer
	                {
	                    NamedWindowDeltaData deltaForConsumer = deltaPerConsumer.Get(consumerView);
	                    if (deltaForConsumer == null)
	                    {
	                        deltaPerConsumer.Put(consumerView, unit.DeltaData);
	                    }
	                    else
	                    {
	                        NamedWindowDeltaData aggregated = new NamedWindowDeltaData(deltaForConsumer, unit.DeltaData);
	                        deltaPerConsumer.Put(consumerView, aggregated);
	                    }
	                }
	            }

	            using(handle.StatementLock.AcquireLock(statementLockFactory))
	            {
	                if (handle.HasVariables)
	                {
	                    variableService.SetLocalVersion();
	                }

	                foreach (KeyValuePair<NamedWindowConsumerView, NamedWindowDeltaData> entryDelta in deltaPerConsumer)
	                {
	                    EventBean[] newData = entryDelta.Value.NewData;
	                    EventBean[] oldData = entryDelta.Value.OldData;
	                    entryDelta.Key.Update(newData, oldData);
	                }

	                // internal join processing, if applicable
	                handle.InternalDispatch();
	            }
	        }

	        dispatches.Clear();
	        dispatchesPerStmt.Clear();

	        return true;
	    }
	}
} // End of namespace
