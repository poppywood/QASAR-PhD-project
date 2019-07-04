
using System;
namespace org.jgap
{

	
	public class DefaultFitnessEvaluator : FitnessEvaluator
	{
		
		/// <summary>String containing the CVS revision. Read out via reflection!</summary>
		private const System.String CVS_REVISION = "$Revision: 1.1 $";
		
		public DefaultFitnessEvaluator()
		{
		}
		
		/// <summary> Compares the first given fitness value with the second and returns true
		/// if the first one is greater than the second one. Otherwise returns false
		/// </summary>
		/// <param name="a_fitness_value1">first fitness value
		/// </param>
		/// <param name="a_fitness_value2">second fitness value
		/// </param>
		/// <returns> true: first fitness value greater than second
		/// 
		/// @since 1.1
		/// </returns>
		public virtual bool isFitter(int a_fitness_value1, int a_fitness_value2)
		{
			return a_fitness_value1 > a_fitness_value2;
		}

        public virtual bool isSmallerFitter(int a_fitness_value1, int a_fitness_value2)
        {
            return a_fitness_value1 < a_fitness_value2;
    
	    }
        }
}