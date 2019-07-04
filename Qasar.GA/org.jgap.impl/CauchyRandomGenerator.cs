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
using RandomGenerator = org.jgap.RandomGenerator;
namespace org.jgap.impl
{
	
	/// <summary>@todo not ready yet</summary>
	/// <summary>@todo not ready yet</summary>
	/// <summary>@todo not ready yet</summary>
	
	/// <summary> Cauchy probability density function</summary>
	/// <seealso cref="http://www.itl.nist.gov/div898/handbook/eda/section3/eda3663.htm">
	/// 
	/// </seealso>
	/// <author>  Klaus Meffert
	/// @since 1.1
	/// </author>
	
	public class CauchyRandomGenerator : RandomGenerator
	{
		private void  InitBlock()
		{
			rn = new System.Random();
		}
		virtual public double CauchyStandardDeviation
		{
			get
			{
				return m_standardDistribution;
			}
			
			set
			{
				m_standardDistribution = value;
			}
			
		}
		/// <summary>String containing the CVS revision. Read out via reflection!</summary>
		private const System.String CVS_REVISION = "$Revision: 1.1 $";
		
		private const double DELTA = 0.0000001;
		
		private double m_standardDistribution;
		
		//UPGRADE_NOTE: The initialization of  'rn' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private System.Random rn;
		
		public CauchyRandomGenerator():this(1)
		{
		}
		
		public CauchyRandomGenerator(double a_standardDistribution)
		{
			InitBlock();
			m_standardDistribution = a_standardDistribution;
		}
		
		/// <summary> Calculates a random density of the cauchy distribution</summary>
		/// <returns> calculated density
		/// 
		/// @since 1.1
		/// </returns>
		private int calculateDensity()
		{
			//compute (standard) cauchy distribution:
			//f(x) = (1/[pi(1+x²)])
			//----------------------------
			
			int result;
			double rate;
			
			double v1 = 10 * nextDouble() - 5; // between -5 and 5
			
			//invert the result as higher values indicate less probable mutation ???
			//------------------------------------------------------------------
			rate = (System.Math.PI * (1 + v1 * v1));
			
			
			//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
			result = (int) System.Math.Round(rate);
			return result;
		}
		
		public virtual int nextInt()
		{
			//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
			return System.Math.Min(System.Int32.MaxValue - 1, (int) System.Math.Round(nextCauchy() * System.Int32.MaxValue));
		}
		
		public virtual int nextInt(int ceiling)
		{
			//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
			return System.Math.Min(ceiling - 1, (int) System.Math.Round(nextCauchy() * ceiling));
		}
		
		public virtual long nextLong()
		{
			//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
			return System.Math.Min(System.Int64.MaxValue - 1, (long) System.Math.Round(nextCauchy() * System.Int64.MaxValue));
		}
		
		public virtual double nextDouble()
		{
			return nextCauchy();
		}
		
		public virtual float nextFloat()
		{
			//UPGRADE_WARNING: Narrowing conversions may produce unexpected results in C#. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042"'
			return System.Math.Min(System.Single.MaxValue - 1, (float) (nextCauchy() * System.Single.MaxValue));
		}
		
		public virtual bool nextBoolean()
		{
			return nextCauchy() >= 0.5d;
		}
		
		public virtual double nextCauchy()
		{
			return calculateDensity(); // * getGaussianStdDeviation ();
		}
	}
}