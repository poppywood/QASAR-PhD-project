using System;
using System.Configuration;
using System.Threading;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Creates timers.
    /// </summary>

    public class TimerFactory
    {
        /// <summary>
        /// Gets the default timer factory
        /// </summary>

        public static ITimerFactory DefaultTimerFactory
        {
            get
            {
                lock( factoryLock )
                {
                    if (defaultTimerFactory == null)
                    {
                        // use the system timer factory unless explicitly instructed
                        // to do otherwise.

                        defaultTimerFactory = new SystemTimerFactory();
                    }
                }

                return defaultTimerFactory;
            }
            set
            {
                lock( factoryLock )
                {
                    defaultTimerFactory = value;
                }
            }
        }

        private static ITimerFactory defaultTimerFactory;
        private static readonly Object factoryLock = new Object();
    }
}
