using System;
using System.Threading;

using com.espertech.esper.util;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Disposable object that acquires a read lock and disposes
    /// of the lock when it goes out of scope.
    /// </summary>

    public class ReaderLock : BaseLock
    {
        /// <summary>
        /// Unmanaged lock object
        /// </summary>
        private FastReaderWriterLock m_uLockObj;
        /// <summary>
        /// Managed lock object
        /// </summary>
        private ManagedReadWriteLock m_mLockObj;
        /// <summary>
        /// Indicates if we acquired the lock
        /// </summary>
        private bool m_lockAcquired;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderLock"/> class.
        /// </summary>
        /// <param name="lockObj">The lock obj.</param>
        public ReaderLock(FastReaderWriterLock lockObj)
        {
            this.m_lockAcquired = false;
            this.m_mLockObj = null;
            this.m_uLockObj = lockObj;

            if (ThreadLogUtil.ENABLED_TRACE)
            {
                ThreadLogUtil.TraceLock(ACQUIRE_TEXT + " read " + m_uLockObj, lockObj);
            }
            
            this.m_uLockObj.AcquireReaderLock(LockConstants.ReaderTimeout);
            this.m_lockAcquired = true;

            if (ThreadLogUtil.ENABLED_TRACE)
            {
                ThreadLogUtil.TraceLock(ACQUIRED_TEXT + " read " + m_uLockObj, lockObj);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderLock"/> class.
        /// </summary>
        /// <param name="lockObj">The lock obj.</param>
        public ReaderLock(ManagedReadWriteLock lockObj)
        {
            this.m_lockAcquired = false;
            this.m_uLockObj = null;
            this.m_mLockObj = lockObj;
            this.m_mLockObj.AcquireReadLock();
            this.m_lockAcquired = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            lock (this)
            {
                // Release the locks.  Only one of the locks will be allocated
                // so there isn't any real risk to having this fire for two
                // lock objects.

                if (this.m_lockAcquired)
                {
                    if (this.m_uLockObj != null)
                    {
                        if (ThreadLogUtil.ENABLED_TRACE)
                        {
                            ThreadLogUtil.TraceLock(RELEASE_TEXT + " read " + m_uLockObj, m_uLockObj);
                        }

                        this.m_uLockObj.ReleaseReaderLock();

                        if (ThreadLogUtil.ENABLED_TRACE)
                        {
                            ThreadLogUtil.TraceLock(RELEASED_TEXT + " read " + m_uLockObj, m_uLockObj);
                        }
                        
                        this.m_uLockObj = null;
                        this.m_lockAcquired = false;
                    }

                    if (this.m_mLockObj != null)
                    {
                        this.m_mLockObj.ReleaseReadLock();
                        this.m_mLockObj = null;
                        this.m_lockAcquired = false;
                    }
                }
            }
        }
    }
}
