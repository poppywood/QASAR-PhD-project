using System;

using com.espertech.esper.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.view
{
	/// <summary> Output condition that is satisfied at the end
	/// of every time interval of a given length.
	/// </summary>

    public sealed class OutputConditionTime : OutputCondition
	{
		/// <summary> Returns the interval size in milliseconds.</summary>
		/// <returns> batch size
		/// </returns>

        public long MsecIntervalSize
		{
			get { return msecIntervalSize; }
		}

		private const bool DO_OUTPUT = true;
		private const bool FORCE_UPDATE = true;

        private long msecIntervalSize;
        private readonly OutputCallback outputCallback;
        private readonly ScheduleSlot scheduleSlot;

        private long? currentReferencePoint;
        private StatementContext context;
        private bool isCallbackScheduled;
        private readonly VariableReader reader;
        private EPStatementHandleCallback handle;
        private bool isMinutesUnit;

        /// <summary>Constructor.</summary>
        /// <param name="intervalSize">is the number of minutes or seconds to batch events for.</param>
        /// <param name="context">is the view context for time scheduling</param>
        /// <param name="outputCallback">is the callback to make once the condition is satisfied</param>
        /// <param name="reader">is for reading the variable value, if a variable was supplied, else null</param>
        /// <param name="isMinutesUnit">is true to indicate the unit is minutes, or false for the unit as seconds</param>
        public OutputConditionTime(double intervalSize,
                                   bool isMinutesUnit,
                                   VariableReader reader,
                                   StatementContext context,
                                   OutputCallback outputCallback)
        {
            if (outputCallback == null)
            {
                throw new ArgumentNullException("outputCallback", "Output condition by count requires a non-null callback");
            }
            if (!isMinutesUnit)
            {
                if ((intervalSize < 0.001) && (reader == null))
                {
                    throw new ArgumentException("Output condition by time requires a millisecond interval size of at least 1 msec or a variable");
                }
            }
            if (context == null)
            {
                throw new ArgumentNullException("context", "OutputConditionTime requires a non-null view context");
            }

            this.reader = reader;
            this.context = context;
            this.outputCallback = outputCallback;
            this.scheduleSlot = context.ScheduleBucket.AllocateSlot();
            this.isMinutesUnit = isMinutesUnit;

            if (reader != null)
            {
                intervalSize = Convert.ToDouble(reader.Value);
            }
            if (isMinutesUnit)
            {
                intervalSize = intervalSize * 60d;
            }
            this.msecIntervalSize = (long) Math.Round(1000 * intervalSize);

        }

        /// <summary>
        /// Update the output condition.
        /// </summary>
        /// <param name="newEventsCount">number of new events incoming</param>
        /// <param name="oldEventsCount">number of old events incoming</param>
		public void UpdateOutputCondition(int newEventsCount, int oldEventsCount)
		{
            if (ExecutionPathDebugLog.isDebugEnabled && log.IsDebugEnabled)
            {
                log.Debug(".updateOutputCondition, " + "  newEventsCount==" + newEventsCount + "  oldEventsCount==" + oldEventsCount);
			}

			if (currentReferencePoint == null)
			{
				currentReferencePoint = context.SchedulingService.Time;
			}

            // If we pull the interval from a variable, then we may need to reschedule
            if (reader != null)
            {
                Object value = reader.Value;
                if (value != null)
                {
                    // Check if the variable changed
                    double intervalSize = Convert.ToDouble(reader.Value);
                    if (isMinutesUnit)
                    {
                        intervalSize = intervalSize * 60d;
                    }

                    long newMsecIntervalSize = (long) Math.Round(1000 * intervalSize);

                    // reschedule if the interval changed
                    if (newMsecIntervalSize != msecIntervalSize)
                    {
                        if (isCallbackScheduled)
                        {
                            // reschedule
                            context.SchedulingService.Remove(handle, scheduleSlot);
                            ScheduleCallback();
                        }
                    }
                }
            }

			// Schedule the next callback if there is none currently scheduled
			if (!isCallbackScheduled)
			{
				ScheduleCallback();
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
			return this.GetType().FullName + " msecIntervalSize=" + msecIntervalSize;
		}

		/// <summary>
		/// Called by the scheduling service after the requested event has
		/// occurred.
		/// </summary>

		private void HandleScheduledCallback(ExtensionServicesContext extensionServicesContext)
		{
            isCallbackScheduled = false;
            outputCallback(OutputConditionTime.DO_OUTPUT, OutputConditionTime.FORCE_UPDATE);
            ScheduleCallback();
		}

		private void ScheduleCallback()
		{
            // If we pull the interval from a variable, get the current interval length
            if (reader != null)
            {
                Object value = reader.Value;
                if (value != null)
                {
                    // Check if the variable changed
                    double intervalSize = Convert.ToDouble(reader.GetValue());
                    if (isMinutesUnit)
                    {
                        intervalSize = intervalSize * 60d;
                    }
                    msecIntervalSize = (long) Math.Round(1000 * intervalSize);
                }
            }

			isCallbackScheduled = true;
			long current = context.SchedulingService.Time;
			long afterMSec = ComputeWaitMSec(current, currentReferencePoint.Value, this.msecIntervalSize);

            if (ExecutionPathDebugLog.isDebugEnabled && log.IsDebugEnabled)
            {
                log.Debug(".scheduleCallback Scheduled new callback for " + " afterMsec=" + afterMSec + " now=" + current + " currentReferencePoint=" + currentReferencePoint + " msecIntervalSize=" + msecIntervalSize);
			}

			ScheduleHandleCallback callback = new ProxyScheduleHandleCallback( HandleScheduledCallback ) ;
            handle = new EPStatementHandleCallback(context.EpStatementHandle, callback);
			context.SchedulingService.Add(afterMSec, handle, scheduleSlot);
		}

		/// <summary>
		/// Given a current time and a reference time and an interval size, compute the amount of
		/// milliseconds till the next interval.
		/// </summary>
		/// <param name="current">is the current time</param>
		/// <param name="reference">is the reference point</param>
		/// <param name="interval">is the interval size</param>
		/// <returns>milliseconds after current time that marks the end of the current interval</returns>

		internal static long ComputeWaitMSec(long current, long reference, long interval)
		{
			// Example:  current c=2300, reference r=1000, interval i=500, solution s=200
			//
			// int n = ((2300 - 1000) / 500) = 2
			// r + (n + 1) * i - c = 200
			//
			// Negative example:  current c=2300, reference r=4200, interval i=500, solution s=400
			// int n = ((2300 - 4200) / 500) = -3
			// r + (n + 1) * i - c = 4200 - 3*500 - 2300 = 400
			//
			long n = (long) ((current - reference) / (interval * 1f));
			if (reference > current)
			// References in the future need to deduct one window
			{
				n = n - 1;
			}
			long solution = reference + (n + 1) * interval - current;

			if (solution == 0)
			{
				return interval;
			}
			return solution;
		}

		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
}
