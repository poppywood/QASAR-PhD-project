/*
* This file is part of JGAP.
*
* JGAP offers a dual license model containing the LGPL as well as the MPL.
*
* For licensing information please see the file license.txt included with JGAP
* or have a look at the top of class org.jgap.Chromosome which representatively
* includes the JGAP license policy applicable for any file delivered with JGAP.
*/
using System;
using org.jgap;
namespace org.jgap.impl
{
	
	/// <summary> The Greedy Crossover is a specific type of crossover. It can only be is
	/// applied if
	/// <ul>
	/// <li>
	/// 1. All genes in the chromosome are different and
	/// </li>
	/// <li>
	/// 2. The set of genes for both chromosomes is identical and only they order
	/// in the chromosome can vary.
	/// </li>
	/// </ul>
	/// 
	/// After the GreedyCrossover, these two conditions always remain true, so
	/// it can be applied again and again.
	/// 
	/// The algorithm throws an assertion error if the two initial chromosomes
	/// does not satisfy these conditions.
	/// 
	/// 
	/// Greedy crossover can be best explained in the terms of the
	/// Traveling Salesman Problem:
	/// 
	/// The algorithm selects the first city of one parent, compares the cities
	/// leaving that city in both parents, and chooses the closer one to extend
	/// the tour. If one city has already appeared in the tour, we choose the
	/// other city. If both cities have already appeared, we randomly select a
	/// non-selected city.
	/// 
	/// See J. Grefenstette, R. Gopal, R. Rosmaita, and D. Gucht.
	/// <i>Genetic algorithms for the traveling salesman problem</i>.
	/// In Proceedings of the Second International Conference on Genetic Algorithms.
	/// Lawrence Eribaum Associates, Mahwah, NJ, 1985.
	/// and also <a href="http://ecsl.cs.unr.edu/docs/techreports/gong/node3.html">
	/// Sushil J. Louis & Gong Li</a>}
	/// 
	/// </summary>
	/// <author>  Audrius Meskauskas
	/// </author>
	/// <author>  <font size=-1>Neil Rotstan, Klaus Meffert (reused code
	/// from {@link org.jgap.impl.CrossoverOperator CrossoverOperator})</font>
	/// </author>
	/// <since> 2.0
	/// </since>
	[Serializable]
	public class GreedyCrossover:GeneticOperator
	{
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Gets a number of genes at the start of chromosome, that are
		/// excluded from the swapping. In the Salesman task, the first city
		/// in the list should (where the salesman leaves from) probably should
		/// not change as it is part of the list. The default value is 1.
		/// 
		/// </summary>
		/// <returns> the start offset used
		/// </returns>
		/// <summary> Sets a number of genes at the start of chromosome, that are
		/// excluded from the swapping. In the Salesman task, the first city
		/// in the list should (where the salesman leaves from) probably should
		/// not change as it is part of the list. The default value is 1.
		/// 
		/// </summary>
		/// <param name="a_offset">the start offset to use
		/// </param>
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
		private const System.String CVS_REVISION = "$Revision: 1.27 $";
		
		/// <summary>Switches assertions on/off. Must be true during tests and debugging. </summary>
		internal bool ASSERTIONS = true;
		
		private int m_startOffset = 0;
		
		/// <summary> Default constructor for dynamic instantiation.<p>
		/// Attention: The configuration used is the one set with the static method
		/// Genotype.setConfiguration.
		/// 
		/// </summary>
		/// <throws>  InvalidConfigurationException </throws>
		/// <summary> 
		/// </summary>
		/// <author>  Klaus Meffert
		/// </author>
		/// <since> 2.6
		/// </since>
		/// <since> 3.0 (since 2.0 without a_configuration)
		/// </since>
		public GreedyCrossover():base()
		{
		}
		
		/// <summary> Using the given configuration.
		/// 
		/// </summary>
		/// <param name="a_configuration">the configuration to use
		/// </param>
		/// <throws>  InvalidConfigurationException </throws>
		/// <summary> 
		/// </summary>
		/// <author>  Klaus Meffert
		/// </author>
		/// <since> 3.0 (since 2.6 without a_configuration)
		/// </since>
		public GreedyCrossover(Configuration a_configuration):base()
		{
		}
		
