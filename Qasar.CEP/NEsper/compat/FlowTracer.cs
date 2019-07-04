using System;

using log4net;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Can be used to trace flow through a process.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FlowTracer<T> : IDisposable
    {
        [ThreadStatic]
        private static string indent;

        private readonly string m_id;
        private readonly string m_saved;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowTracer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public FlowTracer(String id)
        {
            if (String.IsNullOrEmpty(indent))
            {
                indent = "";
            }

            this.m_id = id;
            this.m_saved = indent;

            indent = indent + '>';

            if (log.IsDebugEnabled)
            {
                log.Debug(indent + "Enter > " + m_id);
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(indent + "Leave > " + m_id);
            }

            indent = m_saved;
        }

        #endregion

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
