using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.join.table
{
    /// <summary>
    /// Simple table of events without an index.
    /// </summary>

    public class UnindexedEventTable : EventTable
    {
        private readonly int streamNum;
        private readonly Set<EventBean> eventSet = new HashSet<EventBean>();

        /// <summary> Ctor.</summary>
        /// <param name="streamNum">is the indexed stream's number
        /// </param>

        public UnindexedEventTable(int streamNum)
        {
            this.streamNum = streamNum;
        }

        /// <summary>
        /// Clear out index.
        /// </summary>
        public void Clear()
        {
            eventSet.Clear();
        }

        /// <summary>
        /// Adds the specified events.
        /// </summary>
        /// <param name="addEvents">The events to add.</param>
        public virtual void Add(IEnumerable<EventBean> addEvents)
        {
            if (addEvents == null)
            {
                return;
            }

            foreach( EventBean eventBean in addEvents )
            {
                eventSet.Add(eventBean);
            }
        }

        /// <summary>
        /// Removes the specified events.
        /// </summary>
        /// <param name="removeEvents">The events to remove.</param>
        public virtual void Remove(IEnumerable<EventBean> removeEvents)
        {
            if (removeEvents == null)
            {
                return;
            }

            foreach (EventBean eventBean in removeEvents)
            {
                eventSet.Remove(eventBean);
            }
        }

        /// <summary>
        /// Returns true if the index is empty, or false if not
        /// </summary>
        /// <value></value>
        /// <returns>true for empty index</returns>
        public bool IsEmpty
        {
            get { return eventSet.IsEmpty; }
        }

        /// <summary> Returns events in table.</summary>
        /// <returns> all events
        /// </returns>

        public Set<EventBean> EventSet
        {
            get { return eventSet; }
        }

        public IEnumerator<EventBean> GetEnumerator()
        {
            return eventSet.GetEnumerator();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return "UnindexedEventTable streamNum=" + streamNum;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return eventSet.GetEnumerator();
        }

        #endregion
    }
}
