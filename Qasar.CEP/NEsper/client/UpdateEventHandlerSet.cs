using System;
using System.Collections.Generic;

using net.esper.compat;

namespace net.esper.client
{
    public class UpdateEventHandlerSet : Set<UpdateEventHandler>
    {
        private List<UpdateEventHandler> uList;
        /// <summary>
        /// Lock used for destructive operations.
        /// </summary>
        private readonly MonitorLock uLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateEventHandlerSet"/> class.
        /// </summary>
        public UpdateEventHandlerSet()
        {
            uLock = new MonitorLock();
            uList = new List<UpdateEventHandler>();
        }

        /// <summary> Add an listener that observes events.</summary>
        /// <param name="listener">to add
        /// </param>
        public void AddListener(UpdateListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentException("Null listener reference supplied");
            }

            UpdateEventHandler handler = listener.Update;
            if (handler != null)
            {
                Add(handler);
            }
        }

        /// <summary> Remove an listener that observes events.</summary>
        /// <param name="listener">to remove
        /// </param>
        public void RemoveListener(UpdateListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentException("Null listener reference supplied");
            }

            UpdateEventHandler handler = listener.Update;
            if (handler != null)
            {
                if (!Remove(handler))
                {
                    throw new ArgumentException( "listener was not found in the event set");
                }
            } else
            {
                throw new ArgumentException( "listener produced a null event handler");
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<UpdateEventHandler> GetEnumerator()
        {
            return uList.GetEnumerator();
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

        #region Set<UpdateEventHandler> Members

        public UpdateEventHandler[] ToArray()
        {
            return uList.ToArray();
        }

        public void AddAll(IEnumerable<UpdateEventHandler> source)
        {
            using(uLock.Acquire())
            {
                List<UpdateEventHandler> tList = new List<UpdateEventHandler>(uList);
                foreach (UpdateEventHandler item in source)
                {
                    if (!tList.Contains(item))
                    {
                        tList.Add(item);
                    }
                }

                uList = tList;
            }
        }

        public UpdateEventHandler First
        {
            get { return uList[0]; }
        }

        public bool IsEmpty
        {
            get { return uList.Count == 0; }
        }

        public void RemoveAll(IEnumerable<UpdateEventHandler> items)
        {
            using (uLock.Acquire())
            {
                List<UpdateEventHandler> tList = new List<UpdateEventHandler>(uList);
                foreach (UpdateEventHandler item in items)
                {
                    tList.Remove(item);
                }

                uList = tList;
            }
        }

        #endregion

        #region ICollection<UpdateEventHandler> Members

        public int Count
        {
            get { return uList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void CopyTo(UpdateEventHandler[] array, int arrayIndex)
        {
            uList.CopyTo(array, arrayIndex);
        }

        public bool Contains(UpdateEventHandler item)
        {
            return uList.Contains(item);
        }

        public void Add(UpdateEventHandler item)
        {
            using (uLock.Acquire())
            {
                List<UpdateEventHandler> tList = new List<UpdateEventHandler>(uList);
                if (!tList.Contains(item))
                {
                    tList.Add(item);
                    uList = tList;
                }
            }
        }

        public void Clear()
        {
            using (uLock.Acquire())
            {
                uList = new List<UpdateEventHandler>();
            }
        }

        public bool Remove(UpdateEventHandler item)
        {
            using (uLock.Acquire())
            {
                List<UpdateEventHandler> tList = new List<UpdateEventHandler>(uList);
                if (tList.Remove(item))
                {
                    uList = tList;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion
    }
}
