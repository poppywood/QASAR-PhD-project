using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.pattern;

namespace com.espertech.esper.filter
{
	/// <summary>
    /// This class represents a single, constant value filter parameter in an <see cref="FilterSpecCompiled"/> filter specification.
    /// </summary>

    public sealed class FilterSpecParamConstant : FilterSpecParam
	{
		private readonly Object filterConstant;

		/// <summary> Constructor.</summary>
		/// <param name="propertyName">is the event property name
		/// </param>
		/// <param name="filterOperator">is the type of compare
		/// </param>
		/// <param name="filterConstant">contains the value to match against the event's property value
		/// </param>
		/// <throws>  ArgumentException if an operator was supplied that does not take a single constant value </throws>

        public FilterSpecParamConstant(String propertyName, FilterOperator filterOperator, Object filterConstant) :
            base(propertyName, filterOperator)
		{
			this.filterConstant = filterConstant;

			if (FilterOperatorHelper.IsRangeOperator( filterOperator ))
			{
				throw new ArgumentException("Illegal filter operator " + filterOperator + " supplied to " + "constant filter parameter");
			}
		}

	    /// <summary>Returns the constant value.</summary>
	    /// <returns>constant value</returns>
	    public Object FilterConstant
	    {
	        get { return filterConstant; }
	    }

        /// <summary>
        /// Return the filter parameter constant to filter for.
        /// </summary>
        /// <param name="matchedEvents">is the prior results that can be used to determine filter parameters</param>
        /// <returns>filter parameter constant's value</returns>
		public override Object GetFilterValue(MatchedEventMap matchedEvents)
		{
			return filterConstant;
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override String ToString()
		{
			return base.ToString() + " filterConstant=" + filterConstant;
		}

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
		public override bool Equals(Object obj)
		{
			if (this == obj)
			{
				return true;
			}

			if (!(obj is FilterSpecParamConstant))
			{
				return false;
			}

			FilterSpecParamConstant other = (FilterSpecParamConstant) obj;
			if (!base.Equals(other))
			{
				return false;
			}

            bool result = Object.Equals(this.filterConstant, other.filterConstant);
            return result;
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
            result = 31*result + (filterConstant != null ? filterConstant.GetHashCode() : 0);
            return result;
        }
	}
}