		/// <summary> Compute the distance between "cities", indicated by these two
		/// given genes. The default method expects the genes to be a
		/// IntegerGenes's and returns they absolute difference, that
		/// makes sense only for tests.
		/// 
		/// </summary>
		/// <param name="a_from">Object
		/// </param>
		/// <param name="a_to">Object
		/// </param>
		/// <returns> distance between the two given cities
		/// </returns>
		public virtual double distance(System.Object a_from, System.Object a_to)
		{
			IntegerGene from = (IntegerGene) a_from;
			IntegerGene to = (IntegerGene) a_to;
			return System.Math.Abs(to.intValue() - from.intValue());
		}

        public virtual void operate(Configuration a_activeConfiguration, Chromosome[] a_population, SupportClass.ListCollectionSupport a_candidateChromosomes)
		{
            int size = System.Math.Min(a_activeConfiguration.getPopulationSize(), a_population.Length);
			int numCrossovers = size / 2;
            RandomGenerator generator = a_activeConfiguration.getRandomGenerator();
			// For each crossover, grab two random chromosomes and do what
			// Grefenstette et al say.
			// --------------------------------------------------------------
			for (int i = 0; i < numCrossovers; i++)
			{
                Chromosome firstMate = (Chromosome)a_population[generator.nextInt(size)].Clone();
                Chromosome secondMate = (Chromosome)a_population[generator.nextInt(size)].Clone();
				operate(firstMate, secondMate);
				// Add the modified chromosomes to the candidate pool so that
				// they'll be considered for natural selection during the next
				// phase of evolution.
				// -----------------------------------------------------------
				a_candidateChromosomes.Add(firstMate);
				a_candidateChromosomes.Add(secondMate);
			}
		}
		
		/// <summary> Performs a greedy crossover for the two given chromosoms.
		/// 
		/// </summary>
		/// <param name="a_firstMate">the first chromosome to crossover on
		/// </param>
		/// <param name="a_secondMate">the second chromosome to crossover on
		/// </param>
		/// <throws>  Error if the gene set in the chromosomes is not identical </throws>
		/// <summary> 
		/// </summary>
		/// <author>  Audrius Meskauskas
		/// </author>
		/// <since> 2.1
		/// </since>
		public virtual void  operate(Chromosome a_firstMate, Chromosome a_secondMate)
		{
			Gene[] g1 = a_firstMate.Genes;
			Gene[] g2 = a_secondMate.Genes;
			Gene[] c1, c2;
			try
			{
				c1 = operate(g1, g2);
				c2 = operate(g2, g1);
				a_firstMate.setGenes(c1);
				a_secondMate.setGenes(c2);
			}
			catch (InvalidConfigurationException cex)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				throw new System.ApplicationException("Error occured while operating on:" + a_firstMate + " and " + a_secondMate + ". First " + m_startOffset + " genes were excluded " + "from crossover. Error message: " + cex.Message);
			}
		}
		
