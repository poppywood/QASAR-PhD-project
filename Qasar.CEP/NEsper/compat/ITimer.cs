using System;
using System.Collections.Generic;
using System.Text;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// An object that represents a timer.  Timers must be
    /// disposable.
    /// </summary>

    public interface ITimer : IDisposable
    {
    }
}
