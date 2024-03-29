using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using net.esper.compat;
using net.esper.collection;
using net.esper.core;
using net.esper.events;
using net.esper.view;

namespace net.esper.view.std
{
    /// <summary>
    /// This view is a very simple view presenting the number of elements in a stream or view.
    /// The view computes a single long-typed count of the number of events passed through it similar
    /// to the base statistics COUNT column.
    /// </summary>

    public sealed class SizeView
		: ViewSupport
		, CloneableView
    {
        private StatementContext statementContext;
        private EventType eventType;
        private long size = 0;
        private EventBean lastSizeEvent;

	    /// <summary>Ctor.</summary>
	    /// <param name="statementContext">is services</param>
	    public SizeView(StatementContext statementContext)
	    {
	        this.statementContext = statementContext;
	        this.eventType = CreateEventType(statementContext);
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
			return new SizeView(statementContext);
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
        /// Notify that data has been added or removed from the Viewable parent.
        /// The last object in the newData array of objects would be the newest object added to the parent view.
        /// The first object of the oldData array of objects would be the oldest object removed from the parent view.
        /// If the call to update contains new (inserted) data, then the first argument will be a non-empty list and the
        /// second will be empty. Similarly, if the call is a notification of deleted data, then the first argument will be
        /// empty and the second will be non-empty. Either the newData or oldData will be non-null.
        /// This method won't be called with both arguments being null, but either one could be null.
        /// The same is true for zero-length arrays. Either newData or oldData will be non-empty.
        /// If both are non-empty, then the update is a modification notification.
        /// When update() is called on a view by the parent object, the data in newData will be in the collection of the
        /// parent, and its data structures will be arranged to reflect that.
        /// The data in oldData will not be in the parent's data structures, and any access to the parent will indicate that
        /// that data is no longer there.
        /// </summary>
        /// <param name="newData">is the new data that has been added to the parent view</param>
        /// <param name="oldData">is the old data that has been removed from the parent view</param>
        public override void Update(EventBean[] newData, EventBean[] oldData)
        {
            long priorSize = size;

            // add data points to the window
            if (newData != null)
            {
                size += newData.Length;
            }

            if (oldData != null)
            {
                size -= oldData.Length;
            }

            // If there are child views, fireStatementStopped update method
            if ((this.HasViews) && (priorSize != size))
            {
                DataDictionary postNewData = new DataDictionary();
                postNewData.Put(ViewFieldEnum.SIZE_VIEW__SIZE.Name, size);
                EventBean newEvent = statementContext.EventAdapterService.CreateMapFromValues(postNewData, eventType);

                if (lastSizeEvent != null)
                {
                    UpdateChildren(new EventBean[] { newEvent }, new EventBean[] { lastSizeEvent });
                }
                else
                {
                    DataDictionary postOldData = new DataDictionary();
                    postOldData.Put(ViewFieldEnum.SIZE_VIEW__SIZE.Name, priorSize);
                    EventBean oldEvent = statementContext.EventAdapterService.CreateMapFromValues(postOldData, eventType);

                    UpdateChildren(new EventBean[] { newEvent }, new EventBean[] { oldEvent });
                }

                lastSizeEvent = newEvent;
            }  
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public override IEnumerator<EventBean> GetEnumerator()
        {
            DataDictionary current = new DataDictionary();
            current[ViewFieldEnum.SIZE_VIEW__SIZE.Name] = size;
            yield return statementContext.EventAdapterService.CreateMapFromValues(current, eventType);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return this.GetType().FullName;
        }

		/// <summary>Creates the event type for this view</summary>
		/// <param name="statementContext">is the event adapter service</param>
		/// <returns>event type for view</returns>
	    public static EventType CreateEventType(StatementContext statementContext)
	    {
	        EDictionary<String, Type> schemaMap = new HashDictionary<String, Type>();
		    schemaMap[ViewFieldEnum.SIZE_VIEW__SIZE.Name] = typeof (long);
	        return statementContext.EventAdapterService.CreateAnonymousMapType(schemaMap);
	    }
    }
}
