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
namespace org.jgap
{
	
	/// <summary> Interface for a calculator that determines the mutation rate dynamically
	/// 
	/// </summary>
	/// <author>  Klaus Meffert
	/// @since 1.1
	/// </author>
	public interface MutationRateCalculator
		{
			/// <summary> Calculates the mutation rate</summary>
			/// <param name="a_activeConfiguration">current active configuration
			/// </param>
			/// <returns> the currently applying mutation rate. It is calculated
			/// dynamically
			/// 
			/// @since 1.1
			/// </returns>
			int calculateCurrentRate(Configuration a_activeConfiguration);
		}
}