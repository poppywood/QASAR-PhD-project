using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.collection;
//using com.espertech.esper.collection.apachecommons;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.schedule;

namespace com.espertech.esper.epl.db
{
    /// <summary>
    /// Implements an expiry-time cache that evicts data when data becomes stale
    /// after a given number of seconds.
    /// <para>
    /// The cache reference type indicates which backing Map is used: Weak type uses the
    /// WeakHashMap, Soft type uses the apache commons ReferenceMap, and Hard type simply
    /// uses a HashMap.
    /// </summary>

    public sealed class DataCacheExpiringImpl : DataCache, ScheduleHandleCallback
    {
        /// <summary>
        /// Returns true if the cache is active and currently caching, or false if the cache is inactive and not currently caching
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        /// <returns>true for caching enabled, false for no caching taking place</returns>
        public bool IsActive
        {
            get { return true; }
        }

        /// <summary> Returns the maximum age in milliseconds.</summary>
        /// <returns> millisecon max age
        /// </returns>

        public long MaxAgeMSec
        {
            get { return maxAgeMSec; }
        }

        /// <summary> Returns the purge interval in milliseconds.</summary>
        /// <returns> millisecond purge interval
        /// </returns>

        public long PurgeIntervalMSec
        {
            get { return purgeIntervalMSec; }
        }

        /// <summary> Returns the current cache size.</summary>
        /// <returns> cache size
        /// </returns>

        public long Size
        {
            get { return cache.Count; }
        }

        private readonly long maxAgeMSec;
        private readonly long purgeIntervalMSec;
        private readonly SchedulingService schedulingService;
        private readonly ScheduleSlot scheduleSlot;
        private readonly Map<MultiKey<Object>, Item> cache;
		private readonly EPStatementHandle epStatementHandle;
        private bool isScheduled;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="maxAgeSec">is the maximum age in seconds</param>
        /// <param name="purgeIntervalSec">is the purge interval in seconds</param>
        /// <param name="configurationCacheReferenceType">indicates whether hard, soft or weak references are used in the cache</param>
        /// <param name="schedulingService">is a service for call backs at a scheduled time, for purging</param>
        /// <param name="scheduleSlot">slot for scheduling callbacks for this cache</param>
        /// <param name="epStatementHandle">the statements-own handle for use in registering callbacks with services</param>

        public DataCacheExpiringImpl(double maxAgeSec,
                                     double purgeIntervalSec,
                                     ConfigurationCacheReferenceType configurationCacheReferenceType,
                                     SchedulingService schedulingService,
                                     ScheduleSlot scheduleSlot,
                                     EPStatementHandle epStatementHandle)
        {
            this.maxAgeMSec = (long)maxAgeSec * 1000;
            this.purgeIntervalMSec = (long)purgeIntervalSec * 1000;
            this.schedulingService = schedulingService;
            this.scheduleSlot = scheduleSlot;
            this.cache = CreateCache(configurationCacheReferenceType);
			this.epStatementHandle = epStatementHandle;
        }

        private static Map<MultiKey<Object>, Item> CreateCache( ConfigurationCacheReferenceType referenceType )
        {
            switch( referenceType )
            {
                case ConfigurationCacheReferenceType.HARD:
                    return new HashMap<MultiKey<Object>, Item>(); 
                case ConfigurationCacheReferenceType.SOFT:
                    return new ReferenceMap<MultiKey<Object>, Item>(
                        ReferenceType.SOFT, ReferenceType.SOFT);
                case ConfigurationCacheReferenceType.WEAK:
                    return new ReferenceMap<MultiKey<Object>, Item>(
                        ReferenceType.SOFT, ReferenceType.HARD);
                default:
                    throw new ArgumentException("invalid reference type for cache", "referenceType");
            }
        }

        /// <summary>
        /// Ask the cache if the keyed value is cached, returning a list or rows if the key is in the cache,
        /// or returning null to indicate no such key cached. Zero rows may also be cached.
        /// </summary>
        /// <param name="lookupKeys">is the keys to look up in the cache</param>
        /// <returns>
        /// a list of rows that can be empty is the key was found in the cache, or null if
        /// the key is not found in the cache
        /// </returns>
        public EventTable GetCached(Object[] lookupKeys)
        {
            MultiKey<Object> key = new MultiKey<Object>(lookupKeys);
            Item item = null;
            if ( ! cache.TryGetValue( key, out item ) )
            {
                return null;
            }

            long now = schedulingService.Time;
            if ((now - item.Time) > maxAgeMSec)
            {
                cache.Remove(key);
                return null;
            }

			return item.Data;
        }

        /// <summary>
        /// Puts into the cache a key and a list of rows, or an empty list if zero rows.
        /// <para>
        /// The put method is designed to be called when the cache does not contain a key as
        /// determined by the get method. Implementations typically simply overwrite
        /// any keys put into the cache that already existed in the cache.
        /// </para>
        /// </summary>
        /// <param name="lookupKeys">is the keys to the cache entry</param>
        /// <param name="rows">is a number of rows</param>
        public void PutCached(Object[] lookupKeys, EventTable rows)
        {
            MultiKey<Object> key = new MultiKey<Object>(lookupKeys);
            long now = schedulingService.Time;
            Item item = new Item(rows, now);
            cache[key] = item;

            if (!isScheduled)
            {
	            EPStatementHandleCallback callback = new EPStatementHandleCallback(epStatementHandle, this);
	            schedulingService.Add(purgeIntervalMSec, callback, scheduleSlot);
            }
        }

        /// <summary>
        /// Called when a scheduled callback occurs.
        /// </summary>
        public void ScheduledTrigger(ExtensionServicesContext extensionServicesContext)
        {
            // purge expired
            long now = schedulingService.Time;

            // Declare a list that is used to keep around
            // keys that must be removed.
            IList<MultiKey<Object>> deadKeyList = null;
            // Iterate through the cache
            IEnumerator<MultiKey<Object>> itemKeyEnum = cache.Keys.GetEnumerator();
            while (itemKeyEnum.MoveNext())
            {
                MultiKey<Object> itemKey = itemKeyEnum.Current;
                Item item = cache[itemKey];
                if ((now - item.Time) > maxAgeMSec)
                {
                    if (deadKeyList == null)
                    {
                        deadKeyList = new List<MultiKey<Object>>();
                    }

                    deadKeyList.Add(itemKey);
                }
            }

            if (deadKeyList != null)
            {
                foreach (MultiKey<Object> itemKey in deadKeyList)
                {
                    cache.Remove(itemKey);
                }
            }

            isScheduled = false;
        }

        private sealed class Item
        {
            private readonly EventTable data;
            private readonly long time;

            public EventTable Data
            {
                get { return data; }
            }

            public long Time
            {
                get { return time; }
            }

            public Item(EventTable data, long time)
            {
                this.data = data;
                this.time = time;
            }
        }
    }
}
