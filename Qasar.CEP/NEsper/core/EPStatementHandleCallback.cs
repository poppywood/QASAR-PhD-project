///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.filter;
using com.espertech.esper.schedule;

namespace com.espertech.esper.core
{
	/// <summary>
	/// Statement resource handle and callback for use with <see cref="com.espertech.esper.filter.FilterService"/> and
	/// <see cref="com.espertech.esper.schedule.SchedulingService"/>.
	/// <para/>
	/// Links the statement handle identifying a statement and containing the statement resource lock,
	/// with the actual callback to invoke for a statement together.
	/// </summary>
	public class EPStatementHandleCallback : FilterHandle, ScheduleHandle
	{
	    private static int idCount;

	    private readonly int id;
	    private readonly EPStatementHandle epStatementHandle;
        private readonly FilterHandleCallback filterCallback;
        private readonly ScheduleHandleCallback scheduleCallback;

	    /// <summary>Ctor.</summary>
	    /// <param name="epStatementHandle">is a statement handle</param>
	    /// <param name="callback">is a filter callback</param>
	    public EPStatementHandleCallback(EPStatementHandle epStatementHandle, FilterHandleCallback callback)
	    {
	        this.id = System.Threading.Interlocked.Increment(ref idCount);
	        this.epStatementHandle = epStatementHandle;
	        this.filterCallback = callback;
	    }

	    /// <summary>Ctor.</summary>
	    /// <param name="epStatementHandle">is a statement handle</param>
	    /// <param name="callback">is a schedule callback</param>
	    public EPStatementHandleCallback(EPStatementHandle epStatementHandle, ScheduleHandleCallback callback)
	    {
            this.id = System.Threading.Interlocked.Increment(ref idCount);
            this.epStatementHandle = epStatementHandle;
	        this.scheduleCallback = callback;
	    }

	    /// <summary>Returns the statement handle.</summary>
	    /// <returns>handle containing a statement resource lock</returns>
	    public EPStatementHandle EpStatementHandle
	    {
	    	get { return epStatementHandle; }
	    }

	    /// <summary>
	    /// Returns the statement filter callback, or null if this is a schedule callback handle.
	    /// </summary>
	    /// <returns>filter callback</returns>
	    public FilterHandleCallback FilterCallback
	    {
	    	get { return filterCallback; }
	    }

	    /// <summary>
	    /// Returns the statement schedule callback, or null if this is a filter callback handle.
	    /// </summary>
	    /// <returns>schedule callback</returns>
	    public ScheduleHandleCallback ScheduleCallback
	    {
	    	get { return scheduleCallback; }
	    }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return
                "EPStatementHandleCallback{" +
                "Id=" + id +
                ",EPStatementHandle=" + epStatementHandle +
                ",FilterCallback=" + filterCallback +
                ",ScheduleCallback=" + scheduleCallback +
                "}";
        }
	}
} // End of namespace
