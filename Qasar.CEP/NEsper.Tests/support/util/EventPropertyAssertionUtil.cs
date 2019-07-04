using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.events;

using NUnit.Framework;

namespace net.esper.support.util
{
    public class EventPropertyAssertionUtil
    {
        public static void Compare(EventBean[] events, IList<EDictionary<String, Object>> expectedValues)
        {
            if ((expectedValues == null) && (events == null))
            {
                return;
            }
            if (((expectedValues == null) && (events != null)) ||
                 ((expectedValues != null) && (events == null)))
            {
                Assert.IsTrue(false);
            }

            Assert.AreEqual(expectedValues.Count, events.Length);

            for (int i = 0; i < expectedValues.Count; i++)
            {
                Compare(events[i], expectedValues[i]);
            }
        }

        public static void Compare(IEnumerator<EventBean> iterator, IList<EDictionary<String, Object>> expectedValues)
        {
            List<EventBean> values = new List<EventBean>();
            while (iterator.MoveNext())
            {
                values.Add(iterator.Current);
            }

            try
            {
                iterator.MoveNext();
				Object temp = iterator.Current;
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // Expected exception - next called after MoveNext returned false, for testing
            }

            EventBean[] data = null;
            if (values.Count > 0)
            {
                data = values.ToArray();
            }

            Compare(data, expectedValues);
        }


        private static void Compare(EventBean _event, EDictionary<String, Object> expected)
        {
            foreach (KeyValuePair<String, Object> entry in expected)
            {
                Object valueExpected = entry.Value;
                Object property = _event[entry.Key];

                Assert.AreEqual(valueExpected, property);
            }
        }
    }
}