
using System;
namespace org.jgap
{
	
	
	[Serializable]
	public abstract class FitnessFunction
	{
		/// <summary> Retrieves the fitness value of the given Chromosome. The fitness
		/// value will be a positive integer.
		/// 
		/// </summary>
		/// <param name="a_subject">the Chromosome for which to compute and return the
		/// fitness value.
		/// </param>
		/// <returns> the fitness value of the given Chromosome.
		/// </returns>
		public int getFitnessValue(Chromosome a_subject, Configuration config)
		{
			// Delegate to the evaluate() method to actually compute the
			// fitness value. If the returned value is less than one,
			// then we throw a runtime exception.
			// ---------------------------------------------------------
			int fitnessValue = evaluate(a_subject, config);
			
			if (fitnessValue < 1)
			{
				throw new System.SystemException("Fitness values must be positive! Received value: " + fitnessValue);
			}
			
			return fitnessValue;
		}


        protected internal abstract int evaluate(Chromosome a_subject, Configuration config);
	}
}