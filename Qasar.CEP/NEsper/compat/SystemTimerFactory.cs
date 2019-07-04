using System;
using System.Collections.Generic;
using System.Threading;

using log4net;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Implementation of the timer factory that uses the system timer.
    /// </summary>

    public class SystemTimerFactory : ITimerFactory
    {
        private readonly LinkedList<InternalTimer> timerCallbackList = new LinkedList<InternalTimer>();
        private readonly Object harmonicLock = new Object();
        private ITimer harmonic;
        private long currGeneration; // current generation
        private long lastGeneration; // last generation that produced an event

        /// <summary>
        /// Disposable timer kept for internal purposes; cascades the timer effect.
        /// </summary>

        internal class InternalTimer : ITimer
        {
            internal TimerCallback timerCallback;
            internal long currIter;

            /// <summary>
            /// Initializes a new instance of the <see cref="InternalTimer"/> class.
            /// </summary>
            internal InternalTimer()
            {
                currIter = -1;
            }

            /// <summary>
            /// Called when [timer callback].
            /// </summary>
            /// <param name="currTime">The curr time.</param>
            internal void OnTimerCallback( long currTime )
            {
                long refCount = Interlocked.Increment(ref currIter);
                if (( refCount % 10 ) == 0 )
                {
                    if (timerCallback != null)
                    {
                        timerCallback.Invoke(null);
                    }
                }
            }

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Interlocked.Exchange(ref timerCallback, null);
            }

            #endregion
        }

        /// <summary>
        /// Creates the timer.
        /// </summary>
        private void CreateBaseTimer()
        {
            lock (harmonicLock)
            {
                if (harmonic == null)
                {
                    //harmonic = new Timer(OnTimerEvent, null, 0, 5);
#if USE_HARMONIC_TIMER
                    harmonic = new HarmonicTimer(OnTimerEvent);
#else
                    harmonic = new HighResolutionTimer(OnTimerEvent, null, 0, 10);
#endif
                }
            }
        }

        /// <summary>
        /// Determines whether the specified generations are idling.
        /// </summary>
        /// <param name="idleGenerations">The idle generations.</param>
        /// <returns>
        /// 	<c>true</c> if the specified idle generations is idling; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsIdling(long idleGenerations)
        {
            return idleGenerations == 1000;
        }

        /// <summary>
        /// Determines whether [is prune generation] [the specified generation].
        /// </summary>
        /// <param name="generation">The generation.</param>
        /// <returns>
        /// 	<c>true</c> if [is prune generation] [the specified generation]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsPruneGeneration(long generation)
        {
            return generation%200 == 199;
        }

        /// <summary>
        /// Prunes dead callbacks.
        /// </summary>
        private void OnPruneCallbacks()
        {
            LinkedList<LinkedListNode<InternalTimer>> deadList = new LinkedList<LinkedListNode<InternalTimer>>();
            LinkedListNode<InternalTimer> node = timerCallbackList.First;
            while (node != null)
            {
                InternalTimer _timer = node.Value;
                TimerCallback _timerCallback = _timer.timerCallback;

                if (_timerCallback == null)
                {
                    deadList.AddLast(node);
                }

                node = node.Next;
            }

            // Prune dead nodes
            foreach (LinkedListNode<InternalTimer> pnode in deadList)
            {
                timerCallbackList.Remove(pnode);
            }      
        }

        /// <summary>
        /// Occurs when the timer event fires.
        /// </summary>
        /// <param name="userData">The user data.</param>
        private void OnTimerEvent(Object userData)
        {
            long curr = Interlocked.Increment(ref currGeneration);

            LinkedListNode<InternalTimer> node = timerCallbackList.First;
            if (node != null)
            {
                Interlocked.Exchange(ref lastGeneration, curr);

                while (node != null)
                {
                    node.Value.OnTimerCallback(0);
                    //node.Value.OnTimerCallback(curr);
                    node = node.Next;
                }

                // Callbacks that are no longer in use are occassionally purged
                // from the list.  This is known as the prune cycle and occurs at
                // an interval determined by the IsPruneGeneration() method.
                if (IsPruneGeneration(curr))
                {
                    OnPruneCallbacks();
                }
            }
            else
            {
                long last = Interlocked.Read(ref lastGeneration);

                // If the timer is running and doing nothing, we consider the
                // timer to be idling.  Idling only occurs when there are no callbacks,
                // so if the system is idling, we can actively shutdown the thread
                // timer.
                if (IsIdling(curr - last))
                {
                    lock (harmonicLock)
                    {
                        if ( harmonic != null )
                        {
                            harmonic.Dispose();
                            harmonic = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a timer.  The timer will begin after dueTime (in milliseconds)
        /// has passed and will occur at an interval specified by the period.
        /// </summary>
        /// <param name="timerCallback"></param>
        /// <param name="period"></param>
        /// <returns></returns>

        public ITimer CreateTimer(
            TimerCallback timerCallback,
            long period)
        {
            CreateBaseTimer();

            // Create a disposable timer that can be given back to the
            // caller.  The item is also used to track the lifetime of the
            // callback.
            InternalTimer internalTimer = new InternalTimer();
            internalTimer.timerCallback = timerCallback;
            timerCallbackList.AddLast(internalTimer);

            return internalTimer;
        }

#if USE_HARMONIC_TIMER
        /// <summary>
        /// Thread-based timer that used a harmonic algorithm to ensure that
        /// the thread clicks at a regular interval.
        /// </summary>

        private class HarmonicTimer : ITimer
        {
            private const int INTERNAL_CLOCK_SLIP = 100000;

            private readonly Guid m_id;
            private Thread m_thread;
            private readonly TimerCallback m_timerCallback;
            private readonly long m_tickAlign;
            private readonly long m_tickPeriod;
            private bool m_alive;

            /// <summary>
            /// Starts thread processing.
            /// </summary>

            private void Start()
            {
                log.Debug(".Run - timer thread starting");

                long lTickAlign = m_tickAlign;
                long lTickPeriod = m_tickPeriod;

                try
                {
                    Thread.BeginThreadAffinity();

                    while (m_alive)
                    {
                        // Check the tickAlign to determine if we are here "too early"
                        // The CLR is a little sloppy in the way that thread timers are handled.
                        // In Java, when a timer is setup, the timer will adjust the interval
                        // up and down to match the interval set by the requestor.  As a result,
                        // you will can easily see intervals between calls that look like 109ms,
                        // 94ms, 109ms, 94ms.  This is how the JVM ensures that the caller gets
                        // an average of 100ms.  The CLR however will provide you with 109ms,
                        // 109ms, 109ms, 109ms.  Eventually this leads to slip in the timer.
                        // To account for that we under allocate the timer interval by some
                        // small amount and allow the thread to sleep a wee-bit if the timer
                        // is too early to the next clock cycle.

                        long currTickCount = DateTime.Now.Ticks;
                        long currDelta = lTickAlign - currTickCount;

                        //Console.WriteLine("Curr: {0} {1} {2}", currDelta, currTickCount, Environment.TickCount);

                        while(currDelta > INTERNAL_CLOCK_SLIP)
                        {
                            if (currDelta >= 600000)
                                Thread.Sleep((int) 1); // force-yield quanta
                            else
                                Thread.SpinWait(20);

                            currTickCount = DateTime.Now.Ticks;
                            currDelta = lTickAlign - currTickCount;

                            //Console.WriteLine("Curr: {0} {1} {2}", currDelta, currTickCount, Environment.TickCount);
                        }

                        lTickAlign += lTickPeriod;
                        m_timerCallback(null);
                    }
                }
                catch (ThreadInterruptedException)
                {
                    Thread.EndThreadAffinity();
                }

                log.Debug( ".Run - timer thread stopping");
            }

            /// <summary>
            /// Creates the timer and wraps it
            /// </summary>
            /// <param name="timerCallback"></param>

            public HarmonicTimer(TimerCallback timerCallback)
            {
                m_id = Guid.NewGuid();
                m_alive = true;

                m_timerCallback = timerCallback;

                m_tickPeriod = 100000;
                m_tickAlign = DateTime.Now.Ticks;

                m_thread = new Thread(Start);
                m_thread.Priority = ThreadPriority.AboveNormal;
                m_thread.IsBackground = true;
                m_thread.Name = "ThreadBasedTimer{" + m_id + "}";
                m_thread.Start();
            }

            /// <summary>
            /// Called when the object is destroyed.
            /// </summary>

            ~HarmonicTimer()
            {
                Dispose();
            }

            /// <summary>
            /// Cleans up system resources
            /// </summary>

            public void Dispose()
            {
                this.m_alive = false;

                if (this.m_thread != null)
                {
                    this.m_thread.Interrupt();
                    this.m_thread = null;
                }
            }

            private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
#endif
    }
}
