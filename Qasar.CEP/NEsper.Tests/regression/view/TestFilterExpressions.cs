// ---------------------------------------------------------------------------------- /
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
// ---------------------------------------------------------------------------------- /

using System;

using net.esper.client;
using net.esper.support.bean;
using net.esper.support.client;
using net.esper.support.eql;
using net.esper.support.util;

using NUnit.Framework;

namespace net.esper.regression.view
{
	[TestFixture]
	public class TestFilterExpressions
	{
	    private EPServiceProvider epService;
        private SupportUpdateListener listener;

	    [SetUp]
	    public void SetUp()
	    {
            listener = new SupportUpdateListener();

            EPServiceProviderManager.PurgeAllProviders();
            epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
	        epService.Initialize();
	    }

        [Test]
        public void testEnumSyntaxOne()
        {
            String text = "select * from pattern [" +
                          typeof (SupportBeanWithEnum).FullName + "(supportEnum=" + typeof (SupportEnumHelper).FullName +
                          ".GetEnumFor('ENUM_VALUE_1'))]";
            EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
            stmt.AddListener(listener);

            SupportBeanWithEnum _event = new SupportBeanWithEnum("e1", SupportEnum.ENUM_VALUE_1);
            epService.EPRuntime.SendEvent(_event);
            Assert.IsTrue(listener.IsInvoked);
        }

        [Test]
	    public void testNotEqualsNotIn()
        {
            tryNotEqualsConsolidate("intPrimitive not in (1, 2)");
        }

        [Test]
	    public void testNotEqualsComma()
        {
            tryNotEqualsConsolidate("intPrimitive != 1, intPrimitive != 2");
        }

	    [Test]
        public void testNotEqualsAnd()
	    {
	        tryNotEqualsConsolidate("intPrimitive != 1 and intPrimitive != 2");
	    }

        [Test]
	    public void tryNotEqualsConsolidate(String filter)
        {
            String text = "select * from " + typeof (SupportBean).FullName + "(" + filter + ")";
            EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
            stmt.AddListener(listener);

            for (int i = 0; i < 5; i++)
            {
                epService.EPRuntime.SendEvent(new SupportBean("", i));

                if ((i == 1) || (i == 2))
                {
                    Assert.IsFalse(listener.IsInvoked, "incorrect:" + i);
                }
                else
                {
                    Assert.IsTrue(listener.IsInvoked, "incorrect:" + i);
                }
                listener.Reset();
            }
        }

        [Test]
        public void testEqualsSemanticFilter()
        {
            // Test for Esper-114
            String text = "select * from " + typeof (SupportBeanComplexProps).FullName + "(nested=nested)";
            EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
            stmt.AddListener(listener);

            SupportBeanComplexProps eventOne = SupportBeanComplexProps.MakeDefaultBean();
            eventOne.SimpleProperty = "1";

            epService.EPRuntime.SendEvent(eventOne);
            Assert.IsTrue(listener.IsInvoked);
        }

	    [Test]
        public void testEqualsSemanticExpr()
        {
            // Test for Esper-114
            String text =
                "select * from " + typeof (SupportBeanComplexProps).FullName + "(simpleProperty='1') as s0" +
                ", " + typeof (SupportBeanComplexProps).FullName + "(simpleProperty='2') as s1" +
                " where s0.nested = s1.nested";
            EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
            stmt.AddListener(listener);

            SupportBeanComplexProps eventOne = SupportBeanComplexProps.MakeDefaultBean();
            eventOne.SimpleProperty = "1";

            SupportBeanComplexProps eventTwo = SupportBeanComplexProps.MakeDefaultBean();
            eventTwo.SimpleProperty = "2";

            Assert.AreEqual(eventOne.Nested, eventTwo.Nested);

            epService.EPRuntime.SendEvent(eventOne);
            Assert.IsFalse(listener.IsInvoked);

            epService.EPRuntime.SendEvent(eventTwo);
            Assert.IsTrue(listener.IsInvoked);
        }

