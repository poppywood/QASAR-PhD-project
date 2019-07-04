using System;
using System.Collections.Generic;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// An extended dictionary based upon a closed hashing
    /// algorithm.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>

    [Serializable]
	public class HashMap<K,V> : BaseMap<K,V>
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="HashMap{K,V}"/> class.
        /// </summary>
		public HashMap()
			: base( new Dictionary<K,V>() )
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="HashMap{K,V}"/> class.
        /// </summary>
        public HashMap(int initialCapacity)
            : base(new Dictionary<K, V>(initialCapacity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashMap{K,V}"/> class.
        /// </summary>
		
		public HashMap(IEqualityComparer<K> eqComparer)
			: base( new Dictionary<K,V>( eqComparer ) )
		{
		}
	}
}
