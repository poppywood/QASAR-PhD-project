using System;

using NUnit.Framework;

using org.apache.commons.logging;

using net.esper.compat;

namespace net.esper.schedule
{
    [TestFixture]
    public class TestScheduleComputeHelper
    {
        private static readonly String timeFormat = @"yyyy-MM-dd HH:mm:ss";

        [Test]
        public void testCompute()
        {
            ScheduleSpec spec = null;

            // Try next "5 minutes past the hour"
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.MINUTES, 5);

            checkCorrect(spec, "2004-12-9 15:45:01", "2004-12-9 16:05:00");
            checkCorrect(spec, "2004-12-9 16:04:59", "2004-12-9 16:05:00");
            checkCorrect(spec, "2004-12-9 16:05:00", "2004-12-9 17:05:00");
            checkCorrect(spec, "2004-12-9 16:05:01", "2004-12-9 17:05:00");
            checkCorrect(spec, "2004-12-9 16:05:01", "2004-12-9 17:05:00");
            checkCorrect(spec, "2004-12-9 23:58:01", "2004-12-10 00:05:00");

            // Try next "5, 10 and 15 minutes past the hour"
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.MINUTES, 5);
            spec.AddValue(ScheduleUnit.MINUTES, 10);
            spec.AddValue(ScheduleUnit.MINUTES, 15);

            checkCorrect(spec, "2004-12-9 15:45:01", "2004-12-9 16:05:00");
            checkCorrect(spec, "2004-12-9 16:04:59", "2004-12-9 16:05:00");
            checkCorrect(spec, "2004-12-9 16:05:00", "2004-12-9 16:10:00");
            checkCorrect(spec, "2004-12-9 16:10:00", "2004-12-9 16:15:00");
            checkCorrect(spec, "2004-12-9 16:14:59", "2004-12-9 16:15:00");
            checkCorrect(spec, "2004-12-9 16:15:00", "2004-12-9 17:05:00");

