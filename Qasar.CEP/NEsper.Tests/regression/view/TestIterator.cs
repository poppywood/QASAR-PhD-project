///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.compat;
using net.esper.events;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.view
{
    [TestFixture]
    public class TestIterator
	{
        private EPServiceProvider epService;

        [SetUp]
	    public void SetUp()
	    {
	        epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
	        epService.Initialize();
	    }

        [Test]
	    public void testPatternNoWindow()
	    {
	        // Test for Esper-115
	        String cepStatementString =	"select * from pattern " +
										"[every ( addressInfo = " + typeof(SupportBean).FullName + "(string='address') " +
										"-> txnWD = " + typeof(SupportBean).FullName + "(string='txn') ) ] " +
										"where addressInfo.intBoxed = txnWD.intBoxed";
			EPStatement epStatement = epService.EPAdministrator.CreateEQL(cepStatementString);

			SupportBean myEventBean1 = new SupportBean();
			myEventBean1.SetString("address");
			myEventBean1.SetIntBoxed(9001);
			epService.EPRuntime.SendEvent(myEventBean1);
	        Assert.IsFalse(epStatement.GetEnumerator().MoveNext());

	        SupportBean myEventBean2 = new SupportBean();
	        myEventBean2.SetString("txn");
	        myEventBean2.SetIntBoxed(9001);
	        epService.EPRuntime.SendEvent(myEventBean2);
	        Assert.IsTrue(epStatement.GetEnumerator().MoveNext());

	        IEnumerator<EventBean> itr = epStatement.GetEnumerator();
            Assert.IsTrue(itr.MoveNext());
	        EventBean _event = itr.Current;
	        Assert.AreEqual(myEventBean1, _event.Get("addressInfo"));
	        Assert.AreEqual(myEventBean2, _event.Get("txnWD"));
	    }

        [Test]
	    public void testPatternWithWindow()
	    {
			String cepStatementString =	"select * from pattern " +
										"[every ( addressInfo = " + typeof(SupportBean).FullName + "(string='address') " +
										"-> txnWD = " + typeof(SupportBean).FullName + "(string='txn') ) ].std:lastevent() " +
										"where addressInfo.intBoxed = txnWD.intBoxed";
			EPStatement epStatement = epService.EPAdministrator.CreateEQL(cepStatementString);

			SupportBean myEventBean1 = new SupportBean();
			myEventBean1.SetString("address");
			myEventBean1.SetIntBoxed(9001);
			epService.EPRuntime.SendEvent(myEventBean1);

	        SupportBean myEventBean2 = new SupportBean();
	        myEventBean2.SetString("txn");
	        myEventBean2.SetIntBoxed(9001);
	        epService.EPRuntime.SendEvent(myEventBean2);

	        IEnumerator<EventBean> itr = epStatement.GetEnumerator();
            Assert.IsTrue(itr.MoveNext());
	        EventBean _event = itr.Current;
	        Assert.AreEqual(myEventBean1, _event.Get("addressInfo"));
	        Assert.AreEqual(myEventBean2, _event.Get("txnWD"));
	    }

        [Test]
	    public void testOrderByWildcard()
	    {
	        String stmtText = "select * from " + typeof(SupportMarketDataBean).FullName + ".win:length(5) order by symbol, volume";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        Object eventOne = SendEvent("SYM", 1);
	        ArrayAssertionUtil.AreEqualExactOrderUnderlying(stmt.GetEnumerator(), new Object[] {eventOne});

	        Object eventTwo = SendEvent("OCC", 2);
	        ArrayAssertionUtil.AreEqualExactOrderUnderlying(stmt.GetEnumerator(), new Object[] {eventTwo, eventOne});

	        Object eventThree = SendEvent("TOC", 3);
	        ArrayAssertionUtil.AreEqualExactOrderUnderlying(stmt.GetEnumerator(), new Object[] {eventTwo, eventOne, eventThree});

	        Object eventFour = SendEvent("SYM", 0);
	        ArrayAssertionUtil.AreEqualExactOrderUnderlying(stmt.GetEnumerator(), new Object[] {eventTwo, eventFour, eventOne, eventThree});

	        Object eventFive = SendEvent("SYM", 10);
	        ArrayAssertionUtil.AreEqualExactOrderUnderlying(stmt.GetEnumerator(), new Object[] {eventTwo, eventFour, eventOne, eventFive, eventThree});

	        Object eventSix = SendEvent("SYM", 4);
	        ArrayAssertionUtil.AreEqualExactOrderUnderlying(stmt.GetEnumerator(), new Object[] {eventTwo, eventFour, eventSix, eventFive, eventThree});
	    }

        [Test]
	    public void testOrderByProps()
	    {
	        String[] fields = new String[] {"symbol", "volume"};
	        String stmtText = "select symbol, volume from " + typeof(SupportMarketDataBean).FullName + ".win:length(3) order by symbol, volume";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", 1);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] {new Object[]{"SYM", 1L}});

	        SendEvent("OCC", 2);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] {new Object[]{"OCC", 2L}, new Object[]{"SYM", 1L}});

	        SendEvent("SYM", 0);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] {new Object[]{"OCC", 2L}, new Object[]{"SYM", 0L}, new Object[]{"SYM", 1L}});

	        SendEvent("OCC", 3);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] {new Object[]{"OCC", 2L}, new Object[]{"OCC", 3L}, new Object[]{"SYM", 0L}});
	    }

        [Test]
	    public void testJoin()
	    {
	        String stmtText = "select * from " + typeof(SupportMarketDataBean).FullName + ".win:length(5)," +
	                           typeof(SupportBean).FullName;
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        try
	        {
	            stmt.GetEnumerator();
	            Assert.Fail();
	        }
	        catch (UnsupportedOperationException ex)
	        {
	            // expected
	        }
	    }

        [Test]
        public void testFilter()
	    {
	        String[] fields = new String[] {"symbol", "vol"};
	        String stmtText = "select symbol, volume * 10 as vol from " + typeof(SupportMarketDataBean).FullName + ".win:length(5)" +
	                      " where volume < 0";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", 100);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, null);

	        SendEvent("SYM", -1);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] {new Object[]{"SYM", -10L}});

	        SendEvent("SYM", -6);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", -10L}, new Object[]{"SYM", -60L}});

	        SendEvent("SYM", 1);
	        SendEvent("SYM", 16);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", -10L}, new Object[]{"SYM", -60L}});

	        SendEvent("SYM", -9);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", -10L}, new Object[]{"SYM", -60L}, new Object[]{"SYM", -90L}});

	        SendEvent("SYM", 2);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", -60L}, new Object[]{"SYM", -90L}});

	        SendEvent("SYM", 3);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", -90L}});

	        SendEvent("SYM", 4);
	        SendEvent("SYM", 5);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", -90L}});
	        SendEvent("SYM", 6);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());
	    }

        [Test]
	    public void testGroupByRowPerGroupOrdered()
	    {
	        String[] fields = new String[] {"symbol", "sumVol"};
	        String stmtText = "select symbol, Sum(volume) as sumVol " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".win:length(5) " +
	                          "group by symbol " +
	                          "order by symbol";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", 100);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 100L}});

	        SendEvent("OCC", 5);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"OCC", 5L}, new Object[]{"SYM", 100L}});

	        SendEvent("SYM", 10);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"OCC", 5L}, new Object[]{"SYM", 110L}});

	        SendEvent("OCC", 6);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"OCC", 11L}, new Object[]{"SYM", 110L}});

	        SendEvent("ATB", 8);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"ATB", 8L}, new Object[]{"OCC", 11L}, new Object[]{"SYM", 110L}});

	        SendEvent("ATB", 7);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"ATB", 15L}, new Object[]{"OCC", 11L}, new Object[]{"SYM", 10L}});
	    }

        [Test]
	    public void testGroupByRowPerGroup()
	    {
	        String[] fields = new String[] {"symbol", "sumVol"};
	        String stmtText = "select symbol, Sum(volume) as sumVol " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".win:length(5) " +
	                          "group by symbol";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", 100);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 100L}});

	        SendEvent("SYM", 10);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 110L}});

	        SendEvent("TAC", 1);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 110L}, new Object[]{"TAC", 1L}});

	        SendEvent("SYM", 11);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 121L}, new Object[]{"TAC", 1L}});

	        SendEvent("TAC", 2);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 121L}, new Object[]{"TAC", 3L}});

	        SendEvent("OCC", 55);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 21L}, new Object[]{"TAC", 3L}, new Object[]{"OCC", 55L}});

	        SendEvent("OCC", 4);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"TAC", 3L}, new Object[]{"SYM", 11L}, new Object[]{"OCC", 59L}});

	        SendEvent("OCC", 3);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 11L}, new Object[]{"TAC", 2L}, new Object[]{"OCC", 62L}});
	    }

        [Test]
        public void testGroupByRowPerGroupHaving()
	    {
	        String[] fields = new String[] {"symbol", "sumVol"};
	        String stmtText = "select symbol, Sum(volume) as sumVol " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".win:length(5) " +
	                          "group by symbol having Sum(volume) > 10";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", 100);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 100L}});

	        SendEvent("SYM", 5);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 105L}});

	        SendEvent("TAC", 1);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 105L}});

	        SendEvent("SYM", 3);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 108L}});

	        SendEvent("TAC", 12);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 108L}, new Object[]{"TAC", 13L}});

	        SendEvent("OCC", 55);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"TAC", 13L}, new Object[]{"OCC", 55L}});

	        SendEvent("OCC", 4);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"TAC", 13L}, new Object[]{"OCC", 59L}});

	        SendEvent("OCC", 3);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"TAC", 12L}, new Object[]{"OCC", 62L}});
	    }

        [Test]
        public void testGroupByComplex()
	    {
	        String[] fields = new String[] {"symbol", "msg"};
	        String stmtText = "insert into Cutoff " +
	                          "select symbol, (Convert.ToString(count(*)) || 'x1000.0') as msg " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".std:groupby('symbol').win:length(1) " +
	                          "where price - volume >= 1000.0 group by symbol having count(*) = 1";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("SYM", -1, -1L, null));
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("SYM", 100000d, 0L, null));
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", "1x1000.0"}});

	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("SYM", 1d, 1L, null));
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());
	    }

        [Test]
        public void testGroupByRowPerEventOrdered()
	    {
	        String[] fields = new String[] {"symbol", "price", "sumVol"};
	        String stmtText = "select symbol, price, Sum(volume) as sumVol " +
	                          "from " + typeof (SupportMarketDataBean).FullName + ".win:length(5) " +
	                          "group by symbol " +
	                          "order by symbol";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", -1, 100);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                                              new Object[][]
	                                                  {
	                                                      new Object[] {"SYM", -1d, 100L}
	                                                  });
	    

        SendEvent("TAC", -2, 12);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                                              new Object[][]
	                                                  {
	                                                      new Object[] {"SYM", -1d, 100L},
	                                                      new Object[] {"TAC", -2d, 12L}
	                                                  });

	        SendEvent("TAC", -3, 13);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"SYM", -1d, 100L}, new Object[]{"TAC", -2d, 25L}, new Object[]{"TAC", -3d, 25L}});

	        SendEvent("SYM", -4, 1);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"SYM", -1d, 101L}, new Object[]{"SYM", -4d, 101L}, new Object[]{"TAC", -2d, 25L}, new Object[]{"TAC", -3d, 25L}});

	        SendEvent("OCC", -5, 99);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"OCC", -5d, 99L}, new Object[]{"SYM", -1d, 101L}, new Object[]{"SYM", -4d, 101L}, new Object[]{"TAC", -2d, 25L}, new Object[]{"TAC", -3d, 25L}});

	        SendEvent("TAC", -6, 2);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"OCC", -5d, 99L}, new Object[]{"SYM", -4d, 1L}, new Object[]{"TAC", -2d, 27L}, new Object[]{"TAC", -3d, 27L}, new Object[]{"TAC", -6d, 27L}});
	    }

        [Test]
        public void testGroupByRowPerEvent()
	    {
	        String[] fields = new String[] {"symbol", "price", "sumVol"};
	        String stmtText = "select symbol, price, Sum(volume) as sumVol " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".win:length(5) " +
	                          "group by symbol";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
            Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", -1, 100);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", -1d, 100L}});

	        SendEvent("TAC", -2, 12);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"SYM", -1d, 100L}, new Object[]{"TAC", -2d, 12L}});

	        SendEvent("TAC", -3, 13);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"SYM", -1d, 100L}, new Object[]{"TAC", -2d, 25L}, new Object[]{"TAC", -3d, 25L}});

	        SendEvent("SYM", -4, 1);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"SYM", -1d, 101L}, new Object[]{"TAC", -2d, 25L}, new Object[]{"TAC", -3d, 25L}, new Object[]{"SYM", -4d, 101L}});

	        SendEvent("OCC", -5, 99);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"SYM", -1d, 101L}, new Object[]{"TAC", -2d, 25L}, new Object[]{"TAC", -3d, 25L}, new Object[]{"SYM", -4d, 101L}, new Object[]{"OCC", -5d, 99L}});

	        SendEvent("TAC", -6, 2);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"TAC", -2d, 27L}, new Object[]{"TAC", -3d, 27L}, new Object[]{"SYM", -4d, 1L}, new Object[]{"OCC", -5d, 99L}, new Object[]{"TAC", -6d, 27L}});
	    }

        [Test]
        public void testGroupByRowPerEventHaving()
	    {
	        String[] fields = new String[] {"symbol", "price", "sumVol"};
	        String stmtText = "select symbol, price, Sum(volume) as sumVol " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".win:length(5) " +
	                          "group by symbol having Sum(volume) > 20";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
            Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", -1, 100);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", -1d, 100L}});

	        SendEvent("TAC", -2, 12);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"SYM", -1d, 100L}});

	        SendEvent("TAC", -3, 13);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"SYM", -1d, 100L}, new Object[]{"TAC", -2d, 25L}, new Object[]{"TAC", -3d, 25L}});

	        SendEvent("SYM", -4, 1);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"SYM", -1d, 101L}, new Object[]{"TAC", -2d, 25L}, new Object[]{"TAC", -3d, 25L}, new Object[]{"SYM", -4d, 101L}});

	        SendEvent("OCC", -5, 99);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"SYM", -1d, 101L}, new Object[]{"TAC", -2d, 25L}, new Object[]{"TAC", -3d, 25L}, new Object[]{"SYM", -4d, 101L}, new Object[]{"OCC", -5d, 99L}});

	        SendEvent("TAC", -6, 2);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields,
	                new Object[][] { new Object[]{"TAC", -2d, 27L}, new Object[]{"TAC", -3d, 27L}, new Object[]{"OCC", -5d, 99L}, new Object[]{"TAC", -6d, 27L}});
	    }

        [Test]
        public void testAggregateAll()
	    {
	        String[] fields = new String[] {"symbol", "sumVol"};
	        String stmtText = "select symbol, Sum(volume) as sumVol " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".win:length(3) ";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
            Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", 100);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 100L}});

	        SendEvent("TAC", 1);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 101L}, new Object[]{"TAC", 101L}});

	        SendEvent("MOV", 3);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 104L}, new Object[]{"TAC", 104L}, new Object[]{"MOV", 104L}});

	        SendEvent("SYM", 10);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"TAC", 14L}, new Object[]{"MOV", 14L}, new Object[]{"SYM", 14L}});
	    }

        [Test]
        public void testAggregateAllOrdered()
	    {
	        String[] fields = new String[] {"symbol", "sumVol"};
	        String stmtText = "select symbol, Sum(volume) as sumVol " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".win:length(3) " +
	                          " order by symbol asc";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
            Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", 100);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 100L}});

	        SendEvent("TAC", 1);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 101L}, new Object[]{"TAC", 101L}});

	        SendEvent("MOV", 3);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"MOV", 104L}, new Object[]{"SYM", 104L}, new Object[]{"TAC", 104L}});

	        SendEvent("SYM", 10);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"MOV", 14L}, new Object[]{"SYM", 14L}, new Object[]{"TAC", 14L}});
	    }

        [Test]
        public void testAggregateAllHaving()
	    {
	        String[] fields = new String[] {"symbol", "sumVol"};
	        String stmtText = "select symbol, Sum(volume) as sumVol " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".win:length(3) having Sum(volume) > 100";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("SYM", 100);
            Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent("TAC", 1);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 101L}, new Object[]{"TAC", 101L}});

	        SendEvent("MOV", 3);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{"SYM", 104L}, new Object[]{"TAC", 104L}, new Object[]{"MOV", 104L}});

	        SendEvent("SYM", 10);
            Assert.IsFalse(stmt.GetEnumerator().MoveNext());
	    }

        [Test]
        public void testRowForAll()
	    {
	        String[] fields = new String[] {"sumVol"};
	        String stmtText = "select Sum(volume) as sumVol " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".win:length(3) ";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{null}});

	        SendEvent(100);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{100L}});

	        SendEvent(50);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{150L}});

	        SendEvent(25);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{175L}});

	        SendEvent(10);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{85L}});
	    }

        [Test]
        public void testRowForAllHaving()
	    {
	        String[] fields = new String[] {"sumVol"};
	        String stmtText = "select Sum(volume) as sumVol " +
	                          "from " + typeof(SupportMarketDataBean).FullName + ".win:length(3) having Sum(volume) > 100";

	        EPStatement stmt = epService.EPAdministrator.CreateEQL(stmtText);
            Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent(100);
            Assert.IsFalse(stmt.GetEnumerator().MoveNext());

	        SendEvent(50);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{150L}});

	        SendEvent(25);
	        ArrayAssertionUtil.AreEqualExactOrder(stmt.GetEnumerator(), fields, new Object[][] { new Object[]{175L}});

	        SendEvent(10);
            Assert.IsFalse(stmt.GetEnumerator().MoveNext());
	    }

	    private void SendEvent(String symbol, double price, long volume)
	    {
	        epService.EPRuntime.SendEvent(new SupportMarketDataBean(symbol, price, volume, null));
	    }

	    private SupportMarketDataBean SendEvent(String symbol, long volume)
	    {
	        SupportMarketDataBean _event = new SupportMarketDataBean(symbol, 0, volume, null);
	        epService.EPRuntime.SendEvent(_event);
            return _event;
	    }

	    private void SendEvent(long volume)
	    {
	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("SYM", 0, volume, null));
	    }
	}
} // End of namespace
