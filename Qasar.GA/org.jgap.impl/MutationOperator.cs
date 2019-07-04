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
using Chromosome = org.jgap.Chromosome;
using Configuration = org.jgap.Configuration;
using Gene = org.jgap.Gene;
using GeneticOperator = org.jgap.GeneticOperator;
using MutationRateCalculator = org.jgap.MutationRateCalculator;
using RandomGenerator = org.jgap.RandomGenerator;
namespace org.jgap.impl
{
	
	/// <summary> The mutation operator runs through the genes in each of the Chromosomes
	/// in the population and mutates them in statistical accordance to the
	/// given mutation rate. Mutated Chromosomes are then added to the list of
	/// candidate Chromosomes destined for the natural selection process.
	/// <p>
	/// This MutationOperator supports both fixed and dynamic mutation rates.
	/// A fixed rate is one specified at construction time by the user. A dynamic
	/// rate is one determined by this class if no fixed rate is provided, and is
	/// calculated based on the size of the Chromosomes in the population such
	/// that, on average, one gene will be mutated for every ten Chromosomes
	/// processed by this operator.
	/// 
	/// </summary>
	/// <author>  Neil Rotstan
	/// @since 1.0
	/// </author>
	public class MutationOperator : GeneticOperator
	{
		virtual public MutationRateCalculator MutationRateCalc
		{
			get
			{
				return m_mutationRateCalc;
			}
			
			set
			{
				this.m_mutationRateCalc = value;
				if (value != null)
				{
					m_mutationRate = 0;
				}
			}
			
		}
		/// <summary> The current mutation rate used by this MutationOperator, expressed as
		/// the denominator in the 1 / X ratio. For example, a value of 1000 would
		/// mean that, on average, 1 / 1000 genes would be mutated. A value of zero
		/// disabled mutation entirely.
		/// </summary>
		protected internal int m_mutationRate;
		
		/// <summary> Calculator for dynamically determining the mutation rate. If set to
		/// null the value of m_mutationRate will be used.
		/// Replaces the previously used boolean m_dynamicMutationRate
		/// @since 1.1
		/// </summary>
		private MutationRateCalculator m_mutationRateCalc;
		
		/// <summary> Constructs a new instance of this MutationOperator without a specified
		/// mutation rate, which results in dynamic mutation being turned on. This
		/// means that the mutation rate will be automatically determined by this
		/// operator based upon the number of genes present in the chromosomes.
		/// </summary>
		public MutationOperator()
		{
			MutationRateCalc = new DefaultMutationRateCalculator();
		}
		
		/// <summary> Constructs a new instance of this MutationOperator with a specified
		/// mutation rate calculator, which results in dynamic mutation being turned
		/// on.
		/// </summary>
		/// <param name="a_mutationRateCalculator">calculator for dynamic mutation rate
		/// computation
		/// </param>
		public MutationOperator(MutationRateCalculator a_mutationRateCalculator)
		{
			MutationRateCalc = a_mutationRateCalculator;
		}
		
		/// <summary> Constructs a new instance of this MutationOperator with the given
		/// mutation rate.
		/// 
		/// </summary>
		/// <param name="a_desiredMutationRate">The desired rate of mutation, expressed
		/// as the denominator of the 1 / X fraction.
		/// For example, 1000 would result in 1/1000
		/// genes being mutated on average. A mutation
		/// rate of zero disables mutation entirely.
		/// </param>
		public MutationOperator(int a_desiredMutationRate)
		{
			m_mutationRate = a_desiredMutationRate;
			MutationRateCalc = null;
		}
		
