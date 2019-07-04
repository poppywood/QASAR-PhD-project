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
using Configuration = org.jgap.Configuration;
using Gene = org.jgap.Gene;
using RandomGenerator = org.jgap.RandomGenerator;
using UnsupportedRepresentationException = org.jgap.UnsupportedRepresentationException;
namespace org.jgap.impl
{
	
	/// <summary> A Gene implementation that supports a double values for its allele.
	/// Upper and lower bounds may optionally be provided to restrict the range
	/// of legal values allowed by this Gene instance.
	/// Partly copied from IntegerGene.
	/// 
	/// </summary>
	/// <author>  Klaus Meffert
	/// @since 1.1
	/// </author>
	public class DoubleGene : NumberGene, Gene
	{
		/// <summary> Represents the constant range of values supported by doubles.</summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'DOUBLE_RANGE '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003"'
		//UPGRADE_TODO: The equivalent in .NET for field 'java.lang.Double.MAX_VALUE' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
		//UPGRADE_TODO: The equivalent in .NET for field 'java.lang.Double.MIN_VALUE' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
		protected internal static readonly double DOUBLE_RANGE = System.Double.MaxValue - System.Double.MinValue;
		
		/// <summary> The upper bounds of values represented by this Gene. If not explicitly
		/// provided by the user, this should be set to Double.MAX_VALUE.
		/// </summary>
		protected internal double m_upperBounds;
		
		/// <summary> The lower bounds of values represented by this Gene. If not explicitly
		/// provided by the user, this should be set to Double.MIN_VALUE
		/// </summary>
		protected internal double m_lowerBounds;
		
		/// <summary> Stores the number of double range units that a single bounds-range
		/// unit represents. For example, if the double range is -2 billion to
		/// +2 billion and the bounds range is -1 billion to +1 billion, then
		/// each unit in the bounds range would map to 2 units in the double
		/// range. The value of this variable would therefore be 2. This mapping
		/// unit is used to map illegal allele values that are outside of the
		/// bounds to legal allele values that are within the bounds.
		/// </summary>
		protected internal double m_boundsUnitsToDoubleUnits;
		
		/// <summary> Constructs a new DoubleGene with default settings. No bounds will
		/// be put into effect for values (alleles) of this Gene instance, other
		/// than the standard range of double values.
		/// </summary>
		public DoubleGene()
		{
			//UPGRADE_TODO: The equivalent in .NET for field 'java.lang.Double.MIN_VALUE' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			m_lowerBounds = System.Double.MinValue;
			//UPGRADE_TODO: The equivalent in .NET for field 'java.lang.Double.MAX_VALUE' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			m_upperBounds = System.Double.MaxValue;
			calculateBoundsUnitsToDoubleUnitsRatio();
		}
		
