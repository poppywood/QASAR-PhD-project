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
using net.esper.events;
using NUnit.Framework;

namespace net.esper.support.util
{
    public class SupportUpdateListener : UpdateListener
    {
        private readonly List<EventBean[]> newDataList;
        private readonly List<EventBean[]> oldDataList;
        private EventBean[] lastNewData;
        private EventBean[] lastOldData;
        private bool isInvoked;

        public SupportUpdateListener()
        {
            newDataList = new List<EventBean[]>();
            oldDataList = new List<EventBean[]>();
        }

        public void Update(EventBean[] newData, EventBean[] oldData)
        {
            oldDataList.Add(oldData);
            newDataList.Add(newData);

            lastNewData = newData;
            lastOldData = oldData;

            isInvoked = true;
        }

        public void Reset()
        {
            oldDataList.Clear();
            newDataList.Clear();
            lastNewData = null;
            lastOldData = null;
            isInvoked = false;
        }


        public EventBean[] GetAndResetLastNewData()
        {
            EventBean[] lastNew = lastNewData;
            Reset();
            return lastNew;
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

        public EventBean[] LastNewData
        {
            get { return lastNewData; }
            set { lastNewData = value; }
        }

        public EventBean[] LastOldData
        {
            get { return lastOldData; }
            set { lastOldData = value; }
        }

        public List<EventBean[]> NewDataList
        {
            get { return newDataList; }
        }

        public List<EventBean[]> OldDataList
        {
            get { return oldDataList; }
        }

        public bool IsInvoked
        {
            get { return isInvoked; }
        }

        public bool GetAndClearIsInvoked()
        {
            bool invoked = isInvoked;
            isInvoked = false;
            return invoked;
        }
    }
} // End of namespace