		protected internal virtual Gene[] operate(Gene[] a_g1, Gene[] a_g2)
		{
			int n = a_g1.Length;
			//UPGRADE_TODO: Class 'java.util.LinkedList' was converted to 'System.Collections.ArrayList' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilLinkedList'"
			System.Collections.ArrayList out_Renamed = new System.Collections.ArrayList();
			//UPGRADE_TODO: Class 'java.util.TreeSet' was converted to 'SupportClass.TreeSetSupport' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilTreeSet'"
			SupportClass.SetSupport not_picked = new SupportClass.SetSupport();
			out_Renamed.Add(a_g1[m_startOffset]);
			for (int j = m_startOffset + 1; j < n; j++)
			{
				// g[m_startOffset] picked
				if (ASSERTIONS && not_picked.Contains(a_g1[j]))
				{
					throw new System.ApplicationException("All genes must be different for " + GetType().FullName + ". The gene " + a_g1[j] + "[" + j + "] occurs more " + "than once in one of the chromosomes. ");
				}
				not_picked.Add(a_g1[j]);
			}
			if (ASSERTIONS)
			{
				if (a_g1.Length != a_g2.Length)
				{
					throw new System.ApplicationException("Chromosome sizes must be equal");
				}
                //for (int j = m_startOffset; j < n; j++)
                //{
                //    if (!not_picked.Contains(a_g2[j]))
                //    {
                //        if (!a_g1[m_startOffset].Equals(a_g2[j]))
                //        {
                //            //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                //            //throw new System.ApplicationException("Chromosome gene sets must be identical." + " First gene set: " + a_g1 + ", second gene set: " + a_g2);
                //        }
                //    }
                //}
			}
			while (not_picked.Count > 1)
			{
				Gene last = (Gene) out_Renamed[out_Renamed.Count - 1];
				Gene n1 = findNext(a_g1, last);
				Gene n2 = findNext(a_g2, last);
				Gene picked, other;
				bool pick1;
				if (n1 == null)
				{
					pick1 = false;
				}
				else if (n2 == null)
				{
					pick1 = true;
				}
				else
				{
					pick1 = distance(last, n1) < distance(last, n2);
				}
				if (pick1)
				{
					picked = n1;
					other = n2;
				}
				else
				{
					picked = n2;
					other = n1;
				}
				if (out_Renamed.Contains(picked))
				{
					picked = other;
				}
				if (picked == null || out_Renamed.Contains(picked))
				{
					// select a non-selected // it is not random
					picked = (Gene) not_picked[0];
				}
				out_Renamed.Add(picked);
				not_picked.Remove(picked);
			}
			if (ASSERTIONS && not_picked.Count != 1)
			{
				throw new System.ApplicationException("Given Gene not correctly created (must have length > 1" + ")");
			}
			out_Renamed.Add(not_picked[not_picked.Count - 1]);
			Gene[] g = new Gene[n];

			//System.Collections.IEnumerator gi = out_Renamed.GetEnumerator();
			
            for (int i = 0; i < m_startOffset; i++)
			{
				g[i] = a_g1[i];
			}
			if (ASSERTIONS)
			{
				if (out_Renamed.Count != g.Length - m_startOffset)
				{
					throw new System.ApplicationException("Unexpected internal error. " + "These two must be equal: " + out_Renamed.Count + " and " + (g.Length - m_startOffset) + ", g.length " + g.Length + ", start offset " + m_startOffset);
				}
			}
			for (int i = m_startOffset; i < g.Length; i++)
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
				//g[i] = (Gene) gi.Current;
                g[i] = (Gene)out_Renamed[i];
                
			}
			return g;
		}
		
		protected internal virtual Gene findNext(Gene[] a_g, Gene a_x)
		{
			for (int i = m_startOffset; i < a_g.Length - 1; i++)
			{
				if (a_g[i].Equals(a_x))
				{
					return a_g[i + 1];
				}
			}
			return null;
		}
		
		/// <summary> Compares the given GeneticOperator to this GeneticOperator.
		/// 
		/// </summary>
		/// <param name="a_other">the instance against which to compare this instance
		/// </param>
		/// <returns> a negative number if this instance is "less than" the given
		/// instance, zero if they are equal to each other, and a positive number if
		/// this is "greater than" the given instance
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// </author>
		/// <since> 2.6
		/// </since>
		public virtual int CompareTo(System.Object a_other)
		{
			if (a_other == null)
			{
				return 1;
			}
			GreedyCrossover op = (GreedyCrossover) a_other;
			if (StartOffset < op.StartOffset)
			{
				// start offset less, meaning more to do --> return 1 for "is greater than"
				return 1;
			}
			else if (StartOffset > op.StartOffset)
			{
				return - 1;
			}
			else
			{
				// Everything is equal. Return zero.
				// ---------------------------------
				return 0;
			}
		}
	}
}