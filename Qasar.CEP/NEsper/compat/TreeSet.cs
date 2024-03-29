using System;
using System.Collections.Generic;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// An extended set that uses a tree-based backing store.
    /// As such, the set is always sorted.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    [Serializable]
    public class TreeSet<T> : Set<T>
	{
    	private readonly C5.TreeSet<T> m_store;
    	
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeSet&lt;T&gt;"/> class.
        /// </summary>
        public TreeSet()
        {
        	m_store = new C5.TreeSet<T>() ;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeSet&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="sourceData">The source data.</param>
        public TreeSet(IEnumerable<T> sourceData)
        {
            m_store = new C5.TreeSet<T>();
            m_store.AddAll(sourceData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeSet&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public TreeSet(IComparer<T> comparer)
        {
        	m_store = new C5.TreeSet<T>(comparer) ;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeSet&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="sourceData">The source data.</param>
        /// <param name="comparer">The comparer.</param>
        public TreeSet(IEnumerable<T> sourceData, IComparer<T> comparer)
        {
            m_store = new C5.TreeSet<T>(comparer);
            m_store.AddAll(sourceData);
        }

        /// <summary>
		/// Gets the first item in the set.
		/// </summary>
		/// <value>The first item in the set.</value>

		public T First
		{
			get { return m_store.FindMin(); }
		}

		/// <summary>
		/// Gets the last item in the set.
		/// </summary>

		public T Last
		{
			get { return m_store.FindMax(); }
		}

		/// <summary>
		/// Returns a set that includes all items that are greater than
		/// or equal to the index value.
		/// </summary>
		/// <param name="from">Value to index from.</param>
		/// <returns></returns>
		
		public TreeSet<T> TailSet( T from )
		{
			TreeSet<T> sortedSet = new TreeSet<T>();
			sortedSet.AddAll( m_store.RangeFrom( from ) );
			return sortedSet;
		}

		#region Set<T> Members

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
		
		public void Add( T item )
		{
			m_store.Add( item );
		}

		/// <summary>
		/// Adds all of the items in the source.
		/// </summary>
		/// <param name="source">The source.</param>

		public void AddAll( IEnumerable<T> source )
		{
			m_store.AddAll( source );
		}

        /// <summary>
        /// Removes all items.
        /// </summary>
        /// <param name="items"></param>

        public void RemoveAll(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                m_store.Remove(item);
            }
        }

		#endregion
    	
		public bool IsEmpty {
			get {
				return m_store.IsEmpty;
			}
		}
    	
		public int Count {
			get {
				return m_store.Count;
			}
		}
    	
		public bool IsReadOnly {
			get {
				return m_store.IsReadOnly;
			}
		}
    	
		public T[] ToArray()
		{
			return m_store.ToArray();
		}
    	
		public void Clear()
		{
			m_store.Clear();
		}
    	
		public bool Contains(T item)
		{
			return m_store.Contains(item);
		}
    	
		public void CopyTo(T[] array, int arrayIndex)
		{
			m_store.CopyTo(array, arrayIndex);
		}
    	
		public bool Remove(T item)
		{
			return m_store.Remove(item);
		}
    	
		public IEnumerator<T> GetEnumerator()
		{
			return m_store.GetEnumerator();
		}
    	
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return m_store.GetEnumerator();
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            TreeSet<T> other = obj as TreeSet<T>;
            if ( other == null )
            {
                return false;
            }

            return CollectionHelper.AreEqual(
                this.GetEnumerator(),
                other.GetEnumerator());
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return m_store.GetHashCode();
        }
	}
}
