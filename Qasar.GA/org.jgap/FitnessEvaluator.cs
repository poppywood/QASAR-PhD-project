
using System;
namespace org.jgap
{
	
	public interface FitnessEvaluator
		{
		/// <summary> Compares the first given fitness value with the second and returns true
		/// if the first one is greater than the second one. Otherwise returns false
		/// </summary>
		/// <param name="a_fitness_value1">first fitness value
		/// </param>
		/// <param name="a_fitness_value2">second fitness value
		/// </param>
		/// <returns> true: first fitness value greater than second
		/// @since 1.1
		/// </returns>
		bool isFitter(int a_fitness_value1, int a_fitness_value2);
        bool isSmallerFitter(int a_fitness_value1, int a_fitness_value2);

		}
}