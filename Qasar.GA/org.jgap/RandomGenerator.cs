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
	
	
	/// <summary> The RandomGenerator interface provides an abstraction for the random
	/// number implementation so that more rigorous or alternative implementations
	/// can be provided as desired.
	/// 
	/// </summary>
	/// <author>  Neil Rotstan
	/// @since 1.0
	/// </author>
	public interface RandomGenerator
	{
		/// <summary> Returns the next pseudorandom, uniformly distributed int value
		/// from this random number generator's sequence. The general contract
		/// of nextInt is that one int value is pseudorandomly generated and
		/// returned. All 2^32  possible int values are produced with
		/// (approximately) equal probability.
		/// 
		/// </summary>
		/// <returns> a pseudorandom integer value.
		/// </returns>
		int nextInt();
		
		
		/// <summary> Returns a pseudorandom, uniformly distributed int value between
		/// 0 (inclusive) and the specified value (exclusive), drawn from this
		/// random number generator's sequence. The general contract of nextInt
		/// is that one int value in the specified range is pseudorandomly
		/// generated and returned. All n possible int values are produced with
		/// (approximately) equal probability.
		/// 
		/// </summary>
		/// <returns> a pseudorandom integer value between 0 and the given
		/// ceiling - 1, inclusive.
		/// </returns>
		int nextInt(int ceiling);
		
		
		/// <summary> Returns the next pseudorandom, uniformly distributed long value from
		/// this random number generator's sequence. The general contract of
		/// nextLong() is that one long value is pseudorandomly generated and
		/// returned. All 2^64 possible long values are produced with
		/// (approximately) equal probability.
		/// 
		/// </summary>
		/// <returns> a psuedorandom long value.
		/// </returns>
		long nextLong();
		
		
		/// <summary> Returns the next pseudorandom, uniformly distributed double value
		/// between 0.0 and 1.0 from this random number generator's sequence.
		/// 
		/// </summary>
		/// <returns> a psuedorandom double value.
		/// </returns>
		double nextDouble();
		
		
		/// <summary> Returns the next pseudorandom, uniformly distributed float value
		/// between 0.0 and 1.0 from this random number generator's sequence.
		/// 
		/// </summary>
		/// <returns> a psuedorandom float value.
		/// </returns>
		float nextFloat();
		
		
		/// <summary> Returns the next pseudorandom, uniformly distributed boolean value
		/// from this random number generator's sequence. The general contract
		/// of nextBoolean is that one boolean value is pseudorandomly generated
		/// and returned. The values true and false are produced with
		/// (approximately) equal probability.
		/// 
		/// </summary>
		/// <returns> a pseudorandom boolean value.
		/// </returns>
		bool nextBoolean();
	}
}