using System;

using com.espertech.esper.epl.spec;
using com.espertech.esper.core;

namespace com.espertech.esper.epl.view
{
    /// <summary> An output condition that is satisfied at the first event
    /// of either a time-based or count-based batch.
    /// </summary>
    public class OutputConditionFirst : OutputCondition
    {
        private readonly OutputCallback outputCallback;
        private readonly OutputCondition innerCondition;
        private bool witnessedFirst;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="outputLimitSpec">specifies what kind of condition to create</param>
        /// <param name="statementContext">supplies the services required such as for scheduling callbacks</param>
        /// <param name="outputCallback">is the method to invoke for output</param>
        public OutputConditionFirst(OutputLimitSpec outputLimitSpec, StatementContext statementContext, OutputCallback outputCallback)
        {
            if (outputCallback == null)
            {
                throw new ArgumentException("Output condition by count requires a non-null callback", "outputCallback");
            }
            this.outputCallback = outputCallback;
            OutputLimitSpec innerSpec = new OutputLimitSpec(outputLimitSpec.Rate, outputLimitSpec.VariableName, outputLimitSpec.RateType, OutputLimitLimitType.DEFAULT);
            OutputCallback localCallback = CreateCallbackToLocal();

            this.innerCondition = statementContext.OutputConditionFactory.CreateCondition(innerSpec, statementContext, localCallback);
            this.witnessedFirst = false;
        }

        /// <summary>
        /// Update the output condition.
        /// </summary>
        /// <param name="newEventsCount">number of new events incoming</param>
        /// <param name="oldEventsCount">number of old events incoming</param>
        public virtual void UpdateOutputCondition(int newEventsCount, int oldEventsCount)
        {
            if (!witnessedFirst)
            {
                witnessedFirst = true;
                bool doOutput = true;
                bool forceUpdate = false;
                outputCallback(doOutput, forceUpdate);
            }
            innerCondition.UpdateOutputCondition(newEventsCount, oldEventsCount);
        }

        private OutputCallback CreateCallbackToLocal()
        {
            return new OutputCallback(
                delegate(bool doOutput, bool forceUpdate)
                {
                    this.ContinueOutputProcessing(forceUpdate);
                });
        }

        private void ContinueOutputProcessing(bool forceUpdate)
        {
            bool doOutput = !witnessedFirst;
            outputCallback(doOutput, forceUpdate);
            witnessedFirst = false;
        }
    }
}
