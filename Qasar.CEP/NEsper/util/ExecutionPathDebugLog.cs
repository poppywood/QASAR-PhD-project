namespace net.esper.util
{
    /// <summary>
    /// Utility class that control debug-level logging in the execution path
    /// beyond which is controlled by Log4net.
    /// </summary>
    
    public class ExecutionPathDebugLog
    {
        private static bool isDebugEnabled = false;

        /// <summary>
        /// Gets or sets a flag that allows execution path debug logging.
        /// </summary>
        public static bool IsEnabled
        {
            get { return isDebugEnabled; }
            set { isDebugEnabled = value; }
        }
    }
}