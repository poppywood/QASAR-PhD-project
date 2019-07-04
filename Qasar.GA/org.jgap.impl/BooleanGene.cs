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
using Configuration = org.jgap.Configuration;
using Gene = org.jgap.Gene;
using RandomGenerator = org.jgap.RandomGenerator;
using UnsupportedRepresentationException = org.jgap.UnsupportedRepresentationException;
namespace org.jgap.impl
{
	
	/// <summary> A Gene implementation that supports two possible values (alleles) for each
	/// gene: true and false.
	/// <p>
	/// NOTE: Since this Gene implementation only supports two different
	/// values (true and false), there's only a 50% chance that invocation
	/// of the setToRandomValue() method will actually change the value of
	/// this Gene (if it has a value). As a result, it may be desirable to
	/// use a higher overall mutation rate when this Gene implementation
	/// is in use.
	/// 
	/// </summary>
	/// <author>  Neil Rotstan
	/// @since 1.0
	/// </author>
	[Serializable]
	public class BooleanGene : Gene
	{
		/// <summary> Shared constant representing the "true" boolean value. Shared constants
		/// are used to save memory so that a new Boolean object doesn't have to
		/// be constructed each time.
		/// </summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'TRUE_BOOLEAN '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003"'
		protected internal static readonly System.Boolean TRUE_BOOLEAN = true;
		
		/// <summary> Shared constant representing the "false" boolean value. Shared constants
		/// are used to save memory so that a new Boolean object doesn't have to
		/// be constructed each time.
		/// </summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'FALSE_BOOLEAN '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003"'
		protected internal static readonly System.Boolean FALSE_BOOLEAN = false;

		public static readonly System.Boolean INVALID_BOOLEAN = new Boolean();
		
		/// <summary> References the internal boolean value of this Gene.</summary>
		protected internal System.Boolean m_value = INVALID_BOOLEAN;
		
		/// <summary> Constructs a new BooleanGene with default settings.</summary>
		public BooleanGene()
		{
		}
		
		/// <summary> Provides an implementation-independent means for creating new Gene
		/// instances. The new instance that is created and returned should be
		/// setup with any implementation-dependent configuration that this Gene
		/// instance is setup with (aside from the actual value, of course). For
		/// example, if this Gene were setup with bounds on its value, then the
		/// Gene instance returned from this method should also be setup with
		/// those same bounds. This is important, as the JGAP core will invoke this
		/// method on each Gene in the sample Chromosome in order to create each
		/// new Gene in the same respective gene position for a new Chromosome.
		/// <p>
		/// It should be noted that nothing is guaranteed about the actual value
		/// of the returned Gene and it should therefore be considered to be
		/// undefined.
		/// 
		/// </summary>
		/// <param name="a_activeConfiguration">The current active configuration.
		/// </param>
		/// <returns> A new Gene instance of the same type and with the same
		/// setup as this concrete Gene.
		/// </returns>
		public virtual Gene newGene(Configuration a_activeConfiguration)
		{
			return new BooleanGene();
		}
		
		/// <summary> Sets the value of this Gene to the new given value. This class
		/// expects the value to be a Boolean instance.
		/// 
		/// </summary>
		/// <param name="a_newValue">the new value of this Gene instance.
		/// </param>
		public virtual void  setAllele(System.Object a_newValue)
		{
			if (a_newValue != null)
				m_value = (System.Boolean) a_newValue;
			else
				m_value = INVALID_BOOLEAN;
		}
		
		/// <summary> Retrieves a string representation of this Gene that includes any
		/// information required to reconstruct it at a later time, such as its
		/// value and internal state. This string will be used to represent this
		/// Gene in XML persistence. This is an optional method but, if not
		/// implemented, XML persistence and possibly other features will not be
		/// available. An UnsupportedOperationException should be thrown if no
		/// implementation is provided.
		/// 
		/// </summary>
		/// <returns> A string representation of this Gene's current state.
		/// @throws UnsupportedOperationException to indicate that no implementation
		/// is provided for this method.
		/// </returns>
		public virtual System.String getPersistentRepresentation()
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			return ToString();
		}
		
