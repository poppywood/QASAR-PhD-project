using System;
namespace com.espertech.esper.filter
{
	
	/// <summary> Static factory for implementations of the <see cref="FilterService"/> interface.</summary>
	public sealed class FilterServiceProvider
	{
		/// <summary> Creates an implementation of the FilterEvaluationService interface.</summary>
		/// <returns> implementation
		/// </returns>
		public static FilterService NewService()
		{
			return new FilterServiceImpl();
		}
	}
}