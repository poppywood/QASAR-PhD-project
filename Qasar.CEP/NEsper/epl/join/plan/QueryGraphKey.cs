using System;

using com.espertech.esper.collection;

namespace com.espertech.esper.epl.join.plan
{
    /// <summary>
    /// Key consisting of 2 integer stream numbers, for use by <see cref="QueryGraph"/>.
    /// </summary>
    public class QueryGraphKey
    {
        private readonly UniformPair<Int32> streams;

        /// <summary>Ctor.</summary>
        /// <param name="streamOne">from stream</param>
        /// <param name="streamTwo">to stream</param>
        public QueryGraphKey(int streamOne, int streamTwo)
        {
            if (streamOne > streamTwo)
            {
                int temp = streamTwo;
                streamTwo = streamOne;
                streamOne = temp;
            }
            streams = new UniformPair<Int32>(streamOne, streamTwo);
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
            if (this == obj)
            {
                return true;
            }

            QueryGraphKey other = obj as QueryGraphKey;
            if (other == null)
            {
                return false;
            }

            return other.streams.Equals(this.streams);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return streams.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return "QueryGraphKey " + streams.First + " and " + streams.Second;
        }
    }
}

