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
using Gene = org.jgap.Gene;
using Chromosome = org.jgap.Chromosome;
using Configuration = org.jgap.Configuration;
using GeneticOperator = org.jgap.GeneticOperator;
using RandomGenerator = org.jgap.RandomGenerator;
namespace org.jgap.impl
{
	
	
	/// <summary> The crossover operator randomly selects two Chromosomes from the
	/// population and "mates" them by randomly picking a gene and then
	/// swapping that gene and all subsequent genes between the two
	/// Chromosomes. The two modified Chromosomes are then added to the
	/// list of candidate Chromosomes. This operation is performed half
	/// as many times as there are Chromosomes in the population.
	/// 
	/// </summary>
	/// <author>  Neil Rotstan
	/// @since 1.0
	/// </author>
	[Serializable]
	public class CrossoverOperator : GeneticOperator
	{
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
			int numCrossovers = a_population.Length / 2;
			RandomGenerator generator = a_activeConfiguration.getRandomGenerator();
			
			// For each crossover, grab two random chromosomes, pick a random
			// locus (gene location), and then swap that gene and all genes
			// to the "right" (those with greater loci) of that gene between
			// the two chromosomes.
			// --------------------------------------------------------------
			for (int i = 0; i < numCrossovers; i++)
			{
				Chromosome firstMate = (Chromosome) a_population[generator.nextInt(a_population.Length)].Clone();
				
				Chromosome secondMate = (Chromosome) a_population[generator.nextInt(a_population.Length)].Clone();
				
				Gene[] firstGenes = firstMate.Genes;
				Gene[] secondGenes = secondMate.Genes;
				int locus = generator.nextInt(firstGenes.Length);
				
				// Swap the genes.
				// ---------------
				System.Object firstAllele;
				for (int j = locus; j < firstGenes.Length; j++)
				{
					firstAllele = firstGenes[j].getAllele();
					firstGenes[j].setAllele(secondGenes[j].getAllele());
					secondGenes[j].setAllele(firstAllele);
				}
				
				// Add the modified chromosomes to the candidate pool so that
				// they'll be considered for natural selection during the next
				// phase of evolution.
				// -----------------------------------------------------------
				//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				a_candidateChromosomes.Add(firstMate);
				//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				a_candidateChromosomes.Add(secondMate);
			}
		}
	}
}