///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.core;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.view
{
    /// <summary>
    /// Factory for output condition instances.
    /// </summary>
    public class OutputConditionFactoryDefault : OutputConditionFactory
    {
        /// <summary>Creates an output condition instance.</summary>
        /// <param name="outputLimitSpec">specifies what kind of condition to create</param>
        /// <param name="statementContext">
        /// supplies the services required such as for scheduling callbacks
        /// </param>
        /// <param name="outputCallback">is the method to invoke for output</param>
        /// <returns>instance for performing output</returns>
        public OutputCondition CreateCondition(OutputLimitSpec outputLimitSpec,
                                               StatementContext statementContext,
                                               OutputCallback outputCallback)
        {
            if (outputCallback == null)
            {
                throw new ArgumentException("Output condition by count requires a non-null callback");
            }

            if (outputLimitSpec == null)
            {
                return new OutputConditionNull(outputCallback);
            }


            // Check if a variable is present
            VariableReader reader = null;
            if (outputLimitSpec.VariableName != null)
            {
                reader = statementContext.VariableService.GetReader(outputLimitSpec.VariableName);
                if (reader == null)
                {
                    throw new ArgumentException("Variable named '" + outputLimitSpec.VariableName + "' has not been declared");
                }
            }

            if (outputLimitSpec.DisplayLimit == OutputLimitLimitType.FIRST)
            {
                log.Debug(".createCondition creating OutputConditionFirst");
                return new OutputConditionFirst(outputLimitSpec, statementContext, outputCallback);
            }

            if (outputLimitSpec.RateType == OutputLimitRateType.EVENTS)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(".createCondition creating OutputConditionCount with event rate " + outputLimitSpec);
                }

                if ((reader != null) && (!TypeHelper.IsIntegral(reader.VariableType)))
                {
                    throw new ArgumentException("Variable named '" + outputLimitSpec.VariableName + "' must be type integer, long or short");
                }

                int rate = -1;
                if (outputLimitSpec.Rate != null)
                {
                    rate = (int) outputLimitSpec.Rate.Value;
                }
                return new OutputConditionCount(rate, reader, outputCallback);
            }
            else
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(".createCondition creating OutputConditionTime with interval length " + outputLimitSpec.Rate);
                }
                if ((reader != null) && (!TypeHelper.IsNumeric(reader.VariableType)))
                {
                    throw new ArgumentException("Variable named '" + outputLimitSpec.VariableName + "' must be of numeric type");
                }

                bool isMinutesUnit = outputLimitSpec.RateType == OutputLimitRateType.TIME_MIN;
                return new OutputConditionTime(outputLimitSpec.Rate.Value, isMinutesUnit, reader, statementContext, outputCallback);
            }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
