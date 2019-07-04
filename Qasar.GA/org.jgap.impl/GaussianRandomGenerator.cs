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
	
	/// <summary> Gaussian deviation serving as basis for randomly finding a number.</summary>
	/// <seealso cref="http://tracer.lcc.uma.es/tws/cEA/GMut.htm">
	/// </seealso>
	/// <seealso cref="http://hyperphysics.phy-astr.gsu.edu/hbase/math/gaufcn.html">
	/// 
	/// </seealso>
	/// <author>  Klaus Meffert
	/// @since 1.1
	/// </author>
	public class GaussianRandomGenerator : RandomGenerator
	{
		virtual public double GaussianMean
		{
			set
			{
				m_mean = value;
			}
			
		}
		virtual public double GaussianStdDeviation
		{
			get
			{
				return m_standardDeviation;
			}
			
			set
			{
				m_standardDeviation = value;
			}
			
		}
		/// <summary>String containing the CVS revision. Read out via reflection!</summary>
		private const System.String CVS_REVISION = "$Revision: 1.1 $";
		
		private const double DELTA = 0.0000001;
		
		private System.Random rn;
		
		/// <summary> Mean of the gaussian deviation</summary>
		private double m_mean;
		
		/// <summary> Standard deviation of the gaussian deviation</summary>
		private double m_standardDeviation;
		
		public GaussianRandomGenerator():this(0.0d)
		{
		}
		
		/// <summary> Constructor speicifying the (obliagtory) standard deviation</summary>
		/// <param name="a_standardDeviation">the standard deviation to use
		/// </param>
		public GaussianRandomGenerator(double a_standardDeviation):base()
		{
			init();
			GaussianStdDeviation = a_standardDeviation;
		}
		
		/// <summary> Initializations on construction</summary>
		private void  init()
		{
			rn = new System.Random();
		}
		
		public virtual int nextInt()
		{
			//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
			return System.Math.Min(System.Int32.MaxValue - 1, (int) System.Math.Round(nextGaussian() * System.Int32.MaxValue));
		}
		
		public virtual int nextInt(int ceiling)
		{
			//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
			return System.Math.Min(ceiling - 1, (int) System.Math.Round(nextGaussian() * ceiling));
		}
		
		public virtual long nextLong()
		{
			//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
			return System.Math.Min(System.Int64.MaxValue - 1, (long) System.Math.Round(nextGaussian() * System.Int64.MaxValue));
		}
		
		public virtual double nextDouble()
		{
			return nextGaussian();
		}
		
		public virtual float nextFloat()
		{
			//UPGRADE_WARNING: Narrowing conversions may produce unexpected results in C#. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1042"'
			return System.Math.Min(System.Single.MaxValue - 1, (float) (nextGaussian() * System.Single.MaxValue));
		}
		
		public virtual bool nextBoolean()
		{
			return nextGaussian() >= 0.5d;
		}
		
		/// <returns> the next randomly distributed gaussian with current standard
		/// deviation
		/// </returns>
		private double nextGaussian()
		{
			double x, y, r2;

			do
			{
				/* choose x,y in uniform square (-1,-1) to (+1,+1) */

				x = -1 + 2 * rn.NextDouble();
				y = -1 + 2 * rn.NextDouble();

				/* see if it is in the unit circle */
				r2 = x * x + y * y;
			}
			while (r2 > 1.0 || r2 == 0);

			/* Box-Muller transform */
			return GaussianStdDeviation * y * Math.Sqrt (-2.0 * Math.Log (r2) / r2);
		}
	}
}