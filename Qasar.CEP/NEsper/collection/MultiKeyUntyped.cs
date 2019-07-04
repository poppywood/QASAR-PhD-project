using System;

using com.espertech.esper.compat;
using com.espertech.esper.util;

namespace com.espertech.esper.collection
{
	/// <summary> Functions as a key value for Maps where keys need to be composite values.
	/// The class allows a Map that uses MultiKeyUntyped entries for key values to use multiple objects as keys.
	/// It calculates the hashCode from the key objects on construction and caches the hashCode.
	/// </summary>
	public sealed class MultiKeyUntyped : MetaDefItem
	{
		/// <summary> Returns keys.</summary>
		/// <returns> keys object array
		/// </returns>

        public Object[] Keys
		{
            get { return keys; }
		}

		private readonly Object[] keys;
	    private readonly _ComparisonOperator[] compArray;
		private readonly int hashCode;
		
		/// <summary> Constructor for multiple keys supplied in an object array.</summary>
		/// <param name="keys">is an array of key objects
		/// </param>
		public MultiKeyUntyped(Object[] keys)
		{
			if (keys == null)
			{
				throw new ArgumentException("The array of keys must not be null");
			}

            // Create a table that outlines how we intend to compare
            // the keys during an equality test.  Failure to do so
            // will result in this test being performed during every
            // equality test.

            this.compArray = new _ComparisonOperator[keys.Length];
            
            int total = 0;
			for (int i = 0; i < keys.Length; i++)
			{
			    Object _tempK = keys[i];
				if (_tempK != null)
				{
					total ^= _tempK.GetHashCode();
                    if (TypeHelper.IsIntegral(_tempK.GetType()))
                    {
                        compArray[i] = _IntegerComp;
                    }
                    else if (TypeHelper.IsFloatingPoint(_tempK.GetType()))
                    {
                        compArray[i] = _FloatingComp;
                    }
                    else
                    {
                        compArray[i] = Equals;
                    }
                }
            }
			
			this.hashCode = total;
			this.keys = keys;
		}
		
		/// <summary> Constructor for a single key object.</summary>
		/// <param name="key">is the single key object
		/// </param>
		public MultiKeyUntyped(Object key) : this(new Object[]{key})
		{
		}
		
		/// <summary> Constructor for a pair of key objects.</summary>
		/// <param name="key1">is the first key object
		/// </param>
		/// <param name="key2">is the second key object
		/// </param>
		public MultiKeyUntyped(Object key1, Object key2):this(new Object[]{key1, key2})
		{
		}
		
		/// <summary> Constructor for three key objects.</summary>
		/// <param name="key1">is the first key object
		/// </param>
		/// <param name="key2">is the second key object
		/// </param>
		/// <param name="key3">is the third key object
		/// </param>
		public MultiKeyUntyped(Object key1, Object key2, Object key3):this(new Object[]{key1, key2, key3})
		{
		}
		
		/// <summary> Constructor for four key objects.</summary>
		/// <param name="key1">is the first key object
		/// </param>
		/// <param name="key2">is the second key object
		/// </param>
		/// <param name="key3">is the third key object
		/// </param>
		/// <param name="key4">is the fourth key object
		/// </param>
		public MultiKeyUntyped(Object key1, Object key2, Object key3, Object key4):this(new Object[]{key1, key2, key3, key4})
		{
		}
		
		/// <summary> Returns the number of key objects.</summary>
		/// <returns> size of key object array
		/// </returns>
		public int Count
		{
            get { return keys.Length; }
		}
		
		/// <summary> Returns the key object at the specified position.</summary>
		/// <param name="index">is the array position
		/// </param>
		/// <returns> key object at position
		/// </returns>
		public Object this[int index]
		{
            get { return keys[index]; }
		}

	    private delegate bool _ComparisonOperator(Object o1, Object o2);

        private static bool _IntegerComp( Object o1, Object o2 )
        {
            long i1 = Convert.ToInt64(o1);
            long i2 = Convert.ToInt64(o2);
            return i1 == i2;
        }

        private static bool _FloatingComp( Object o1, Object o2 )
        {
            decimal d1 = Convert.ToDecimal(o1);
            decimal d2 = Convert.ToDecimal(o2);
            return d1 == d2;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="other">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
		public override bool Equals(Object other)
		{
			if (other == this)
			{
				return true;
			}

            MultiKeyUntyped otherKeys = other as MultiKeyUntyped;
            if ( otherKeys != null )
			{
			    Object[] _keys1 = otherKeys.keys;
			    Object[] _keys2 = keys;

                if (( _keys1.Length == _keys2.Length ) && ( hashCode == otherKeys.hashCode ))
                {
                    _ComparisonOperator[] tempCompArray = compArray;

                    for( int ii = _keys2.Length - 1 ; ii >= 0 ; ii-- )
                    {
                        Object k1 = _keys1[ii];
                        Object k2 = _keys2[ii];

                        bool k1n = k1 == null;
                        bool k2n = k2 == null;

                        if (k1n && k2n) // null equality
                        {
                        }
                        else if (k1n || k2n) // null inequality
                        {
                            return false;
                        }
                        else if (!tempCompArray[ii](k1, k2))
                        {
                            return false;
                        }
                    }

                    return true;
                }
			}

            return false;
		}

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override int GetHashCode()
		{
			return hashCode;
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override String ToString()
		{
			return "MultiKeyUntyped" + ArrayHelper.Render( keys ) ;
		}
	}
}
