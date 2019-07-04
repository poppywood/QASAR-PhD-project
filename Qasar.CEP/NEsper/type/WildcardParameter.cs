using System;
using System.IO;

using com.espertech.esper.compat;

namespace com.espertech.esper.type
{	
	/// <summary>
    /// Represents a wildcard as a parameter.
    /// </summary>
	
    [Serializable]
    public class WildcardParameter : NumberSetParameter
	{
        /// <summary>
        /// Returns true if all values between and including min and max are supplied by the parameter.
        /// </summary>
        /// <param name="min">lower end of range</param>
        /// <param name="max">upper end of range</param>
        /// <returns>
        /// true if parameter specifies all int values between min and max, false if not
        /// </returns>
        public Boolean IsWildcard(int min, int max)
        {
            return true;
        }

        /// <summary>
        /// Return a set of int values representing the value of the parameter for the given range.
        /// </summary>
        /// <param name="min">lower end of range</param>
        /// <param name="max">upper end of range</param>
        /// <returns>set of integer</returns>
        public Set<int> GetValuesInRange(int min, int max)
        {
            Set<int> result = new HashSet<Int32>();
            for (int i = min; i <= max; i++)
            {
                result.Add(i);
            }
            return result;
        }
        
        public void ToEPL(StringWriter writer)
	    {
	        writer.Write('*');
	    }
    }
}
