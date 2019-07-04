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
* 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/
using System;
using Configuration = org.jgap.Configuration;
using Gene = org.jgap.Gene;
using RandomGenerator = org.jgap.RandomGenerator;
using UnsupportedRepresentationException = org.jgap.UnsupportedRepresentationException;
namespace org.jgap.impl
{
	
	/// <summary> A Gene implementation that supports a string for its allele. The valid
	/// alphabet as well as the minimum and maximum length of the string can be specified.
	/// An alphabet == null indicates that all characters are seen to be valid.
	/// An alphabet == "" indicates that no character is seen to be valid.
	/// Partly copied from IntegerGene.
	/// 
	/// </summary>
	/// <author>  Klaus Meffert
	/// @since 1.1
	/// </author>
	[Serializable]
	public class StringGene : Gene
	{
		virtual public int MaxLength
		{
			get
			{
				return m_maxLength;
			}
			
			set
			{
				this.m_maxLength = value;
			}
			
		}
		virtual public int MinLength
		{
			get
			{
				return m_minLength;
			}
			
			set
			{
				this.m_minLength = value;
			}
			
		}
		/// <summary> Sets the valid alphabet of the StringGene</summary>
		/// <param name="a_alphabet">valid aplhabet for allele
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		virtual public System.String Alphabet
		{
			get
			{
				return m_alphabet;
			}
			
			set
			{
				//check if a substring is equal to the PERSISTENT_FIELD_DELIMITER
				//which is not allowed currently
				//---------------------------------------------------------------
				if (containsString(value, org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER))
				{
					throw new System.ArgumentException("The alphabet may not contain a " + "substring equal to the persistent field delimiter (which is " + org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER + " currently).");
				}
				/// <summary>@todo optionally check if alphabet contains doublettes</summary>
				this.m_alphabet = value;
			}
			
		}
		
		//Constants for ready-to-use alphabets or serving as part of concetenation
		public const System.String ALPHABET_CHARACTERS_UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public const System.String ALPHABET_CHARACTERS_LOWER = "abcdefghijklmnopqrstuvwxyz";
		public const System.String ALPHABET_CHARACTERS_DIGITS = "0123456789";
		public const System.String ALPHABET_CHARACTERS_SPECIAL = "+.*/\\,;@";
		
		/// <summary>String containing the CVS revision. Read out via reflection!</summary>
		private const System.String CVS_REVISION = "$Revision: 1.1 $";
		
		private int m_minLength;
		private int m_maxLength;
		
		private System.String m_alphabet;
		
		private System.Random rn;
		
		/// <summary> References the internal String value (allele) of this Gene.</summary>
		protected internal System.String m_value = null;
		
		private void  init()
		{
			rn = new System.Random();
		}
		
		public StringGene()
		{
			init();
		}
		
		/// <summary> </summary>
		/// <param name="a_minLength">minimum valid length of allele
		/// </param>
		/// <param name="a_maxLength">maximum valid length of allele
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public StringGene(int a_minLength, int a_maxLength):this(a_minLength, a_maxLength, null)
		{
		}
		
		/// <summary> </summary>
		/// <param name="a_minLength">minimum valid length of allele
		/// </param>
		/// <param name="a_maxLength">maximum valid length of allele
		/// </param>
		/// <param name="a_alphabet">valid aplhabet for allele
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public StringGene(int a_minLength, int a_maxLength, System.String a_alphabet)
		{
			if (a_minLength < 0)
			{
				throw new System.ArgumentException("minimum length must be greater than" + " zero!");
			}
			if (a_maxLength < a_minLength)
			{
				throw new System.ArgumentException("minimum length must be smaller than" + " or equal to maximum length!");
			}
			init();
			m_minLength = a_minLength;
			m_maxLength = a_maxLength;
			Alphabet = a_alphabet;
		}
		
		/// <summary> Executed by the genetic engine when this Gene instance is no
		/// longer needed and should perform any necessary resource cleanup.
		/// 
		/// @since 1.1
		/// </summary>
		public virtual void  cleanup()
		{
			// No specific cleanup is necessary for this implementation.
			// ---------------------------------------------------------
		}
		
