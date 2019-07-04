using System;
using System.Collections.Generic;
using System.Threading;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Implementation of the TimerFactory that uses the HighResolutionTimer.
    /// </summary>

    public class HighResolutionTimerFactory : ITimerFactory
    {
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
            return new HighResolutionTimer(
                timerCallback,
                null,
                0,
                period);
        }
    }
}