	    [Test]
	    public void testPatternFunc3Stream()
	    {
	        String text;

	        text = "select * from pattern [" +
	                "a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportBean).FullName + " -> " +
	                "c=" + typeof(SupportBean).FullName + "(intBoxed=a.intBoxed, intBoxed=b.intBoxed and intBoxed != null)]";
	        TryPattern3Stream(text, new int?[] {null, 2, 1, null,   8,  1,  2}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {null, 3, 1,    8, null, 4, -2}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {null, 3, 1,    8, null, 5, null}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                    new bool[] {false, false, true, false, false, false, false});

	        text = "select * from pattern [" +
	                "a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportBean).FullName + " -> " +
	                "c=" + typeof(SupportBean).FullName + "(intBoxed=a.intBoxed or intBoxed=b.intBoxed)]";
	        TryPattern3Stream(text, new int?[] {null, 2, 1, null,   8, 1, 2}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {null, 3, 1,    8, null, 4, -2}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {null, 3, 1,    8, null, 5, null}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                    new bool[] {true, true, true, true, true, false, false});

	        text = "select * from pattern [" +
	                "a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportBean).FullName + " -> " +
	                "c=" + typeof(SupportBean).FullName + "(intBoxed=a.intBoxed, intBoxed=b.intBoxed)]";
	        TryPattern3Stream(text, new int?[] {null, 2, 1, null,   8,  1,  2}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {null, 3, 1,    8, null, 4, -2}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {null, 3, 1,    8, null, 5, null}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                    new bool[] {true, false, true, false, false, false, false});

	        text = "select * from pattern [" +
	                "a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportBean).FullName + " -> " +
	                "c=" + typeof(SupportBean).FullName + "(intBoxed!=a.intBoxed, intBoxed!=b.intBoxed)]";
	        TryPattern3Stream(text, new int?[] {null, 2, 1, null,   8,  1,  2}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {null, 3, 1,    8, null, 4, -2}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {null, 3, 1,    8, null, 5, null}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                    new bool[] {false, false, false, false, false, true, false});

	        text = "select * from pattern [" +
	                "a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportBean).FullName + " -> " +
	                "c=" + typeof(SupportBean).FullName + "(intBoxed!=a.intBoxed)]";
	        TryPattern3Stream(text, new int?[] {2,    8,    null, 2, 1, null, 1}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {-2,   null, null, 3, 1,    8, 4}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {null, null, null, 3, 1,    8, 5}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                               new bool[] {false, false, false, true, false, true, true});

	        text = "select * from pattern [" +
	                "a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportBean).FullName + " -> " +
	                "c=" + typeof(SupportBean).FullName + "(intBoxed=a.intBoxed, doubleBoxed=b.doubleBoxed)]";
	        TryPattern3Stream(text, new int?[] {2, 2, 1, 2, 1, 7, 1}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {0, 0, 0, 0, 0, 0, 0}, new double?[] {1d, 2d, 0d, 2d, 0d, 1d, 0d},
	                                new int?[] {2, 2, 3, 2, 1, 7, 5}, new double?[] {1d, 1d, 1d, 2d, 1d, 1d, 1d},
	                               new bool[] {true, false, false, true, false, true, false});

	        text = "select * from pattern [" +
	                "a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportBean).FullName + " -> " +
	                "c=" + typeof(SupportBean).FullName + "(intBoxed in (a.intBoxed, b.intBoxed))]";
	        TryPattern3Stream(text, new int?[] {2,    1, 1,     null,   1, null,    1}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {1,    2, 1,     null, null,   2,    0}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {2,    2, 3,     null,   1, null,  null}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                           new bool[]   {true, true, false, false, true, false, false});

	        text = "select * from pattern [" +
	                "a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportBean).FullName + " -> " +
	                "c=" + typeof(SupportBean).FullName + "(intBoxed in [a.intBoxed:b.intBoxed])]";
	        TryPattern3Stream(text, new int?[] {2,    1, 1,     null,   1, null,    1}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {1,    2, 1,     null, null,   2,    0}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {2,    1, 3,     null,   1, null,  null}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                           new bool[]   {true, true, false, false, false, false, false});

	        text = "select * from pattern [" +
	                "a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportBean).FullName + " -> " +
	                "c=" + typeof(SupportBean).FullName + "(intBoxed not in [a.intBoxed:b.intBoxed])]";
	        TryPattern3Stream(text, new int?[] {2,    1, 1,     null,   1, null,    1}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {1,    2, 1,     null, null,   2,    0}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                                new int?[] {2,    1, 3,     null,   1, null,  null}, new double?[] {0d, 0d, 0d, 0d, 0d, 0d, 0d},
	                           new bool[]   {false, false, true, false, false, false, false});
	    }

