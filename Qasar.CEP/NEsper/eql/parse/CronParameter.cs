///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;

using net.esper.compat;

namespace net.esper.eql.parse
{
    /// <summary>
    /// Hold parameters for timer:at.
    /// </summary>
	public class CronParameter : NumberSetParameter
    {
	    private readonly CronOperator _operator;
        private readonly int? day;
        private int? month;

        private DateTime currDate;

	    /// <summary>
	    /// Enumeration for special keywords in crontab timer.
	    /// </summary>
	    public enum CronOperator
	    {
	        /// <summary>
	        /// Last day of week or month.
	        /// </summary>
	        last,
	        /// <summary>
	        /// Weekday (nearest to a date)
	        /// </summary>
	        w,
	        /// <summary>
	        /// Last weekday in a month
	        /// </summary>
	        lw 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CronParameter"/> class.
        /// </summary>
        /// <param name="cronOperator">The cron operator.</param>
        /// <param name="day">The day text.</param>

        public CronParameter(String cronOperator, String day)
        {
            this._operator = AssignOperator(cronOperator);
            this.currDate = DateTime.Now;

            if (day != null)
            {
                this.day = Int32.Parse(day);
            }
        }

        /// <summary>
        /// Sets the month.
        /// </summary>
        /// <value>The month.</value>
	    public int Month
        {
            set { this.month = value; }
        }

        /// <summary>
        /// Returns true if all values between and including min and max are supplied by the parameter.
        /// </summary>
        /// <param name="min">lower end of range</param>
        /// <param name="max">upper end of range</param>
        /// <returns>
        /// true if parameter specifies all int values between min and max, false if not
        /// </returns>
        public bool IsWildcard(int min, int max)
        {
            return false;
        }

