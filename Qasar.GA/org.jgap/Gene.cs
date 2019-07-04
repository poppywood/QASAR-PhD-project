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
namespace org.jgap
{
	
	/// <summary> Genes represent the discrete components of a potential solution
	/// (the Chromosome). This interface exists so that custom gene implementations
	/// can be easily plugged-in, which can add a great deal of flexibility and
	/// convenience for many applications. Note that it's very important that
	/// implementations of this interface also implement the equals() method.
	/// Without a proper implementation of equals(), some genetic operations will
	/// fail to work properly.
	/// 
	/// </summary>
	/// <author>  Neil Rotstan
	/// @since 1.0
	/// </author>
	public struct Gene_Fields{
		/// <summary> Represents the delimiter that is used to separate fields in the
		/// persistent representation of DoubleGene instances.
		/// </summary>
		public readonly static System.String PERSISTENT_FIELD_DELIMITER = ":";
	}
	public interface Gene : System.IComparable
	{
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
		//UPGRADE_NOTE: Access modifiers of method 'newGene' were changed to 'public'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1204"'
		Gene newGene(Configuration a_activeConfiguration);
		
		/// <summary> Sets the value of this Gene to the new given value. The actual
		/// type of the value is implementation-dependent.
		/// 
		/// </summary>
		/// <param name="a_newValue">the new value of this Gene instance.
		/// </param>
		void  setAllele(System.Object a_newValue);
		
		/// <summary> Retrieves the value represented by this Gene. The actual type
		/// of the value is implementation-dependent.
		/// 
		/// </summary>
		/// <returns> the value of this Gene.
		/// </returns>
		System.Object getAllele();
		
		/// <summary> Retrieves a string representation of the value of this Gene instance
		/// that includes any information required to reconstruct it at a later
		/// time, such as its value and internal state. This string will be used to
		/// represent this Gene instance in XML persistence. This is an optional
		/// method but, if not implemented, XML persistence and possibly other
		/// features will not be available. An UnsupportedOperationException should
		/// be thrown if no implementation is provided.
		/// 
		/// </summary>
		/// <returns> A string representation of this Gene's current state.
		/// @throws UnsupportedOperationException to indicate that no implementation
		/// is provided for this method.
		/// </returns>
		//UPGRADE_NOTE: Access modifiers of method 'getPersistentRepresentation' were changed to 'public'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1204"'
		System.String getPersistentRepresentation();
		
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
		//UPGRADE_NOTE: Access modifiers of method 'setValueFromPersistentRepresentation' were changed to 'public'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1204"'
		void  setValueFromPersistentRepresentation(System.String a_representation);
		
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
		//UPGRADE_NOTE: Access modifiers of method 'setToRandomValue' were changed to 'public'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1204"'
		void  setToRandomValue(RandomGenerator a_numberGenerator);
		
		/// <summary> Executed by the genetic engine when this Gene instance is no
		/// longer needed and should perform any necessary resource cleanup.
		/// </summary>
		void  cleanup();
		
		/// <returns> a string representation of the gene
		/// 
		/// @since 1.1 (in the interface)
		/// </returns>
		System.String ToString();
		
		/// <returns> the size of the gene, i.e the number of atomic elements.
		/// Always 1 for numbers
		/// 
		/// @since 1.1
		/// </returns>
		int size();
		
		/// <summary> Applies a mutation of a given intensity (percentage) onto the atomic
		/// element at given index (NumberGenes only have one atomic element)
		/// </summary>
		/// <param name="index">index of atomic element, between 0 and size()-1
		/// </param>
		/// <param name="a_percentage">percentage of mutation (greater than -1 and smaller
		/// than 1).
		/// 
		/// @since 1.1
		/// </param>
		void  applyMutation(int index, double a_percentage);
	}
}