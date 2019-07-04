using System;

namespace Qasar.Controller
{
	/// <summary>
	/// Summary description for Calculus.
	/// </summary>
	public class Calculus
	{
		/// <summary>
		/// Calculates the Y-Ingral Power of X.
		/// </summary>
		/// <param name="X">Target number</param>
		/// <param name="Y">Power</param>
		public static Decimal Pow(Decimal X, int Y)
		{
			Decimal result = 1 ;
			for(int count = 0; count < Y; count ++)
				result *= X ;
			return result ;
		}
		/// <summary>
		/// Calculates the Factorial of X as a Decimal.
		/// </summary>
		public static Decimal FactorialM(int X)
		{
			Decimal Result = 1 ;
			for(int i = 2; i <= X; i ++)
				Result *= i ;
			return Result ;
		}
		/// <summary>
		/// Calculates the X-Power of Euler constant.
		/// </summary>
		/// <param name="X">Power</param>
		public static Decimal Exp(Decimal X)
		{
			int i = 0 ;
			Decimal Xi = 1 ;
			Decimal iFactorial = 1 ;
			Decimal Result = 0 ;
			Decimal term = Xi / iFactorial ;
			while ((double)term > 1.0e-27)
			{
				Result += term ;
				Xi *= X ;
				i ++ ;
				iFactorial *= i ;
				term = Xi / iFactorial ;
			}
			return Result ;
		}
		/// <summary>
		/// Calculates the natural logarithm of X.
		/// </summary>
		public static Decimal Ln(Decimal X)
		{
			int Sign = 1 ;
			int i = 1 ;
			Decimal XMinusOne = X ;
			Decimal term = Sign * XMinusOne / i ;
			Decimal Result = 0 ;
			while((double)term > 1.0e-27)
			{
				Result += term ;
				XMinusOne *= X - 1 ;
				i ++ ;
				Sign *= -1 ;
				term = Sign * XMinusOne / i ;
			}
			return Result ;
		}
		/// <summary>
		/// Calculates the Base B logarithm of A.
		/// </summary>
		public static Decimal Log(Decimal A, Decimal B)
		{
			return Ln(A) / Ln(B) ;
		}
		/// <summary>
		/// Calculates the e Base logarithm of A
		/// </summary>
		/// <param name="A"></param>
		/// <returns></returns>
		public static Decimal Log(Decimal A)
		{
			return Ln(A) ;
		}
		/// <summary>
		/// Calculates de B-Power of A.
		/// </summary>
		public static Decimal Pow(Decimal A, Decimal B)
		{
			return Exp(B * Ln(A)) ;
		}
		/// <summary>
		/// Returns the whole number closest to X toward zero
		/// </summary>
		/// <param name="X"></param>
		/// <returns></returns>
		public static Decimal Floor(Decimal X)
		{
			return Decimal.Truncate(X) ;
		}
		public static Decimal Sqrt(Decimal X)
		{
			return Pow(X, 0.5M) ;
		}
		public const Decimal E = 2.71828182845904523536028747135M ;
		public const Decimal Sqrt2 = 1.41421356237309504880168872421M ;
		public const Decimal Pi = 3.14159265358979323846264338328M ;
		public const Decimal SqrtPi = 1.77245385090551602729816748334M ;
	}
}
