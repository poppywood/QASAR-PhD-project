using System;

using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
    /// <summary>
    /// Spec for building an EventBatch.
    /// </summary>

    public class OutputLimitSpec : MetaDefItem
    {
        private readonly OutputLimitLimitType displayLimit;
        private readonly OutputLimitRateType rateType;
        private readonly double? rate;
        private readonly String variableName;

        /// <summary>Ctor.For batching events by event count.</summary>
        /// <param name="rate">is the fixed output rate, or null if by variable</param>
        /// <param name="displayLimit">indicates whether to output only the first, only the last, or all events</param>
        /// <param name="variableForRate">an optional variable name instead of the rate</param>
        /// <param name="rateType">type of the rate</param>
        public OutputLimitSpec(double? rate, String variableForRate, OutputLimitRateType rateType, OutputLimitLimitType displayLimit)
        {
            this.rate = rate;
            this.displayLimit = displayLimit;
            this.variableName = variableForRate;
            this.rateType = rateType;
        }

        /// <summary>Returns the type of output limit.</summary>
        /// <returns>limit</returns>
        public OutputLimitLimitType DisplayLimit
        {
            get { return displayLimit; }
        }

        /// <summary>Returns the type of rate.</summary>
        /// <returns>rate type</returns>
        public OutputLimitRateType RateType
        {
            get { return rateType; }
        }

        /// <summary>Returns the rate, or null or -1 if a variable is used instead</summary>
        /// <returns>rate if set</returns>
        public double? Rate
        {
            get { return rate; }
        }

        /// <summary>Returns the variable name if set, or null if a fixed rate</summary>
        /// <returns>variable name</returns>
        public String VariableName
        {
            get { return variableName; }
        }
    }
}