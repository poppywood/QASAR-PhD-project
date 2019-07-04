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
using NaturalSelector = org.jgap.NaturalSelector;
using RandomGenerator = org.jgap.RandomGenerator;
namespace org.jgap.impl
{
	
	
	/// <summary> A basic implementation of NaturalSelector that models a roulette wheel.
	/// When a Chromosome is added, it gets a number of "slots" on the wheel equal
	/// to its fitness value. When the select method is invoked, the wheel is
	/// "spun" and the Chromosome occupying the spot on which it lands is selected.
	/// Then the wheel is spun again and again until the requested number of
	/// Chromosomes have been selected. Since Chromosomes with higher fitness
	/// values get more slots on the wheel, there's a higher statistical probability
	/// that they'll be chosen, but it's not guaranteed.
	/// 
	/// </summary>
	/// <author>  Neil Rotstan
	/// @since 1.0
	/// </author>
	public class WeightedRouletteSelector : NaturalSelector
	{
		public WeightedRouletteSelector()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilHashMap"'
			m_wheel = new System.Collections.Hashtable();
			m_counterPool = new Pool();
		}
		//UPGRADE_NOTE: Final was removed from the declaration of 'ZERO_BIG_DECIMAL '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003"'
		private static readonly System.Decimal ZERO_BIG_DECIMAL = new System.Decimal(0.0);
		
		
		/// <summary> Represents the "roulette wheel." Each key in the Map is a Chromosome
		/// and each value is an instance of the SlotCounter inner class, which
		/// keeps track of how many slots on the wheel each Chromosome is occupying.
		/// </summary>
		//UPGRADE_NOTE: The initialization of  'm_wheel' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private System.Collections.IDictionary m_wheel;
		
		/// <summary> Keeps track of the total number of slots that are in use on the
		/// roulette wheel. This is equal to the combined fitness values of
		/// all Chromosome instances that have been added to this wheel.
		/// </summary>
		private double m_totalNumberOfUsedSlots = 0.0;
		
