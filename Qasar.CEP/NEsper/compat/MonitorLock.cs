using System;
using System.Threading;

using log4net;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// MonitorLock is a class for assisting people with synchronized operations.
    /// Traditionally, code might have looked something like this:
    /// <code>
    /// lock( object ) { 
    ///   ...
    /// }
    /// </code>
    /// However, this has a few issues.  It's prone to deadlock because the lock
    /// operator does not have a timeout.  It's also difficult to determine who
    /// owns a lock at a given time.  So eventually people changed to this form:
    /// <code>
    /// if (Monitor.TryEnter(object, timeout)) {
    ///   try {
    ///    ...
    ///   } finally {
    ///     Monitor.Exit(object);
    ///   }
    /// }
    /// </code>
    /// It gets bulky and begins to become difficult to maintain over time.
    /// MonitorLock works much like the lock( object ) model except that it relies
    /// upon the IDisposable interface to help with scoping of the lock.  So to
    /// use MonitorLock, first instantiate one and then replace your lock(object)
    /// with this:
    /// <code>
    /// using(lockObj.Acquire()) {
    ///   ...
    /// }
    /// </code>
    /// Your code will work as before except that the monitorLock will use a timed
    /// entry into critical sections and it can be used to diagnose issues that
    /// may be occuring in your thread locking.
    /// <para>
    /// MonitorLock allows users to specify events that can be consumed on lock
    /// acquisition or release.  Additionally, it can inform you when a lock
    /// is acquired within an existing lock.  And last, if you want to know where
    /// your locks are being acquired, it can maintain a StackTrace of points
    /// where allocations are occuring.
    /// </para>
    /// </summary>

    public class MonitorLock : ILockable
    {
        /// <summary>
        /// Default timeout for lock acquisition.
        /// </summary>

        private const int s_uDefaultLockTimeout = 5000;

        /// <summary>
        /// Gets the default timeout.
        /// </summary>
        /// <value>The default timeout.</value>
        public static int DefaultTimeout
        {
            get { return s_uDefaultLockTimeout; }
        }

        /// <summary>
        /// Gets the number of milliseconds until the lock acquisition fails.
        /// </summary>
        /// <value>The lock timeout.</value>

        public int LockTimeout
        {
            get { return m_uLockTimeout; }
        }

        /// <summary>
        /// Uniquely identifies the lock.
        /// </summary>

        private readonly Guid m_uLockId;

        /// <summary>
        /// Underlying object that is locked
        /// </summary>

        private readonly Object m_uLockObj;

        /// <summary>
        /// Number of milliseconds until the lock acquisition fails
        /// </summary>

        private readonly int m_uLockTimeout;

        /// <summary>
        /// Owner of the lock.
        /// </summary>

        private Thread m_uLockOwner;

        /// <summary>
        /// Used to track recursive locks.
        /// </summary>

        private int m_uLockDepth;

        /// <summary>
        /// Indication as to whether we are debugging or not
        /// </summary>

        private readonly bool m_uLockDebug;

        /// <summary>
        /// Gets the lock depth.
        /// </summary>
        /// <value>The lock depth.</value>

        public int LockDepth
        {
            get { return m_uLockDepth; }
        }

#if MONITOR_PERFORMANCE_TESTING
        private readonly ThreadLocal<PerfInfo> m_uDiagInfo;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorLock"/> class.
        /// </summary>
        public MonitorLock()
            : this( DefaultTimeout )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorLock"/> class.
        /// </summary>
        public MonitorLock(int lockTimeout)
        {
            m_uLockDebug = log.IsDebugEnabled;
            m_uLockId = Guid.NewGuid();
            m_uLockObj = new Object();
            m_uLockDepth = 0;
            m_uLockTimeout = lockTimeout;

#if MONITOR_PERFORMANCE_TESTING
            m_uDiagInfo = new ThreadLocal<PerfInfo>(CreateDiagInfo);
#endif
        }

#if MONITOR_PERFORMANCE_TESTING
        /// <summary>
        /// Creates the diag info.
        /// </summary>
        /// <returns></returns>
        private static PerfInfo CreateDiagInfo()
        {
            return new PerfInfo();
        }
#endif

        /// <summary>
        /// Gets a value indicating whether this instance is held by current thread.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is held by current thread; otherwise, <c>false</c>.
        /// </value>
        public bool IsHeldByCurrentThread
        {
            get { return m_uLockOwner == Thread.CurrentThread; }
        }

        /// <summary>
        /// Acquires a lock against this instance.
        /// </summary>
        public IDisposable Acquire()
        {
            return new DisposableLock(this);
        }

        /// <summary>
        /// Internally acquires the lock.
        /// </summary>
        private void InternalAcquire()
        {
            bool isDebugEnabled = m_uLockDebug;
            if (isDebugEnabled)
            {
                log.Debug("InternalAcquire [attempt] - " + m_uLockId);
            }

            Thread lThread = Thread.CurrentThread;

            if (m_uLockOwner == lThread)
            {
                // This condition is only true when the lock request
                // is nested.  The first time in, m_uLockOwner is 0
                // because it is not owned and forces the caller to acquire
                // the spinlock to set the value; the nested call is true,
                // but only because its within an already locked scope.
                m_uLockDepth++;
            }
            else
            {
#if MONITOR_PERFORMANCE_TESTING
                PerfInfo uDiagInfo = m_uDiagInfo.GetOrCreate();
                uDiagInfo.m_uDiagTimeAtStart = Environment.TickCount;
#endif
                if (Monitor.TryEnter(m_uLockObj, m_uLockTimeout))
                {
#if MONITOR_PERFORMANCE_TESTING
                    uDiagInfo.m_uDiagTimeAtAcquire = Environment.TickCount;
#endif
                    m_uLockOwner = lThread;
                    m_uLockDepth = 1;
                }
                else
                {
                    throw new ApplicationException("Unable to obtain lock before timeout occurred");
                }
            }

            if ( isDebugEnabled )
            {
                log.Debug("InternalAcquire - " + m_uLockId + "; depth=" + m_uLockDepth);
            }
        }

        /// <summary>
        /// Internally releases the lock.
        /// </summary>
        private void InternalRelease()
        {
            // Only called when you hold the lock
            --m_uLockDepth;

            bool isDebugEnabled = m_uLockDebug;
            if (isDebugEnabled)
            {
                log.Debug("InternalRelease - " + m_uLockId + "; depth=" + m_uLockDepth);
            }

            if (m_uLockDepth == 0)
            {
                m_uLockOwner = null;
                Monitor.Exit(m_uLockObj);

#if MONITOR_PERFORMANCE_TESTING
                PerfInfo uDiagInfo = m_uDiagInfo.GetOrCreate();
                uDiagInfo.m_uDiagTimeAtRelease = Environment.TickCount;

                if ( uDiagInfo.m_uDiagTimeAtRelease != uDiagInfo.m_uDiagTimeAtAcquire )
                {
                    log.Info(String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                                           m_uLockId,
                                           uDiagInfo.m_uDiagTimeAtStart,
                                           uDiagInfo.m_uDiagTimeAtAcquire,
                                           uDiagInfo.m_uDiagTimeAtAcquire - uDiagInfo.m_uDiagTimeAtStart,
                                           uDiagInfo.m_uDiagTimeAtRelease,
                                           uDiagInfo.m_uDiagTimeAtRelease - uDiagInfo.m_uDiagTimeAtAcquire));
                    if (( uDiagInfo.m_uDiagTimeAtRelease - uDiagInfo.m_uDiagTimeAtAcquire ) > 100 )
                    {
                        log.Info("Excessive amount of time: " + new System.Diagnostics.StackTrace());
                    }
                }
#endif

                if (isDebugEnabled)
                {
                    log.Debug("InternalRelease - MonitorExit - " + m_uLockId);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return "MonitorLock{" +
                   "m_uLockId=" + m_uLockId +
                   ";m_uLockOwner=" + m_uLockOwner +
                   "}";
        }

        /// <summary>
        /// A disposable object that is allocated and acquires the
        /// lock and automatically releases the lock when it is
        /// disposed.
        /// </summary>

        internal class DisposableLock : IDisposable
        {
            private MonitorLock m_lockObj;
            private bool m_lockAcquired;

            /// <summary>
            /// Initializes a new instance of the <see cref="DisposableLock"/> class.
            /// </summary>
            /// <param name="lockObj">The lock obj.</param>
            internal DisposableLock(MonitorLock lockObj)
            {
                this.m_lockAcquired = false;
                this.m_lockObj = lockObj;
                lockObj.InternalAcquire();
                this.m_lockAcquired = true;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                lock (this)
                {
                    if (this.m_lockAcquired && (this.m_lockObj != null))
                    {
                        this.m_lockObj.InternalRelease();
                        this.m_lockObj = null;
                        this.m_lockAcquired = false;
                    }
                }
            }
        }

#if MONITOR_PERFORMANCE_TESTING
        internal class PerfInfo
        {
            public long m_uDiagTimeAtStart;     // time when lock acquisition begins
            public long m_uDiagTimeAtAcquire;   // time when lock acquired
            public long m_uDiagTimeAtRelease;   // time when lock released
        }
#endif

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
