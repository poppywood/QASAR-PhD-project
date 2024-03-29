using System;
namespace com.espertech.esper.emit
{
	
	/// <summary> Static factory for implementations of the EmitService interface.</summary>
	public sealed class EmitServiceProvider
	{
		/// <summary> Creates an implementation of the EmitService interface.</summary>
		/// <returns> implementation
		/// </returns>
		public static EmitService NewService()
		{
			return new EmitServiceImpl();
		}
	}
}