///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.join;
using com.espertech.esper.events;
using com.espertech.esper.view;

namespace com.espertech.esper.epl.view
{
    /// <summary>
    /// Base output processing view that has the responsibility to serve up event type and statement
    /// iterator.
    /// <para/>
    /// Implementation classes may enforce an output rate stabilizing or limiting policy.
    /// </summary>
    public abstract class OutputProcessView : View, JoinSetIndicator
    {
        /// <summary>Processes the parent views result set generating events for pushing out to child view. </summary>
        protected readonly ResultSetProcessor resultSetProcessor;
        private JoinExecutionStrategy joinExecutionStrategy;

        /// <summary>Strategy to performs the output once it's decided we need to output. </summary>
        protected readonly OutputStrategy outputStrategy;

        /// <summary>Manages listeners/subscribers to a statement, informing about current result generation needs. </summary>
        protected readonly StatementResultService statementResultService;

        /// <summary>The view to ultimately dispatch to. </summary>
        protected UpdateDispatchView childView;

        /// <summary>The parent view for iteration. </summary>
        protected Viewable parentView;

        /// <summary>An indicator on whether we always need synthetic events such as for insert-into. </summary>
        protected bool isGenerateSynthetic;

        /// <summary>Ctor. </summary>
        /// <param name="resultSetProcessor">processes the results posted by parent view or joins</param>
        /// <param name="outputStrategy">the strategy to use for producing output</param>
        /// <param name="isInsertInto">true if this is an insert-into</param>
        /// <param name="statementResultService">for awareness of listeners and subscriber</param>
        protected OutputProcessView(ResultSetProcessor resultSetProcessor, OutputStrategy outputStrategy, bool isInsertInto, StatementResultService statementResultService)
        {
            this.resultSetProcessor = resultSetProcessor;
            this.outputStrategy = outputStrategy;
            this.statementResultService = statementResultService;

            // by default, generate synthetic events only if we insert-into
            this.isGenerateSynthetic = isInsertInto;
        }

        public Viewable Parent
        {
            get { return parentView; }
            set { this.parentView = value; }
        }

        public View AddView(View view)
        {
            if (childView != null)
            {
                throw new IllegalStateException("Child view has already been supplied");
            }
            childView = (UpdateDispatchView)view;
            return this;
        }

        public IList<View> Views
        {
            get
            {
                List<View> views = new List<View>();
                if (childView != null) {
                    views.Add(childView);
                }
                return views;
            }
        }

        public bool RemoveView(View view)
        {
            if (view != childView)
            {
                throw new IllegalStateException("Cannot remove child view, view has not been supplied");
            }
            childView = null;
            return true;
        }

        public virtual bool HasViews
        {
            get { return childView != null; }
        }

        public EventType EventType
        {
            get
            {
                EventType eventType = resultSetProcessor.ResultEventType;
                if (eventType != null) {
                    return eventType;
                }
                return parentView.EventType;
            }
        }

        /// <summary>
        /// For joins, supplies the join execution strategy that provides iteration over statement results.
        /// </summary>
        /// <value>The join execution strategy.</value>
        public JoinExecutionStrategy JoinExecutionStrategy
        {
            get { return this.joinExecutionStrategy; }
            set { this.joinExecutionStrategy = value; }
        }

        /// <summary>
        /// Notify that data has been added or removed from the Viewable parent.
        /// The last object in the newData array of objects would be the newest object added to the parent view.
        /// The first object of the oldData array of objects would be the oldest object removed from the parent view.
        /// <para>
        /// If the call to update contains new (inserted) data, then the first argument will be a non-empty list and the
        /// second will be empty. Similarly, if the call is a notification of deleted data, then the first argument will be
        /// empty and the second will be non-empty. Either the newData or oldData will be non-null.
        /// This method won't be called with both arguments being null, but either one could be null.
        /// The same is true for zero-length arrays. Either newData or oldData will be non-empty.
        /// If both are non-empty, then the update is a modification notification.
        /// </para>
        /// <para>
        /// When update() is called on a view by the parent object, the data in newData will be in the collection of the
        /// parent, and its data structures will be arranged to reflect that.
        /// The data in oldData will not be in the parent's data structures, and any access to the parent will indicate that
        /// that data is no longer there.
        /// </para>
        /// </summary>
        /// <param name="newData">is the new data that has been added to the parent view
        /// </param>
        /// <param name="oldData">is the old data that has been removed from the parent view
        /// </param>
        public virtual void Update(EventBean[] newData, EventBean[] oldData)
        {
            throw new NotImplementedException();
        }

        /// <summary> Process join result set.</summary>
        /// <param name="newEvents">set of event tuples representing new data
        /// </param>
        /// <param name="oldEvents">set of event tuples representing old data
        /// </param>
        public virtual void Process(Set<MultiKey<EventBean>> newEvents, Set<MultiKey<EventBean>> oldEvents)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (joinExecutionStrategy != null)
            {
                Set<MultiKey<EventBean>> joinSet = joinExecutionStrategy.StaticJoin();
                return resultSetProcessor.GetEnumerator(joinSet);
            }
            if (resultSetProcessor != null)
            {
                return resultSetProcessor.GetEnumerator(parentView);
            }
            else
            {
                return parentView.GetEnumerator();
            }
        }

        public virtual IEnumerator<EventBean> GetEnumerator()
        {
            if (joinExecutionStrategy != null)
            {
                Set<MultiKey<EventBean>> joinSet = joinExecutionStrategy.StaticJoin();
                return resultSetProcessor.GetEnumerator(joinSet);
            }
            if (resultSetProcessor != null)
            {
                return resultSetProcessor.GetEnumerator(parentView);
            }
            else
            {
                return parentView.GetEnumerator();
            }
        }
    }
}
