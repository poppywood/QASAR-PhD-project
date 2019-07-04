///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Threading;

using com.espertech.esper.compat;
using com.espertech.esper.timer;
using log4net;

namespace com.espertech.esper.schedule
{
    /// <summary>
	/// Implements the schedule service by simply keeping a sorted set of long millisecond
    /// values and a set of handles for each.
    /// </summary>

    public sealed class SchedulingServiceImpl : SchedulingService
    {
        // Map of time and handle
        private readonly TreeMap<long, TreeMap<ScheduleSlot, ScheduleHandle>> timeHandleMap;

        // Map of handle and handle list for faster removal
        private readonly Map<ScheduleHandle, TreeMap<ScheduleSlot, ScheduleHandle>> handleSetMap;

        private readonly TimeSourceService timeSourceService;

        // Current time - used for evaluation as well as for adding new handles
        private long currentTime;

        // Current bucket number - for use in ordering handles by bucket
        private int curBucketNum;

        private readonly MonitorLock dataLock;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timeSourceService">time source provider</param>
        public SchedulingServiceImpl(TimeSourceService timeSourceService)
        {
            this.dataLock = new MonitorLock();
            this.timeSourceService = timeSourceService;
            this.timeHandleMap = new TreeMap<long, TreeMap<ScheduleSlot, ScheduleHandle>>();
            this.handleSetMap = new HashMap<ScheduleHandle, TreeMap<ScheduleSlot, ScheduleHandle>>();
            // initialize time to just before now as there is a check for duplicate external time events
            this.currentTime = timeSourceService.GetTimeMillis() - 1;
        }

        public void Destroy()
        {
            handleSetMap.Clear();
            timeHandleMap.Clear();
        }

        /// <summary>
        /// Returns a bucket from which slots can be allocated for ordering concurrent handles.
        /// </summary>
        /// <returns>bucket</returns>
        public ScheduleBucket AllocateBucket()
        {
			using(dataLock.Acquire())
			{
				curBucketNum++;
				return new ScheduleBucket(curBucketNum);
			}
        }

        /// <summary>
        /// Gets the last time known to the scheduling service.
        /// </summary>
        /// <value></value>
        /// <returns> time that has last been set on this service
        /// </returns>
        public long Time
        {
            get { return Interlocked.Read(ref this.currentTime); }
            set { Interlocked.Exchange(ref this.currentTime, value); }
        }

        /// <summary>
        /// Add a handle for after the given milliseconds from the current time.
        /// If the same handle (equals) was already added before, the method will not add a new
        /// handle or change the existing handle to a new time, but throw an exception.
        /// </summary>
        /// <param name="afterMSec">number of millisec to get a handle</param>
        /// <param name="handle">to add</param>
        /// <param name="slot">allows ordering of concurrent handles</param>
        /// <throws>  ScheduleServiceException thrown if the add operation did not complete </throws>
        public void Add(long afterMSec, ScheduleHandle handle, ScheduleSlot slot)
        {
            using (dataLock.Acquire())
			{
	            if (handleSetMap.ContainsKey(handle))
	            {
	                String message = "Handle already in collection";
	                log.Fatal(".add " + message);
	                throw new ScheduleHandleExistsException(message);
	            }

			    long lCurrentTime = Interlocked.Read(ref currentTime);
                long triggerOnTime = lCurrentTime + afterMSec;

	            AddTrigger(slot, handle, triggerOnTime);
			}
        }

        /// <summary>
        /// Adds the specified spec.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="slot">The slot.</param>
        public void Add(ScheduleSpec spec, ScheduleHandle handle, ScheduleSlot slot)
        {
			using(dataLock.Acquire())
			{
	            if (handleSetMap.ContainsKey(handle))
	            {
	                String message = "Handle already in collection";
	                log.Fatal(".add " + message);
                    throw new ScheduleHandleExistsException(message);
	            }

                long lCurrentTime = Interlocked.Read(ref currentTime);
	            long nextScheduledTime = ScheduleComputeHelper.ComputeNextOccurance(spec, lCurrentTime);
	            if (nextScheduledTime <= lCurrentTime)
	            {
	                String message = "Schedule computation returned invalid time, operation not completed";
	                log.Fatal(".add " + message + "  nextScheduledTime=" + nextScheduledTime + "  currentTime=" + lCurrentTime);
	                return;
	            }

	            AddTrigger(slot, handle, nextScheduledTime);
			}
        }

