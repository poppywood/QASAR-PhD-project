using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.eql.parse;
using net.esper.pattern;
using net.esper.schedule;
using net.esper.util;

using org.apache.commons.logging;

namespace net.esper.pattern.observer
{
	/// <summary>
    /// Factory for 'crontab' observers that indicate truth when a time point was reached.
    /// </summary>
	
    public class TimerAtObserverFactory 
		: ObserverFactory
		, MetaDefItem
	{
        /// <summary>
        /// The schedule specification for the timer-at.
        /// </summary>
	    protected ScheduleSpec spec = null;

        /// <summary>
        /// Sets the observer object parameters.
        /// </summary>
	    public IList<Object> ObserverParameters
	    {
			set
			{
		    	if (log.IsDebugEnabled)
		        {
		            log.Debug(".setObserverParameters " + value);
		        }
	
		        if ((value.Count < 5) || (value.Count > 6))
		        {
		            throw new ObserverParameterException("Invalid number of parameters for timer:at");
		        }

                spec = ComputeValues(value);
			}
		}

        /// <summary>
        /// Computes the values.
        /// </summary>
        /// <param name="unitParameter">The unit parameter.</param>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        private static TreeSet<Int32> ComputeValues(Object unitParameter, ScheduleUnit unit)
        {
            if (unitParameter is Int32)
            {
                TreeSet<Int32> result = new TreeSet<Int32>();
                result.Add((Int32)unitParameter);
                return result;
            }

            NumberSetParameter numberSet = (NumberSetParameter)unitParameter;
            if (numberSet.IsWildcard(unit.Min(), unit.Min()))
            {
                return null;
            }

            Set<Int32> _result = numberSet.GetValuesInRange(unit.Min(), unit.Max());
            TreeSet<Int32> resultSorted = new TreeSet<Int32>();
            resultSorted.AddAll(_result);

            return resultSorted;
        }

        private static ScheduleSpec ComputeValues(IList<Object> args)
        {
            HashDictionary<ScheduleUnit, TreeSet<Int32>> unitMap = new HashDictionary<ScheduleUnit, TreeSet<Int32>>();
            Object minutes = args[0];
            Object hours = args[1];
            Object daysOfMonth = args[2];
            Object months = args[3];
            Object daysOfWeek = args[4];
            unitMap.Put(ScheduleUnit.MINUTES, ComputeValues(minutes, ScheduleUnit.MINUTES));
            unitMap.Put(ScheduleUnit.HOURS, ComputeValues(hours, ScheduleUnit.HOURS));
            TreeSet<Int32> resultMonths = ComputeValues(months, ScheduleUnit.MONTHS);
            if (daysOfWeek is CronParameter && daysOfMonth is CronParameter)
            {
                throw
                    new ObserverParameterException(
                        "Invalid combination between days of week and days of month fields for timer:at");
            }
            if (resultMonths != null && resultMonths.Count == 1)
            {
                // If other arguments are cronParameters, use it for later computations
                CronParameter parameter = null;
                if (daysOfMonth is CronParameter)
                {
                    parameter = ((CronParameter) daysOfMonth);
                }
                else if (daysOfWeek is CronParameter)
                {
                    parameter = ((CronParameter) daysOfWeek);
                }
                if (parameter != null)
                {
                    parameter.Month = resultMonths.First;
                }
            }
            TreeSet<Int32> resultDaysOfWeek = ComputeValues(daysOfWeek, ScheduleUnit.DAYS_OF_WEEK);
            TreeSet<Int32> resultDaysOfMonth = ComputeValues(daysOfMonth, ScheduleUnit.DAYS_OF_MONTH);
            if (resultDaysOfWeek != null && resultDaysOfWeek.Count == 1)
            {
                // The result is in the form "last xx of the month
                // Days of week is replaced by a wildcard and days of month is updated with
                // the computation of "last xx day of month".
                // In this case "days of month" parameter has to be a wildcard.
                if (resultDaysOfWeek.First > 6)
                {
                    if (resultDaysOfMonth != null)
                    {
                        throw
                            new ObserverParameterException(
                                "Invalid combination between days of week and days of month fields for timer:at");
                    }
                    resultDaysOfMonth = resultDaysOfWeek;
                    resultDaysOfWeek = null;
                }
            }
            if (resultDaysOfMonth != null && resultDaysOfMonth.Count == 1)
            {
                if (resultDaysOfWeek != null)
                {
                    throw
                        new ObserverParameterException(
                            "Invalid combination between days of week and days of month fields for timer:at");
                }
            }
            unitMap.Put(ScheduleUnit.DAYS_OF_WEEK, resultDaysOfWeek);
            unitMap.Put(ScheduleUnit.DAYS_OF_MONTH, resultDaysOfMonth);
            unitMap.Put(ScheduleUnit.MONTHS, resultMonths);
            
            if (args.Count > 5)
            {
                unitMap.Put(ScheduleUnit.SECONDS, ComputeValues(args[5], ScheduleUnit.SECONDS));
            }

            return new ScheduleSpec(unitMap);
        }

	    /// <summary>
        /// Make an observer instance.
        /// </summary>
        /// <param name="context">services that may be required by observer implementation</param>
        /// <param name="beginState">Start state for observer</param>
        /// <param name="observerEventEvaluator">receiver for events observed</param>
        /// <param name="stateNodeId">optional id for the associated pattern state node</param>
        /// <param name="observerState">state node for observer</param>
        /// <returns>observer instance</returns>
		public virtual EventObserver MakeObserver(PatternContext context,
												  MatchedEventMap beginState,
												  ObserverEventEvaluator observerEventEvaluator,
												  Object stateNodeId,
												  Object observerState)
		{
			return new TimerAtObserver(spec, context, beginState, observerEventEvaluator);
		}
		
		private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
}
