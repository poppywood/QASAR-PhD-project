using System;
using System.Collections.Generic;
using System.IO;

using com.espertech.esper.compat;

namespace com.espertech.esper.type
{
	/// <summary>
    /// Parameter supplying a single int value is a set of numbers.
    /// </summary>

    [Serializable]
    public class IntParameter : NumberSetParameter
	{
		/// <summary> Returns int value.</summary>
		/// <returns> int value
		/// </returns>
		
        virtual public int IntValue
		{
			get { return intValue; }
		}

        private readonly int intValue;
		
		/// <summary> Ctor.</summary>
		/// <param name="intValue">single in value
		/// </param>

        public IntParameter(int intValue)
		{
			this.intValue = intValue;
		}

        /// <summary>
        /// Returns true if all values between and including min and max are supplied by the parameter.
        /// </summary>
        /// <param name="min">lower end of range</param>
        /// <param name="max">upper end of range</param>
        /// <returns>
        /// true if parameter specifies all int values between min and max, false if not
        /// </returns>
		public virtual bool IsWildcard(int min, int max)
		{
			if ((intValue == min) && (intValue == max))
			{
				return true;
			}
			return false;
		}

        /// <summary>
        /// Return a set of int values representing the value of the parameter for the given range.
        /// </summary>
        /// <param name="min">lower end of range</param>
        /// <param name="max">upper end of range</param>
        /// <returns>set of integer</returns>
		public Set<Int32> GetValuesInRange(int min, int max)
		{
			Set<Int32> values = new HashSet<Int32>();
			
			if ((intValue >= min) && (intValue <= max))
			{
                values.Add(intValue);
			}
			
			return values;
		}
		
		public void ToEPL(StringWriter writer)
	    {
			writer.Write(intValue.ToString());
	    }
	}
}
