using System;

namespace org.jgap.impl
{
	/// <summary>
	/// Summary description for RandomWrapper.
	/// </summary>
	/// 
	/// <author> Jerry Vos </author>
	/// <version> $Revision: 1.1 $ $Date: 2004/11/17 18:09:19 $ </version>
	public class RandomWrapper : System.Random
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
		public int nextInt()
		{
			return base.Next();
		}
		
		
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
		public int nextInt(int ceiling)
		{
			return base.Next(ceiling);
		}
		
		
		/// <summary> Returns the next pseudorandom, uniformly distributed long value from
		/// this random number generator's sequence. The general contract of
		/// nextLong() is that one long value is pseudorandomly generated and
		/// returned. All 2^64 possible long values are produced with
		/// (approximately) equal probability.
		/// 
		/// </summary>
		/// <returns> a psuedorandom long value.
		/// </returns>
		public long nextLong()
		{
			double randDouble = base.NextDouble();

			if (randDouble < .5)
			{
				return Convert.ToInt64(base.NextDouble() * Int64.MinValue);				
			}
			else
			{
				return Convert.ToInt64(base.NextDouble() * Int64.MaxValue);
			}
		}
		
		
		/// <summary> Returns the next pseudorandom, uniformly distributed double value
		/// between 0.0 and 1.0 from this random number generator's sequence.
		/// 
		/// </summary>
		/// <returns> a psuedorandom double value.
		/// </returns>
		public double nextDouble()
		{
			return base.NextDouble();
		}
		
		
		/// <summary> Returns the next pseudorandom, uniformly distributed float value
		/// between 0.0 and 1.0 from this random number generator's sequence.
		/// 
		/// </summary>
		/// <returns> a psuedorandom float value.
		/// </returns>
		public float nextFloat()
		{
			return Convert.ToSingle(base.NextDouble());
		}
		
		
		/// <summary> Returns the next pseudorandom, uniformly distributed boolean value
		/// from this random number generator's sequence. The general contract
		/// of nextBoolean is that one boolean value is pseudorandomly generated
		/// and returned. The values true and false are produced with
		/// (approximately) equal probability.
		/// 
		/// </summary>
		/// <returns> a pseudorandom boolean value.
		/// </returns>
		public bool nextBoolean()
		{
			return (base.NextDouble() < 0.5);
		}
	}
}
