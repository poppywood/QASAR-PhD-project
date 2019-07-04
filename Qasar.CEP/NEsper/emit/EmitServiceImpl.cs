using System;
using System.Collections.Generic;
using System.Threading;

using com.espertech.esper.client;
using com.espertech.esper.compat;

namespace com.espertech.esper.emit
{
	/// <summary>
    /// Implementation of the event emit service.
    /// </summary>

    public class EmitServiceImpl : EmitService
	{
        /// <summary>
        /// Number of events emitted.
        /// </summary>
        /// <value></value>
        /// <returns> total of events emitted
        /// </returns>
		public long NumEventsEmitted
		{
            get { return Interlocked.Read( ref numEventsEmitted ); }
		}

		private readonly Map<String, IList<EmittedListener>> channelEmitListeners ;
		private readonly FastReaderWriterLock channelEmitListenersRWLock ;
		private long numEventsEmitted ;

        /// <summary>
        /// The default channel
        /// </summary>

        public const string DEFAULT_CHANNEL = "" ;
		
		/// <summary>
        /// Constructor.
        /// </summary>
		
        protected internal EmitServiceImpl()
		{
        	this.channelEmitListeners = new HashMap<String, IList<EmittedListener>>();
        	this.channelEmitListenersRWLock = new FastReaderWriterLock() ;
		}

        /// <summary>
        /// Add emitted event listener for the specified channel, or the default channel if the channel value is null.
        /// The listener will be invoked when an event is emitted on the subscribed channel. Listeners subscribed to the
        /// default channel are invoked for all emitted events regardless of what channel the event is emitted onto.
        /// </summary>
        /// <param name="listener">is the callback to receive when events are emitted</param>
        /// <param name="channel">is the channel to listen to, with null values allowed to indicate the default channel</param>
        public void AddListener(EmittedListener listener, String channel)
        {
            using (new WriterLock(channelEmitListenersRWLock))
            {
                // Check if the listener already exists, to make sure the same listener
                // doesn't subscribe twice to the same or the default channel
                foreach (KeyValuePair<String, IList<EmittedListener>> entry in channelEmitListeners)
                {
                    if (entry.Value.Contains(listener))
                    {
                        // If already subscribed to the default channel, do not add
                        // If already subscribed to the same channel, do not add
                        if ((entry.Key == null) ||
                            ((channel != null) && (channel == entry.Key)))
                        {
                            return;
                        }

                        // If subscribing to default channel, remove from existing channel
                        if (channel == null)
                        {
                            entry.Value.Remove(listener);
                        }
                    }
                }

                // Add listener, its a new listener or new channel for an existing listener
                IList<EmittedListener> listeners = channelEmitListeners.Get(channel);
                if (listeners == null)
                {
                    listeners = new List<EmittedListener>();
                    channelEmitListeners.Put(channel, listeners);
                }

                listeners.Add(listener);
            }
        }

	    /// <summary>
        /// Removes all listeners for emitted events.
        /// </summary>
		public void ClearListeners()
		{
            using (new WriterLock(channelEmitListenersRWLock))
            {
                channelEmitListeners.Clear();
            }
		}

        /// <summary>
        /// Emit an event to the specified channel. All listeners listening to the exact same channel and
        /// all listeners listening to the default channel are handed the event emitted.
        /// </summary>
        /// <param name="_object">is the event to emit</param>
        /// <param name="channel">is the channel to emit to</param>
		public void EmitEvent(Object _object, String channel)
		{
            IList<EmittedListener> listeners;

            using (new ReaderLock(channelEmitListenersRWLock))
            {
                // Emit to specific channel first
                if (channel != null)
                {
                    listeners = channelEmitListeners.Get(channel);
                    if (listeners != null)
                    {
                        foreach (EmittedListener listener in listeners)
                        {
                            listener.Emitted(_object);
                        }
                    }
                }

                // Emit to default channel if there are any listeners
                listeners = channelEmitListeners.Get(null);
                if (listeners != null)
                {
                    foreach (EmittedListener listener in listeners)
                    {
                        listener.Emitted(_object);
                    }
                }
            }

            Interlocked.Increment(ref numEventsEmitted);
		}

        public void ResetStats()
        {
            Interlocked.Exchange(ref numEventsEmitted, 0);
        }
	}
}
