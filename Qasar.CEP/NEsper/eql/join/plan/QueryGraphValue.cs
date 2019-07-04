using System;
using System.Collections.Generic;

namespace net.esper.eql.join.plan
{
    /// <summary>
    /// Property lists stored as a value for each stream-to-stream relationship, for use by {@link QueryGraph}.
    /// </summary>
    public class QueryGraphValue
    {
        private readonly List<String> propertiesLeft;
        private readonly List<String> propertiesRight;

        /// <summary>Ctor.</summary>
        public QueryGraphValue()
        {
            propertiesLeft = new List<String>();
            propertiesRight = new List<String>();
        }

        /// <summary>Add key and index property.</summary>
        /// <param name="keyProperty">key property</param>
        /// <param name="indexProperty">index property</param>
        /// <returns>
        /// true if added and either property did not exist, false if either already existed
        /// </returns>
        public bool Add(String keyProperty, String indexProperty)
        {
            if (propertiesLeft.Contains(keyProperty))
            {
                return false;
            }
            if (propertiesRight.Contains(indexProperty))
            {
                return false;
            }
            propertiesLeft.Add(keyProperty);
            propertiesRight.Add(indexProperty);
            return true;
        }

        /// <summary>Returns property names for left stream.</summary>
        /// <returns>property names</returns>
        public List<String> PropertiesLeft
        {
            get { return propertiesLeft; }
        }

        /// <summary>Returns property names for right stream.</summary>
        /// <returns>property names</returns>
        public List<String> PropertiesRight
        {
            get { return propertiesRight; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return "QueryGraphValue " +
                    " propertiesLeft=" + propertiesLeft +
                    " propertiesRight=" + propertiesRight;
        }
    }
}