		/// <summary> An internal pool in which discarded SlotCounter instances can be stored
		/// so that they can be reused over and over again, thus saving memory
		/// and the overhead of constructing new ones each time.
		/// </summary>
		//UPGRADE_NOTE: The initialization of  'm_counterPool' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private Pool m_counterPool;
		
		
		/// <summary> Add a Chromosome instance to this selector's working pool of Chromosomes.
		/// 
		/// </summary>
		/// <param name="a_activeConfigurator:">The current active Configuration to be used
		/// during the add process.
		/// </param>
		/// <param name="a_chromosomeToAdd:">The specimen to add to the pool.
		/// </param>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'add'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual void  add(Configuration a_activeConfigurator, Chromosome a_chromosomeToAdd)
		{
			lock (this)
			{
				// The "roulette wheel" is represented by a Map. Each key is a
				// Chromosome and each value is an instance of the SlotCounter inner
				// class. The counter keeps track of the total number of slots that
				// each chromosome is occupying on the wheel (which is equal to the
				// combined total of their fitness values). If the Chromosome is
				// already in the Map, then we just increment its number of slots
				// by its fitness value. Otherwise we add it to the Map.
				// -----------------------------------------------------------------
				//UPGRADE_TODO: Method 'java.util.Map.get' was converted to 'System.Collections.IDictionary.Item' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilMapget_javalangObject"'
				SlotCounter counter = (SlotCounter) m_wheel[a_chromosomeToAdd];
				
				if (counter != null)
				{
					// The Chromosome is already in the map.
					// -------------------------------------
					counter.increment();
				}
				else
				{
					// We need to add this Chromosome and an associated SlotCounter
					// to the map. First, we reset the Chromosome's
					// isSelectedForNextGeneration flag to false. Later, if the
					// Chromosome is actually selected to move on to the next
					// generation population by the select() method, then it will
					// be set to true.
					// ------------------------------------------------------------
					a_chromosomeToAdd.IsSelectedForNextGeneration = false;
					
					// We're going to need a SlotCounter. See if we can get one
					// from the pool. If not, construct a new one.
					// --------------------------------------------------------
					counter = (SlotCounter) m_counterPool.acquirePooledObject();
					if (counter == null)
					{
						counter = new SlotCounter();
					}
					
					counter.reset(a_chromosomeToAdd.FitnessValue);
					object tempObject;
					tempObject = counter;
					m_wheel[a_chromosomeToAdd] = tempObject;
					System.Object generatedAux = tempObject;
				}
			}
		}
		
		
		/// <summary> Select a given number of Chromosomes from the pool that will move on
		/// to the next generation population. This selection should be guided by
		/// the fitness values, but fitness should be treated as a statistical
		/// probability of survival, not as the sole determining factor. In other
		/// words, Chromosomes with higher fitness values should be more likely to
		/// be selected than those with lower fitness values, but it should not be
		/// guaranteed.
		/// 
		/// </summary>
		/// <param name="a_activeConfiguration:">The current active Configuration that is
		/// to be used during the selection process.
		/// </param>
		/// <param name="a_howManyToSelect:">The number of Chromosomes to select.
		/// 
		/// </param>
		/// <returns> An array of the selected Chromosomes.
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'select'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual Chromosome[] select(Configuration a_activeConfiguration, int a_howManyToSelect)
		{
			lock (this)
			{
				RandomGenerator generator = a_activeConfiguration.getRandomGenerator();
				Chromosome[] selections = new Chromosome[a_howManyToSelect];
				
				scaleFitnessValues();
				
				// Build three arrays from the key/value pairs in the wheel map: one
				// that contains the fitness values for each chromosome, one that
				// contains the total number of occupied slots on the wheel for each
				// chromosome, and one that contains the chromosomes themselves. The
				// array indices are used to associate the values of the three arrays
				// together (eg, if a chromosome is at index 5, then its fitness value
				// and counter values are also at index 5 of their respective arrays).
				// -------------------------------------------------------------------
				SupportClass.SetSupport entries = SupportClass.EntrySet(m_wheel);
				int numberOfEntries = entries.Count;
				double[] fitnessValues = new double[numberOfEntries];
				double[] counterValues = new double[numberOfEntries];
				Chromosome[] chromosomes = new Chromosome[numberOfEntries];
				
				m_totalNumberOfUsedSlots = 0.0;
				System.Collections.IEnumerator entryIterator = entries.GetEnumerator();
				for (int i = 0; i < numberOfEntries; i++)
				{
                    entryIterator.MoveNext();
                    //UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					System.Collections.IDictionaryEnumerator chromosomeEntry = (System.Collections.IDictionaryEnumerator) entryIterator.Current;
                    chromosomeEntry.MoveNext();
					Chromosome currentChromosome = (Chromosome) chromosomeEntry.Key;
	
					SlotCounter currentCounter = (SlotCounter) chromosomeEntry.Value;
					
					fitnessValues[i] = currentCounter.FitnessValue;
					counterValues[i] = currentCounter.FitnessValue * currentCounter.CounterValue;
					chromosomes[i] = currentChromosome;
					
					// We're also keeping track of the total number of slots,
					// which is the sum of all the counter values.
					// ------------------------------------------------------
					m_totalNumberOfUsedSlots += counterValues[i];
				}
				
				// To select each chromosome, we just "spin" the wheel and grab
				// whichever chromosome it lands on.
				// ------------------------------------------------------------
				Chromosome selectedChromosome;
				
				for (int i = 0; i < a_howManyToSelect; i++)
				{
					selectedChromosome = spinWheel(generator, fitnessValues, counterValues, chromosomes);
					
					selectedChromosome.IsSelectedForNextGeneration = true;
					selections[i] = selectedChromosome;
				}
				
				return selections;
			}
		}
		
		
		/// <summary> This method "spins" the wheel and returns the Chromosome that is
		/// "landed upon." Each time a chromosome is selected, one instance of it
		/// is removed from the wheel so that it cannot be selected again.
		/// 
		/// </summary>
		/// <param name="a_generator">The random number generator to be used during the
		/// spinning process.
		/// </param>
		/// <param name="a_fitnessValues">An array of fitness values of the respective
		/// Chromosomes.
		/// </param>
		/// <param name="a_counterValues">An array of total counter values of the
		/// respective Chromosomes.
		/// </param>
		/// <param name="a_chromosomes">The respective Chromosome instances from which
		/// selection is to occur.
		/// </param>
		private Chromosome spinWheel(RandomGenerator a_generator, double[] a_fitnessValues, double[] a_counterValues, Chromosome[] a_chromosomes)
		{
			// Randomly choose a slot on the wheel.
			// ------------------------------------
			double selectedSlot = System.Math.Abs(a_generator.nextDouble() * m_totalNumberOfUsedSlots);
			
			// Loop through the wheel until we find our selected slot. Here's
			// how this works: we have three arrays, one with the fitness values
			// of the chromosomes, one with the total number of slots on the
			// wheel that each chromosome occupies (its counter value), and
			// one with the chromosomes themselves. The array indices associate
			// each of the three together (eg, if a chromosome is at index 5,
			// then its fitness value and counter value are also at index 5 of
			// their respective arrays).
			//
			// We've already chosen a random slot number on the wheel from which
			// we want to select the Chromosome. We loop through each of the
			// array indices and, for each one, we add the number of occupied slots
			// (the counter value) to an ongoing total until that total
			// reaches or exceeds the chosen slot number. When that happenes,
			// we've found the chromosome sitting in that slot and we return it.
			// ------------------------------------------------------------------
			double currentSlot = 0.0;
			
			for (int i = 0; i < a_counterValues.Length; i++)
			{
				// Increment our ongoing total and see if we've landed on the
				// selected slot.
				// ----------------------------------------------------------
				currentSlot += a_counterValues[i];
				
				if (currentSlot > selectedSlot)
				{
					// Remove one instance of the chromosome from the wheel by
					// decrementing the slot counter by the fitness value.
					// --------------------------------------------------------
					a_counterValues[i] -= a_fitnessValues[i];
					m_totalNumberOfUsedSlots -= a_fitnessValues[i];
					
					// Now return our selected Chromosome
					// ----------------------------------
					return a_chromosomes[i];
				}
			}
			
			// If we have reached here, it means we have not found any chromosomes
			// to select and something is wrong with our logic. For some reason
			// the selected slot has exceeded the slots on our wheel. To help
			// with debugging, we tally up the total number of slots left on
			// the wheel and report it along with the chosen slot number that we
			// couldn't find.
			// -------------------------------------------------------------------
			long totalSlotsLeft = 0;
			for (int i = 0; i < a_counterValues.Length; i++)
			{
				totalSlotsLeft = (long) (totalSlotsLeft + a_counterValues[i]);
			}
			
			throw new System.SystemException("Logic Error. This code should never " + "be reached. Please report this as a bug to the " + "JGAP team: selected slot " + selectedSlot + " " + "exceeded " + totalSlotsLeft + " number of slots left. " + "We thought there were " + m_totalNumberOfUsedSlots + " slots left.");
		}
		
		
		/// <summary> Empty out the working pool of Chromosomes.</summary>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'empty'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual void  empty()
		{
			lock (this)
			{
				// Put all of the old SlotCounters into the pool so that we can
				// reuse them later instead of constructing new ones.
				// ------------------------------------------------------------
				m_counterPool.releaseAllObjects(m_wheel.Values);
				
				// Now clear the wheel and reset the internal state.
				// -------------------------------------------------
				m_wheel.Clear();
				m_totalNumberOfUsedSlots = 0;
			}
		}
		
		
		private void  scaleFitnessValues()
		{
			// First, add up all the fitness values. While we're doing this,
			// keep track of the largest fitness value we encounter.
			// -------------------------------------------------------------
			double largestFitnessValue = 0.0;
			System.Decimal totalFitness = ZERO_BIG_DECIMAL;
			
			System.Collections.IEnumerator counterIterator = m_wheel.Values.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			while (counterIterator.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				SlotCounter counter = (SlotCounter) counterIterator.Current;
				if (counter.FitnessValue > largestFitnessValue)
				{
					largestFitnessValue = counter.FitnessValue;
				}
				
				System.Decimal counterFitness = new System.Decimal(counter.FitnessValue);
				totalFitness = System.Decimal.Add(totalFitness, System.Decimal.Multiply(counterFitness, new System.Decimal(counter.CounterValue)));
			}
			
			// Now divide the total fitness by the largest fitness value to
			// compute the scaling factor.
			// ------------------------------------------------------------
			double scalingFactor = System.Decimal.ToDouble(System.Decimal.Divide(totalFitness, new System.Decimal(largestFitnessValue)));
			
			// Now divide each of the fitness values by the scaling factor to
			// scale them down.
			// --------------------------------------------------------------
			counterIterator = m_wheel.Values.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			while (counterIterator.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				SlotCounter counter = (SlotCounter) counterIterator.Current;
				counter.scaleFitnessValue(scalingFactor);
			}
		}
	}
	
	
	/// <summary> Implements a counter that is used to keep track of the total number of
	/// slots that a single Chromosome is occupying in the roulette wheel. Since
	/// all equal copies of a chromosome have the same fitness value, the increment
	/// method always adds the fitness value of the chromosome. Following
	/// construction of this class, the reset() method must be invoked to provide
	/// the initial fitness value of the Chromosome for which this SlotCounter is
	/// to be associated. The reset() method may be reinvoked to begin counting
	/// slots for a new Chromosome.
	/// </summary>
	class SlotCounter
	{
		/// <summary> Retrieves the fitness value of the chromosome for which this instance
		/// is acting as a counter.
		/// 
		/// </summary>
		/// <returns> The fitness value that was passed in at reset time.
		/// </returns>
		virtual public double FitnessValue
		{
			get
			{
				return m_fitnessValue;
			}
			
		}
		/// <summary> Retrieves the current value of this counter: ie, the number of slots
		/// on the roulette wheel that are  currently occupied by the Chromosome
		/// associated with this SlotCounter instance.
		/// 
		/// </summary>
		/// <returns> the current value of this counter.
		/// </returns>
		virtual public int CounterValue
		{
			get
			{
				return m_count;
			}
			
		}
		/// <summary> The fitness value of the Chromosome for which we are keeping count of
		/// roulette wheel slots. Although this value is constant for a Chromosome,
		/// it's not declared final here so that the slots can be reset and later
		/// reused for other Chromosomes, thus saving some memory and the overhead
		/// of constructing them from scratch.
		/// </summary>
		private double m_fitnessValue = 0.0;
		
		/// <summary> The current number of Chromosomes represented by this counter.</summary>
		private int m_count = 0;
		
		
		/// <summary> Resets the internal state of this SlotCounter instance so that it can
		/// be used to count slots for a new Chromosome.
		/// 
		/// </summary>
		/// <param name="a_initialFitness">The fitness value of the Chromosome for which
		/// this instance is acting as a counter.
		/// </param>
		public virtual void  reset(double a_initialFitness)
		{
			m_fitnessValue = a_initialFitness;
			m_count = 1;
		}
		
		
		/// <summary> Increments the value of this counter by the fitness value that was
		/// passed in at reset time.
		/// </summary>
		public virtual void  increment()
		{
			m_count++;
		}
		
		
		/// <summary> Scales this SlotCounter's fitness value by the given scaling factor.
		/// 
		/// </summary>
		/// <param name="a_scalingFactor">The factor by which the fitness value is to be
		/// scaled.
		/// </param>
		public virtual void  scaleFitnessValue(double a_scalingFactor)
		{
			m_fitnessValue /= a_scalingFactor;
		}
	}
}