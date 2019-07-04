///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;

namespace com.espertech.esper.core
{
    /// <summary>Event indicating statement lifecycle management. </summary>
    public class StatementLifecycleEvent
    {
        private readonly EPStatement statement;
        private readonly LifecycleEventType eventType;
        private readonly Object[] paramList;
    
        /// <summary>Event types. </summary>
        public enum LifecycleEventType {
            /// <summary>Statement created. </summary>
            CREATE,
            /// <summary>Statement state change. </summary>
            STATECHANGE,
            /// <summary>listener added </summary>
            LISTENER_ADD,
            /// <summary>Listener removed. </summary>
            LISTENER_REMOVE,
            /// <summary>All listeners removed. </summary>
            LISTENER_REMOVE_ALL
        }

        /// <summary>Ctor. </summary>
        /// <param name="statement">the statement</param>
        /// <param name="eventType">the tyoe if event</param>
        /// <param name="paramList">event parameters</param>
        internal StatementLifecycleEvent(EPStatement statement, LifecycleEventType eventType, params Object[] paramList)
        {
            this.statement = statement;
            this.eventType = eventType;
            this.paramList = paramList;
        }
    
        /// <summary>Returns the statement instance for the event. </summary>
        /// <returns>statement</returns>
        public EPStatement Statement
        {
            get { return statement; }
        }
    
        /// <summary>Returns the event type. </summary>
        /// <returns>type of event</returns>
        public LifecycleEventType EventType {
            get { return eventType; }
        }
    
        /// <summary>Returns event parameters. </summary>
        /// <returns>paramList</returns>
        public Object[] ParamList {
            get { return paramList; }
        }
    }
}
