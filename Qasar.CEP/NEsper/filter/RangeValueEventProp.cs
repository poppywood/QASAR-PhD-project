using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.pattern;
using com.espertech.esper.util;

namespace com.espertech.esper.filter
{
    /// <summary>
    /// An event property as a filter parameter representing a range.
    /// </summary>

    public class RangeValueEventProp : FilterSpecParamRangeValue
    {
        private readonly String resultEventAsName;
        private readonly String resultEventProperty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultEventAsName">the event tag</param>
        /// <param name="resultEventProperty">the event property name</param>

        public RangeValueEventProp(String resultEventAsName, String resultEventProperty)
        {
            this.resultEventAsName = resultEventAsName;
            this.resultEventProperty = resultEventProperty;
        }

        /// <summary>
        /// Returns the filter value representing the endpoint.
        /// </summary>
        /// <param name="matchedEvents">is the prior results</param>
        /// <returns>filter value</returns>
        public double? GetFilterValue(MatchedEventMap matchedEvents)
        {
            EventBean _event = matchedEvents.GetMatchingEvent(resultEventAsName);
            if (_event == null)
            {
                throw new IllegalStateException("Matching event named " +
                                                '\'' + resultEventAsName + "' not found in event result set");
            }

            Object value = _event[resultEventProperty];
            if (value == null)
            {
                return null;
            }
            return Convert.ToDouble(value);
        }

        /// <summary>Returns the tag name or stream name to use for the event property.</summary>
        /// <returns>tag name</returns>
	    public String ResultEventAsName
	    {
	        get { return resultEventAsName; }
	    }

	    /// <summary>Returns the name of the event property.</summary>
	    /// <returns>event property name</returns>
	    public String ResultEventProperty
	    {
	        get { return resultEventProperty; }
	    }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return "resultEventProp=" + resultEventAsName + "." + resultEventProperty;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override Boolean Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is RangeValueEventProp))
            {
                return false;
            }

            RangeValueEventProp other = (RangeValueEventProp)obj;
            if ((other.resultEventAsName.Equals(this.resultEventAsName)) &&
                 (other.resultEventProperty.Equals(this.resultEventProperty)))
            {
                return true;
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
            return resultEventProperty.GetHashCode();
        }
    }
}
