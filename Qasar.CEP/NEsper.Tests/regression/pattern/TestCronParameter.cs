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

using net.esper.client;
using net.esper.client.time;
using net.esper.compat;
using net.esper.events;
using net.esper.regression.support;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.regression.pattern
{
    [TestFixture]
	public class TestCronParameter : SupportBeanConstants
	{
	    private EPStatement patternStmt;
	    private String expressionText;
	    private static long baseTime;
	    private EventCollection testData;
	    private EventExpressionCase testCase;
	    private SupportUpdateListener listener;
        private DateTime calendar;

        public const int JANUARY = 1;
        public const int FEBRUARY = 2;
		public const int MARCH = 3;
		public const int APRIL = 4;
		public const int MAY = 5;
		public const int JUNE = 6;
		public const int JULY = 7;
		public const int AUGUST = 8;
		public const int SEPTEMBER = 9;
		public const int OCTOBER = 10;
		public const int NOVEMBER = 11;
		public const int DECEMBER = 12;
	
		[SetUp]
	    public void setUp()
	    {
	        listener = new SupportUpdateListener();
	        testCase = null;
	        calendar = DateTime.Now;
	    }

		[Test]
	    public void testOperator()
	    {
	        // Observer for last day of current month
            calendar = new DateTime(2007, GetCurrentMonth(), GetLastDayOfMonth(), 8, 00, 00);
	        PrintCurrentTime(calendar);
	        baseTime = DateTimeHelper.TimeInMillis(calendar);
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *, last,*,*,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for last day of August 2007
	        // For Java: January=0, February=1, March=2, April=3, May=4, June=5,
	        //            July=6, August=7, September=8, November=9, October=10, December=11
            // For .NET: January=1, February=2, March=3, April=4, May=5, June=6,
            //            July=7, August=8, September=9, November=10, October=11, December=12
            // For Esper: January=1, February=2, March=3, April=4, May=5, June=6,
	        //            July=7, August=8, September=9, November=10, October=11, December=12
	        calendar = new DateTime(2007, AUGUST, 31, 8, 00, 00);
	        PrintCurrentTime(calendar);
	        baseTime = DateTimeHelper.TimeInMillis(calendar);
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *, last,8,*,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for last day of February 2007
            calendar = new DateTime(2007, FEBRUARY, 28, 8, 00, 00);
	        PrintCurrentTime(calendar);
            baseTime = DateTimeHelper.TimeInMillis(calendar);
            testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *, last,2,*,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for last day of week (Saturday)
	        calendar = DateTime.Now;
	        calendar = new DateTime(
                calendar.Year,
                calendar.Month,
                calendar.Day,
                calendar.Hour,
                calendar.Minute,
                calendar.Second,
	            0);
	        calendar = DateTimeHelper.MoveToDayOfWeek(calendar, DayOfWeek.Saturday);

            PrintCurrentTime(calendar);
            baseTime = DateTimeHelper.TimeInMillis(calendar);
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *,*,*,last,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for last Friday of current month,
	        // 0=Sunday, 1=Monday, 2=Tuesday, 3=Wednesday, 4= Thursday, 5=Friday, 6=Saturday
            calendar = new DateTime(2007, GetCurrentMonth(), GetLastDayOfWeekInMonth(5), 8, 00, 00);
	        PrintCurrentTime(calendar);
            baseTime = DateTimeHelper.TimeInMillis(calendar);
            testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *,*,*,5 last,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for last Sunday of month, 0 = Sunday
	        calendar = new DateTime(2007, GetCurrentMonth(), GetLastDayOfWeekInMonth(0), 8, 00, 00);
	        PrintCurrentTime(calendar);
            baseTime = DateTimeHelper.TimeInMillis(calendar);
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *,*,*,0 last,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for last Friday of June
            calendar = new DateTime(2007, JUNE, 29, 8, 00, 00);
	        PrintCurrentTime(calendar);
	        baseTime = DateTimeHelper.TimeInMillis(calendar);;
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *,*,6,5 last,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for last weekday of the current month
            calendar = new DateTime(2007, GetCurrentMonth(), GetLastWeekDayOfMonth(null), 8, 00, 00);
	        PrintCurrentTime(calendar);
	        baseTime = DateTimeHelper.TimeInMillis(calendar);;
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *,lastweekday,*,*,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for last weekday of September 2007, it's Friday September 28th
            calendar = new DateTime(2007, SEPTEMBER, 28, 10, 00, 00);
	        PrintCurrentTime(calendar);
	        baseTime = DateTimeHelper.TimeInMillis(calendar);;
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *,lastweekday,9,*,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for last weekday of February, it's Wednesday February 28th
	        calendar = new DateTime(2007, FEBRUARY, 28, 8, 00, 00);
	        PrintCurrentTime(calendar);
	        baseTime = DateTimeHelper.TimeInMillis(calendar);;
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *,lastweekday,2,*,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for nearest weekday for current month on the 10th
            calendar = new DateTime(2007, GetCurrentMonth(), GetLastWeekDayOfMonth(10), 8, 00, 00);
	        PrintCurrentTime(calendar);
	        baseTime = DateTimeHelper.TimeInMillis(calendar);;
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *,10 weekday,*,*,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for nearest weekday of September 1st (Saturday), it's Monday September 3rd (no "jump" over month)
            calendar = new DateTime(2007, SEPTEMBER, 3, 8, 00, 00);
	        PrintCurrentTime(calendar);
	        baseTime = DateTimeHelper.TimeInMillis(calendar);;
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *,1 weekday,9,*,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();

	        // Observer for nearest weekday of September 30th (Sunday), it's Friday September 28th (no "jump" over month)
            calendar = new DateTime(2007, SEPTEMBER, 28, 8, 00, 00);
	        PrintCurrentTime(calendar);
	        baseTime = DateTimeHelper.TimeInMillis(calendar);;
	        testData = GetEventSet(baseTime, 1000 * 60 * 10);
	        expressionText = "timer:at(*, *,30 weekday,9,*,*)";
	        testCase = new EventExpressionCase(expressionText);
	        testCase.Add("A1");
	        RunTestEvent();
	    }

	    private void RunTestEvent()
	    {
	        int totalEventsReceived = 0;

	        EPServiceProvider serviceProvider = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
	        serviceProvider.Initialize();

	        EPRuntime runtime = serviceProvider.EPRuntime;
	        runtime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
	        // Send the start time to the runtime
	        TimerEvent startTime = new CurrentTimeEvent(baseTime);
	        runtime.SendEvent(startTime);
	        log.Debug(".runTest Start time is " + startTime);
	        try
	        {
	            patternStmt = serviceProvider.EPAdministrator.CreatePattern(expressionText);
	        }
	        catch (Exception ex)
	        {
	            log.Fatal(".runTest Failed to create statement for pattern expression=" + expressionText, ex);
	            Assert.Fail();
	        }
	        patternStmt.AddListener(listener);

	        // Send actual test events
	        foreach (KeyValuePair<String, Object> entry in testData)
	        {
	            String eventId = entry.Key;

	            // Manipulate the time when this event was send
	            if (testData.GetTime(eventId) != null)
	            {
	                TimerEvent currentTimeEvent = new CurrentTimeEvent(testData.GetTime(eventId).Value);
	                runtime.SendEvent(currentTimeEvent);
	                log.Debug(".runTest Sending event " + entry.Key
	                        + " = " + entry.Value +
	                        "  timed " + currentTimeEvent);
	            }

	            // Send event itself
	            runtime.SendEvent(entry.Value);

	            // Check expected results for this event
	            CheckResults(eventId);

	            // Count and clear the list of events that each listener has received
	            totalEventsReceived += CountListenerEvents();
	        }

	        // Count number of expected matches
	        int totalExpected = 0;
	        foreach (IList<EventDescriptor> events in testCase.ExpectedResults.Values)
	        {
	            totalExpected += events.Count;
	        }

	        if (totalExpected != totalEventsReceived)
	        {
	            log.Debug(".test Count expected does not match count received, expected=" + totalExpected +
	                    " received=" + totalEventsReceived);
	            Assert.IsTrue(false);
	        }

	        // Kill expression
	        patternStmt.RemoveAllListeners();

	        // Send test events again to also test that all were indeed killed
	        foreach (KeyValuePair<String, Object> entry in testData)
	        {
	            runtime.SendEvent(entry.Value);
	        }

	        if (listener.NewDataList.Count > 0)
	        {
	            log.Debug(".test A match was received after stopping all expressions");
	            Assert.IsTrue(false);
	        }

	    }

	    private void CheckResults(String eventId)
	    {
	        log.Debug(".checkResults Checking results for event " + eventId);

	        String expressionText = patternStmt.Text;

	        LinkedDictionary<String, IList<EventDescriptor>> allExpectedResults = testCase.ExpectedResults;
	        EventBean[] receivedResults = listener.LastNewData;

	        // If nothing at all was expected for this event, make sure nothing was received
	        if (!(allExpectedResults.ContainsKey(eventId)))
	        {
	            if ((receivedResults != null) && (receivedResults.Length > 0))
	            {
	                log.Debug(".checkResults Incorrect result for expression : " + expressionText);
	                log.Debug(".checkResults Expected no results for event " + eventId + ", but received " + receivedResults.Length + " events");
	                log.Debug(".checkResults Received, have " + receivedResults.Length + " entries");
	                PrintList(receivedResults);
	                Assert.IsFalse(true);
	            }
	        }

            IList<EventDescriptor> expectedResults = allExpectedResults.Get(eventId);

	        // Compare the result lists, not caring about the order of the elements
	        if (!(CompareLists(receivedResults, expectedResults)))
	        {
	            log.Debug(".checkResults Incorrect result for expression : " + expressionText);
	            log.Debug(".checkResults Expected size=" + expectedResults.Count + " received size=" + (receivedResults == null ? 0 : receivedResults.Length));

	            log.Debug(".checkResults Expected, have " + expectedResults.Count + " entries");
	            PrintList(expectedResults);
	            log.Debug(".checkResults Received, have " + (receivedResults == null ? 0 : receivedResults.Length) + " entries");
	            PrintList(receivedResults);

	            Assert.IsFalse(true);
	        }
	    }

	    private bool CompareLists(EventBean[] receivedResults,
                                  IList<EventDescriptor> expectedResults)
	    {
	        int receivedSize = (receivedResults == null) ? 0 : receivedResults.Length;
	        if (expectedResults.Count != receivedSize)
	        {
	            return false;
	        }

	        // To make sure all received events have been expected
	        IList<EventDescriptor> expectedResultsClone = new List<EventDescriptor>(expectedResults);

	        // Go through the list of expected results and remove from received result list if found
	        foreach (EventDescriptor desc in expectedResults)
	        {
	            EventDescriptor foundMatch = null;

	            foreach (EventBean received in receivedResults)
	            {
	                if (CompareEvents(desc, received))
	                {
	                    foundMatch = desc;
	                    break;
	                }
	            }

	            // No match between expected and received
	            if (foundMatch == null)
	            {
	                return false;
	            }

	            expectedResultsClone.Remove(foundMatch);
	        }

	        // Any left over received results also invalidate the test
	        if (expectedResultsClone.Count > 0)
	        {
	            return false;
	        }
	        return true;
	    }

	    private static bool CompareEvents(EventDescriptor eventDesc, EventBean eventBean)
	    {
	        foreach (KeyValuePair<String, Object> entry in eventDesc.EventProperties)
	        {
	            if (!(eventBean.Get(entry.Key) == (entry.Value)))
	            {
	                return false;
	            }
	        }
	        return true;
	    }

	    private void PrintList(IList<EventDescriptor> events)
	    {
	        int index = 0;
	        foreach (EventDescriptor desc in events)
	        {
	            StringBuilder buffer = new StringBuilder();
	            int count = 0;

	            foreach (KeyValuePair<String, Object> entry in desc.EventProperties)
	            {
	                buffer.Append(" (" + (count++) + ") ");
	                buffer.Append("tag=" + entry.Key);

	                String id = FindValue(entry.Value);
	                buffer.Append("  eventId=" + id);
	            }

	            log.Debug(".printList (" + index + ") : " + buffer.ToString());
	            index++;
	        }
	    }

	    private void PrintList(EventBean[] events)
	    {
	        if (events == null)
	        {
	            log.Debug(".printList : null-value events array");
	            return;
	        }

	        log.Debug(".printList : " + events.Length + " elements...");
	        for (int i = 0; i < events.Length; i++)
	        {
	            log.Debug("  " + EventBeanUtility.PrintEvent(events[i]));
	        }
	    }

	    private String FindValue(Object value)
	    {
	        foreach (KeyValuePair<String, Object> entry in testData)
	        {
	            if (value == entry.Value)
	            {
	                return entry.Key;
	            }
	        }
	        return null;
	    }

	    private int CountListenerEvents()
	    {
	        int count = 0;
	        foreach (EventBean[] events in listener.NewDataList)
	        {
	            count += events.Length;
	        }
	        listener.Reset();
	        return count;
	    }

	    private EventCollection GetEventSet(long baseTime, long numMSecBetweenEvents)
	    {
	        LinkedDictionary<String, Object> testData = new LinkedDictionary<String, Object>();
	        testData.Put("A1", new SupportBean_A("A1"));
	        LinkedDictionary<String, long> times = MakeExternalClockTimes(testData, baseTime, numMSecBetweenEvents);
	        return new EventCollection(testData, times);
	    }

	    private LinkedDictionary<String, long> MakeExternalClockTimes(LinkedDictionary<String, Object> testData,
	                                                               long baseTime,
	                                                               long numMSecBetweenEvents)
	    {
	        LinkedDictionary<String, long> testDataTimers = new LinkedDictionary<String, long>();

	        testDataTimers.Put(EventCollection.ON_START_EVENT_ID, baseTime);

	        foreach (String id in testData.Keys)
	        {
	            baseTime += numMSecBetweenEvents;
	            testDataTimers.Put(id, baseTime);
	        }

	        return testDataTimers;
	    }

	    private int GetCurrentMonth()
	    {
	        SetTime();
	        return calendar.Month;
	    }

	    private int GetLastDayOfMonth()
	    {
	        SetTime();

	        int daysInMonth = DateTime.DaysInMonth(calendar.Year, calendar.Month);
	        calendar = new DateTime(calendar.Year, calendar.Month, daysInMonth);
	        return calendar.Day;
	    }

	    private int GetLastDayOfWeekInMonth(int day)
	    {
	        if (day < 0 || day > 7)
	        {
	            throw new ArgumentException("Last xx day of the month has to be a day of week (0-7)");
	        }
	        DayOfWeek dayOfWeek = GetDayOfWeek(day);
	        SetTime();
	        calendar = DateTimeHelper.EndOfMonth(calendar);
	        int dayDiff = calendar.DayOfWeek - dayOfWeek;
	        if (dayDiff > 0)
	        {
	            calendar = calendar.AddDays(-dayDiff);
	        }
	        else if (dayDiff < 0)
	        {
	            calendar = calendar.AddDays(-7 - dayDiff);
	        }
	        return calendar.Day;
	    }

	    private int GetLastWeekDayOfMonth(int? day)
	    {
	        int computeDay = day ?? GetLastDayOfMonth();

	        SetTime();
	        if (!CheckDayValidInMonth(computeDay, calendar.Month, calendar.Year))
	        {
	            throw new ArgumentException("Invalid day for " + calendar.Month);
	        }

	        calendar = new DateTime(calendar.Year, calendar.Month, computeDay);
	        DayOfWeek dayOfWeek = calendar.DayOfWeek;
	        if ((dayOfWeek >= DayOfWeek.Monday) && (dayOfWeek <= DayOfWeek.Friday))
	        {
	            return computeDay;
	        }
	        if (dayOfWeek == DayOfWeek.Saturday)
	        {
	            if (computeDay == 1)
	            {
                    calendar = calendar.AddDays(2);
                }
	            else
	            {
                    calendar = calendar.AddDays(-1);
	            }
	        }
            if (dayOfWeek == DayOfWeek.Sunday)
	        {
	            if ((computeDay == 28) || (computeDay == 29) || (computeDay == 30) || (computeDay == 31))
	            {
	                calendar = calendar.AddDays(-2);
	            }
	            else
	            {
                    calendar = calendar.AddDays(2);
	            }
	        }
	        return calendar.Day;
	    }

	    private DayOfWeek GetDayOfWeek(int day)
	    {
	        SetTime();
	        int delta = day - (int) calendar.DayOfWeek;
	        calendar = calendar.AddDays(delta);
	        return calendar.DayOfWeek;
	    }

	    private void SetTime()
	    {
	        DateTime date = DateTime.Now;
            calendar = new DateTime( 
                date.Year,
                date.Month,
                1,
                0,
                0,
                0);
	    }

	    private static bool CheckDayValidInMonth(int day, int month, int year)
	    {
	        try
	        {
	            new DateTime(year, month, day);
	        }
	        catch (ArgumentOutOfRangeException e)
	        {
	            return false;
	        }
	        return true;
	    }

	    private void PrintCurrentTime(DateTime date)
	    {
	        Console.WriteLine(
                date.ToLongDateString() + " " +
                date.ToShortDateString() );
	    }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
} // End of namespace
