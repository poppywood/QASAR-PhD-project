using System;
using System.Collections.Generic;

using com.espertech.esper.compat;

namespace com.espertech.esper.collection
{
	/// <summary>
    /// Sorted, reference-counting set based on a SortedDictionary implementation that stores keys and a reference counter for
	/// each unique key value. Each time the same key is added, the reference counter increases.
	/// Each time a key is removed, the reference counter decreases.
	/// </summary>

    public class SortedRefCountedSet<K>
	{
    	private readonly C5.TreeDictionary<K, MutableInt> refSet;

		/// <summary>
		///  Constructor.
		/// </summary>

		public SortedRefCountedSet()
		{
            if (typeof(K) is IComparable)
            {
                refSet = new C5.TreeDictionary<K, MutableInt>();
            }
            else
            {
                refSet = new C5.TreeDictionary<K, MutableInt>(new DefaultComparer());
            }
		}

		/// <summary> Add a key to the set. Add with a reference count of one if the key didn't exist in the set.
		/// Increase the reference count by one if the key already exists.
		/// </summary>
		/// <param name="key">to add
		/// </param>

		public virtual void Add(K key)
		{
            MutableInt value;
            if (!refSet.Find(ref key, out value))
            {
                refSet.Add(key, new MutableInt());
            }
            else
            {
                value.Value++;
            }

            //if (refSet.FindOrAdd(key, ref value))
            //{
            //    ++value.Value;
            //    //refSet.Update(key, ++value);
            //}
		}

        /// <summary>
        /// Add a key to the set with the given number of references.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="numReferences">The num references.</param>
        public void Add(K key, int numReferences)
        {
            MutableInt value;
            if (!refSet.Find( ref key, out value ))
            {
                refSet[key] = new MutableInt(numReferences);
                return;
            }
            throw new ArgumentException("Key '" + key + "' already in collection");
        }

        /// <summary>
        /// Clear out the collection.
        /// </summary>
        public void Clear()
        {
            refSet.Clear();
        }

		/// <summary> Remove a key from the set. Removes the key if the reference count is one.
		/// Decreases the reference count by one if the reference count is more then one.
		/// </summary>
		/// <param name="key">to add
		/// </param>
		/// <throws>  IllegalStateException is a key is removed that wasn't added to the map </throws>

        public virtual void Remove(K key)
		{
            MutableInt value;

            if (!refSet.Find(ref key, out value))
            {
                // This could happen if a sort operation gets a remove stream that duplicates events.
                // Generally points to an invalid combination of data windows.
                // throw new IllegalStateException("Attempting to remove key from map that wasn't added");
                return;
            }

			if (value.Value == 1)
			{
				refSet.Remove(key);
				return ;
			}

			value.Value--;
            //refSet[key] = value;
		}

		/// <summary> Returns the largest key value, or null if the collection is empty.</summary>
		/// <returns> largest key value, null if none
		/// </returns>

        public virtual K MaxValue
		{
        	get
        	{
        		return
        			( refSet.Count != 0 ) ?
        			( refSet.FindMax().Key ) :
        			( default(K) ) ;
        	}
		}

		/// <summary> Returns the smallest key value, or null if the collection is empty.</summary>
		/// <returns> smallest key value, default(K) if none
		/// </returns>

        public virtual K MinValue
		{
        	get
        	{
        		return
        			( refSet.Count != 0 ) ?
        			( refSet.FindMin().Key ) :
        			( default(K) ) ;
        	}
		}

        sealed class MutableInt
        {
            public int Value = 1;

            public MutableInt() {}
            public MutableInt(int initialValue)
            {
                Value = initialValue;
            }
        }

        sealed class DefaultComparer : IComparer<K>
        {
            public int Compare(K x, K y)
            {
                IComparable xx = (IComparable)x;
                return xx.CompareTo(y);
            }
        }
	}
}
