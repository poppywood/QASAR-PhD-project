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
using MutationRateCalculator = org.jgap.MutationRateCalculator;
using Configuration = org.jgap.Configuration;
namespace org.jgap.impl
{
	
	/// <summary> Default implementation of a mutation rate calculcator
	/// 
	/// </summary>
	/// <author>  Klaus Meffert
	/// @since 1.1
	/// </author>
	public class DefaultMutationRateCalculator : MutationRateCalculator
	{
		/// <summary>String containing the CVS revision. Read out via reflection!</summary>
		private const System.String CVS_REVISION = "$Revision: 1.1 $";
		
		public DefaultMutationRateCalculator()
		{
		}
		
		/// <summary> Calculates the mutation rate</summary>
		/// <param name="a_activeConfiguration">current active configuration
		/// </param>
		/// <returns> calculated divisor of mutation rate probability (dividend is 1)
		/// 
		/// @since 1.1 (same functionality since earlier, but not encapsulated)
		/// </returns>
		public virtual int calculateCurrentRate(Configuration a_activeConfiguration)
		{
			return a_activeConfiguration.ChromosomeSize;
		}
	}
}