using System;
using System.Collections.Generic;
using System.Threading;

using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.collection
{
    /// <summary> Container for events per time slot. The time is provided as long milliseconds by client classes.
    /// Events are for a specified timestamp and the implementation creates and adds the event to a slot for that timestamp.
    /// Events can be expired from the window via the expireEvents method when their timestamp is before
    /// (or less then) an expiry timestamp passed in. Expiry removes the event from the window.
    /// The window allows iteration through its contents.
    ///
    /// It is assumed that the timestamp passed to the add method is ascending. The window is backed by a
    /// collection reflecting the timestamp order rather then any sorted map or linked hash map for
    /// performance reasons.
    /// </summary>

    public sealed class TimeWindow : IEnumerable<EventBean>
    {
        private readonly LinkedList<Pair<long, List<EventBean>>> window;
        private readonly Map<EventBean, IList<EventBean>> reverseIndex;
        private long? oldestTimestamp;
        private long generation;

        /// <summary> Returns the oldest timestamp in the collection if there is at least one entry,
        /// else it returns null if the window is empty.
        /// </summary>
        /// <returns> null if empty, oldest timestamp if not empty
        /// </returns>

        public long? OldestTimestamp
        {
            get { return oldestTimestamp; }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
		public int Count
		{
			get { return window.Count; }
		}

        /// <summary> Returns true if the window is currently empty.</summary>
        /// <returns> true if empty, false if not
        /// </returns>

        public bool IsEmpty
        {
            get { return window.Count == 0; }
        }

        /// <summary>Ctor.</summary>
        /// <param name="isSupportRemoveStream">true to indicate the time window should support effective removal of eventsin the window based on the remove stream events received, or false to not accomodate removal at all</param>
        public TimeWindow(bool isSupportRemoveStream)
        {
            this.window = new LinkedList<Pair<long, List<EventBean>>>();
            this.oldestTimestamp = null;

            if (isSupportRemoveStream)
            {
                reverseIndex = new HashMap<EventBean, IList<EventBean>>();
            }
        }

        /// <summary> Adds event to the time window for the specified timestamp.</summary>
        /// <param name="timestamp">the time slot for the event
        /// </param>
        /// <param name="bean">event to add
        /// </param>

        public void Add(long timestamp, EventBean bean)
        {
            Interlocked.Increment(ref generation);

            // On add to an empty window, set the oldest event's timestamp
            if (oldestTimestamp == null)
            {
                oldestTimestamp = timestamp;
            }

            // Empty window
            if (window.Count == 0)
            {
                List<EventBean> listOfBeans = new List<EventBean>();
                listOfBeans.Add(bean);

                Pair<long, List<EventBean>> pair = new Pair<long, List<EventBean>>(timestamp, listOfBeans);
                window.AddLast(pair);

                if (reverseIndex != null)
                {
                    reverseIndex[bean] = listOfBeans;
                }

                return;
            }

            Pair<long, List<EventBean>> lastPair = window.Last.Value;

            // Windows last timestamp matches the one supplied
            if (lastPair.First == timestamp)
            {
                lastPair.Second.Add(bean);
                if (reverseIndex != null)
                {
                    reverseIndex[bean] = lastPair.Second;
                }
                return;
            }

            // Append to window
            List<EventBean> _listOfBeans = new List<EventBean>();
            _listOfBeans.Add(bean);

            Pair<long, List<EventBean>> _pair = new Pair<long, List<EventBean>>(timestamp, _listOfBeans);
            if (reverseIndex != null)
            {
                reverseIndex[bean] = _listOfBeans;
            }
            window.AddLast(_pair);
        }

        /// <summary>
        /// Removes the event from the window, if remove stream handling is enabled.
        /// </summary>
        /// <param name="event">to remove</param>
        public void Remove(EventBean @event)
        {
            if (reverseIndex == null)
            {
                throw new UnsupportedOperationException("Time window does not accept event removal");
            }
            IList<EventBean> list = reverseIndex.Get(@event);
            if (list != null)
            {
                list.Remove(@event);
            }
            reverseIndex.Remove(@event);
        }

        /// <summary> Return and remove events in time-slots earlier (less) then the timestamp passed in,
        /// returning the list of events expired.
        /// </summary>
        /// <param name="expireBefore">is the timestamp from which on to keep events in the window
        /// </param>
        /// <returns> a list of events expired and removed from the window, or null if none expired
        /// </returns>

        public List<EventBean> ExpireEvents(long expireBefore)
        {
            if (window.Count == 0)
            {
                return null;
            }

            Pair<Int64, List<EventBean>> pair = window.First.Value;

            // If the first entry's timestamp is after the expiry date, nothing to expire
            if (pair.First >= expireBefore)
            {
                return null;
            }

            List<EventBean> resultBeans = new List<EventBean>();

            // Repeat until the window is empty or the timestamp is above the expiry time
            do
            {
                resultBeans.AddRange(pair.Second);
                window.RemoveFirst();

                if (window.Count == 0)
                {
                    break;
                }

                pair = window.First.Value;
            }
            while (pair.First < expireBefore);

            if (window.Count == 0)
            {
                oldestTimestamp = null;
            }
            else
            {
                oldestTimestamp = pair.First;
            }

            if (reverseIndex != null)
            {
                foreach (EventBean expired in resultBeans)
                {
                    reverseIndex.Remove(expired);
                }
            }

            return resultBeans;
        }

        /// <summary>
        /// Returns event iterator.
        /// </summary>
        /// <returns>iterator over events currently in window</returns>

        public IEnumerator<EventBean> GetEnumerator()
        {
            return GetEnumerator(Interlocked.Read(ref generation));
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <param name="myGeneration">My generation.</param>
        /// <returns></returns>
        private IEnumerator<EventBean> GetEnumerator(long myGeneration)
        {
            foreach( Pair<Int64, List<EventBean>> pair in window )
            {
                foreach( EventBean eventBean in pair.Second )
                {
                    if (Interlocked.Read(ref generation) > myGeneration)
                    {
                        throw new InvalidOperationException();
                    }

                    yield return eventBean ;
                }
            }
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
            return GetEnumerator();
        }

        #endregion


        /// <summary>
        /// Returns the reverse index, for testing purposes.
        /// </summary>
        /// <value>The index of the reverse.</value>
        /// <returns>reverse index</returns>
        public Map<EventBean, IList<EventBean>> ReverseIndex
        {
            get { return reverseIndex; }
        }
    }
}
