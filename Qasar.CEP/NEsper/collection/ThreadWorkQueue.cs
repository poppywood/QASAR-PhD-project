using System;
using System.Collections.Generic;

namespace com.espertech.esper.collection
{
	/// <summary>
    /// Simple queue implementation based on a Linked List per thread.
	/// Objects can be added to the queue tail or queue head.
	/// </summary>

    public class ThreadWorkQueue
    {
        [ThreadStatic]
        private static LinkedList<Object> threadQueue;
        private static LinkedList<Object> LocalThreadQueue
        {
            get
            {
                if (threadQueue == null)
                {
                    threadQueue = new LinkedList<Object>();
                }
                return threadQueue;
            }
        }

        /// <summary>Adds event to the end of the event queue.</summary>
        /// <param name="ev">event to add</param>
        public static void Add(Object ev)
        {
            LinkedList<Object> queue = LocalThreadQueue;
            queue.AddLast(ev);
        }

        /// <summary>Adds event to the front of the queue.</summary>
        /// <param name="ev">event to add</param>

        public static void AddFront(Object ev)
        {
            LinkedList<Object> queue = LocalThreadQueue;
            queue.AddFirst(ev);
        }

        /// <summary>
        /// Returns the next event to GetSelectListEvents, or null
        /// if there are no more events.
        /// </summary>
        /// <returns>next event to GetSelectListEvents</returns>

        public static Object Next()
        {
            LinkedList<Object> queue = LocalThreadQueue;
            LinkedListNode<Object> head = queue.First;
            if (head == null)
                return null;

            Object item = head.Value;
            queue.RemoveFirst();
            return item;
        }

        /// <summary>Returns an indicator whether the queue is empty.</summary>
        /// <returns>true for empty, false for not empty</returns>
        public static bool IsEmpty
        {
            get
            {
                LinkedList<Object> queue = LocalThreadQueue;
                return queue.First == null;
            }
        }
    }
}