		/// <summary> Sets the value and internal state of this Gene from the string
		/// representation returned by a previous invocation of the
		/// getPersistentRepresentation() method. This is an optional method but,
		/// if not implemented, XML persistence and possibly other features will not
		/// be available. An UnsupportedOperationException should be thrown if no
		/// implementation is provided.
		/// 
		/// </summary>
		/// <param name="a_representation">the string representation retrieved from a
		/// prior call to the getPersistentRepresentation()
		/// method.
		/// 
		/// @throws UnsupportedOperationException to indicate that no implementation
		/// is provided for this method.
		/// @throws UnsupportedRepresentationException if this Gene implementation
		/// does not support the given string representation.
		/// </param>
		public virtual void  setValueFromPersistentRepresentation(System.String a_representation)
		{
			if ((System.Object) a_representation != null)
			{
				if (a_representation.Equals("null"))
				{
					m_value = INVALID_BOOLEAN;
				}
				else if (a_representation.ToUpper().Equals("TRUE"))
				{
					m_value = TRUE_BOOLEAN;
				}
				else if (a_representation.ToUpper().Equals("FALSE"))
				{
					m_value = FALSE_BOOLEAN;
				}
				else
				{
					throw new UnsupportedRepresentationException("Unknown boolean gene representation: " + a_representation);
				}
			}
		}
		
		/// <summary> Retrieves the value represented by this Gene. All values returned
		/// by this class will be Boolean instances.
		/// 
		/// </summary>
		/// <returns> the Boolean value of this Gene.
		/// </returns>
		public virtual System.Object getAllele()
		{
			return m_value;
		}
		
		/// <summary> Retrieves the boolean value of this Gene. This may be more convenient
		/// in some cases than the more general getAllele() method.
		/// 
		/// </summary>
		/// <returns> the boolean value of this Gene.
		/// </returns>
		public virtual bool booleanValue()
		{
			return m_value;
		}
		
		/// <summary> Sets the value (allele) of this Gene to a random legal value. This
		/// method exists for the benefit of mutation and other operations that
		/// simply desire to randomize the value of a gene.
		/// <p>
		/// NOTE: Since this Gene implementation only supports two different
		/// values (true and false), there's only a 50% chance that invocation
		/// of this method will actually change the value of this Gene (if
		/// it has a value). As a result, it may be desirable to use a higher
		/// overall mutation rate when this Gene implementation is in use.
		/// 
		/// </summary>
		/// <param name="a_numberGenerator">The random number generator that should be
		/// used to create any random values. It's important
		/// to use this generator to maintain the user's
		/// flexibility to configure the genetic engine
		/// to use the random number generator of their
		/// choice.
		/// </param>
		public virtual void  setToRandomValue(RandomGenerator a_numberGenerator)
		{
			if (a_numberGenerator.nextBoolean() == true)
			{
				m_value = TRUE_BOOLEAN;
			}
			else
			{
				m_value = FALSE_BOOLEAN;
			}
		}
		
