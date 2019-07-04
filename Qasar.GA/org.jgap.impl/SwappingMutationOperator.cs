
using System;
using org.jgap;
namespace org.jgap.impl
{
	[Serializable]
	public class SwappingMutationOperator:MutationOperator
	{
		virtual public int StartOffset
		{
			get
			{
				return m_startOffset;
			}
			
			set
			{
				m_startOffset = value;
			}
			
		}
		/// <summary>String containing the CVS revision. Read out via reflection!</summary>
		private const System.String CVS_REVISION = "$Revision: 1.19 $";
		
		private int m_startOffset = 0;
		
		/// <summary> Constructs a new instance of this operator.<p>
		/// Attention: The configuration used is the one set with the static method
		/// Genotype.setConfiguration.
		/// 
		/// </summary>
		/// <throws>  InvalidConfigurationException </throws>
		/// <summary> 
		/// </summary>
		/// <author>  Klaus Meffert
		/// </author>
		public SwappingMutationOperator():base()
		{
		}
		
		
		/// <summary> Constructs a new instance of this operator with a specified
		/// mutation rate calculator, which results in dynamic mutation being turned
		/// on.
		/// 
		/// </summary>
		/// <param name="a_config">the configuration to use
		/// </param>
		/// <param name="a_mutationRateCalculator">calculator for dynamic mutation rate
		/// computation
		/// </param>
		/// <throws>  InvalidConfigurationException </throws>
		/// <summary> 
		/// </summary>
		/// <author>  Klaus Meffert
		/// </author>
		/// <since> 3.0 (previously: without a_config)
		/// </since>
		public SwappingMutationOperator(Configuration a_config, MutationRateCalculator a_mutationRateCalculator):base(a_mutationRateCalculator)
		{
		}
		
		/// <summary> Constructs a new instance of this MutationOperator with the given
		/// mutation rate.
		/// 
		/// </summary>
		/// <param name="a_config">the configuration to use
		/// </param>
		/// <param name="a_desiredMutationRate">desired rate of mutation, expressed as
		/// the denominator of the 1 / X fraction. For example, 1000 would result
		/// in 1/1000 genes being mutated on average. A mutation rate of zero disables
		/// mutation entirely
		/// </param>
		/// <throws>  InvalidConfigurationException </throws>
		/// <summary> 
		/// </summary>
		/// <author>  Klaus Meffert
		/// </author>
		/// <since> 3.0 (previously: without a_config)
		/// </since>
		public SwappingMutationOperator(Configuration a_config, int a_desiredMutationRate):base(a_desiredMutationRate)
		{
		}
		
		/// <param name="a_population">the population of chromosomes from the current
		/// evolution prior to exposure to any genetic operators. Chromosomes in this
		/// array should not be modified. Please, notice, that the call in
		/// Genotype.evolve() to the implementations of GeneticOperator overgoes this
		/// due to performance issues
		/// </param>
		/// <param name="a_candidateChromosomes">the pool of chromosomes that have been
		/// selected for the next evolved population
		/// 
		/// </param>
		/// <author>  Audrius Meskauskas
		/// </author>
		/// <author>  Klaus Meffert
		/// </author>
		/// <since> 2.0
		/// </since>
        public override void operate(Configuration a_activeConfiguration, Chromosome[] a_population, SupportClass.ListCollectionSupport a_candidateChromosomes)
		{
			// this was a private variable, now it is local reference.
			//UPGRADE_NOTE: Final was removed from the declaration of 'm_mutationRateCalc '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
			MutationRateCalculator m_mutationRateCalc = MutationRateCalc;
			// If the mutation rate is set to zero and dynamic mutation rate is
			// disabled, then we don't perform any mutation.
			// ----------------------------------------------------------------
            if (base.m_mutationRate == 0 && m_mutationRateCalc == null)
			{
				return ;
			}
			// Determine the mutation rate. If dynamic rate is enabled, then
			// calculate it based upon the number of genes in the chromosome.
			// Otherwise, go with the mutation rate set upon construction.
			// --------------------------------------------------------------
			int currentRate;
			if (m_mutationRateCalc != null)
			{
				currentRate = m_mutationRateCalc.calculateCurrentRate(a_activeConfiguration);
			}
			else
			{
				currentRate = base.m_mutationRate;
			}
            RandomGenerator generator = a_activeConfiguration.getRandomGenerator();
			// It would be inefficient to create copies of each Chromosome just
			// to decide whether to mutate them. Instead, we only make a copy
			// once we've positively decided to perform a mutation.
			// ----------------------------------------------------------------
			int size = a_population.Length;
			for (int i = 0; i < size; i++)
			{
				Chromosome x = a_population[i];
				// This returns null if not mutated:
				Chromosome xm = operate(x, currentRate, generator);
				if (xm != null)
				{
					a_candidateChromosomes.Add(xm);
				}
			}
		}
		
		/// <summary> Operate on the given chromosome with the given mutation rate.
		/// 
		/// </summary>
		/// <param name="a_x">chromosome to operate
		/// </param>
		/// <param name="a_rate">mutation rate
		/// </param>
		/// <param name="a_generator">random generator to use (must not be null)
		/// </param>
		/// <returns> mutated chromosome of null if no mutation has occured.
		/// 
		/// </returns>
		/// <author>  Audrius Meskauskas
		/// </author>
		/// <since> 2.0
		/// </since>
		protected internal virtual Chromosome operate(Chromosome a_x, int a_rate, RandomGenerator a_generator)
		{
			Chromosome chromosome = null;
			// ----------------------------------------
			for (int j = m_startOffset; j < a_x.size(); j++)
			{
				// Ensure probability of 1/currentRate for applying mutation.
				// ----------------------------------------------------------
				if (a_generator.nextInt(a_rate) == 0)
				{
					if (chromosome == null)
					{
						chromosome = (Chromosome) a_x.Clone();
					}
					Gene[] genes = chromosome.Genes;
					Gene[] mutated = operate(a_generator, j, genes);
					// setGenes is not required for this operator, but it may
					// be needed for the derived operators.
					// ------------------------------------------------------
					try
					{
						chromosome.setGenes(mutated);
					}
					catch (InvalidConfigurationException cex)
					{
						//throw new Error("Gene type not allowed by constraint checker", cex);
					}
				}
			}
			return chromosome;
		}
		
		/// <summary> Operate on the given array of genes. This method is only called
		/// when it is already clear that the mutation must occur under the given
		/// mutation rate.
		/// 
		/// </summary>
		/// <param name="a_generator">a random number generator that may be needed to
		/// perform a mutation
		/// </param>
		/// <param name="a_target_gene">an index of gene in the chromosome that will mutate
		/// </param>
		/// <param name="a_genes">the array of all genes in the chromosome
		/// </param>
		/// <returns> the mutated gene array
		/// 
		/// </returns>
		/// <author>  Audrius Meskauskas
		/// </author>
		/// <since> 2.0
		/// </since>
		protected internal virtual Gene[] operate(RandomGenerator a_generator, int a_target_gene, Gene[] a_genes)
		{
			// swap this gene with the other one now:
			//  mutateGene(genes[j], generator);
			// -------------------------------------
			int other = m_startOffset + a_generator.nextInt(a_genes.Length - m_startOffset);
			Gene t = a_genes[a_target_gene];
			a_genes[a_target_gene] = a_genes[other];
			a_genes[other] = t;
			return a_genes;
		}
	}
}