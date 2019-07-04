///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.util;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.core
{
    /// <summary>Implements tracking of statement listeners and subscribers for a given statement such as to efficiently dispatch in situations where 0, 1 or more listeners are attached and/or 0 or 1 subscriber (such as iteration-only statement). </summary>
    public class StatementResultServiceImpl : StatementResultService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
        // Part of the statement context
        private EPStatementSPI epStatement;
        private EPServiceProvider epServiceProvider;
        private bool isInsertInto;
        private bool isPattern;
        private StatementLifecycleSvc statementLifecycleSvc;
    
        // For natural delivery derived out of select-clause expressions
        private Type[] selectClauseTypes;
        private String[] selectClauseColumnNames;
    
        // Listeners and subscribers and derived information
        private EPStatementListenerSet statementListenerSet;
        private bool isMakeNatural;
        private bool isMakeSynthetic;
        private ResultDeliveryStrategy statementResultNaturalStrategy;
    
        // For iteration over patterns
        private EventBean lastIterableEvent;

        /// <summary>Buffer for holding dispatchable events. </summary>
        protected ThreadLocal<LinkedList<UniformPair<EventBean[]>>> lastResults =
            new FastThreadLocal<LinkedList<UniformPair<EventBean[]>>>(CreateLocalData);
        protected static LinkedList<UniformPair<EventBean[]>> CreateLocalData()
        {
            return new LinkedList<UniformPair<EventBean[]>>();
        }
    
        /// <summary>Ctor. </summary>
        /// <param name="statementLifecycleSvc">handles persistence for statements</param>
        public StatementResultServiceImpl(StatementLifecycleSvc statementLifecycleSvc)
        {
            log.Debug(".ctor");
            this.statementLifecycleSvc = statementLifecycleSvc;
        }
    
        public void SetContext(EPStatementSPI epStatement, EPServiceProvider epServiceProvider, bool isInsertInto, bool isPattern)
        {
            this.epStatement = epStatement;
            this.epServiceProvider = epServiceProvider;
            this.isInsertInto = isInsertInto;
            this.isPattern = isPattern;
            isMakeSynthetic = isInsertInto || isPattern;
        }
    
        public void SetSelectClause(Type[] selectClauseTypes, String[] selectClauseColumnNames)
        {
            if ((selectClauseTypes == null) || (selectClauseTypes.Length == 0))
            {
                throw new ArgumentException("Invalid null or zero-element list of select clause expression types");
            }
            if ((selectClauseColumnNames == null) || (selectClauseColumnNames.Length == 0))
            {
                throw new ArgumentException("Invalid null or zero-element list of select clause column names");
            }
            this.selectClauseTypes = selectClauseTypes;
            this.selectClauseColumnNames = selectClauseColumnNames;
        }
    
        public bool IsMakeSynthetic
        {
            get { return isMakeSynthetic; }
        }
    
        public bool IsMakeNatural
        {
            get { return isMakeNatural; }
        }
    
        public EventBean LastIterableEvent
        {
            get { return lastIterableEvent; }
        }
    
        public void SetUpdateListeners(EPStatementListenerSet statementListenerSet)
        {
            // indicate that listeners were updated for potential persistence of listener set, once the statement context is known
            if (epStatement != null)
            {
                this.statementLifecycleSvc.UpdatedListeners(epStatement.StatementId,
                                                            epStatement.Name,
                                                            statementListenerSet);
            }
    
            this.statementListenerSet = statementListenerSet;
    
            isMakeNatural = statementListenerSet.Subscriber != null;
            isMakeSynthetic = !(statementListenerSet.Listeners.IsEmpty &&
                                statementListenerSet.StmtAwareListeners.IsEmpty)
                              || isPattern || isInsertInto;
    
            if (statementListenerSet.Subscriber == null)
            {
                statementResultNaturalStrategy = null;
                isMakeNatural = false;
                return;
            }
    
            statementResultNaturalStrategy = ResultDeliveryStrategyFactory.Create(statementListenerSet.Subscriber,
                    selectClauseTypes, selectClauseColumnNames);
            isMakeNatural = true;
        }
    
        // Called by OutputProcessView
        public void Indicate(UniformPair<EventBean[]> results)
        {
            if (results != null)
            {
                if ((results.First != null) && (results.First.Length != 0))
                {
                    lastResults.GetOrCreate().AddLast(results);
                    lastIterableEvent = results.First[0];
                }
                else if ((results.Second != null) && (results.Second.Length != 0))
                {
                    lastResults.GetOrCreate().AddLast(results);
                }
            }
        }
    
        public void Execute()
        {
            LinkedList<UniformPair<EventBean[]>> dispatches = lastResults.GetOrCreate();
            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                log.Debug(".execute dispatches: " + dispatches.Count);
            }
    
            UniformPair<EventBean[]> events = EventBeanUtility.FlattenList(dispatches);
    
            if (log.IsDebugEnabled)
            {
                ViewSupport.DumpUpdateParams(".execute", events);
            }
    
            if (statementResultNaturalStrategy != null)
            {
                statementResultNaturalStrategy.Execute(events);
            }
    
            EventBean[] newEventArr = events != null ? events.First : null;
            EventBean[] oldEventArr = events != null ? events.Second : null;
    
            foreach (UpdateListener listener in statementListenerSet.Listeners)
            {
                listener.Update(newEventArr, oldEventArr);
            }
            if (statementListenerSet.StmtAwareListeners.Count != 0)
            {
                foreach (StatementAwareUpdateListener listener in statementListenerSet.StmtAwareListeners)
                {
                    listener.Update(newEventArr, oldEventArr, epStatement, epServiceProvider);
                }
            }
    
            dispatches.Clear();
        }
    
        /// <summary>Dispatches when the statement is stopped any remaining results. </summary>
        public void DispatchOnStop()
        {
            lastIterableEvent = null;
            LinkedList<UniformPair<EventBean[]>> dispatches = lastResults.GetOrCreate();
            if (dispatches.First == null)
            {
                return;
            }
            Execute();
        }
    }
}
