///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace net.esper.pattern.observer
{
	/// <summary>
	/// Abstract class for applications to extend to implement a pattern observer.
	/// </summary>
	public abstract class EventObserverSupport : EventObserver
	{
		abstract public void StartObserve();
		abstract public void StopObserve();
	}
} // End of namespace
