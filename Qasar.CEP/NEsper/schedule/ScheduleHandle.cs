///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.schedule
{
	/// <summary>
	/// Marker interface for use with <see cref="SchedulingService"/>. Implementations serve as a schedule trigger values when
	/// the schedule is reached to trigger or return the handle.
	/// </summary>
	public interface ScheduleHandle
	{
	}
} // End of namespace