        /// <summary>
        /// Return a set of int values representing the value of the parameter for the given range.
        /// </summary>
        /// <param name="min">lower end of range</param>
        /// <param name="max">upper end of range</param>
        /// <returns>set of integer</returns>
        public Set<Int32> GetValuesInRange(int min, int max)
        {
            Set<Int32> values = new HashSet<Int32>();
            if (((min != 0) && (min != 1)) ||
                ((max != 6) && (max != 31)))
            {
                throw new ArgumentException("Invalid usage for timer:at");
            }

            switch (_operator)
            {
                case CronOperator.last:
                    // If max=6, determine last day of Week (In US Saturday=7)
                    if ((min == 0) && (max == 6))
                    {
                        if (day == null)
                        {
                            values.Add(DetermineLastDayOfWeek());
                        }
                        else
                        {
                            values.Add(DetermineLastDayOfWeekInMonth());
                        }
                    }
                    else if ((min == 1) && (max == 31))
                    {
                        // "Last day of month" or "the last xxx day of the month"
                        if (day == null)
                        {
                            values.Add(DetermineLastDayOfMonth());
                        }
                        else
                        {
                            values.Add(DetermineLastDayOfWeekInMonth());
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Invalid value for last operator");
                    }
                    break;
                case CronOperator.lw:
                    values.Add(DetermineLastWeekDayOfMonth());
                    break;
                case CronOperator.w:
                    values.Add(DetermineLastWeekDayOfMonth());
                    break;
                default:
                    throw new ArgumentException("Invalid special operator for observer");
            }
            return values;
        }

        /// <summary>
        /// Determines the last day of month.
        /// </summary>
        /// <returns></returns>
	    private int DetermineLastDayOfMonth()
        {
	        SetTime();

            // Get the globalization calendar
	        Calendar gCalendar = CultureInfo.CurrentCulture.Calendar;
            // Get the number of days in the current month and year
	        int daysInMonth = gCalendar.GetDaysInMonth(currDate.Year, currDate.Month);
	        // Advance the current date time
            currDate = new DateTime(
	            currDate.Year,
	            currDate.Month,
	            daysInMonth);

            // ?? isnt this just daysInMonth?
	        return currDate.Day;
	    }

        /// <summary>
        /// Determines the last day of week in the month.
        /// </summary>
        /// <returns></returns>
	    private int DetermineLastDayOfWeekInMonth()
        {
	        if (day == null)
            {
	            return DetermineLastDayOfMonth();
	        }

            if (day < 0 || day > 7)
            {
                throw new ArgumentException("Last xx day of the month has to be a day of week (0-7)");
            }

	        DayOfWeek dayOfWeek = GetDayOfWeek();

	        SetTime();

            // Get the globalization calendar
            Calendar gCalendar = CultureInfo.CurrentCulture.Calendar;

            int daysInMonth = gCalendar.GetDaysInMonth(currDate.Year, currDate.Month);
            currDate = new DateTime(
                currDate.Year,
                currDate.Month,
                daysInMonth);

            int dayDiff = (currDate.DayOfWeek - dayOfWeek);
	        if (dayDiff > 0) {
	            currDate = currDate.AddDays(-dayDiff);
	        } else if (dayDiff < 0) {
	            currDate = currDate.AddDays(-7 - dayDiff);
	        }

	        return currDate.Day;
	    }

        /// <summary>
        /// Determines the last day of week.
        /// </summary>
        /// <returns></returns>
	    private int DetermineLastDayOfWeek()
        {
	        SetTime();
            return (int) DayOfWeek.Saturday;
            
            //calendar.set(Calendar.DAY_OF_WEEK, Calendar.SATURDAY);
            //return calendar.get(Calendar.DAY_OF_WEEK) - 1;
	    }

        /// <summary>
        /// Gets the day of week.
        /// </summary>
        /// <returns></returns>
	    private DayOfWeek GetDayOfWeek()
        {
	        SetTime();
            return
                day.HasValue ?
                (DayOfWeek) day.Value :
                DayOfWeek.Saturday;
	    }

        /// <summary>
        /// Determines the last week day of the month.
        /// </summary>
        /// <returns></returns>
	    private int DetermineLastWeekDayOfMonth()
        {
            int computeDay;
            
            if ( day == null )
            {
                computeDay = DetermineLastDayOfMonth();
            }
            else
            {
                computeDay = day.Value;
            }

            SetTime();

            // Advance the date time to the computeDay
            try
            {
                currDate = new DateTime(currDate.Year, currDate.Month, computeDay);
            }
            catch( ArgumentOutOfRangeException )
            {
                throw new ArgumentException("Invalid day for " + currDate.Month);
            }

            DayOfWeek dayOfWeek = currDate.DayOfWeek;
            if (( dayOfWeek >= DayOfWeek.Monday ) &&
                ( dayOfWeek <= DayOfWeek.Friday ))
            {
                return computeDay;
            }

            if (dayOfWeek == DayOfWeek.Saturday)
            {
                if (computeDay == 1)
                {
                    currDate = currDate.AddDays(2);
                }
                else
                {
                    currDate = currDate.AddDays(-1);
                }
            }

            if (dayOfWeek == DayOfWeek.Sunday)
            {
                if ((computeDay == 28) ||
                    (computeDay == 29) ||
                    (computeDay == 30) ||
                    (computeDay == 31))
                {
                    currDate = currDate.AddDays(-2);
                }
                else
                {
                    currDate = currDate.AddDays(2);
                }
            }

            return currDate.Day;
	    }

        /// <summary>
        /// Assigns the operator.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
	    private static CronOperator AssignOperator(String name)
	    {
            switch( name.ToLowerInvariant() )
            {
                case "last":
                case "lastoperator":
	                return CronOperator.last;
                case "weekdayoperator":
	               return CronOperator.w;
                case "lastweekday":
    	            return CronOperator.lw;
                default:
                    throw new ArgumentException("name");
	        }
	    }

	    private void PrintTime()
        {
	        DateTime _date = currDate;
	        Console.WriteLine(_date.ToLongDateString());
            //String aMonth = new SimpleDateFormat("MMM").format(date);
            //String aDay = new SimpleDateFormat("EEE").format(date);
            //Console.WriteLine("This day: " + aDay + " of " + aMonth + " " + calendar.get(Calendar.YEAR));
	    }

        /// <summary>
        /// Sets the time.
        /// </summary>
	    private void SetTime()
        {
	        DateTime date = DateTime.Now;
	        currDate = new DateTime(date.Year, month ?? date.Month, 1);
	        //printTime();
	    }
	}
} // End of namespace
