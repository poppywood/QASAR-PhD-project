using System;
using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.pattern;
using com.espertech.esper.util;

namespace com.espertech.esper.filter
{
    /// <summary>
    /// This class represents one filter parameter in an <see cref="FilterSpecCompiled"/> filter specification.
    /// <para>Each filerting parameter has an attribute name and operator type.</para>
    /// </summary>

    public abstract class FilterSpecParam : MetaDefItem
    {
        /// <summary> Returns the property name for the filter parameter.</summary>
        /// <returns> property name
        /// </returns>
        virtual public String PropertyName
        {
            get { return propertyName; }
        }
        
        /// <summary> Returns the filter operator type.</summary>
        /// <returns> filter operator type
        /// </returns>
        virtual public FilterOperator FilterOperator
        {
            get { return filterOperator; }
        }

        private readonly String propertyName;
        private readonly FilterOperator filterOperator;

        internal FilterSpecParam(String propertyName, FilterOperator filterOperator)
        {
            this.propertyName = propertyName;
            this.filterOperator = filterOperator;
        }

        /// <summary> Return the filter parameter constant to filter for.</summary>
        /// <param name="matchedEvents">is the prior results that can be used to determine filter parameters
        /// </param>
        /// <returns> filter parameter constant's value
        /// </returns>
        public abstract Object GetFilterValue(MatchedEventMap matchedEvents);

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        
        public override String ToString()
        {
            return "FilterSpecParam" + " property=" + propertyName + " filterOp=" + filterOperator;
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

            if (!(obj is FilterSpecParam))
            {
                return false;
            }

            FilterSpecParam other = (FilterSpecParam)obj;

            if (this.propertyName != other.propertyName)
            {
                return false;
            }
            if (this.filterOperator != other.filterOperator)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            int result;
            result = propertyName.GetHashCode();
            result = 31*result + filterOperator.GetHashCode();
            return result;
        }
    }
}
