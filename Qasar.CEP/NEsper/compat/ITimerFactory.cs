using System;
using System.Threading;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Factory object that creates timers.
    /// </summary>

    public interface ITimerFactory
    {
        /// <summary>
        /// Creates a timer.  The timer will begin after dueTime (in milliseconds)
        /// has passed and will occur at an interval specified by the period.
        /// </summary>
        /// <param name="timerCallback"></param>
        /// <param name="period"></param>
        /// <returns></returns>

        ITimer CreateTimer(
            TimerCallback timerCallback,
            long period);
    }
}
