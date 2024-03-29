using System;
using System.Collections.Generic;
using System.Threading;

using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.filter
{
	/// <summary>
    /// Index for filter parameter constants for the range operators (range open/closed/half).
	/// The implementation is based on the SortedDictionary implementation of SortedDictionary and stores only expression
	/// parameter values of type DoubleRange.
	/// </summary>

	public sealed class FilterParamIndexRange : FilterParamIndexPropBase
	{
		private readonly TreeMap<DoubleRange, EventEvaluator> ranges;
        private readonly FastReaderWriterLock rangesRWLock;
		private double largestRangeValueDouble = Double.MinValue;

		/// <summary> Constructs the index for matching ranges.</summary>
		/// <param name="attributeName">is the name of the event attribute field
		/// </param>
		/// <param name="filterOperator">is the type of range
		/// </param>
		/// <param name="eventType">is type of events handled
		/// </param>

		public FilterParamIndexRange( String attributeName, FilterOperator filterOperator, EventType eventType )
			: base( attributeName, filterOperator, eventType )
		{
			ranges = new TreeMap<DoubleRange, EventEvaluator>( new DoubleRangeComparator() );
            rangesRWLock = new FastReaderWriterLock();

			if ( ! FilterOperatorHelper.IsRangeOperator( filterOperator ) )
			{
				throw new ArgumentException( "Invalid filter operator " + filterOperator );
			}
		}

        /// <summary>
        /// Gets or sets the <see cref="com.espertech.esper.filter.EventEvaluator"/> with the specified expression value.
        /// </summary>
        /// <value></value>
		public override EventEvaluator this[Object expressionValue]
		{
            get
            {
            	DoubleRange doubleRange = expressionValue as DoubleRange;
            	if ( doubleRange == null )
                {
                    throw new ArgumentException("Supplied expressionValue must be of type DoubleRange");
                }

                return ranges.Get(doubleRange);
            }

            set
            {
            	DoubleRange range = expressionValue as DoubleRange;
            	if ( range == null )
                {
                    throw new ArgumentException("Supplied expressionValue must be of type DoubleRange");
                }

		        if ((range.Max == null) || (range.Min == null))
		        {
		            return;     // endpoints null - we don't enter
		        }
		        
		        double max = range.Max.Value;
		        double min = range.Min.Value;
		
                if (Math.Abs(max - min) > largestRangeValueDouble)
                {
                    largestRangeValueDouble = Math.Abs(max - min);
                }

                ranges[range] = value;
            }
		}

        /// <summary>
        /// Remove the event evaluation instance for the given constant. Returns true if
        /// the constant was found, or false if not.
        /// The calling class must make sure that access to the underlying resource is protected
        /// for multi-threaded writes, the ReadWriteLock method must supply a lock for this purpose.
        /// </summary>
        /// <param name="filterConstant">is the value supplied in the filter paremeter</param>
        /// <returns>
        /// true if found and removed, false if not found
        /// </returns>
		public override bool Remove( Object filterConstant )
		{
			DoubleRange doubleRange = filterConstant as DoubleRange ;
			if ( doubleRange == null )
			{
				throw new ArgumentException( "Supplied filterConstant must be of type DoubleRange" ) ;
			}
			
			return ranges.Remove( doubleRange ) ;
		}

        /// <summary>
        /// Return the number of distinct filter parameter constants stored.
        /// The calling class must make sure that access to the underlying resource is protected
        /// for multi-threaded writes, the ReadWriteLock method must supply a lock for this purpose.
        /// </summary>
        /// <value></value>
        /// <returns> Number of entries in index
        /// </returns>
		public override int Count
		{
            get { return ranges.Count; }
		}

        /// <summary>
        /// Supplies the lock for protected access.
        /// </summary>
        /// <value></value>
        /// <returns> lock
        /// </returns>
        public override FastReaderWriterLock ReadWriteLock
		{
            get { return rangesRWLock; }
		}

        /// <summary>
        /// Matches the event.
        /// </summary>
        /// <param name="eventBean">The event bean.</param>
        /// <param name="matches">The matches.</param>
		public override void MatchEvent( EventBean eventBean, IList<FilterHandle> matches )
		{
			Object objAttributeValue = this.Getter.GetValue( eventBean );

            if (ExecutionPathDebugLog.isDebugEnabled && log.IsDebugEnabled)
            {
				log.Debug(
					".match Finding range matches, attribute=" + this.PropertyName + 
					"  attrValue=" + objAttributeValue );
			}

			if ( objAttributeValue == null )
			{
				return;
			}

			double attributeValue = Convert.ToDouble( objAttributeValue );

			DoubleRange rangeStart = new DoubleRange( attributeValue - largestRangeValueDouble, attributeValue );
			DoubleRange rangeEnd = new DoubleRange( attributeValue, Double.MaxValue );

			TreeMap<DoubleRange, EventEvaluator> subMap = ranges.Range( rangeStart, rangeEnd );

			// For not including either endpoint
			// A bit awkward to duplicate the loop code, however better than checking the bool many times over
			// This may be a bit of an early performance optimization - the optimizer after all may do this better
			if ( this.FilterOperator == FilterOperator.RANGE_OPEN )
			// include neither endpoint
			{
				foreach ( KeyValuePair<DoubleRange, EventEvaluator> entry in subMap )
				{
					if ( ( attributeValue > entry.Key.Min ) && ( attributeValue < entry.Key.Max ) )
					{
						entry.Value.MatchEvent( eventBean, matches );
					}
				}
			}
			else if ( this.FilterOperator == FilterOperator.RANGE_CLOSED )
			// include all endpoints
			{
				foreach ( KeyValuePair<DoubleRange, EventEvaluator> entry in subMap )
				{
					if ( ( attributeValue >= entry.Key.Min ) && ( attributeValue <= entry.Key.Max ) )
					{
						entry.Value.MatchEvent( eventBean, matches );
					}
				}
			}
			else if ( this.FilterOperator == FilterOperator.RANGE_HALF_CLOSED )
			// include high endpoint not low endpoint
			{
				foreach ( KeyValuePair<DoubleRange, EventEvaluator> entry in subMap )
				{
					if ( ( attributeValue > entry.Key.Min ) && ( attributeValue <= entry.Key.Max ) )
					{
						entry.Value.MatchEvent( eventBean, matches );
					}
				}
			}
			else if ( this.FilterOperator == FilterOperator.RANGE_HALF_OPEN )
			// include low endpoint not high endpoint
			{
				foreach ( KeyValuePair<DoubleRange, EventEvaluator> entry in subMap )
				{
					if ( ( attributeValue >= entry.Key.Min ) && ( attributeValue < entry.Key.Max ) )
					{
						entry.Value.MatchEvent( eventBean, matches );
					}
				}
			}
			else
			{
				throw new IllegalStateException( "Invalid filter operator " + this.FilterOperator );
			}
		}

		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
}
