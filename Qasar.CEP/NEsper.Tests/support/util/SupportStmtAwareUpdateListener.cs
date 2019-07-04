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
using net.esper.collection;
using net.esper.events;

using NUnit.Framework;

namespace net.esper.support.util
{
	public class SupportStmtAwareUpdateListener : StatementAwareUpdateListener
	{
        private readonly IList<EPStatement> statementList;
	    private readonly IList<EPServiceProvider> svcProviderList;
	    private readonly IList<EventBean[]> newDataList;
	    private readonly IList<EventBean[]> oldDataList;
	    private EventBean[] lastNewData;
	    private EventBean[] lastOldData;
	    private bool isInvoked;

	    public SupportStmtAwareUpdateListener()
	    {
	        newDataList = new List<EventBean[]>();
	        oldDataList = new List<EventBean[]>();
	        statementList = new List<EPStatement>();
	        svcProviderList = new List<EPServiceProvider>();
	    }

	    public void Update(EventBean[] newData, EventBean[] oldData, EPStatement statement, EPServiceProvider serviceProvider)
	    {
	        statementList.Add(statement);
	        svcProviderList.Add(serviceProvider);

	        this.oldDataList.Add(oldData);
	        this.newDataList.Add(newData);

	        this.lastNewData = newData;
	        this.lastOldData = oldData;

	        isInvoked = true;
	    }

	    public void Reset()
	    {
	        statementList.Clear();
	        svcProviderList.Clear();
	        this.oldDataList.Clear();
	        this.newDataList.Clear();
	        this.lastNewData = null;
	        this.lastOldData = null;
	        isInvoked = false;
	    }

	    public EventBean[] GetLastNewData()
	    {
	        return lastNewData;
	    }

	    public EventBean[] GetAndResetLastNewData()
	    {
	        EventBean[] lastNew = lastNewData;
	        Reset();
	        return lastNew;
	    }

	    public IList<EPStatement> StatementList
	    {
	        get {return statementList;}
	    }

	    public IList<EPServiceProvider> SvcProviderList
	    {
	        get {return svcProviderList;}
	    }

	    public EventBean AssertOneGetNewAndReset()
	    {
	        Assert.IsTrue(isInvoked);

	        Assert.AreEqual(1, newDataList.Count);
	        Assert.AreEqual(1, oldDataList.Count);

	        Assert.AreEqual(1, lastNewData.Length);
	        Assert.IsNull(lastOldData);

	        EventBean lastNew = lastNewData[0];
	        Reset();
	        return lastNew;
	    }

	    public EventBean AssertOneGetOldAndReset()
	    {
	        Assert.IsTrue(isInvoked);

            Assert.AreEqual(1, newDataList.Count);
            Assert.AreEqual(1, oldDataList.Count);

            Assert.AreEqual(1, lastOldData.Length);
	        Assert.IsNull(lastNewData);

	        EventBean lastNew = lastOldData[0];
	        Reset();
	        return lastNew;
	    }

	    public EventBean[] GetLastOldData()
	    {
	        return lastOldData;
	    }

	    public IList<EventBean[]> NewDataList
	    {
	        get {return newDataList;}
	    }

	    public IList<EventBean[]> OldDataList
	    {
	        get {return oldDataList;}
	    }

	    public bool IsInvoked
	    {
	        get {return isInvoked;}
	    }

        public bool GetAndClearIsInvoked()
	    {
            bool invoked = isInvoked;
	        isInvoked = false;
	        return invoked;
	    }

	    public void SetLastNewData(EventBean[] lastNewData)
	    {
	        this.lastNewData = lastNewData;
	    }

	    public void SetLastOldData(EventBean[] lastOldData)
	    {
	        this.lastOldData = lastOldData;
	    }

	    public EventBean[] GetNewDataListFlattened()
	    {
	        return Flatten(newDataList);
	    }

	    private static EventBean[] Flatten(IEnumerable<EventBean[]> list)
	    {
	        int count = 0;
	        foreach (EventBean[] events in list)
	        {
	            if (events != null)
	            {
	                count += events.Length;
	            }
	        }

	        EventBean[] array = new EventBean[count];
	        count = 0;
	        foreach (EventBean[] events in list)
	        {
	            if (events != null)
	            {
                    for (int i = 0; i < events.Length; i++)
	                {
	                    array[count++] = events[i];
	                }
	            }
	        }
	        return array;
	    }

	    public void AssertUnderlyingAndReset(Object[] expectedUnderlyingNew, Object[] expectedUnderlyingOld)
	    {
            Assert.AreEqual(1, NewDataList.Count);
	        Assert.AreEqual(1, OldDataList.Count);

	        EventBean[] newEvents = GetLastNewData();
	        EventBean[] oldEvents = GetLastOldData();

	        if (expectedUnderlyingNew != null)
	        {
                Assert.AreEqual(expectedUnderlyingNew.Length, newEvents.Length);
	            for (int i = 0; i < expectedUnderlyingNew.Length; i++)
	            {
	                Assert.AreSame(expectedUnderlyingNew[i], newEvents[i].Underlying);
	            }
	        }
	        else
	        {
	            Assert.IsNull(newEvents);
	        }

	        if (expectedUnderlyingOld != null)
	        {
                Assert.AreEqual(expectedUnderlyingOld.Length, oldEvents.Length);
	            for (int i = 0; i < expectedUnderlyingOld.Length; i++)
	            {
	                Assert.AreSame(expectedUnderlyingOld[i], oldEvents[i].Underlying);
	            }
	        }
	        else
	        {
	            Assert.IsNull(oldEvents);
	        }

	        Reset();
	    }

	    public void AssertFieldEqualsAndReset(String fieldName, Object[] expectedNew, Object[] expectedOld)
	    {
            Assert.AreEqual(1, NewDataList.Count);
            Assert.AreEqual(1, OldDataList.Count);

	        EventBean[] newEvents = GetLastNewData();
	        EventBean[] oldEvents = GetLastOldData();

	        if (expectedNew != null)
	        {
                Assert.AreEqual(expectedNew.Length, newEvents.Length);
	            for (int i = 0; i < expectedNew.Length; i++)
	            {
	                Object result = newEvents[i].Get(fieldName);
                    Assert.AreEqual(expectedNew[i], result);
	            }
	        }
	        else
	        {
	            Assert.IsNull(newEvents);
	        }

	        if (expectedOld != null)
	        {
                Assert.AreEqual(expectedOld.Length, oldEvents.Length);
	            for (int i = 0; i < expectedOld.Length; i++)
	            {
                    Assert.AreEqual(expectedOld[i], oldEvents[i].Get(fieldName));
	            }
	        }
	        else
	        {
	            Assert.IsNull(oldEvents);
	        }

	        Reset();
	    }

	    public UniformPair<EventBean[]> GetDataListsFlattened()
	    {
	        return new UniformPair<EventBean[]>(Flatten(newDataList), Flatten(oldDataList));
	    }
	}
} // End of namespace
