using System;

using com.espertech.esper.compat;
using com.espertech.esper.collection;
using com.espertech.esper.events;

namespace com.espertech.esper.filter
{
    /// <summary> This class is responsible for changes to <see cref="EventTypeIndex"/> for addition and removal of filters.
    /// It delegates the work to make modifications to the filter parameter tree to an <see cref="IndexTreeBuilder"/>.
    /// It enforces a policy that a filter callback can only be added once.
    /// </summary>

    public class EventTypeIndexBuilder
    {
        private readonly Map<FilterHandle, Pair<EventType, IndexTreePath>> callbacks;
        private readonly MonitorLock callbacksLock;
        private readonly EventTypeIndex eventTypeIndex;

        /// <summary> Constructor - takes the event type index to manipulate as its parameter.</summary>
        /// <param name="eventTypeIndex">index to manipulate
        /// </param>

        public EventTypeIndexBuilder(EventTypeIndex eventTypeIndex)
        {
            this.eventTypeIndex = eventTypeIndex;
            this.callbacks = new HashMap<FilterHandle, Pair<EventType, IndexTreePath>>();
            this.callbacksLock = new MonitorLock();
        }

        /// <summary>
        /// Destroy the service.
        /// </summary>
        public void Destroy()
        {
            callbacks.Clear();
        }

        /// <summary> Add a filter to the event type index structure, and to the filter subtree.
        /// Throws an IllegalStateException exception if the callback is already registered.
        /// </summary>
        /// <param name="filterValueSet">is the filter information
        /// </param>
        /// <param name="filterCallback">is the callback
        /// </param>
        public void Add(FilterValueSet filterValueSet, FilterHandle filterCallback)
        {
            EventType eventType = filterValueSet.EventType;

            // Check if a filter tree exists for this event type
            FilterHandleSetNode rootNode = eventTypeIndex[eventType];

            // Make sure we have a root node
            using (callbacksLock.Acquire())
            {
                if (rootNode == null)
                {
                    rootNode = eventTypeIndex[eventType];
                    if (rootNode == null)
                    {
                        rootNode = new FilterHandleSetNode();
                        eventTypeIndex.Add(eventType, rootNode);
                    }
                }

                // Make sure the filter callback doesn't already exist
                if (callbacks.ContainsKey(filterCallback))
                {
                    throw new IllegalStateException("Callback for filter specification already exists in collection");
                }
            }

            // Now add to tree
            IndexTreeBuilder treeBuilder = new IndexTreeBuilder();
            IndexTreePath path = treeBuilder.Add(filterValueSet, filterCallback, rootNode);

            using (callbacksLock.Acquire())
            {
                callbacks[filterCallback] = new Pair<EventType, IndexTreePath>(eventType, path);
            }
        }

        /// <summary> Remove a filter callback from the given index node.</summary>
        /// <param name="filterCallback">is the callback to remove
        /// </param>
        public void Remove(FilterHandle filterCallback)
        {
            Pair<EventType, IndexTreePath> pair;

            using (callbacksLock.Acquire())
            {
                pair = callbacks.Get(filterCallback, null);
            }

            if (pair == null)
            {
                throw new ArgumentException("Filter callback to be removed not found");
            }

            FilterHandleSetNode rootNode = eventTypeIndex[pair.First];

            // Now remove from tree
            IndexTreeBuilder treeBuilder = new IndexTreeBuilder();
            treeBuilder.Remove(filterCallback, pair.Second, rootNode);

            // Remove from callbacks list
            using (callbacksLock.Acquire())
            {
                callbacks.Remove(filterCallback);
            }
        }
    }
}
