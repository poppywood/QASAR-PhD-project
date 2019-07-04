using System;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Base class for disposable lock pattern.
    /// </summary>
    abstract public class BaseLock : IDisposable
    {
        /// <summary>Acquire text.</summary>
        internal const string ACQUIRE_TEXT = "Acquire ";

        /// <summary>Acquired text.</summary>
        internal const string ACQUIRED_TEXT = "Got     ";

        /// <summary>Release text.</summary>
        internal const string RELEASE_TEXT = "Release ";

        /// <summary>Released text.</summary>
        internal const string RELEASED_TEXT = "Freed   ";

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();
    }
}
