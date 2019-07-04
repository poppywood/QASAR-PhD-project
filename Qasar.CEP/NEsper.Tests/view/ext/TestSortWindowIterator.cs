///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using NUnit.Framework;

using net.esper.collection;
using net.esper.compat;
using net.esper.events;
using net.esper.support.events;
using net.esper.support.util;
using net.esper.util;

namespace net.esper.view.ext
{
	[TestFixture]
	public class TestSortWindowIterator
	{
	    private EDictionary<String, EventBean> events;
		private TreeDictionary<MultiKeyUntyped, LinkedList<EventBean>> testMap;
		private IComparer<MultiKeyUntyped> comparator;

	    [SetUp]
	    public void SetUp()
	    {
	        events = EventFactoryHelper.MakeEventMap(new String[] {"a", "b", "c", "d", "f", "g"});
	        comparator = new MultiKeyComparator(new bool[] {false});
	        testMap = new TreeDictionary<MultiKeyUntyped, LinkedList<EventBean>>(comparator);
	    }

	    [Test]
	    public void testEmpty()
	    {
	        IEnumerator<EventBean> it = new SortWindowIterator(testMap);
	        ArrayAssertionUtil.AreEqualExactOrder(it, null);
	    }

	    [Test]
	    public void testOneElement()
	    {
	        LinkedList<EventBean> list = new LinkedList<EventBean>();
	        list.AddLast(events.Get("a"));
	        MultiKeyUntyped key = new MultiKeyUntyped(new Object[] {"akey"});
	        testMap.Put(key, list);

	        IEnumerator<EventBean> it = new SortWindowIterator(testMap);
	        ArrayAssertionUtil.AreEqualExactOrder(it, new EventBean[] {events.Get("a")} );
	    }

	    [Test]
	    public void testTwoInOneEntryElement()
	    {
            LinkedList<EventBean> list = new LinkedList<EventBean>();
            list.AddLast(events.Get("a"));
            list.AddLast(events.Get("b"));
	        MultiKeyUntyped key = new MultiKeyUntyped(new Object[] {"keyA"});
	        testMap.Put(key, list);

	        IEnumerator<EventBean> it = new SortWindowIterator(testMap);
	        ArrayAssertionUtil.AreEqualExactOrder(it, new EventBean[] {events.Get("a"), events.Get("b")} );
	    }

	    [Test]
	    public void testTwoSeparateEntryElement()
	    {
            LinkedList<EventBean> list1 = new LinkedList<EventBean>();
            list1.AddLast(events.Get("a"));
	        MultiKeyUntyped keyB = new MultiKeyUntyped(new Object[] {"keyB"});
	        testMap.Put(keyB, list1);
            LinkedList<EventBean> list2 = new LinkedList<EventBean>();
            list2.AddLast(events.Get("b"));
	        MultiKeyUntyped keyA = new MultiKeyUntyped(new Object[] {"keyA"});
	        testMap.Put(keyA, list2); // Actually before list1

	        IEnumerator<EventBean> it = new SortWindowIterator(testMap);
	        ArrayAssertionUtil.AreEqualExactOrder(it, new EventBean[] {events.Get("b"), events.Get("a")} );
	    }

	    [Test]
	    public void testTwoByTwoEntryElement()
	    {
            LinkedList<EventBean> list1 = new LinkedList<EventBean>();
	        list1.AddLast(events.Get("a"));
            list1.AddLast(events.Get("b"));
	        MultiKeyUntyped keyB = new MultiKeyUntyped(new Object[] {"keyB"});
	        testMap.Put(keyB, list1);
            LinkedList<EventBean> list2 = new LinkedList<EventBean>();
            list2.AddLast(events.Get("c"));
            list2.AddLast(events.Get("d"));
	        MultiKeyUntyped keyC = new MultiKeyUntyped(new Object[] {"keyC"});
	        testMap.Put(keyC, list2);

	        IEnumerator<EventBean> it = new SortWindowIterator(testMap);
	        ArrayAssertionUtil.AreEqualExactOrder(it, new EventBean[] {events.Get("a"), events.Get("b"), events.Get("c"), events.Get("d")} );
	    }

	    [Test]
	    public void testMixedEntryElement()
	    {
            LinkedList<EventBean> list1 = new LinkedList<EventBean>();
            list1.AddLast(events.Get("a"));
	        MultiKeyUntyped keyA = new MultiKeyUntyped(new Object[] {"keyA"});
	        testMap.Put(keyA, list1);
            LinkedList<EventBean> list2 = new LinkedList<EventBean>();
            list2.AddLast(events.Get("c"));
            list2.AddLast(events.Get("d"));
	        MultiKeyUntyped keyB = new MultiKeyUntyped(new Object[] {"keyB"});
	        testMap.Put(keyB, list2);
            LinkedList<EventBean> list3 = new LinkedList<EventBean>();
            list3.AddLast(events.Get("e"));
            list3.AddLast(events.Get("f"));
            list3.AddLast(events.Get("g"));
	        MultiKeyUntyped keyC = new MultiKeyUntyped(new Object[] {"keyC"});
	        testMap.Put(keyC, list3);

	        IEnumerator<EventBean> it = new SortWindowIterator(testMap);
	        ArrayAssertionUtil.AreEqualExactOrder(it, new EventBean[] {events.Get("a"), events.Get("c"), events.Get("d"),
	                events.Get("e"), events.Get("f"), events.Get("g")} );
	    }
	}


} // End of namespace
