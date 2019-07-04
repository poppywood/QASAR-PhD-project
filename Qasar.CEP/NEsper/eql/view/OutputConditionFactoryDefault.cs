///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using net.esper.core;
using net.esper.eql.spec;

using org.apache.commons.logging;

namespace net.esper.eql.view
{
    /// <summary>Factory for output condition instances.</summary>
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
            else if (outputLimitSpec.IsDisplayFirstOnly)
            {
                log.Debug(".createCondition creating OutputConditionFirst");
                return new OutputConditionFirst(outputLimitSpec, statementContext, outputCallback);
            }
            if (outputLimitSpec.IsEventLimit)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(".createCondition creating OutputConditionCount with event rate " + outputLimitSpec.EventRate);
                }
                return new OutputConditionCount(outputLimitSpec.EventRate, outputCallback);
            }
            else
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug(".createCondition creating OutputConditionTime with interval length " + outputLimitSpec.TimeRate);
                }
                return new OutputConditionTime(outputLimitSpec.TimeRate, statementContext, outputCallback);
            }
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