		/// <summary> Constructs a new DoubleGene with the specified lower and upper
		/// bounds for values (alleles) of this Gene instance.
		/// 
		/// </summary>
		/// <param name="a_lowerBounds">The lowest value that this Gene may possess,
		/// inclusive.
		/// </param>
		/// <param name="a_upperBounds">The highest value that this Gene may possess,
		/// inclusive.
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public DoubleGene(double a_lowerBounds, double a_upperBounds)
		{
			m_lowerBounds = a_lowerBounds;
			m_upperBounds = a_upperBounds;
			calculateBoundsUnitsToDoubleUnitsRatio();
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
		/// <param name="a_activeConfiguration">ignored here.
		/// </param>
		/// <returns> A new Gene instance of the same type and with the same
		/// setup as this concrete Gene.
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public override Gene newGene(Configuration a_activeConfiguration)
		{
			return new DoubleGene(m_lowerBounds, m_upperBounds);
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
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public override System.String getPersistentRepresentation()
		{
			// The persistent representation includes the value, lower bound,
			// and upper bound. Each is separated by a colon.
			// --------------------------------------------------------------
			//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			return ToString() + org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER + m_lowerBounds + org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER + m_upperBounds;
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
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public override void  setValueFromPersistentRepresentation(System.String a_representation)
		{
			if ((System.Object) a_representation != null)
			{
				SupportClass.Tokenizer tokenizer = new SupportClass.Tokenizer(a_representation, org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER);
				// Make sure the representation contains the correct number of
				// fields. If not, throw an exception.
				// -----------------------------------------------------------
				if (tokenizer.Count != 3)
				{
					throw new UnsupportedRepresentationException("The format of the given persistent representation " + "is not recognized: it does not contain three tokens.");
				}
				System.String valueRepresentation = tokenizer.NextToken();
				System.String lowerBoundRepresentation = tokenizer.NextToken();
				System.String upperBoundRepresentation = tokenizer.NextToken();
				// First parse and set the representation of the value.
				// ----------------------------------------------------
				if (valueRepresentation.Equals("null"))
				{
					m_value = null;
				}
				else
				{
					try
					{
						m_value = System.Double.Parse(valueRepresentation);
					}
					catch (System.FormatException)
					{
						throw new UnsupportedRepresentationException("The format of the given persistent representation " + "is not recognized: field 1 does not appear to be " + "a double value.");
					}
				}
				// Now parse and set the lower bound.
				// ----------------------------------
				try
				{
					m_lowerBounds = System.Double.Parse(lowerBoundRepresentation);
				}
				catch (System.FormatException )
				{
					throw new UnsupportedRepresentationException("The format of the given persistent representation " + "is not recognized: field 2 does not appear to be " + "a double value.");
				}
				// Now parse and set the upper bound.
				// ----------------------------------
				try
				{
					m_upperBounds = System.Double.Parse(upperBoundRepresentation);
				}
				catch (System.FormatException )
				{
					throw new UnsupportedRepresentationException("The format of the given persistent representation " + "is not recognized: field 3 does not appear to be " + "a double value.");
				}
				// We need to recalculate the bounds units to double units
				// ratio since our lower and upper bounds have probably just
				// been changed.
				// -------------------------------------------------------------
				calculateBoundsUnitsToDoubleUnitsRatio();
			}
		}
		
		/// <summary> Retrieves the double value of this Gene, which may be more convenient in
		/// some cases than the more general getAllele() method.
		/// 
		/// </summary>
		/// <returns> the double value of this Gene.
		/// </returns>
		public virtual double doubleValue()
		{
			return ((System.Double) m_value);
		}
		
		/// <summary> Sets the value (allele) of this Gene to a random Double value between
		/// the lower and upper bounds (if any) of this Gene.
		/// 
		/// </summary>
		/// <param name="a_numberGenerator">The random number generator that should be
		/// used to create any random values. It's important
		/// to use this generator to maintain the user's
		/// flexibility to configure the genetic engine
		/// to use the random number generator of their
		/// choice.
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public override void  setToRandomValue(RandomGenerator a_numberGenerator)
		{
			m_value = a_numberGenerator.nextDouble();
			// If the value isn't between the upper and lower bounds of this
			// DoubleGene, map it to a value within those bounds.
			// -------------------------------------------------------------
			mapValueToWithinBounds();
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
		protected internal override int compareToNative(System.Object o1, System.Object o2)
		{
			return ((System.Double) o1).CompareTo(o2);
		}
		
		/// <summary> Maps the value of this DoubleGene to within the bounds specified by
		/// the m_upperBounds and m_lowerBounds instance variables. The value's
		/// relative position within the double range will be preserved within the
		/// bounds range (in other words, if the value is about halfway between the
		/// double max and min, then the resulting value will be about halfway
		/// between the upper bounds and lower bounds). If the value is null or
		/// is already within the bounds, it will be left unchanged.
		/// 
		/// </summary>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		protected internal override void  mapValueToWithinBounds()
		{
			if (m_value != null)
			{
				System.Double d_value = ((System.Double) m_value);
				// If the value exceeds either the upper or lower bounds, then
				// map the value to within the legal range. To do this, we basically
				// calculate the distance between the value and the double min,
				// determine how many bounds units that represents, and then add
				// that number of units to the upper bound.
				// -----------------------------------------------------------------
				if (d_value > m_upperBounds || d_value < m_lowerBounds)
				{
					//UPGRADE_TODO: The equivalent in .NET for field 'java.lang.Double.MIN_VALUE' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
					double differenceFromDoubleMin = System.Double.MinValue + d_value;
					double differenceFromBoundsMin = (differenceFromDoubleMin / m_boundsUnitsToDoubleUnits);
					m_value = m_upperBounds + differenceFromBoundsMin;
				}
			}
		}
		
		/// <summary> Calculates and sets the m_boundsUnitsToDoubleUnits field based
		/// on the current lower and upper bounds of this DoubleGene. For example,
		/// if the double range is -2 billion to +2 billion and the bounds range
		/// is -1 billion to +1 billion, then each unit in the bounds range would
		/// map to 2 units in the double range. The m_boundsUnitsToDoubleUnits
		/// field would therefore be 2. This mapping unit is used to map illegal
		/// allele values that are outside of the bounds to legal allele values that
		/// are within the bounds.
		/// 
		/// </summary>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		protected internal virtual void  calculateBoundsUnitsToDoubleUnitsRatio()
		{
			double divisor = m_upperBounds - m_lowerBounds + 1.0d;
			if (divisor == 0)
			{
				m_boundsUnitsToDoubleUnits = DOUBLE_RANGE;
			}
			else
			{
				m_boundsUnitsToDoubleUnits = DOUBLE_RANGE / divisor;
			}
		}
		
		/// <summary> See interface Gene for description</summary>
		/// <param name="index">must always be 1 (because there is only 1 atomic element)
		/// </param>
		/// <param name="a_percentage">percentage of mutation (greater than -1 and smaller
		/// than 1).
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public override void  applyMutation(int index, double a_percentage)
		{
			double newValue = doubleValue() * (1 + a_percentage);
			setAllele((System.Object) (newValue));
		}
	}
}