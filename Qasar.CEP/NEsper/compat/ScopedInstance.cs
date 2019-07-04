using System;
using System.Collections.Generic;
using System.Text;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Provides a generic item that can be scoped statically as a singleton; avoids the
    /// need to define a threadstatic variable.  Also provides a consistent model for
    /// providing this service.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class ScopedInstance<T>
    {
        [ThreadStatic]
        private static T instance;

        /// <summary>
        /// Gets the current instance value.
        /// </summary>
        /// <value>The current.</value>
        public static T Current
        {
            get { return instance; }
        }

        /// <summary>
        /// Sets the specified instance.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IDisposable Set(T item)
        {
            return new DisposableScope(item);
        }

        /// <summary>
        /// Disposable scope
        /// </summary>
        private class DisposableScope : IDisposable
        {
            private readonly T previous;

            /// <summary>
            /// Initializes a new instance of the <see cref="ScopedInstance&lt;T&gt;.DisposableScope"/> class.
            /// </summary>
            /// <param name="item">The item.</param>
            internal DisposableScope( T item )
            {
                previous = instance;
                instance = item;
            }

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                instance = previous;
            }

            #endregion
        }
    }
}
