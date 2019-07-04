using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.collection;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.db
{
	/// <summary> Query result data cache implementation that uses a least-recently-used algorithm
	/// to store and evict query results.
	/// </summary>

    public class DataCacheLRUImpl : DataCache
    {
        private readonly int cacheSize;
        private readonly float hashTableLoadFactor = 0.75f;
        private readonly LinkedHashMap<MultiKey<Object>, EventTable> cache;

        /// <summary> Ctor.</summary>
        /// <param name="cacheSize">is the maximum cache size
        /// </param>

        public DataCacheLRUImpl(int cacheSize)
        {
            this.cacheSize = cacheSize;
            int hashTableCapacity = (int)Math.Ceiling(cacheSize / hashTableLoadFactor) + 1;
            this.cache = new LinkedHashMap<MultiKey<Object>, EventTable>(hashTableCapacity);
            this.cache.ShuffleOnAccess = true;
            this.cache.RemoveEldest +=
                delegate { return this.cache.Count > this.cacheSize; };
        }

        /// <summary> Retrieves an entry from the cache.
        /// The retrieved entry becomes the MRU (most recently used) entry.
        /// </summary>
        /// <param name="lookupKeys">the key whose associated value is to be returned.
        /// </param>
        /// <returns> the value associated to this key, or null if no value with this key exists in the cache.
        /// </returns>
        public EventTable GetCached(Object[] lookupKeys)
        {
            EventTable rvalue = null;
            MultiKey<Object> keys = new MultiKey<Object>(lookupKeys);
			cache.TryGetValue( keys, out rvalue );
			return rvalue;
        }

        /// <summary> Adds an entry to this cache.
        /// If the cache is full, the LRU (least recently used) entry is dropped.
        /// </summary>
        /// <param name="key">the key with which the specified value is to be associated.
        /// </param>
        /// <param name="value">a value to be associated with the specified key.
        /// </param>

        public void PutCached(Object[] key, EventTable value)
        {
            lock (this)
            {
                MultiKey<Object> mkeys = new MultiKey<Object>(key);
                cache[mkeys] = value;
            }
        }

        /// <summary> Returns the maximum cache size.</summary>
        /// <returns> maximum cache size
        /// </returns>
        public int CacheSize
        {
            get { return cacheSize; }
        }

        public bool IsActive
        {
            get { return true; }
        }
    }
}
