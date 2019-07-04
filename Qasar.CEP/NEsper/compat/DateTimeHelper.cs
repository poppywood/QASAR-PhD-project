using System;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Assistant class to help with conversions between Java-style and
    /// granularity dates and CLR-style DateTime.
    /// </summary>

    public class DateTimeHelper
    {
        /// <summary>
        /// Number of ticks per millisecond
        /// </summary>

        public const int TICKS_PER_MILLI = 10000;

        /// <summary>
        /// Number of nanoseconds per tick
        /// </summary>

        public const int NANOS_PER_TICK = 100;

        /// <summary>
        /// Converts ticks to milliseconds
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>

        public static long TicksToMillis(long ticks)
        {
            return ticks / TICKS_PER_MILLI;
        }

        /// <summary>
        /// Converts ticks to nanoseconds
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>

        public static long TicksToNanos(long ticks)
        {
            return ticks * NANOS_PER_TICK;
        }

        /// <summary>
        /// Converts milliseconds to ticks
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>

        public static long MillisToTicks(long millis)
        {
            return millis * TICKS_PER_MILLI;
        }

        /// <summary>
        /// Nanoses to ticks.
        /// </summary>
        /// <param name="nanos">The nanos.</param>
        public static long NanosToTicks(long nanos)
        {
            return nanos / NANOS_PER_TICK;
        }

        /// <summary>
        /// Converts milliseconds to DateTime 
        /// </summary>
        /// <param name="millis">The millis.</param>
        /// <returns></returns>
        public static DateTime MillisToDateTime(long millis)
        {
            return new DateTime(MillisToTicks(millis));
        }

        /// <summary>
        /// Gets the number of nanoseconds needed to represent
        /// the datetime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long TimeInNanos(DateTime dateTime)
        {
            return TicksToNanos(dateTime.Ticks);
        }

        /// <summary>
        /// Gets the number of milliseconds needed to represent
        /// the datetime.  This is needed to convert from Java
        /// datetime granularity (milliseconds) to CLR datetimes.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>

        public static long TimeInMillis(DateTime dateTime)
        {
            return TicksToMillis(dateTime.Ticks);
        }

        /// <summary>
        /// Gets the datetime that matches the number of milliseconds provided.
        /// As with TimeInMillis, this is needed to convert from Java datetime
        /// granularity to CLR granularity.
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>

        public static DateTime TimeFromMillis(long millis)
        {
            return new DateTime(MillisToTicks(millis));
        }

        /// <summary>
        /// Returns the current time in millis
        /// </summary>

        public static long GetCurrentTimeMillis()
        {
            return TimeInMillis(DateTime.Now);
        }

        /// <summary>
        /// Returns the current time in millis
        /// </summary>

        public static long CurrentTimeMillis
        {
            get { return TimeInMillis(DateTime.Now); }
        }

        /// <summary>
        /// Gets the current time in nanoseconds.
        /// </summary>
        /// <value>The current time nanos.</value>
        public static long CurrentTimeNanos
        {
            get { return TimeInNanos(DateTime.Now); }
        }

        /// <summary>
        /// Converts millis in CLR to millis in Java
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>

        public static long MillisToJavaMillis(long millis)
        {
            return millis - 62135575200000L;
        }

        /// <summary>
        /// Converts milliseconds in Java to milliseconds in CLR
        /// </summary>
        /// <param name="javaMillis"></param>
        /// <returns></returns>

        public static long JavaMillisToMillis(long javaMillis)
        {
            return javaMillis + 62135575200000L;
        }

        /// <summary>
        /// Returns a datetime for the week given on the specified day of the week.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <returns></returns>
        public static DateTime MoveToDayOfWeek( DateTime from, DayOfWeek dayOfWeek )
        {
            DayOfWeek current = from.DayOfWeek;
            return from.AddDays(dayOfWeek - current);    
        }

        /// <summary>
        /// Returns a datetime for the end of the month.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public static DateTime EndOfMonth(DateTime from)
        {
            return new DateTime(
                from.Year,
                from.Month,
                DateTime.DaysInMonth(from.Year, from.Month),
                0,
                0,
                0);
        }
    }
}