		/// <summary> The operate method will be invoked on each of the genetic operators
		/// referenced by the current Configuration object during the evolution
		/// phase. Operators are given an opportunity to run in the order that
		/// they are added to the Configuration. Implementations of this method
		/// may reference the population of Chromosomes as it was at the beginning
		/// of the evolutionary phase and/or they may instead reference the
		/// candidate Chromosomes, which are the results of prior genetic operators.
		/// In either case, only Chromosomes added to the list of candidate
		/// chromosomes will be considered for natural selection. Implementations
		/// should never modify the original population, but should first make copies
		/// of the Chromosomes selected for modification and operate upon the copies.
		/// 
		/// </summary>
		/// <param name="a_activeConfiguration">The current active genetic configuration.
		/// </param>
		/// <param name="a_population">The population of chromosomes from the current
		/// evolution prior to exposure to any genetic operators.
		/// Chromosomes in this array should never be modified.
		/// </param>
		/// <param name="a_candidateChromosomes">The pool of chromosomes that are candidates
		/// for the next evolved population. Only these
		/// chromosomes will go to the natural
		/// phase, so it's important to add any
		/// modified copies of Chromosomes to this
		/// list if it's desired for them to be
		/// considered for natural selection.
		/// </param>
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		public virtual void  operate(Configuration a_activeConfiguration, Chromosome[] a_population, SupportClass.ListCollectionSupport a_candidateChromosomes)
		{
			// If the mutation rate is set to zero and dynamic mutation rate is
			// disabled, then we don't perform any mutation.
			// ----------------------------------------------------------------
			if (m_mutationRate == 0 && m_mutationRateCalc == null)
			{
				return ;
			}
			
			// Determine the mutation rate. If dynamic rate is enabled, then
			// calculate it based upon the number of genes in the chromosome.
			// Otherwise, go with the mutation rate set upon construction.
			// --------------------------------------------------------------
			int currentRate = m_mutationRateCalc != null?m_mutationRateCalc.calculateCurrentRate(a_activeConfiguration):m_mutationRate;
			
			RandomGenerator generator = a_activeConfiguration.getRandomGenerator();
			
			// It would be inefficient to create copies of each Chromosome just
			// to decide whether to mutate them. Instead, we only make a copy
			// once we've positively decided to perform a mutation.
			// ----------------------------------------------------------------
			for (int i = 0; i < a_population.Length; i++)
			{
				Gene[] genes = a_population[i].Genes;
				Chromosome copyOfChromosome = null;
				
				// For each Chromosome in the population...
				// ----------------------------------------
				for (int j = 0; j < genes.Length; j++)
				{
					// Ensure probability of 1/currentRate for applying mutation
					// ---------------------------------------------------------
					if (generator.nextInt(currentRate) == 0)
					{
						// Now that we want to actually modify the Chromosome,
						// let's make a copy of it (if we haven't already) and
						// add it to the candidate chromosomes so that it will
						// be considered for natural selection during the next
						// phase of evolution. Then we'll set the gene's value
						// to a random value as the implementation of our
						// "mutation" of the gene.
						// ---------------------------------------------------
						if (copyOfChromosome == null)
						{
							// ...take a copy of it...
							// -----------------------
							copyOfChromosome = (Chromosome) a_population[i].Clone();
							
							// ...add it to the candidate pool...
							// ----------------------------------
							//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
							a_candidateChromosomes.Add(copyOfChromosome);
							
							// ...then Gaussian mutate all its genes...
							// ----------------------------------------
							genes = copyOfChromosome.Genes;
						}
						// Significant architectural changes made here due to
						// request 708772 (also changed Gene classes)
						// --------------------------------------------------
						
						// Process all atomic elements in the gene. For a StringGene
						// this would be the length of the string, for an
						// IntegerGene, it is always one element
						// ---------------------------------------------------------
						if (genes[j] is CompositeGene)
						{
							CompositeGene compositeGene = (CompositeGene) genes[j];
							for (int k = 0; k < compositeGene.size(); k++)
							{
								mutateGene(compositeGene.geneAt(k), generator);
							}
						}
						else
						{
							mutateGene(genes[j], generator);
						}
						// End of changed for request 708772
					}
				}
			}
		}
		
		/// <summary> Helper: mutate all atomic elements of a gene</summary>
		/// <param name="a_gene">the gene to be mutated
		/// </param>
		/// <param name="a_generator">the generator delivering amount of mutation
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		private void  mutateGene(Gene a_gene, RandomGenerator a_generator)
		{
			for (int k = 0; k < a_gene.size(); k++)
			{
				// Retrieve value between 0 and 1 (not included) from
				// generator. Then map this value to range -1 and 1
				// (-1 included, 1 not)
				// --------------------------------------------------
				double percentage = - 1 + a_generator.nextDouble() * 2;
				
				// Mutate atomic element by calculated percentage
				// ----------------------------------------------
				a_gene.applyMutation(k, percentage);
			}
		}
	}
}