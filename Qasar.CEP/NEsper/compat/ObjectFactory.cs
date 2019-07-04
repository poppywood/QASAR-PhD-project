using System;
using System.Collections.Generic;
using System.Text;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Parses an object from an input.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    public delegate Object ObjectFactory<T>(T input);
}
