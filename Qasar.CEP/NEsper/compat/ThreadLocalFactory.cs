namespace com.espertech.esper.compat
{
    /// <summary>
    /// Creator and manufacturer of thread local objects.
    /// </summary>
    public interface ThreadLocalFactory
    {
        /// <summary>
        /// Create a thread local object of the specified type param.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        ThreadLocal<T> CreateThreadLocal<T>(FactoryDelegate<T> factory)
            where T : class;
    }
}
