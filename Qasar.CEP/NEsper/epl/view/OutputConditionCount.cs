using System;

using com.espertech.esper.epl.variable;
using com.espertech.esper.util;
using log4net;

namespace com.espertech.esper.epl.view
{
	/// <summary>
	/// Output limit condition that is satisfied when either
	/// the total number of new events arrived or the total number
	/// of old events arrived is greater than a preset value.
	/// </summary>

	public sealed class OutputConditionCount : OutputCondition
	{
		private const bool DO_OUTPUT = true;
		private const bool FORCE_UPDATE = false;

        private long eventRate;
        private int newEventsCount;
        private int oldEventsCount;
        private readonly OutputCallback outputCallback;
        private readonly VariableReader variableReader;

		/// <summary> Returns the number of new events.</summary>
		/// <returns> number of new events
		/// </returns>
		public int NewEventsCount
		{
            get { return newEventsCount; }
		}

		/// <summary> Returns the number of old events.</summary>
		/// <returns> number of old events
		/// </returns>
		public int OldEventsCount
		{
            get { return oldEventsCount; }
		}

		/// <summary> Returns the event rate.</summary>
		/// <returns> event rate
		/// </returns>
		public long EventRate
		{
            get { return eventRate; }
		}

		private bool IsSatisfied
		{
            get { return (newEventsCount >= eventRate) || (oldEventsCount >= eventRate); }
		}

        /// <summary>Constructor.</summary>
        /// <param name="eventRate">is the number of old or new events thatmust arrive in order for the condition to be satisfied</param>
        /// <param name="outputCallback">is the callback that is made when the conditoin is satisfied</param>
        /// <param name="variableReader">is for reading the variable value, if a variable was supplied, else null</param>
        public OutputConditionCount(int eventRate, VariableReader variableReader, OutputCallback outputCallback)
        {
            if ((eventRate < 1) && (variableReader == null))
            {
                throw new ArgumentException("Limiting output by event count requires an event count of at least 1 or a variable name");
            }
            if (outputCallback == null)
            {
                throw new ArgumentNullException("outputCallback", "Output condition by count requires a non-null callback");
            }
            this.eventRate = eventRate;
            this.outputCallback = outputCallback;
            this.variableReader = variableReader;
        }

        /// <summary>
        /// Updates the output condition.
        /// </summary>
        /// <param name="newDataCount">The new data count.</param>
        /// <param name="oldDataCount">The old data count.</param>
		public void UpdateOutputCondition(int newDataCount, int oldDataCount)
		{
            if (variableReader != null)
            {
                Object value = variableReader.Value;
                if (value != null)
                {
                    eventRate = Convert.ToInt64(value);
                }
            }

			this.newEventsCount += newDataCount;
			this.oldEventsCount += oldDataCount;

			if (log.IsDebugEnabled)
			{
				log.Debug(".updateBatchCondition, " + "  newEventsCount==" + newEventsCount + "  oldEventsCount==" + oldEventsCount);
			}

			if (IsSatisfied)
			{
                if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
                {
                    log.Debug(".updateOutputCondition() condition satisfied");
                }
                this.newEventsCount = 0;
				this.oldEventsCount = 0;
				outputCallback(DO_OUTPUT, FORCE_UPDATE);
			}
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override String ToString()
		{
			return this.GetType().FullName + " eventRate=" + eventRate;
		}

		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
}
