using System;
using System.IO;

using com.espertech.esper.util;

namespace com.espertech.esper.type
{
    /// <summary>
    /// Parameter for views that accept time period information such as "5 sec 100 msec".
    /// </summary>

    [Serializable]
    public class TimePeriodParameter : MetaDefItem, EQLParameterType
    {
        /// <summary> Returns the number of seconds.</summary>
        /// <returns> the number of seconds specified by time period string
        /// </returns>
        virtual public double NumSeconds
        {
            get { return numSeconds; }
        }

        private readonly double numSeconds;

        /// <summary> Ctor.</summary>
        /// <param name="numSeconds">is the number of seconds represented by time period string
        /// </param>
        public TimePeriodParameter(double numSeconds)
        {
            this.numSeconds = numSeconds;
        }

        /// <summary>
        /// Returns true if the objects are equal.
        /// </summary>
        /// <param name="_object">Object to compare against.</param>
        /// <returns></returns>
        public override bool Equals(Object _object)
        {
            TimePeriodParameter other = _object as TimePeriodParameter;
            return
                (other != null) ?
                (other.numSeconds == this.numSeconds) :
                (false);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return numSeconds.GetHashCode();
        }
        
        public void ToEPL(StringWriter writer)
	    {
        	writer.Write(numSeconds.ToString());
	        writer.Write(" sec");
	    }
    }
}