	    [Test]
	    public void testPatternFunc()
	    {
	        String text;

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(intBoxed = a.intBoxed and doubleBoxed = a.doubleBoxed)]";
	        TryPattern(text, new int?[] {null, 2, 1, null, 8, 1, 2}, new double?[] {2d, 2d, 2d, 1d, 5d, 6d, 7d},
	                         new int?[] {null, 3, 1, 8, null, 1, 2}, new double?[] {2d, 3d, 2d, 1d, 5d, 6d, 8d},
	                    new bool[] {true, false, true, false, false, true, false});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(a.doubleBoxed = doubleBoxed)]";
	        TryPattern(text, new int?[] {0, 0}, new double?[] {2d, 2d},
	                         new int?[] {0, 0}, new double?[] {2d, 3d},
	                    new bool[] {true, false});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(a.doubleBoxed = b.doubleBoxed)]";
	        TryPattern(text, new int?[] {0, 0}, new double?[] {2d, 2d},
	                         new int?[] {0, 0}, new double?[] {2d, 3d},
	                    new bool[] {true, false});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(a.doubleBoxed != doubleBoxed)]";
	        TryPattern(text, new int?[] {0, 0}, new double?[] {2d, 2d},
	                         new int?[] {0, 0}, new double?[] {2d, 3d},
	                    new bool[] {false, true});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(a.doubleBoxed != b.doubleBoxed)]";
	        TryPattern(text, new int?[] {0, 0}, new double?[] {2d, 2d},
	                         new int?[] {0, 0}, new double?[] {2d, 3d},
	                    new bool[] {false, true});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed in [a.doubleBoxed:a.intBoxed])]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {false, true, true, true, true, false});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed in (a.doubleBoxed:a.intBoxed])]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {false, false, true, true, true, false});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(b.doubleBoxed in (a.doubleBoxed:a.intBoxed))]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {false, false, true, true, false, false});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed in [a.doubleBoxed:a.intBoxed))]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {false, true, true, true, false, false});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed not in [a.doubleBoxed:a.intBoxed])]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {true, false, false, false, false, true});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed not in (a.doubleBoxed:a.intBoxed])]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {true, true, false, false, false, true});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(b.doubleBoxed not in (a.doubleBoxed:a.intBoxed))]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {true, true, false, false, true, true});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed not in [a.doubleBoxed:a.intBoxed))]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {true, false, false, false, true, true});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed not in (a.doubleBoxed, a.intBoxed, 9))]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {true, false, true, false, false, true});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed in (a.doubleBoxed, a.intBoxed, 9))]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {false, true, false, true, true, false});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(b.doubleBoxed in (doubleBoxed, a.intBoxed, 9))]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {true, true, true, true, true, true});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed not in (doubleBoxed, a.intBoxed, 9))]";
	        TryPattern(text, new int?[] {1, 1, 1, 1, 1, 1}, new double?[] {10d, 10d, 10d, 10d, 10d, 10d},
	                         new int?[] {0, 0, 0, 0, 0, 0}, new double?[] {0d, 1d, 2d, 9d, 10d, 11d},
	                    new bool[] {false, false, false, false, false, false});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed = " + typeof(SupportStaticMethodLib).FullName + ".MinusOne(a.doubleBoxed))]";
	        TryPattern(text, new int?[] {0, 0, 0}, new double?[] {10d, 10d, 10d},
	                         new int?[] {0, 0, 0}, new double?[] {9d, 10d, 11d, },
	                    new bool[] {true, false, false});

	        text = "select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed = " + typeof(SupportStaticMethodLib).FullName + ".MinusOne(a.doubleBoxed) or " +
	                    "doubleBoxed = " + typeof(SupportStaticMethodLib).FullName + ".MinusOne(a.intBoxed))]";
	        TryPattern(text, new int?[] {0, 0, 12}, new double?[] {10d, 10d, 10d},
	                         new int?[] {0, 0, 0}, new double?[] {9d, 10d, 11d, },
	                    new bool[] {true, false, true});
	    }

        private void TryPattern(String text,
                                int?[] intBoxedA,
                                double?[] doubleBoxedA,
                                int?[] intBoxedB,
                                double?[] doubleBoxedB,
                                bool[] expected)
        {
            Assert.AreEqual(intBoxedA.Length, doubleBoxedA.Length);
            Assert.AreEqual(intBoxedB.Length, doubleBoxedB.Length);
            Assert.AreEqual(expected.Length, doubleBoxedA.Length);
            Assert.AreEqual(intBoxedA.Length, doubleBoxedB.Length);

            for (int i = 0; i < intBoxedA.Length; i++)
            {
                EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
                stmt.AddListener(listener);

                SendBeanIntDouble(intBoxedA[i], doubleBoxedA[i]);
                SendBeanIntDouble(intBoxedB[i], doubleBoxedB[i]);
                Assert.AreEqual(expected[i], listener.GetAndClearIsInvoked(), "failed at index " + i);
                stmt.Stop();
            }
        }

	    private void TryPattern3Stream(String text,
	                            int?[] intBoxedA,
	                            double?[] doubleBoxedA,
	                            int?[] intBoxedB,
	                            double?[] doubleBoxedB,
	                            int?[] intBoxedC,
	                            double?[] doubleBoxedC,
	                            bool[] expected)
	    {
	        Assert.AreEqual(intBoxedA.Length, doubleBoxedA.Length);
	        Assert.AreEqual(intBoxedB.Length, doubleBoxedB.Length);
	        Assert.AreEqual(expected.Length, doubleBoxedA.Length);
	        Assert.AreEqual(intBoxedA.Length, doubleBoxedB.Length);
	        Assert.AreEqual(intBoxedC.Length, doubleBoxedC.Length);
	        Assert.AreEqual(intBoxedB.Length, doubleBoxedC.Length);

	        for (int i = 0; i < intBoxedA.Length; i++)
	        {
	            EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
                stmt.AddListener(listener);

	            SendBeanIntDouble(intBoxedA[i], doubleBoxedA[i]);
	            SendBeanIntDouble(intBoxedB[i], doubleBoxedB[i]);
	            SendBeanIntDouble(intBoxedC[i], doubleBoxedC[i]);
                Assert.AreEqual(expected[i], listener.GetAndClearIsInvoked(), "failed at index " + i);
	            stmt.Stop();
	        }
	    }

	    [Test]
	    public void testIn3ValuesAndNull()
	    {
	        String text;

	        text = "select * from " + typeof(SupportBean).FullName + "(intPrimitive in (intBoxed, doubleBoxed))";
	        Try3Fields(text, new int[]{1, 1, 1}, new int?[]{0, 1, 0}, new double?[]{2d, 2d, 1d}, new bool[]{false, true, true});

	        text = "select * from " + typeof(SupportBean).FullName + "(intPrimitive in (intBoxed, " +
	            typeof(SupportStaticMethodLib).FullName + ".MinusOne(doubleBoxed)))";
	        Try3Fields(text, new int[]{1, 1, 1}, new int?[]{0, 1, 0}, new double?[]{2d, 2d, 1d}, new bool[]{true, true, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intPrimitive not in (intBoxed, doubleBoxed))";
	        Try3Fields(text, new int[]{1, 1, 1}, new int?[]{0, 1, 0}, new double?[]{2d, 2d, 1d}, new bool[]{true, false, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed = doubleBoxed)";
	        Try3Fields(text, new int[]{1, 1, 1}, new int?[]{null, 1, null}, new double?[]{null, null, 1d}, new bool[]{true, false, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed in (doubleBoxed))";
	        Try3Fields(text, new int[]{1, 1, 1}, new int?[]{null, 1, null}, new double?[]{null, null, 1d}, new bool[]{true, false, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed not in (doubleBoxed))";
	        Try3Fields(text, new int[]{1, 1, 1}, new int?[]{null, 1, null}, new double?[]{null, null, 1d}, new bool[]{false, true, true});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed in [doubleBoxed:10))";
	        Try3Fields(text, new int[]{1, 1, 1}, new int?[]{null, 1, 2}, new double?[]{null, null, 1d}, new bool[]{false, false, true});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed not in [doubleBoxed:10))";
	        Try3Fields(text, new int[]{1, 1, 1}, new int?[]{null, 1, 2}, new double?[]{null, null, 1d}, new bool[]{false, true, false});
	    }

	    [Test]
	    public void testFilterStaticFunc()
	    {
	        String text;

	        text = "select * from " + typeof(SupportBean).FullName + "(" +
	                typeof(SupportStaticMethodLib).FullName + ".IsStringEquals('b', string))";
	        TryFilter(text, true);

	        text = "select * from " + typeof(SupportBean).FullName + "(" +
	                typeof(SupportStaticMethodLib).FullName + ".IsStringEquals('bx', string || 'x'))";
	        TryFilter(text, true);

	        text = "select * from " + typeof(SupportBean).FullName + "('b'=string," +
	                typeof(SupportStaticMethodLib).FullName + ".IsStringEquals('bx', string || 'x'))";
	        TryFilter(text, true);

	        text = "select * from " + typeof(SupportBean).FullName + "('b'=string, string='b', string != 'a')";
	        TryFilter(text, true);

	        text = "select * from " + typeof(SupportBean).FullName + "(string != 'a', string != 'c')";
	        TryFilter(text, true);

	        text = "select * from " + typeof(SupportBean).FullName + "(string = 'b', string != 'c')";
	        TryFilter(text, true);

	        text = "select * from " + typeof(SupportBean).FullName + "(string != 'a' and string != 'c')";
	        TryFilter(text, true);

	        text = "select * from " + typeof(SupportBean).FullName + "(string = 'a' and string = 'c' and " +
	                typeof(SupportStaticMethodLib).FullName + ".IsStringEquals('bx', string || 'x'))";
	        TryFilter(text, false);
	    }

	    [Test]
	    public void testFilterRelationalOpRange()
	    {
	        String text;

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed in [2:3])";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, true, true, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed in [2:3] and intBoxed in [2:3])";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, true, true, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed in [2:3] and intBoxed in [2:2])";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, true, false, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed in [1:10] and intBoxed in [3:2])";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, true, true, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed in [3:3] and intBoxed in [1:3])";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, false, true, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed in [3:3] and intBoxed in [1:3] and intBoxed in [4:5])";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, false, false, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed not in [3:3] and intBoxed not in [1:3])";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, false, false, true});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed not in (2:4) and intBoxed not in (1:3))";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {true, false, false, true});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed not in [2:4) and intBoxed not in [1:3))";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, false, false, true});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed not in (2:4] and intBoxed not in (1:3])";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {true, false, false, false});

	        text = "select * from " + typeof(SupportBean).FullName + " where intBoxed not in (2:4)";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {true, true, false, true});

	        text = "select * from " + typeof(SupportBean).FullName + " where intBoxed not in [2:4]";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {true, false, false, false});

	        text = "select * from " + typeof(SupportBean).FullName + " where intBoxed not in [2:4)";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {true, false, false, true});

	        text = "select * from " + typeof(SupportBean).FullName + " where intBoxed not in (2:4]";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {true, true, false, false});

	        text = "select * from " + typeof(SupportBean).FullName + " where intBoxed in (2:4)";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, false, true, false});

	        text = "select * from " + typeof(SupportBean).FullName + " where intBoxed in [2:4]";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, true, true, true});

	        text = "select * from " + typeof(SupportBean).FullName + " where intBoxed in [2:4)";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, true, true, false});

	        text = "select * from " + typeof(SupportBean).FullName + " where intBoxed in (2:4]";
	        TryFilterRelationalOpRange(text, new int[] {1, 2, 3, 4}, new bool[] {false, false, true, true});
	    }

	    public void TryFilterRelationalOpRange(String text, int[] testData, bool[] isReceived)
	    {
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
	        stmt.AddListener(listener);

	        Assert.AreEqual(testData.Length,  isReceived.Length);
	        for (int i = 0; i < testData.Length; i++)
	        {
	            SendBeanIntDouble(testData[i], 0D);
                Assert.AreEqual(isReceived[i], listener.GetAndClearIsInvoked(), "failed testing index " + i);
	        }
            stmt.RemoveListener(listener);
	    }

	    private void TryFilter(String text, bool isReceived)
	    {
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
	        stmt.AddListener(listener);

	        SendBeanString("a");
	        Assert.IsFalse(listener.GetAndClearIsInvoked());
	        SendBeanString("b");
	        Assert.AreEqual(isReceived, listener.GetAndClearIsInvoked());
	        SendBeanString("c");
	        Assert.IsFalse(listener.GetAndClearIsInvoked());

            stmt.RemoveListener(listener);
	    }

	    private void Try3Fields(String text,
	                            int[] intPrimitive,
	                            int?[] intBoxed,
	                            double?[] doubleBoxed,
	                            bool[] expected)
	    {
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
	        stmt.AddListener(listener);

	        Assert.AreEqual(intPrimitive.Length, doubleBoxed.Length);
	        Assert.AreEqual(intBoxed.Length, doubleBoxed.Length);
	        Assert.AreEqual(expected.Length, doubleBoxed.Length);
	        for (int i = 0; i < intBoxed.Length; i++)
	        {
	            SendBeanIntIntDouble(intPrimitive[i], intBoxed[i], doubleBoxed[i]);
                Assert.AreEqual(expected[i], listener.GetAndClearIsInvoked(), "failed at index " + i);
	        }

	        stmt.Stop();
	    }

	    [Test]
	    public void testFilterBooleanExpr()
	    {
	        String text = "select * from " + typeof(SupportBean).FullName + "(2*intBoxed=doubleBoxed)";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
	        stmt.AddListener(listener);

	        SendBeanIntDouble(20, 50d);
	        Assert.IsFalse(listener.GetAndClearIsInvoked());
	        SendBeanIntDouble(25, 50d);
	        Assert.IsTrue(listener.GetAndClearIsInvoked());

	        text = "select * from " + typeof(SupportBean).FullName + "(2*intBoxed=doubleBoxed, string='s')";
	        stmt = epService.EPAdministrator.CreateEQL(text);
	        SupportUpdateListener listenerTwo = new SupportUpdateListener();
            stmt.AddListener(listenerTwo);

	        SendBeanIntDoubleString(25, 50d, "s");
	        Assert.IsTrue(listenerTwo.GetAndClearIsInvoked());
	        SendBeanIntDoubleString(25, 50d, "x");
	        Assert.IsFalse(listenerTwo.GetAndClearIsInvoked());
	    }

	    [Test]
	    public void testFilterWithEqualsSameCompare()
	    {
	        String text;

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed=doubleBoxed)";
	        TryFilterWithEqualsSameCompare(text, new int[] {1, 1}, new double[] {1, 10}, new bool[] {true, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed=intBoxed and doubleBoxed=doubleBoxed)";
	        TryFilterWithEqualsSameCompare(text, new int[] {1, 1}, new double[] {1, 10}, new bool[] {true, true});

	        text = "select * from " + typeof(SupportBean).FullName + "(doubleBoxed=intBoxed)";
	        TryFilterWithEqualsSameCompare(text, new int[] {1, 1}, new double[] {1, 10}, new bool[] {true, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(doubleBoxed in (intBoxed))";
	        TryFilterWithEqualsSameCompare(text, new int[] {1, 1}, new double[] {1, 10}, new bool[] {true, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed in (doubleBoxed))";
	        TryFilterWithEqualsSameCompare(text, new int[] {1, 1}, new double[] {1, 10}, new bool[] {true, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(doubleBoxed not in (10, intBoxed))";
	        TryFilterWithEqualsSameCompare(text, new int[] {1, 1, 1}, new double[] {1, 5, 10}, new bool[] {false, true, false});

	        text = "select * from " + typeof(SupportBean).FullName + "(doubleBoxed in (intBoxed:20))";
	        TryFilterWithEqualsSameCompare(text, new int[] {0, 1, 2}, new double[] {1, 1, 1}, new bool[] {true, false, false});
	    }

	    private void TryFilterWithEqualsSameCompare(String text, int[] intBoxed, double[] doubleBoxed, bool[] expected)
	    {
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
	        stmt.AddListener(listener);

	        Assert.AreEqual(intBoxed.Length, doubleBoxed.Length);
	        Assert.AreEqual(expected.Length, doubleBoxed.Length);
	        for (int i = 0; i < intBoxed.Length; i++)
	        {
	            SendBeanIntDouble(intBoxed[i], doubleBoxed[i]);
                Assert.AreEqual(expected[i], listener.GetAndClearIsInvoked(), "failed at index " + i);
	        }

	        stmt.Stop();
	    }

	    [Test]
	    public void testInvalid()
	    {
	        TryInvalid("select * from pattern [every a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportMarketDataBean).FullName + "(sum(a.longBoxed) = 2)]",
	                "Aggregation functions not allowed within filters [select * from pattern [every a=net.esper.support.bean.SupportBean -> b=net.esper.support.bean.SupportMarketDataBean(sum(a.longBoxed) = 2)]]");

	        TryInvalid("select * from pattern [every a=" + typeof(SupportBean).FullName + "(prior(1, a.longBoxed))]",
	                "Prior function cannot be used in this context [select * from pattern [every a=net.esper.support.bean.SupportBean(prior(1, a.longBoxed))]]");

	        TryInvalid("select * from pattern [every a=" + typeof(SupportBean).FullName + "(prev(1, a.longBoxed))]",
	                "Previous function cannot be used in this context [select * from pattern [every a=net.esper.support.bean.SupportBean(prev(1, a.longBoxed))]]");

	        TryInvalid("select * from " + typeof(SupportBean).FullName + "(5 - 10)",
	                "Filter expression not returning a bool value: '(5-10)' [select * from net.esper.support.bean.SupportBean(5 - 10)]");

	        TryInvalid("select * from pattern [a=" + typeof(SupportBean).FullName + " -> b=" +
	                typeof(SupportBean).FullName + "(doubleBoxed not in (doubleBoxed, x.intBoxed, 9))]",
	                "Failed to resolve property 'x.intBoxed' to a stream or nested property in a stream [select * from pattern [a=net.esper.support.bean.SupportBean -> b=net.esper.support.bean.SupportBean(doubleBoxed not in (doubleBoxed, x.intBoxed, 9))]]");

	        TryInvalid("select * from pattern [a=" + typeof(SupportBean).FullName
	                + " -> b=" + typeof(SupportBean).FullName + "(c.intPrimitive=a.intPrimitive)"
	                + " -> c=" + typeof(SupportBean).FullName
	                + "]",
	                "Failed to resolve property 'c.intPrimitive' to a stream or nested property in a stream [select * from pattern [a=net.esper.support.bean.SupportBean -> b=net.esper.support.bean.SupportBean(c.intPrimitive=a.intPrimitive) -> c=net.esper.support.bean.SupportBean]]");
	    }

	    private void TryInvalid(String text, String expectedMsg)
	    {
	        try
	        {
	            epService.EPAdministrator.CreateEQL(text);
	            Assert.Fail();
	        }
	        catch (EPStatementException ex)
	        {
	            Assert.AreEqual(expectedMsg, ex.Message);
	        }
	    }

	    [Test]
	    public void testPatternWithExpr()
	    {
	        String text = "select * from pattern [every a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportMarketDataBean).FullName + "(a.longBoxed=volume*2)]";
	        TryPatternWithExpr(text);

	        text = "select * from pattern [every a=" + typeof(SupportBean).FullName + " -> " +
	                "b=" + typeof(SupportMarketDataBean).FullName + "(volume*2=a.longBoxed)]";
	        TryPatternWithExpr(text);
	    }

	    private void TryPatternWithExpr(String text)
	    {
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
	        stmt.AddListener(listener);

	        SendBeanLong(10L);
	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("IBM", 0, 0L, ""));
	        Assert.IsFalse(listener.GetAndClearIsInvoked());

	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("IBM", 0, 5L, ""));
	        Assert.IsTrue(listener.GetAndClearIsInvoked());

	        SendBeanLong(0L);
	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("IBM", 0, 0L, ""));
	        Assert.IsTrue(listener.GetAndClearIsInvoked());
	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("IBM", 0, 1L, ""));
	        Assert.IsFalse(listener.GetAndClearIsInvoked());

	        SendBeanLong(20L);
	        epService.EPRuntime.SendEvent(new SupportMarketDataBean("IBM", 0, 10L, ""));
	        Assert.IsTrue(listener.GetAndClearIsInvoked());

	        stmt.RemoveAllListeners();
	    }

	    [Test]
	    public void testMathExpression()
	    {
	        String text;

	        text = "select * from " + typeof(SupportBean).FullName + "(intBoxed*doubleBoxed > 20)";
	        TryArithmatic(text);

	        text = "select * from " + typeof(SupportBean).FullName + "(20 < intBoxed*doubleBoxed)";
	        TryArithmatic(text);

	        text = "select * from " + typeof(SupportBean).FullName + "(20/intBoxed < doubleBoxed)";
	        TryArithmatic(text);

	        text = "select * from " + typeof(SupportBean).FullName + "(20/intBoxed/doubleBoxed < 1)";
	        TryArithmatic(text);
	    }

	    private void TryArithmatic(String text)
	    {
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(text);
	        stmt.AddListener(listener);

	        SendBeanIntDouble(5, 5d);
	        Assert.IsTrue(listener.GetAndClearIsInvoked());

	        SendBeanIntDouble(5, 4d);
	        Assert.IsFalse(listener.GetAndClearIsInvoked());

	        SendBeanIntDouble(5, 4.001d);
	        Assert.IsTrue(listener.GetAndClearIsInvoked());

	        stmt.Destroy();
	    }

	    [Test]
	    public void testExpressionReversed()
	    {
	        String expr = "select * from " + typeof(SupportBean).FullName + "(5 = intBoxed)";
	        EPStatement stmt = epService.EPAdministrator.CreateEQL(expr);
	        stmt.AddListener(listener);

	        SendBean("intBoxed", 5);
	        Assert.IsTrue(listener.GetAndClearIsInvoked());
	    }

	    private void SendBeanIntDouble(int? intBoxed, double? doubleBoxed)
	    {
	        SupportBean _event = new SupportBean();
	        _event.SetIntBoxed(intBoxed);
	        _event.SetDoubleBoxed(doubleBoxed);
	        epService.EPRuntime.SendEvent(_event);
	    }

	    private void SendBeanIntDoubleString(int? intBoxed, double? doubleBoxed, String _string)
	    {
	        SupportBean _event = new SupportBean();
	        _event.SetIntBoxed(intBoxed);
	        _event.SetDoubleBoxed(doubleBoxed);
	        _event.SetString(_string);
	        epService.EPRuntime.SendEvent(_event);
	    }

	    private void SendBeanIntIntDouble(int intPrimitive, int? intBoxed, double? doubleBoxed)
	    {
	        SupportBean _event = new SupportBean();
	        _event.SetIntPrimitive(intPrimitive);
	        _event.SetIntBoxed(intBoxed);
	        _event.SetDoubleBoxed(doubleBoxed);
	        epService.EPRuntime.SendEvent(_event);
	    }

	    private void SendBeanLong(long? longBoxed)
	    {
	        SupportBean _event = new SupportBean();
	        _event.SetLongBoxed(longBoxed);
	        epService.EPRuntime.SendEvent(_event);
	    }

	    private void SendBeanString(String _string)
	    {
	        SupportBean num = new SupportBean(_string, -1);
	        epService.EPRuntime.SendEvent(num);
	    }

	    private void SendBean(String fieldName, Object value)
	    {
	        SupportBean _event = new SupportBean();
	        if (fieldName.Equals("string"))
	        {
	            _event.SetString((String) value);
	        }
	        else if (fieldName.Equals("boolPrimitive"))
	        {
	            _event.SetBoolPrimitive((Boolean) value);
	        }
	        else if (fieldName.Equals("intBoxed"))
	        {
	            _event.SetIntBoxed((int?) value);
	        }
	        else if (fieldName.Equals("longBoxed"))
	        {
	            _event.SetLongBoxed((long?) value);
	        }
	        else
	        {
	            throw new ArgumentException("field name not known");
	        }
	        epService.EPRuntime.SendEvent(_event);
	    }
	}
} // End of namespace
