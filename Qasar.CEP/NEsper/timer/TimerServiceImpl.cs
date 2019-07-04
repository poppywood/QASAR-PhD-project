using System;
using System.Threading;

using com.espertech.esper.compat;

using log4net;

namespace com.espertech.esper.timer
{
    /// <summary>
    /// Implementation of the internal clocking service interface.
    /// </summary>

    public class TimerServiceImpl : TimerService
    {
        private readonly long msecTimerResolution;
        private ITimer timer ;
        private TimerCallback timerCallback;
        private EPLTimerTask timerTask;
        private bool timerTaskCancelled;

        /// <summary>
        /// Set the callback method to invoke for clock ticks.
        /// </summary>
        /// <value></value>

        public TimerCallback Callback
        {
            set { this.timerCallback = value; }
        }

        /// <summary>
        /// Gets the msec timer resolution.
        /// </summary>
        /// <value>The msec timer resolution.</value>
        public long MsecTimerResolution
        {
            get { return msecTimerResolution; }
        }

        /// <summary>
        /// Returns a flag indicating whether statistics are enabled.
        /// </summary>

        public bool AreStatsEnabled
        {
            get { return timerTask._enableStats; }
            set { timerTask._enableStats = value; }
        }

        public long MaxDrift
        {
            get { return timerTask._maxDrift; }
        }

        public long LastDrift
        {
            get { return timerTask._lastDrift; }
        }

        public long TotalDrift
        {
            get { return timerTask._totalDrift; }
        }

        ///<summary>
        /// Gets the number of times the timer has been invoked.
        ///</summary>
        public long InvocationCount
        {
            get { return timerTask._invocationCount; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="msecTimerResolution">the millisecond resolution or interval the internal timer thread</param>
        public TimerServiceImpl(long msecTimerResolution)
        {
            this.msecTimerResolution = msecTimerResolution;
            this.timerTaskCancelled = false;
        }

        /// <summary>
        /// Handles the timer event
        /// </summary>
        /// <param name="state">The user state object.</param>

        private void OnTimerElapsed(Object state)
        {
            if (! timerTaskCancelled)
            {
                if (timerCallback != null)
                {
                    timerCallback();
                }
            }
        }

        /// <summary>
        /// Start clock expecting callbacks at regular intervals and a fixed rate.
        /// Catch-up callbacks are possible should the callback fall behind.
        /// </summary>
        public void StartInternalClock()
        {
            if (timer != null)
            {
                log.Warn(".StartInternalClock Internal clock is already started, stop first before starting, operation not completed");
                return;
            }

            if (log.IsDebugEnabled)
            {
                log.Debug(".StartInternalClock Starting internal clock daemon thread, resolution=" + msecTimerResolution);
            }

            if (timerCallback == null)
            {
                throw new IllegalStateException("Timer callback not set");
            }

            timerTask = new EPLTimerTask(timerCallback);

            timerTaskCancelled = false;
            timer = TimerFactory.DefaultTimerFactory.CreateTimer(
                OnTimerElapsed, msecTimerResolution);
        }

        /// <summary>
        /// Stop internal clock.
        /// </summary>
        /// <param name="warnIfNotStarted">use true to indicate whether to warn if the clock is not Started, use false to not warn
        /// and expect the clock to be not Started.</param>
        public void StopInternalClock(bool warnIfNotStarted)
        {
            if (timer == null)
            {
                if (warnIfNotStarted)
                {
                    log.Warn(".StopInternalClock Internal clock is already Stopped, Start first before Stopping, operation not completed");
                }
                return;
            }

            if (log.IsDebugEnabled)
            {
                log.Debug(".StopInternalClock Stopping internal clock daemon thread");
            }

            timerTaskCancelled = true;
            timer.Dispose();

            try
            {
                // Sleep for at least 100 ms to await the internal timer
                Thread.Sleep(100);
            }
            catch (System.Threading.ThreadInterruptedException)
            {
            }

            timer = null;
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}