		/// <summary> Sets the value (allele) of this Gene to a random String according to the
		/// valid alphabet and boundaries of length
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
		public virtual void  setToRandomValue(RandomGenerator a_numberGenerator)
		{
			if ((System.Object) m_alphabet == null || m_alphabet.Length < 1)
			{
				throw new System.SystemException("The valid alphabet is empty!");
			}
			
			if (m_maxLength < m_minLength || m_maxLength < 1)
			{
				throw new System.SystemException("Illegal valid maximum and/or minimum " + "length of alphabet!");
			}
			
			//randomize length of string
			//--------------------------
			
			int length;
			char value_Renamed;
			int index;
			
			length = m_maxLength - m_minLength + 1;
			
			int i = a_numberGenerator.nextInt() % length;
			if (i < 0)
			{
				i = - i;
			}
			length = m_minLength + i;
			
			//for each character: randomize character value (which can be represented
			//by an integer value)
			//-----------------------------------------------------------------------
			m_value = "";
			//UPGRADE_NOTE: Final was removed from the declaration of 'alphabetLength '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003"'
			int alphabetLength = m_alphabet.Length;
			for (int j = 0; j < length; j++)
			{
				index = a_numberGenerator.nextInt(alphabetLength);
				value_Renamed = m_alphabet[index];
				m_value += value_Renamed;
			}
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
		public virtual void  setValueFromPersistentRepresentation(System.String a_representation)
		{
			if ((System.Object) a_representation != null)
			{
				SupportClass.Tokenizer tokenizer = new SupportClass.Tokenizer(a_representation, org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER);
				// Make sure the representation contains the correct number of
				// fields. If not, throw an exception.
				// -----------------------------------------------------------
				if (tokenizer.Count != 4)
				{
					throw new UnsupportedRepresentationException("The format of the given persistent representation " + "is not recognized: it does not contain four tokens.");
				}
				System.String valueRepresentation = tokenizer.NextToken();
				System.String minLengthRepresentation = tokenizer.NextToken();
				System.String maxLengthRepresentation = tokenizer.NextToken();
				System.String alphabetRepresentation = tokenizer.NextToken();
				
				// Now parse and set the minimum length.
				// ----------------------------------
				try
				{
					m_minLength = System.Int32.Parse(minLengthRepresentation);
				}
				catch (System.FormatException )
				{
					throw new UnsupportedRepresentationException("The format of the given persistent representation " + "is not recognized: field 2 does not appear to be " + "an integer value.");
				}
				// Now parse and set the maximum length.
				// ----------------------------------
				try
				{
					m_maxLength = System.Int32.Parse(maxLengthRepresentation);
				}
				catch (System.FormatException )
				{
					throw new UnsupportedRepresentationException("The format of the given persistent representation " + "is not recognized: field 3 does not appear to be " + "an integer value.");
				}
				
				System.String tempValue;
				// Parse and set the representation of the value.
				// ----------------------------------------------
				if (valueRepresentation.Equals("null"))
				{
					tempValue = null;
				}
				else
				{
					if (valueRepresentation.Equals(("\"\"")))
					{
						tempValue = "";
					}
					else
					{
						tempValue = valueRepresentation;
					}
				}
				
				//check if minLength and maxLength are violated
				//---------------------------------------------
				if ((System.Object) tempValue != null)
				{
					if (m_minLength > tempValue.Length)
					{
						throw new UnsupportedRepresentationException("The value given" + " is shorter than the allowed maximum length.");
					}
					if (m_maxLength < tempValue.Length)
					{
						throw new UnsupportedRepresentationException("The value given" + " is longer than the allowed maximum length.");
					}
				}
				
				//check if all characters are within the alphabet
				//-----------------------------------------------
				if (!isValidAlphabet(tempValue, alphabetRepresentation))
				{
					throw new UnsupportedRepresentationException("The value given" + " contains invalid characters.");
				}
				
				m_value = tempValue;
				
				// Now set the alphabet that should be valid
				// -----------------------------------------
				m_alphabet = alphabetRepresentation;
			}
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
		public virtual System.String getPersistentRepresentation()
		{
			// The persistent representation includes the value, minimum length,
			// maximum length and valid alphabet. Each is separated by a colon.
			// ----------------------------------------------------------------
			//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			return ToString() + org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER + m_minLength + org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER + m_maxLength + org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER + m_alphabet;
		}
		
		/// <summary> Retrieves the value (allele) represented by this Gene. All values
		/// returned by this class will be String instances.
		/// 
		/// </summary>
		/// <returns> the String value of this Gene.
		/// 
		/// @since 1.1
		/// </returns>
		public virtual System.Object getAllele()
		{
			return m_value;
		}
		
		/// <summary> Sets the value (allele) of this Gene to the new given value. This class
		/// expects the value to be a String instance. If the value is shorter or
		/// longer than the minimum or maximum length or any character is not within
		/// the valid alphabet an exception is throwsn
		/// 
		/// </summary>
		/// <param name="a_newValue">the new value of this Gene instance.
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual void  setAllele(System.Object a_newValue)
		{
			if (a_newValue != null)
			{
				System.String temp = (System.String) a_newValue;
				if (temp.Length < m_minLength || temp.Length > m_maxLength)
				{
					throw new System.ArgumentException("The given value is too short or too long!");
				}
				//check for validity of alphabet
				//------------------------------
				if (!isValidAlphabet(temp, m_alphabet))
				{
					throw new System.ArgumentException("The given value contains" + " at least one invalid character.");
				}
			}
			
			m_value = ((System.String) a_newValue);
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
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual Gene newGene(Configuration a_activeConfiguration)
		{
			return new StringGene(m_minLength, m_maxLength, m_alphabet);
		}
		
		/// <summary> Compares this StringGene with the specified object (which must also
		/// be a StringGene) for order, which is determined by the String
		/// value of this Gene compared to the one provided for comparison.
		/// 
		/// </summary>
		/// <param name="other">the StringGene to be compared to this StringGene.
		/// </param>
		/// <returns> a negative int, zero, or a positive int as this object
		/// is less than, equal to, or greater than the object provided for
		/// comparison.
		/// 
		/// @throws ClassCastException if the specified object's type prevents it
		/// from being compared to this StringGene.
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual int CompareTo(System.Object other)
		{
			StringGene otherStringGene = (StringGene) other;
			// First, if the other gene (or its value) is null, then this is
			// the greater allele. Otherwise, just use the String's compareTo
			// method to perform the comparison.
			// ---------------------------------------------------------------
			if (otherStringGene == null)
			{
				return 1;
			}
			else if ((System.Object) otherStringGene.m_value == null)
			{
				// If our value is also null, then we're the same. Otherwise,
				// this is the greater gene.
				// ----------------------------------------------------------
				return (System.Object) m_value == null?0:1;
			}
			else
			{
				try
				{
					return m_value.CompareTo(otherStringGene.m_value);
				}
				catch (System.InvalidCastException e)
				{
					SupportClass.WriteStackTrace(e, Console.Error);
					throw e;
				}
			}
		}
		
		public virtual int size()
		{
			return m_value.Length;
		}
		
		/// <summary> Retrieves a string representation of this StringGene's value that
		/// may be useful for display purposes.
		/// 
		/// </summary>
		/// <returns> a string representation of this StringGene's value.
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public override System.String ToString()
		{
			if ((System.Object) m_value == null)
			{
				return "null";
			}
			else
			{
				if (m_value.Equals(""))
				{
					return "\"\"";
				}
				else
				{
					return m_value.ToString();
				}
			}
		}
		
		/// <summary> Compares this Gene with the given object and returns true if
		/// the other object is a Gene of this type and has the same value (allele) as
		/// this Gene. Otherwise it returns false.
		/// 
		/// </summary>
		/// <param name="other">the object to compare to this Gene for equality.
		/// </param>
		/// <returns> true if this Gene is equal to the given object,
		/// false otherwise.
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public  override bool Equals(System.Object other)
		{
			try
			{
				return CompareTo(other) == 0;
			}
			catch (System.InvalidCastException )
			{
				// If the other object isn't a StringGene, then we're not equal.
				// -------------------------------------------------------------
				return false;
			}
		}
		
		/// <summary> Retrieves the hash code value for this StringGene.
		/// 
		/// </summary>
		/// <returns> this StringGene's hash code.
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public override int GetHashCode()
		{
			// If our internal Double is null, then return zero. Otherwise,
			// just return the hash code of the String.
			// -------------------------------------------------------------
			if ((System.Object) m_value == null)
			{
				return 0;
			}
			else
			{
				return m_value.GetHashCode();
			}
		}
		
		/// <summary> Retrieves the String value of this Gene, which may be more convenient in
		/// some cases than the more general getAllele() method.
		/// 
		/// </summary>
		/// <returns> the String value of this Gene.
		/// 
		/// @since 1.1
		/// </returns>
		public virtual System.String stringValue()
		{
			return (System.String) m_value;
		}
		
		/// <summary> Checks whether a substring is contained within another string</summary>
		/// <param name="totalString">the total string to examine
		/// </param>
		/// <param name="subString">the substring to look for
		/// </param>
		/// <returns> true: the totalString contains the subString
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		private bool containsString(System.String totalString, System.String subString)
		{
			if ((System.Object) totalString == null || (System.Object) subString == null)
			{
				return false;
			}
			return totalString.IndexOf(subString) >= 0;
		}
		
		/// <summary> Checks whether a string value is valid concerning a given alphabet</summary>
		/// <param name="a_value">the value to check
		/// </param>
		/// <param name="a_alphabet">the valid alphabet to check against
		/// </param>
		/// <returns> true: given string value is valid
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		private bool isValidAlphabet(System.String a_value, System.String a_alphabet)
		{
			if ((System.Object) a_value == null || a_value.Length < 1)
			{
				return true;
			}
			if ((System.Object) a_alphabet == null)
			{
				return true;
			}
			if (a_alphabet.Length < 1)
			{
				return false;
			}
			
			//loop over all characters of a_value
			//-----------------------------------
			int length = a_value.Length;
			char c;
			for (int i = 0; i < length; i++)
			{
				c = a_value[i];
				if (a_alphabet.IndexOf((System.Char) c) < 0)
				{
					return false;
				}
			}
			return true;
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
			System.String s = stringValue();
			int ch = s[index];
			//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
			int newValue = (int) System.Math.Round(ch * (1.0d + a_percentage));
			
			// Set mutated character by concatenating the String by using "ch"
			// ---------------------------------------------------------------
			s = s.Substring(0, (index) - (0)) + ch + s.Substring(index + 1);
			
			setAllele(s);
			
			// If the value isn't in the alphabet of this Gene
			//  Gene, map it to a value within the alphabet closest to wanted value.
			// -------------------------------------------------------------
			/// <summary>@todo implement       mapValueToWithinBounds ();</summary>
		}
	}
}