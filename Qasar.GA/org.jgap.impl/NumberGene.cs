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
using Gene = org.jgap.Gene;
namespace org.jgap.impl
{
	
	/// <summary> Base class for all Genes based on numbers.
	/// Known implementations: IntegerGene, DoubleGene
	/// 
	/// </summary>
	/// <author>  Klaus Meffert
	/// @since 1.1 (most code moved and adapted from IntegerGene)
	/// </author>
	[Serializable]
	public abstract class NumberGene : Gene
	{
		public abstract System.String getPersistentRepresentation();

		/// <summary> Sets the value of this Gene to a random legal value for the
		/// implementation. This method exists for the benefit of mutation and other
		/// operations that simply desire to randomize the value of a gene.
		/// 
		/// </summary>
		/// <param name="a_numberGenerator">The random number generator that should be
		/// used to create any random values. It's important
		/// to use this generator to maintain the user's
		/// flexibility to configure the genetic engine
		/// to use the random number generator of their
		/// choice.
		/// </param>
		public abstract void setToRandomValue(RandomGenerator a_numberGenerator);

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
		public abstract void setValueFromPersistentRepresentation(System.String a_representation);

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
		public abstract Gene newGene(Configuration a_activeConfiguration);

		/// <summary> References the internal value (allele) of this Gene
		/// E.g., for DoubleGene this is of type Double
		/// </summary>
		protected internal System.Object m_value = null;
		
		/// <summary> Executed by the genetic engine when this Gene instance is no
		/// longer needed and should perform any necessary resource cleanup.
		/// </summary>
		public virtual void  cleanup()
		{
			// No specific cleanup is necessary for this implementation.
			// ---------------------------------------------------------
		}
		
		/// <summary> Compares this IntegerGene with the given object and returns true if
		/// the other object is a IntegerGene and has the same value (allele) as
		/// this IntegerGene. Otherwise it returns false.
		/// 
		/// </summary>
		/// <param name="other">the object to compare to this IntegerGene for equality.
		/// </param>
		/// <returns> true if this Gene is equal to the given object,
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
				// If the other object isn't an Gene of current type
				// (like IntegerGene), then we're not equal.
				// -------------------------------------------------
				return false;
			}
		}
		
		/// <summary> Retrieves the hash code value for this IntegerGene.
		/// 
		/// </summary>
		/// <returns> this IntegerGene's hash code.
		/// </returns>
		public override int GetHashCode()
		{
			// If our internal Integer is null, then return zero. Otherwise,
			// just return the hash code of the Integer.
			// -------------------------------------------------------------
			if (m_value == null)
			{
				return 0;
			}
			else
			{
				return m_value.GetHashCode();
			}
		}
		
		/// <summary> Compares this IntegerGene with the specified object (which must also
		/// be an IntegerGene) for order, which is determined by the integer
		/// value of this Gene compared to the one provided for comparison.
		/// 
		/// </summary>
		/// <param name="other">the IntegerGene to be compared to this IntegerGene.
		/// </param>
		/// <returns> a negative integer, zero, or a positive integer as this object
		/// is less than, equal to, or greater than the object provided for
		/// comparison.
		/// 
		/// @throws ClassCastException if the specified object's type prevents it
		/// from being compared to this IntegerGene.
		/// </returns>
		public virtual int CompareTo(System.Object other)
		{
			NumberGene otherGene = (NumberGene) other;
			
			// First, if the other gene (or its value) is null, then this is
			// the greater allele. Otherwise, just use the Integer's compareTo
			// method to perform the comparison.
			// ---------------------------------------------------------------
			if (otherGene == null)
			{
				return 1;
			}
			else if (otherGene.m_value == null)
			{
				// If our value is also null, then we're the same. Otherwise,
				// this is the greater gene.
				// ----------------------------------------------------------
				return m_value == null?0:1;
			}
			else
			{
				try
				{
					return compareToNative(m_value, otherGene.m_value);
				}
				catch (System.InvalidCastException e)
				{
					SupportClass.WriteStackTrace(e, Console.Error);
					throw e;
				}
			}
		}
		
		/// <summary> Compares to objects by first casting them into their expected type
		/// (e.g. Integer for IntegerGene) and then calling the compareTo-method
		/// of the casted type.
		/// </summary>
		/// <param name="o1">first object to be compared, always is not null
		/// </param>
		/// <param name="o2">second object to be compared, always is not null
		/// </param>
		/// <returns> a negative integer, zero, or a positive integer as this object
		/// is less than, equal to, or greater than the object provided for
		/// comparison.
		/// </returns>
		protected internal abstract int compareToNative(System.Object o1, System.Object o2);
		
		/// <summary> Retrieves a string representation of this IntegerGene's value that
		/// may be useful for display purposes.
		/// 
		/// </summary>
		/// <returns> a string representation of this IntegerGene's value.
		/// </returns>
		public override System.String ToString()
		{
			if (m_value == null)
			{
				return "null";
			}
			else
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				return m_value.ToString();
			}
		}
		
		/// <summary> Sets the value (allele) of this Gene to the new given value. This class
		/// expects the value to be an instance of current type (e.g. Integer).
		/// If the value is above or below the upper or lower bounds, it will be
		/// mappped to within the allowable range.
		/// 
		/// </summary>
		/// <param name="a_newValue">the new value of this Gene instance.
		/// </param>
		public virtual void  setAllele(System.Object a_newValue)
		{
			m_value = a_newValue;
			
			// If the value isn't between the upper and lower bounds of this
			// Gene, map it to a value within those bounds.
			// -------------------------------------------------------------
			mapValueToWithinBounds();
		}
		
		/// <summary> Retrieves the value (allele) represented by this Gene. All values
		/// returned by this class will be Integer instances.
		/// 
		/// </summary>
		/// <returns> the Integer value of this Gene.
		/// </returns>
		public virtual System.Object getAllele()
		{
			return m_value;
		}
		
		/// <returns> the size of the gene, i.e the number of atomic elements.
		/// Always 1 for numbers
		/// 
		/// @since 1.1
		/// </returns>
		public virtual int size()
		{
			return 1;
		}
		
		/// <summary> Maps the value of this IntegerGene to within the bounds specified by
		/// the m_upperBounds and m_lowerBounds instance variables. The value's
		/// relative position within the integer range will be preserved within the
		/// bounds range (in other words, if the value is about halfway between the
		/// integer max and min, then the resulting value will be about halfway
		/// between the upper bounds and lower bounds). If the value is null or
		/// is already within the bounds, it will be left unchanged.
		/// </summary>
		protected internal abstract void  mapValueToWithinBounds();
		
		/// <summary> Applies a mutation of a given intensity (percentage) onto the atomic
		/// element at given index (NumberGenes only have one atomic element)
		/// </summary>
		/// <param name="index">index of atomic element, between 0 and size()-1
		/// </param>
		/// <param name="a_percentage">percentage of mutation (greater than -1 and smaller
		/// than 1).
		/// </param>
		public abstract void  applyMutation(int index, double a_percentage);
	}
}