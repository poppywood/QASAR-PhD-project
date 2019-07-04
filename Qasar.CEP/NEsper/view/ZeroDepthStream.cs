using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.events;
using net.esper.util;

using LogFactory = org.apache.commons.logging.LogFactory;
using Log = org.apache.commons.logging.Log;

namespace net.esper.view
{
	/// <summary>
    /// Event stream implementation that does not keep any window by itself of the events
    /// coming into the stream.
    /// </summary>
    
    public sealed class ZeroDepthStream : EventStream
    {
	    private readonly bool isDebugEnabled;
        private readonly ELinkedList<View> children = new ELinkedList<View>();
        private readonly EventType eventType;
        private EventBean lastInsertedEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeroDepthStream"/> class.
        /// </summary>
        /// <param name="eventType">type of event</param>

        public ZeroDepthStream(EventType eventType)
        {
            this.isDebugEnabled = ExecutionPathDebugLog.IsEnabled && log.IsDebugEnabled;
            this.eventType = eventType;
        }

        /// <summary>
        /// Inserts the specified event.
        /// </summary>
        /// <param name="ev">The ev.</param>
        
        public void Insert(EventBean ev)
        {
            if (isDebugEnabled)
            {
                log.Debug(".Insert Received event, updating child views, event=" + ev);
            }

			// Get a new array created rather then re-use the old one since some client listeners
			// to this view may keep reference to the new data
			EventBean[] row = new EventBean[]{ev};

            foreach( View view in children )
			{
				view.Update(row, null);
            }

            lastInsertedEvent = ev;
        }

        /// <summary>
        /// Provides metadata information about the type of object the event collection contains.
        /// </summary>
        /// <value></value>
        /// <returns> metadata for the objects in the collection
        /// </returns>
        public EventType EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<EventBean> GetEnumerator()
        {
            if (lastInsertedEvent != null)
            {
                yield return lastInsertedEvent;
            }
        }

        /// <summary>
        /// Add a view to the viewable object.
        /// </summary>
        /// <param name="view">to add</param>
        /// <returns>view to add</returns>
        public View AddView(View view)
        {
            log.Debug(".AddView Adding view to children");
            //log.Debug(".AddView Adding view to children: " + (new System.Diagnostics.StackTrace()).ToString());

            children.Add(view);
            view.Parent = this;
            return view;
        }

        /// <summary>
        /// Returns all added views.
        /// </summary>
        /// <returns>list of added views</returns>
        public IList<View> Views
        {
        	get { return children; }
        }

        /// <summary>
        /// Remove a view.
        /// </summary>
        /// <param name="view">to remove</param>
        /// <returns>
        /// true to indicate that the view to be removed existed within this view, false if the view to
        /// remove could not be found
        /// </returns>
        public bool RemoveView(View view)
        {
            log.Debug(".RemoveView Adding view to children");

            bool isRemoved = children.Remove(view);
            view.Parent = null;
            return isRemoved;
        }

        /// <summary>
        /// Test is there are any views to the Viewable.
        /// </summary>
        /// <value></value>
        /// <returns> true indicating there are child views, false indicating there are no child views
        /// </returns>
        public bool HasViews
        {
            get { return (children.Count > 0); }
        }

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
