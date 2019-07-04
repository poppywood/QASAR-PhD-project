///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;

using net.esper.compat;
using net.esper.core;

namespace net.esper.util
{
	/// <summary>
	/// Simple lock based on {@link ReentrantLock} that associates a name with the lock and traces locking and unlocking.
	/// </summary>
	public sealed class ManagedLockImpl : MonitorLock, ManagedLock
	{
	    private readonly Guid id;
        private readonly String name;

	    /// <summary>Ctor.</summary>
	    /// <param name="name">of lock</param>
	    public ManagedLockImpl(String name)
	    {
	        this.id = Guid.NewGuid();
	        this.name = name;
	    }

	    /// <summary>Lock.</summary>
	    public IDisposable AcquireLock(StatementLockFactory statementLockFactory)
	    {
	        return base.Acquire();
	    }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return "ManagedLockImpl{" +
                   "id=" + id +
                   ";name=" + name +
                   ";acquired=" + base.IsHeldByCurrentThread +
                   ";base=" + base.ToString() +
                   "}";
        }
	}
} // End of namespace
