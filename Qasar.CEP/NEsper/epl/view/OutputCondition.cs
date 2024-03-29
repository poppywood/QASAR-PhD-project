using System;
namespace com.espertech.esper.epl.view
{
	/// <summary> A condition that must be satisfied before output processing
	/// is allowed to continue. Once the condition is satisfied, it
	/// makes a callback to continue output processing.
	/// </summary>
	public interface OutputCondition
	{
		/// <summary> Update the output condition.</summary>
		/// <param name="newEventsCount">number of new events incoming
		/// </param>
		/// <param name="oldEventsCount">number of old events incoming
		/// </param>
		void UpdateOutputCondition(int newEventsCount, int oldEventsCount);
	}
}