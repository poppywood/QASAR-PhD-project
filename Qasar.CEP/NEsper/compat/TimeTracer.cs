using System;
using System.Diagnostics;

using log4net;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Times flow and execution time for a scope.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TimeTracer<T> : IDisposable
    {
        private readonly string m_id;
        private readonly Stopwatch m_stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTracer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public TimeTracer( String id )
        {
            this.m_id = id;
            this.m_stopwatch = new Stopwatch();
            this.m_stopwatch.Start();
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(m_id + " - elapsed time " + m_stopwatch.Elapsed);
            }

            this.m_stopwatch.Stop();
        }

        #endregion

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
