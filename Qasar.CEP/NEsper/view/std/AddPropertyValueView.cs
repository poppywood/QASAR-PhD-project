using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using net.esper.core;
using net.esper.compat;
using net.esper.events;
using net.esper.view;

using Log = org.apache.commons.logging.Log;
using LogFactory = org.apache.commons.logging.LogFactory;

namespace net.esper.view.std
{
    /// <summary>
    /// This view simply adds a property to the events posted to it. This is useful for the group-merge views.
    /// </summary>

    public sealed class AddPropertyValueView
		: ViewSupport
		, CloneableView
    {
	    private readonly StatementContext statementContext;
	    private readonly String[] propertyNames;
	    private readonly Object[] propertyValues;
	    private readonly EventType eventType;
	    private bool mustAddProperty;

        // Keep a history of posted old events to avoid reconstructing the event
        // and adhere to the contract of posting the same reference to child views
        private readonly EDictionary<EventBean, EventBean> newToOldEventMap;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="statementContext">contains required view services</param>
        /// <param name="fieldNames">is the name of the field that is added to any events received by this view.</param>
        /// <param name="mergeValues">is the values of the field that is added to any events received by this view.</param>
        /// <param name="mergedResultEventType">is the event type that the merge view reports to it's child views</param>
	    public AddPropertyValueView(StatementContext statementContext, String[] fieldNames, Object[] mergeValues, EventType mergedResultEventType)
	    {
	        this.propertyNames = fieldNames;
	        this.propertyValues = mergeValues;
	        this.eventType = mergedResultEventType;
	        this.statementContext = statementContext;
            this.newToOldEventMap = new HashDictionary<EventBean, EventBean>();
	    }

        /// <summary>
        /// Duplicates the view.
        /// <p>
        /// Expected to return a same view in initialized state for grouping.
        /// </p>
        /// </summary>
        /// <param name="statementContext">is services for the view</param>
        /// <returns>duplicated view</returns>
	    public View CloneView(StatementContext statementContext)
	    {
	        return new AddPropertyValueView(statementContext, propertyNames, propertyValues, eventType);
	    }

        /// <summary>
        /// Sets the View's parent Viewable.
        /// </summary>
        /// <value></value>
        /// <returns> viewable
        /// </returns>
        public override Viewable Parent
        {
            set
            {
                Viewable parent = value;

                if (log.IsDebugEnabled)
                {
                    log.Debug(".setParent parent=" + parent);
                }

                base.Parent = value;

		        if (parent.EventType != eventType)
		        {
		            mustAddProperty = true;
		        }
		        else
		        {
		            mustAddProperty = false;
		        }
            }
        }

        /// <summary>
        /// Gets or sets the field name for which to set the
        /// merge value for.
        /// </summary>

        public String[] PropertyNames
        {
        	get { return propertyNames; }
        }

        /// <summary>
        /// Gets or sets the value to set for the field
        /// </summary>

        public Object[] PropertyValues
        {
        	get { return propertyValues; }
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
        /// <param name="newData">is the new data that has been added to the parent view</param>
        /// <param name="oldData">is the old data that has been removed from the parent view</param>
        public override void Update(EventBean[] newData, EventBean[] oldData)
        {
            if (!mustAddProperty)
            {
                UpdateChildren(newData, oldData);
                return;
            }

            EventBean[] newEvents = null;
            EventBean[] oldEvents = null;

            if (newData != null)
            {
                newEvents = new EventBean[newData.Length];

                int index = 0;
                foreach (EventBean newEvent in newData)
                {
                    EventBean ev = AddProperty(newEvent, propertyNames, propertyValues, eventType, statementContext.EventAdapterService);
                    newEvents[index++] = ev;
                    newToOldEventMap.Put(newEvent, ev);
                }
            }

            if (oldData != null)
            {
                oldEvents = new EventBean[oldData.Length];

                int index = 0;
                foreach (EventBean oldEvent in oldData)
                {
                    EventBean outgoing;
                    if ( newToOldEventMap.Remove(oldEvent, out outgoing ) )
                    {
                        oldEvents[index++] = outgoing;
                    }
                    else
                    {
                        EventBean _event = AddProperty(oldEvent, propertyNames, propertyValues, eventType, statementContext.EventAdapterService);
                        oldEvents[index++] = _event;
                    }
                }
            }

            UpdateChildren(newEvents, oldEvents);
        }

        /// <summary>
        /// Provides metadata information about the type of object the event collection contains.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// metadata for the objects in the collection
        /// </returns>
        public override EventType EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public override IEnumerator<EventBean> GetEnumerator()
        {
            IEnumerator<EventBean> parentIterator = parent.GetEnumerator() ;
            while (parentIterator.MoveNext())
            {
                EventBean nextEvent = parentIterator.Current;
                if (this.mustAddProperty)
                {
                    EventBean ev = AddProperty(
                        nextEvent,
                        this.PropertyNames,
                        this.PropertyValues,
                        this.eventType,
                        this.statementContext.EventAdapterService);
                    yield return ev;
                }
                else
                {
                    yield return nextEvent;
                }
            }
        }

        /// <summary>
        /// Add a property to the event passed in.
        /// </summary>
        /// <param name="originalEvent">event to add property to</param>
        /// <param name="propertyNames">names of properties to add</param>
        /// <param name="propertyValues">value of properties to add</param>
        /// <param name="targetEventType">new event type</param>
        /// <param name="eventAdapterService">service for generating events and handling event types</param>param>
        /// <returns>event with added property</returns>

        public static EventBean AddProperty(
        	EventBean originalEvent,
            String[] propertyNames,
            Object[] propertyValues,
            EventType targetEventType,
            EventAdapterService eventAdapterService)
        {
            DataDictionary values = new DataDictionary();

            // Copy properties of original event, add property value
            foreach (String property in originalEvent.EventType.PropertyNames)
            {
                values[property] = originalEvent[property];
            }

            for (int i = 0; i < propertyNames.Length; i++)
            {
                values[propertyNames[i]] = propertyValues[i];
            }

            return eventAdapterService.CreateMapFromValues(values, targetEventType);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return
                this.GetType().Name +
                " propertyNames=" + CollectionHelper.Render(propertyNames) +
                " propertyValue=" + CollectionHelper.Render(propertyValues);
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
