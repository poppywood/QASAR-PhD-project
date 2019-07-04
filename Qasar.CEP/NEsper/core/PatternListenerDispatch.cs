using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.dispatch;
using com.espertech.esper.events;

namespace com.espertech.esper.core
{
	/// <summary>
	/// Dispatchable for dispatching events to update listeners.
	/// </summary>

	public class PatternListenerDispatch : Dispatchable
	{
		private readonly Set<UpdateEventHandler> eventHandlers;
		private EventBean singleEvent;
		private List<EventBean> eventList;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventHandlers">The event handlers.</param>

        public PatternListenerDispatch(Set<UpdateEventHandler> eventHandlers)
		{
            this.eventHandlers = eventHandlers;
		}

        /// <summary>
        /// Add an event to be dispatched.
        /// </summary>
        /// <param name="_event">event to add</param>

		public virtual void Add( EventBean _event )
		{
			if ( singleEvent == null )
			{
				singleEvent = _event;
			}
			else
			{
				if ( eventList == null )
				{
					eventList = new List<EventBean>( 5 );
					eventList.Add( singleEvent );
				}

				eventList.Add( _event );
			}
		}

        /// <summary>
        /// Fires the update event.
        /// </summary>
        /// <param name="newEvents">The new events.</param>
        /// <param name="oldEvents">The old events.</param>
        protected void FireUpdateEvent(EventBean[] newEvents, EventBean[] oldEvents)
        {
            foreach (UpdateEventHandler eventHandler in eventHandlers)
            {
                eventHandler(newEvents, oldEvents);
            }
        }

        /// <summary>
        /// Execute any listeners.
        /// </summary>
		public virtual void Execute()
		{
			EventBean[] eventArray;

			if ( eventList != null )
			{
                eventArray = eventList.ToArray();
				eventList = null;
				singleEvent = null;
			}
			else
			{
				eventArray = new EventBean[] { singleEvent };
				singleEvent = null;
			}

            FireUpdateEvent(eventArray, null);
		}

		/// <summary> Returns true if at least one event has been added.</summary>
		/// <returns> true if it has data, false if not
		/// </returns>

		public virtual bool HasData
		{
            get
            {
                if (singleEvent != null)
                {
                    return true;
                }
                return false;
            }
		}
	}
}