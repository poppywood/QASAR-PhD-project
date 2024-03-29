using System;

using net.esper.client;
using net.esper.client.time;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.eql
{

    [TestFixture]
    public class Test3StreamOuterFullJoin
    {
        private EPServiceProvider epService;
        private EPStatement joinView;
        private SupportUpdateListener updateListener;

        private static readonly String _event_S0;
        private static readonly String _event_S1;
        private static readonly String _event_S2;

        [SetUp]
        public virtual void setUp()
        {
            EPServiceProviderManager.PurgeAllProviders();
            epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
            epService.EPRuntime.SendEvent(new TimerControlEvent(TimerControlEvent.ClockTypeEnum.CLOCK_EXTERNAL));
            epService.Initialize();
            updateListener = new SupportUpdateListener();
        }

        private Object[][] getAndResetNewEvents()
        {
            EventBean[] newEvents = updateListener.LastNewData;
            updateListener.Reset();
            return ArrayHandlingUtil.GetUnderlyingEvents(newEvents, new String[] { "s0", "s1", "s2" });
        }

        [Test]
        public void testFullJoin_2sides()
        {
            /**
            * Query:
            *                  s0
            *           s1 <->      <-> s2
            */
            String joinStatement =
                "select * from " +
                _event_S0 + ".win:length(1000) as s0 " + " full outer join " +
                _event_S1 + ".win:length(1000) as s1 on s0.p00 = s1.p10 " + " full outer join " +
                _event_S2 + ".win:length(1000) as s2 on s0.p00 = s2.p20 ";

            joinView = epService.EPAdministrator.CreateEQL(joinStatement);
            joinView.AddListener(updateListener);

            runAssertsFullJoin_2sides();
        }

        private void runAssertsFullJoin_2sides()
        {
            // Test s0 outer join to 2 streams, 2 results for each (cartesian product)
            //
            Object[] s1Events = SupportBean_S1.makeS1("A", new String[] { "A-s1-1", "A-s1-2" });
            SendEvent(s1Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { null, s1Events[1], null } }, getAndResetNewEvents());

            Object[] s2Events = SupportBean_S2.makeS2("A", new String[] { "A-s2-1", "A-s2-2" });
            SendEvent(s2Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { null, null, s2Events[1] } }, getAndResetNewEvents());

            Object[] s0Events = SupportBean_S0.makeS0("A", new String[] { "A-s0-1" });
            SendEvent(s0Events);
            Object[][] expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] }, new Object[] { s0Events[0], s1Events[1], s2Events[0] }, new Object[] { s0Events[0], s1Events[0], s2Events[1] }, new Object[] { s0Events[0], s1Events[1], s2Events[1] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s0 outer join to s1 and s2, no results for each s1 and s2
            //
            s0Events = SupportBean_S0.makeS0("B", new String[] { "B-s0-1" });
            SendEvent(s0Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], null, null } }, getAndResetNewEvents());

            s0Events = SupportBean_S0.makeS0("B", new String[] { "B-s0-2" });
            SendEvent(s0Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], null, null } }, getAndResetNewEvents());

            // Test s0 outer join to s1 and s2, one row for s1 and no results for s2
            //
            s1Events = SupportBean_S1.makeS1("C", new String[] { "C-s1-1" });
            sendEventsAndReset(s1Events);

            s0Events = SupportBean_S0.makeS0("C", new String[] { "C-s0-1" });
            SendEvent(s0Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], s1Events[0], null } }, getAndResetNewEvents());

            // Test s0 outer join to s1 and s2, two rows for s1 and no results for s2
            //
            s1Events = SupportBean_S1.makeS1("D", new String[] { "D-s1-1", "D-s1-2" });
            sendEventsAndReset(s1Events);

            s0Events = SupportBean_S0.makeS0("D", new String[] { "D-s0-1" });
            SendEvent(s0Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], s1Events[0], null }, new Object[] { s0Events[0], s1Events[1], null } }, getAndResetNewEvents());

            // Test s0 outer join to s1 and s2, one row for s2 and no results for s1
            //
            s2Events = SupportBean_S2.makeS2("E", new String[] { "E-s2-1" });
            sendEventsAndReset(s2Events);

            s0Events = SupportBean_S0.makeS0("E", new String[] { "E-s0-1" });
            SendEvent(s0Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], null, s2Events[0] } }, getAndResetNewEvents());

            // Test s0 outer join to s1 and s2, two rows for s2 and no results for s1
            //
            s2Events = SupportBean_S2.makeS2("F", new String[] { "F-s2-1", "F-s2-2" });
            sendEventsAndReset(s2Events);

            s0Events = SupportBean_S0.makeS0("F", new String[] { "F-s0-1" });
            SendEvent(s0Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], null, s2Events[0] }, new Object[] { s0Events[0], null, s2Events[1] } }, getAndResetNewEvents());

            // Test s0 outer join to s1 and s2, one row for s1 and two rows s2
            //
            s1Events = SupportBean_S1.makeS1("G", new String[] { "G-s1-1" });
            sendEventsAndReset(s1Events);

            s2Events = SupportBean_S2.makeS2("G", new String[] { "G-s2-1", "G-s2-2" });
            sendEventsAndReset(s2Events);

            s0Events = SupportBean_S0.makeS0("G", new String[] { "G-s0-2" });
            SendEvent(s0Events);
            expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] }, new Object[] { s0Events[0], s1Events[0], s2Events[1] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s0 outer join to s1 and s2, one row for s2 and two rows s1
            //
            s1Events = SupportBean_S1.makeS1("H", new String[] { "H-s1-1", "H-s1-2" });
            sendEventsAndReset(s1Events);

            s2Events = SupportBean_S2.makeS2("H", new String[] { "H-s2-1" });
            sendEventsAndReset(s2Events);

            s0Events = SupportBean_S0.makeS0("H", new String[] { "H-s0-2" });
            SendEvent(s0Events);
            expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] }, new Object[] { s0Events[0], s1Events[1], s2Events[0] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s0 outer join to s1 and s2, one row for each s1 and s2
            //
            s1Events = SupportBean_S1.makeS1("I", new String[] { "I-s1-1" });
            sendEventsAndReset(s1Events);

            s2Events = SupportBean_S2.makeS2("I", new String[] { "I-s2-1" });
            sendEventsAndReset(s2Events);

            s0Events = SupportBean_S0.makeS0("I", new String[] { "I-s0-2" });
            SendEvent(s0Events);
            expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s1 inner join to s0 and outer to s2:  s0 with 1 rows, s2 with 2 rows
            //
            s0Events = SupportBean_S0.makeS0("Q", new String[] { "Q-s0-1" });
            SendEvent(s0Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], null, null } }, getAndResetNewEvents());

            s2Events = SupportBean_S2.makeS2("Q", new String[] { "Q-s2-1", "Q-s2-2" });
            SendEvent(s2Events[0]);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], null, s2Events[0] } }, getAndResetNewEvents());
            SendEvent(s2Events[1]);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], null, s2Events[1] } }, getAndResetNewEvents());

            s1Events = SupportBean_S1.makeS1("Q", new String[] { "Q-s1-1" });
            SendEvent(s1Events);
            expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] }, new Object[] { s0Events[0], s1Events[0], s2Events[1] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s1 inner join to s0 and outer to s2:  s0 with 0 rows, s2 with 2 rows
            //
            s2Events = SupportBean_S2.makeS2("R", new String[] { "R-s2-1", "R-s2-2" });
            SendEvent(s2Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { null, null, s2Events[1] } }, getAndResetNewEvents());

            s1Events = SupportBean_S1.makeS1("R", new String[] { "R-s1-1" });
            SendEvent(s1Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { null, s1Events[0], null } }, getAndResetNewEvents());

            // Test s1 inner join to s0 and outer to s2:  s0 with 1 rows, s2 with 0 rows
            //
            s0Events = SupportBean_S0.makeS0("S", new String[] { "S-s0-1" });
            SendEvent(s0Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], null, null } }, getAndResetNewEvents());

            s1Events = SupportBean_S1.makeS1("S", new String[] { "S-s1-1" });
            SendEvent(s1Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], s1Events[0], null } }, getAndResetNewEvents());

            // Test s1 inner join to s0 and outer to s2:  s0 with 1 rows, s2 with 1 rows
            //
            s0Events = SupportBean_S0.makeS0("T", new String[] { "T-s0-1" });
            SendEvent(s0Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], null, null } }, getAndResetNewEvents());

            s2Events = SupportBean_S2.makeS2("T", new String[] { "T-s2-1" });
            sendEventsAndReset(s2Events);

            s1Events = SupportBean_S1.makeS1("T", new String[] { "T-s1-1" });
            SendEvent(s1Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] } }, getAndResetNewEvents());

            // Test s1 inner join to s0 and outer to s2:  s0 with 2 rows, s2 with 0 rows
            //
            s0Events = SupportBean_S0.makeS0("U", new String[] { "U-s0-1", "U-s0-1" });
            sendEventsAndReset(s0Events);

            s1Events = SupportBean_S1.makeS1("U", new String[] { "U-s1-1" });
            SendEvent(s1Events);
            expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], null }, new Object[] { s0Events[1], s1Events[0], null } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s1 inner join to s0 and outer to s2:  s0 with 2 rows, s2 with 1 rows
            //
            s0Events = SupportBean_S0.makeS0("V", new String[] { "V-s0-1", "V-s0-1" });
            sendEventsAndReset(s0Events);

            s2Events = SupportBean_S2.makeS2("V", new String[] { "V-s2-1" });
            sendEventsAndReset(s2Events);

            s1Events = SupportBean_S1.makeS1("V", new String[] { "V-s1-1" });
            SendEvent(s1Events);
            expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] }, new Object[] { s0Events[1], s1Events[0], s2Events[0] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s1 inner join to s0 and outer to s2:  s0 with 2 rows, s2 with 2 rows
            //
            s0Events = SupportBean_S0.makeS0("W", new String[] { "W-s0-1", "W-s0-2" });
            sendEventsAndReset(s0Events);

            s2Events = SupportBean_S2.makeS2("W", new String[] { "W-s2-1", "W-s2-2" });
            sendEventsAndReset(s2Events);

            s1Events = SupportBean_S1.makeS1("W", new String[] { "W-s1-1" });
            SendEvent(s1Events);
            expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] }, new Object[] { s0Events[1], s1Events[0], s2Events[0] }, new Object[] { s0Events[0], s1Events[0], s2Events[1] }, new Object[] { s0Events[1], s1Events[0], s2Events[1] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s2 inner join to s0 and outer to s1:  s0 with 1 rows, s1 with 2 rows
            //
            s0Events = SupportBean_S0.makeS0("J", new String[] { "J-s0-1" });
            sendEventsAndReset(s0Events);

            s1Events = SupportBean_S1.makeS1("J", new String[] { "J-s1-1", "J-s1-2" });
            sendEventsAndReset(s1Events);

            s2Events = SupportBean_S2.makeS2("J", new String[] { "J-s2-1" });
            SendEvent(s2Events);
            expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] }, new Object[] { s0Events[0], s1Events[1], s2Events[0] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s2 inner join to s0 and outer to s1:  s0 with 0 rows, s1 with 2 rows
            //
            s1Events = SupportBean_S1.makeS1("K", new String[] { "K-s1-1", "K-s1-2" });
            sendEventsAndReset(s2Events);

            s2Events = SupportBean_S2.makeS2("K", new String[] { "K-s2-1" });
            sendEventsAndReset(s2Events);

            // Test s2 inner join to s0 and outer to s1:  s0 with 1 rows, s1 with 0 rows
            //
            s0Events = SupportBean_S0.makeS0("L", new String[] { "L-s0-1" });
            sendEventsAndReset(s0Events);

            s2Events = SupportBean_S2.makeS2("L", new String[] { "L-s2-1" });
            SendEvent(s2Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], null, s2Events[0] } }, getAndResetNewEvents());

            // Test s2 inner join to s0 and outer to s1:  s0 with 1 rows, s1 with 1 rows
            //
            s0Events = SupportBean_S0.makeS0("M", new String[] { "M-s0-1" });
            sendEventsAndReset(s0Events);

            s1Events = SupportBean_S1.makeS1("M", new String[] { "M-s1-1" });
            sendEventsAndReset(s1Events);

            s2Events = SupportBean_S2.makeS2("M", new String[] { "M-s2-1" });
            SendEvent(s2Events);
            ArrayAssertionUtil.AssertRefAnyOrderArr(new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] } }, getAndResetNewEvents());

            // Test s2 inner join to s0 and outer to s1:  s0 with 2 rows, s1 with 0 rows
            //
            s0Events = SupportBean_S0.makeS0("N", new String[] { "N-s0-1", "N-s0-1" });
            sendEventsAndReset(s0Events);

            s2Events = SupportBean_S2.makeS2("N", new String[] { "N-s2-1" });
            SendEvent(s2Events);
            expected = new Object[][] { new Object[] { s0Events[0], null, s2Events[0] }, new Object[] { s0Events[1], null, s2Events[0] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s2 inner join to s0 and outer to s1:  s0 with 2 rows, s1 with 1 rows
            //
            s0Events = SupportBean_S0.makeS0("O", new String[] { "O-s0-1", "O-s0-1" });
            sendEventsAndReset(s0Events);

            s1Events = SupportBean_S1.makeS1("O", new String[] { "O-s1-1" });
            sendEventsAndReset(s1Events);

            s2Events = SupportBean_S2.makeS2("O", new String[] { "O-s2-1" });
            SendEvent(s2Events);
            expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] }, new Object[] { s0Events[1], s1Events[0], s2Events[0] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());

            // Test s2 inner join to s0 and outer to s1:  s0 with 2 rows, s1 with 2 rows
            //
            s0Events = SupportBean_S0.makeS0("P", new String[] { "P-s0-1", "P-s0-2" });
            sendEventsAndReset(s0Events);

            s1Events = SupportBean_S1.makeS1("P", new String[] { "P-s1-1", "P-s1-2" });
            sendEventsAndReset(s1Events);

            s2Events = SupportBean_S2.makeS2("P", new String[] { "P-s2-1" });
            SendEvent(s2Events);
            expected = new Object[][] { new Object[] { s0Events[0], s1Events[0], s2Events[0] }, new Object[] { s0Events[1], s1Events[0], s2Events[0] }, new Object[] { s0Events[0], s1Events[1], s2Events[0] }, new Object[] { s0Events[1], s1Events[1], s2Events[0] } };
            ArrayAssertionUtil.AssertRefAnyOrderArr(expected, getAndResetNewEvents());
        }

        private void SendEvent(Object _event)
        {
            epService.EPRuntime.SendEvent(_event);
        }

        private void sendEventsAndReset(Object[] events)
        {
            SendEvent(events);
            updateListener.Reset();
        }

        private void SendEvent(Object[] events)
        {
            for (int i = 0; i < events.Length; i++)
            {
                epService.EPRuntime.SendEvent(events[i]);
            }
        }

        static Test3StreamOuterFullJoin()
        {
            _event_S0 = typeof(SupportBean_S0).FullName;
            _event_S1 = typeof(SupportBean_S1).FullName;
            _event_S2 = typeof(SupportBean_S2).FullName;
        }
    }
}