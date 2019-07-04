/*
* Copyright 2001-2003 Neil Rotstan
*
* This file is part of JGAP.
*
* JGAP is free software; you can redistribute it and/or modify
* it under the terms of the GNU Lesser Public License as published by
* the Free Software Foundation; either version 2.1 of the License, or
* (at your option) any later version.
*
* JGAP is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU Lesser Public License for more details.
*
* You should have received a copy of the GNU Lesser Public License
* along with JGAP; if not, write to the Free Software Foundation, Inc.,
* 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/
using System;
using System.Runtime.InteropServices;
namespace org.jgap
{
	
	
	/// <summary> Genotypes are fixed-length populations of chromosomes. As an instance of
	/// a Genotype is evolved, all of its Chromosomes are also evolved. A Genotype
	/// may be constructed normally, whereby an array of Chromosomes must be
	/// provided, or the static randomInitialGenotype() method can be used to
	/// generate a Genotype with a randomized Chromosome population.
	/// 
	/// </summary>
	/// <author>  Neil Rotstan
	/// @since 1.0
	/// </author>
	[Serializable]
	public class Genotype
	{
		/// <summary> Sets the active Configuration object on this Genotype and its
		/// member Chromosomes. This method should be invoked immediately following
		/// deserialization of this Genotype. If an active Configuration has already
		/// been set on this Genotype, then this method will do nothing.
		/// 
		/// </summary>
		/// <param name="a_activeConfiguration">The current active Configuration object
		/// that is to be referenced internally by
		/// this Genotype and its member Chromosome
		/// instances.
		/// 
		/// @throws InvalidConfigurationException if the Configuration object is
		/// null or cannot be locked because it is in an invalid or
		/// incomplete state.
		/// </param>
		virtual public Configuration ActiveConfiguration
		{
			set
			{
				// Only assign the given Configuration object if we don't already
				// have one.
				// --------------------------------------------------------------
				if (m_activeConfiguration == null)
				{
					if (value == null)
					{
						throw new InvalidConfigurationException("The given Configuration object may not be null.");
					}
					else
					{
						// Make sure the Configuration object is locked and cannot be
						// changed.
						// ----------------------------------------------------------
						value.lockSettings();
						
						m_activeConfiguration = value;
						
						// Since this method is invoked following deserialization of
						// this Genotype, the constructor hasn't been invoked. So make
						// sure any other transient fields are initialized properly.
						// -----------------------------------------------------------
						//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
						m_workingPool = new SupportClass.ListCollectionSupport();
						
						// Now set this Configuration on each of the member
						// Chromosome instances.
						// ------------------------------------------------
						for (int i = 0; i < m_chromosomes.Length; i++)
						{
							m_chromosomes[i].ActiveConfiguration = m_activeConfiguration;
						}
					}
				}
			}
			
		}
		/// <summary> Retrieves the array of Chromosomes that make up the population of this
		/// Genotype instance.
		/// 
		/// </summary>
		/// <returns> The population of Chromosomes.
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'getChromosomes'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		virtual public Chromosome[] Chromosomes
		{
			get
			{
				lock (this)
				{
					return m_chromosomes;
				}
			}
			
		}
		/// <summary> Retrieves the Chromosome in the population with the highest fitness
		/// value.
		/// 
		/// </summary>
		/// <returns> The Chromosome with the highest fitness value, or null if
		/// there are no chromosomes in this Genotype.
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'getFittestChromosome'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		virtual public Chromosome FittestChromosome
		{
			get
			{
				lock (this)
				{
					if (m_chromosomes.Length == 0)
					{
						return null;
					}
					
					// Set the best fitness value to that of the first chromosome.
					// Then loop over the rest of the chromosomes and see if any has
					// a better fitness value.
					// The decision whether a fitness value if better than another is
					// delegated to a FitnessEvaluator
					// --------------------------------------------------------------
					Chromosome fittestChromosome = m_chromosomes[0];
					int fittestValue = fittestChromosome.FitnessValue;
					
					for (int i = 1; i < m_chromosomes.Length; i++)
					{
						if (m_fitnessEvaluator.isFitter(m_chromosomes[i].FitnessValue, fittestValue))
						{
							fittestChromosome = m_chromosomes[i];
							fittestValue = fittestChromosome.FitnessValue;
						}
					}
					
					return fittestChromosome;
				}
			}
			
		}
		virtual public FitnessEvaluator FitnessEvaluator
		{
			get
			{
				return m_fitnessEvaluator;
			}
			
		}
		/// <summary> The current active Configuration instance.</summary>
		[NonSerialized()]
		protected internal Configuration m_activeConfiguration;
		
		/// <summary> The array of Chromosomes that makeup the Genotype's population.</summary>
		protected internal Chromosome[] m_chromosomes;
		
		/// <summary> The working pool of Chromosomes, which is where Chromosomes that are
		/// to be candidates for the next natural selection process are deposited.
		/// This list is passed to each of the genetic operators as they are
		/// invoked during each phase of evolution so that they can add the
		/// Chromosomes they operated upon to it, and then it is eventually passed
		/// to the NaturalSelector so that it can choose which Chromosomes will
		/// go on to the next generation and which will be discarded. It is wiped
		/// clean after each cycle of evolution.
		/// </summary>
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		[NonSerialized()]
		protected internal SupportClass.ListCollectionSupport m_workingPool;
		
		/// <summary> The fitness evaluator. See interface class FitnessEvaluator for details
		/// @since 1.1
		/// </summary>
		private FitnessEvaluator m_fitnessEvaluator;
		
		/// <summary> Constructs a new Genotype instance with the given array of
		/// Chromosomes and the given active Configuration instance. Note
		/// that the Configuration object must be in a valid state
		/// when this method is invoked, or a InvalidconfigurationException
		/// will be thrown.
		/// 
		/// </summary>
		/// <param name="a_activeConfiguration:">The current active Configuration object.
		/// </param>
		/// <param name="a_initialChromosomes:">The Chromosome population to be
		/// managed by this Genotype instance.
		/// @throws IllegalArgumentException if either the given Configuration object
		/// or the array of Chromosomes is null, or if any of the Genes
		/// in the array of Chromosomes is null.
		/// @throws InvalidConfigurationException if the given Configuration object
		/// is in an invalid state.
		/// </param>
		public Genotype(Configuration a_activeConfiguration, Chromosome[] a_initialChromosomes):this(a_activeConfiguration, a_initialChromosomes, new DefaultFitnessEvaluator())
		{
		}
		
		/// <summary> Same as constructor without parameter of type FitnessEvaluator.
		/// Additionally a specific fitnessEvaluator can be specified here. See
		/// interface class FitnessEvaluator for details.
		/// </summary>
		/// <param name="a_activeConfiguration">The current active Configuration object.
		/// </param>
		/// <param name="a_initialChromosomes">The Chromosome population to be
		/// managed by this Genotype instance.
		/// </param>
		/// <param name="a_fitnessEvaluator">a specific fitness value evaluator
		/// @throws InvalidConfigurationException
		/// @since 1.1
		/// </param>
		public Genotype(Configuration a_activeConfiguration, Chromosome[] a_initialChromosomes, FitnessEvaluator a_fitnessEvaluator)
		{
			// Sanity checks: Make sure neither the Configuration, the array
			// of Chromosomes, nor any of the Genes inside the array are null.
			// ---------------------------------------------------------------
			if (a_activeConfiguration == null)
			{
				throw new System.ArgumentException("The Configuration instance may not be null.");
			}
			
			if (a_initialChromosomes == null)
			{
				throw new System.ArgumentException("The array of Chromosomes may not be null.");
			}
			
			if (a_fitnessEvaluator == null)
			{
				throw new System.ArgumentException("The fitness evaluator may not be null.");
			}
			for (int i = 0; i < a_initialChromosomes.Length; i++)
			{
				if (a_initialChromosomes[i] == null)
				{
					throw new System.ArgumentException("The Gene instance at index " + i + " of the array of " + "Chromosomes is null. No Gene instance in this array " + "may be null.");
				}
			}
			
			// Lock the settings of the Configuration object so that the cannot
			// be altered.
			// ----------------------------------------------------------------
			a_activeConfiguration.lockSettings();
			
			m_chromosomes = a_initialChromosomes;
			m_activeConfiguration = a_activeConfiguration;
			
			m_fitnessEvaluator = a_fitnessEvaluator;
			
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			m_workingPool = new SupportClass.ListCollectionSupport();
		}
		
		
		/// <summary> Evolve the population of Chromosomes within this Genotype. This will
		/// execute all of the genetic operators added to the present active
		/// Configuration and then invoke the natural selector to choose which
		/// chromosomes will be included in the next generation population. Note
		/// that the population size always remains constant.
		/// </summary>
		public virtual void  evolve()
		{
			lock (this)
			{
				verifyConfigurationAvailable();
				
				// Process all natural selectors applicable before executing the
				// Genetic Operators.
				//JOONEGAP BEGIN
				// -------------------------------------------------------------
				// Add the chromosomes pool to the natural selector.
				// ----------------------------------------------------------------
				foreach (Chromosome currentChromosome in m_chromosomes)
				{					
					m_activeConfiguration.getNaturalSelector().add(m_activeConfiguration, currentChromosome);
				}
				
				// Repopulate the population of chromosomes with those selected
				// by the natural selector.
				// ------------------------------------------------------------
				
				if (m_activeConfiguration.getNaturalSelectors(true).size() > 0)
				{
					m_chromosomes = m_activeConfiguration.getNaturalSelectors(true).get_Renamed(0).select(m_activeConfiguration, m_activeConfiguration.getPopulationSize());
					
					// Clean up the natural selector.
					// ------------------------------
					m_activeConfiguration.getNaturalSelector().empty();
				}
				//JOONEGAP END
				
				// Execute all of the Genetic Operators.
				// -------------------------------------
				SupportClass.ListCollectionSupport geneticOperators = m_activeConfiguration.GeneticOperators;
				System.Collections.IEnumerator operatorIterator = geneticOperators.GetEnumerator();
				
				foreach (GeneticOperator gOperator in geneticOperators)
				{
					gOperator.operate(m_activeConfiguration, m_chromosomes, m_workingPool);
				}
				
				
				// Process all natural selectors applicable after executing the
				// Genetic Operators.
				// -------------------------------------------------------------
				
				// Add the chromosomes in the working pool to the natural selector.
				// ----------------------------------------------------------------
				foreach (Chromosome currentChromosome in m_workingPool)
				{
					m_activeConfiguration.getNaturalSelectors(false).get_Renamed(0).add(m_activeConfiguration, currentChromosome);
				}
				
				// Repopulate the population of chromosomes with those selected
				// by the natural selector.
				// ------------------------------------------------------------
				if (m_activeConfiguration.getNaturalSelectors(false).size() > 0)
				{
					m_chromosomes = m_activeConfiguration.getNaturalSelectors(false).get_Renamed(0).select(m_activeConfiguration, m_chromosomes.Length);
					
					// Fire an event to indicate we've performed an evolution.
					// -------------------------------------------------------
					//m_activeConfiguration.EventManager.fireGeneticEvent(new GeneticEvent(GeneticEvent.GENOTYPE_EVOLVED_EVENT, this));
					
					// Iterate over the Chromosomes in the working pool. Clean up any that
					// haven't been selected to go on to the next generation.
					// -------------------------------------------------------------------
					foreach (Chromosome currentChromosome in m_workingPool)
					{
						if (!currentChromosome.SelectedForNextGeneration)
						{
							currentChromosome.cleanup();
						}
					}
					
					// Clear out the working pool in preparation for the next evolution
					// cycle.
					// ----------------------------------------------------------------
					m_workingPool.Clear();
					
					// Clean up the natural selector.
					// ------------------------------
					m_activeConfiguration.getNaturalSelectors(false).get_Renamed(0).empty();
				}
			}
		}
		
		
		/// <summary> Evolves this Genotype the specified number of times. This is
		/// equivalent to invoking the standard evolve() method the given number
		/// of times in a row.
		/// 
		/// </summary>
		/// <param name="a_numberOfEvolutions">The number of times to evolve this Genotype
		/// before returning.
		/// </param>
		public virtual void  evolve(int a_numberOfEvolutions)
		{
			for (int i = 0; i < a_numberOfEvolutions; i++)
			{
				evolve();
			}
		}
		
		
		/// <summary> Return a string representation of this Genotype instance,
		/// useful for dispaly purposes.
		/// 
		/// </summary>
		/// <returns> A string representation of this Genotype instance.
		/// </returns>
		public override System.String ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			
			for (int i = 0; i < m_chromosomes.Length; i++)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				buffer.Append(m_chromosomes[i].ToString());
				buffer.Append(" [");
				buffer.Append(m_chromosomes[i].FitnessValue);
				buffer.Append(']');
				buffer.Append('\n');
			}
			
			return buffer.ToString();
		}
		
		
		/// <summary> Convenience method that returns a newly constructed Genotype
		/// instance configured according to the given Configuration instance.
		/// The population of Chromosomes will created according to the setup of
		/// the sample Chromosome in the Configuration object, but the gene values
		/// (alleles) will be set to random legal values.
		/// <p>
		/// Note that the given Configuration instance must be in a valid state
		/// at the time this method is invoked, or an InvalidConfigurationException
		/// will be thrown.
		/// 
		/// </summary>
		/// <returns> A newly constructed Genotype instance.
		/// 
		/// @throws IllegalArgumentException if the given Configuration object is
		/// null.
		/// @throws InvalidConfigurationException if the given Configuration
		/// instance not in a valid state.
		/// </returns>
		public static Genotype randomInitialGenotype(Configuration a_activeConfiguration)
		{
			if (a_activeConfiguration == null)
			{
				throw new System.ArgumentException("The Configuration instance may not be null.");
			}
			
			a_activeConfiguration.lockSettings();
			
			// Create an array of chromosomes equal to the desired size in the
			// active Configuration and then populate that array with Chromosome
			// instances constructed according to the setup in the sample
			// Chromosome, but with random gene values (alleles). The Chromosome
			// class' randomInitialChromosome() method will take care of that for
			// us.
			// ------------------------------------------------------------------
			int populationSize = a_activeConfiguration.getPopulationSize();
			Chromosome[] chromosomes = new Chromosome[populationSize];
			
			for (int i = 0; i < populationSize; i++)
			{
				chromosomes[i] = Chromosome.randomInitialChromosome(a_activeConfiguration);
			}
			
			return new Genotype(a_activeConfiguration, chromosomes);
		}
		
		
		/// <summary> Compares this Genotype against the specified object. The result is true
		/// if the argument is an instance of the Genotype class, has exactly the
		/// same number of chromosomes as the given Genotype, and, for each
		/// Chromosome in this Genotype, there is an equal chromosome in the
		/// given Genotype. The chromosomes do not need to appear in the same order
		/// within the populations.
		/// 
		/// </summary>
		/// <param name="other">The object to compare against.
		/// </param>
		/// <returns> true if the objects are the same, false otherwise.
		/// </returns>
		public  override bool Equals(System.Object other)
		{
			try
			{
				// First, if the other Genotype is null, then they're not equal.
				// -------------------------------------------------------------
				if (other == null)
				{
					return false;
				}
				
				Genotype otherGenotype = (Genotype) other;
				
				// First, make sure the other Genotype has the same number of
				// chromosomes as this one.
				// ----------------------------------------------------------
				if (m_chromosomes.Length != otherGenotype.m_chromosomes.Length)
				{
					return false;
				}
				
				// Next, prepare to compare the chromosomes of the other Genotype
				// against the chromosomes of this Genotype. To make this a lot
				// simpler, we first sort the chromosomes in both this Genotype
				// and the one we're comparing against. This won't affect the
				// genetic algorithm (it doesn't care about the order), but makes
				// it much easier to perform the comparison here.
				// --------------------------------------------------------------
				//UPGRADE_TODO: Method 'java.util.Arrays.sort' was converted to 'System.Array.Sort' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilArrayssort_javalangObject[]"'
				System.Array.Sort(m_chromosomes);
				//UPGRADE_TODO: Method 'java.util.Arrays.sort' was converted to 'System.Array.Sort' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilArrayssort_javalangObject[]"'
				System.Array.Sort(otherGenotype.m_chromosomes);
				
				for (int i = 0; i < m_chromosomes.Length; i++)
				{
					if (!(m_chromosomes[i].Equals(otherGenotype.m_chromosomes[i])))
					{
						return false;
					}
				}
				
				return true;
			}
			catch (System.InvalidCastException )
			{
				return false;
			}
		}
		
		
		/// <summary> Verifies that a Configuration object has been properly set on this
		/// Genotype instance. If not, then an IllegalStateException is thrown.
		/// In general, this method should be invoked by any operation on this
		/// Genotype that makes use of the Configuration instance.
		/// </summary>
		private void  verifyConfigurationAvailable()
		{
			if (m_activeConfiguration == null)
			{
				throw new System.SystemException("The active Configuration object must be set on this " + "Genotype prior to invocation of other operations.");
			}
		}
	}
}