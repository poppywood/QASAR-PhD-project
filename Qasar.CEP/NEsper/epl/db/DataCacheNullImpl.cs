using System;
using System.Collections.Generic;

using com.espertech.esper.epl.join.table;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.db
{
	/// <summary>
    /// Null implementation for a data cache that doesn't ever hit.
    /// </summary>
	
    public class DataCacheNullImpl : DataCache
	{
        /// <summary>
        /// Ask the cache if the keyed value is cached, returning a list or rows if the key is in the cache,or returning null to indicate no such key cached. Zero rows may also be cached.
        /// </summary>
        /// <param name="lookupKeys">is the keys to look up in the cache</param>
        /// <returns>
        /// a list of rows that can be empty is the key was found in the cache, or null ifthe key is not found in the cache
        /// </returns>
        public EventTable GetCached(Object[] lookupKeys)
        {
            return null;
        }

        /// <summary>
        /// Puts into the cache a key and a list of rows, or an empty list if zero rows.
        /// <para/>
        /// The put method is designed to be called when the cache does not contain a key as
        /// determined by the get method. Implementations typically simply overwrite any keys put
        /// into the cache that already existed in the cache.
        /// </summary>
        /// <param name="lookupKeys">is the keys to the cache entry</param>
        /// <param name="rows">is a number of rows</param>
        public void PutCached(Object[] lookupKeys, EventTable rows)
        {
        }

        /// <summary>
        /// Returns true if the cache is active and currently caching, or false if the cache is inactive and not currently caching
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        /// <returns>true for caching enabled, false for no caching taking place</returns>
        public bool IsActive
        {
            get { return false; }
        }
	}
}
