using System;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Constants we keep for our locking algorithms.
    /// </summary>

	public class LockConstants
	{
        /// <summary>
        /// Number of milliseconds until monitor locks timeout
        /// </summary>
        public const int MonitorTimeout = 50000000;
        /// <summary>
        /// Number of milliseconds until read locks timeout
        /// </summary>
		public const int ReaderTimeout = 50000000 ;
        /// <summary>
        /// Number of milliseconds until write locks timeout
        /// </summary>
		public const int WriterTimeout = 50000000 ;
	}
}
