using System;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Gets a value from the underlying
    /// </summary>
    /// <param name="underlying"></param>
    /// <returns></returns>

    public delegate Object ValueGetter(Object underlying);
}