            // Try next "0 and 30 and 59 minutes past the hour"
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.MINUTES, 0);
            spec.AddValue(ScheduleUnit.MINUTES, 30);
            spec.AddValue(ScheduleUnit.MINUTES, 59);

            checkCorrect(spec, "2004-12-9 15:45:01", "2004-12-9 15:59:00");
            checkCorrect(spec, "2004-12-9 15:59:01", "2004-12-9 16:00:00");
            checkCorrect(spec, "2004-12-9 16:04:59", "2004-12-9 16:30:00");
            checkCorrect(spec, "2004-12-9 16:30:00", "2004-12-9 16:59:00");
            checkCorrect(spec, "2004-12-9 16:59:30", "2004-12-9 17:00:00");

            // Try minutes combined with seconds
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.MINUTES, 0);
            spec.AddValue(ScheduleUnit.MINUTES, 30);
            spec.AddValue(ScheduleUnit.SECONDS, 0);
            spec.AddValue(ScheduleUnit.SECONDS, 30);

            checkCorrect(spec, "2004-12-9 15:59:59", "2004-12-9 16:00:00");
            checkCorrect(spec, "2004-12-9 16:00:00", "2004-12-9 16:00:30");
            checkCorrect(spec, "2004-12-9 16:00:29", "2004-12-9 16:00:30");
            checkCorrect(spec, "2004-12-9 16:00:30", "2004-12-9 16:30:00");
            checkCorrect(spec, "2004-12-9 16:29:59", "2004-12-9 16:30:00");
            checkCorrect(spec, "2004-12-9 16:30:00", "2004-12-9 16:30:30");
            checkCorrect(spec, "2004-12-9 17:00:00", "2004-12-9 17:00:30");

            // Try hours combined with seconds
            spec = new ScheduleSpec();
            for (int i = 10; i <= 14; i++)
            {
                spec.AddValue(ScheduleUnit.HOURS, i);
            }
            spec.AddValue(ScheduleUnit.SECONDS, 15);

            checkCorrect(spec, "2004-12-9 15:59:59", "2004-12-10 10:00:15");
            checkCorrect(spec, "2004-12-10 10:00:15", "2004-12-10 10:01:15");
            checkCorrect(spec, "2004-12-10 10:01:15", "2004-12-10 10:02:15");
            checkCorrect(spec, "2004-12-10 14:01:15", "2004-12-10 14:02:15");
            checkCorrect(spec, "2004-12-10 14:59:15", "2004-12-11 10:00:15");

            // Try hours combined with minutes
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.HOURS, 9);
            spec.AddValue(ScheduleUnit.MINUTES, 5);

            checkCorrect(spec, "2004-12-9 15:59:59", "2004-12-10 9:05:00");
            checkCorrect(spec, "2004-11-30 15:59:59", "2004-12-1 9:05:00");
            checkCorrect(spec, "2004-11-30 9:04:59", "2004-11-30 9:05:00");
            checkCorrect(spec, "2004-12-31 9:05:01", "2005-01-01 9:05:00");

            // Try day of month as the 31st
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.DAYS_OF_MONTH, 31);

            checkCorrect(spec, "2004-11-30 15:59:59", "2004-12-31 0:00:00");
            checkCorrect(spec, "2004-12-30 15:59:59", "2004-12-31 0:00:00");
            checkCorrect(spec, "2004-12-31 00:00:00", "2004-12-31 0:01:00");
            checkCorrect(spec, "2005-01-01 00:00:00", "2005-01-31 0:00:00");
            checkCorrect(spec, "2005-02-01 00:00:00", "2005-03-31 0:00:00");
            checkCorrect(spec, "2005-04-01 00:00:00", "2005-05-31 0:00:00");

            // Try day of month as the 29st, for february testing
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.DAYS_OF_MONTH, 29);

            checkCorrect(spec, "2004-11-30 15:59:59", "2004-12-29 0:00:00");
            checkCorrect(spec, "2004-12-29 00:00:00", "2004-12-29 0:01:00");
            checkCorrect(spec, "2004-12-29 00:01:00", "2004-12-29 0:02:00");
            checkCorrect(spec, "2004-02-28 15:59:59", "2004-02-29 0:00:00");
            checkCorrect(spec, "2003-02-28 15:59:59", "2003-03-29 0:00:00");
            checkCorrect(spec, "2005-02-27 15:59:59", "2005-03-29 0:00:00");

            // Try 4:00 every day
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.HOURS, 16);
            spec.AddValue(ScheduleUnit.MINUTES, 0);

            checkCorrect(spec, "2004-10-01 15:59:59", "2004-10-01 16:00:00");
            checkCorrect(spec, "2004-10-01 00:00:00", "2004-10-01 16:00:00");
            checkCorrect(spec, "2004-09-30 16:00:00", "2004-10-01 16:00:00");
            checkCorrect(spec, "2004-12-30 16:00:00", "2004-12-31 16:00:00");
            checkCorrect(spec, "2004-12-31 16:00:00", "2005-01-01 16:00:00");

            // Try every weekday at 10 am - scrum time!
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.HOURS, 10);
            spec.AddValue(ScheduleUnit.MINUTES, 0);
            for (int i = 1; i <= 5; i++)
            {
                spec.AddValue(ScheduleUnit.DAYS_OF_WEEK, i);
            }

            checkCorrect(spec, "2004-12-05 09:50:59", "2004-12-06 10:00:00");
            checkCorrect(spec, "2004-12-06 09:59:59", "2004-12-06 10:00:00");
            checkCorrect(spec, "2004-12-07 09:50:00", "2004-12-07 10:00:00");
            checkCorrect(spec, "2004-12-08 09:00:00", "2004-12-08 10:00:00");
            checkCorrect(spec, "2004-12-09 08:00:00", "2004-12-09 10:00:00");
            checkCorrect(spec, "2004-12-10 09:50:50", "2004-12-10 10:00:00");
            checkCorrect(spec, "2004-12-11 00:00:00", "2004-12-13 10:00:00");
            checkCorrect(spec, "2004-12-12 09:00:50", "2004-12-13 10:00:00");
            checkCorrect(spec, "2004-12-13 09:50:50", "2004-12-13 10:00:00");
            checkCorrect(spec, "2004-12-13 10:00:00", "2004-12-14 10:00:00");
            checkCorrect(spec, "2004-12-13 10:00:01", "2004-12-14 10:00:00");

            // Every Monday and also on the 1st and 15th of each month, at midnight
            // (tests the or between DAYS_OF_MONTH and DAYS_OF_WEEK)
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.DAYS_OF_MONTH, 1);
            spec.AddValue(ScheduleUnit.DAYS_OF_MONTH, 15);
            spec.AddValue(ScheduleUnit.HOURS, 0);
            spec.AddValue(ScheduleUnit.MINUTES, 0);
            spec.AddValue(ScheduleUnit.SECONDS, 0);
            spec.AddValue(ScheduleUnit.DAYS_OF_WEEK, 1);

            checkCorrect(spec, "2004-12-05 09:50:59", "2004-12-06 00:00:00");
            checkCorrect(spec, "2004-12-06 00:00:00", "2004-12-13 00:00:00");
            checkCorrect(spec, "2004-12-07 01:20:00", "2004-12-13 00:00:00");
            checkCorrect(spec, "2004-12-12 23:00:00", "2004-12-13 00:00:00");
            checkCorrect(spec, "2004-12-13 23:00:00", "2004-12-15 00:00:00");
            checkCorrect(spec, "2004-12-14 23:00:00", "2004-12-15 00:00:00");
            checkCorrect(spec, "2004-12-15 23:00:00", "2004-12-20 00:00:00");
            checkCorrect(spec, "2004-12-18 23:00:00", "2004-12-20 00:00:00");
            checkCorrect(spec, "2004-12-20 00:01:00", "2004-12-27 00:00:00");
            checkCorrect(spec, "2004-12-27 00:01:00", "2005-01-01 00:00:00");
            checkCorrect(spec, "2005-01-01 00:01:00", "2005-01-03 00:00:00");
            checkCorrect(spec, "2005-01-03 00:01:00", "2005-01-10 00:00:00");
            checkCorrect(spec, "2005-01-10 00:01:00", "2005-01-15 00:00:00");
            checkCorrect(spec, "2005-01-15 00:01:00", "2005-01-17 00:00:00");
            checkCorrect(spec, "2005-01-17 00:01:00", "2005-01-24 00:00:00");
            checkCorrect(spec, "2005-01-24 00:01:00", "2005-01-31 00:00:00");
            checkCorrect(spec, "2005-01-31 00:01:00", "2005-02-01 00:00:00");

            // Every second month on every second weekday
            spec = new ScheduleSpec();
            for (int i = 1; i <= 12; i += 2)
            {
                spec.AddValue(ScheduleUnit.MONTHS, i);
            }
            for (int i = 0; i <= 6; i += 2)
            // Adds Sunday, Tuesday, Thursday, Saturday
            {
                spec.AddValue(ScheduleUnit.DAYS_OF_WEEK, i);
            }

            checkCorrect(spec, "2004-09-01 00:00:00", "2004-09-02 00:00:00"); // Sept 1 2004 is a Wednesday
            checkCorrect(spec, "2004-09-02 00:00:00", "2004-09-02 00:01:00");
            checkCorrect(spec, "2004-09-02 23:59:00", "2004-09-04 00:00:00");
            checkCorrect(spec, "2004-09-04 23:59:00", "2004-09-05 00:00:00"); // Sept 5 2004 is a Sunday
            checkCorrect(spec, "2004-09-05 23:57:00", "2004-09-05 23:58:00");
            checkCorrect(spec, "2004-09-05 23:58:00", "2004-09-05 23:59:00");
            checkCorrect(spec, "2004-09-05 23:59:00", "2004-09-07 00:00:00");
            checkCorrect(spec, "2004-09-30 23:58:00", "2004-09-30 23:59:00"); // Sept 30 in a Thursday
            checkCorrect(spec, "2004-09-30 23:59:00", "2004-11-02 00:00:00");

            // Every second month on every second weekday
            spec = new ScheduleSpec();
            for (int i = 1; i <= 12; i += 2)
            {
                spec.AddValue(ScheduleUnit.MONTHS, i);
            }
            for (int i = 0; i <= 6; i += 2)
            // Adds Sunday, Tuesday, Thursday, Saturday
            {
                spec.AddValue(ScheduleUnit.DAYS_OF_WEEK, i);
            }

            checkCorrect(spec, "2004-09-01 00:00:00", "2004-09-02 00:00:00"); // Sept 1 2004 is a Wednesday
            checkCorrect(spec, "2004-09-02 00:00:00", "2004-09-02 00:01:00");
            checkCorrect(spec, "2004-09-02 23:59:00", "2004-09-04 00:00:00");
            checkCorrect(spec, "2004-09-04 23:59:00", "2004-09-05 00:00:00"); // Sept 5 2004 is a Sunday
            checkCorrect(spec, "2004-09-05 23:57:00", "2004-09-05 23:58:00");
            checkCorrect(spec, "2004-09-05 23:58:00", "2004-09-05 23:59:00");
            checkCorrect(spec, "2004-09-05 23:59:00", "2004-09-07 00:00:00");

            // Every 5 seconds, between 9am and until 4pm, all weekdays except Saturday and Sunday
            spec = new ScheduleSpec();
            for (int i = 0; i <= 59; i += 5)
            {
                spec.AddValue(ScheduleUnit.SECONDS, i);
            }
            for (int i = 1; i <= 5; i++)
            {
                spec.AddValue(ScheduleUnit.DAYS_OF_WEEK, i);
            }
            for (int i = 9; i <= 15; i++)
            {
                spec.AddValue(ScheduleUnit.HOURS, i);
            }

            checkCorrect(spec, "2004-12-12 20:00:00", "2004-12-13 09:00:00"); // Dec 12 2004 is a Sunday
            checkCorrect(spec, "2004-12-13 09:00:01", "2004-12-13 09:00:05");
            checkCorrect(spec, "2004-12-13 09:00:05", "2004-12-13 09:00:10");
            checkCorrect(spec, "2004-12-13 09:00:11", "2004-12-13 09:00:15");
            checkCorrect(spec, "2004-12-13 09:00:15", "2004-12-13 09:00:20");
            checkCorrect(spec, "2004-12-13 09:00:24", "2004-12-13 09:00:25");
            checkCorrect(spec, "2004-12-13 15:59:50", "2004-12-13 15:59:55");
            checkCorrect(spec, "2004-12-13 15:59:55", "2004-12-14 09:00:00");
            checkCorrect(spec, "2004-12-14 12:27:35", "2004-12-14 12:27:40");
            checkCorrect(spec, "2004-12-14 12:29:55", "2004-12-14 12:30:00");
            checkCorrect(spec, "2004-12-17 00:03:00", "2004-12-17 09:00:00");
            checkCorrect(spec, "2004-12-17 15:59:50", "2004-12-17 15:59:55");
            checkCorrect(spec, "2004-12-17 15:59:55", "2004-12-20 09:00:00");

            // Feb 14, 12pm
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.MONTHS, 2);
            spec.AddValue(ScheduleUnit.DAYS_OF_MONTH, 14);
            spec.AddValue(ScheduleUnit.HOURS, 12);
            spec.AddValue(ScheduleUnit.MINUTES, 0);

            checkCorrect(spec, "2004-12-12 20:00:00", "2005-02-14 12:00:00");
            checkCorrect(spec, "2003-12-12 20:00:00", "2004-02-14 12:00:00");
            checkCorrect(spec, "2004-02-01 20:00:00", "2004-02-14 12:00:00");

            // Dec 31, 23pm and 50 seconds (countdown)
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.MONTHS, 12);
            spec.AddValue(ScheduleUnit.DAYS_OF_MONTH, 31);
            spec.AddValue(ScheduleUnit.HOURS, 23);
            spec.AddValue(ScheduleUnit.MINUTES, 59);
            spec.AddValue(ScheduleUnit.SECONDS, 50);

            checkCorrect(spec, "2004-12-12 20:00:00", "2004-12-31 23:59:50");
            checkCorrect(spec, "2004-12-31 23:59:55", "2005-12-31 23:59:50");

            // Feb 29 (2004 leap year), 01pm any minute and 02 seconds
            spec = new ScheduleSpec();
            spec.AddValue(ScheduleUnit.MONTHS, 2);
            spec.AddValue(ScheduleUnit.DAYS_OF_MONTH, 29);
            spec.AddValue(ScheduleUnit.HOURS, 1);
            spec.AddValue(ScheduleUnit.SECONDS, 2);

            checkCorrect(spec, "2004-02-18 00:00:00", "2004-02-29 01:00:02");
            checkCorrect(spec, "2004-02-29 01:00:02", "2004-02-29 01:01:02");
            checkCorrect(spec, "2004-02-29 01:59:02", "2008-02-29 01:00:02");
            checkCorrect(spec, "2008-02-29 01:59:02", "2012-02-29 01:00:02");
        }

        public virtual void checkCorrect(ScheduleSpec spec, String now, String expected)
        {
            DateTime nowDate = DateTime.Parse(now);
            DateTime expectedDate = DateTime.Parse(expected);

            long result = ScheduleComputeHelper.ComputeNextOccurance(spec, DateTimeHelper.TimeInMillis(nowDate));
            DateTime resultDate = DateTimeHelper.TimeFromMillis(result);

            if (!(resultDate.Equals(expectedDate)))
            {
                log.Debug(".checkCorrect Difference in result found, spec=" + spec);
                log.Debug(".checkCorrect      now=" + nowDate.ToString(timeFormat) + " long=" + DateTimeHelper.TimeInMillis(nowDate));
                log.Debug(".checkCorrect expected=" + expectedDate.ToString(timeFormat) + " long=" + DateTimeHelper.TimeInMillis(expectedDate));
                log.Debug(".checkCorrect   result=" + resultDate.ToString(timeFormat) + " long=" + DateTimeHelper.TimeInMillis(resultDate));
                Assert.IsTrue(false);
            }
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}