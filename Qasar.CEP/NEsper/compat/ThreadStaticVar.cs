using System;

namespace net.esper.compat
{
    public class ThreadStaticVar<T> : IDisposable
    {
        [ThreadStatic] private static T value;

        private readonly T m_savedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadStaticVar&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="tempValue">The temp value.</param>
        public ThreadStaticVar( T tempValue )
        {
            m_savedValue = value;
            value = tempValue;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            value = m_savedValue;
        }
    }
}
