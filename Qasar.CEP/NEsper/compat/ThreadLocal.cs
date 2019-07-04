using System;
using System.Collections.Generic;
using System.Threading;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// ThreadLocal provides the engine with a way to store information that
    /// is local to the instance and a the thread.  While the CLR provides the
    /// ThreadStatic attribute, it can only be applied to static variables;
    /// some usage patterns in esper (such as statement-specific thread-specific
    /// processing data) require that data be associated by instance and thread.
    /// The CLR provides a solution to this known as LocalDataStoreSlot.  It
    /// has been documented that this method is slower than its ThreadStatic
    /// counterpart, but it allows for instance-based allocation.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public interface ThreadLocal<T> : IDisposable
        where T : class
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        T Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the data or creates it if not found.
        /// </summary>
        /// <returns></returns>
        T GetOrCreate();
    }
}
