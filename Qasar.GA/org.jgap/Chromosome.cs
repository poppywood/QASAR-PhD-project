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
using ChromosomePool = org.jgap.impl.ChromosomePool;
namespace org.jgap
{
	
	/// <summary> Chromosomes represent potential solutions and consist of a fixed-length
	/// collection of genes. Each gene represents a discrete part of the solution.
	/// Each gene in the Chromosome may be backed by a different concrete
	/// implementation of the Gene interface, but all genes in a respective
	/// position (locus) must share the same concrete implementation across
	/// Chromosomes within a single population (genotype). In other words, gene 1
	/// in a chromosome must share the same concrete implementation as gene 1 in all
	/// other chromosomes in the population.
	/// 
	/// </summary>
	/// <author>  Neil Rotstan
	/// @since 1.0
	/// </author>
	[Serializable]
	public class Chromosome : System.IComparable, System.ICloneable
	{
		/// <summary> Sets the active Configuration object on this Chromosome. This method
		/// should be invoked immediately following deserialization of this
		/// Chromosome. If an active Configuration has already been set on this
		/// Chromosome, then this method will do nothing.
		/// 
		/// </summary>
		/// <param name="a_activeConfiguration">The current active Configuration object
		/// that is to be referenced internally by
		/// this Chromosome instance.
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
					}
				}
			}
			
		}
		/// <summary> Retrieves the set of genes that make up this Chromosome. This method
		/// exists primarily for the benefit of GeneticOperators that require the
		/// ability to manipulate Chromosomes at a low level.
		/// 
		/// </summary>
		/// <returns> an array of the Genes contained within this Chromosome.
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'getGenes'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
        public virtual void setGenes(Gene[] a_genes)
        {
            m_genes = a_genes;
        }
        
        virtual public Gene[] Genes
		{
			get
			{
				lock (this)
				{
					return m_genes;
				}
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions.
		/// <summary> Retrieves the fitness value of this Chromosome, as determined by the
		/// active fitness function. If a bulk fitness function is in use and
		/// has not yet assigned a fitness value to this Chromosome, then -1 is
		/// returned.
		/// 
		/// </summary>
		/// <returns> a positive integer value representing the fitness of this
		/// Chromosome, or -1 if a bulk fitness function is in use and has
		/// not yet assigned a fitness value to this Chromosome.
		/// </returns>
		/// <summary> Sets the fitness value of this Chromosome. This method is for use
		/// by bulk fitness functions and should not be invokved from anything
		/// else.
		/// 
		/// </summary>
		/// <param name="a_newFitnessValue">a positive integer representing the fitness
		/// of this Chromosome.
		/// </param>
		virtual public int FitnessValue
		{
			get
			{
				if (m_fitnessValue < 0)
				{
					// We don't have a fitness value yet. We'll see if there's a
					// "normal" fitness function configured (as opposed to a bulk
					// fitness function) and, if so, then we'll use it to determine
					// our fitness so that we can return it. First, though, we have
					// to make sure that a Configuration object has been set on this
					// Chromosome or else throw an IllegalStateException.
					// -------------------------------------------------------------
					if (m_activeConfiguration == null)
					{
						throw new System.SystemException("The active Configuration object must be set on this " + "Chromosome prior to invocation of the getFitnessValue() " + "method.");
					}
					
					// Now grab the "normal" fitness function and, if one exists,
					// ask it to calculate our fitness value.
					// ----------------------------------------------------------
					FitnessFunction normalFitnessFunction = m_activeConfiguration.getFitnessFunction();
					
					if (normalFitnessFunction != null)
					{
						m_fitnessValue = normalFitnessFunction.getFitnessValue(this, m_activeConfiguration);
					}
				}
				
				return m_fitnessValue;
			}
			
			set
			{
				if (value > 0)
				{
					m_fitnessValue = value;
				}
			}
			
		}
		/// <summary> Sets whether this Chromosome has been selected by the natural selector
		/// to continue to the next generation.
		/// 
		/// </summary>
		/// <param name="a_isSelected">true if this Chromosome has been selected, false
		/// otherwise.
		/// </param>
		virtual public bool IsSelectedForNextGeneration
		{
			set
			{
				m_isSelectedForNextGeneration = value;
			}
			
		}
		/// <summary> Retrieves whether this Chromosome has been selected by the natural
		/// selector to continue to the next generation.
		/// 
		/// </summary>
		/// <returns> true if this Chromosome has been selected, false otherwise.
		/// </returns>
		virtual public bool SelectedForNextGeneration
		{
			get
			{
				return m_isSelectedForNextGeneration;
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions.
		/// <summary> Retrieves the application-specific data that is attached to this
		/// Chromosome. Attaching application-specific data may be useful for
		/// some applications when it comes time to evaluate this Chromosome
		/// in the fitness function. JGAP ignores this data.
		/// 
		/// </summary>
		/// <returns> The application-specific data previously attached to this
		/// Chromosome, or null if there is no attached data.
		/// </returns>
		/// <summary> This sets the application-specific data that is attached to this
		/// Chromosome. Attaching application-specific data may be useful for
		/// some applications when it comes time to evaluate this Chromosome
		/// in the fitness function. JGAP ignores this data.
		/// 
		/// </summary>
		/// <param name="a_newData">The new application-specific data to attach to this
		/// Chromosome.
		/// </param>
		virtual public System.Object ApplicationData
		{
			get
			{
				return m_applicationData;
			}
			
			set
			{
				m_applicationData = value;
			}
			
		}
		/// <summary> The current active genetic configuration.</summary>
		[NonSerialized()]
		protected internal Configuration m_activeConfiguration = null;
		
		/// <summary> Application-specific data that is attached to this Chromosome.
		/// This data may assist the application in evaluating this Chromosome
		/// in the fitness function. JGAP completely ignores the data, aside
		/// from allowing it to be set and retrieved.
		/// </summary>
		private System.Object m_applicationData = null;
		
		/// <summary> The array of Genes contained in this Chromosome.</summary>
		protected internal Gene[] m_genes = null;
		
		/// <summary> Keeps track of whether or not this Chromosome has been selected by
		/// the natural selector to move on to the next generation.
		/// </summary>
		protected internal bool m_isSelectedForNextGeneration = false;
		
		/// <summary> Stores the fitness value of this Chromosome as determined by the
		/// active fitness function. A value of -1 indicates that this field
		/// has not yet been set with this Chromosome's fitness values (valid
		/// fitness values are always positive).
		/// </summary>
		protected internal int m_fitnessValue = - 1;
		
		/// <summary> Stores the hash-code of this Chromosome so that it doesn't need
		/// to be recalculated each time.
		/// </summary>
		private int m_hashCode = 0;
		
		/// <summary> Constructs a Chromosome of the given size separate from any specific
		/// Configuration. This constructor will use the given sample Gene to
		/// construct a new Chromosome instance containing genes all of the same
		/// type as the sample Gene. This can be useful for constructing sample
		/// chromosomes that use the same Gene type for all of their genes and that
		/// are to be used to setup a Configuration object.
		/// 
		/// </summary>
		/// <param name="a_sampleGene">A sample concrete Gene instance that will be used
		/// as a template for all of the genes in this
		/// Chromosome.
		/// </param>
		/// <param name="a_desiredSize">The desired size (number of genes) of this
		/// Chromosome.
		/// </param>
		public Chromosome(Gene a_sampleGene, int a_desiredSize)
		{
			// Do some sanity checking to make sure the parameters we were
			// given are valid.
			// -----------------------------------------------------------
			if (a_sampleGene == null)
			{
				throw new System.ArgumentException("Sample Gene cannot be null.");
			}
			
			if (a_desiredSize <= 0)
			{
				throw new System.ArgumentException("Chromosome size must be positive.");
			}
			
			// Create the array of Genes and populate it with new Gene
			// instances created from the sample gene.
			// -------------------------------------------------------
			m_genes = new Gene[a_desiredSize];
			for (int i = 0; i < m_genes.Length; i++)
			{
				m_genes[i] = a_sampleGene.newGene(null);
			}
			
			m_activeConfiguration = null;
		}
		
		/// <summary> Constructs a Chromosome separate from any specific Configuration. This
		/// can be useful for constructing sample chromosomes that are to be used
		/// to setup a Configuration object.
		/// 
		/// </summary>
		/// <param name="a_initialGenes">The genes of this Chromosome.
		/// </param>
		public Chromosome(Gene[] a_initialGenes)
		{
			// Sanity checks: make sure the genes array isn't null and
			// that none of the genes contained within it are null.
			// -------------------------------------------------------
			if (a_initialGenes == null)
			{
				throw new System.ArgumentException("The given array of genes cannot be null.");
			}
			
			for (int i = 0; i < a_initialGenes.Length; i++)
			{
				if (a_initialGenes[i] == null)
				{
					throw new System.ArgumentException("The gene at index " + i + " in the given array of " + "genes was found to be null. No genes in the array " + "may be null.");
				}
			}
			
			m_genes = a_initialGenes;
			m_activeConfiguration = null;
		}
		
		/// <summary> Constructs this Chromosome instance with the given array of gene values.
		/// For convenience, the randomInitialChromosome method is provided
		/// that will take care of generating a Chromosome instance of a
		/// specified size. This constructor exists in case it's desirable
		/// to initialize Chromosomes with genes that contain specific values
		/// (alleles).
		/// 
		/// </summary>
		/// <param name="a_activeConfiguration">The current, active Configuration object.
		/// </param>
		/// <param name="a_initialGenes">The array of genes that are to be contained
		/// within this Chromosome instance.
		/// 
		/// @throws InvalidConfigurationException if the given Configuration
		/// instance is null or invalid.
		/// @throws IllegalArgumentException if any of the given parameters are
		/// invalid, such as a null genes array or element.
		/// </param>
		public Chromosome(Configuration a_activeConfiguration, Gene[] a_initialGenes)
		{
			// Sanity checks: make sure the parameters are all valid.
			// ------------------------------------------------------
			if (a_initialGenes == null)
			{
				throw new System.ArgumentException("The given array of genes cannot be null.");
			}
			
			for (int i = 0; i < a_initialGenes.Length; i++)
			{
				if (a_initialGenes[i] == null)
				{
					throw new System.ArgumentException("The gene at index " + i + " in the given array of " + "genes was found to be null. No genes in the array " + "may be null.");
				}
			}
			
			if (a_activeConfiguration == null)
			{
				throw new InvalidConfigurationException("Configuration instance must not be null");
			}
			
			// Lock the configuration settings so that they can't be changed from
			// now on, and then populate our instance variables.
			// ------------------------------------------------------------------
			//a_activeConfiguration.lockSettings();
			m_activeConfiguration = a_activeConfiguration;
			
			m_genes = a_initialGenes;
		}
		
		/// <summary> Returns a copy of this Chromosome. The returned instance can evolve
		/// independently of this instance. Note that, if possible, this method
		/// will first attempt to acquire a Chromosome instance from the active
		/// ChromosomePool (if any) and set its value appropriately before
		/// returning it. If that is not possible, then a new Chromosome instance
		/// will be constructed and its value set appropriately before returning.
		/// 
		/// </summary>
		/// <returns> A copy of this Chromosome.
		/// </returns>
		//UPGRADE_TODO: The equivalent of method 'java.lang.Object.clone' is not an override method. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1143"'
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'clone'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		public System.Object Clone()
		{
			lock (this)
			{
				// Before doing anything, make sure that a Configuration object
				// has been set on this Chromosome. If not, then throw an
				// IllegalStateException.
				// ------------------------------------------------------------
				if (m_activeConfiguration == null)
				{
					throw new System.SystemException("The active Configuration object must be set on this " + "Chromosome prior to invocation of the clone() method.");
				}
				
				// Now, first see if we can pull a Chromosome from the pool and just
				// set its gene values (alleles) appropriately.
				// ------------------------------------------------------------
				ChromosomePool pool = m_activeConfiguration.ChromosomePool;
				if (pool != null)
				{
					Chromosome copy = pool.acquireChromosome();
					if (copy != null)
					{
						Gene[] genes = copy.Genes;
						for (int i = 0; i < genes.Length; i++)
						{
							genes[i].setAllele(m_genes[i].getAllele());
						}
						
						return copy;
					}
				}
				
				// If we get this far, then we couldn't fetch a Chromosome from the
				// pool, so we need to create a new one. First we make a copy of each
				// of the Genes. We explicity use the Gene at each respective gene
				// location (locus) to create the new Gene that is to occupy that same
				// locus in the new Chromosome.
				// -------------------------------------------------------------------
				Gene[] copyOfGenes = new Gene[m_genes.Length];
				
				for (int i = 0; i < copyOfGenes.Length; i++)
				{
					copyOfGenes[i] = m_genes[i].newGene(m_activeConfiguration);
					copyOfGenes[i].setAllele(m_genes[i].getAllele());
				}
				
				// Now construct a new Chromosome with the copies of the genes and
				// return it.
				// ---------------------------------------------------------------
				try
				{
					return new Chromosome(m_activeConfiguration, copyOfGenes);
				}
				catch (InvalidConfigurationException )
				{
					// This should never happen because the configuration has already
					// been validated and locked for this instance of the Chromosome,
					// and the configuration given to the new instance should be
					// identical.
					// --------------------------------------------------------------
					throw new System.SystemException("Fatal Error: clone method produced an " + "InvalidConfigurationException. This should never happen." + "Please report this as a bug to the JGAP team.");
				}
			}
		}
		
		/// <summary> Returns the Gene at the given index (locus) within the Chromosome. The
		/// first gene is at index zero and the last gene is at the index equal to
		/// the size of this Chromosome - 1.
		/// 
		/// </summary>
		/// <param name="a_desiredLocus:">The index of the gene value to be returned.
		/// </param>
		/// <returns> The Gene at the given index.
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'getGene'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual Gene getGene(int a_desiredLocus)
		{
			lock (this)
			{
				return m_genes[a_desiredLocus];
			}
		}
		
		/// <summary> Returns the size of this Chromosome (the number of genes it contains).
		/// A Chromosome's size is constant and will never change.
		/// 
		/// </summary>
		/// <returns> The number of genes contained within this Chromosome instance.
		/// </returns>
		public virtual int size()
		{
			return m_genes.Length;
		}
		
		/// <summary> Returns a string representation of this Chromosome, useful
		/// for some display purposes.
		/// 
		/// </summary>
		/// <returns> A string representation of this Chromosome.
		/// </returns>
		public override System.String ToString()
		{
			System.Text.StringBuilder representation = new System.Text.StringBuilder();
			representation.Append("[ ");
			
			// Append the representations of each of the gene Alleles.
			// -------------------------------------------------------
			for (int i = 0; i < m_genes.Length - 1; i++)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				representation.Append(m_genes[i].ToString());
				representation.Append(", ");
			}
			//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			representation.Append(m_genes[m_genes.Length - 1].ToString());
			representation.Append(" ]");
			
			return representation.ToString();
		}
		
		/// <summary> Convenience method that returns a new Chromosome instance with its
		/// genes values (alleles) randomized. Note that, if possible, this method
		/// will acquire a Chromosome instance from the active ChromosomePool
		/// (if any) and then randomize its gene values before returning it. If a
		/// Chromosome cannot be acquired from the pool, then a new instance will
		/// be constructed and its gene values randomized before returning it.
		/// 
		/// </summary>
		/// <param name="a_activeConfiguration">The current active configuration.
		/// 
		/// @throws InvalidConfigurationException if the given Configuration
		/// instance is invalid.
		/// @throws IllegalArgumentException if the given Configuration instance
		/// is null.
		/// </param>
		public static Chromosome randomInitialChromosome(Configuration a_activeConfiguration)
		{
			// Sanity check: make sure the given configuration isn't null.
			// -----------------------------------------------------------
			if (a_activeConfiguration == null)
			{
				throw new System.ArgumentException("Configuration instance must not be null");
			}
			
			// Lock the configuration settings so that they can't be changed
			// from now on.
			// -------------------------------------------------------------
			a_activeConfiguration.lockSettings();
			
			// First see if we can get a Chromosome instance from the pool.
			// If we can, we'll randomize its gene values (alleles) and then
			// return it.
			// ------------------------------------------------------------
			ChromosomePool pool = a_activeConfiguration.ChromosomePool;
			if (pool != null)
			{
				Chromosome randomChromosome = pool.acquireChromosome();
				if (randomChromosome != null)
				{
					Gene[] genes = randomChromosome.Genes;
					RandomGenerator generator = a_activeConfiguration.getRandomGenerator();
					
					for (int i = 0; i < genes.Length; i++)
					{
						genes[i].setToRandomValue(generator);
					}
					
					return randomChromosome;
				}
			}
			
			// If we got this far, then we weren't able to get a Chromosome from
			// the pool, so we have to construct a new instance and build it from
			// scratch.
			// ------------------------------------------------------------------
			Chromosome sampleChromosome = a_activeConfiguration.SampleChromosome;
			
			Gene[] sampleGenes = sampleChromosome.Genes;
			
			Gene[] newGenes = new Gene[sampleGenes.Length];
			RandomGenerator generator2 = a_activeConfiguration.getRandomGenerator();
			
			for (int i = 0; i < newGenes.Length; i++)
			{
				// We use the newGene() method on each of the genes in the
				// sample Chromosome to generate our new Gene instances for
				// the Chromosome we're returning. This guarantees that the
				// new Genes are setup with all of the correct internal state
				// for the respective gene position they're going to inhabit.
				// -----------------------------------------------------------
				newGenes[i] = sampleGenes[i].newGene(a_activeConfiguration);
				
				// Set the gene's value (allele) to a random value.
				// ------------------------------------------------
				newGenes[i].setToRandomValue(generator2);
			}
			
			// Finally, construct the new chromosome with the new random
			// genes values and return it.
			// ---------------------------------------------------------
			return new Chromosome(a_activeConfiguration, newGenes);
		}
		
		/// <summary> Compares this Chromosome against the specified object. The result is
		/// true if and the argument is an instance of the Chromosome class
		/// and has a set of genes equal to this one.
		/// 
		/// </summary>
		/// <param name="other">The object to compare against.
		/// </param>
		/// <returns> true if the objects are the same, false otherwise.
		/// </returns>
		public  override bool Equals(System.Object other)
		{
			// If class is not equal, return false. If we would not do this here,
			// a problem arises in the compareTo-method which only works correctly
			// with chromosomes of the same class
			// ------------------------------------------------------------------
			if (other != null && !this.GetType().FullName.Equals(other.GetType().FullName))
			{
				return false;
			}
			try
			{
				return CompareTo(other) == 0;
			}
			catch (System.InvalidCastException )
			{
				return false;
			}
		}
		
		/// <summary> Retrieve a hash code for this Chromosome.
		/// 
		/// </summary>
		/// <returns> the hash code of this Chromosome.
		/// </returns>
		public override int GetHashCode()
		{
			// Take the hash codes of the genes and XOR them all together.
			// I'm not really sure how effective this is, but I notice that's
			// what the java.lang.Long class does (albeit with only two
			// hashcode values).
			// --------------------------------------------------------------
			if (m_hashCode == 0)
			{
				for (int i = 0; i < m_genes.Length; i++)
				{
					m_hashCode ^= m_genes[i].GetHashCode();
				}
			}
			
			return m_hashCode;
		}
		
		/// <summary> Compares the given Chromosome to this Chromosome. This chromosome is
		/// considered to be "less than" the given chromosome if it has a fewer
		/// number of genes or if any of its gene values (alleles) are less than
		/// their corresponding gene values in the other chromosome.
		/// 
		/// </summary>
		/// <param name="other">The Chromosome against which to compare this chromosome.
		/// </param>
		/// <returns> a negative number if this chromosome is "less than" the given
		/// chromosome, zero if they are equal to each other, and a positive
		/// number if this chromosome is "greater than" the given chromosome.
		/// </returns>
		public virtual int CompareTo(System.Object other)
		{
			// First, if the other Chromosome is null, then this chromosome is
			// automatically the "greater" Chromosome.
			// ---------------------------------------------------------------
			if (other == null)
			{
				return 1;
			}
			
			Chromosome otherChromosome = (Chromosome) other;
			Gene[] otherGenes = otherChromosome.m_genes;
			
			// If the other Chromosome doesn't have the same number of genes,
			// then whichever has more is the "greater" Chromosome.
			// --------------------------------------------------------------
			if (otherGenes.Length != m_genes.Length)
			{
				return m_genes.Length - otherGenes.Length;
			}
			
			// Next, compare the gene values (alleles) for differences. If
			// one of the genes is not equal, then we return the result of its
			// comparison.
			// ---------------------------------------------------------------
			for (int i = 0; i < m_genes.Length; i++)
			{
				int comparison = m_genes[i].CompareTo(otherGenes[i]);
				
				if (comparison != 0)
				{
					return comparison;
				}
			}
			
			// Everything is equal. Return zero.
			// ---------------------------------
			return 0;
		}
		
		/// <summary> Invoked when this Chromosome is no longer needed and should perform
		/// any necessary cleanup. Note that this method will attempt to release
		/// this Chromosome instance to the active ChromosomePool, if any.
		/// </summary>
		public virtual void  cleanup()
		{
			// First, reset our internal state.
			// --------------------------------
			m_fitnessValue = - 1;
			m_hashCode = 0;
			m_isSelectedForNextGeneration = false;
			
			// Next we want to try to release this Chromosome to a ChromosomePool
			// if one has been setup so that we can save a little time and memory
			// next time a Chromosome is needed. Before trying this, however, we
			// have to make sure the active Configuration has been set on this
			// Chromosome or else throw an IllegalStateException.
			// ------------------------------------------------------------------
			if (m_activeConfiguration == null)
			{
				throw new System.SystemException("The active Configuration object must be set on this " + "Chromosome prior to invocation of the cleanup() method.");
			}
			
			// Now fetch the active ChromosomePool from the Configuration object
			// and, if the pool exists, release this Chromosome to it.
			// -----------------------------------------------------------------
			ChromosomePool pool = m_activeConfiguration.ChromosomePool;
			
			if (pool != null)
			{
				// Note that the pool will take care of any gene cleanup for us,
				// so we don't need to worry about it here.
				// -------------------------------------------------------------
				pool.releaseChromosome(this);
			}
			else
			{
				// No pool is available, so we need to finish cleaning up, which
				// basically entails requesting each of our genes to clean
				// themselves up as well.
				// -------------------------------------------------------------
				for (int i = 0; i < m_genes.Length; i++)
				{
					m_genes[i].cleanup();
				}
			}
		}
	}
}