        /// <summary>
        /// Remove a handle.
        /// If the handle to be removed was not found an exception is thrown.
        /// </summary>
        /// <param name="handle">to remove</param>
        /// <param name="slot">for which the handle was added</param>
        /// <throws>  ScheduleServiceException thrown if the handle was not located </throws>
        public void Remove(ScheduleHandle handle, ScheduleSlot slot)
        {
            using (dataLock.Acquire())
            {
                TreeMap<ScheduleSlot, ScheduleHandle> handleSet;

                if ( !handleSetMap.Remove(handle, out handleSet))
                {
                    // If it already has been removed then that's fine;
                    // Such could be the case when 2 timers fireStatementStopped at the same time, and one stops the other
                    return;
                }

                handleSet.Remove(slot);

                if (log.IsInfoEnabled)
                {
                    log.Info(".Remove - handle = " + handle);
                }
            }
        }

        /// <summary>
        /// Evaluate the current time and perform any handles.
        /// </summary>
        public void Evaluate(ICollection<ScheduleHandle> handles)
        {
			using(dataLock.Acquire())
			{
                long lCurrentTime = Interlocked.Read(ref currentTime);

	            // Get the values on or before the current time - to get those that are exactly on the
	            // current time we just add one to the current time for getting the head map
			    IEnumerable<KeyValuePair<Int64, TreeMap<ScheduleSlot, ScheduleHandle>>> headMap =
			        timeHandleMap.HeadFast(lCurrentTime + 1);

	            // First determine all triggers to shoot
	            IList<Int64> removeKeys = new List<Int64>();
                foreach (KeyValuePair<Int64, TreeMap<ScheduleSlot, ScheduleHandle>> entry in headMap)
			    {
                    removeKeys.Add(entry.Key);
	                foreach (ScheduleHandle handle in entry.Value.Values)
	                {
	                    handles.Add(handle);
	                }
	            }

	            // Next remove all handles
                foreach( KeyValuePair<Int64, TreeMap<ScheduleSlot, ScheduleHandle>> entry in headMap )
	            {
	                foreach (ScheduleHandle handle in entry.Value.Values)
	                {
	                    handleSetMap.Remove(handle);
	                }
	            }

	            // Remove all triggered msec values
	            foreach (Int64 key in removeKeys)
	            {
	                timeHandleMap.Remove(key);
	            }
			}
        }

        private void AddTrigger(ScheduleSlot slot, ScheduleHandle handle, long triggerTime)
        {
            TreeMap<ScheduleSlot, ScheduleHandle> handleSet = timeHandleMap.Get(triggerTime);
            if (handleSet == null)
            {
                timeHandleMap[triggerTime] = handleSet = new TreeMap<ScheduleSlot, ScheduleHandle>();
            }

            handleSet[slot] = handle;
            handleSetMap[handle] = handleSet;

            if (log.IsInfoEnabled)
            {
                log.Info(".AddTrigger - handle = " + handle);
            }
        }

        /// <summary>Returns time handle count.</summary>
        /// <returns>count</returns>
        public int TimeHandleCount
        {
            get { return timeHandleMap.Count; }
        }

        /// <summary>Returns furthest in the future handle.</summary>
        /// <returns>future handle</returns>
        public long FurthestTimeHandle
        {
            get { return timeHandleMap.LastKey; }
        }

        /// <summary>Returns count of handles.</summary>
        /// <returns>count</returns>
        public int ScheduleHandleCount
        {
            get { return handleSetMap.Count; }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
