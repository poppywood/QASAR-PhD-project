using System;

namespace com.espertech.esper.timer
{
    /// <summary>
    /// Invoked by the internal clocking service at regular intervals.
    /// </summary>

    public delegate void TimerCallback();
}