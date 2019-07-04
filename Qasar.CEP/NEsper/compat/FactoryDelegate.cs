namespace com.espertech.esper.compat
{
    /// <summary>
    /// Factory pattern delegate that creates an object of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public delegate T FactoryDelegate<T>();
}
