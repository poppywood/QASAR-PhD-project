using System;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Collection of utility methods to help with enumerated types.
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// Parses the specified text value and converts it into the specified
        /// type of enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="textValue">The text value.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static T Parse<T>( String textValue, bool ignoreCase )
        {
            return (T) Enum.Parse(typeof (T), textValue, ignoreCase);
        }

        /// <summary>
        /// Parses the specified text value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="textValue">The text value.</param>
        /// <returns></returns>
        public static T Parse<T>( String textValue )
        {
            return Parse<T>(textValue, true);
        }
    }
}