		/// <summary> Compares this BooleanGene with the specified object for order. A
		/// false value is considered to be less than a true value. A null value
		/// is considered to be less than any non-null value.
		/// 
		/// </summary>
		/// <param name="other">the BooleanGene to be compared.
		/// </param>
		/// <returns>  a negative integer, zero, or a positive integer as this object
		/// is less than, equal to, or greater than the specified object.
		/// 
		/// @throws ClassCastException if the specified object's type prevents it
		/// from being compared to this BooleanGene.
		/// </returns>
		public virtual int CompareTo(System.Object other)
		{
			BooleanGene otherBooleanGene = (BooleanGene) other;
			
			// First, if the other gene is null, then this is the greater gene.
			// ----------------------------------------------------------------
			if (otherBooleanGene == null)
			{
				return 1;
			}
			else if ((System.Object) otherBooleanGene.m_value == null)
			{
				// If our value is also null, then we're the same. Otherwise,
				// we're the greater gene.
				// ----------------------------------------------------------
				return (System.Object) m_value == null?0:1;
			}
			
			// The Boolean class doesn't implement the Comparable interface, so
			// we have to do the comparison ourselves.
			// ----------------------------------------------------------------
			if (m_value == false)
			{
				if (otherBooleanGene.m_value == false)
				{
					// Both are false and therefore the same. Return zero.
					// ---------------------------------------------------
					return 0;
				}
				else
				{
					// This allele is false, but the other one is true. This
					// allele is the lesser.
					// -----------------------------------------------------
					return - 1;
				}
			}
			else if (otherBooleanGene.m_value == true)
			{
				// Both alleles are true and therefore the same. Return zero.
				// ----------------------------------------------------------
				return 0;
			}
			else
			{
				// This allele is true, but the other is false. This allele is
				// the greater.
				// -----------------------------------------------------------
				return 1;
			}
		}
		
		/// <summary> Compares this BooleanGene with the given object and returns true if
		/// the other object is a BooleanGene and has the same value as this
		/// BooleanGene. Otherwise it returns false.
		/// 
		/// </summary>
		/// <param name="other">the object to compare to this BooleanGene for equality.
		/// </param>
		/// <returns> true if this BooleanGene is equal to the given object,
		/// false otherwise.
		/// </returns>
		public  override bool Equals(System.Object other)
		{
			try
			{
				return CompareTo(other) == 0;
			}
			catch (System.InvalidCastException )
			{
				// If the other object isn't a BooleanGene, then we're not equal.
				// --------------------------------------------------------------
				return false;
			}
		}
		
		/// <summary> Retrieves the hash code value of this BooleanGene.
		/// 
		/// </summary>
		/// <returns> this BooleanGene's hash code.
		/// </returns>
		public override int GetHashCode()
		{
			// If the internal Boolean hasn't been set, return zero. Otherwise,
			// just return the Boolean's hash code.
			// ----------------------------------------------------------------
			if ((System.Object) m_value == null)
			{
				return 0;
			}
			else
			{
				return m_value.GetHashCode();
			}
		}
		
		/// <summary> Retrieves a string representation of this BooleanGene's value that
		/// may be useful for display purposes.
		/// 
		/// </summary>
		/// <returns> a string representation of this BooleanGene's value.
		/// </returns>
		public override System.String ToString()
		{
			if ((System.Object) m_value == null)
			{
				return "null";
			}
			else
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Boolean.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				return m_value.ToString();
			}
		}
		
		/// <summary> Executed by the genetic engine when this Gene instance is no
		/// longer needed and should perform any necessary resource cleanup.
		/// </summary>
		public virtual void  cleanup()
		{
			// No specific cleanup is necessary for this implementation.
			// ---------------------------------------------------------
		}
		
		/// <returns> the size of the gene, i.e the number of atomic elements.
		/// Always 1 for BooleanGene
		/// 
		/// @since 1.1
		/// </returns>
		public virtual int size()
		{
			return 1;
		}
		
		/// <summary> Applies a mutation of a given intensity (percentage) onto the atomic
		/// element at given index (NumberGenes only have one atomic element)
		/// </summary>
		/// <param name="index">index of atomic element, between 0 and size()-1
		/// </param>
		/// <param name="a_percentage">percentage of mutation (greater than -1 and smaller
		/// than 1).
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual void  applyMutation(int index, double a_percentage)
		{
			if (a_percentage >= 0)
			{
				// change to TRUE
				// ---------------
				if (!m_value)
				{
					m_value = true;
				}
			}
			else
			{
				// change to FALSE
				// ---------------
				if (m_value)
				{
					m_value = false;
				}
			}
		}
	}
}