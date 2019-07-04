using System;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// A simple locking mechanism
    /// </summary>

    public interface ILockable
    {
        /// <summary>
        /// Acquires the lock; the lock is released when the disposable
        /// object that was returned is disposed.
        /// </summary>
        /// <returns></returns>

        IDisposable Acquire();
    }
}
