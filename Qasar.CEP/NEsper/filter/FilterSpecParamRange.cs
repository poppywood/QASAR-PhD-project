using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.pattern;

namespace com.espertech.esper.filter
{
	/// <summary>
	/// This class represents a range filter parameter in an <see cref="FilterSpecCompiled"/> filter specification.
	/// </summary>

	public sealed class FilterSpecParamRange : FilterSpecParam
	{
		private readonly FilterSpecParamRangeValue min;
		private readonly FilterSpecParamRangeValue max;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">is the event property name</param>
        /// <param name="filterOperator">is the type of range operator</param>
        /// <param name="min">is the begin point of the range</param>
        /// <param name="max">is the end point of the range</param>
        /// <throws>  ArgumentException if an operator was supplied that does not take a double range value </throws>

		public FilterSpecParamRange(String propertyName, FilterOperator filterOperator, FilterSpecParamRangeValue min, FilterSpecParamRangeValue max):base(propertyName, filterOperator)
		{
			this.min = min;
			this.max = max;

			if ((! FilterOperatorHelper.IsRangeOperator( filterOperator )) &&
			    (! FilterOperatorHelper.IsInvertedRangeOperator( filterOperator )))
			{
				throw new ArgumentException("Illegal filter operator " + filterOperator + " supplied to " + "range filter parameter");
			}
		}

        /// <summary>
        /// Return the filter parameter constant to filter for.
        /// </summary>
        /// <param name="matchedEvents">is the prior results that can be used to determine filter parameters</param>
        /// <returns>filter parameter constant's value</returns>
	    public override Object GetFilterValue(MatchedEventMap matchedEvents)
	    {
	        double? begin = min.GetFilterValue(matchedEvents);
	        double? end = max.GetFilterValue(matchedEvents);
	        return new DoubleRange(begin, end);
	    }

	    /// <summary>Returns the lower endpoint.</summary>
	    /// <returns>lower endpoint</returns>
	    public FilterSpecParamRangeValue Min
	    {
	        get { return min; }
	    }

	    /// <summary>Returns the upper endpoint.</summary>
	    /// <returns>upper endpoint</returns>
	    public FilterSpecParamRangeValue Max
	    {
	        get { return max; }
	    }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override String ToString()
		{
			return base.ToString() + "  range=(min=" + min.ToString() + ",max=" + max.ToString() + ")";
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
		public  override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}

			if (!(obj is FilterSpecParamRange))
			{
				return false;
			}

			FilterSpecParamRange other = (FilterSpecParamRange) obj;
			if (!base.Equals(other))
			{
				return false;
			}

			if ((this.min.Equals(other.min) && (this.max.Equals(other.max))))
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
            int result = base.GetHashCode();
            result = 31 * result + (min != null ? min.GetHashCode() : 0);
            result = 31 * result + (max != null ? max.GetHashCode() : 0);
            return result;
        }
	}
}
