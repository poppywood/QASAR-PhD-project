// ---------------------------------------------------------------------------------- /
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
// ---------------------------------------------------------------------------------- /

using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.compat;
using net.esper.events;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.support.util
{
	public class SupportMTUpdateListener : UpdateListener
	{
	    private readonly List<EventBean[]> newDataList;
	    private readonly List<EventBean[]> oldDataList;
	    private EventBean[] lastNewData;
	    private EventBean[] lastOldData;
	    private bool isInvoked;
	    private readonly Object dataLock;
	    private readonly bool isDebugEnabled;

	    public SupportMTUpdateListener()
	    {
            dataLock = new Object();
	        newDataList = new List<EventBean[]>(1000);
	        oldDataList = new List<EventBean[]>(1000);
	        isDebugEnabled = log.IsDebugEnabled;
	    }

	    public void Update(EventBean[] newData, EventBean[] oldData)
	    {
            lock(dataLock)
	    	{
		        this.oldDataList.Add(oldData);
		        this.newDataList.Add(newData);
	
		        this.lastNewData = newData;
		        this.lastOldData = oldData;

                if (isDebugEnabled)
                {
                    log.Debug("update: " + newData.Length + " / " + newDataList.Count);
                }

	    	    isInvoked = true;
	    	}
	    }

	    public void Reset()
	    {
            lock (dataLock)
            {
		        this.oldDataList.Clear();
		        this.newDataList.Clear();
		        this.lastNewData = null;
		        this.lastOldData = null;
		        isInvoked = false;
	    	}
	    }

	    public EventBean[] LastNewData
	    {
            get { return lastNewData; }
            set { lastNewData = value; }
	    }

	    public EventBean[] GetAndResetLastNewData()
	    {
            lock (dataLock)
            {
		        EventBean[] lastNew = lastNewData;
		        Reset();
		        return lastNew;
	    	}
	    }

	    public EventBean AssertOneGetNewAndReset()
	    {
            lock (dataLock)
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
	    }

	    public EventBean AssertOneGetOldAndReset()
	    {
            lock (dataLock)
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
	    }

	    public EventBean[] LastOldData
	    {
            get { return lastOldData; }
            set { lastOldData = value; }
	    }

	    public IList<EventBean[]> NewDataList
	    {
            get { return newDataList; }
	    }

	    public IList<EventBean[]> OldDataList
	    {
            get { return oldDataList; }
	    }

	    public bool IsInvoked
	    {
            get { return isInvoked; }
	    }

	    public bool GetAndClearIsInvoked()
	    {
            lock (dataLock)
            {
		        bool invoked = isInvoked;
		        isInvoked = false;
		        return invoked;
	    	}
	    }

	    public EventBean[] GetNewDataListFlattened()
	    {
            lock (dataLock)
            {
	        	return Flatten(newDataList);
	    	}
	    }

	    public EventBean[] GetOldDataListFlattened()
	    {
            lock (dataLock)
            {
	        	return Flatten(oldDataList);
	    	}
	    }

	    private EventBean[] Flatten(List<EventBean[]> list)
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

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
} // End of namespace
