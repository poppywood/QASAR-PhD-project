using System.Collections.Generic;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Collection of utilities specifically to help with enumeration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumerationHelper<T>
    {
        /// <summary>
        /// Creates the empty enumerator.
        /// </summary>
        /// <returns></returns>
        public static IEnumerator<T> CreateEmptyEnumerator()
        {
            return new NullEnumerator<T>();
        }

        /// <summary>
        /// Creates the singleton enumerator.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IEnumerator<T> CreateSingletonEnumerator(T item)
        {
            yield return item;
        }

        /// <summary>
        /// Creates an enumerator that skips a number of items in the
        /// subEnumerator.
        /// </summary>
        /// <param name="subEnumerator">The child enumerator.</param>
        /// <param name="numToAdvance">The num to advance.</param>
        /// <returns></returns>
        public static IEnumerable<T> AdvanceEnumerable( IEnumerator<T> subEnumerator, int numToAdvance )
        {
            bool hasMore = true;

            for( int ii = 0 ; ii < numToAdvance ; ii++ ) {
                if (!subEnumerator.MoveNext()) {
                    hasMore = false;
                    break;
                }
            }

            if ( hasMore ) {
                while( subEnumerator.MoveNext() ) {
                    yield return subEnumerator.Current;
                }
            }
        }
    }
}
