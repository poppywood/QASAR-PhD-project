///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.pattern;

namespace com.espertech.esper.filter
{
	/// <summary>
	/// Contains the filter criteria to sift through events. The filter criteria are the event class to look for and
	/// a set of parameters (attribute names, operators and constant/range values).
	/// </summary>
	public sealed class FilterSpecCompiled
	{
	    private readonly EventType eventType;
        private readonly String eventTypeAlias;
	    private readonly List<FilterSpecParam> parameters;

        /// <summary>Constructor - validates parameter list against event type, throws exception if invalidproperty names or mismatcing filter operators are found.</summary>
        /// <param name="eventType">is the event type</param>
        /// <param name="parameters">is a list of filter parameters</param>
        /// <param name="eventTypeAlias">is the alias name of the event type</param>
        public FilterSpecCompiled(EventType eventType, String eventTypeAlias, List<FilterSpecParam> parameters)
        {
            this.eventType = eventType;
            this.eventTypeAlias = eventTypeAlias;
            this.parameters = parameters;
        }

        /// <summary>
        /// Gets the event type alias name.
        /// </summary>
        /// <value>The event type alias.</value>
	    public string EventTypeAlias
	    {
	        get { return eventTypeAlias; }
	    }

	    /// <summary>
        /// Returns type of event to filter for.
        /// </summary>
        /// <value>The type of the event.</value>
        /// <returns>event type</returns>
	    public EventType EventType
	    {
            get { return eventType; }
	    }

        /// <summary>
        /// Returns list of filter parameters.
        /// </summary>
        /// <value>The parameters.</value>
        /// <returns>list of filter params</returns>
	    public List<FilterSpecParam> Parameters
	    {
            get { return parameters; }
	    }

        /// <summary>
        /// Returns the values for the filter, using the supplied result events to ask filter parameters
        /// for the value to filter for.
        /// </summary>
        /// <param name="matchedEvents">contains the result events to use for determining filter values</param>
        /// <returns>filter values</returns>
	    public FilterValueSet GetValueSet(MatchedEventMap matchedEvents)
	    {
	        List<FilterValueSetParam> valueList = new List<FilterValueSetParam>();

	        // Ask each filter specification parameter for the actual value to filter for
	        foreach (FilterSpecParam specParam in parameters)
	        {
	            Object filterForValue = specParam.GetFilterValue(matchedEvents);

	            FilterValueSetParam valueParam = new FilterValueSetParamImpl(specParam.PropertyName,
	                    specParam.FilterOperator, filterForValue);
	            valueList.Add(valueParam);
	        }
	        return new FilterValueSetImpl(eventType, valueList);
	    }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
	    public override String ToString()
	    {
	        StringBuilder buffer = new StringBuilder();
	        buffer.Append("FilterSpecCompiled type=" + this.eventType);
	        buffer.Append(" parameters=" + CollectionHelper.Render(parameters));
	        return buffer.ToString();
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

	        if (!(obj is FilterSpecCompiled))
	        {
	            return false;
	        }

	        FilterSpecCompiled other = (FilterSpecCompiled) obj;

	        if (this.eventType != other.eventType)
	        {
	            return false;
	        }
	        if (this.parameters.Count != other.parameters.Count)
	        {
	            return false;
	        }

	        IEnumerator<FilterSpecParam> iterOne = parameters.GetEnumerator();
            IEnumerator<FilterSpecParam> iterOther = other.parameters.GetEnumerator();
            while (iterOne.MoveNext())
            {
                if (!iterOther.MoveNext())
                {
                    return false;
                }
                else if (!Object.Equals(iterOne.Current, iterOther.Current))
                {
                    return false;
                }
            }

	        return true;
	    }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
	    public override int GetHashCode()
	    {
	        int hashCode = eventType.GetHashCode();
	        foreach (FilterSpecParam param in parameters)
	        {
	            hashCode = hashCode ^ param.PropertyName.GetHashCode();
	        }
	        return hashCode;
	    }
	}
} // End of namespace
