/*
* Copyright 2003 Klaus Meffert
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
using org.jgap;
namespace org.jgap.impl
{
	
	/// <summary> Implementation of a NaturalSelector that takes the top n chromosomes into
	/// the next generation. n can be specified. Which chromosomes are the best is
	/// decided by evaluating their fitness value
	/// 
	/// </summary>
	/// <author>  Klaus Meffert
	/// @since 1.1
	/// </author>
	public class BestChromosomesSelector : NaturalSelector
	{	
		/// <summary>String containing the CVS revision. Read out via reflection!</summary>
		private const System.String CVS_REVISION = "$Revision: 1.1 $";
		
		/// <summary> Stores the chromosomes to be taken into account for selection</summary>
		private System.Collections.ArrayList chromosomes;
		
		/// <summary> Indicated whether the list of added chromosomes needs sorting</summary>
		private bool needsSorting;
		
		/// <summary> Comparator that is only concerned about fitness values</summary>
		private FitnessValueComparator fitnessValueComparator;
		
		public BestChromosomesSelector()
		{
			chromosomes = new System.Collections.ArrayList(10);
			needsSorting = false;
			fitnessValueComparator = new FitnessValueComparator(this);
		}
		
		/// <summary> Add a Chromosome instance to this selector's working pool of Chromosomes.</summary>
		/// <param name="a_activeConfigurator:">The current active Configuration to be used
		/// during the add process.
		/// </param>
		/// <param name="a_chromosomeToAdd:">The specimen to add to the pool.
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual void  add(Configuration a_activeConfigurator, Chromosome a_chromosomeToAdd)
		{
			lock (this)
			{
				// Check if chromosome already added
				if (chromosomes.Contains(a_chromosomeToAdd))
				{
					return ;
				}
				// New chromosome, insert it into the sorted collection of chromosomes
				a_chromosomeToAdd.IsSelectedForNextGeneration = false;
				chromosomes.Add(a_chromosomeToAdd);
				
				// Indicate that the list of chromosomes to add needs sorting
				// ----------------------------------------------------------
				needsSorting = true;
			}
		}
		
		/// <summary> Select a given number of Chromosomes from the pool that will move on
		/// to the next generation population. This selection will be guided by the
		/// fitness values. The chromosomes with the best fitness value win.
		/// 
		/// </summary>
		/// <param name="a_activeConfiguration:">The current active Configuration that is
		/// to be used during the selection process.
		/// </param>
		/// <param name="a_howManyToSelect:">The number of Chromosomes to select.
		/// 
		/// </param>
		/// <returns> An array of the selected Chromosomes.
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual Chromosome[] select(Configuration a_activeConfiguration, int a_howManyToSelect)
		{
			lock (this)
			{
				// Sort the collection of chromosomes previously added for evaluation.
				// Only do this if necessary.
				// -------------------------------------------------------------------
				if (needsSorting)
				{
					//UPGRADE_TODO: Method 'java.util.Collections.sort' was converted to 'SupportClass.CollectionsManager.Sort' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
					chromosomes.Sort(fitnessValueComparator);
					needsSorting = false;
				}

				if (chromosomes.Count < a_howManyToSelect)
					a_howManyToSelect = chromosomes.Count;

				Chromosome[] selections = new Chromosome[a_howManyToSelect];
				
				// To select a chromosome, we just go thru the sorted list.
				// --------------------------------------------------------
				Chromosome selectedChromosome;
				
				for (int i = 0; i < a_howManyToSelect; i++)
				{
					selectedChromosome = (Chromosome) chromosomes[i];
					
					selectedChromosome.IsSelectedForNextGeneration = true;
					selections[i] = selectedChromosome;
				}
				
				return selections;
			}
		}
		
		/// <summary> Empty out the working pool of Chromosomes.
		/// 
		/// @since 1.1
		/// </summary>
		public virtual void  empty()
		{
			lock (this)
			{
				// clear the list of chromosomes
				// -----------------------------
				chromosomes.Clear();
				needsSorting = false;
			}
		}
		
		/// <summary>
		/// Comparator regarding only the fitness value. Best fitness value will
		/// be on first position of resulted sorted list
		/// </summary>
		/// 
		/// <author>  Klaus Meffert
		/// </author>
		/// <version>  1.0
		/// </version>
		private class FitnessValueComparator : System.Collections.IComparer
		{
			public FitnessValueComparator(BestChromosomesSelector enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			private void  InitBlock(BestChromosomesSelector enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private BestChromosomesSelector enclosingInstance;
			public BestChromosomesSelector Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			public virtual int Compare(System.Object first, System.Object second)
			{
				Chromosome chrom1 = (Chromosome) first;
				Chromosome chrom2 = (Chromosome) second;
				
                //if (chrom1.FitnessValue < chrom2.FitnessValue)
                //{
                //    return 1;
                //}
                //else if (chrom1.FitnessValue > chrom2.FitnessValue)
                //{
                //    return - 1;
                //}
                //else
                //{
                //    return 0;
                //}

                if (chrom1.FitnessValue < chrom2.FitnessValue)
                {
                    return 1;
                }
                else if (chrom1.FitnessValue > chrom2.FitnessValue)
                {
                    return - 1;
                }
                else
                {
                    return 0;
                }
			}
		}
	}
}