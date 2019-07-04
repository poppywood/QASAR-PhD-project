using System;
using System.Collections ;
using Mathematics = System.Math ;

namespace Qasar.Controller
{
	public class Statistics
	{
		#region Constants
		/// <summary>
		/// Euler-Mascheroni Constant represented as a Decimal.
		/// </summary>
		public static readonly Decimal EulerMascheroniM = 0.577215664901532860606512090M ;
		/// <summary>
		/// Euler-Mascheroni Constant represented as a Double.
		/// </summary>
		public static readonly double EulerMascheroniD = 0.577215664901533 ;
		/// <summary>
		/// Euler-Mascheroni Constant represented as a Float.
		/// </summary>
		public static readonly float EulerMascheroniF = 0.5772157F ;
		#endregion
		#region AveDev
		/// <summary>
		/// Calculates the Average of the absolute deviations of numbers from their mean. 
		/// </summary>
		/// <param name="X">Set of numbers to calculate the average deviation</param>
		public static Decimal AveDev(Decimal[] X)
		{
			Decimal ave = Average(X) ;
			Decimal Result = 0 ;
			for(int i = 0; i < X.Length; i++)
				Result += Mathematics.Abs(X[i] - ave) ;
			return Result / X.Length ;
		}
		/// <summary>
		/// Calculates the Average of the absolute deviations of numbers from their mean.
		/// </summary>
		/// <param name="X">Set of numbers to calculate the average deviation</param>
		public static double AveDev(double[] X)
		{
			double ave = Average(X) ;
			double Result = 0;
			for(int i = 0; i < X.Length; i++)
				Result += Mathematics.Abs(X[i] - ave) ;
			return Result / X.Length ;
		}
		/// <summary>
		/// Calculates the Average of the absolute deviations of numbers from their mean.
		/// </summary>
		/// <param name="X">Set of numbers to calculate the average deviation</param>
		public static float AveDev(float[] X)
		{
			return (float)AveDev(ToDoubleArray(X)) ;
		}
		#endregion
		#region Average
		/// <summary>
		/// Calculates the Average(Arithmetic Mean) of the arguments.
		/// </summary>
		/// <param name="X">Set of numbers to calculate the average</param>
		public static Decimal Average(Decimal[] X)
		{
			Decimal Result = 0 ;
			for(int i = 0; i < X.Length; i ++)
				Result += X[i] ;
			return Result / X.Length ;
		}
		/// <summary>
		/// Calculates the Average(Arithmetic Mean) of the arguments.
		/// </summary>
		/// <param name="X">Set of numbers to calculate the average</param>
		public static double Average(double[] X)
		{
			double Result = 0 ;
			for(int i = 0; i < X.Length; i ++)
				Result += X[i] ;
			return Result / X.Length ;
		}
		/// <summary>
		/// Calculates the Average(Arithmetic Mean) of the arguments.
		/// </summary>
		/// <param name="X">Set of numbers to calculate the average</param>
		public static float Average(float[] X)
		{
			return (float)Average(ToDoubleArray(X)) ;
		}
		#endregion
		#region AverageTools
		/// <summary>
		/// Returns the average of the values within range [ii; fi]
		/// </summary>
		/// <param name="X">Values</param>
		/// <param name="ii">Initial index</param>
		/// <param name="fi">Final index</param>
		/// <returns></returns>
		public static double RangedAverage(double[] X, int ii, int fi)
		{
			double sum = .0 ;
			for(int i = ii; i <= fi; i++)
				sum += X[i] ;
			return sum / ((double)fi - ii + 1) ;
		}
		/// <summary>
		/// Returns the average of the values within range [ii; fi]
		/// </summary>
		/// <param name="X">Values</param>
		/// <param name="ii">Initial index</param>
		/// <param name="fi">Final index</param>
		/// <returns></returns>
		public static float RangedAverage(float[] X, int ii, int fi)
		{
			float sum = .0F ;
			for(int i = ii; i <= fi; i++)
				sum += X[i] ;
			return sum / ((float)fi - ii + 1) ;
		}
		/// <summary>
		/// Returns the average of the values within range [ii; fi]
		/// </summary>
		/// <param name="X">Values</param>
		/// <param name="ii">Initial index</param>
		/// <param name="fi">Final index</param>
		/// <returns></returns>
		public static decimal RangedAverage(decimal[] X, int ii, int fi)
		{
			decimal sum = .0M ;
			for(int i = ii; i <= fi; i++)
				sum += X[i] ;
			return sum / ((decimal)fi - ii + 1) ;
		}
		#endregion
		#region BetaDist
		/// <summary>
		/// Calculates the Cumulative Distribution of the Beta Function mapping <code>X</code> within 
		/// <code>[XLowerBound; XUpperBound] to [0; 1]</code>
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of Beta Distribution</param>
		/// <param name="beta">Beta parameter of Beta Distribution</param>
		/// <param name="XLowerBound">Lower bound for X</param>
		/// <param name="XUpperBound">Upper bound for X</param>
		public static double BetaDist(double X, double alpha, double beta, double XLowerBound, double XUpperBound)
		{
			return BetaCumulative(X / (XUpperBound - XLowerBound), alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution of the Beta Function mapping <code>X</code> within 
		/// <code>[XLowerBound; XUpperBound] to [0; 1]</code>
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of Beta Distribution</param>
		/// <param name="beta">Beta parameter of Beta Distribution</param>
		/// <param name="XLowerBound">Lower bound for X</param>
		/// <param name="XUpperBound">Upper bound for X</param>
		public static float BetaDist(float X, float alpha, float beta, float XLowerBound, float XUpperBound)
		{
			return BetaCumulative((float)(X / (XUpperBound - XLowerBound)), (float)alpha, (float)beta) ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution of the Beta Function mapping <code>X</code> within 
		/// <code>[XLowerBound; XUpperBound] to [0; 1]</code>
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of Beta Distribution</param>
		/// <param name="beta">Beta parameter of Beta Distribution</param>
		/// <param name="XLowerBound">Lower bound for X</param>
		/// <param name="XUpperBound">Upper bound for X</param>
		public static Decimal BetaDist(Decimal X, Decimal alpha, Decimal beta, Decimal XLowerBound, Decimal XUpperBound)
		{
			return BetaCumulative(X / (XUpperBound - XLowerBound), alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution of the Beta Function.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of Beta Distribution</param>
		/// <param name="beta">Beta parameter of Beta Distribution</param>
		public static double BetaDist(double X, double alpha, double beta)
		{
			return BetaCumulative(X, alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution of the Beta Function.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of Beta Distribution</param>
		/// <param name="beta">Beta parameter of Beta Distribution</param>
		public static float BetaDist(float X, float alpha, float beta)
		{
			return BetaCumulative(X, alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution of the Beta Function.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of Beta Distribution</param>
		/// <param name="beta">Beta parameter of Beta Distribution</param>
		public static Decimal BetaDist(Decimal X, Decimal alpha, Decimal beta)
		{
			return BetaCumulative(X, alpha, beta) ;
		}
		#endregion
		#region BetaInv
		/// <summary>
		/// Calculates the Inverse of the Beta Cumulative Distribution Function and mapps the result to 
		/// <code>[XLowerBound; XUpperBound]</code>
		/// </summary>
		/// <param name="Probability">Probability asociated with the beta distribution</param>
		/// <param name="alpha">alpha parameter of the distribution</param>
		/// <param name="beta">beta parameter of the distribution</param>
		/// <param name="XLowerBound">Lower bound for X</param>
		/// <param name="XUpperBound">Upper bound for X</param>
		public static double BetaInv(double Probability, double alpha, double beta, double XLowerBound, double XUpperBound)
		{
			double Result = BetaInv(Probability, alpha, beta) ;
			return Result * (XUpperBound - XLowerBound) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Beta Cumulative Distribution Function and mapps the result to 
		/// <code>[XLowerBound; XUpperBound]</code>
		/// </summary>
		/// <param name="Probability">Probability asociated with the beta distribution</param>
		/// <param name="alpha">alpha parameter of the distribution</param>
		/// <param name="beta">beta parameter of the distribution</param>
		/// <param name="XLowerBound">Lower bound for X</param>
		/// <param name="XUpperBound">Upper bound for X</param>
		public static float BetaInv(float Probability, float alpha, float beta, float XLowerBound, float XUpperBound)
		{
			float Result = BetaInv(Probability, alpha, beta) ;
			return Result * (XUpperBound - XLowerBound) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Beta Cumulative Distribution Function and mapps the result to 
		/// <code>[XLowerBound; XUpperBound]</code>
		/// </summary>
		/// <param name="Probability">Probability asociated with the beta distribution</param>
		/// <param name="alpha">alpha parameter of the distribution</param>
		/// <param name="beta">beta parameter of the distribution</param>
		/// <param name="XLowerBound">Lower bound for X</param>
		/// <param name="XUpperBound">Upper bound for X</param>
		public static Decimal BetaInv(Decimal Probability, Decimal alpha, Decimal beta, Decimal XLowerBound, Decimal XUpperBound)
		{
			Decimal Result = BetaInv(Probability, alpha, beta) ;
			return Result * (XUpperBound - XLowerBound) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Beta Cumulative Distribution Function. Uses Newton's method to converge
		/// the solution.
		/// Reference:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964.
		/// </summary>
		/// <param name="Probability">Probability asociated with the beta distribution</param>
		/// <param name="alpha">alpha parameter of the distribution</param>
		/// <param name="beta">beta parameter of the distribution</param>
		public static double BetaInv(double Probability, double alpha, double beta)
		{
			if (Probability < 0 || Probability > 1 | alpha <= 0 | beta <= 0)
				return double.NaN ;
			if (Probability == 0)
				return 0 ;
			if (Probability == 1)
				return 1 ;
			int count_limit = 100 ;
			int count = 0 ;
			double pk = Probability ;
			double xk = alpha / (alpha + beta) ;
			if (xk == 0)
				xk = Mathematics.Sqrt(double.Epsilon) ;
			if (xk == 1)
				xk = 1 - Mathematics.Sqrt(double.Epsilon) ;
			double h = 1 ;
			double crit = Mathematics.Sqrt(double.Epsilon) ;
			while(Mathematics.Abs(h) > crit * Mathematics.Abs(xk) && Mathematics.Abs(h) > crit 
				&& count < count_limit)
			{
				count ++ ;
				h = (BetaCumulative(xk, alpha, beta) - pk) / BetaProbability(xk, alpha, beta) ;
				double xnew = xk - h ;
				if (xnew <= 0)
					xnew = xk / 10 ;
				if (xnew >= 1)
					xnew = 1 - (1 - xk) / 10 ;
				xk = xnew ;
			}
			return xk ;
		}		/// <summary>
		/// Calculates the Inverse of the Beta Cumulative Distribution Function. Uses Newton's method to converge
		/// the solution.
		/// Reference:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964.
		/// </summary>
		/// <param name="Probability">Probability asociated with the beta distribution</param>
		/// <param name="alpha">alpha parameter of the distribution</param>
		/// <param name="beta">beta parameter of the distribution</param>
		public static float BetaInv(float Probability, float alpha, float beta)
		{
			if (Probability < 0 || Probability > 1 | alpha <= 0 | beta <= 0)
				return float.NaN ;
			if (Probability == 0)
				return 0 ;
			if (Probability == 1)
				return 1 ;
			int count_limit = 100 ;
			int count = 0 ;
			double pk = Probability ;
			double xk = alpha / (alpha + beta) ;
			if (xk == 0)
				xk = Mathematics.Sqrt(float.Epsilon) ;
			if (xk == 1)
				xk = 1 - Mathematics.Sqrt(float.Epsilon) ;
			double h = 1 ;
			double crit = Mathematics.Sqrt(float.Epsilon) ;
			while(Mathematics.Abs(h) > crit * Mathematics.Abs(xk) && Mathematics.Abs(h) > crit 
				&& count < count_limit)
			{
				count ++ ;
				h = (BetaCumulative(xk, alpha, beta) - pk) / BetaProbability(xk, alpha, beta) ;
				double xnew = xk - h ;
				if (xnew <= 0)
					xnew = xk / 10 ;
				if (xnew >= 1)
					xnew = 1 - (1 - xk) / 10 ;
				xk = xnew ;
			}
			return (float)xk ;
		}
		/// Calculates the Inverse of the Beta Cumulative Distribution Function. Uses Newton's method to converge
		/// the solution.
		/// Reference:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964.
		/// </summary>
		/// <param name="Probability">Probability asociated with the beta distribution</param>
		/// <param name="alpha">alpha parameter of the distribution</param>
		/// <param name="beta">beta parameter of the distribution</param>
		public static Decimal BetaInv(Decimal Probability, Decimal alpha, Decimal beta)
		{
			if (Probability < 0 || Probability > 1 | alpha <= 0 | beta <= 0)
				return -1 ;
			if (Probability == 0)
				return 0 ;
			if (Probability == 1)
				return 1 ;
			int count_limit = 100 ;
			int count = 0 ;
			Decimal pk = Probability ;
			Decimal xk = alpha / (alpha + beta) ;
			if (xk == 0)
				xk = 1e-27M ;
			if (xk == 1)
				xk = 1 - 1e-27M ;
			Decimal h = 1 ;
			Decimal crit = 1e-27M ;
			while(Mathematics.Abs(h) > crit * Mathematics.Abs(xk) && Mathematics.Abs(h) > crit 
				&& count < count_limit)
			{
				count ++ ;
				h = (BetaCumulative(xk, alpha, beta) - pk) / BetaProbability(xk, alpha, beta) ;
				Decimal xnew = xk - h ;
				if (xnew <= 0)
					xnew = xk / 10 ;
				if (xnew >= 1)
					xnew = 1 - (1 - xk) / 10 ;
				xk = xnew ;
			}
			return xk ;
		}
		#endregion
		#region Beta Tools
		/// <summary>
		/// Evaluates the Beta Function with the specified arguments.
		/// </summary>
		public static double Beta(double alpha, double beta)
		{
			return Mathematics.Exp(GammaLn(alpha) + GammaLn(beta) - GammaLn(alpha + beta)) ;
		}
		/// <summary>
		/// Evaluates the Beta Function with the specified arguments.
		/// </summary>
		public static float Beta(float alpha, float beta)
		{
			return (float)Beta((double)alpha, (double)beta) ;
		}
		/// <summary>
		/// Evaluates the Beta Function with the specified arguments.
		/// </summary>
		public static Decimal Beta(Decimal alpha, Decimal beta)
		{
			return Calculus.Exp(GammaLn(alpha) + GammaLn(beta) - GammaLn(alpha + beta)) ;
		}
		/// <summary>
		/// Evaluates the Natural Logarithm of the Beta Function with the specified arguments.
		/// </summary>
		public static double BetaLn(double alpha, double beta)
		{
			return GammaLn(alpha) + GammaLn(beta) - GammaLn(alpha + beta) ;
		}
		/// <summary>
		/// Evaluates the Natural Logarithm of the Beta Function with the specified arguments.
		/// </summary>
		public static float BetaLn(float alpha, float beta)
		{
			return (float)BetaLn((double)alpha, (double)beta) ;
		}
		/// <summary>
		/// Evaluates the Natural Logarithm of the Beta Function with the specified arguments.
		/// </summary>
		public static Decimal BetaLn(Decimal alpha, Decimal beta)
		{
			return GammaLn(alpha) + GammaLn(beta) - GammaLn(alpha + beta) ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution Function of the Beta Function.
		/// </summary>
		/// <param name="X">Value to evaluate function</param>
		/// <param name="alpha">alpha parameter of the distribution</param>
		/// <param name="beta">beta parameter of the distribution</param>
		/// <param name="XLowerBound">Optional lower bound for X</param>
		/// <param name="XUpperBound">Optional upper bound for X</param>
		/// <remarks>This function uses the <code>BetaIncomplete</code> function to get result.</remarks>
		public static double BetaCumulative(double X, double alpha, double beta)
		{
			if (alpha <= 0 || beta <= 0)
				return double.NaN ;
			if (X >= 1)
				return 1 ;
			double Result = BetaIncomplete(X, alpha, beta) ;
			if (Result > 1) Result = 1 ;
			return Result ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution Function of the Beta Function.
		/// </summary>
		/// <param name="X">Value to evaluate function</param>
		/// <param name="alpha">alpha parameter of the distribution</param>
		/// <param name="beta">beta parameter of the distribution</param>
		/// <param name="XLowerBound">Optional lower bound for X</param>
		/// <param name="XUpperBound">Optional upper bound for X</param>
		/// <remarks>This function uses the <code>BetaIncomplete</code> function to get result.</remarks>
		public static float BetaCumulative(float X, float alpha, float beta)
		{
			return (float)BetaCumulative((double)X, (double)alpha, (double)beta) ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution Function of the Beta Function.
		/// </summary>
		/// <param name="X">Value to evaluate function</param>
		/// <param name="alpha">alpha parameter of the distribution</param>
		/// <param name="beta">beta parameter of the distribution</param>
		/// <param name="XLowerBound">Optional lower bound for X</param>
		/// <param name="XUpperBound">Optional upper bound for X</param>
		/// <remarks>This function uses the <code>BetaIncomplete</code> function to get result.</remarks>
		public static Decimal BetaCumulative(Decimal X, Decimal alpha, Decimal beta)
		{
			if (alpha <= 0 || beta <= 0)
				return 0 ;
			if (X >= 1)
				return 1 ;
			Decimal Result = BetaIncomplete(X, alpha, beta) ;
			if (Result > 1) Result = 1 ;
			return Result ;
		}
		/// <summary>
		/// Calculates the Probability Density Function of the Beta Function.
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 26.1.33.
		/// </summary>
		public static double BetaProbability(double X, double alpha, double beta)
		{
			if (alpha <= 0 || beta <= 0)
				return double.NaN ;
			if ((X == 0 && alpha < 1) || (X == 1 && beta < 1))
				return double.PositiveInfinity ;
			return Mathematics.Pow(X, alpha - 1) * Mathematics.Pow(1 - X, beta - 1) / Beta(alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Probability Density Function of the Beta Function.
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 26.1.33.
		/// </summary>
		public static float BetaProbability(float X, float alpha, float beta)
		{
			return (float)BetaProbability((double)X, (double)alpha, (double)beta) ;
		}
		/// <summary>
		/// Calculates the Probability Density Function of the Beta Function.
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 26.1.33.
		/// </summary>
		public static Decimal BetaProbability(Decimal X, Decimal alpha, Decimal beta)
		{
			if (alpha <= 0 || beta <= 0)
				return -1 ;
			if ((X == 0 && alpha < 1) || (X == 1 && beta < 1))
				return Decimal.MaxValue ;
			return Calculus.Pow(X, alpha - 1) * Calculus.Pow(1 - X, beta - 1) / Beta(alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Incomplete Beta Function for specified parameters. 
		/// </summary>
		/// <remarks>This function uses the <code>BetaCore</code> function to get result.
		/// </remarks>
		public static double BetaIncomplete(double X, double alpha, double beta)
		{
			double bt = Mathematics.Exp(GammaLn(alpha + beta) - GammaLn(alpha) - GammaLn(beta) +
				alpha * Mathematics.Log(X + (X==0? 1 : 0)) +
				beta * Mathematics.Log(1 - X + (X==1? 1 : 0))) ;
			if (X < (alpha + 1) / (alpha + beta + 2))
			{
				return bt * BetaCore(X, alpha, beta) / alpha ;
			}
			else
			{
				return 1 - bt * BetaCore(1 - X, beta, alpha) / beta ;
			}
		}
		/// <summary>
		/// Calculates the Incomplete Beta Function for specified parameters. 
		/// </summary>
		/// <remarks>This function uses the <code>BetaCore</code> function to get result.
		/// </remarks>
		public static float BetaIncomplete(float X, float alpha, float beta)
		{
			return (float)BetaIncomplete((double)X, (double)alpha, (double)beta) ;
		}
		/// <summary>
		/// Calculates the Incomplete Beta Function for specified parameters. 
		/// </summary>
		/// <remarks>This function uses the <code>BetaCore</code> function to get result.
		/// </remarks>
		public static Decimal BetaIncomplete(Decimal X, Decimal alpha, Decimal beta)
		{
			Decimal bt = Calculus.Exp(GammaLn(alpha + beta) - GammaLn(alpha) - GammaLn(beta) +
				alpha * Calculus.Log(X + (X==0? 1 : 0)) +
				beta * Calculus.Log(1 - X + (X==1? 1 : 0))) ;
			if (X < (alpha + 1) / (alpha + beta + 2))
			{
				return bt * BetaCore(X, alpha, beta) / alpha ;
			}
			else
			{
				return 1 - bt * BetaCore(1 - X, beta, alpha) / beta ;
			}
		}
		/// <summary>
		/// Core algorithm to calculate the Incomplete Beta Function.
		/// </summary>
		public static double BetaCore(double X, double alpha, double beta)
		{
			double a = alpha, b = beta ;
			double y = X ;
			double qab = a + b ;
			double qap = a + 1 ;
			double qam = a - 1 ;
			double am = 1 ;
			double bm = am ;
			y = am ;
			double bz = 1 - qab * X / qap ;
			double d = 0 ;
			double app = d ;
			double ap = d ;
			double bpp = d ;
			double bp = d ;
			double yold = d ;
			int m = 1 ;
			while(Mathematics.Abs(y - yold) > 10 * double.Epsilon * Mathematics.Abs(y))
			{
				double tem = 2 * m ;
				d = m * (b - m) * X / ((qam + tem) * (a + tem)) ;
				ap = y + d * am ;
				bp = bz + d * bm ;
				d = -(a + m) * (qab + m) * X / ((a + tem) * (qap + tem)) ;
				app = ap + d * y ;
				bpp = bp + d * bz ;
				yold = y ;
				am = ap / bpp ;
				bm = bp / bpp ;
				y = app / bpp ;
				if (m == 1)
					bz = 1 ;
				m ++ ;
			}
			return y ;
		}
		/// <summary>
		/// Core algorithm to calculate the Incomplete Beta Function.
		/// </summary>
		public static Decimal BetaCore(Decimal X, Decimal alpha, Decimal beta)
		{
			Decimal a = alpha, b = beta ;
			Decimal y = X ;
			Decimal qab = a + b ;
			Decimal qap = a + 1 ;
			Decimal qam = a - 1 ;
			Decimal am = 1 ;
			Decimal bm = am ;
			y = am ;
			Decimal bz = 1 - qab * X / qap ;
			Decimal d = 0 ;
			Decimal app = d ;
			Decimal ap = d ;
			Decimal bpp = d ;
			Decimal bp = d ;
			Decimal yold = d ;
			int m = 1 ;
			while(Mathematics.Abs(y - yold) > 10 * 1e-27M * Mathematics.Abs(y))
			{
				Decimal tem = 2 * m ;
				d = m * (b - m) * X / ((qam + tem) * (a + tem)) ;
				ap = y + d * am ;
				bp = bz + d * bm ;
				d = -(a + m) * (qab + m) * X / ((a + tem) * (qap + tem)) ;
				app = ap + d * y ;
				bpp = bp + d * bz ;
				yold = y ;
				am = ap / bpp ;
				bm = bp / bpp ;
				y = app / bpp ;
				if (m == 1)
					bz = 1 ;
				m ++ ;
			}
			return y ;		}
		#endregion
		#region BinomDist
		/// <summary>
		/// Calculates the Binomial Distribution of an individual term.
		/// </summary>
		/// <param name="SuccessesNumber">Number of success in trials</param>
		/// <param name="Trials">Number of trials</param>
		/// <param name="Probability">Probability of success for each trial</param>
		/// <param name="Cumulative">When this value is true, the result will be the probability that there are
		/// at most SuccessesNumber successes</param>
		public static Decimal BinomDist(int SuccessesNumber, int Trials, Decimal Probability, bool Cumulative)
		{
			Decimal OneMinusProbability = 1 - Probability, t = Probability / OneMinusProbability ;
			int n1 = Trials + 1 ;
			Decimal Result = Calculus.Pow(OneMinusProbability, Trials) ;
			Decimal CumulativeResult = Result ;
			for(int successes = 1; successes <= SuccessesNumber; successes++)
			{
				Result = (n1 - successes) * t * Result / successes ;
				CumulativeResult += Result ;
			}
			return Cumulative? CumulativeResult : Result ;
		}
		/// <summary>
		/// Calculates the Binomial Distribution of an individual term.
		/// </summary>
		/// <param name="SuccessesNumber">Number of success in trials</param>
		/// <param name="Trials">Number of trials</param>
		/// <param name="Probability">Probability of success for each trial</param>
		/// <param name="Cumulative">When this value is true, the result will be the probability that there are
		/// at most SuccessesNumber successes</param>
		public static double BinomDist(int SuccessesNumber, int Trials, double Probability, bool Cumulative)
		{
			double OneMinusProbability = 1 - Probability, t = Probability / OneMinusProbability ;
			int n1 = Trials + 1 ;
			double Result = Mathematics.Pow(OneMinusProbability, Trials) ;
			double CumulativeResult = Result ;
			for(int successes = 1; successes <= SuccessesNumber; successes++)
			{
				Result = (n1 - successes) * t * Result / successes ;
				CumulativeResult += Result ;
			}
			return Cumulative? CumulativeResult : Result ;
		}
		/// <summary>
		/// Calculates the Binomial Distribution of an individual term.
		/// </summary>
		/// <param name="SuccessesNumber">Number of success in trials</param>
		/// <param name="Trials">Number of trials</param>
		/// <param name="Probability">Probability of success for each trial</param>
		/// <param name="Cumulative">When this value is true, the result will be the probability that there are
		/// at most SuccessesNumber successes</param>
		public static double BinomDist(int SuccessesNumber, int Trials, float Probability, bool Cumulative)
		{
			return (float)BinomDist(SuccessesNumber, Trials, (double)Probability, Cumulative) ;
		}
		#endregion
		#region ChiDist
		/// <summary>
		/// Calculates the Chi-squared Cumulative Distribution Function. This function calculates the probability
		/// that a random variable 'x' takes a greater value than 'X': P(x&gt;X). 
		/// </summary>
		/// <remarks>If you want to get the probability that 'x' takes a smaller value than 'X' use 
		/// <code>ChiDistSmaller(X, DegressOfFreedom)</code> instead or just evaluate 
		/// <code>1 - ChiDist(X, DegressOfFreedom)</code> your self.
		/// </remarks>
		/// <remarks>This function uses <code>1 - Gamma(X, DegressOfFreedom / 2, 2)</code> to get the result.</remarks>
		/// <param name="X">Value of a Random variable</param>
		/// <param name="DegreesOfFreedom">Number of degrees of freedom</param>
		public static double ChiDist(double X, int DegressOfFreedom)
		{
			return 1 - GammaCumulative(X, (double)DegressOfFreedom / 2, 2) ;
		}
		/// <summary>
		/// Calculates the Chi-squared Cumulative Distribution Function. This function calculates the probability
		/// that a random variable 'x' takes a greater value than 'X': P(x&gt;X). 
		/// </summary>
		/// <remarks>If you want to get the probability that 'x' takes a smaller value than 'X' use 
		/// <code>ChiDistSmaller(X, DegressOfFreedom)</code> instead or just evaluate 
		/// <code>1 - ChiDist(X, DegressOfFreedom)</code> your self.
		/// </remarks>
		/// <remarks>This function uses <code>1 - Gamma(X, DegressOfFreedom / 2, 2)</code> to get the result.</remarks>
		/// <param name="X">Value of a Random variable</param>
		/// <param name="DegreesOfFreedom">Number of degrees of freedom</param>
		public static float ChiDist(float X, int DegressOfFreedom)
		{
			return 1f - GammaCumulative(X, DegressOfFreedom / 2f, 2f) ;
		}
		/// <summary>
		/// Calculates the Chi-squared Cumulative Distribution Function. This function calculates the probability
		/// that a random variable 'x' takes a greater value than 'X': P(x&gt;X). 
		/// </summary>
		/// <remarks>If you want to get the probability that 'x' takes a smaller value than 'X' use 
		/// <code>ChiDistSmaller(X, DegressOfFreedom)</code> instead or just evaluate 
		/// <code>1 - ChiDist(X, DegressOfFreedom)</code> your self.
		/// </remarks>
		/// <remarks>This function uses <code>1 - Gamma(X, DegressOfFreedom / 2, 2)</code> to get the result.</remarks>
		/// <param name="X">Value of a Random variable</param>
		/// <param name="DegreesOfFreedom">Number of degrees of freedom</param>
		public static Decimal ChiDist(Decimal X, int DegressOfFreedom)
		{
			return 1M - GammaCumulative(X, DegressOfFreedom / 2M, 2M) ;
		}
		/// <summary>
		/// Calculates the one tailed probability of the Chi-squared Distribution. This function calculates the probability
		/// that a random variable 'x' takes a smaller value than 'X': P(x&lt;X).
		/// </summary>
		/// <remarks>If you want to get the probability that 'x' takes a greater value than 'X' use 
		/// <code>ChiDist(X, DegressOfFreedom)</code> instead or just evaluate 
		/// <code>1 - ChiDistSmaller(X, DegressOfFreedom)</code> your self.
		/// </remarks>
		/// <remarks>This function uses <code>Gamma(X, DegressOfFreedom / 2, 2)</code> to get the result.</remarks>
		/// <param name="X">Value of a Random variable</param>
		/// <param name="DegreesOfFreedom">Number of degrees of freedom</param>
		public static double ChiDistSmaller(double X, int DegressOfFreedom)
		{
			return GammaCumulative(X, (double)DegressOfFreedom / 2.0, 2.0) ;
		}
		/// <summary>
		/// Calculates the one tailed probability of the Chi-squared Distribution. This function calculates the probability
		/// that a random variable 'x' takes a smaller value than 'X': P(x&lt;X).
		/// </summary>
		/// <remarks>If you want to get the probability that 'x' takes a greater value than 'X' use 
		/// <code>ChiDist(X, DegressOfFreedom)</code> instead or just evaluate 
		/// <code>1 - ChiDistSmaller(X, DegressOfFreedom)</code> your self.
		/// </remarks>
		/// <remarks>This function uses <code>Gamma(X, DegressOfFreedom / 2, 2)</code> to get the result.</remarks>
		/// <param name="X">Value of a Random variable</param>
		/// <param name="DegreesOfFreedom">Number of degrees of freedom</param>
		public static float ChiDistSmaller(float X, int DegressOfFreedom)
		{
			return GammaCumulative(X, DegressOfFreedom / 2f, 2f) ;
		}
		/// <summary>
		/// Calculates the one tailed probability of the Chi-squared Distribution. This function calculates the probability
		/// that a random variable 'x' takes a smaller value than 'X': P(x&lt;X).
		/// </summary>
		/// <remarks>If you want to get the probability that 'x' takes a greater value than 'X' use 
		/// <code>ChiDist(X, DegressOfFreedom)</code> instead or just evaluate 
		/// <code>1 - ChiDistSmaller(X, DegressOfFreedom)</code> your self.
		/// </remarks>
		/// <remarks>This function uses <code>Gamma(X, DegressOfFreedom / 2, 2)</code> to get the result.</remarks>
		/// <param name="X">Value of a Random variable</param>
		/// <param name="DegreesOfFreedom">Number of degrees of freedom</param>
		public static Decimal ChiDistSmaller(Decimal X, int DegressOfFreedom)
		{
			return GammaCumulative(X, DegressOfFreedom / 2M, 2M) ;
		}
		#endregion
		#region ChiInv
		/// <summary>
		/// Calculates the Inverse of the Chi-squared Cumulative Distribution Function, <code>Probability</code> fullfils
		/// P(x&lt;X).
		/// </summary>
		/// <param name="Probability">The probability of a random variable 'x' such that P(x&gt;X)</param>
		/// <param name="DegressOfFreedom">Parameter of Chi-squared Cumulative Distribution Function</param>
		/// <remarks>If you want to get X such that <code>Probability</code>=P(x&lt;X) use <code>ChiSmallerInv</code> instead</remarks>
		public static double ChiInv(double Probability, int DegressOfFreedom)
		{
			return GammaInv(1 - Probability, (double)DegressOfFreedom / 2, 2) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Chi-squared Cumulative Distribution Function, <code>Probability</code> fullfils
		/// P(x&lt;X).
		/// </summary>
		/// <param name="Probability">The probability of a random variable 'x' such that P(x&lt;X)</param>
		/// <param name="DegressOfFreedom">Parameter of the Chi-squared Cumulative Distribution Function</param>
		/// <remarks>If you want to get X such that <code>Probability</code>=P(x&gt;X) use <code>ChiInv</code> instead</remarks>
		public static double ChiSmallerInv(double Probability, int DegressOfFreedom)
		{
			return GammaInv(Probability, (double)DegressOfFreedom / 2, 2) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Chi-squared Cumulative Distribution Function, <code>Probability</code> fullfils
		/// P(x&lt;X).
		/// </summary>
		/// <param name="Probability">The probability of a random variable 'x' such that P(x&gt;X)</param>
		/// <param name="DegressOfFreedom">Parameter of Chi-squared Cumulative Distribution Function</param>
		/// <remarks>If you want to get X such that <code>Probability</code>=P(x&lt;X) use <code>ChiSmallerInv</code> instead</remarks>
		public static float ChiInv(float Probability, int DegressOfFreedom)
		{
			return GammaInv(1 - Probability, (float)DegressOfFreedom / 2, 2) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Chi-squared Cumulative Distribution Function, <code>Probability</code> fullfils
		/// P(x&lt;X).
		/// </summary>
		/// <param name="Probability">The probability of a random variable 'x' such that P(x&lt;X)</param>
		/// <param name="DegressOfFreedom">Parameter of the Chi-squared Cumulative Distribution Function</param>
		/// <remarks>If you want to get X such that <code>Probability</code>=P(x&gt;X) use <code>ChiInv</code> instead</remarks>
		public static float ChiSmallerInv(float Probability, int DegressOfFreedom)
		{
			return GammaInv(Probability, (float)DegressOfFreedom / 2, 2) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Chi-squared Cumulative Distribution Function, <code>Probability</code> fullfils
		/// P(x&lt;X).
		/// </summary>
		/// <param name="Probability">The probability of a random variable 'x' such that P(x&gt;X)</param>
		/// <param name="DegressOfFreedom">Parameter of Chi-squared Cumulative Distribution Function</param>
		/// <remarks>If you want to get X such that <code>Probability</code>=P(x&lt;X) use <code>ChiSmallerInv</code> instead</remarks>
		public static Decimal ChiInv(Decimal Probability, int DegressOfFreedom)
		{
			return GammaInv(1 - Probability, (Decimal)DegressOfFreedom / 2, 2) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Chi-squared Cumulative Distribution Function, <code>Probability</code> fullfils
		/// P(x&lt;X).
		/// </summary>
		/// <param name="Probability">The probability of a random variable 'x' such that P(x&lt;X)</param>
		/// <param name="DegressOfFreedom">Parameter of the Chi-squared Cumulative Distribution Function</param>
		/// <remarks>If you want to get X such that <code>Probability</code>=P(x&gt;X) use <code>ChiInv</code> instead</remarks>
		public static Decimal ChiSmallerInv(Decimal Probability, int DegressOfFreedom)
		{
			return GammaInv(Probability, (Decimal)DegressOfFreedom / 2, 2) ;
		}
		#endregion
		#region ChiTest
		/// <summary>
		/// Calculates the test for independence. CHITEST returns the value from the chi-squared (γ2) distribution for 
		/// the statistic and the appropriate degrees of freedom. You can use γ2 tests to determine whether hypothesized 
		/// results are verified by an experiment.
		/// </summary>
		/// <param name="actual">Table containing observed values</param>
		/// <param name="expected">Table containing expected values</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static double ChiTest(double[,] actual, double[,] expected)
		{
			if (actual.GetLength(0) != expected.GetLength(0) ||
				actual.GetLength(1) != expected.GetLength(1))
				throw new ArrayDimensionsException() ;
			double sum = 0 ;
			int I = actual.GetLength(0) ;
			int J = actual.GetLength(1) ;
			for(int i = 0; i < I; i ++)
				for(int j = 0; j < J; j ++)
					sum += Mathematics.Pow(actual[i, j] - expected[i, j], 2) / expected[i, j] ;
			int v = (I - 1) * (J - 1) ;
			return ChiDist(sum, v) ;
		}
		/// <summary>
		/// Calculates the test for independence. CHITEST returns the value from the chi-squared (γ2) distribution for 
		/// the statistic and the appropriate degrees of freedom. You can use γ2 tests to determine whether hypothesized 
		/// results are verified by an experiment.
		/// </summary>
		/// <param name="actual">Table containing observed values</param>
		/// <param name="expected">Table containing expected values</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static float ChiTest(float[,] actual, float[,] expected)
		{
			if (actual.GetLength(0) != expected.GetLength(0) ||
				actual.GetLength(1) != expected.GetLength(1))
				throw new ArrayDimensionsException() ;
			float sum = 0 ;
			int I = actual.GetLength(0) ;
			int J = actual.GetLength(1) ;
			for(int i = 0; i < I; i ++)
				for(int j = 0; j < J; j ++)
					sum += (float)(Mathematics.Pow(actual[i, j] - expected[i, j], 2) / expected[i, j]) ;
			int v = (I - 1) * (J - 1) ;
			return ChiDist(sum, v) ;
		}
		/// <summary>
		/// Calculates the test for independence. CHITEST returns the value from the chi-squared (γ2) distribution for 
		/// the statistic and the appropriate degrees of freedom. You can use γ2 tests to determine whether hypothesized 
		/// results are verified by an experiment.
		/// </summary>
		/// <param name="actual">Table containing observed values</param>
		/// <param name="expected">Table containing expected values</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static Decimal ChiTest(Decimal[,] actual, Decimal[,] expected)
		{
			if (actual.GetLength(0) != expected.GetLength(0) ||
				actual.GetLength(1) != expected.GetLength(1))
				throw new ArrayDimensionsException() ;
			Decimal sum = 0 ;
			int I = actual.GetLength(0) ;
			int J = actual.GetLength(1) ;
			for(int i = 0; i < I; i ++)
				for(int j = 0; j < J; j ++)
					sum += Calculus.Pow(actual[i, j] - expected[i, j], 2) / expected[i, j] ;
			int v = (I - 1) * (J - 1) ;
			return ChiDist(sum, v) ;
		}
		#endregion
		#region Confidence
		/// <summary>
		/// Calculates the confidence interval for a population mean.
		/// </summary>
		/// <param name="Alpha">Significance level</param>
		/// <param name="Std">Population Stadard Deviation</param>
		/// <param name="SampleSize">Size of the population</param>
		public static double Confidence(double Alpha, double Std, int SampleSize)
		{
			double Point = - NormInv(0.5 - (1 - Alpha) / 2, 0, Std) ;
			return Point /** Std *// Mathematics.Sqrt(SampleSize) ;
		}
		/// <summary>
		/// Calculates the confidence interval for a population mean.
		/// </summary>
		/// <param name="Alpha">Significance level</param>
		/// <param name="Std">Population Stadard Deviation</param>
		/// <param name="SampleSize">Size of the population</param>
		public static float Confidence(float Alpha, float Std, int SampleSize)
		{
			return (float)Confidence((double)Alpha, (double)Std, SampleSize) ;
		}
		/// <summary>
		/// Calculates the confidence interval for a population mean.
		/// </summary>
		/// <param name="Alpha">Significance level</param>
		/// <param name="Std">Population Stadard Deviation</param>
		/// <param name="SampleSize">Size of the population</param>
		public static Decimal Confidence(Decimal Alpha, Decimal Std, int SampleSize)
		{
			Decimal Point = - NormInv(0.5M - (1M - Alpha) / 2M, 0, Std) ;
			return Point /** Std *// Calculus.Sqrt(SampleSize) ;
		}
		#endregion
		#region Correl
		/// <summary>
		/// Caculates the correlation coeficient of parameters.
		/// </summary>
		public static double Correl(double[] array1, double[] array2)
		{
			double std1 = StDevP(array1) ;
			double std2 = StDevP(array2) ;
			double Cov = Covar(array1, array2) ;
			return  Cov / (std1 * std2) ;
		}
		/// <summary>
		/// Caculates the correlation coeficient of parameters.
		/// </summary>
		public static float Correl(float[] array1, float[] array2)
		{
			return  (float)Correl(ToDoubleArray(array1), ToDoubleArray(array2)) ;
		}
		/// <summary>
		/// Caculates the correlation coeficient of parameters.
		/// </summary>
		public static Decimal Correl(Decimal[] array1, Decimal[] array2)
		{
			Decimal std1 = StDevP(array1) ;
			Decimal std2 = StDevP(array2) ;
			Decimal Cov = Covar(array1, array2) ;
			return  Cov / (std1 * std2) ;
		}
		#endregion
		#region Covar
		/// <summary>
		/// Calculates the covariance of parameters.
		/// </summary>
		public static double Covar(double[] array1, double[] array2)
		{
			double mean1 = Average(array1) ;
			double mean2 = Average(array2) ;
			double xy = 0 ;
			double mean2x = 0 ;
			double mean1y = 0 ;
			for(int i = 0; i < array1.Length; i++)
			{
				xy += array1[i] * array2[i] ;
				mean2x += array1[i] * mean2 ;
				mean1y += array2[i] * mean1 ;
			}
			return (double)1/array1.Length * (xy - mean2x - mean1y) + mean1 * mean2 ;
		}
		/// <summary>
		/// Calculates the covariance of parameters.
		/// </summary>
		public static float Covar(float[] array1, float[] array2)
		{
			return (float)Covar(ToDoubleArray(array1), ToDoubleArray(array2)) ;
		}
		/// <summary>
		/// Calculates the covariance of parameters.
		/// </summary>
		public static Decimal Covar(Decimal[] array1, Decimal[] array2)
		{
			Decimal mean1 = Average(array1) ;
			Decimal mean2 = Average(array2) ;
			Decimal xy = 0 ;
			Decimal mean2x = 0 ;
			Decimal mean1y = 0 ;
			for(int i = 0; i < array1.Length; i++)
			{
				xy += array1[i] * array2[i] ;
				mean2x += array1[i] * mean2 ;
				mean1y += array2[i] * mean1 ;
			}
			return 1M/array1.Length * (xy - mean2x - mean1y) + mean1 * mean2 ;
		}
		#endregion
		#region Combinations
		/// <summary>
		/// Calculates the M size samples from an N sized population.
		/// </summary>
		/// <remarks>This function is uses the continuos extention of the Factorial Function(The Gamma Function)</remarks>
		public static double Combinations(int N, int M)
		{
			return Mathematics.Exp(GammaLn((double)N + 1) - GammaLn((double)M + 1) - 
				GammaLn((double)N - M + 1)) ;
		}
		/// <summary>
		/// Calculates the M size samples from an N sized population. Result is returned as a float
		/// </summary>
		/// <remarks>This function is uses the continuos extention of the Factorial Function(The Gamma Function)</remarks>
		public static float CombinationsF(int N, int M)
		{
			return (float)Combinations(N, M) ;
		}
		/// <summary>
		/// Calculates the M size samples from an N sized population. Result is returned as a Decimal
		/// </summary>
		/// <remarks>This function is uses the continuos extention of the Factorial Function(The Gamma Function)</remarks>
		public static Decimal CombinationsM(int N, int M)
		{
			return Calculus.Exp(GammaLn((Decimal)N + 1M) - GammaLn((Decimal)M + 1M) -
				GammaLn((Decimal)(N - M) + 1M)) ;
		}
		#endregion
		#region CritBinom
		/// <summary>
		/// Calculates the smallest value for which the cumulative binomial distribution is greater than or 
		/// equal to a criterion value.
		/// </summary>
		/// <param name="trials">Number of trials</param>
		/// <param name="Probability">Probability of success</param>
		/// <param name="alpha">Criterion value</param>
		public static int CritBinom(int trials, double Probability, double alpha)
		{
			for (int i = 0; i <= trials; i ++)
				if (BinomDist(i, trials, Probability, true) >= alpha)
					return i ;
			return -1 ;
		}
		/// <summary>
		/// Calculates the smallest value for which the cumulative binomial distribution is greater than or 
		/// equal to a criterion value.
		/// </summary>
		/// <param name="trials">Number of trials</param>
		/// <param name="Probability">Probability of success</param>
		/// <param name="alpha">Criterion value</param>
		public static int CritBinom(int trials, float Probability, float alpha)
		{
			return CritBinom(trials, (double)Probability, (double)alpha) ;
		}
		/// <summary>
		/// Calculates the smallest value for which the cumulative binomial distribution is greater than or 
		/// equal to a criterion value.
		/// </summary>
		/// <param name="trials">Number of trials</param>
		/// <param name="Probability">Probability of success</param>
		/// <param name="alpha">Criterion value</param>
		public static int CritBinom(int trials, Decimal Probability, Decimal alpha)
		{
			for (int i = 0; i <= trials; i ++)
				if (BinomDist(i, trials, Probability, true) >= alpha)
					return i ;
			return -1 ;
		}
		#endregion
		#region DevSq
		/// <summary>
		/// Calculates the sum of the square of deviations.
		/// </summary>
		/// <param name="X">Data values</param>
		public static double DevSq(double[] X)
		{
			double sumX2 = 0 ;
			double sumX = 0 ;
			double ave = Average(X) ;
			for(int i = 0; i < X.Length; i++)
			{
				sumX2 += X[i] * X[i] ;
				sumX += X[i] ;
			}
			return sumX2 - 2.0 * sumX * ave + ave * ave * X.Length;
		}
		/// <summary>
		/// Calculates the sum of the square of deviations.
		/// </summary>
		/// <param name="X">Data values</param>
		public static float DevSq(float[] X)
		{
			double sumX2 = 0 ;
			double sumX = 0 ;
			double ave = Average(X) ;
			for(int i = 0; i < X.Length; i++)
			{
				sumX2 += X[i] * X[i] ;
				sumX += X[i] ;
			}
			return (float)(sumX2 - 2.0 * sumX * ave + ave * ave * X.Length) ;
		}
		/// <summary>
		/// Calculates the sum of the square of deviations.
		/// </summary>
		/// <param name="X">Data values</param>
		public static Decimal DevSq(Decimal[] X)
		{
			Decimal sumX2 = 0 ;
			Decimal sumX = 0 ;
			Decimal ave = Average(X) ;
			for(int i = 0; i < X.Length; i++)
			{
				sumX2 += X[i] * X[i] ;
				sumX += X[i] ;
			}
			return sumX2 - 2M * sumX * ave + ave * ave * X.Length;
		}
		#endregion
		#region ExpDist
		/// <summary>
		/// Calculates the Exponential Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="lambda">Lambda parameter for Exponential Distribution</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function; if false,
		/// returns the Probability Density Function</param>
		public static double ExpDist(double X, double lambda, bool Cumulative)
		{
			return Cumulative ? (1 - Mathematics.Exp(-lambda * X)) : 
				(lambda * Mathematics.Exp(-lambda * X)) ;
		}
		/// <summary>
		/// Calculates the Exponential Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="lambda">Lambda parameter for Exponential Distribution</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function; if false,
		/// returns the Probability Density Function</param>
		public static float ExpDist(float X, float lambda, bool Cumulative)
		{
			return (float)ExpDist((double)X, (double)lambda, Cumulative) ;
		}
		/// <summary>
		/// Calculates the Exponential Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="lambda">Lambda parameter for Exponential Distribution</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function; if false,
		/// returns the Probability Density Function</param>
		public static Decimal ExpDist(Decimal X, Decimal lambda, bool Cumulative)
		{
			return Cumulative ? (1 - Calculus.Exp(-lambda * X)) : 
				(lambda * Calculus.Exp(-lambda * X)) ;
		}
		#endregion
		#region FDist
		/// <summary>
		/// Calculates the F Cumulative Distribution Function
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 26.6.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom1">Numerator degress of freedom</param>
		/// <param name="DegressOfFreedom2">Denominator degress of freedom</param>
		/// <returns></returns>
		public static double FDist(double X, int DegressOfFreedom1, int DegressOfFreedom2)
		{
			if (double.NaN == X || DegressOfFreedom1 <= 0 || DegressOfFreedom2 <= 0)
				return double.NaN ;
			double xx = X / (X + (double)DegressOfFreedom2 / DegressOfFreedom1) ;
			return 1 - BetaIncomplete(xx, (double)DegressOfFreedom1 / 2, (double)DegressOfFreedom2 / 2) ;
		}
		/// <summary>
		/// Calculates the F Cumulative Distribution Function
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 26.6.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom1">Numerator degress of freedom</param>
		/// <param name="DegressOfFreedom2">Denominator degress of freedom</param>
		/// <returns></returns>
		public static float FDist(float X, int DegressOfFreedom1, int DegressOfFreedom2)
		{
			return (float)FDist((double)X, DegressOfFreedom1, DegressOfFreedom2) ;
		}
		/// <summary>
		/// Calculates the F Cumulative Distribution Function
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 26.6.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom1">Numerator degress of freedom</param>
		/// <param name="DegressOfFreedom2">Denominator degress of freedom</param>
		/// <returns></returns>
		public static Decimal FDist(Decimal X, int DegressOfFreedom1, int DegressOfFreedom2)
		{
			if (DegressOfFreedom1 <= 0M || DegressOfFreedom2 <= 0M)
				return -1M ;
			Decimal xx = X / (X + (Decimal)DegressOfFreedom2 / DegressOfFreedom1) ;
			return 1M - BetaIncomplete(xx, (Decimal)DegressOfFreedom1 / 2M, (Decimal)DegressOfFreedom2 / 2M) ;
		}
		#endregion
		#region FInv
		/// <summary>
		/// Calculates the Inverse of the F Cumulative Distribution Function.
		/// </summary>
		/// <param name="Probability">Value to evaluate at</param>
		/// <param name="DegressOfFreedom1">Numerator degress of freedom</param>
		/// <param name="DegressOfFreedom2">Denominator degress of freedom</param>
		public static double FInv(double Probability, int DegressOfFreedom1, int DegressOfFreedom2)
		{
			double z = BetaInv(Probability, (double)DegressOfFreedom2 / 2, (double)DegressOfFreedom1 / 2) ;
			return (DegressOfFreedom2 / z - DegressOfFreedom2) / DegressOfFreedom1 ;
		}
		/// <summary>
		/// Calculates the Inverse of the F Cumulative Distribution Function.
		/// </summary>
		/// <param name="Probability">Value to evaluate at</param>
		/// <param name="DegressOfFreedom1">Numerator degress of freedom</param>
		/// <param name="DegressOfFreedom2">Denominator degress of freedom</param>
		public static float FInv(float Probability, int DegressOfFreedom1, int DegressOfFreedom2)
		{
			return (float)FInv((double)Probability, DegressOfFreedom1, DegressOfFreedom2) ;
		}
		/// <summary>
		/// Calculates the Inverse of the F Cumulative Distribution Function.
		/// </summary>
		/// <param name="Probability">Value to evaluate at</param>
		/// <param name="DegressOfFreedom1">Numerator degress of freedom</param>
		/// <param name="DegressOfFreedom2">Denominator degress of freedom</param>
		public static Decimal FInv(Decimal Probability, int DegressOfFreedom1, int DegressOfFreedom2)
		{
			Decimal z = BetaInv(Probability, (Decimal)DegressOfFreedom2 / 2, (Decimal)DegressOfFreedom1 / 2) ;
			return (DegressOfFreedom2 / z - DegressOfFreedom2) / DegressOfFreedom1 ;
		}
		#endregion
		#region Fisher
		/// <summary>
		/// Calculates the Fisher Transformation.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		public static double Fisher(double X)
		{
			return .5 * Mathematics.Log((1.0 + X)/(1.0 - X)) ;
		}
		/// <summary>
		/// Calculates the Fisher Transformation.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		public static float Fisher(float X)
		{
			return (float)(.5 * Mathematics.Log((1.0 + X)/(1.0 - X))) ;
		}
		/// <summary>
		/// Calculates the Fisher Transformation.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		public static Decimal Fisher(Decimal X)
		{
			return .5M * Calculus.Log((1M + X)/(1M - X)) ;
		}
		#endregion
		#region FisherInv
		/// <summary>
		/// Calculates the Inverse of the Fisher Transformation.
		/// </summary>
		/// <param name="Y">Value to evaluate at</param>
		public static double FisherInv(double Y)
		{
			return (Mathematics.Exp(2.0 * Y) - 1.0) / (Mathematics.Exp(2.0 * Y) + 1.0) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Fisher Transformation.
		/// </summary>
		/// <param name="Y">Value to evaluate at</param>
		public static float FisherInv(float Y)
		{
			return (float)((Mathematics.Exp(2.0 * Y) - 1.0) / (Mathematics.Exp(2.0 * Y) + 1.0)) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Fisher Transformation.
		/// </summary>
		/// <param name="Y">Value to evaluate at</param>
		public static Decimal FisherInv(Decimal Y)
		{
			return (Calculus.Exp(2M * Y) - 1M) / (Calculus.Exp(2M * Y) + 1M) ;
		}
		#endregion
		#region ForeCast
		/// <summary>
		/// Calculates, or predicts, a future value by using existing values.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="knownYs">Array of known Y values</param>
		/// <param name="knownXs">Array of known X values</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static double ForeCast(double X, double[] knownYs, double[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			double sumX = 0 ;
			double sumY = 0 ;
			double sumXY = 0 ;
			double sumXsq = 0 ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i++)
			{
				sumX += knownXs[i] ;
				sumY += knownYs[i] ;
				sumXY += knownXs[i] * knownYs[i] ;
				sumXsq += knownXs[i] * knownXs[i] ;
			}
			double b = (n * sumXY - sumX * sumY) / (n * sumXsq - sumX * sumX) ;
			double a = (sumY - b * sumX) / n ;
			return a + b * X ;
		}
		/// <summary>
		/// Calculates, or predicts, a future value by using existing values.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="knownYs">Array of known Y values</param>
		/// <param name="knownXs">Array of known X values</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static float ForeCast(float X, float[] knownYs, float[] knownXs)
		{
			return (float)ForeCast((double)X, ToDoubleArray(knownXs), ToDoubleArray(knownYs)) ;
		}
		/// <summary>
		/// Calculates, or predicts, a future value by using existing values.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="knownYs">Array of known Y values</param>
		/// <param name="knownXs">Array of known X values</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static Decimal ForeCast(Decimal X, Decimal[] knownYs, Decimal[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			Decimal sumX = 0 ;
			Decimal sumY = 0 ;
			Decimal sumXY = 0 ;
			Decimal sumXsq = 0 ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i++)
			{
				sumX += knownXs[i] ;
				sumY += knownYs[i] ;
				sumXY += knownXs[i] * knownYs[i] ;
				sumXsq += knownXs[i] * knownXs[i] ;
			}
			Decimal b = (n * sumXY - sumX * sumY) / (n * sumXsq - sumX * sumX) ;
			Decimal a = (sumY - b * sumX) / n ;
			return a + b * X ;
		}
		#endregion
		#region Frequency
		/// <summary>
		/// Calculates how often values occur within a range of values.
		/// </summary>
		/// <param name="data">Values for which you want to count frequencies.</param>
		/// <param name="bins">Intervals into which you want to group the values in data</param>
		public static int[] Frequency(double[] data, double[] bins)
		{
			int[] result = new int[bins.Length + 1] ;
			for(int i = 0; i < data.Length; i ++)
			{
				double x = data[i] ;
				if (x <= bins[0])
				{
					result[0] += 1 ;
					continue ;
				}
				else if (x > bins[bins.Length - 1])
				{
					result[result.Length - 1] += 1 ;
				}
				for(int j = 0; j < bins.Length - 1; j ++)
					if (x > bins[j] && x <= bins[j + 1])
					{
						result[j + 1] += 1 ;
						break ;
					}
			}
			return result ;
		}
		/// <summary>
		/// Calculates how often values occur within a range of values.
		/// </summary>
		/// <param name="data">Values for which you want to count frequencies.</param>
		/// <param name="bins">Intervals into which you want to group the values in data</param>
		public static int[] Frequency(float[] data, float[] bins)
		{
			return Frequency(ToDoubleArray(data), ToDoubleArray(bins)) ;
		}
		/// <summary>
		/// Calculates how often values occur within a range of values.
		/// </summary>
		/// <param name="data">Values for which you want to count frequencies.</param>
		/// <param name="bins">Intervals into which you want to group the values in data</param>
		public static int[] Frequency(Decimal[] data, Decimal[] bins)
		{
			int[] result = new int[bins.Length + 1] ;
			for(int i = 0; i < data.Length; i ++)
			{
				Decimal x = data[i] ;
				if (x <= bins[0])
				{
					result[0] += 1 ;
					continue ;
				}
				else if (x > bins[bins.Length - 1])
				{
					result[result.Length - 1] += 1 ;
				}
				for(int j = 0; j < bins.Length - 1; j ++)
					if (x > bins[j] && x <= bins[j + 1])
					{
						result[j + 1] += 1 ;
						break ;
					}
			}
			return result ;
		}
		#endregion
		#region FTest
		/// <summary>
		/// Returns the result of an F-test. An F-test returns the one-tailed probability that the variances 
		/// in X and Y are not significantly different. 
		/// </summary>
		/// <param name="X">Sample data X</param>
		/// <param name="Y">Sample data Y</param>
		public static double FTest(double[] X, double[] Y)
		{
			double F = FStatistic(X, Y) ;
			int N = Mathematics.Max(X.Length, Y.Length) ;
			int n = Mathematics.Min(X.Length, Y.Length) ;
			double p = FDist(F, N - 1, n - 1) ;
			return 2 * Mathematics.Min(1.0 - p, p) ;
		}
		/// <summary>
		/// Returns the result of an F-test. An F-test returns the one-tailed probability that the variances 
		/// in X and Y are not significantly different. 
		/// </summary>
		/// <param name="X">Sample data X</param>
		/// <param name="Y">Sample data Y</param>
		public static float FTest(float[] X, float[] Y)
		{
			float F = FStatistic(X, Y) ;
			int N = Mathematics.Max(X.Length, Y.Length) ;
			int n = Mathematics.Min(X.Length, Y.Length) ;
			float p = FDist(F, N - 1, n - 1) ;
			return 2F * Mathematics.Min(1.0F - p, p) ;
		}
		/// <summary>
		/// Returns the result of an F-test. An F-test returns the one-tailed probability that the variances 
		/// in X and Y are not significantly different. 
		/// </summary>
		/// <param name="X">Sample data X</param>
		/// <param name="Y">Sample data Y</param>
		public static Decimal FTest(Decimal[] X, Decimal[] Y)
		{
			Decimal F = FStatistic(X, Y) ;
			int N = Mathematics.Max(X.Length, Y.Length) ;
			int n = Mathematics.Min(X.Length, Y.Length) ;
			Decimal p = FDist(F, N - 1, n - 1) ;
			return 2 * Mathematics.Min(1M - p, p) ;
		}
		/// <summary>
		/// Calculates the F Statistic.
		/// </summary>
		/// <param name="X">Sample data X</param>
		/// <param name="Y">Sample data Y</param>
		public static double FStatistic(double[] X, double[] Y)
		{
			double sx = StDev(X) ;
			sx *= sx ;
			double sy = StDev(Y) ;
			sy *= sy ;
			double sM = Mathematics.Max(sx, sy) ;
			double sm = Mathematics.Min(sx, sy) ;
			return sM / sm ;
		}
		/// <summary>
		/// Calculates the F Statistic.
		/// </summary>
		/// <param name="X">Sample data X</param>
		/// <param name="Y">Sample data Y</param>
		public static float FStatistic(float[] X, float[] Y)
		{
			float sx = StDev(X) ;
			sx *= sx ;
			float sy = StDev(Y) ;
			sy *= sy ;
			float sM = Mathematics.Max(sx, sy) ;
			float sm = Mathematics.Min(sx, sy) ;
			return sM / sm ;
		}
		/// <summary>
		/// Calculates the F Statistic.
		/// </summary>
		/// <param name="X">Sample data X</param>
		/// <param name="Y">Sample data Y</param>
		public static Decimal FStatistic(Decimal[] X, Decimal[] Y)
		{
			Decimal sx = StDev(X) ;
			sx *= sx ;
			Decimal sy = StDev(Y) ;
			sy *= sy ;
			Decimal sM = Mathematics.Max(sx, sy) ;
			Decimal sm = Mathematics.Min(sx, sy) ;
			return sM / sm ;
		}
		#endregion
		#region GammaDist
		/// <summary>
		/// Calculates the Gamma Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of the Gamma Distribution</param>
		/// <param name="beta">Beta parameter of the Gamma Distribution</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function; if false,
		/// returns the Probability Density Function</param>
		public static double GammaDist(double X, double alpha, double beta, bool Cumulative)
		{
			return Cumulative? GammaCumulative(X, alpha, beta) : GammaProbability(X, alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Gamma Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of the Gamma Distribution</param>
		/// <param name="beta">Beta parameter of the Gamma Distribution</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function; if false,
		/// returns the Probability Density Function</param>
		public static float GammaDist(float X, float alpha, float beta, bool Cumulative)
		{
			return (float)GammaDist((double)X, (double)alpha, (double)beta, Cumulative) ;
		}
		/// <summary>
		/// Calculates the Gamma Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of the Gamma Distribution</param>
		/// <param name="beta">Beta parameter of the Gamma Distribution</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function; if false,
		/// returns the Probability Density Function</param>
		public static Decimal GammaDist(Decimal X, Decimal alpha, Decimal beta, bool Cumulative)
		{
			return Cumulative? GammaCumulative(X, alpha, beta) : GammaProbability(X, alpha, beta) ;
		}
		#endregion
		#region GammaInv
		/// <summary>
		/// Calculates the Inverse of the Gamma Cumulative Distribution Function
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 6.5.
		/// </summary>
		/// <param name="Probability"></param>
		/// <param name="alpha"></param>
		/// <param name="beta"></param>
		/// <returns></returns>
		public static double GammaInv(double Probability, double alpha, double beta)
		{
			double a = alpha ;
			double b = beta ;
			//			x = zeros(size(p));
			//
			//			k = find(p<0 | p>1 | a <= 0 | b <= 0);
			//			if any(k),
			//					tmp  = NaN;
			//					x(k) = tmp(ones(size(k)));
			//			end
			if (Probability < 0 || Probability > 1 || a <= 0 || b <= 0)
				return double.NaN ;
			//
			//			% The inverse cdf of 0 is 0, and the inverse cdf of 1 is 1.  
			//			k0 = find(p == 0 & a > 0 & b > 0);
			//			if any(k0),
			//					x(k0) = zeros(size(k0)); 
			//			end
			if (Probability == 0)
				return 0 ;
			//
			//			k1 = find(p == 1 & a > 0 & b > 0);
			//			if any(k1), 
			//					tmp = Inf;
			//					x(k1) = tmp(ones(size(k1))); 
			//			end
			if (Probability == 1)
				return double.PositiveInfinity ;
			//
			//			% Newton's Method
			//			% Permit no more than count_limit interations.
			//			count_limit = 100;
			//			count = 0;
			int count_limit = 100 ;
			int count = 0 ;
			//
			//			k = find(p > 0  &  p < 1 & a > 0 & b > 0);
			//			if (~any(k(:))), return; end
			//			pk = p(k);
			double pk = Probability ;
			//
			//			% Supply a starting guess for the iteration.
			//			%   Use a method of moments fit to the lognormal distribution. 
			//			mn = a(k) .* b(k);
			double mn = a * b ;
			//			v = mn .* b(k);
			double v = mn * b ;
			//			temp = log(v + mn .^ 2); 
			double temp = Mathematics.Log(v + mn * mn) ;
			//			mu = 2 * log(mn) - 0.5 * temp;
			double mu = 2 * Mathematics.Log(mn) - 0.5 * temp ;
			//			sigma = -2 * log(mn) + temp;
			double sigma = -2 * Mathematics.Log(mn) + temp ;
			//			xk = exp(norminv(pk,mu,sigma));
			double xk = Mathematics.Exp(NormInv(pk, mu, sigma)) ;
			//
			//			h = ones(size(pk)); 
			double h = 1 ;
			//
			//			% Break out of the iteration loop for three reasons:
			//			%  1) the last update is very small (compared to x)
			//			%  2) the last update is very small (compared to sqrt(eps))
			//			%  3) There are more than 100 iterations. This should NEVER happen. 
			//
			//			while(any(abs(h) > sqrt(eps)*abs(xk))  &  max(abs(h)) > sqrt(eps)    ...
			//																			& count < count_limit), 
			while(Mathematics.Abs(h) > Mathematics.Sqrt(double.Epsilon) * Mathematics.Abs(xk) &&
				Mathematics.Abs(h) > Mathematics.Sqrt(double.Epsilon) & count < count_limit)
			{
				//			                                 
				//					count = count + 1;
				count ++ ;
				//					h = (gamcdf(xk,a(k),b(k)) - pk) ./ gampdf(xk,a(k),b(k));
				h = (GammaCumulative(xk, a, b) - pk) / GammaProbability(xk, a, b) ;
				//					xnew = xk - h;
				double xnew = xk - h ;
				//					% Make sure that the current guess stays greater than zero.
				//					% When Newton's Method suggests steps that lead to negative guesses
				//					% take a step 9/10ths of the way to zero:
				//					ksmall = find(xnew < 0);
				//					if any(ksmall),
				//							xnew(ksmall) = xk(ksmall) / 10;
				//							h = xk-xnew;
				//					end
				if (xnew < 0)
				{
					xnew = xk / 10 ;
					h = xk - xnew ;
				}
				//					xk = xnew;
				xk = xnew ;
				//			end
			}
			//
			//
			//			% Store the converged value in the correct place
			//			x(k) = xk;
			//
			//			if count == count_limit, 
			//					fprintf('\nWarning: GAMINV did not converge.\n');
			//					str = 'The last step was:  ';
			//					outstr = sprintf([str,'%13.8f'],h);
			//					fprintf(outstr);
			//			end
			return xk ;
		}
		/// <summary>
		/// Calculates the Inverse of the Gamma Cumulative Distribution Function
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 6.5.
		/// </summary>
		/// <param name="Probability"></param>
		/// <param name="alpha"></param>
		/// <param name="beta"></param>
		/// <returns></returns>
		public static float GammaInv(float Probability, float alpha, float beta)
		{
			float a = alpha ;
			float b = beta ;
			if (Probability < 0 || Probability > 1 || a <= 0 || b <= 0)
				return float.NaN ;
			if (Probability == 0f)
				return 0 ;
			if (Probability == 1f)
				return float.PositiveInfinity ;
			int count_limit = 100 ;
			int count = 0 ;
			float pk = Probability ;
			float mn = a * b ;
			float v = mn * b ;
			float temp = (float)Mathematics.Log(v + mn * mn) ;
			float mu = 2f * (float)Mathematics.Log(mn) - 0.5f * temp ;
			float sigma = -2f * (float)Mathematics.Log(mn) + temp ;
			float xk = (float)Mathematics.Exp(NormInv(pk, mu, sigma)) ;
			float h = 1 ;
			while(Mathematics.Abs(h) > Mathematics.Sqrt(float.Epsilon) * Mathematics.Abs(xk) &&
				Mathematics.Abs(h) > Mathematics.Sqrt(float.Epsilon) & count < count_limit)
			{
				count ++ ;
				h = (GammaCumulative(xk, a, b) - pk) / GammaProbability(xk, a, b) ;
				float xnew = xk - h ;
				if (xnew < 0)
				{
					xnew = xk / 10 ;
					h = xk - xnew ;
				}
				xk = xnew ;
			}
			return xk ;
		}
		/// <summary>
		/// Calculates the Inverse of the Gamma Cumulative Distribution Function
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 6.5.
		/// </summary>
		/// <param name="Probability"></param>
		/// <param name="alpha"></param>
		/// <param name="beta"></param>
		/// <returns></returns>
		public static Decimal GammaInv(Decimal Probability, Decimal alpha, Decimal beta)
		{
			Decimal a = alpha ;
			Decimal b = beta ;
			if (Probability < 0 || Probability > 1 || a <= 0 || b <= 0)
				return Decimal.MinusOne ;
			if (Probability == 0)
				return 0 ;
			if (Probability == 1)
				return Decimal.MaxValue ;
			int count_limit = 100 ;
			int count = 0 ;
			Decimal pk = Probability ;
			Decimal mn = a * b ;
			Decimal v = mn * b ;
			Decimal temp = Calculus.Log(v + mn * mn) ;
			Decimal mu = 2 * Calculus.Log(mn) - 0.5M * temp ;
			Decimal sigma = -2 * Calculus.Log(mn) + temp ;
			Decimal xk = Calculus.Exp(NormInv(pk, mu, sigma)) ;
			Decimal h = 1 ;
			while(Mathematics.Abs(h) > 1e-27M * Mathematics.Abs(xk) &&
				Mathematics.Abs(h) > 1e-27M & count < count_limit)
			{
				count ++ ;
				h = (GammaCumulative(xk, a, b) - pk) / GammaProbability(xk, a, b) ;
				Decimal xnew = xk - h ;
				if (xnew < 0)
				{
					xnew = xk / 10 ;
					h = xk - xnew ;
				}
				xk = xnew ;
			}
			return xk ;
		}
		#endregion
		#region Gamma Tools
		/// <summary>
		/// Calculates the Gamma Function in X.
		/// 
		/// This is based on a FORTRAN program by W. J. Cody, Argonne National Laboratory, NETLIB/SPECFUN, October 12, 1989.
		/// References: 
		/// 1) "An Overview of Software Development for Special Functions", W. J. Cody, Lecture Notes in Mathematics, 506, Numerical Analysis Dundee, 1975, G. A. Watson (ed.), Springer Verlag, Berlin, 1976. 
		/// 2) Computer Approximations, Hart, Et. Al., Wiley and sons, New York, 1968.
		/// </summary>
		public static double Gamma(double X)
		{
			#region Constants
			//			p = [-1.71618513886549492533811e+0; 2.47656508055759199108314e+1;
			//
			//					-3.79804256470945635097577e+2; 6.29331155312818442661052e+2;
			//
			//						8.66966202790413211295064e+2; -3.14512729688483675254357e+4;
			//
			//					-3.61444134186911729807069e+4; 6.64561438202405440627855e+4];
			double[] p =  {
							  -1.71618513886549492533811e+0, 2.47656508055759199108314e+1,
							  -3.79804256470945635097577e+2, 6.29331155312818442661052e+2,
							  8.66966202790413211295064e+2, -3.14512729688483675254357e+4,
							  -3.61444134186911729807069e+4, 6.64561438202405440627855e+4				
						  } ;
			//
			//			q = [-3.08402300119738975254353e+1; 3.15350626979604161529144e+2;
			//
			//					-1.01515636749021914166146e+3; -3.10777167157231109440444e+3;
			//
			//						2.25381184209801510330112e+4; 4.75584627752788110767815e+3;
			//
			//					-1.34659959864969306392456e+5; -1.15132259675553483497211e+5];
			double[] q =  {
							  -3.08402300119738975254353e+1, 3.15350626979604161529144e+2,
							  -1.01515636749021914166146e+3, -3.10777167157231109440444e+3,
							  2.25381184209801510330112e+4, 4.75584627752788110767815e+3,
							  -1.34659959864969306392456e+5, -1.15132259675553483497211e+5				
						  } ;
			//
			//			c = [-1.910444077728e-03; 8.4171387781295e-04;
			//
			//					-5.952379913043012e-04; 7.93650793500350248e-04;
			//
			//					-2.777777777777681622553e-03; 8.333333333333333331554247e-02;
			//
			//						5.7083835261e-03];
			double[] c =  {
							  -1.910444077728e-03, 8.4171387781295e-04,
							  -5.952379913043012e-04, 7.93650793500350248e-04,
							  -2.777777777777681622553e-03, 8.333333333333333331554247e-02,
							  5.7083835261e-03
						  } ;
			#endregion
			#region Map Negative argument to a Positive one
			//			%
			//
			//			%  Catch negative x.
			//
			//			%
			//
			//				kneg = find(x <= 0);
			//
			//				if ~isempty(kneg)
			//
			//						y = -x(kneg);
			//
			//						y1 = fix(y);
			//
			//						res(kneg) = y - y1;
			//
			//						fact = -pi ./ sin(pi*res(kneg)) .* (1 - 2*rem(y1,2));
			//
			//						x(kneg) = y + 1;
			//
			//				end
			//
			//			%
			//
			//			%  x is now positive.
			#endregion
			#region Calculates X within [0; 12)
			if (X >= 0 && X < 12)
			{
				//
				//			%  Map x in interval [0,1] to [1,2]
				//
				//			%
				//
				//				k1 = find(x < 1);
				//
				//				x1 = x(k1);
				//
				//				x(k1) = x1 + 1;
				//
				//			%
				double X1 = X ;
				double Xn = 0 ;
				bool Xw01 = false ;
				if (X < 1) 
				{
					X = X + 1 ;
					Xw01 = true ;
				}
				//
				//			%  Map x in interval [1,12] to [1,2]
				//
				//			%
				//
				//				k = find(x < 12);
				//
				//				xn(k) = fix(x(k)) - 1;
				//
				//				x(k) = x(k) - xn(k);
				//
				bool Xw112 = false ;
				if (X < 12)
				{
					Xn = Mathematics.Floor(X) - 1 ;
					X = X - Xn ;
					Xw112 = true ;
				}
				//			%
				//
				//			%  Evaluate approximation for 1 < x < 2
				//
				//			%
				//
				//				if ~isempty(k)
				//
				//						z = x(k) - 1;
				//
				//						xnum = 0*z;
				//
				//						xden = xnum + 1;
				//
				//						for i = 1:8
				//
				//							xnum = (xnum + p(i)) .* z;
				//
				//							xden = xden .* z + q(i);
				//
				//						end
				//
				//						res(k) = xnum ./ xden + 1;
				//
				//				end
				// Calculates approximation for X within [1; 2]
				double Z = X - 1 ;
				double xnum = 0 ;
				double xden = 1 ;
				for (int i = 0; i < 8; i++)
				{
					xnum = (xnum + p[i]) * Z ;
					xden = xden * Z + q[i] ;
				}
				double Result = xnum / xden + 1 ;
				//
				//			%
				//
				//			%  Adjust result for case  0.0 < x < 1.0
				//
				//			%
				//
				//				res(k1) = res(k1) ./ x1;
				//
				//			%
				if (Xw01)
					Result = Result / X1 ;
				//
				//			%  Adjust result for case  2.0 < x < 12.0
				//
				//			%
				//
				//				for j = 1:max(xn(:))
				//
				//						k = find(xn);
				//
				//						res(k) = res(k) .* x(k);
				//
				//						x(k) = x(k) + 1;
				//
				//						xn(k) = xn(k) - 1;
				//
				//				end
				if (Xw112)
				{
					while(Xn >= 1)
					{
						Result = Result * X ;
						X = X + 1 ;
						Xn = Xn - 1 ;
					}
				}
				return Result ;
			}
				#endregion
				#region Calculates X within [12; Inf)
				//			k = find(x >= 12);
				//
				//			if ~isempty(k)
				//
				//					y = x(k);
				//
				//					ysq = y .* y;
				//
				//					sum = c(7);
				//
				//					for i = 1:6
				//
				//						sum = sum ./ ysq + c(i);
				//
				//					end
				//
				//					spi = 0.9189385332046727417803297;
				//
				//					sum = sum ./ y - y + spi;
				//
				//					sum = sum + (y-0.5).*log(y);
				//
				//					res(k) = exp(sum);
				//
				//			end
			else
			{
				double y = X ;
				double ysq = y * y ;
				double sum = c[6] ;
				for(int i = 0; i < 6; i ++)
					sum = sum / ysq + c[i] ;
				double spi = 0.9189385332046727417803297 ;
				sum = sum / y - y + spi ;
				sum = sum + (y - 0.5) * Mathematics.Log(y) ;
				return Mathematics.Exp(sum) ;
			}
			#endregion
		}
		/// <summary>
		/// Calculates the Gamma Function in X.
		/// 
		/// This is based on a FORTRAN program by W. J. Cody, Argonne National Laboratory, NETLIB/SPECFUN, October 12, 1989.
		/// References: 
		/// 1) "An Overview of Software Development for Special Functions", W. J. Cody, Lecture Notes in Mathematics, 506, Numerical Analysis Dundee, 1975, G. A. Watson (ed.), Springer Verlag, Berlin, 1976. 
		/// 2) Computer Approximations, Hart, Et. Al., Wiley and sons, New York, 1968.
		/// </summary>
		public static float Gamma(float X)
		{
			return (float)Gamma((double)X) ;
		}
		/// <summary>
		/// Calculates the Gamma Function in X.
		/// 
		/// This is based on a FORTRAN program by W. J. Cody, Argonne National Laboratory, NETLIB/SPECFUN, October 12, 1989.
		/// References: 
		/// 1) "An Overview of Software Development for Special Functions", W. J. Cody, Lecture Notes in Mathematics, 506, Numerical Analysis Dundee, 1975, G. A. Watson (ed.), Springer Verlag, Berlin, 1976. 
		/// 2) Computer Approximations, Hart, Et. Al., Wiley and sons, New York, 1968.
		/// </summary>
		public static Decimal Gamma(Decimal X)
		{
			#region Constants
			Decimal[] p =  {
							   -1.71618513886549492533811e+0M, 2.47656508055759199108314e+1M,
							   -3.79804256470945635097577e+2M, 6.29331155312818442661052e+2M,
							   8.66966202790413211295064e+2M, -3.14512729688483675254357e+4M,
							   -3.61444134186911729807069e+4M, 6.64561438202405440627855e+4M				
						   } ;
			Decimal[] q =  {
							   -3.08402300119738975254353e+1M, 3.15350626979604161529144e+2M,
							   -1.01515636749021914166146e+3M, -3.10777167157231109440444e+3M,
							   2.25381184209801510330112e+4M, 4.75584627752788110767815e+3M,
							   -1.34659959864969306392456e+5M, -1.15132259675553483497211e+5M				
						   } ;
			Decimal[] c =  {
							   -1.910444077728e-03M, 8.4171387781295e-04M,
							   -5.952379913043012e-04M, 7.93650793500350248e-04M,
							   -2.777777777777681622553e-03M, 8.333333333333333331554247e-02M,
							   5.7083835261e-03M
						   } ;
			#endregion
			#region Map Negative argument to a Positive one
			#endregion
			#region Calculates X within [0; 12)
			if (X >= 0 && X < 12)
			{
				Decimal X1 = X ;
				Decimal Xn = 0 ;
				bool Xw01 = false ;
				if (X < 1) 
				{
					X = X + 1 ;
					Xw01 = true ;
				}
				bool Xw112 = false ;
				if (X < 12)
				{
					Xn = Calculus.Floor(X) - 1 ;
					X = X - Xn ;
					Xw112 = true ;
				}
				Decimal Z = X - 1 ;
				Decimal xnum = 0 ;
				Decimal xden = 1 ;
				for (int i = 0; i < 8; i++)
				{
					xnum = (xnum + p[i]) * Z ;
					xden = xden * Z + q[i] ;
				}
				Decimal Result = xnum / xden + 1 ;
				if (Xw01)
					Result = Result / X1 ;
				if (Xw112)
				{
					while(Xn >= 1)
					{
						Result = Result * X ;
						X = X + 1 ;
						Xn = Xn - 1 ;
					}
				}
				return Result ;
			}
				#endregion
				#region Calculates X within [12; Inf)
			else
			{
				Decimal y = X ;
				Decimal ysq = y * y ;
				Decimal sum = c[6] ;
				for(int i = 0; i < 6; i ++)
					sum = sum / ysq + c[i] ;
				Decimal spi = 0.9189385332046727417803297M ;
				sum = sum / y - y + spi ;
				sum = sum + (y - 0.5M) * Calculus.Log(y) ;
				return Calculus.Exp(sum) ;
			}
			#endregion		
		}
		/// <summary>
		/// Calculates the Natural Logarithm of the Gamma Function without calculating Gamma(X).
		/// 
		/// This is based on a FORTRAN program by W. J. Cody, Argonne National Laboratory, NETLIB/SPECFUN, June 16, 1988.
		/// 
		/// References:
		///  1) W. J. Cody and K. E. Hillstrom, 'Chebyshev Approximations for the Natural Logarithm of the Gamma Function,' Math. Comp. 21, 1967, pp. 198-203.
		///  2) K. E. Hillstrom, ANL/AMD Program ANLC366S, DGAMMA/DLGAMA, May, 1969.
		///  3) Hart, Et. Al., Computer Approximations, Wiley and sons, New York, 1968.
		/// </summary>
		public static double GammaLn(double X)
		{
			#region Constants
			double d1 = -5.772156649015328605195174e-1;
			double[] p1 = { 
							  4.945235359296727046734888e0, 2.018112620856775083915565e2, 
							  2.290838373831346393026739e3, 1.131967205903380828685045e4, 
							  2.855724635671635335736389e4, 3.848496228443793359990269e4, 
							  2.637748787624195437963534e4, 7.225813979700288197698961e3
						  } ;
			double[] q1 = { 
							  6.748212550303777196073036e1, 1.113332393857199323513008e3, 
							  7.738757056935398733233834e3, 2.763987074403340708898585e4, 
							  5.499310206226157329794414e4, 6.161122180066002127833352e4, 
							  3.635127591501940507276287e4, 8.785536302431013170870835e3
						  } ;
			double d2 = 4.227843350984671393993777e-1;
			double[] p2 = {
							  4.974607845568932035012064e0, 5.424138599891070494101986e2, 
							  1.550693864978364947665077e4, 1.847932904445632425417223e5, 
							  1.088204769468828767498470e6, 3.338152967987029735917223e6, 
							  5.106661678927352456275255e6, 3.074109054850539556250927e6		
						  };
			double[] q2 = {
							  1.830328399370592604055942e2, 7.765049321445005871323047e3, 
							  1.331903827966074194402448e5, 1.136705821321969608938755e6, 
							  5.267964117437946917577538e6, 1.346701454311101692290052e7, 
							  1.782736530353274213975932e7, 9.533095591844353613395747e6		
						  } ;
			double d4 = 1.791759469228055000094023e0;
			double[] p4 = {
							  1.474502166059939948905062e4, 2.426813369486704502836312e6, 
							  1.214755574045093227939592e8, 2.663432449630976949898078e9, 
							  2.940378956634553899906876e10, 1.702665737765398868392998e11, 
							  4.926125793377430887588120e11, 5.606251856223951465078242e11
						  };
			double[] q4 = {
							  2.690530175870899333379843e3, 6.393885654300092398984238e5, 
							  4.135599930241388052042842e7, 1.120872109616147941376570e9, 
							  1.488613728678813811542398e10, 1.016803586272438228077304e11, 
							  3.417476345507377132798597e11, 4.463158187419713286462081e11
						  } ;
			double[] c =  {
							  -1.910444077728e-03, 8.4171387781295e-04, 
							  -5.952379913043012e-04, 7.93650793500350248e-04, 
							  -2.777777777777681622553e-03, 8.333333333333333331554247e-02, 
							  5.7083835261e-03		 
						  } ;
			#endregion
			#region Setup first step
			double Result = X ;
			#endregion
			#region Calculates GammaLn(X) for X within [0; Eps]
			//			k = find(x <= eps);
			//
			//			if ~isempty(k)
			//
			//					res(k) = -log(x(k));
			//
			//			end
			if (X >= 0 && X <= double.Epsilon)
			{
				return - Mathematics.Log(X) ;
			}
			#endregion
			#region Calculates GammaLn(X) for X within (Eps; 0.5]
			//			k = find((x > eps) & (x <= 0.5));
			//
			//			if ~isempty(k)
			//
			//					y = x(k);
			//
			//					xden = ones(size(k));
			//
			//					xnum = 0;
			//
			//					for i = 1:8
			//
			//						xnum = xnum .* y + p1(i);
			//
			//						xden = xden .* y + q1(i);
			//
			//					end
			//
			//					res(k) = -log(y) + (y .* (d1 + y .* (xnum ./ xden)));
			//
			//			end
			if (X > double.Epsilon && X <= 0.5)
			{
				double xden = 1 ;
				double xnum = 0 ;
				for(int i = 0; i < 8; i++)
				{
					xnum = xnum * X + p1[i] ;
					xden = xden * X + q1[i] ;
				}
				return -Mathematics.Log(X) + (X * (d1 + X * (xnum / xden))) ;
			}
			#endregion
			#region Calculates GammaLn for x within (0.5; 0.6796875]
			//			k = find((x > 0.5) & (x <= 0.6796875));
			//
			//			if ~isempty(k)
			//
			//					xm1 = (x(k) - 0.5) - 0.5;
			//
			//					xden = ones(size(k));
			//
			//					xnum = 0;
			//
			//					for i = 1:8
			//
			//						xnum = xnum .* xm1 + p2(i);
			//
			//						xden = xden .* xm1 + q2(i);
			//
			//					end
			//
			//					res(k) = -log(x(k)) + xm1 .* (d2 + xm1 .* (xnum ./ xden));
			//
			//			end
			if (0.5 < X && X <= 0.6796875)
			{
				double xm1 = (X - 0.5) - 0.5 ;
				double xden = 1 ;
				double xnum = 0 ;
				for(int i = 0; i < 8; i++)
				{
					xnum = xnum * xm1 + p2[i] ;
					xden = xden * xm1 + q2[i] ;
				}
				return -Mathematics.Log(X) + xm1 * (d2 + xm1 * (xnum / xden)) ;
			}
			#endregion
			#region Calculates GammaLn for x within [0.6796875; 1.5]
			//			k = find((x > 0.6796875) & (x <= 1.5));
			//
			//			if ~isempty(k)
			//
			//					xm1 = (x(k) - 0.5) - 0.5;
			//
			//					xden = ones(size(k));
			//
			//					xnum = 0;
			//
			//					for i = 1:8
			//
			//						xnum = xnum .* xm1 + p1(i);
			//
			//						xden = xden .* xm1 + q1(i);
			//
			//					end
			//
			//					res(k) = xm1 .* (d1 + xm1 .* (xnum ./ xden));
			//
			//			end
			if (0.6796875 < X && X <= 1.5)
			{
				double xm1 = X - 1 ;
				double xden = 1 ;
				double xnum = 0 ;
				for(int i = 0; i < 8; i++)
				{
					xnum = xnum * xm1 + p1[i] ;
					xden = xden * xm1 + q1[i] ;
				}
				return xm1 * (d1 + xm1 * (xnum / xden)) ;
			}
			#endregion
			#region Calculates GammaLn for X within (1.5; 4]
			//			k = find((x > 1.5) & (x <= 4));
			//
			//			if ~isempty(k)
			//
			//					xm2 = x(k) - 2;
			//
			//					xden = ones(size(k));
			//
			//					xnum = 0;
			//
			//					for i = 1:8
			//
			//						xnum = xnum .* xm2 + p2(i);
			//
			//						xden = xden .* xm2 + q2(i);
			//
			//					end
			//
			//					res(k) = xm2 .* (d2 + xm2 .* (xnum ./ xden));
			//
			//			end
			if (1.5 < X && X <= 4)
			{
				double xm2 = X - 2 ;
				double xden = 1 ;
				double xnum = 0 ;
				for(int i = 0; i < 8; i++)
				{
					xnum = xnum * xm2 + p2[i] ;
					xden = xden * xm2 + q2[i] ;
				}
				return xm2 * (d2 + xm2 * (xnum / xden)) ;
			}
			#endregion
			#region Calculate GammaLn(X) for X within (4; 12]
			//			k = find((x > 4) & (x <= 12));
			//
			//			if ~isempty(k)
			//
			//					xm4 = x(k) - 4;
			//
			//					xden = -ones(size(k));
			//
			//					xnum = 0;
			//
			//					for i = 1:8
			//
			//						xnum = xnum .* xm4 + p4(i);
			//
			//						xden = xden .* xm4 + q4(i);
			//
			//					end
			//
			//					res(k) = d4 + xm4 .* (xnum ./ xden);
			//
			//			end
			if (4 < X && X <= 12)
			{
				double xm4 = X - 4 ;
				double xden = -1 ;
				double xnum = 0 ;
				for(int i = 0; i < 8; i++)
				{
					xnum = xnum * xm4 + p4[i] ;
					xden = xden * xm4 + q4[i] ;
				}
				return d4 + xm4 * (xnum / xden) ;
			}
			#endregion
			#region Calculate GammaLn(X) for X within (12;Inf)
			//			k = find(x > 12);
			//
			//			if ~isempty(k)
			//
			//					y = x(k);
			//
			//					r = c(7)*ones(size(k));
			//
			//					ysq = y .* y;
			//
			//					for i = 1:6
			//
			//						r = r ./ ysq + c(i);
			//
			//					end
			//
			//					r = r ./ y;
			//
			//					corr = log(y);
			//
			//					spi = 0.9189385332046727417803297;
			//
			//					res(k) = r + spi - 0.5*corr + y .* (corr-1);
			//
			//			end
			if (12 < X)
			{
				double y = X ;
				double r = c[6] ;
				double ysq = y * y ;
				for(int i = 0; i < 6; i++)
					r = r / ysq + c[i] ;
				r = r / y ;
				double corr = Mathematics.Log(y) ;
				double spi = 0.9189385332046727417803297 ;
				return r + spi - 0.5 * corr + y * (corr - 1) ;
			}
			#endregion
			return Result ;
		}
		/// <summary>
		/// Calculates the Natural Logarithm of the Gamma Function without calculating Gamma(X).
		/// 
		/// This is based on a FORTRAN program by W. J. Cody, Argonne National Laboratory, NETLIB/SPECFUN, June 16, 1988.
		/// 
		/// References:
		///  1) W. J. Cody and K. E. Hillstrom, 'Chebyshev Approximations for the Natural Logarithm of the Gamma Function,' Math. Comp. 21, 1967, pp. 198-203.
		///  2) K. E. Hillstrom, ANL/AMD Program ANLC366S, DGAMMA/DLGAMA, May, 1969.
		///  3) Hart, Et. Al., Computer Approximations, Wiley and sons, New York, 1968.
		/// </summary>
		public static float GammaLn(float X)
		{
			return (float)GammaLn((double)X) ;
		}
		/// <summary>
		/// Calculates the Natural Logarithm of the Gamma Function without calculating Gamma(X).
		/// 
		/// This is based on a FORTRAN program by W. J. Cody, Argonne National Laboratory, NETLIB/SPECFUN, June 16, 1988.
		/// 
		/// References:
		///  1) W. J. Cody and K. E. Hillstrom, 'Chebyshev Approximations for the Natural Logarithm of the Gamma Function,' Math. Comp. 21, 1967, pp. 198-203.
		///  2) K. E. Hillstrom, ANL/AMD Program ANLC366S, DGAMMA/DLGAMA, May, 1969.
		///  3) Hart, Et. Al., Computer Approximations, Wiley and sons, New York, 1968.
		/// </summary>
		public static Decimal GammaLn(Decimal X)
		{
			#region Constants
			Decimal d1 = -5.772156649015328605195174e-1M;
			Decimal[] p1 = { 
							   4.945235359296727046734888e0M, 2.018112620856775083915565e2M, 
							   2.290838373831346393026739e3M, 1.131967205903380828685045e4M, 
							   2.855724635671635335736389e4M, 3.848496228443793359990269e4M, 
							   2.637748787624195437963534e4M, 7.225813979700288197698961e3M
						   } ;
			Decimal[] q1 = { 
							   6.748212550303777196073036e1M, 1.113332393857199323513008e3M, 
							   7.738757056935398733233834e3M, 2.763987074403340708898585e4M, 
							   5.499310206226157329794414e4M, 6.161122180066002127833352e4M, 
							   3.635127591501940507276287e4M, 8.785536302431013170870835e3M
						   } ;
			Decimal d2 = 4.227843350984671393993777e-1M;
			Decimal[] p2 = {
							   4.974607845568932035012064e0M, 5.424138599891070494101986e2M, 
							   1.550693864978364947665077e4M, 1.847932904445632425417223e5M, 
							   1.088204769468828767498470e6M, 3.338152967987029735917223e6M, 
							   5.106661678927352456275255e6M, 3.074109054850539556250927e6M		
						   };
			Decimal[] q2 = {
							   1.830328399370592604055942e2M, 7.765049321445005871323047e3M, 
							   1.331903827966074194402448e5M, 1.136705821321969608938755e6M, 
							   5.267964117437946917577538e6M, 1.346701454311101692290052e7M, 
							   1.782736530353274213975932e7M, 9.533095591844353613395747e6M		
						   } ;
			Decimal d4 = 1.791759469228055000094023e0M;
			Decimal[] p4 = {
							   1.474502166059939948905062e4M, 2.426813369486704502836312e6M, 
							   1.214755574045093227939592e8M, 2.663432449630976949898078e9M, 
							   2.940378956634553899906876e10M, 1.702665737765398868392998e11M, 
							   4.926125793377430887588120e11M, 5.606251856223951465078242e11M
						   };
			Decimal[] q4 = {
							   2.690530175870899333379843e3M, 6.393885654300092398984238e5M, 
							   4.135599930241388052042842e7M, 1.120872109616147941376570e9M, 
							   1.488613728678813811542398e10M, 1.016803586272438228077304e11M, 
							   3.417476345507377132798597e11M, 4.463158187419713286462081e11M
						   } ;
			Decimal[] c =  {
							   -1.910444077728e-03M, 8.4171387781295e-04M, 
							   -5.952379913043012e-04M, 7.93650793500350248e-04M, 
							   -2.777777777777681622553e-03M, 8.333333333333333331554247e-02M, 
							   5.7083835261e-03M		 
						   } ;
			#endregion
			#region Setup first step
			Decimal Result = X ;
			#endregion
			#region Calculates GammaLn(X) for X within [0; 1e-27M]
			if (X >= 0 && X <= 1e-27M)
			{
				return - Calculus.Log(X) ;
			}
			#endregion
			#region Calculates GammaLn(X) for X within (Eps; 0.5]
			if (X > 1e-27M && X <= 0.5M)
			{
				Decimal xden = 1 ;
				Decimal xnum = 0 ;
				for(int i = 0; i < 8; i++)
				{
					xnum = xnum * X + p1[i] ;
					xden = xden * X + q1[i] ;
				}
				return -Calculus.Log(X) + (X * (d1 + X * (xnum / xden))) ;
			}
			#endregion
			#region Calculates GammaLn for x within (0.5; 0.6796875]
			if (0.5M < X && X <= 0.6796875M)
			{
				Decimal xm1 = (X - 0.5M) - 0.5M ;
				Decimal xden = 1 ;
				Decimal xnum = 0 ;
				for(int i = 0; i < 8; i++)
				{
					xnum = xnum * xm1 + p2[i] ;
					xden = xden * xm1 + q2[i] ;
				}
				return -Calculus.Log(X) + xm1 * (d2 + xm1 * (xnum / xden)) ;
			}
			#endregion
			#region Calculates GammaLn for x within [0.6796875; 1.5]
			if (0.6796875M < X && X <= 1.5M)
			{
				Decimal xm1 = X - 1 ;
				Decimal xden = 1 ;
				Decimal xnum = 0 ;
				for(int i = 0; i < 8; i++)
				{
					xnum = xnum * xm1 + p1[i] ;
					xden = xden * xm1 + q1[i] ;
				}
				return xm1 * (d1 + xm1 * (xnum / xden)) ;
			}
			#endregion
			#region Calculates GammaLn for X within (1.5; 4]
			if (1.5M < X && X <= 4M)
			{
				Decimal xm2 = X - 2 ;
				Decimal xden = 1 ;
				Decimal xnum = 0 ;
				for(int i = 0; i < 8; i++)
				{
					xnum = xnum * xm2 + p2[i] ;
					xden = xden * xm2 + q2[i] ;
				}
				return xm2 * (d2 + xm2 * (xnum / xden)) ;
			}
			#endregion
			#region Calculate GammaLn(X) for X within (4; 12]
			if (4M < X && X <= 12M)
			{
				Decimal xm4 = X - 4 ;
				Decimal xden = -1 ;
				Decimal xnum = 0 ;
				for(int i = 0; i < 8; i++)
				{
					xnum = xnum * xm4 + p4[i] ;
					xden = xden * xm4 + q4[i] ;
				}
				return d4 + xm4 * (xnum / xden) ;
			}
			#endregion
			#region Calculate GammaLn(X) for X within (12;Inf)
			if (12M < X)
			{
				Decimal y = X ;
				Decimal r = c[6] ;
				Decimal ysq = y * y ;
				for(int i = 0; i < 6; i++)
					r = r / ysq + c[i] ;
				r = r / y ;
				Decimal corr = Calculus.Log(y) ;
				Decimal spi = 0.9189385332046727417803297M ;
				return r + spi - 0.5M * corr + y * (corr - 1M) ;
			}
			#endregion
			return Result ;
		}
		/// <summary>
		/// Calculates the Gamma Cumulative Distribution Function.
		/// </summary>
		/// <param name="X">Value to Evaluate at</param>
		/// <param name="alpha">alpha parameter of Gamma Distribution</param>
		/// <param name="beta">beta parameter of Gamma Distribution</param>
		/// <remarks>This function uses <code>GammaIncomplete(X / beta, alpha)</code> to get the result. 
		/// <code>GammaIncomplete</code> is the Incomplete Gamma Function.</remarks>
		public static double GammaCumulative(double X, double alpha, double beta)
		{
			return GammaIncomplete(X / beta, alpha) ;
		}
		/// <summary>
		/// Calculates the Gamma Cumulative Distribution Function.
		/// </summary>
		/// <param name="X">Value to Evaluate at</param>
		/// <param name="alpha">alpha parameter of Gamma Distribution</param>
		/// <param name="beta">beta parameter of Gamma Distribution</param>
		/// <remarks>This function uses <code>GammaIncomplete(X / beta, alpha)</code> to get the result. 
		/// <code>GammaIncomplete</code> is the Incomplete Gamma Function.</remarks>
		public static float GammaCumulative(float X, float alpha, float beta)
		{
			return GammaIncomplete(X / alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Gamma Cumulative Distribution Function.
		/// </summary>
		/// <param name="X">Value to Evaluate at</param>
		/// <param name="alpha">alpha parameter of Gamma Distribution</param>
		/// <param name="beta">beta parameter of Gamma Distribution</param>
		/// <remarks>This function uses <code>GammaIncomplete(X / beta, alpha)</code> to get the result. 
		/// <code>GammaIncomplete</code> is the Incomplete Gamma Function.</remarks>
		public static Decimal GammaCumulative(Decimal X, Decimal alpha, Decimal beta)
		{
			return GammaIncomplete(X / alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Gamma Probability Density Function.
		/// </summary>
		/// <param name="X">Value to Evaluate at</param>
		/// <param name="alpha">alpha parameter of the Gamma Function</param>
		/// <param name="beta">beta parameter of the Gamma Function</param>
		public static double GammaProbability(double X, double alpha, double beta)
		{
			if (alpha <= 0 || beta <= 0)
				return double.NaN ;
			//			k=find(x > 0 & ~(a <= 0 | b <= 0));
			//			if any(k)
			//					y(k) = (a(k) - 1) .* log(x(k)) - (x(k) ./ b(k)) - gammaln(a(k)) - a(k) .* log(b(k));
			//					y(k) = exp(y(k));
			//			end
			if (X > 0)
			{
				return Mathematics.Exp((alpha - 1) * Mathematics.Log(X) - (X / beta) - GammaLn(alpha) - alpha * Mathematics.Log(beta)) ;
			}
			//			y(x == 0 & a < 1) = Inf;
			if (X == 0 && alpha < 1)
				return double.PositiveInfinity ;
			//			k2 = find(x == 0 & a == 1);
			//			if any(k2)
			//				y(k2) = (1./b(k2));
			//			end
			if (X == 0 && alpha == 1)
				return 1 / beta ;
			return double.NaN ;
		}
		/// <summary>
		/// Calculates the Gamma Probability Density Function.
		/// </summary>
		/// <param name="X">Value to Evaluate at</param>
		/// <param name="alpha">alpha parameter of the Gamma Function</param>
		/// <param name="beta">beta parameter of the Gamma Function</param>
		public static float GammaProbability(float X, float alpha, float beta)
		{
			return (float)GammaProbability((double)X, (double)alpha, (double)beta) ;
		}/// <summary>
		/// Calculates the Gamma Probability Density Function.
		/// </summary>
		/// <param name="X">Value to Evaluate at</param>
		/// <param name="alpha">alpha parameter of the Gamma Function</param>
		/// <param name="beta">beta parameter of the Gamma Function</param>
		public static Decimal GammaProbability(Decimal X, Decimal alpha, Decimal beta)
		{
			if (alpha <= 0 || beta <= 0)
				return -1 ;
			if (X > 0)
			{
				return Calculus.Exp((alpha - 1) * Calculus.Log(X) - (X / beta) - GammaLn(alpha) - alpha * Calculus.Log(beta)) ;
			}
			if (X == 0 && alpha < 1)
				return Decimal.MaxValue ;
			if (X == 0 && alpha == 1)
				return 1 / beta ;
			return -1 ;
		}
		/// <summary>
		/// Calculates the Incomplete Gamma Function.
		/// </summary>
		public static double GammaIncomplete(double X, double a)
		{
			#region Local Initializations
			//			gam = gammaln(a+realmin);
			double gam = GammaLn(a) ;
			//			if all(size(x)==1), x = x(ones(size(a))); end
			//			if all(size(a)==1), a = a(ones(size(x))); gam = gam(ones(size(x))); end
			//
			//			b = x;
			double Result = X ;
			//
			//			k = find(x == 0);
			//			if ~isempty(k)
			//				b(k) = 0;
			//			end
			//			k = find(a == 0);
			//			if ~isempty(k)
			//				b(k) = 1;
			//			end
			if (a == 0)
				Result = 1 ;
			#endregion
			#region Series expansion for X < a + 1
			if (X!= 0 && a != 0 && X < a + 1)
			{
				//			k = find((a ~= 0) & (x ~= 0) & (x < a+1));
				//			if ~isempty(k)
				//				ap = a(k);
				//				sum = 1./ap;
				//				del = sum;
				//				while norm(del,'inf') >= 10*eps*norm(sum,'inf')
				//						ap = ap + 1;
				//						del = x(k) .* del ./ ap;
				//						sum = sum + del;
				//				end
				//				b(k) = sum .* exp(-x(k) + a(k).*log(x(k)) - gam(k));
				//			end
				double ap = a ;
				double sum = 1 / ap ;
				double del = sum ;
				while(Mathematics.Abs(del) >= 10 * double.Epsilon * Mathematics.Abs(sum))
				{
					ap += 1 ;
					del = X * del / ap ;
					sum += del ;
				}
				return sum * Mathematics.Exp(-X + a * Mathematics.Log(X) - gam) ;
			}
			#endregion
			#region Continued fraction for x >= a+1
			if (X != 0 && a != 0 && X >= a + 1)
			{
				//			k = find((a ~= 0) & (x ~= 0) & (x >= a+1));
				//			if ~isempty(k)
				//				a0 = ones(size(k));
				//				a1 = x(k);
				//				b0 = zeros(size(k));
				//				b1 = a0;
				//				fac = 1;
				//				n = 1;
				//				g = b1;
				//				gold = b0;
				//				while norm(g-gold,'inf') >= 10*eps*norm(g,'inf');
				//						gold = g;
				//						ana = n - a(k);
				//						a0 = (a1 + a0 .*ana) .* fac;
				//						b0 = (b1 + b0 .*ana) .* fac;
				//						anf = n*fac;
				//						a1 = x(k) .* a0 + anf .* a1;
				//						b1 = x(k) .* b0 + anf .* b1;
				//						fac = 1 ./ a1;
				//						g = b1 .* fac;
				//						n = n + 1;
				//				end
				//				b(k) = 1 - exp(-x(k) + a(k).*log(x(k)) - gam(k)) .* g;
				//			end
				double a0 = 1 ;
				double a1 = X ;
				double b0 = 0 ;
				double b1 = a0 ;
				double fac = 1 ;
				int n = 1 ;
				double g = b1 ;
				double gold = b0 ;
				while(Mathematics.Abs(g - gold) >= 10 * double.Epsilon * Mathematics.Abs(g))
				{
					gold = g ;
					double ana = n - a ;
					a0 = (a1 + a0 * ana) * fac ;
					b0 = (b1 + b0 * ana) * fac ;
					double anf = n * fac ;
					a1 = X * a0 + anf * a1 ;
					b1 = X * b0 + anf * b1 ;
					fac = 1 / a1 ;
					g = b1 * fac ;
					n ++ ;
				}
				return 1 - Mathematics.Exp(-X + a * Mathematics.Log(X) - gam) * g ;
			}
			#endregion
			return Result ;
		}
		/// <summary>
		/// Calculates the Incomplete Gamma Function.
		/// </summary>
		public static float GammaIncomplete(float X, float a)
		{
			#region Local Initializations
			float gam = GammaLn(a) ;
			float Result = X ;
			if (a == 0)
				Result = 1 ;
			#endregion
			#region Series expansion for X < a + 1
			if (X!= 0 && a != 0 && X < a + 1)
			{
				float ap = a ;
				float sum = 1 / ap ;
				float del = sum ;
				while(Mathematics.Abs(del) >= 10 * float.Epsilon * Mathematics.Abs(sum))
				{
					ap += 1 ;
					del = X * del / ap ;
					sum += del ;
				}
				return sum * (float)Mathematics.Exp(-X + a * Mathematics.Log(X) - gam) ;
			}
			#endregion
			#region Continued fraction for x >= a+1
			if (X != 0 && a != 0 && X >= a + 1)
			{
				float a0 = 1 ;
				float a1 = X ;
				float b0 = 0 ;
				float b1 = a0 ;
				float fac = 1 ;
				int n = 1 ;
				float g = b1 ;
				float gold = b0 ;
				while(Mathematics.Abs(g - gold) >= 10 * float.Epsilon * Mathematics.Abs(g))
				{
					gold = g ;
					float ana = n - a ;
					a0 = (a1 + a0 * ana) * fac ;
					b0 = (b1 + b0 * ana) * fac ;
					float anf = n * fac ;
					a1 = X * a0 + anf * a1 ;
					b1 = X * b0 + anf * b1 ;
					fac = 1f / a1 ;
					g = b1 * fac ;
					n ++ ;
				}
				return 1f - (float)Mathematics.Exp(-X + a * Mathematics.Log(X) - gam) * g ;
			}
			#endregion
			return Result ;
		}
		/// <summary>
		/// Calculates the Incomplete Gamma Function.
		/// </summary>
		public static Decimal GammaIncomplete(Decimal X, Decimal a)
		{
			#region Local Initializations
			Decimal gam = GammaLn(a) ;
			Decimal Result = X ;
			if (a == 0)
				Result = 1 ;
			#endregion
			#region Series expansion for X < a + 1
			if (X!= 0 && a != 0 && X < a + 1)
			{
				Decimal ap = a ;
				Decimal sum = 1 / ap ;
				Decimal del = sum ;
				while(Mathematics.Abs(del) >= 10 * 1e-27M * Mathematics.Abs(sum))
				{
					ap += 1 ;
					del = X * del / ap ;
					sum += del ;
				}
				return sum * Calculus.Exp(-X + a * Calculus.Log(X) - gam) ;
			}
			#endregion
			#region Continued fraction for x >= a+1
			if (X != 0 && a != 0 && X >= a + 1)
			{
				Decimal a0 = 1 ;
				Decimal a1 = X ;
				Decimal b0 = 0 ;
				Decimal b1 = a0 ;
				Decimal fac = 1 ;
				int n = 1 ;
				Decimal g = b1 ;
				Decimal gold = b0 ;
				while(Mathematics.Abs(g - gold) >= 10 * 1e-27M * Mathematics.Abs(g))
				{
					gold = g ;
					Decimal ana = n - a ;
					a0 = (a1 + a0 * ana) * fac ;
					b0 = (b1 + b0 * ana) * fac ;
					Decimal anf = n * fac ;
					a1 = X * a0 + anf * a1 ;
					b1 = X * b0 + anf * b1 ;
					fac = 1 / a1 ;
					g = b1 * fac ;
					n ++ ;
				}
				return 1 - Calculus.Exp(-X + a * Calculus.Log(X) - gam) * g ;
			}
			#endregion
			return Result ;
		}
		#endregion
		#region GeoMean
		/// <summary>
		/// Calculates the Geometric Mean of an array
		/// </summary>
		/// <param name="X">Data array</param>
		public static double GeoMean(double[] X)
		{
			double mult = 1 ;
			for(int i = 0; i < X.Length; i ++)
				mult *= X[i] ;
			return Mathematics.Pow(mult, 1.0 / X.Length) ;
		}
		/// <summary>
		/// Calculates the Geometric Mean of an array
		/// </summary>
		/// <param name="X">Data array</param>
		public static float GeoMean(float[] X)
		{
			float mult = 1 ;
			for(int i = 0; i < X.Length; i ++)
				mult *= X[i] ;
			return (float)Mathematics.Pow(mult, 1.0 / X.Length) ;
		}
		/// <summary>
		/// Calculates the Geometric Mean of an array
		/// </summary>
		/// <param name="X">Data array</param>
		public static Decimal GeoMean(Decimal[] X)
		{
			Decimal mult = 1 ;
			for(int i = 0; i < X.Length; i ++)
				mult *= X[i] ;
			return Calculus.Pow(mult, 1M / X.Length) ;
		}
		#endregion
		#region Growth
		/// <summary>
		/// Calculates predicted exponential groth by exponential curve fitting existing data (y = b*m^x).
		/// </summary>
		/// <param name="knownYs">Known Y values et</param>
		/// <param name="knownXs">Known X values set</param>
		/// <param name="newXs">Values to predict curve</param>
		/// <param name="calculateIntercept">If true b is calculated, if false b is forced to 0</param>
		/// <returns>The y-values along that exponential curve for the array of newXs that you specify</returns>
		public static double[] Growth(double[] knownYs, double[] knownXs, double[] newXs, bool calculateIntercept)
		{
			SlopeInterceptPairD pair = CurveFit(knownYs, knownXs) ;
			if (!calculateIntercept)
				pair.Intercept = .0 ;
			double[] result = new double[newXs.Length] ;
			for(int i = 0; i < result.Length; i++)
			{
				result[i] = pair.Intercept * Mathematics.Pow(pair.Slope, newXs[i]) ;
			}
			return result ;
		}
		/// <summary>
		/// Calculates predicted exponential groth by exponential curve fitting existing data (y = b*m^x).
		/// </summary>
		/// <param name="knownYs">Known Y values et</param>
		/// <param name="knownXs">Known X values set</param>
		/// <param name="newXs">Values to predict curve</param>
		/// <param name="calculateIntercept">If true b is calculated, if false b is forced to 0</param>
		/// <returns>The y-values along that exponential curve for the array of newXs that you specify</returns>
		public static float[] Growth(float[] knownYs, float[] knownXs, float[] newXs, bool calculateIntercept)
		{
			SlopeInterceptPairF pair = CurveFit(knownYs, knownXs) ;
			if (!calculateIntercept)
				pair.Intercept = .0F ;
			float[] result = new float[newXs.Length] ;
			for(int i = 0; i < result.Length; i++)
			{
				result[i] = pair.Intercept * (float)Mathematics.Pow(pair.Slope, newXs[i]) ;
			}
			return result ;
		}
		/// <summary>
		/// Calculates predicted exponential groth by exponential curve fitting existing data (y = b*m^x).
		/// </summary>
		/// <param name="knownYs">Known Y values et</param>
		/// <param name="knownXs">Known X values set</param>
		/// <param name="newXs">Values to predict curve</param>
		/// <param name="calculateIntercept">If true b is calculated, if false b is forced to 0</param>
		/// <returns>The y-values along that exponential curve for the array of newXs that you specify</returns>
		public static Decimal[] Growth(Decimal[] knownYs, Decimal[] knownXs, Decimal[] newXs, bool calculateIntercept)
		{
			SlopeInterceptPairM pair = CurveFit(knownYs, knownXs) ;
			if (!calculateIntercept)
				pair.Intercept = .0M ;
			Decimal[] result = new Decimal[newXs.Length] ;
			for(int i = 0; i < result.Length; i++)
			{
				result[i] = pair.Intercept * Calculus.Pow(pair.Slope, newXs[i]) ;
			}
			return result ;
		}
		#endregion
		#region HarMean
		/// <summary>
		/// Calculates the Harmonic Mean of an array.
		/// </summary>
		/// <param name="X">Array of data</param>
		public static double HarMean(double[] X)
		{
			double sum = 0 ;
			for(int i = 0; i < X.Length; i++)
				sum += 1.0 / X[i] ;
			return 1.0 / (sum / X.Length) ;
		}
		/// <summary>
		/// Calculates the Harmonic Mean of an array.
		/// </summary>
		/// <param name="X">Array of data</param>
		public static float HarMean(float[] X)
		{
			float sum = 0 ;
			for(int i = 0; i < X.Length; i++)
				sum += 1.0F / X[i] ;
			return 1.0F / (sum / X.Length) ;
		}
		/// <summary>
		/// Calculates the Harmonic Mean of an array.
		/// </summary>
		/// <param name="X">Array of data</param>
		public static Decimal HarMean(Decimal[] X)
		{
			Decimal sum = 0 ;
			for(int i = 0; i < X.Length; i++)
				sum += 1.0M / X[i] ;
			return 1.0M / (sum / X.Length) ;
		}
		#endregion
		#region HypgeoDist
		/// <summary>
		/// Calculates the Hypergeometric Probability Density Function.
		/// Remarks:
		/// 1) Mood, Alexander M., Graybill, Franklin A. and Boes, Duane C.,"Introduction to the Theory of Statistics, Third Edition", McGraw Hill 1974 p. 91.
		/// </summary>
		/// <param name="Successes">Number of successes in a sample</param>
		/// <param name="NumberOfSamples">Sample size</param>
		/// <param name="PopulationSuccess">Number of successes in a sample</param>
		/// <param name="PopulationSize">Population Size</param>
		public static double HypGeoDist(int Successes, int SampleSize, int PopulationSuccesses, int PopulationSize)
		{
			double MX = Combinations(PopulationSuccesses, Successes) ;
			double NMnx = Combinations(PopulationSize - PopulationSuccesses, SampleSize - Successes) ;
			double Nn = Combinations(PopulationSize, SampleSize) ;
			return MX * NMnx / Nn ;
		}
		/// <summary>
		/// Calculates the Hypergeometric Probability Density Function.
		/// Remarks:
		/// 1) Mood, Alexander M., Graybill, Franklin A. and Boes, Duane C.,"Introduction to the Theory of Statistics, Third Edition", McGraw Hill 1974 p. 91.
		/// </summary>
		/// <param name="Successes">Number of successes in a sample</param>
		/// <param name="NumberOfSamples">Sample size</param>
		/// <param name="PopulationSuccess">Number of successes in a sample</param>
		/// <param name="PopulationSize">Population Size</param>
		public static float HypGeoDistF(int Successes, int SampleSize, int PopulationSuccesses, int PopulationSize)
		{
			return (float)HypGeoDist(Successes, SampleSize, PopulationSuccesses, PopulationSize) ;
		}
		/// <summary>
		/// Calculates the Hypergeometric Probability Density Function.
		/// Remarks:
		/// 1) Mood, Alexander M., Graybill, Franklin A. and Boes, Duane C.,"Introduction to the Theory of Statistics, Third Edition", McGraw Hill 1974 p. 91.
		/// </summary>
		/// <param name="Successes">Number of successes in a sample</param>
		/// <param name="NumberOfSamples">Sample size</param>
		/// <param name="PopulationSuccess">Number of successes in a sample</param>
		/// <param name="PopulationSize">Population Size</param>
		public static Decimal HypGeoDistM(int Successes, int SampleSize, int PopulationSuccesses, int PopulationSize)
		{
			Decimal MX = CombinationsM(PopulationSuccesses, Successes) ;
			Decimal NMnx = CombinationsM(PopulationSize - PopulationSuccesses, SampleSize - Successes) ;
			Decimal Nn = CombinationsM(PopulationSize, SampleSize) ;
			return MX * NMnx / Nn ;
		}
		#endregion
		#region Intercept
		/// <summary>
		/// Calculates the point at which a line will intersect the y-axis by using existing x-values and y-values
		/// </summary>
		/// <param name="knownYs">Array of known Y values</param>
		/// <param name="knownXs">Array of known X values</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static double Intercept(double[] knownYs, double[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			double sumX = 0 ;
			double sumY = 0 ;
			double sumXY = 0 ;
			double sumXsq = 0 ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i++)
			{
				sumX += knownXs[i] ;
				sumY += knownYs[i] ;
				sumXY += knownXs[i] * knownYs[i] ;
				sumXsq += knownXs[i] * knownXs[i] ;
			}
			double b = (n * sumXY - sumX * sumY) / (n * sumXsq - sumX * sumX) ;
			return (sumY - b * sumX) / n ;
		}
		/// <summary>
		/// Calculates the point at which a line will intersect the y-axis by using existing x-values and y-values
		/// </summary>
		/// <param name="knownYs">Array of known Y values</param>
		/// <param name="knownXs">Array of known X values</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static float Intercept(float[] knownYs, float[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			float sumX = 0 ;
			float sumY = 0 ;
			float sumXY = 0 ;
			float sumXsq = 0 ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i++)
			{
				sumX += knownXs[i] ;
				sumY += knownYs[i] ;
				sumXY += knownXs[i] * knownYs[i] ;
				sumXsq += knownXs[i] * knownXs[i] ;
			}
			float b = (n * sumXY - sumX * sumY) / (n * sumXsq - sumX * sumX) ;
			return (sumY - b * sumX) / n ;
		}
		/// <summary>
		/// Calculates the point at which a line will intersect the y-axis by using existing x-values and y-values
		/// </summary>
		/// <param name="knownYs">Array of known Y values</param>
		/// <param name="knownXs">Array of known X values</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static Decimal Intercept(Decimal[] knownYs, Decimal[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			Decimal sumX = 0 ;
			Decimal sumY = 0 ;
			Decimal sumXY = 0 ;
			Decimal sumXsq = 0 ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i++)
			{
				sumX += knownXs[i] ;
				sumY += knownYs[i] ;
				sumXY += knownXs[i] * knownYs[i] ;
				sumXsq += knownXs[i] * knownXs[i] ;
			}
			Decimal b = (n * sumXY - sumX * sumY) / (n * sumXsq - sumX * sumX) ;
			return (sumY - b * sumX) / n ;
		}
		#endregion
		#region Kurt
		/// <summary>
		/// Calculates the kurtosis of parameter data.
		/// </summary>
		public static double Kurt(double[] data)
		{
			int n = data.Length ;
			double sumX4M0 = 0.0 ;
			double sumX3M1 = 0.0 ;
			double sumX2M2 = 0.0 ;
			double sumX1M3 = 0.0 ;
			double sumX0M4 = 0.0 ;
			double std = StDev(data) ;
			double mean = Average(data) ;
			for(int i = 0; i < n; i++)
			{
				double x = data[i] ;
				sumX4M0 += x * x * x * x ;
				sumX3M1 += x * x * x * mean ;
				sumX2M2 += x * x * mean * mean ;
				sumX1M3 += x * mean * mean * mean ;
				sumX0M4 += mean * mean * mean * mean ;
			}
			double poli = (sumX4M0 - 4 * sumX3M1 + 6 * sumX2M2 - 4 * sumX1M3 + sumX0M4) / (std * std * std * std) ;
			poli *= n * (n + 1) / (double)((n - 1) * (n - 2) * (n - 3)) ;
			poli -= 3 * (n - 1) * (n - 1) / (double)((n - 2) * (n - 3)) ;
			return poli ;
		}
		/// <summary>
		/// Calculates the kurtosis of parameter data.
		/// </summary>
		public static float Kurt(float[] data)
		{
			int n = data.Length ;
			float sumX4M0 = 0.0F ;
			float sumX3M1 = 0.0F ;
			float sumX2M2 = 0.0F ;
			float sumX1M3 = 0.0F ;
			float sumX0M4 = 0.0F ;
			float std = StDev(data) ;
			float mean = Average(data) ;
			for(int i = 0; i < n; i++)
			{
				float x = data[i] ;
				sumX4M0 += x * x * x * x ;
				sumX3M1 += x * x * x * mean ;
				sumX2M2 += x * x * mean * mean ;
				sumX1M3 += x * mean * mean * mean ;
				sumX0M4 += mean * mean * mean * mean ;
			}
			float poli = (sumX4M0 - 4 * sumX3M1 + 6 * sumX2M2 - 4 * sumX1M3 + sumX0M4) / (std * std * std * std) ;
			poli *= n * (n + 1) / (float)((n - 1) * (n - 2) * (n - 3)) ;
			poli -= 3 * (n - 1) * (n - 1) / (float)((n - 2) * (n - 3)) ;
			return poli ;
		}
		/// <summary>
		/// Calculates the kurtosis of parameter data.
		/// </summary>
		public static Decimal Kurt(Decimal[] data)
		{
			int n = data.Length ;
			Decimal sumX4M0 = 0.0M ;
			Decimal sumX3M1 = 0.0M ;
			Decimal sumX2M2 = 0.0M ;
			Decimal sumX1M3 = 0.0M ;
			Decimal sumX0M4 = 0.0M ;
			Decimal std = StDev(data) ;
			Decimal mean = Average(data) ;
			for(int i = 0; i < n; i++)
			{
				Decimal x = data[i] ;
				sumX4M0 += x * x * x * x ;
				sumX3M1 += x * x * x * mean ;
				sumX2M2 += x * x * mean * mean ;
				sumX1M3 += x * mean * mean * mean ;
				sumX0M4 += mean * mean * mean * mean ;
			}
			Decimal poli = (sumX4M0 - 4 * sumX3M1 + 6 * sumX2M2 - 4 * sumX1M3 + sumX0M4) / (std * std * std * std) ;
			poli *= n * (n + 1) / (Decimal)((n - 1) * (n - 2) * (n - 3)) ;
			poli -= 3 * (n - 1) * (n - 1) / (Decimal)((n - 2) * (n - 3)) ;
			return poli ;
		}
		#endregion
		#region Large
		/// <summary>
		/// Locates the k-th largest number in data array.
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">k &le; 0 or k &gt; data.Length</exception>
		public static double Large(double[] data, int k)
		{
			return Pick(data, data.Length - k) ;
		}
		/// <summary>
		/// Locates the k-th largest number in data array.
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">k &le; 0 or k &gt; data.Length</exception>
		public static float Large(float[] data, int k)
		{
			return Pick(data, data.Length - k) ;
		}
		/// <summary>
		/// Locates the k-th largest number in data array.
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">k &le; 0 or k &gt; data.Length</exception>
		public static Decimal Large(Decimal[] data, int k)
		{
			return Pick(data, data.Length - k) ;
		}
		#endregion
		#region Linest
		/// <summary>
		/// Calculates the statistics for a line by using the "least squares" method to calculate a straight 
		/// line that best fits your data, and returns an struct that describes the line.
		/// </summary>
		/// <param name="knownYs">Known values for the dependent variable Y</param>
		/// <param name="knownXs">Known values for the independent variables Xi</param>
		/// <remarks>knownXs will contain an array of columns, each column will contain a single variable
		/// known values. Index will work such that knownXs[j][i] contains the i-th value of the j-th variable, the
		/// i-th row and the i-th column. Note the Index j,i is usually referred(in math books) as i,j</remarks>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters are not 
		/// right</exception>
		public static MultipleRegressionResultsD Linest(double[] knownYs, double[][] knownXs)
		{
			#region Locals
			double[] Y = knownYs ; // Alias to make code simpler
			double[][] X = knownXs ; // Same as above
			int N = X.Length ; // Independent variables count
			int P = Y.Length ; // Known values count
			int i, j ; // i will be used as the index for rows(values)
			// j will be used as the index for cols(variables)
			double aveY = Average(Y) ; // No comment
			#endregion

			#region Calculating sum/ave/var of columns of X (input data)
			// Contains the columns sum, there're X.Length columns
			double[] sumCol = new double[N] ;
			// Contains the columns mean.
			double[] aveCol = new double[N] ;
			double[] varCol = new double[N] ;
			for(j = 0; j < N; j++)
			{
				for(i = 0; i < P; i++)
					sumCol[j] += X[j][i] ;
				aveCol[j] = sumCol[j] / P ;
				varCol[j] = Var(X[j]) ;
			}
			#endregion

			#region Calculating sum/ave of rows of X (input data)
			// Contains the rows sum, there're Y.Length rows
			double[] sumRow = new double[P] ;
			// Contains the rows mean.
			double[] aveRow = new double[P] ;
			for(i = 0; i < P; i++)
			{
				for(j = 0; j < N; j++)
					sumRow[i] += X[j][i] ;
				aveRow[i] = sumRow[i] / N ;
			}
			#endregion

			#region Setting up Normal Equaltions System
			#region Coefficients Matrix
			//			double[,] L = new double[N, N] ;
			//			for(int li = 0; li < N; li++)
			//				for(int lj = 0; lj < N; lj++)
			//				{
			//					double sumXiXj = 0 ;
			//					for(i = 0; i < P; i++)
			//						sumXiXj += X[li][i] * X[lj][i] ;
			//					double aveXiXj = sumXiXj / P ;
			//					L[li, lj] = aveXiXj - aveCol[li] * aveCol[lj] ;
			//				}
			double[,] L = new double[N + 1, N + 1] ;
			L[0, 0] = P ;
			for(int li = 0; li < N; li++)
				L[li + 1, 0] = L[0, li + 1] = sumCol[li] ;
			for(int li = 0; li < N; li++)
				for(int lj = 0; lj < N; lj++)
				{
					double sumXiXj = .0 ;
					for(i = 0; i < P; i++)
						sumXiXj += X[li][i] * X[lj][i] ;
					L[li + 1, lj + 1] = sumXiXj ;

				}
			#endregion
			#region Independet Terms Vector
			double[] B = new double[N + 1] ;
			B[0] = Sum(Y) ;
			for(int bi = 0; bi < N; bi++)
			{
				double sumXjY = 0 ;
				for(i = 0; i < P; i ++)
					sumXjY += X[bi][i] * Y[i] ;
				B[bi + 1] = sumXjY ;
			}
			#endregion
			#endregion

			#region Solving Normal Equaltions System
			double[] Bi = new double[N + 1] ;
			int[] Pivot = new int[N + 1] ;
			double[,] W = new double[N + 1, N + 1] ;
			double[,] Inv = new double[N + 1, N + 1] ;
			MatrixD CovM = new MatrixD(Inv) ;
			Array.Copy(L, 0, W, 0, L.Length) ;
			MatrixD.Inverse(W, Inv, Pivot) ;
			MatrixD.Subst(W, Pivot, B, Bi) ;
			//			MatrixD.Solve(L, B, Bi) ; // This one holds the slopes, still have to calculate intercept
			#endregion

			#region Calculating other results
			#region Calculating intercept
			double intercept = Bi[0] ;
			//			for(int ix = 0; ix < N; ix++)
			//				intercept -= Bi[ix] * aveCol[ix] ;
			#endregion
			#region Calculating deviations
			double[] se = new double[N + 1] ;
			double[] Fi = new double[P] ;
			double ssresid = .0 ; // Residual sum of squares
			double ssreg = .0 ;  // Regression sum of squares
			double sstot = .0 ;  // Total sum of squares
			#region Iterating over columns
			for(i = 0; i < P; i++)
			{
				Fi[i] = intercept ; // B0
				#region Calculating Yestimate for i-th row
				for(j = 0; j < N; j++)
					Fi[i] += Bi[j + 1] * X[j][i] ; // Bj * Xij
				#endregion
				double diff = Y[i] - Fi[i] ;
				ssresid += diff * diff ; // Adds residual (Yobserved - Yestimate)^2
				diff = Y[i] - aveY ;
				sstot += diff * diff ; // Adds total sum of squares (Yobserved - Average(Y))^2
			}
			#endregion
			ssreg = sstot - ssresid ;
			double sey = ssresid / (P - N - 1.0) ;
			CovM.Multiply(sey) ;
			for(j = 0; j <= N; j++)
				se[j] = Mathematics.Sqrt(Inv[j,j]) ;
			sey = Mathematics.Sqrt(sey) ;
			int df = P - N - 1 ;
			#endregion
			#endregion

			#region Setting results
			MultipleRegressionResultsD results = new MultipleRegressionResultsD() ;
			results.coefficients = Bi ;
			results.ssreg = ssreg ;
			results.ssresid = ssresid ;
			results.r2 = ssreg / sstot ;
			results.df = df ;
			results.F = (ssreg / N)/ (ssresid / df);
			results.sey = sey ;
			results.se = se ;
			results.CoefficientsMatrix = new MatrixD(L) ;
			results.CovarianceMatrix = CovM ;
			#endregion
			
			return results ;
		}
		/// <summary>
		/// Calculates the statistics for a line by using the "least squares" method to calculate a straight 
		/// line that best fits your data, and returns an struct that describes the line.
		/// </summary>
		/// <param name="knownYs">Known values for the dependent variable Y</param>
		/// <param name="knownXs">Known values for the independent variables Xi</param>
		/// <remarks>knownXs will contain an array of columns, each column will contain a single variable
		/// known values. Index will work such that knownXs[j][i] contains the i-th value of the j-th variable, the
		/// i-th row and the i-th column. Note the Index j,i is usually referred(in math books) as i,j</remarks>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters are not 
		/// right</exception>
		public static MultipleRegressionResultsF Linest(float[] knownYs, float[][] knownXs)
		{
			#region Locals
			float[] Y = knownYs ; // Alias to make code simpler
			float[][] X = knownXs ; // Same as above
			int N = X.Length ; // Independent variables count
			int P = Y.Length ; // Known values count
			int i, j ; // i will be used as the index for rows(values)
			// j will be used as the index for cols(variables)
			float aveY = Average(Y) ; // No comment
			#endregion

			#region Calculating sum/ave/var of columns of X (input data)
			// Contains the columns sum, there're X.Length columns
			float[] sumCol = new float[N] ;
			// Contains the columns mean.
			float[] aveCol = new float[N] ;
			float[] varCol = new float[N] ;
			for(j = 0; j < N; j++)
			{
				for(i = 0; i < P; i++)
					sumCol[j] += X[j][i] ;
				aveCol[j] = sumCol[j] / P ;
				varCol[j] = Var(X[j]) ;
			}
			#endregion

			#region Calculating sum/ave of rows of X (input data)
			// Contains the rows sum, there're Y.Length rows
			float[] sumRow = new float[P] ;
			// Contains the rows mean.
			float[] aveRow = new float[P] ;
			for(i = 0; i < P; i++)
			{
				for(j = 0; j < N; j++)
					sumRow[i] += X[j][i] ;
				aveRow[i] = sumRow[i] / N ;
			}
			#endregion

			#region Setting up Normal Equaltions System
			#region Coefficients Matrix
			float[,] L = new float[N + 1, N + 1] ;
			L[0, 0] = P ;
			for(int li = 0; li < N; li++)
				L[li + 1, 0] = L[0, li + 1] = sumCol[li] ;
			for(int li = 0; li < N; li++)
				for(int lj = 0; lj < N; lj++)
				{
					float sumXiXj = .0F ;
					for(i = 0; i < P; i++)
						sumXiXj += X[li][i] * X[lj][i] ;
					L[li + 1, lj + 1] = sumXiXj ;

				}
			#endregion
			#region Independet Terms Vector
			float[] B = new float[N + 1] ;
			B[0] = Sum(Y) ;
			for(int bi = 0; bi < N; bi++)
			{
				float sumXjY = 0 ;
				for(i = 0; i < P; i ++)
					sumXjY += X[bi][i] * Y[i] ;
				B[bi + 1] = sumXjY ;
			}
			#endregion
			#endregion

			#region Solving Normal Equaltions System
			float[] Bi = new float[N + 1] ;
			int[] Pivot = new int[N + 1] ;
			float[,] W = new float[N + 1, N + 1] ;
			float[,] Inv = new float[N + 1, N + 1] ;
			MatrixF CovM = new MatrixF(Inv) ;
			Array.Copy(L, 0, W, 0, L.Length) ;
			MatrixF.Inverse(W, Inv, Pivot) ;
			MatrixF.Subst(W, Pivot, B, Bi) ;
			#endregion

			#region Calculating other results
			#region Calculating intercept
			float intercept = Bi[0] ;
			#endregion
			#region Calculating deviations
			float[] se = new float[N + 1] ;
			float[] Fi = new float[P] ;
			float ssresid = .0f ; // Residual sum of squares
			float ssreg = .0f ;  // Regression sum of squares
			float sstot = .0f ;  // Total sum of squares
			#region Iterating over columns
			for(i = 0; i < P; i++)
			{
				Fi[i] = intercept ; // B0
				#region Calculating Yestimate for i-th row
				for(j = 0; j < N; j++)
					Fi[i] += Bi[j + 1] * X[j][i] ; // Bj * Xij
				#endregion
				float diff = Y[i] - Fi[i] ;
				ssresid += diff * diff ; // Adds residual (Yobserved - Yestimate)^2
				diff = Y[i] - aveY ;
				sstot += diff * diff ; // Adds total sum of squares (Yobserved - Average(Y))^2
			}
			#endregion
			ssreg = sstot - ssresid ;
			float sey = ssresid / (P - N - 1.0F) ;
			CovM.Multiply(sey) ;
			for(j = 0; j <= N; j++)
				se[j] = (float)Mathematics.Sqrt(Inv[j,j]) ;
			sey = (float)Mathematics.Sqrt(sey) ;
			int df = P - N - 1 ;
			#endregion
			#endregion

			#region Setting results
			MultipleRegressionResultsF results = new MultipleRegressionResultsF() ;
			results.coefficients = Bi ;
			results.ssreg = ssreg ;
			results.ssresid = ssresid ;
			results.r2 = ssreg / sstot ;
			results.df = df ;
			results.F = (ssreg / N)/ (ssresid / df);
			results.sey = sey ;
			results.se = se ;
			results.CoefficientsMatrix = new MatrixF(L) ;
			results.CovarianceMatrix = CovM ;
			#endregion
			
			return results ;
		}
		/// <summary>
		/// Calculates the statistics for a line by using the "least squares" method to calculate a straight 
		/// line that best fits your data, and returns an struct that describes the line.
		/// </summary>
		/// <param name="knownYs">Known values for the dependent variable Y</param>
		/// <param name="knownXs">Known values for the independent variables Xi</param>
		/// <remarks>knownXs will contain an array of columns, each column will contain a single variable
		/// known values. Index will work such that knownXs[j][i] contains the i-th value of the j-th variable, the
		/// i-th row and the i-th column. Note the Index j,i is usually referred(in math books) as i,j</remarks>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters are not 
		/// right</exception>
		public static MultipleRegressionResultsM Linest(Decimal[] knownYs, Decimal[][] knownXs)
		{
			#region Locals
			Decimal[] Y = knownYs ; // Alias to make code simpler
			Decimal[][] X = knownXs ; // Same as above
			int N = X.Length ; // Independent variables count
			int P = Y.Length ; // Known values count
			int i, j ; // i will be used as the index for rows(values)
			// j will be used as the index for cols(variables)
			Decimal aveY = Average(Y) ; // No comment
			#endregion

			#region Calculating sum/ave/var of columns of X (input data)
			// Contains the columns sum, there're X.Length columns
			Decimal[] sumCol = new Decimal[N] ;
			// Contains the columns mean.
			Decimal[] aveCol = new Decimal[N] ;
			Decimal[] varCol = new Decimal[N] ;
			for(j = 0; j < N; j++)
			{
				for(i = 0; i < P; i++)
					sumCol[j] += X[j][i] ;
				aveCol[j] = sumCol[j] / P ;
				varCol[j] = Var(X[j]) ;
			}
			#endregion

			#region Calculating sum/ave of rows of X (input data)
			// Contains the rows sum, there're Y.Length rows
			Decimal[] sumRow = new Decimal[P] ;
			// Contains the rows mean.
			Decimal[] aveRow = new Decimal[P] ;
			for(i = 0; i < P; i++)
			{
				for(j = 0; j < N; j++)
					sumRow[i] += X[j][i] ;
				aveRow[i] = sumRow[i] / N ;
			}
			#endregion

			#region Setting up Normal Equaltions System
			#region Coefficients Matrix
			Decimal[,] L = new Decimal[N + 1, N + 1] ;
			L[0, 0] = P ;
			for(int li = 0; li < N; li++)
				L[li + 1, 0] = L[0, li + 1] = sumCol[li] ;
			for(int li = 0; li < N; li++)
				for(int lj = 0; lj < N; lj++)
				{
					Decimal sumXiXj = .0M ;
					for(i = 0; i < P; i++)
						sumXiXj += X[li][i] * X[lj][i] ;
					L[li + 1, lj + 1] = sumXiXj ;

				}
			#endregion
			#region Independet Terms Vector
			Decimal[] B = new Decimal[N + 1] ;
			B[0] = Sum(Y) ;
			for(int bi = 0; bi < N; bi++)
			{
				Decimal sumXjY = 0 ;
				for(i = 0; i < P; i ++)
					sumXjY += X[bi][i] * Y[i] ;
				B[bi + 1] = sumXjY ;
			}
			#endregion
			#endregion

			#region Solving Normal Equaltions System
			Decimal[] Bi = new Decimal[N + 1] ;
			int[] Pivot = new int[N + 1] ;
			Decimal[,] W = new Decimal[N + 1, N + 1] ;
			Decimal[,] Inv = new Decimal[N + 1, N + 1] ;
			MatrixM CovM = new MatrixM(Inv) ;
			Array.Copy(L, 0, W, 0, L.Length) ;
			MatrixM.Inverse(W, Inv, Pivot) ;
			MatrixM.Subst(W, Pivot, B, Bi) ;
			//			MatrixD.Solve(L, B, Bi) ; // This one holds the slopes, still have to calculate intercept
			#endregion

			#region Calculating other results
			#region Calculating intercept
			Decimal intercept = Bi[0] ;
			//			for(int ix = 0; ix < N; ix++)
			//				intercept -= Bi[ix] * aveCol[ix] ;
			#endregion
			#region Calculating deviations
			Decimal[] se = new Decimal[N + 1] ;
			Decimal[] Fi = new Decimal[P] ;
			Decimal ssresid = .0M ; // Residual sum of squares
			Decimal ssreg = .0M ;  // Regression sum of squares
			Decimal sstot = .0M ;  // Total sum of squares
			#region Iterating over columns
			for(i = 0; i < P; i++)
			{
				Fi[i] = intercept ; // B0
				#region Calculating Yestimate for i-th row
				for(j = 0; j < N; j++)
					Fi[i] += Bi[j + 1] * X[j][i] ; // Bj * Xij
				#endregion
				Decimal diff = Y[i] - Fi[i] ;
				ssresid += diff * diff ; // Adds residual (Yobserved - Yestimate)^2
				diff = Y[i] - aveY ;
				sstot += diff * diff ; // Adds total sum of squares (Yobserved - Average(Y))^2
			}
			#endregion
			ssreg = sstot - ssresid ;
			Decimal sey = ssresid / (P - N - 1.0M) ;
			CovM.Multiply(sey) ;
			for(j = 0; j <= N; j++)
				se[j] = Calculus.Sqrt(Inv[j,j]) ;
			sey = Calculus.Sqrt(sey) ;
			int df = P - N - 1 ;
			#endregion
			#endregion

			#region Setting results
			MultipleRegressionResultsM results = new MultipleRegressionResultsM() ;
			results.coefficients = Bi ;
			results.ssreg = ssreg ;
			results.ssresid = ssresid ;
			results.r2 = ssreg / sstot ;
			results.df = df ;
			results.F = (ssreg / N)/ (ssresid / df);
			results.sey = sey ;
			results.se = se ;
			results.CoefficientsMatrix = new MatrixM(L) ;
			results.CovarianceMatrix = CovM ;
			#endregion
			
			return results ;
		}
		#endregion
		#region Longest
		/// <summary>
		/// In regression analysis, calculates an exponential curve that fits your data and returns 
		/// an struct of values that describes the curve.
		/// </summary>
		/// <param name="knownYs">Known values for the dependent variable Y</param>
		/// <param name="knownXs">Known values for the independent variables Xi</param>
		/// <remarks>knownXs will contain an array of columns, each column will contain a single variable
		/// known values. Index will work such that knownXs[j][i] contains the i-th value of the j-th variable, the
		/// i-th row and the i-th column. Note the Index j,i is usually referred(in math books) as i,j</remarks>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters are not 
		/// right</exception>
		public static MultipleRegressionResultsD Logest(double[] knownYs, double[][] knownXs)
		{
			double[] Y = new double[knownYs.Length] ;
			for(int i = 0; i < knownYs.Length; i++)
				Y[i] = Mathematics.Log(knownYs[i]) ;
			MultipleRegressionResultsD results = Linest(Y, knownXs) ;
			double[] mi = results.coefficients ;
			for(int i = 0; i < knownXs.Length + 1; i++)
				mi[i] = Mathematics.Exp(mi[i]) ;
			return results ;
		}
		/// <summary>
		/// In regression analysis, calculates an exponential curve that fits your data and returns 
		/// an struct of values that describes the curve.
		/// </summary>
		/// <param name="knownYs">Known values for the dependent variable Y</param>
		/// <param name="knownXs">Known values for the independent variables Xi</param>
		/// <remarks>knownXs will contain an array of columns, each column will contain a single variable
		/// known values. Index will work such that knownXs[j][i] contains the i-th value of the j-th variable, the
		/// i-th row and the i-th column. Note the Index j,i is usually referred(in math books) as i,j</remarks>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters are not 
		/// right</exception>
		public static MultipleRegressionResultsF Logest(float[] knownYs, float[][] knownXs)
		{
			float[] Y = new float[knownYs.Length] ;
			for(int i = 0; i < knownYs.Length; i++)
				Y[i] = (float)Mathematics.Log(knownYs[i]) ;
			MultipleRegressionResultsF results = Linest(Y, knownXs) ;
			float[] mi = results.coefficients ;
			for(int i = 0; i < knownXs.Length + 1; i++)
				mi[i] = (float)Mathematics.Exp(mi[i]) ;
			return results ;
		}
		/// <summary>
		/// In regression analysis, calculates an exponential curve that fits your data and returns 
		/// an struct of values that describes the curve.
		/// </summary>
		/// <param name="knownYs">Known values for the dependent variable Y</param>
		/// <param name="knownXs">Known values for the independent variables Xi</param>
		/// <remarks>knownXs will contain an array of columns, each column will contain a single variable
		/// known values. Index will work such that knownXs[j][i] contains the i-th value of the j-th variable, the
		/// i-th row and the i-th column. Note the Index j,i is usually referred(in math books) as i,j</remarks>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters are not 
		/// right</exception>
		public static MultipleRegressionResultsM Logest(Decimal[] knownYs, Decimal[][] knownXs)
		{
			Decimal[] Y = new Decimal[knownYs.Length] ;
			for(int i = 0; i < knownYs.Length; i++)
				Y[i] = Calculus.Log(knownYs[i]) ;
			MultipleRegressionResultsM results = Linest(Y, knownXs) ;
			Decimal[] mi = results.coefficients ;
			for(int i = 0; i < knownXs.Length + 1; i++)
				mi[i] = Calculus.Exp(mi[i]) ;
			return results ;
		}
		#endregion
		#region LogInv
		/// <summary>
		/// Calculates the Inverse of the LogNormal Cumulative Distribution Function.
		/// </summary>
		/// <param name="Probability">Value to evaluate at</param>
		/// <param name="mu">Mu parameter of the LogNormal Distribution</param>
		/// <param name="sigma">Sigma parameter of the LogNormal Distribution</param>
		/// <returns></returns>
		public static double LogInv(double Probability, double mu, double sigma)
		{
			return Mathematics.Exp(NormInv(Probability, mu, sigma)) ;
		}
		/// <summary>
		/// Calculates the Inverse of the LogNormal Cumulative Distribution Function.
		/// </summary>
		/// <param name="Probability">Value to evaluate at</param>
		/// <param name="mu">Mu parameter of the LogNormal Distribution</param>
		/// <param name="sigma">Sigma parameter of the LogNormal Distribution</param>
		/// <returns></returns>
		public static float LogInv(float Probability, float mu, float sigma)
		{
			return (float)Mathematics.Exp(NormInv((double)Probability, (double)mu, (double)sigma)) ;
		}
		/// <summary>
		/// Calculates the Inverse of the LogNormal Cumulative Distribution Function.
		/// </summary>
		/// <param name="Probability">Value to evaluate at</param>
		/// <param name="mu">Mu parameter of the LogNormal Distribution</param>
		/// <param name="sigma">Sigma parameter of the LogNormal Distribution</param>
		/// <returns></returns>
		public static Decimal LogInv(Decimal Probability, Decimal mu, Decimal sigma)
		{
			return Calculus.Exp(NormInv(Probability, mu, sigma)) ;
		}
		#endregion
		#region LogNormDist
		/// <summary>
		/// Calculates the LogNormal Cumulative Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter of the LogNormal Distribution</param>
		/// <param name="sigma">Sigma Parameter of the LogNormal Distribution</param>
		/// <returns></returns>
		public static double LogNormDist(double X, double mu, double sigma)
		{
			return NormCumulative(Mathematics.Log(X), mu, sigma) ;
		}
		/// <summary>
		/// Calculates the LogNormal Cumulative Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter of the LogNormal Distribution</param>
		/// <param name="sigma">Sigma Parameter of the LogNormal Distribution</param>
		/// <returns></returns>
		public static float LogNormDist(float X, float mu, float sigma)
		{
			return (float)NormCumulative(Mathematics.Log(X), (double)mu, (double)sigma) ;
		}
		/// <summary>
		/// Calculates the LogNormal Cumulative Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter of the LogNormal Distribution</param>
		/// <param name="sigma">Sigma Parameter of the LogNormal Distribution</param>
		/// <returns></returns>
		public static Decimal LogNormDist(Decimal X, Decimal mu, Decimal sigma)
		{
			return (Decimal)NormCumulative(Calculus.Log(X), mu, sigma) ;
		}
		#endregion
		#region Max
		/// <summary>
		/// Gets the largest value in data.
		/// </summary>
		public static double Max(double[] data)
		{
			double max = double.MinValue ;
			foreach(double x in data)
				if (x > max)
					max = x ;
			return max ;
		}
		/// <summary>
		/// Gets the largest value in data.
		/// </summary>
		public static float Max(float[] data)
		{
			float max = float.MinValue ;
			foreach(float x in data)
				if (x > max)
					max = x ;
			return max ;
		}
		/// <summary>
		/// Gets the largest value in data.
		/// </summary>
		public static Decimal Max(Decimal[] data)
		{
			Decimal max = Decimal.MinValue ;

			foreach(Decimal x in data)
				if (x > max)
					max = x ;
			return max ;
		}
		#endregion
		#region Median
		/// <summary>
		/// Calculates the Median of data.
		/// </summary>
		public static double Median(double[] data)
		{
			int n = data.Length ;
			if (n % 2 == 1)
				return Pick(data, n / 2) ;
			return (Pick(data, n / 2 - 1) + Pick(data, n / 2)) / 2.0 ;
		}
		/// <summary>
		/// Calculates the Median of data.
		/// </summary>
		public static float Median(float[] data)
		{
			int n = data.Length ;
			if (n % 2 == 1)
				return Pick(data, n / 2) ;
			return (Pick(data, n / 2 - 1) + Pick(data, n / 2)) / 2.0f ;
		}
		/// <summary>
		/// Calculates the Median of data.
		/// </summary>
		public static Decimal Median(Decimal[] data)
		{
			int n = data.Length ;
			if (n % 2 == 1)
				return Pick(data, n / 2) ;
			return (Pick(data, n / 2 - 1) + Pick(data, n / 2)) / 2.0M ;
		}
		#endregion
		#region Min
		/// <summary>
		/// Gets the smallest value in data
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static double Min(double[] data)
		{
			double min = double.MaxValue ;
			foreach(double x in data)
				if (x < min)
					min = x ;
			return min ;
		}
		/// <summary>
		/// Gets the smallest value in data
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static float Min(float[] data)
		{
			float min = float.MaxValue ;
			foreach(float x in data)
				if (x < min)
					min = x ;
			return min ;
		}
		/// <summary>
		/// Gets the smallest value in data
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Decimal Min(Decimal[] data)
		{
			Decimal min = Decimal.MaxValue ;
			foreach(Decimal x in data)
				if (x < min)
					min = x ;
			return min ;
		}
		#endregion
		#region Mode
		/// <summary>
		/// Returns the most frequently ocurring element in data
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static double Mode(double[] data)
		{
			Hashtable h = new Hashtable() ;
			double mode = double.NaN ;
			int freq = 0 ;
			foreach(double x in data)
			{
				if (h[x] == null)
					h[x] = 0 ;
				else
				{
					int xfreq = (int)h[x] + 1 ;
					h[x] = xfreq ;
					if (freq < xfreq)
					{
						mode = x ;
						freq = xfreq ;
					}
				}
			}
			return mode ;
		}
		/// <summary>
		/// Returns the most frequently ocurring element in data
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static float Mode(float[] data)
		{
			Hashtable h = new Hashtable() ;
			float mode = float.NaN ;
			int freq = 0 ;
			foreach(float x in data)
			{
				if (h[x] == null)
					h[x] = 0 ;
				else
				{
					int xfreq = (int)h[x] + 1 ;
					h[x] = xfreq ;
					if (freq < xfreq)
					{
						mode = x ;
						freq = xfreq ;
					}
				}
			}
			return mode ;
		}
		/// <summary>
		/// Returns the most frequently ocurring element in data
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Decimal Mode(Decimal[] data)
		{
			Hashtable h = new Hashtable() ;
			Decimal mode = -1M ;
			int freq = 0 ;
			foreach(Decimal x in data)
			{
				if (h[x] == null)
					h[x] = 0 ;
				else
				{
					int xfreq = (int)h[x] + 1 ;
					h[x] = xfreq ;
					if (freq < xfreq)
					{
						mode = x ;
						freq = xfreq ;
					}
				}
			}
			return mode ;
		}
		#endregion
		#region NegBinomDist
		/// <summary>
		/// Calculates the Negative Binomial Probability Density Function.
		/// </summary>
		/// <param name="Failures">Number of failures</param>
		/// <param name="Threshold">Threshold number of successes</param>
		/// <param name="Probability">Probability for success</param>
		public static double NegBinomDist(int Failures, int Threshold, double Probability)
		{
			return Combinations(Failures + Threshold - 1, Threshold - 1) * 
				Mathematics.Pow(Probability, Threshold) * Mathematics.Pow(1 - Probability, Failures) ;
		}
		/// <summary>
		/// Calculates the Negative Binomial Probability Density Function.
		/// </summary>
		/// <param name="Failures">Number of failures</param>
		/// <param name="Threshold">Threshold number of successes</param>
		/// <param name="Probability">Probability for success</param>
		public static float NegBinomDist(int Failures, int Threshold, float Probability)
		{
			return (float)(Combinations(Failures + Threshold - 1, Threshold - 1) * 
				Mathematics.Pow(Probability, Threshold) * Mathematics.Pow(1 - Probability, Failures)) ;
		}
		/// <summary>
		/// Calculates the Negative Binomial Probability Density Function.
		/// </summary>
		/// <param name="Failures">Number of failures</param>
		/// <param name="Threshold">Threshold number of successes</param>
		/// <param name="Probability">Probability for success</param>
		public static Decimal NegBinomDist(int Failures, int Threshold, Decimal Probability)
		{
			return CombinationsM(Failures + Threshold - 1, Threshold - 1) * 
				Calculus.Pow(Probability, Threshold) * Calculus.Pow(1M - Probability, Failures) ;
		}
		#endregion
		#region NormDist
		/// <summary>
		/// Calculates the Normal Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter for Normal Distribution (Mean)</param>
		/// <param name="sigma">Sigma parameter for Normal Distribution (Standart Deviation)</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static double NormDist(double X, double mu, double sigma, bool Cumulative)
		{
			return Cumulative? NormCumulative(X, mu, sigma) : NormProbability(X, mu, sigma) ;
		}
		/// <summary>
		/// Calculates the Normal Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter for Normal Distribution (Mean)</param>
		/// <param name="sigma">Sigma parameter for Normal Distribution (Standart Deviation)</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static float NormDist(float X, float mu, float sigma, bool Cumulative)
		{
			return Cumulative? NormCumulative(X, mu, sigma) : NormProbability(X, mu, sigma) ;
		}
		/// <summary>
		/// Calculates the Normal Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter for Normal Distribution (Mean)</param>
		/// <param name="sigma">Sigma parameter for Normal Distribution (Standart Deviation)</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static Decimal NormDist(Decimal X, Decimal mu, Decimal sigma, bool Cumulative)
		{
			return Cumulative? NormCumulative(X, mu, sigma) : NormProbability(X, mu, sigma) ;
		}
		#endregion
		#region NormInv
		/// <summary>
		/// Calculates the Inverse of the Normal Cumulative Distribution Function.
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 7.1.1 and 26.2.2
		/// </summary>
		/// <param name="Probability"></param>
		/// <param name="mu"></param>
		/// <param name="sigma"></param>
		/// <returns></returns>
		public static double NormInv(double Probability, double mu, double sigma)
		{
			//			% Allocate space for x.
			//			x = zeros(size(p));
			//
			//			% Return NaN if the arguments are outside their respective limits.
			//			k = find(sigma <= 0 | p < 0 | p > 1 | isnan(p));
			//			if any(k)
			//					tmp  = NaN;
			//					x(k) = tmp(ones(size(k))); 
			//			end
			if (sigma <= 0 || Probability < 0 || Probability > 1)
				return double.NaN ;
			//
			//			% Put in the correct values when P is either 0 or 1.
			//			k = find(p == 0);
			//			if any(k)
			//					tmp  = Inf;
			//					x(k) = -tmp(ones(size(k)));
			//			end
			if (Probability == 0)
				return double.NegativeInfinity ;
			//
			//			k = find(p == 1);
			//			if any(k)
			//					tmp  = Inf;
			//					x(k) = tmp(ones(size(k))); 
			//			end
			if (Probability == 1)
				return double.PositiveInfinity ;
			//
			//			% Compute the inverse function for the intermediate values.
			//			k = find(p > 0  &  p < 1 & sigma > 0);
			//			if any(k),
			//					x(k) = sqrt(2) * sigma(k) .* erfinv(2 * p(k) - 1) + mu(k);
			//			end
			return Mathematics.Sqrt(2) * sigma * ErfInv(2 * Probability - 1) + mu ;
		}
		/// <summary>
		/// Calculates the Inverse of the Normal Cumulative Distribution Function.
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 7.1.1 and 26.2.2
		/// </summary>
		/// <param name="Probability"></param>
		/// <param name="mu"></param>
		/// <param name="sigma"></param>
		/// <returns></returns>
		public static float NormInv(float Probability, float mu, float sigma)
		{
			if (sigma <= 0 || Probability < 0 || Probability > 1)
				return float.NaN ;
			if (Probability == 0)
				return float.NegativeInfinity ;
			if (Probability == 1)
				return float.PositiveInfinity ;
			return (float)(Mathematics.Sqrt(2) * sigma * ErfInv(2 * Probability - 1)) + mu ;
		}
		/// <summary>
		/// Calculates the Inverse of the Normal Cumulative Distribution Function.
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 7.1.1 and 26.2.2
		/// </summary>
		/// <param name="Probability"></param>
		/// <param name="mu"></param>
		/// <param name="sigma"></param>
		/// <returns></returns>
		public static Decimal NormInv(Decimal Probability, Decimal mu, Decimal sigma)
		{
			if (sigma <= 0 || Probability < 0 || Probability > 1)
				return -1 ;
			if (Probability == 0)
				return Decimal.MinValue ;
			if (Probability == 1)
				return Decimal.MaxValue ;
			return Calculus.Sqrt2 * sigma * ErfInv(2M * Probability - 1M) + mu ;
		}
		#endregion
		#region NormSDist
		/// <summary>
		/// Calculates the Normal Distribution Function with mean 0 and standard deviation 1.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static double NormSDist(double X, bool Cumulative)
		{
			return Cumulative ? NormCumulative(X, 0, 1) : NormProbability(X, 0, 1) ;
		}
		/// <summary>
		/// Calculates the Normal Distribution Function with mean 0 and standard deviation 1.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static float NormSDist(float X, bool Cumulative)
		{
			return Cumulative ? NormCumulative(X, 0, 1) : NormProbability(X, 0, 1) ;
		}
		/// <summary>
		/// Calculates the Normal Distribution Function with mean 0 and standard deviation 1.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static Decimal NormSDist(Decimal X, bool Cumulative)
		{
			return Cumulative ? NormCumulative(X, 0, 1) : NormProbability(X, 0, 1) ;
		}
		#endregion
		#region NormSInv
		/// <summary>
		/// Calculates the Inverse of the Normal Cumulative Distribution Function with mean 0 and standard deviation 1.
		/// </summary>
		/// <param name="Probability">Value to evaluate at</param>
		public static double NormSInv(double Probability)
		{
			return NormInv(Probability, 0, 1) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Normal Cumulative Distribution Function with mean 0 and standard deviation 1.
		/// </summary>
		/// <param name="Probability">Value to evaluate at</param>
		public static float NormSInv(float Probability)
		{
			return NormInv(Probability, 0, 1) ;
		}
		/// <summary>
		/// Calculates the Inverse of the Normal Cumulative Distribution Function with mean 0 and standard deviation 1.
		/// </summary>
		/// <param name="Probability">Value to evaluate at</param>
		public static Decimal NormSInv(Decimal Probability)
		{
			return NormInv(Probability, 0, 1) ;
		}
		#endregion
		#region Norm Tools
		/// <summary>
		/// Calculates the Probability Density Function of the Normal Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter for Normal Distribution (Mean)</param>
		/// <param name="sigma">Sigma parameter for Normal Distribution (Standart Deviation)</param>
		public static double NormProbability(double X, double mu, double sigma)
		{
			//			k = find(sigma > 0);
			//			if any(k)
			//					xn = (x(k) - mu(k)) ./ sigma(k);
			//					y(k) = exp(-0.5 * xn .^2) ./ (sqrt(2*pi) .* sigma(k));
			//			end
			if (sigma > 0)
			{
				double xn = (X - mu) / sigma ;
				return Mathematics.Exp(- 0.5 * (xn * xn)) / (Mathematics.Sqrt(2 * Mathematics.PI) * sigma) ;
			}
			else
				return double.NaN ;
		}
		/// <summary>
		/// Calculates the Probability Density Function of the Normal Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter for Normal Distribution (Mean)</param>
		/// <param name="sigma">Sigma parameter for Normal Distribution (Standart Deviation)</param>
		public static float NormProbability(float X, float mu, float sigma)
		{
			//			k = find(sigma > 0);
			//			if any(k)
			//					xn = (x(k) - mu(k)) ./ sigma(k);
			//					y(k) = exp(-0.5 * xn .^2) ./ (sqrt(2*pi) .* sigma(k));
			//			end
			if (sigma > 0)
			{
				float xn = (X - mu) / sigma ;
				return (float)(Mathematics.Exp(- 0.5 * (xn * xn)) / (Mathematics.Sqrt(2 * Mathematics.PI) * sigma)) ;
			}
			else
				return float.NaN ;
		}
		/// <summary>
		/// Calculates the Probability Density Function of the Normal Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter for Normal Distribution (Mean)</param>
		/// <param name="sigma">Sigma parameter for Normal Distribution (Standart Deviation)</param>
		public static Decimal NormProbability(Decimal X, Decimal mu, Decimal sigma)
		{
			if (sigma > 0)
			{
				Decimal xn = (X - mu) / sigma ;
				return Calculus.Exp(- 0.5M * (xn * xn)) / (Calculus.Sqrt2 * Calculus.SqrtPi * sigma) ;
			}
			else
				return -1 ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution Function of the Normal Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter for Normal Distribution (Mean)</param>
		/// <param name="sigma">Sigma parameter for Normal Distribution (Standart Deviation)</param>
		public static double NormCumulative(double X, double mu, double sigma)
		{
			return 0.5 * ErfCore( - (X - mu) / (sigma * Mathematics.Sqrt(2)), 1) ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution Function of the Normal Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter for Normal Distribution (Mean)</param>
		/// <param name="sigma">Sigma parameter for Normal Distribution (Standart Deviation)</param>
		public static float NormCumulative(float X, float mu, float sigma)
		{
			return (float)(0.5 * ErfCore( - (X - mu) / (sigma * Mathematics.Sqrt(2)), 1)) ;
		}
		/// <summary>
		/// Calculates the Cumulative Distribution Function of the Normal Distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="mu">Mu parameter for Normal Distribution (Mean)</param>
		/// <param name="sigma">Sigma parameter for Normal Distribution (Standart Deviation)</param>
		public static Decimal NormCumulative(Decimal X, Decimal mu, Decimal sigma)
		{
			return 0.5M * ErfCore( - (X - mu) / (sigma * Calculus.Sqrt2), 1) ;
		}
		#endregion
		#region Pearson
		/// <summary>
		/// Calculates the Pearson Product of parameter.
		/// </summary>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static double Pearson(double[] array1, double[] array2)
		{
			if (array1.Length != array2.Length)
				throw new ArrayDimensionsException() ;
			double sumXY = .0 ;
			double sumX = .0 ;
			double sumY = .0 ;
			double sumX2 = .0 ;
			double sumY2 = .0 ;
			int n = array1.Length ;
			for(int i = 0; i < n; i++)
			{
				sumXY += array2[i] * array1[i] ;
				sumX += array2[i] ;
				sumY += array1[i] ;
				sumX2 += array2[i] * array2[i] ;
				sumY2 += array1[i] * array1[i] ;
			}
			return (n * sumXY - sumX * sumY) / 
				(Mathematics.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY))) ;
		}
		/// <summary>
		/// Calculates the Pearson Product of parameter.
		/// </summary>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static float Pearson(float[] array1, float[] array2)
		{
			if (array1.Length != array2.Length)
				throw new ArrayDimensionsException() ;
			float sumXY = .0F ;
			float sumX = .0F ;
			float sumY = .0F ;
			float sumX2 = .0F ;
			float sumY2 = .0F ;
			int n = array1.Length ;
			for(int i = 0; i < n; i++)
			{
				sumXY += array2[i] * array1[i] ;
				sumX += array2[i] ;
				sumY += array1[i] ;
				sumX2 += array2[i] * array2[i] ;
				sumY2 += array1[i] * array1[i] ;
			}
			return (float)((n * sumXY - sumX * sumY) / 
				(Mathematics.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY)))) ;
		}
		/// <summary>
		/// Calculates the Pearson Product of parameter.
		/// </summary>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static Decimal Pearson(Decimal[] array1, Decimal[] array2)
		{
			if (array1.Length != array2.Length)
				throw new ArrayDimensionsException() ;
			Decimal sumXY = .0M ;
			Decimal sumX = .0M ;
			Decimal sumY = .0M ;
			Decimal sumX2 = .0M ;
			Decimal sumY2 = .0M ;
			int n = array1.Length ;
			for(int i = 0; i < n; i++)
			{
				sumXY += array2[i] * array1[i] ;
				sumX += array2[i] ;
				sumY += array1[i] ;
				sumX2 += array2[i] * array2[i] ;
				sumY2 += array1[i] * array1[i] ;
			}
			return (n * sumXY - sumX * sumY) / 
				(Calculus.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY))) ;
		}
		#endregion
		#region Percentile
		/// <summary>
		/// Calculates the k-th percentile in data.
		/// </summary>
		/// <remarks>This functions uses an array sorting algorithm, so it's O(n log(n)). O(n) search will be implemented
		/// in future releases.</remarks>
		public static double Percentile(double[] data, double k)
		{
			int n = data.Length ;
			if (k == 1)
				return Pick(data, n - 1) ;
			if (k == 0)
				return Pick(data, 0) ;
			double index = k * (n - 1) ;
			double lb = Pick(data, (int)(index)) ;
			double ub = Pick(data, (int)(index) + 1) ;
			return lb + (ub - lb) * (index - Mathematics.Floor(index)) ;
		}
		/// <summary>
		/// Calculates the k-th percentile in data.
		/// </summary>
		/// <remarks>This functions uses an array sorting algorithm, so it's O(n log(n)). O(n) search will be implemented
		/// in future releases.</remarks>
		public static float Percentile(float[] data, float k)
		{
			int n = data.Length ;
			if (k == 1)
				return Pick(data, n - 1) ;
			if (k == 0)
				return Pick(data, 0) ;
			float index = k * (n - 1) ;
			float lb = Pick(data, (int)(index)) ;
			float ub = Pick(data, (int)(index) + 1) ;
			return lb + (float)((ub - lb) * (index - Mathematics.Floor(index))) ;
		}
		/// <summary>
		/// Calculates the k-th percentile in data.
		/// </summary>
		/// <remarks>This functions uses an array sorting algorithm, so it's O(n log(n)). O(n) search will be implemented
		/// in future releases.</remarks>
		public static Decimal Percentile(Decimal[] data, Decimal k)
		{
			int n = data.Length ;
			if (k == 1)
				return Pick(data, n - 1) ;
			if (k == 0)
				return Pick(data, 0) ;
			Decimal index = k * (n - 1) ;
			Decimal lb = Pick(data, (int)(index)) ;
			Decimal ub = Pick(data, (int)(index) + 1) ;
			return lb + (ub - lb) * (index - Calculus.Floor(index)) ;
		}
		#endregion
		#region PercentRank
		/// <summary>
		/// Calculates the rank of a value in a data set as a percentage of the data set. This function can be 
		/// used to evaluate the relative standing of a value within a data set.
		/// </summary>
		/// <param name="data">Data array</param>
		/// <param name="value">Element to find standing for</param>
		/// <remarks>If the element is contained in array function returns <code>Smaller Values/(Smaller Values + Larger Values)</code>, 
		/// if not, functions returns values proportional distance between its greater smaller value contained in array and 
		/// its smaller greater value.</remarks>
		public static double PercentRank(double[] data, double value)
		{
			int n = data.Length ;
			int smallers = 0 ;
			int largers = 0 ;
			int equals = 0 ;
			double greaterSmallerValue = double.MinValue ;
			double smallerGreaterValue = double.MaxValue ;
			for(int index = 0; index < n;)
			{
				while(index < n && data[index] < value)
				{
					if (greaterSmallerValue < data[index])
						greaterSmallerValue = data[index] ;
					index ++ ;
					smallers ++ ;
				}
				while(index < n && data[index] > value)
				{
					if (smallerGreaterValue > data[index])
						smallerGreaterValue = data[index] ;
					index ++ ;
					largers ++ ;
				}
				while(index < n && data[index] == value)
				{
					index ++ ;
					equals ++ ;
				}
			}
			if (equals > 0)
				return (double)smallers / (smallers + largers) ;
			double pSmallerGreaterValue = PercentRank(data, smallerGreaterValue) ;
			double pGreaterSmallerValue = PercentRank(data, greaterSmallerValue) ;
			return pSmallerGreaterValue + (value - smallerGreaterValue) * 
				(pGreaterSmallerValue - pSmallerGreaterValue) / (greaterSmallerValue - smallerGreaterValue) ;
		}
		/// <summary>
		/// Calculates the rank of a value in a data set as a percentage of the data set. This function can be 
		/// used to evaluate the relative standing of a value within a data set.
		/// </summary>
		/// <param name="data">Data array</param>
		/// <param name="value">Element to find standing for</param>
		/// <remarks>If the element is contained in array function returns <code>Smaller Values/(Smaller Values + Larger Values)</code>, 
		/// if not, functions returns values proportional distance between its greater smaller value contained in array and 
		/// its smaller greater value.</remarks>
		public static float PercentRank(float[] data, float value)
		{
			int n = data.Length ;
			int smallers = 0 ;
			int largers = 0 ;
			int equals = 0 ;
			float greaterSmallerValue = float.MinValue ;
			float smallerGreaterValue = float.MaxValue ;
			for(int index = 0; index < n;)
			{
				while(index < n && data[index] < value)
				{
					if (greaterSmallerValue < data[index])
						greaterSmallerValue = data[index] ;
					index ++ ;
					smallers ++ ;
				}
				while(index < n && data[index] > value)
				{
					if (smallerGreaterValue > data[index])
						smallerGreaterValue = data[index] ;
					index ++ ;
					largers ++ ;
				}
				while(index < n && data[index] == value)
				{
					index ++ ;
					equals ++ ;
				}
			}
			if (equals > 0)
				return (float)smallers / (smallers + largers) ;
			float pSmallerGreaterValue = PercentRank(data, smallerGreaterValue) ;
			float pGreaterSmallerValue = PercentRank(data, greaterSmallerValue) ;
			return pSmallerGreaterValue + (value - smallerGreaterValue) * 
				(pGreaterSmallerValue - pSmallerGreaterValue) / (greaterSmallerValue - smallerGreaterValue) ;
		}
		/// <summary>
		/// Calculates the rank of a value in a data set as a percentage of the data set. This function can be 
		/// used to evaluate the relative standing of a value within a data set.
		/// </summary>
		/// <param name="data">Data array</param>
		/// <param name="value">Element to find standing for</param>
		/// <remarks>If the element is contained in array function returns <code>Smaller Values/(Smaller Values + Larger Values)</code>, 
		/// if not, functions returns values proportional distance between its greater smaller value contained in array and 
		/// its smaller greater value.</remarks>
		public static Decimal PercentRank(Decimal[] data, Decimal value)
		{
			int n = data.Length ;
			int smallers = 0 ;
			int largers = 0 ;
			int equals = 0 ;
			Decimal greaterSmallerValue = Decimal.MinValue ;
			Decimal smallerGreaterValue = Decimal.MaxValue ;
			for(int index = 0; index < n;)
			{
				while(index < n && data[index] < value)
				{
					if (greaterSmallerValue < data[index])
						greaterSmallerValue = data[index] ;
					index ++ ;
					smallers ++ ;
				}
				while(index < n && data[index] > value)
				{
					if (smallerGreaterValue > data[index])
						smallerGreaterValue = data[index] ;
					index ++ ;
					largers ++ ;
				}
				while(index < n && data[index] == value)
				{
					index ++ ;
					equals ++ ;
				}
			}
			if (equals > 0)
				return (Decimal)smallers / (smallers + largers) ;
			Decimal pSmallerGreaterValue = PercentRank(data, smallerGreaterValue) ;
			Decimal pGreaterSmallerValue = PercentRank(data, greaterSmallerValue) ;
			return pSmallerGreaterValue + (value - smallerGreaterValue) * 
				(pGreaterSmallerValue - pSmallerGreaterValue) / (greaterSmallerValue - smallerGreaterValue) ;
		}
		#endregion
		#region Permut
		/// <summary>
		/// Calculates the numbers of permutations.
		/// </summary>
		/// <param name="size">Number of objects</param>
		/// <param name="sample">Sample size</param>
		public static double Permut(double objects, double sample)
		{
			long result = 1L ;
			int current = (int)objects ;
			while(current > (objects - sample))
				result *= current-- ;
			return result ;
		}
		/// <summary>
		/// Calculates the numbers of permutations.
		/// </summary>
		/// <param name="size">Number of objects</param>
		/// <param name="sample">Sample size</param>
		public static float Permut(float objects, float sample)
		{
			long result = 1L ;
			int current = (int)objects ;
			while(current > (objects - sample))
				result *= current-- ;
			return result ;
		}
		/// <summary>
		/// Calculates the numbers of permutations.
		/// </summary>
		/// <param name="size">Number of objects</param>
		/// <param name="sample">Sample size</param>
		public static Decimal Permut(Decimal objects, Decimal sample)
		{
			long result = 1L ;
			int current = (int)objects ;
			while(current > (objects - sample))
				result *= current-- ;
			return result ;
		}
		#endregion
		#region Poisson
		/// <summary>
		/// Calculates the Poisson Distribution.
		/// </summary>
		/// <param name="x">Number of events</param>
		/// <param name="mean">Expected numeric value</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static double Poisson(int x, double mean, bool Cumulative)
		{
			return Cumulative? PoissonCumulative(x, mean) : PoissonProbability(x, mean) ;
		}
		/// <summary>
		/// Calculates the Poisson Distribution.
		/// </summary>
		/// <param name="x">Number of events</param>
		/// <param name="mean">Expected numeric value</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static float Poisson(int x, float mean, bool Cumulative)
		{
			return Cumulative? PoissonCumulative(x, mean) : PoissonProbability(x, mean) ;
		}
		/// <summary>
		/// Calculates the Poisson Distribution.
		/// </summary>
		/// <param name="x">Number of events</param>
		/// <param name="mean">Expected numeric value</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static Decimal Poisson(int x, Decimal mean, bool Cumulative)
		{
			return Cumulative? PoissonCumulative(x, mean) : PoissonProbability(x, mean) ;
		}
		#endregion
		#region Poisson Tools
		/// <summary>
		/// Calculates the Poisson Probability Density Function.
		/// </summary>
		/// <param name="x">Number of events</param>
		/// <param name="mean">Expected numeric value</param>
		/// <returns></returns>
		public static double PoissonProbability(int x, double mean)
		{
			return Mathematics.Exp(- mean) * Mathematics.Pow(mean, x) / Gamma(x + 1.0) ;
		}
		/// <summary>
		/// Calculates the Poisson Probability Density Function.
		/// </summary>
		/// <param name="x">Number of events</param>
		/// <param name="mean">Expected numeric value</param>
		/// <returns></returns>
		public static float PoissonProbability(int x, float mean)
		{
			return (float)(Mathematics.Exp(- mean) * Mathematics.Pow(mean, x) / Gamma(x + 1.0)) ;
		}
		/// <summary>
		/// Calculates the Poisson Probability Density Function.
		/// </summary>
		/// <param name="x">Number of events</param>
		/// <param name="mean">Expected numeric value</param>
		/// <returns></returns>
		public static Decimal PoissonProbability(int x, Decimal mean)
		{
			return Calculus.Exp(- mean) * Calculus.Pow(mean, x) / Gamma(x + 1.0M) ;
		}
		/// <summary>
		/// Calculates the Poisson Cumulative Distribution Function.
		/// </summary>
		/// <param name="x">Number of events</param>
		/// <param name="mean">Expected numeric value</param>
		/// <returns></returns>
		public static double PoissonCumulative(int x, double mean)
		{
			double result = 0 ;
			for(int i = 0; i <= x; i ++)
				result += PoissonProbability(i, mean) ;
			return result ;
		}
		/// <summary>
		/// Calculates the Poisson Cumulative Distribution Function.
		/// </summary>
		/// <param name="x">Number of events</param>
		/// <param name="mean">Expected numeric value</param>
		/// <returns></returns>
		public static float PoissonCumulative(int x, float mean)
		{
			float result = 0 ;
			for(int i = 0; i <= x; i ++)
				result += PoissonProbability(i, mean) ;
			return result ;
		}
		/// <summary>
		/// Calculates the Poisson Cumulative Distribution Function.
		/// </summary>
		/// <param name="x">Number of events</param>
		/// <param name="mean">Expected numeric value</param>
		/// <returns></returns>
		public static Decimal PoissonCumulative(int x, Decimal mean)
		{
			Decimal result = 0 ;
			for(int i = 0; i <= x; i ++)
				result += PoissonProbability(i, mean) ;
			return result ;
		}
		#endregion
		#region Prob
		/// <summary>
		/// Returns the probability that values in a range are between two limits. 
		/// </summary>
		/// <param name="xRanges">Range values</param>
		/// <param name="RangeProb">Range probability</param>
		/// <param name="lower">Lower limit</param>
		/// <param name="upper">Upper limit</param>
		public static double Prob(double[] xRanges, double[] RangeProb, double lower, double upper)
		{
			double result = 0 ;
			for(int i = 0; i < xRanges.Length; i ++)
				if (lower <= xRanges[i] && xRanges[i] <= upper)
					result += RangeProb[i] ;
			return result ;
		}
		/// <summary>
		/// Returns the probability that values in a range are between two limits. 
		/// </summary>
		/// <param name="xRanges">Range values</param>
		/// <param name="RangeProb">Range probability</param>
		/// <param name="lower">Lower limit</param>
		/// <param name="upper">Upper limit</param>
		public static float Prob(float[] xRanges, float[] RangeProb, float lower, float upper)
		{
			float result = 0 ;
			for(int i = 0; i < xRanges.Length; i ++)
				if (lower <= xRanges[i] && xRanges[i] <= upper)
					result += RangeProb[i] ;
			return result ;
		}
		/// <summary>
		/// Returns the probability that values in a range are between two limits. 
		/// </summary>
		/// <param name="xRanges">Range values</param>
		/// <param name="RangeProb">Range probability</param>
		/// <param name="lower">Lower limit</param>
		/// <param name="upper">Upper limit</param>
		public static Decimal Prob(Decimal[] xRanges, Decimal[] RangeProb, Decimal lower, Decimal upper)
		{
			Decimal result = 0 ;
			for(int i = 0; i < xRanges.Length; i ++)
				if (lower <= xRanges[i] && xRanges[i] <= upper)
					result += RangeProb[i] ;
			return result ;
		}
		#endregion
		#region Quartile
		/// <summary>
		/// Calculates the k-th Quartile of data.
		/// </summary>
		public static double Quartile(double[] data, int k)
		{
			return Percentile(data, k / 4) ;
		}
		/// <summary>
		/// Calculates the k-th Quartile of data.
		/// </summary>
		public static float Quartile(float[] data, int k)
		{
			return Percentile(data, k / 4) ;
		}
		/// <summary>
		/// Calculates the k-th Quartile of data.
		/// </summary>
		public static Decimal Quartile(Decimal[] data, int k)
		{
			return Percentile(data, k / 4) ;
		}
		#endregion
		#region Rank
		/// <summary>
		/// Calculates the rank of a number in a list of numbers. The rank of a number is its size relative 
		/// to other values in a list. (If you were to sort the list, the rank of the number would be its position.)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="data"></param>
		/// <param name="descending"></param>
		/// <returns></returns>
		public static double Rank(double value, double[] data, bool descending)
		{
			int smallers = 1 ;
			int greaters = 1 ;
			int n = data.Length ;
			for (int i = 0; i < n; i++)
				if (data[i] < value)
					smallers ++ ;
				else if (data[i] > value)
					greaters ++ ;
			return descending? greaters : smallers ;
		}
		/// <summary>
		/// Calculates the rank of a number in a list of numbers. The rank of a number is its size relative 
		/// to other values in a list. (If you were to sort the list, the rank of the number would be its position.)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="data"></param>
		/// <param name="descending"></param>
		/// <returns></returns>
		public static float Rank(float value, float[] data, bool descending)
		{
			int smallers = 1 ;
			int greaters = 1 ;
			int n = data.Length ;
			for (int i = 0; i < n; i++)
				if (data[i] < value)
					smallers ++ ;
				else if (data[i] > value)
					greaters ++ ;
			return descending? greaters : smallers ;
		}
		/// <summary>
		/// Calculates the rank of a number in a list of numbers. The rank of a number is its size relative 
		/// to other values in a list. (If you were to sort the list, the rank of the number would be its position.)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="data"></param>
		/// <param name="descending"></param>
		/// <returns></returns>
		public static Decimal Rank(Decimal value, Decimal[] data, bool descending)
		{
			int smallers = 1 ;
			int greaters = 1 ;
			int n = data.Length ;
			for (int i = 0; i < n; i++)
				if (data[i] < value)
					smallers ++ ;
				else if (data[i] > value)
					greaters ++ ;
			return descending? greaters : smallers ;
		}
		#endregion
		#region Rsq
		/// <summary>
		/// Calculates the Square of the Pearson Product.
		/// </summary>
		public static double Rsq(double[] knownYs, double[] knownXs)
		{
			double pearson = Pearson(knownYs, knownXs) ;
			return  pearson * pearson ;
		}
		/// <summary>
		/// Calculates the Square of the Pearson Product.
		/// </summary>
		public static float Rsq(float[] knownYs, float[] knownXs)
		{
			float pearson = Pearson(knownYs, knownXs) ;
			return  pearson * pearson ;
		}
		/// <summary>
		/// Calculates the Square of the Pearson Product.
		/// </summary>
		public static Decimal Rsq(Decimal[] knownYs, Decimal[] knownXs)
		{
			Decimal pearson = Pearson(knownYs, knownXs) ;
			return  pearson * pearson ;
		}
		#endregion
		#region Skew
		/// <summary>
		/// Calculates the skewness of values in X.
		/// </summary>
		public static double Skew(double[] X)
		{
			double sumX3M0 = 0 ;
			double sumX2M1 = 0 ;
			double sumX1M2 = 0 ;
			double sumX0M3 = 0 ;
			double mean = Average(X) ;
			double s = StDev(X) ;
			int n = X.Length ;
			for (int i = 0; i < n; i++)
			{
				double x = X[i] ;
				sumX3M0 += x * x * x ;
				sumX2M1 += x * x ;
				sumX1M2 += x ;
			}
			sumX2M1 *= mean ;
			sumX1M2 *= mean * mean ;
			sumX0M3 = mean * mean * mean * n ;
			return (n / (double)((n - 1) * (n - 2))) * (sumX3M0 - 3 * sumX2M1 + 3 * sumX1M2 - sumX0M3) / (s * s * s) ;
		}
		/// <summary>
		/// Calculates the skewness of values in X.
		/// </summary>
		public static float Skew(float[] X)
		{
			float sumX3M0 = 0 ;
			float sumX2M1 = 0 ;
			float sumX1M2 = 0 ;
			float sumX0M3 = 0 ;
			float mean = Average(X) ;
			float s = StDev(X) ;
			int n = X.Length ;
			for (int i = 0; i < n; i++)
			{
				float x = X[i] ;
				sumX3M0 += x * x * x ;
				sumX2M1 += x * x ;
				sumX1M2 += x ;
			}
			sumX2M1 *= mean ;
			sumX1M2 *= mean * mean ;
			sumX0M3 = mean * mean * mean * n ;
			return (n / (float)((n - 1) * (n - 2))) * (sumX3M0 - 3 * sumX2M1 + 3 * sumX1M2 - sumX0M3) / (s * s * s) ;
		}
		/// <summary>
		/// Calculates the skewness of values in X.
		/// </summary>
		public static Decimal Skew(Decimal[] X)
		{
			Decimal sumX3M0 = 0 ;
			Decimal sumX2M1 = 0 ;
			Decimal sumX1M2 = 0 ;
			Decimal sumX0M3 = 0 ;
			Decimal mean = Average(X) ;
			Decimal s = StDev(X) ;
			int n = X.Length ;
			for (int i = 0; i < n; i++)
			{
				Decimal x = X[i] ;
				sumX3M0 += x * x * x ;
				sumX2M1 += x * x ;
				sumX1M2 += x ;
			}
			sumX2M1 *= mean ;
			sumX1M2 *= mean * mean ;
			sumX0M3 = mean * mean * mean * n ;
			return (n / (Decimal)((n - 1) * (n - 2))) * (sumX3M0 - 3 * sumX2M1 + 3 * sumX1M2 - sumX0M3) / (s * s * s) ;
		}
		#endregion
		#region Slope
		/// <summary>
		/// Calculates the Slope of the linear regression line through data points in 
		/// knownYs and knownXs. The slope is the vertical distance divided by the horizontal 
		/// distance between any two points on the line, which is the rate of change along the regression line.
		/// </summary>
		public static double Slope(double[] knownYs, double[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			double sumXY = .0 ;
			double sumX = .0 ;
			double sumY = .0 ;
			double sumX2 = .0 ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i ++)
			{
				double x = knownXs[i] ;
				double y = knownYs[i] ;
				sumXY += x * y ;
				sumX += x ;
				sumX2 += x * x ;
				sumY += y ;
			}
			return (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX) ;
		}
		/// <summary>
		/// Calculates the Slope of the linear regression line through data points in 
		/// knownYs and knownXs. The slope is the vertical distance divided by the horizontal 
		/// distance between any two points on the line, which is the rate of change along the regression line.
		/// </summary>
		public static float Slope(float[] knownYs, float[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			float sumXY = .0F ;
			float sumX = .0F ;
			float sumY = .0F ;
			float sumX2 = .0F ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i ++)
			{
				float x = knownXs[i] ;
				float y = knownYs[i] ;
				sumXY += x * y ;
				sumX += x ;
				sumX2 += x * x ;
				sumY += y ;
			}
			return (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX) ;
		}
		/// <summary>
		/// Calculates the Slope of the linear regression line through data points in 
		/// knownYs and knownXs. The slope is the vertical distance divided by the horizontal 
		/// distance between any two points on the line, which is the rate of change along the regression line.
		/// </summary>
		public static Decimal Slope(Decimal[] knownYs, Decimal[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			Decimal sumXY = .0M ;
			Decimal sumX = .0M ;
			Decimal sumY = .0M ;
			Decimal sumX2 = .0M ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i ++)
			{
				Decimal x = knownXs[i] ;
				Decimal y = knownYs[i] ;
				sumXY += x * y ;
				sumX += x ;
				sumX2 += x * x ;
				sumY += y ;
			}
			return (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX) ;
		}
		#endregion
		#region Small
		/// <summary>
		/// Locates the k-th smallest number in data.
		/// </summary>
		public static double Small(double[] data, int k)
		{
			return Pick(data, k - 1) ;
		}
		/// <summary>
		/// Locates the k-th smallest number in data.
		/// </summary>
		public static float Small(float[] data, int k)
		{
			return Pick(data, k - 1) ;
		}
		/// <summary>
		/// Locates the k-th smallest number in data.
		/// </summary>
		public static Decimal Small(Decimal[] data, int k)
		{
			return Pick(data, k - 1) ;
		}
		#endregion
		#region Standarize
		/// <summary>
		/// Calculates a normalized value from a distribution characterized by mu and sigma.
		/// </summary>
		/// <param name="X">Value to normalize</param>
		/// <param name="mu">Distribution's mean</param>
		/// <param name="sigma">Distribution's standard deviation</param>
		public static double Standarize(double X, double mu, double sigma)
		{
			return (X - mu) / sigma ;
		}
		/// <summary>
		/// Calculates a normalized value from a distribution characterized by mu and sigma.
		/// </summary>
		/// <param name="X">Value to normalize</param>
		/// <param name="mu">Distribution's mean</param>
		/// <param name="sigma">Distribution's standard deviation</param>
		public static float Standarize(float X, float mu, float sigma)
		{
			return (X - mu) / sigma ;
		}
		/// <summary>
		/// Calculates a normalized value from a distribution characterized by mu and sigma.
		/// </summary>
		/// <param name="X">Value to normalize</param>
		/// <param name="mu">Distribution's mean</param>
		/// <param name="sigma">Distribution's standard deviation</param>
		public static Decimal Standarize(Decimal X, Decimal mu, Decimal sigma)
		{
			return (X - mu) / sigma ;
		}
		#endregion
		#region StDev
		/// <summary>
		/// Calculates the Standard Deviation of a sample of a population.
		/// </summary>
		/// <remarks>If your data represents the whole population you should use StDevP</remarks>
		/// <param name="X">Set of numbers(representing a sample of a population) to calculate 
		/// the Standard Deviation</param>
		public static Decimal StDev(Decimal[] X)
		{
			Decimal x = 0, x2 = 0 ;
			for(int i = 0; i < X.Length; i++)
			{
				x += X[i] ;
				x2 += X[i] * X[i] ;
			}
			int n = X.Length ;
			return (Decimal)Mathematics.Sqrt((double)( n * x2 - x * x) / (n * (n - 1))) ;
		}
		/// <summary>
		/// Calculates the Standard Deviation of a sample of a population.
		/// </summary>
		/// <remarks>If your data represents the whole population you should use StDevP</remarks>
		/// <param name="X">Set of numbers(representing a sample of a population) to calculate 
		/// the Standard Deviation</param>
		public static double StDev(double[] X)
		{
			double x = 0, x2 = 0 ;
			for(int i = 0; i < X.Length; i++)
			{
				x += X[i] ;
				x2 += X[i] * X[i] ;
			}
			int n = X.Length ;
			return Mathematics.Sqrt((n * x2 - x * x) / (n * (n - 1))) ;
		}
		/// <summary>
		/// Calculates the Standard Deviation of a sample of a population.
		/// </summary>
		/// <remarks>If your data represents the whole population you should use StDevP</remarks>
		/// <param name="X">Set of numbers(representing a sample of a population) to calculate 
		/// the Standard Deviation</param>
		public static float StDev(float[] X)
		{
			return (float)StDev(ToDoubleArray(X)) ;
		}
		#endregion
		#region StDevP
		/// <summary>
		/// Calculates the Standard Deviation of a population
		/// </summary>
		/// <remarks>If your data represents a sample of a population you should use StDev</remarks>
		/// <param name="X">Set of numbers(representing a population) to calculate the Standard Deviation</param>
		public static Decimal StDevP(Decimal[] X)
		{
			Decimal x = 0, x2 = 0 ;
			for(int i = 0; i < X.Length; i++)
			{
				x += X[i] ;
				x2 += X[i] * X[i] ;
			}
			int n = X.Length ;
			return (Decimal)Mathematics.Sqrt((double)( n * x2 - x * x) / (n * n)) ;			
		}
		/// <summary>
		/// Calculates the Standard Deviation of a population
		/// </summary>
		/// <remarks>If your data represents a sample of a population you should use StDev</remarks>
		/// <param name="X">Set of numbers(representing a population) to calculate the Standard Deviation</param>
		public static double StDevP(double[] X)
		{
			double x = 0, x2 = 0 ;
			for(int i = 0; i < X.Length; i++)
			{
				x += X[i] ;
				x2 += X[i] * X[i] ;
			}
			int n = X.Length ;
			return Mathematics.Sqrt(( n * x2 - x * x) / (n * n)) ;			
		}
		/// <summary>
		/// Calculates the Standard Deviation of a population
		/// </summary>
		/// <remarks>If your data represents a sample of a population you should use StDev</remarks>
		/// <param name="X">Set of numbers(representing a population) to calculate the Standard Deviation</param>
		public static float StDevP(float[] X)
		{
			return (float)StDevP(ToDoubleArray(X)) ;
		}
		#endregion
		#region SteXY
		/// <summary>
		/// Returns the standard error of the predicted y-value for each x in the regression. The standard error 
		/// is a measure of the amount of error in the prediction of y for an individual x.
		/// </summary>
		/// <param name="knownYs">Dependent data values</param>
		/// <param name="knownXs">Independent data values</param>
		/// <returns></returns>
		public static double SteYX(double[] knownYs, double[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			double sumXY = .0 ;
			double sumX = .0 ;
			double sumY = .0 ;
			double sumX2 = .0 ;
			double sumY2 = .0 ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i ++)
			{
				double x = knownXs[i] ;
				double y = knownYs[i] ;
				sumXY += x * y ;
				sumX += x ;
				sumY += y ;
				sumX2 += x * x ;
				sumY2 += y * y ;
			}
			double num = n * sumXY - sumX * sumY ;
			num *= num ;
			double fraction = num / (n * sumX2 - sumX * sumX) ;
			return Mathematics.Sqrt(1.0 / (n * (n - 2.0)) * (n * sumY2 - sumY * sumY - (fraction))) ;
		}
		/// <summary>
		/// Returns the standard error of the predicted y-value for each x in the regression. The standard error 
		/// is a measure of the amount of error in the prediction of y for an individual x.
		/// </summary>
		/// <param name="knownYs">Dependent data values</param>
		/// <param name="knownXs">Independent data values</param>
		/// <returns></returns>
		public static float SteYX(float[] knownYs, float[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			double sumXY = .0 ;
			double sumX = .0 ;
			double sumY = .0 ;
			double sumX2 = .0 ;
			double sumY2 = .0 ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i ++)
			{
				float x = knownXs[i] ;
				float y = knownYs[i] ;
				sumXY += x * y ;
				sumX += x ;
				sumY += y ;
				sumX2 += x * x ;
				sumY2 += y * y ;
			}
			double num = n * sumXY - sumX * sumY ;
			num *= num ;
			double fraction = num / (n * sumX2 - sumX * sumX) ;
			return (float)Mathematics.Sqrt(1.0 / (n * (n - 2.0)) * (n * sumY2 - sumY * sumY - (fraction))) ;
		}
		/// <summary>
		/// Returns the standard error of the predicted y-value for each x in the regression. The standard error 
		/// is a measure of the amount of error in the prediction of y for an individual x.
		/// </summary>
		/// <param name="knownYs">Dependent data values</param>
		/// <param name="knownXs">Independent data values</param>
		/// <returns></returns>
		public static Decimal SteYX(Decimal[] knownYs, Decimal[] knownXs)
		{
			if (knownXs.Length != knownYs.Length)
				throw new ArrayDimensionsException() ;
			Decimal sumXY = .0M ;
			Decimal sumX = .0M ;
			Decimal sumY = .0M ;
			Decimal sumX2 = .0M ;
			Decimal sumY2 = .0M ;
			int n = knownXs.Length ;
			for(int i = 0; i < n; i ++)
			{
				Decimal x = knownXs[i] ;
				Decimal y = knownYs[i] ;
				sumXY += x * y ;
				sumX += x ;
				sumY += y ;
				sumX2 += x * x ;
				sumY2 += y * y ;
			}
			Decimal num = n * sumXY - sumX * sumY ;
			num *= num ;
			Decimal fraction = num / (n * sumX2 - sumX * sumX) ;
			return Calculus.Sqrt(1.0M / (n * (n - 2.0M)) * (n * sumY2 - sumY * sumY - (fraction))) ;
		}
		#endregion
		#region TDist
		/// <summary>
		/// Calculates the Percentage Points (probability) for the Student t-distribution where a numeric value (x) is a 
		/// calculated value of t for which the Percentage Points are to be computed. The t-distribution is used in 
		/// the hypothesis testing of small sample data sets. Use this function in place of a table of critical values 
		/// for the t-distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress of freedom distribution parameter</param>
		/// <param name="tails">Specifies the distribution number of tails. This value must be either 1 or 2</param>
		/// <returns></returns>
		public static double TDist(double X, int DegressOfFreedom, int tails)
		{
			if (tails != 2 && tails != 1)
				throw new ArgumentException("Tails must be either 1 or 2") ;
			return (1 - TDistCumulative(X, DegressOfFreedom)) / (3 - tails) ;
		}
		/// <summary>
		/// Calculates the Percentage Points (probability) for the Student t-distribution where a numeric value (x) is a 
		/// calculated value of t for which the Percentage Points are to be computed. The t-distribution is used in 
		/// the hypothesis testing of small sample data sets. Use this function in place of a table of critical values 
		/// for the t-distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress of freedom distribution parameter</param>
		/// <param name="tails">Specifies the distribution number of tails. This value must be either 1 or 2</param>
		/// <returns></returns>
		public static float TDist(float X, int DegressOfFreedom, int tails)
		{
			if (tails != 2 && tails != 1)
				throw new ArgumentException("Tails must be either 1 or 2") ;
			return (1 - TDistCumulative(X, DegressOfFreedom)) / (3 - tails) ;
		}
		/// <summary>
		/// Calculates the Percentage Points (probability) for the Student t-distribution where a numeric value (x) is a 
		/// calculated value of t for which the Percentage Points are to be computed. The t-distribution is used in 
		/// the hypothesis testing of small sample data sets. Use this function in place of a table of critical values 
		/// for the t-distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress of freedom distribution parameter</param>
		/// <param name="tails">Specifies the distribution number of tails. This value must be either 1 or 2</param>
		/// <returns></returns>
		public static Decimal TDist(Decimal X, int DegressOfFreedom, int tails)
		{
			if (tails != 2 && tails != 1)
				throw new ArgumentException("Tails must be either 1 or 2") ;
			return (1 - TDistCumulative(X, DegressOfFreedom)) / (3 - tails) ;
		}
		#endregion
		#region TTools
		/// <summary>
		/// Calculates the T-Student Probability Density Function.
		/// References:
		///  References:
		///  1) E. Kreyszig, "Introductory Mathematical Statistics", John Wiley, New York, 1970, Section 10.3, pages 144-146.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress of freedom distribution parameter</param>
		public static double TDistProbability(double X, int DegressOfFreedom)
		{
			double den = Mathematics.Pow(DegressOfFreedom / (DegressOfFreedom + X * X), (1.0 + DegressOfFreedom) / 2.0) ;
			double num = Mathematics.Sqrt(DegressOfFreedom) * Beta(DegressOfFreedom / 2.0, 0.5) ;
			return (den / num) ;
		}
		/// <summary>
		/// Calculates the T-Student Probability Density Function.
		/// References:
		///  References:
		///  1) E. Kreyszig, "Introductory Mathematical Statistics", John Wiley, New York, 1970, Section 10.3, pages 144-146.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress of freedom distribution parameter</param>
		public static float TDistProbability(float X, int DegressOfFreedom)
		{
			double den = Mathematics.Pow(DegressOfFreedom / (DegressOfFreedom + X * X), (1.0 + DegressOfFreedom) / 2.0) ;
			double num = Mathematics.Sqrt(DegressOfFreedom) * Beta(DegressOfFreedom / 2.0, 0.5) ;
			return (float)(den / num) ;
		}
		/// <summary>
		/// Calculates the T-Student Probability Density Function.
		/// References:
		///  References:
		///  1) E. Kreyszig, "Introductory Mathematical Statistics", John Wiley, New York, 1970, Section 10.3, pages 144-146.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress of freedom distribution parameter</param>
		public static Decimal TDistProbability(Decimal X, int DegressOfFreedom)
		{
			Decimal den = Calculus.Pow(DegressOfFreedom / (DegressOfFreedom + X * X), (1.0M + DegressOfFreedom) / 2.0M) ;
			Decimal num = Calculus.Sqrt(DegressOfFreedom) * Beta(DegressOfFreedom / 2.0M, 0.5M) ;
			return (den / num) ;
		}
		/// <summary>
		/// Calculates the T-Student Cumulative Distribution Function.
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 26.7.
		/// 2) L. Devroye, "Non-Uniform Random Variate Generation", Springer-Verlag, 1986
		/// 3) E. Kreyszig, "Introductory Mathematical Statistics", John Wiley, 1970, Section 10.3, pages 144-146.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress of freedom distribution parameter</param>
		public static double TDistCumulative(double X, int DegressOfFreedom)
		{
			//			function p = tcdf(x,v);    
			double normcutoff = 1e7;
			//			% use special cases for some specific values of v
			//			k = find(v==1);
			//					% See Devroye pages 29 and 450.
			//					% (This is also the Cauchy distribution)
			//			if any(k)
			//					p(k) = .5 + atan(x(k))/pi;
			//			end
			double v = DegressOfFreedom ;
			if (v <= 0)
				return double.NaN ;
			if (v == 1)
				return Mathematics.Atan(X)/Mathematics.PI + .5 ;
			//			k = find(v>=normcutoff);
			//			if any(k)
			//					p(k) = normcdf(x(k));
			//			end
			if (v >= normcutoff)
				return NormCumulative(X, 0, 1) ;
			//
			//			% See Abramowitz and Stegun, formulas 26.5.27 and 26.7.1
			//			k = find(x ~= 0 & v ~= 1 & v > 0 & v < normcutoff);
			//			if any(k),                            % first compute F(-|x|)
			//					xx = v(k) ./ (v(k) + x(k).^2);
			//					p(k) = betainc(xx, v(k)/2, 0.5)/2;
			//			end
			double xx = v / (v + X * X) ;
			double p = BetaIncomplete(xx, v / 2.0, 0.5) / 2 ;
			//
			//			% Adjust for x>0.  Right now p<0.5, so this is numerically safe.
			//			k = find(x > 0 & v ~= 1 & v > 0 & v < normcutoff);
			//			if any(k), p(k) = 1 - p(k); end
			if (X > 0)
				p = 1 - p ;
			return p ;
			//
			//			p(x == 0 & v ~= 1 & v > 0) = 0.5;
			//
			//			% Return NaN for invalid inputs.
			//			p(v <= 0 | isnan(x) | isnan(v)) = NaN;
		}
		/// <summary>
		/// Calculates the T-Student Cumulative Distribution Function.
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 26.7.
		/// 2) L. Devroye, "Non-Uniform Random Variate Generation", Springer-Verlag, 1986
		/// 3) E. Kreyszig, "Introductory Mathematical Statistics", John Wiley, 1970, Section 10.3, pages 144-146.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress of freedom distribution parameter</param>
		public static float TDistCumulative(float X, int DegressOfFreedom)
		{
			float normcutoff = 1e7F;
			float v = DegressOfFreedom ;
			if (v <= 0)
				return float.NaN ;
			if (v == 1)
				return (float)(Mathematics.Atan(X)/Mathematics.PI) + .5F ;
			if (v >= normcutoff)
				return NormCumulative(X, 0F, 1F) ;
			float xx = v / (v + X * X) ;
			float p = BetaIncomplete(xx, v / 2.0F, 0.5F) / 2F ;
			if (X > 0)
				p = 1 - p ;
			return p ;
		}
		/// <summary>
		/// Calculates the T-Student Cumulative Distribution Function.
		/// References:
		/// 1) M. Abramowitz and I. A. Stegun, "Handbook of Mathematical Functions", Government Printing Office, 1964, 26.7.
		/// 2) L. Devroye, "Non-Uniform Random Variate Generation", Springer-Verlag, 1986
		/// 3) E. Kreyszig, "Introductory Mathematical Statistics", John Wiley, 1970, Section 10.3, pages 144-146.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress of freedom distribution parameter</param>
		public static Decimal TDistCumulative(Decimal X, int DegressOfFreedom)
		{
			Decimal normcutoff = 1e7M;
			Decimal v = DegressOfFreedom ;
			if (v <= 0)
				return -1M ;
			if (v == 1)
				return (Decimal)((decimal)(Mathematics.Atan((double)X))/Calculus.Pi) + .5M ;
			if (v >= normcutoff)
				return NormCumulative(X, 0M, 1M) ;
			Decimal xx = v / (v + X * X) ;
			Decimal p = BetaIncomplete(xx, v / 2.0M, 0.5M) / 2M ;
			if (X > 0)
				p = 1 - p ;
			return p ;
		}
		#endregion
		#region TInv
		/// <summary>
		/// Calculates the Inverse of the 2-tailed t-Student Probability Density Function.
		/// </summary>
		/// <param name="Y">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress Of Freedom parameter of t-Student Distribution</param>
		public static double TInv(double Y, int DegressOfFreedom)
		{
			int max_count = 1000 ;
			double stepSize = 10.0 ;
			double xl = .0 ;
			double xu = xl + 10.0 ;
			double yl = TDist(xl, DegressOfFreedom, 2) ;
			double yu = TDist(xu, DegressOfFreedom, 2) ;
			#region Finding an initial approach
			while(!(yl >= Y && yu <= Y))
			{
				xl += stepSize ;
				xu += stepSize ;
				yl = yu ;
				yu = TDist(xu, DegressOfFreedom, 2) ;
			}
			#endregion
			int count = 0 ;
			while(count < max_count && Mathematics.Abs(yu - Y) > (double.Epsilon * 100))
			{
				double m = (xu + xl) / 2.0 ;
				double ym = TDist(m, DegressOfFreedom, 2) ;
				if (ym <= Y)
				{
					xu = m ;
					yu = ym ;
				}
				else
				{
					xl = m ;
					yl = ym ;
				}
				count ++ ;
			}
			return xu ;
		}
		/// <summary>
		/// Calculates the Inverse of the 2-tailed t-Student Probability Density Function.
		/// </summary>
		/// <param name="Y">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress Of Freedom parameter of t-Student Distribution</param>
		public static decimal TInv(decimal Y, int DegressOfFreedom)
		{
			int max_count = 1000 ;
			decimal stepSize = 10.0M ;
			decimal xl = .0M ;
			decimal xu = xl + 10.0M ;
			decimal yl = TDist(xl, DegressOfFreedom, 2) ;
			decimal yu = TDist(xu, DegressOfFreedom, 2) ;
			#region Finding an initial approach
			while(!(yl >= Y && yu <= Y))
			{
				xl += stepSize ;
				xu += stepSize ;
				yl = yu ;
				yu = TDist(xu, DegressOfFreedom, 2) ;
			}
			#endregion
			int count = 0 ;
			while(count < max_count && Mathematics.Abs(yu - Y) > (1e-24M))
			{
				decimal m = (xu + xl) / 2.0M ;
				decimal ym = TDist(m, DegressOfFreedom, 2) ;
				if (ym <= Y)
				{
					xu = m ;
					yu = ym ;
				}
				else
				{
					xl = m ;
					yl = ym ;
				}
				count ++ ;
			}
			return xu ;
		}
		/// <summary>
		/// Calculates the Inverse of the 2-tailed t-Student Probability Density Function.
		/// </summary>
		/// <param name="Y">Value to evaluate at</param>
		/// <param name="DegressOfFreedom">Degress Of Freedom parameter of t-Student Distribution</param>
		public static float TInv(float Y, int DegressOfFreedom)
		{
			int max_count = 1000 ;
			float stepSize = 10.0F ;
			float xl = .0F ;
			float xu = xl + 10.0F ;
			float yl = TDist(xl, DegressOfFreedom, 2) ;
			float yu = TDist(xu, DegressOfFreedom, 2) ;
			#region Finding an initial approach
			while(!(yl >= Y && yu <= Y))
			{
				xl += stepSize ;
				xu += stepSize ;
				yl = yu ;
				yu = TDist(xu, DegressOfFreedom, 2) ;
			}
			#endregion
			int count = 0 ;
			while(count < max_count && Mathematics.Abs(yu - Y) > (float.Epsilon * 100F))
			{
				float m = (xu + xl) / 2.0F ;
				float ym = TDist(m, DegressOfFreedom, 2) ;
				if (ym <= Y)
				{
					xu = m ;
					yu = ym ;
				}
				else
				{
					xl = m ;
					yl = ym ;
				}
				count ++ ;
			}
			return xu ;
		}
		#endregion
		#region Trend
		/// <summary>
		/// Calculates values along a linear trend. Fits a straight line(y = m*x + b) (using the method of least squares) to the 
		/// arrays knownYs and knownXs.
		/// </summary>
		/// <param name="knownYs">Known Y values et</param>
		/// <param name="knownXs">Known X values set</param>
		/// <param name="newXs">Values to predict line</param>
		/// <param name="calculateIntercept">If true b is calculated, if false b is forced to 0</param>
		/// <returns>The y-values along that line for the array of newXs that you specify</returns>
		public static double[] Trend(double[] knownYs, double[] knownXs, double[] newXs, bool calculateIntercept)
		{
			SlopeInterceptPairD pair = LinearFit(knownYs, knownXs) ;
			if (!calculateIntercept) 
				pair.Intercept = .0 ;
			double[] result = new double[newXs.Length] ;
			for(int i = 0; i < newXs.Length; i ++)
				result[i] = pair.Slope * newXs[i] + pair.Intercept ;
			return result ;
		}
		/// <summary>
		/// Calculates values along a linear trend. Fits a straight line(y = m*x + b) (using the method of least squares) to the 
		/// arrays knownYs and knownXs.
		/// </summary>
		/// <param name="knownYs">Known Y values et</param>
		/// <param name="knownXs">Known X values set</param>
		/// <param name="newXs">Values to predict line</param>
		/// <param name="calculateIntercept">If true b is calculated, if false b is forced to 0</param>
		/// <returns>The y-values along that line for the array of newXs that you specify</returns>
		public static float[] Trend(float[] knownYs, float[] knownXs, float[] newXs, bool calculateIntercept)
		{
			SlopeInterceptPairF pair = LinearFit(knownYs, knownXs) ;
			if (!calculateIntercept) 
				pair.Intercept = .0F ;
			float[] result = new float[newXs.Length] ;
			for(int i = 0; i < newXs.Length; i ++)
				result[i] = pair.Slope * newXs[i] + pair.Intercept ;
			return result ;
		}
		/// <summary>
		/// Calculates values along a linear trend. Fits a straight line(y = m*x + b) (using the method of least squares) to the 
		/// arrays knownYs and knownXs.
		/// </summary>
		/// <param name="knownYs">Known Y values et</param>
		/// <param name="knownXs">Known X values set</param>
		/// <param name="newXs">Values to predict line</param>
		/// <param name="calculateIntercept">If true b is calculated, if false b is forced to 0</param>
		/// <returns>The y-values along that line for the array of newXs that you specify</returns>
		public static decimal[] Trend(decimal[] knownYs, decimal[] knownXs, decimal[] newXs, bool calculateIntercept)
		{
			SlopeInterceptPairM pair = LinearFit(knownYs, knownXs) ;
			if (!calculateIntercept) 
				pair.Intercept = .0M ;
			decimal[] result = new decimal[newXs.Length] ;
			for(int i = 0; i < newXs.Length; i ++)
				result[i] = pair.Slope * newXs[i] + pair.Intercept ;
			return result ;
		}
		#endregion
		#region TrimMean
		/// <summary>
		/// Calculates the mean of data excluding percent elements from the top and the bottom tails of data.
		/// </summary>
		public static double TrimMean(double[] data, double percent)
		{
			data = Sort(data) ;
			int exclude = (int)(data.Length * percent) ;
			return RangedAverage(data, exclude / 2, data.Length - 1 - exclude / 2) ;
		}
		/// <summary>
		/// Calculates the mean of data excluding percent elements from the top and the bottom tails of data.
		/// </summary>
		public static float TrimMean(float[] data, float percent)
		{
			data = Sort(data) ;
			int exclude = (int)(data.Length * percent) ;
			return RangedAverage(data, exclude / 2, data.Length - 1 - exclude / 2) ;
		}
		/// <summary>
		/// Calculates the mean of data excluding percent elements from the top and the bottom tails of data.
		/// </summary>
		public static decimal TrimMean(decimal[] data, decimal percent)
		{
			data = Sort(data) ;
			int exclude = (int)(data.Length * percent) ;
			return RangedAverage(data, exclude / 2, data.Length - 1 - exclude / 2) ;
		}
		#endregion
		#region TTest
		/// <summary>
		/// Returns the probability associated with a Student's t-Test. Use TTEST to determine whether two samples
		/// are likely to have come from the same two underlying populations that have the same mean.
		/// </summary>
		/// <param name="X">First data array</param>
		/// <param name="Y">Second data array</param>
		/// <param name="tails">Specifies the number of tails.</param>
		/// <param name="type">Specifies the kind of test to perform
		/// 1) Paired Test
		/// 2) 2 Sample equal variance (homoscedastic)
		/// 3) 2 Sample unequal variance (heteroscedastic)
		/// </param>
		/// <returns></returns>
		public static double TTest(double[] X, double[] Y, int tails, int type)
		{
			if (tails != 1 && tails != 2)
				throw new ArgumentException("tails must be either 1 or 2") ;
			switch(type)
			{
				case 1 : 
					return TPairedTest(X, Y) / (3.0 - tails) ;
				case 2:
					return THomoscedasticTest(X, Y) / (3.0 - tails) ;
				case 3:
					return THereteroscedasticTest(X, Y) / (3.0 - tails) ;
			}
			throw new ArgumentException("type must be 1, 2 or 3") ;
		}
		/// <summary>
		/// Returns the probability associated with a Student's t-Test. Use TTEST to determine whether two samples
		/// are likely to have come from the same two underlying populations that have the same mean.
		/// </summary>
		/// <param name="X">First data array</param>
		/// <param name="Y">Second data array</param>
		/// <param name="tails">Specifies the number of tails.</param>
		/// <param name="type">Specifies the kind of test to perform
		/// 1) Paired Test
		/// 2) 2 Sample equal variance (homoscedastic)
		/// 3) 2 Sample unequal variance (heteroscedastic)
		/// </param>
		/// <returns></returns>
		public static float TTest(float[] X, float[] Y, int tails, int type)
		{
			if (tails != 1 && tails != 2)
				throw new ArgumentException("tails must be either 1 or 2") ;
			switch(type)
			{
				case 1 : 
					return TPairedTest(X, Y) / (3.0F - tails) ;
				case 2:
					return THomoscedasticTest(X, Y) / (3.0F - tails) ;
				case 3:
					return THereteroscedasticTest(X, Y) / (3.0F - tails) ;
			}
			throw new ArgumentException("type must be 1, 2 or 3") ;
		}
		/// <summary>
		/// Returns the probability associated with a Student's t-Test. Use TTEST to determine whether two samples
		/// are likely to have come from the same two underlying populations that have the same mean.
		/// </summary>
		/// <param name="X">First data array</param>
		/// <param name="Y">Second data array</param>
		/// <param name="tails">Specifies the number of tails.</param>
		/// <param name="type">Specifies the kind of test to perform
		/// 1) Paired Test
		/// 2) 2 Sample equal variance (homoscedastic)
		/// 3) 2 Sample unequal variance (heteroscedastic)
		/// </param>
		/// <returns></returns>
		public static decimal TTest(decimal[] X, decimal[] Y, int tails, int type)
		{
			if (tails != 1 && tails != 2)
				throw new ArgumentException("tails must be either 1 or 2") ;
			switch(type)
			{
				case 1 : 
					return TPairedTest(X, Y) / (3.0M - tails) ;
				case 2:
					return THomoscedasticTest(X, Y) / (3.0M - tails) ;
				case 3:
					return THereteroscedasticTest(X, Y) / (3.0M - tails) ;
			}
			throw new ArgumentException("type must be 1, 2 or 3") ;
		}
		#endregion
		#region TTestTools
		/// <summary>
		/// Calculates the 2 tailied probability associated with a Student's Paired t-Test.
		/// </summary>
		/// <param name="X">First sample</param>
		/// <param name="Y">Second sample</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static double TPairedTest(double[] X, double[] Y)
		{
			if (X.Length != Y.Length)
				throw new ArrayDimensionsException() ;
			double aveX = Average(X) ;
			double aveY = Average(Y) ;
			double sum = .0 ;
			int n = X.Length ;
			for(int i = 0; i < n; i ++)
			{
				double term = X[i] - aveX - Y[i] + aveY ;
				sum += term * term ;
			}
			double t = Mathematics.Abs(aveX - aveY) * Mathematics.Sqrt(n * (n - 1.0) / sum) ;
			return TDistCumulative(t, n - 1) ;
		}
		/// <summary>
		/// Calculates the 2 tailied probability associated with a Student's Paired t-Test.
		/// </summary>
		/// <param name="X">First sample</param>
		/// <param name="Y">Second sample</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static float TPairedTest(float[] X, float[] Y)
		{
			if (X.Length != Y.Length)
				throw new ArrayDimensionsException() ;
			float aveX = Average(X) ;
			float aveY = Average(Y) ;
			float sum = .0F ;
			int n = X.Length ;
			for(int i = 0; i < n; i ++)
			{
				float term = X[i] - aveX - Y[i] + aveY ;
				sum += term * term ;
			}
			float t = (float)(Mathematics.Abs(aveX - aveY) * Mathematics.Sqrt(n * (n - 1.0) / sum)) ;
			return TDistCumulative(t, n - 1) ;
		}
		/// <summary>
		/// Calculates the 2 tailied probability associated with a Student's Paired t-Test.
		/// </summary>
		/// <param name="X">First sample</param>
		/// <param name="Y">Second sample</param>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters aren't 
		/// equal</exception>
		public static decimal TPairedTest(decimal[] X, decimal[] Y)
		{
			if (X.Length != Y.Length)
				throw new ArrayDimensionsException() ;
			decimal aveX = Average(X) ;
			decimal aveY = Average(Y) ;
			decimal sum = .0M ;
			int n = X.Length ;
			for(int i = 0; i < n; i ++)
			{
				decimal term = X[i] - aveX - Y[i] + aveY ;
				sum += term * term ;
			}
			decimal t = Mathematics.Abs(aveX - aveY) * Calculus.Sqrt(n * (n - 1.0M) / sum) ;
			return TDistCumulative(t, n - 1) ;
		}
		/// <summary>
		/// Calculates the 2 tailed probability associated with a Student's Homoscedastic t-Test.
		/// References:
		/// 1) E. Kreyszig, "Introductory Mathematical Statistics", John Wiley, 1970, section 13.4. (Table 13.4.1 on page 210)
		/// </summary>
		/// <param name="X">First sample</param>
		/// <param name="Y">Second sample</param>
		public static double THomoscedasticTest(double[] X, double[] Y)
		{
			double dfx = X.Length - 1.0 ;
			double dfy = Y.Length - 1.0 ;
			double dfe = dfx + dfy ;
			double msx = dfx * Var(X) ;
			double msy = dfy * Var(Y) ;
			double difference = Average(X) - Average(Y) ;
			double pooleds = Mathematics.Sqrt((msx + msy) * (1.0 / (dfx + 1.0) + 1.0/(dfy + 1.0)) / dfe) ;
			double ratio = difference / pooleds ;
			double tstat = ratio ;
			double df = dfe ;
			double p = TDistCumulative(ratio, (int)df) ;
			p = 2.0 * Mathematics.Min(p, 1.0 - p) ;
			return p ;
		}
		/// <summary>
		/// Calculates the 2 tailed probability associated with a Student's Homoscedastic t-Test.
		/// References:
		/// 1) E. Kreyszig, "Introductory Mathematical Statistics", John Wiley, 1970, section 13.4. (Table 13.4.1 on page 210)
		/// </summary>
		/// <param name="X">First sample</param>
		/// <param name="Y">Second sample</param>
		public static float THomoscedasticTest(float[] X, float[] Y)
		{
			double dfx = X.Length - 1.0 ;
			double dfy = Y.Length - 1.0 ;
			double dfe = dfx + dfy ;
			double msx = dfx * Var(X) ;
			double msy = dfy * Var(Y) ;
			double difference = Average(X) - Average(Y) ;
			double pooleds = Mathematics.Sqrt((msx + msy) * (1.0 / (dfx + 1.0) + 1.0/(dfy + 1.0)) / dfe) ;
			double ratio = difference / pooleds ;
			double tstat = ratio ;
			double df = dfe ;
			double p = TDistCumulative(ratio, (int)df) ;
			p = 2.0 * Mathematics.Min(p, 1.0 - p) ;
			return (float)p ;
		}
		/// <summary>
		/// Calculates the 2 tailed probability associated with a Student's Homoscedastic t-Test.
		/// References:
		/// 1) E. Kreyszig, "Introductory Mathematical Statistics", John Wiley, 1970, section 13.4. (Table 13.4.1 on page 210)
		/// </summary>
		/// <param name="X">First sample</param>
		/// <param name="Y">Second sample</param>
		public static decimal THomoscedasticTest(decimal[] X, decimal[] Y)
		{
			decimal dfx = X.Length - 1.0M ;
			decimal dfy = Y.Length - 1.0M ;
			decimal dfe = dfx + dfy ;
			decimal msx = dfx * Var(X) ;
			decimal msy = dfy * Var(Y) ;
			decimal difference = Average(X) - Average(Y) ;
			decimal pooleds = Calculus.Sqrt((msx + msy) * (1.0M / (dfx + 1.0M) + 1.0M / (dfy + 1.0M)) / dfe) ;
			decimal ratio = difference / pooleds ;
			decimal tstat = ratio ;
			decimal df = dfe ;
			decimal p = TDistCumulative(ratio, (int)df) ;
			p = 2.0M * Mathematics.Min(p, 1.0M - p) ;
			return p ;
		}
		/// <summary>
		/// Calculates the 2 tailed probability associated with a Student's Heteroscedastic t-Test.
		/// </summary>
		/// <param name="X">First sample</param>
		/// <param name="Y">Second sample</param>
		public static double THereteroscedasticTest(double[] X, double[] Y)
		{
			double aveX = Average(X) ;
			double aveY = Average(Y) ;
			double s2X = Var(X) ;
			double s2Y = Var(Y) ;
			double nx = X.Length ;
			double ny = Y.Length ;
			double fracX = s2X / nx ;
			double fracY = s2Y / ny ;
			double fracX2 = fracX * fracX ;
			double fracY2 = fracY * fracY ;
			double t = (aveX - aveY) / Mathematics.Sqrt(s2X / (nx + 1.0) + s2Y / (ny + 1.0)) ;
			double num = fracX + fracY ;
			num *= num ;
			double den = fracX2 / (nx - 1.0) + fracY2 / (ny - 1.0) ;
			int df = (int)Mathematics.Round(num / den) ;
			double p = TDistCumulative(t, df) ;
			return 2 * Mathematics.Min(p, 1.0 - p) ;
		}
		/// <summary>
		/// Calculates the 2 tailed probability associated with a Student's Heteroscedastic t-Test.
		/// </summary>
		/// <param name="X">First sample</param>
		/// <param name="Y">Second sample</param>
		public static float THereteroscedasticTest(float[] X, float[] Y)
		{
			double aveX = Average(X) ;
			double aveY = Average(Y) ;
			double s2X = Var(X) ;
			double s2Y = Var(Y) ;
			double nx = X.Length ;
			double ny = Y.Length ;
			double fracX = s2X / nx ;
			double fracY = s2Y / ny ;
			double fracX2 = fracX * fracX ;
			double fracY2 = fracY * fracY ;
			double t = (aveX - aveY) / Mathematics.Sqrt(s2X / (nx + 1.0) + s2Y / (ny + 1.0)) ;
			double num = fracX + fracY ;
			num *= num ;
			double den = fracX2 / (nx - 1.0) + fracY2 / (ny - 1.0) ;
			int df = (int)Mathematics.Round(num / den) ;
			double p = TDistCumulative(t, df) ;
			return (float)(2 * Mathematics.Min(p, 1.0 - p)) ;
		}
		/// <summary>
		/// Calculates the 2 tailed probability associated with a Student's Heteroscedastic t-Test.
		/// </summary>
		/// <param name="X">First sample</param>
		/// <param name="Y">Second sample</param>
		public static decimal THereteroscedasticTest(decimal[] X, decimal[] Y)
		{
			decimal aveX = Average(X) ;
			decimal aveY = Average(Y) ;
			decimal s2X = Var(X) ;
			decimal s2Y = Var(Y) ;
			decimal nx = X.Length ;
			decimal ny = Y.Length ;
			decimal fracX = s2X / nx ;
			decimal fracY = s2Y / ny ;
			decimal fracX2 = fracX * fracX ;
			decimal fracY2 = fracY * fracY ;
			decimal t = (aveX - aveY) / Calculus.Sqrt(s2X / (nx + 1.0M) + s2Y / (ny + 1.0M)) ;
			decimal num = fracX + fracY ;
			num *= num ;
			decimal den = fracX2 / (nx - 1.0M) + fracY2 / (ny - 1.0M) ;
			int df = (int)Mathematics.Round(num / den) ;
			decimal p = TDistCumulative(t, df) ;
			return 2 * Mathematics.Min(p, 1.0M - p) ;
		}
		#endregion
		#region Var
		/// <summary>
		/// Calculates the variance based on a sample.
		/// </summary>
		/// <param name="X">Sample data</param>
		public static double Var(double[] X)
		{
			double sumX = .0 ;
			double sumX2 = .0 ;
			int n = X.Length ;
			foreach(double x in X)
			{
				sumX += x ;
				sumX2 += x * x ;
			}
			return (n * sumX2 - sumX * sumX) / (double)(n * (n - 1));
		}
		/// <summary>
		/// Calculates the variance based on a sample.
		/// </summary>
		/// <param name="X">Sample data</param>
		public static float Var(float[] X)
		{
			float sumX = .0F ;
			float sumX2 = .0F ;
			int n = X.Length ;
			foreach(float x in X)
			{
				sumX += x ;
				sumX2 += x * x ;
			}
			return (n * sumX2 - sumX * sumX) / (float)(n * (n - 1));
		}
		/// <summary>
		/// Calculates the variance based on a sample.
		/// </summary>
		/// <param name="X">Sample data</param>
		public static Decimal Var(Decimal[] X)
		{
			Decimal sumX = .0M ;
			Decimal sumX2 = .0M ;
			int n = X.Length ;
			foreach(Decimal x in X)
			{
				sumX += x ;
				sumX2 += x * x ;
			}
			return (n * sumX2 - sumX * sumX) / (Decimal)(n * (n - 1));
		}
		#endregion
		#region VarP
		/// <summary>
		/// Calculates the variance based on a polulation.
		/// </summary>
		/// <param name="X">Population data</param>
		public static double VarP(double[] X)
		{
			double sumX = .0 ;
			double sumX2 = .0 ;
			int n = X.Length ;
			foreach(double x in X)
			{
				sumX += x ;
				sumX2 += x * x ;
			}
			return (n * sumX2 - sumX * sumX) / (double)(n * n);			
		}
		/// <summary>
		/// Calculates the variance based on a polulation.
		/// </summary>
		/// <param name="X">Population data</param>
		public static float VarP(float[] X)
		{
			float sumX = .0F ;
			float sumX2 = .0F ;
			int n = X.Length ;
			foreach(float x in X)
			{
				sumX += x ;
				sumX2 += x * x ;
			}
			return (n * sumX2 - sumX * sumX) / (float)(n * n);			
		}
		/// <summary>
		/// Calculates the variance based on a polulation.
		/// </summary>
		/// <param name="X">Population data</param>
		public static Decimal VarP(Decimal[] X)
		{
			Decimal sumX = .0M ;
			Decimal sumX2 = .0M ;
			int n = X.Length ;
			foreach(Decimal x in X)
			{
				sumX += x ;
				sumX2 += x * x ;
			}
			return (n * sumX2 - sumX * sumX) / (Decimal)(n * n);			
		}
		#endregion
		#region Weibull
		/// <summary>
		/// Calculates the Weibull distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of distribution</param>
		/// <param name="beta">Beta parameter of distribution</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static double Weibull(double X, double alpha, double beta, bool Cumulative)
		{
			return Cumulative? WeibullCumulative(X, alpha, beta) : WeibullProbability(X, alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Weibull distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of distribution</param>
		/// <param name="beta">Beta parameter of distribution</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static float Weibull(float X, float alpha, float beta, bool Cumulative)
		{
			return Cumulative? WeibullCumulative(X, alpha, beta) : WeibullProbability(X, alpha, beta) ;
		}
		/// <summary>
		/// Calculates the Weibull distribution.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of distribution</param>
		/// <param name="beta">Beta parameter of distribution</param>
		/// <param name="Cumulative">if true, function returns the Cumulative Distribution Function, if false, 
		/// returns the Probability Density Function.</param>
		public static decimal Weibull(decimal X, decimal alpha, decimal beta, bool Cumulative)
		{
			return Cumulative? WeibullCumulative(X, alpha, beta) : WeibullProbability(X, alpha, beta) ;
		}
		#endregion
		#region Weibull Tools
		/// <summary>
		/// Calculates the Weibull Probability Density Function.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of distribution</param>
		/// <param name="beta">Beta parameter of distribution</param>
		public static double WeibullProbability(double X, double alpha, double beta)
		{
			return 1 - Mathematics.Exp(- Mathematics.Pow(X / beta, alpha)) ;
		}
		/// <summary>
		/// Calculates the Weibull Probability Density Function.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of distribution</param>
		/// <param name="beta">Beta parameter of distribution</param>
		public static float WeibullProbability(float X, float alpha, float beta)
		{
			return 1F - (float)Mathematics.Exp(- Mathematics.Pow(X / beta, alpha)) ;
		}
		/// <summary>
		/// Calculates the Weibull Probability Density Function.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		/// <param name="alpha">Alpha parameter of distribution</param>
		/// <param name="beta">Beta parameter of distribution</param>
		public static decimal WeibullProbability(decimal X, decimal alpha, decimal beta)
		{
			return 1 - Calculus.Exp(- Calculus.Pow(X / beta, alpha)) ;
		}
		/// <summary>
		/// Calculates the Weibull Cumulative Distribution Function.
		/// </summary>
		/// <param name="X"></param>
		/// <param name="alpha"></param>
		/// <param name="beta"></param>
		/// <returns></returns>
		public static double WeibullCumulative(double X, double alpha, double beta)
		{
			return alpha * Mathematics.Pow(X, alpha - 1) / Mathematics.Pow(beta, alpha) *
				Mathematics.Exp(- Mathematics.Pow(X / beta, alpha)) ;
		}
		/// <summary>
		/// Calculates the Weibull Cumulative Distribution Function.
		/// </summary>
		/// <param name="X"></param>
		/// <param name="alpha"></param>
		/// <param name="beta"></param>
		/// <returns></returns>
		public static float WeibullCumulative(float X, float alpha, float beta)
		{
			return (float)(alpha * Mathematics.Pow(X, alpha - 1) / Mathematics.Pow(beta, alpha) *
				Mathematics.Exp(- Mathematics.Pow(X / beta, alpha))) ;
		}
		/// <summary>
		/// Calculates the Weibull Cumulative Distribution Function.
		/// </summary>
		/// <param name="X"></param>
		/// <param name="alpha"></param>
		/// <param name="beta"></param>
		/// <returns></returns>
		public static decimal WeibullCumulative(decimal X, decimal alpha, decimal beta)
		{
			return alpha * Calculus.Pow(X, alpha - 1M) / Calculus.Pow(beta, alpha) *
				Calculus.Exp(- Calculus.Pow(X / beta, alpha)) ;
		}
		#endregion
		#region ZTest
		/// <summary>
		/// Returns the two-tailed P-value of a z-test. The z-test generates a standard score for x with respect
		/// to the data set (array) and returns the two-tailed probability for the normal distribution. You can 
		/// use this function to assess the likelihood that a particular observation is drawn from a particular 
		/// population.
		/// </summary>
		/// <param name="array">Values to test against</param>
		/// <param name="X">Value to test</param>
		/// <param name="sigma">population known standard deviation. If omited sample standard deviation will be used.</param>
		public static double ZTest(double[] array, double X, double sigma)
		{
			return 1 - NormSDist((Average(array) - X) / (sigma / Mathematics.Sqrt(array.Length)), true) ;
		}
		/// <summary>
		/// Returns the two-tailed P-value of a z-test. The z-test generates a standard score for x with respect
		/// to the data set (array) and returns the two-tailed probability for the normal distribution. You can 
		/// use this function to assess the likelihood that a particular observation is drawn from a particular 
		/// population.
		/// </summary>
		/// <param name="array">Values to test against</param>
		/// <param name="X">Value to test</param>
		public static double ZTest(double[] array, double X)
		{
			return ZTest(array, X, StDev(array)) ;
		}
		/// <summary>
		/// Returns the two-tailed P-value of a z-test. The z-test generates a standard score for x with respect
		/// to the data set (array) and returns the two-tailed probability for the normal distribution. You can 
		/// use this function to assess the likelihood that a particular observation is drawn from a particular 
		/// population.
		/// </summary>
		/// <param name="array">Values to test against</param>
		/// <param name="X">Value to test</param>
		/// <param name="sigma">population known standard deviation. If omited sample standard deviation will be used.</param>
		public static float ZTest(float[] array, float X, float sigma)
		{
			return (float)(1 - NormSDist((Average(array) - X) / (sigma / Mathematics.Sqrt(array.Length)), true)) ;
		}
		/// <summary>
		/// Returns the two-tailed P-value of a z-test. The z-test generates a standard score for x with respect
		/// to the data set (array) and returns the two-tailed probability for the normal distribution. You can 
		/// use this function to assess the likelihood that a particular observation is drawn from a particular 
		/// population.
		/// </summary>
		/// <param name="array">Values to test against</param>
		/// <param name="X">Value to test</param>
		public static float ZTest(float[] array, float X)
		{
			return ZTest(array, X, StDev(array)) ;
		}
		/// <summary>
		/// Returns the two-tailed P-value of a z-test. The z-test generates a standard score for x with respect
		/// to the data set (array) and returns the two-tailed probability for the normal distribution. You can 
		/// use this function to assess the likelihood that a particular observation is drawn from a particular 
		/// population.
		/// </summary>
		/// <param name="array">Values to test against</param>
		/// <param name="X">Value to test</param>
		/// <param name="sigma">population known standard deviation. If omited sample standard deviation will be used.</param>
		public static decimal ZTest(decimal[] array, decimal X, decimal sigma)
		{
			return 1 - NormSDist((Average(array) - X) / (sigma / Calculus.Sqrt(array.Length)), true) ;
		}
		/// <summary>
		/// Returns the two-tailed P-value of a z-test. The z-test generates a standard score for x with respect
		/// to the data set (array) and returns the two-tailed probability for the normal distribution. You can 
		/// use this function to assess the likelihood that a particular observation is drawn from a particular 
		/// population.
		/// </summary>
		/// <param name="array">Values to test against</param>
		/// <param name="X">Value to test</param>
		public static decimal ZTest(decimal[] array, decimal X)
		{
			return ZTest(array, X, StDev(array)) ;
		}
		#endregion
		#region Error Functions and Tools
		/// <summary>
		/// Calculates the Error Function.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		public static double Erf(double X)
		{
			return ErfCore(X, 0) ;
		}
		/// <summary>
		/// Calculates the Error Function.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		public static float Erf(float X)
		{
			return ErfCore(X, 0) ;
		}
		/// <summary>
		/// Calculates the Error Function.
		/// </summary>
		/// <param name="X">Value to evaluate at</param>
		public static Decimal Erf(Decimal X)
		{
			return ErfCore(X, 0) ;
		}
		/// <summary>
		/// Core Algorithm for Error Functions
		/// </summary>
		/// <param name="X"></param>
		/// <param name="jint"></param>
		/// <returns></returns>
		public static double ErfCore(double X, double jint)
		{
			double result = double.NaN ;
			//			%
			//			%   evaluate  erf  for  |x| <= 0.46875
			//			%
			//					xbreak = 0.46875;
			double xbreak = 0.46875 ;
			//					k = find(abs(x) <= xbreak);
			//					if ~isempty(k)
			if (Mathematics.Abs(X) <= xbreak)
			{
				//							a = [3.16112374387056560e00; 1.13864154151050156e02;
				//									3.77485237685302021e02; 3.20937758913846947e03;
				//									1.85777706184603153e-1];
				double[] a = {
								 3.16112374387056560e00, 1.13864154151050156e02,
								 3.77485237685302021e02, 3.20937758913846947e03,
								 1.85777706184603153e-1
							 } ;
				//							b = [2.36012909523441209e01; 2.44024637934444173e02;
				//									1.28261652607737228e03; 2.84423683343917062e03];
				double[] b = {
								 2.36012909523441209e01, 2.44024637934444173e02,
								 1.28261652607737228e03, 2.84423683343917062e03
							 } ;
				//
				//									y = abs(x(k));
				double y = Mathematics.Abs(X) ;
				//									z = y .* y;
				double z = y * y ;
				//									xnum = a(5)*z;
				double xnum = a[4] * z ;
				//									xden = z;
				double xden = z ;
				//									for i = 1:3
				//										xnum = (xnum + a(i)) .* z;
				//										xden = (xden + b(i)) .* z;
				//									end
				for(int i = 0; i < 3; i++)
				{
					xnum = (xnum + a[i]) * z ;
					xden = (xden + b[i]) * z ;
				}
				//									result(k) = x(k) .* (xnum + a(4)) ./ (xden + b(4));
				result = X * (xnum + a[3]) / (xden + b[3]) ;
				if (jint != 0) 
					result = 1 - result ;
				if (jint == 2) 
					result = Mathematics.Exp(z) * result ;
				//									if jint ~= 0, result(k) = 1 - result(k); end
				//									if jint == 2, result(k) = exp(z) .* result(k); end
				//					end
			}
			//			%
			//			%   evaluate  erfc  for 0.46875 <= |x| <= 4.0
			//			%
			//					k = find((abs(x) > xbreak) &  (abs(x) <= 4.));
			//					if ~isempty(k)
			if (Mathematics.Abs(X) > xbreak && Mathematics.Abs(X) <= 4)
			{
				//							c = [5.64188496988670089e-1; 8.88314979438837594e00;
				//									6.61191906371416295e01; 2.98635138197400131e02;
				//									8.81952221241769090e02; 1.71204761263407058e03;
				//									2.05107837782607147e03; 1.23033935479799725e03;
				//									2.15311535474403846e-8];
				double[] c = {
								 5.64188496988670089e-1, 8.88314979438837594e00,
								 6.61191906371416295e01, 2.98635138197400131e02,
								 8.81952221241769090e02, 1.71204761263407058e03,
								 2.05107837782607147e03, 1.23033935479799725e03,
								 2.15311535474403846e-8
							 } ;
				//							d = [1.57449261107098347e01; 1.17693950891312499e02;
				//									5.37181101862009858e02; 1.62138957456669019e03;
				//									3.29079923573345963e03; 4.36261909014324716e03;
				//									3.43936767414372164e03; 1.23033935480374942e03];
				double[] d = {
								 1.57449261107098347e01, 1.17693950891312499e02,
								 5.37181101862009858e02, 1.62138957456669019e03,
								 3.29079923573345963e03, 4.36261909014324716e03,
								 3.43936767414372164e03, 1.23033935480374942e03
							 } ;
				//
				//									y = abs(x(k));
				double y = Mathematics.Abs(X) ;
				//									xnum = c(9)*y;
				double xnum = c[8] * y ;
				//									xden = y;
				double xden = y ;
				//									for i = 1:7
				//										xnum = (xnum + c(i)) .* y;
				//										xden = (xden + d(i)) .* y;
				//									end
				for(int i = 0; i < 7; i ++)
				{
					xnum = (xnum + c[i]) * y ;
					xden = (xden + d[i]) * y ;
				}
				//									result(k) = (xnum + c(8)) ./ (xden + d(8));
				result = (xnum + c[7]) / (xden + d[7]) ;
				//									if jint ~= 2
				//										z = fix(y*16)/16;
				//										del = (y-z).*(y+z);
				//										result(k) = exp(-z.*z) .* exp(-del) .* result(k);
				//									end
				if (jint != 2)
				{
					double z = Fix(y * 16) / 16 ;
					double del = (y - z) * (y + z) ;
					result = Mathematics.Exp(-z * z) * Mathematics.Exp(-del) * result ;
				}
				//					end
			}
			//			%
			//			%   evaluate  erfc  for |x| > 4.0
			//			%
			//					k = find(abs(x) > 4.0);
			//					if ~isempty(k)
			if (Mathematics.Abs(X) > 4)
			{
				//							p = [3.05326634961232344e-1; 3.60344899949804439e-1;
				//									1.25781726111229246e-1; 1.60837851487422766e-2;
				//									6.58749161529837803e-4; 1.63153871373020978e-2];
				double[] p = {
								 3.05326634961232344e-1, 3.60344899949804439e-1,
								 1.25781726111229246e-1, 1.60837851487422766e-2,
								 6.58749161529837803e-4, 1.63153871373020978e-2
							 } ;
				//							q = [2.56852019228982242e00; 1.87295284992346047e00;
				//									5.27905102951428412e-1; 6.05183413124413191e-2;
				//									2.33520497626869185e-3];
				double[] q = {
								 2.56852019228982242e00, 1.87295284992346047e00,
								 5.27905102951428412e-1, 6.05183413124413191e-2,
								 2.33520497626869185e-3
							 } ;
				//
				//									y = abs(x(k));
				double y = Mathematics.Abs(X) ;
				//									z = 1 ./ (y .* y);
				double z = 1 / (y * y) ;
				//									xnum = p(6).*z;
				double xnum = p[5] * z ;
				//									xden = z;
				double xden = z ;
				//									for i = 1:4
				//										xnum = (xnum + p(i)) .* z;
				//										xden = (xden + q(i)) .* z;
				//									end
				for(int i = 0; i < 4; i ++)
				{
					xnum = (xnum + p[i]) * z ;
					xden = (xden + q[i]) * z ;
				}
				//									result(k) = z .* (xnum + p(5)) ./ (xden + q(5));
				result = z * (xnum + p[4]) / (xden + q[4]) ;
				//									result(k) = (1/sqrt(pi) -  result(k)) ./ y;
				result = (1 / Mathematics.Sqrt(Mathematics.PI)) - result / y ;
				//									if jint ~= 2
				//										z = fix(y*16)/16;
				//										del = (y-z).*(y+z);
				//										result(k) = exp(-z.*z) .* exp(-del) .* result(k);
				//										k = find(~isfinite(result));
				//										result(k) = 0*k;
				//									end
				if (jint != 2)
				{
					z = Fix(y * 16) / 16 ;
					double del = (y - z) * (y + z) ;
					result = Mathematics.Exp(-z * z) * Mathematics.Exp(-del) * result ;
					if (! IsFinite(result)) result = 0 ;
				}
				//					end
			}
			//			%
			//			%   fix up for negative argument, erf, etc.
			//			%
			//					if jint == 0
			//									k = find(x > xbreak);
			//									result(k) = (0.5 - result(k)) + 0.5;
			//									k = find(x < -xbreak);
			//									result(k) = (-0.5 + result(k)) - 0.5;
			if (jint == 0)
			{
				if (X > xbreak)
					result = (0.5 - result) + 0.5 ;
				if (X < -xbreak)
					result = (-0.5 + result) - 0.5 ;
			}
				//					elseif jint == 1
				//									k = find(x < -xbreak);
				//									result(k) = 2. - result(k);
			else if (jint == 1)
			{
				if (X < -xbreak)
					result = 2 - result ;
			}
				//					else  % jint must = 2 
				//									k = find(x < -xbreak);
				//									z = fix(x(k)*16)/16;
				//									del = (x(k)-z).*(x(k)+z);
				//									y = exp(z.*z) .* exp(del);
				//									result(k) = (y+y) - result(k);
				//					end
			else
			{
				if (X < -xbreak)
				{
					double z = Fix(X * 16) / 16 ;
					double del = (X - z) * (X + z) ;
					double y = Mathematics.Exp(z * z) * Mathematics.Exp(del) ;
					result = (y + y) - result ;
				}
			}
			return result ;
		
		}
		/// <summary>
		/// Core Algorithm for Error Functions
		/// </summary>
		/// <param name="X"></param>
		/// <param name="jint"></param>
		/// <returns></returns>
		public static float ErfCore(float X, float jint)
		{
			return (float)ErfCore((double)X, (double)jint) ;
		}
		/// <summary>
		/// Core Algorithm for Error Functions
		/// </summary>
		/// <param name="X"></param>
		/// <param name="jint"></param>
		/// <returns></returns>
		public static Decimal ErfCore(Decimal X, Decimal jint)
		{
			Decimal result = -1 ;
			Decimal xbreak = 0.46875M ;
			if (Mathematics.Abs(X) <= xbreak)
			{
				Decimal[] a = {
								  3.16112374387056560e00M, 1.13864154151050156e02M,
								  3.77485237685302021e02M, 3.20937758913846947e03M,
								  1.85777706184603153e-1M
							  } ;
				Decimal[] b = {
								  2.36012909523441209e01M, 2.44024637934444173e02M,
								  1.28261652607737228e03M, 2.84423683343917062e03M
							  } ;
				Decimal y = Mathematics.Abs(X) ;
				Decimal z = y * y ;
				Decimal xnum = a[4] * z ;
				Decimal xden = z ;
				for(int i = 0; i < 3; i++)
				{
					xnum = (xnum + a[i]) * z ;
					xden = (xden + b[i]) * z ;
				}
				result = X * (xnum + a[3]) / (xden + b[3]) ;
				if (jint != 0) 
					result = 1 - result ;
				if (jint == 2) 
					result = Calculus.Exp(z) * result ;
			}
			if (Mathematics.Abs(X) > xbreak && Mathematics.Abs(X) <= 4)
			{
				Decimal[] c = {
								  5.64188496988670089e-1M, 8.88314979438837594e00M,
								  6.61191906371416295e01M, 2.98635138197400131e02M,
								  8.81952221241769090e02M, 1.71204761263407058e03M,
								  2.05107837782607147e03M, 1.23033935479799725e03M,
								  2.15311535474403846e-8M
							  } ;
				Decimal[] d = {
								  1.57449261107098347e01M, 1.17693950891312499e02M,
								  5.37181101862009858e02M, 1.62138957456669019e03M,
								  3.29079923573345963e03M, 4.36261909014324716e03M,
								  3.43936767414372164e03M, 1.23033935480374942e03M
							  } ;
				Decimal y = Mathematics.Abs(X) ;
				Decimal xnum = c[8] * y ;
				Decimal xden = y ;
				for(int i = 0; i < 7; i ++)
				{
					xnum = (xnum + c[i]) * y ;
					xden = (xden + d[i]) * y ;
				}
				result = (xnum + c[7]) / (xden + d[7]) ;
				if (jint != 2)
				{
					Decimal z = Fix(y * 16) / 16 ;
					Decimal del = (y - z) * (y + z) ;
					result = Calculus.Exp(-z * z) * Calculus.Exp(-del) * result ;
				}
			}
			if (Mathematics.Abs(X) > 4)
			{
				Decimal[] p = {
								  3.05326634961232344e-1M, 3.60344899949804439e-1M,
								  1.25781726111229246e-1M, 1.60837851487422766e-2M,
								  6.58749161529837803e-4M, 1.63153871373020978e-2M
							  } ;
				Decimal[] q = {
								  2.56852019228982242e00M, 1.87295284992346047e00M,
								  5.27905102951428412e-1M, 6.05183413124413191e-2M,
								  2.33520497626869185e-3M
							  } ;
				Decimal y = Mathematics.Abs(X) ;
				Decimal z = 1 / (y * y) ;
				Decimal xnum = p[5] * z ;
				Decimal xden = z ;
				for(int i = 0; i < 4; i ++)
				{
					xnum = (xnum + p[i]) * z ;
					xden = (xden + q[i]) * z ;
				}
				result = z * (xnum + p[4]) / (xden + q[4]) ;
				result = (1M / Calculus.SqrtPi) - result / y ;
				if (jint != 2)
				{
					z = Fix(y * 16) / 16 ;
					Decimal del = (y - z) * (y + z) ;
					result = Calculus.Exp(-z * z) * Calculus.Exp(-del) * result ;
				}
			}
			if (jint == 0M)
			{
				if (X > xbreak)
					result = (0.5M - result) + 0.5M ;
				if (X < -xbreak)
					result = (-0.5M + result) - 0.5M ;
			}
			else if (jint == 1M)
			{
				if (X < -xbreak)
					result = 2 - result ;
			}
			else
			{
				if (X < -xbreak)
				{
					Decimal z = Fix(X * 16) / 16 ;
					Decimal del = (X - z) * (X + z) ;
					Decimal y = Calculus.Exp(z * z) * Calculus.Exp(del) ;
					result = (y + y) - result ;
				}
			}
			return result ;
		}
		/// <summary>
		/// Calculates the Inverse Error Function
		/// </summary>
		/// <returns>X such that Y = Erf(X)</returns>
		public static double ErfInv(double Y)
		{
			double y = Y ;
			//			siz = size(y); y = y(:);
			//			if ~isreal(y), error('Y must be real.'); end
			//			x = repmat(NaN,size(y));
			double x = double.NaN ;
			//
			//			% Coefficients in rational approximations.
			//
			//			a = [ 0.886226899 -1.645349621  0.914624893 -0.140543331];
			double[] a = {0.886226899, -1.645349621, 0.914624893, -0.140543331} ;
			//			b = [-2.118377725  1.442710462 -0.329097515  0.012229801];
			double[] b = {-2.118377725, 1.442710462, -0.329097515, 0.012229801} ;
			//			c = [-1.970840454 -1.624906493  3.429567803  1.641345311];
			double[] c = {-1.970840454, -1.624906493, 3.429567803, 1.641345311} ;
			//			d = [ 3.543889200  1.637067800];
			double[] d = {3.543889200, 1.637067800} ;
			//
			//			% Central range.
			//
			//			y0 = .7;
			double y0 = .7 ;
			//			k = find(abs(y) <= y0);
			//			if ~isempty(k)
			//					z = y(k).*y(k);
			//					x(k) = y(k) .* (((a(4)*z+a(3)).*z+a(2)).*z+a(1)) ./ ...
			//							((((b(4)*z+b(3)).*z+b(2)).*z+b(1)).*z+1);
			//			end;
			if (Mathematics.Abs(y) <= y0)
			{
				double z = y * y ;
				x = y * (((a[3] * z + a[2]) * z + a[1]) * z + a[0]) /
					((((b[3] * z + b[2]) * z + b[1]) * z + b[0]) * z + 1) ;
			}
			//			% Near end points of range.
			//
			//			k = find(( y0 < y ) & (y <  1));
			//			if ~isempty(k)
			//					z = sqrt(-log((1-y(k))/2));
			//					x(k) = (((c(4)*z+c(3)).*z+c(2)).*z+c(1)) ./ ((d(2)*z+d(1)).*z+1);
			//			end
			if (y0 < y && y < 1)
			{
				double z = Mathematics.Sqrt(-Mathematics.Log((1 - y) / 2)) ;
				x = (((c[3] * z + c[2]) * z + c[1]) * z + c[0]) / ((d[1] * z + d[0]) * z + 1) ;
			}
			//
			//			k = find((-y0 > y ) & (y > -1));
			//			if ~isempty(k)
			//					z = sqrt(-log((1+y(k))/2));
			//					x(k) = -(((c(4)*z+c(3)).*z+c(2)).*z+c(1)) ./ ((d(2)*z+d(1)).*z+1);
			//			end
			if (-y0 > y && y > -1)
			{
				double z = Mathematics.Sqrt(-Mathematics.Log((1 + y) / 2)) ;
				x = -(((c[3] * z + c[2]) * z + c[1]) * z + c[0]) / ((d[1] * z + d[0]) * z + 1) ;
			}
			//
			//			% Two steps of Newton-Raphson correction to full accuracy.
			//			% Without these steps, erfinv(y) would be about 3 times
			//			% faster to compute, but accurate to only about 6 digits.
			//
			//			x = x - (erf(x) - y) ./ (2/sqrt(pi) * exp(-x.^2));
			//			x = x - (erf(x) - y) ./ (2/sqrt(pi) * exp(-x.^2));
			x = x - (Erf(x) - y) / (2 / Mathematics.Sqrt(Mathematics.PI) * Mathematics.Exp(-x*x)) ;
			x = x - (Erf(x) - y) / (2 / Mathematics.Sqrt(Mathematics.PI) * Mathematics.Exp(-x*x)) ;
			//
			//			% Exceptional cases.
			//
			//			k = find(y == -1);
			//			if ~isempty(k), x(k) = -inf*k; end
			//			k = find(y == 1);
			//			if ~isempty(k), x(k) = inf*k; end
			//			k = find(abs(y) > 1);
			//			if ~isempty(k), x(k) = NaN; end
			//
			//			x = reshape(x,siz);
			return x ;
		}
		/// <summary>
		/// Calculates the Inverse Error Function
		/// </summary>
		/// <returns>X such that Y = Erf(X)</returns>
		public static float ErfInv(float Y)
		{
			return (float)ErfInv((double)Y) ;
		}
		/// <summary>
		/// Calculates the Inverse Error Function
		/// </summary>
		/// <returns>X such that Y = Erf(X)</returns>
		public static Decimal ErfInv(Decimal Y)
		{
			Decimal y = Y ;
			Decimal x = -1 ;
			Decimal[] a = {0.886226899M, -1.645349621M, 0.914624893M, -0.140543331M} ;
			Decimal[] b = {-2.118377725M, 1.442710462M, -0.329097515M, 0.012229801M} ;
			Decimal[] c = {-1.970840454M, -1.624906493M, 3.429567803M, 1.641345311M} ;
			Decimal[] d = {3.543889200M, 1.637067800M} ;
			Decimal y0 = .7M ;
			if (Mathematics.Abs(y) <= y0)
			{
				Decimal z = y * y ;
				x = y * (((a[3] * z + a[2]) * z + a[1]) * z + a[0]) /
					((((b[3] * z + b[2]) * z + b[1]) * z + b[0]) * z + 1) ;
			}
			if (y0 < y && y < 1)
			{
				Decimal z = Calculus.Sqrt(-Calculus.Log((1M - y) / 2M)) ;
				x = (((c[3] * z + c[2]) * z + c[1]) * z + c[0]) / ((d[1] * z + d[0]) * z + 1) ;
			}
			if (-y0 > y && y > -1)
			{
				Decimal z = Calculus.Sqrt(-Calculus.Log((1M + y) / 2M)) ;
				x = -(((c[3] * z + c[2]) * z + c[1]) * z + c[0]) / ((d[1] * z + d[0]) * z + 1) ;
			}
			x = x - (Erf(x) - y) / (2 / Calculus.SqrtPi * Calculus.Exp(-x*x)) ;
			x = x - (Erf(x) - y) / (2 / Calculus.SqrtPi * Calculus.Exp(-x*x)) ;
			return x ;
		}
		#endregion
		#region Sum
		public static double Sum(double[] X)
		{
			double result = 0 ;
			for(int i = 0; i < X.Length; i++)
				result += X[i];
			return result ;
		}
		public static float Sum(float[] X)
		{
			return (float)Sum(ToDoubleArray(X)) ;
		}
		public static Decimal Sum(Decimal[] X)
		{
			Decimal result = 0M ;
			for(int i = 0; i < X.Length; i++)
				result += X[i] ;
			return result ;
		}
		#endregion

		#region Internal Tool Functions
		internal static double Fix(double arg)
		{
			int sign = arg < 0 ? -1 : 1 ;
			arg = Mathematics.Abs(arg) ;
			arg = Mathematics.Floor(arg) ;
			return arg * sign ;
		}
		internal static Decimal Fix(Decimal arg)
		{
			int sign = arg < 0 ? -1 : 1 ;
			arg = Mathematics.Abs(arg) ;
			arg = Calculus.Floor(arg) ;
			return arg * sign ;
		}
		internal static bool IsFinite(double number)
		{
			return number == double.NaN || number == double.PositiveInfinity ||
				number == double.NegativeInfinity ;
		}
		internal static double[] ToDoubleArray(float[] floats)
		{
			double[] result = new double[floats.Length] ;
			Array.Copy(floats, 0, result, 0, floats.Length) ;
			return result ;
		}
		internal static double[] Sort(double[] data)
		{
			// TODO: Improve Sort Algorithm
			double[] result = new double[data.Length] ;
			Array.Copy(data, 0, result, 0, data.Length) ;
			Array.Sort(result) ;
			return result ;
		}
		internal static float[] Sort(float[] data)
		{
			// TODO: Improve Sort Algorithm
			float[] result = new float[data.Length] ;
			Array.Copy(data, 0, result, 0, data.Length) ;
			Array.Sort(result) ;
			return result ;
		}
		internal static Decimal[] Sort(Decimal[] data)
		{
			// TODO: Improve Sort Algorithm
			Decimal[] result = new Decimal[data.Length] ;
			Array.Copy(data, 0, result, 0, data.Length) ;
			Array.Sort(result) ;
			return result ;
		}
		/// <summary>
		/// O(n log(n)) i-th elment search. O(n) will be implemented in future releases.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		internal static double Pick(double[] data, int i)
		{
			data = Sort(data) ;
			return data[i] ;
		}
		internal static float Pick(float[] data, int i)
		{
			data = Sort(data) ;
			return data[i] ;
		}
		internal static Decimal Pick(Decimal[] data, int i)
		{
			data = Sort(data) ;
			return data[i] ;
		}
		internal static SlopeInterceptPairD LinearFit(double[] Y, double[] X)
		{
			if (X.Length != Y.Length)
				throw new ArrayDimensionsException() ;
			double sumXY = .0 ;
			double sumX = .0 ;
			double sumY = .0 ;
			double sumX2 = .0 ;
			double sumY2 = .0 ;
			double n = X.Length ;
			for(int i = 0; i < Y.Length; i++)
			{
				double x = X[i] ;
				double y = Y[i] ;
				sumXY += x * y ;
				sumX += x ;
				sumY += y ;
				sumX2 += x * x ;
				sumY2 += y * y ;
			}
			SlopeInterceptPairD result = new SlopeInterceptPairD() ;
			double den = n * sumX2 - sumX * sumX ;
			result.Slope = (n * sumXY - sumX * sumY) / den ;
			result.Intercept = (sumY * sumX2 - sumX * sumXY) / den ;
			return result ;
		}
		internal static SlopeInterceptPairF LinearFit(float[] Y, float[]X)
		{
			if (X.Length != Y.Length)
				throw new ArrayDimensionsException() ;
			float sumXY = .0F ;
			float sumX = .0F ;
			float sumY = .0F ;
			float sumX2 = .0F ;
			float sumY2 = .0F ;
			float n = X.Length ;
			for(int i = 0; i < Y.Length; i++)
			{
				float x = X[i] ;
				float y = Y[i] ;
				sumXY += x * y ;
				sumX += x ;
				sumY += y ;
				sumX2 += x * x ;
				sumY2 += y * y ;
			}
			SlopeInterceptPairF result = new SlopeInterceptPairF() ;
			float den = n * sumX2 - sumX * sumX ;
			result.Slope = (n * sumXY - sumX * sumY) / den ;
			result.Intercept = (sumY * sumX2 - sumX * sumXY) / den ;
			return result ;		
		}
		internal static SlopeInterceptPairM LinearFit(Decimal[] Y, Decimal[] X)
		{
			if (X.Length != Y.Length)
				throw new ArrayDimensionsException() ;
			Decimal sumXY = .0M ;
			Decimal sumX = .0M ;
			Decimal sumY = .0M ;
			Decimal sumX2 = .0M ;
			Decimal sumY2 = .0M ;
			Decimal n = X.Length ;
			for(int i = 0; i < Y.Length; i++)
			{
				Decimal x = X[i] ;
				Decimal y = Y[i] ;
				sumXY += x * y ;
				sumX += x ;
				sumY += y ;
				sumX2 += x * x ;
				sumY2 += y * y ;
			}
			SlopeInterceptPairM result = new SlopeInterceptPairM() ;
			Decimal den = n * sumX2 - sumX * sumX ;
			result.Slope = (n * sumXY - sumX * sumY) / den ;
			result.Intercept = (sumY * sumX2 - sumX * sumXY) / den ;
			return result ;
		}
		internal static SlopeInterceptPairD CurveFit(double[] Y, double[] X)
		{
			double[] logY = new double[Y.Length] ;
			for(int i = 0; i < logY.Length; i ++)
			{
				logY[i] = Mathematics.Log(Y[i]) ;
			}
			SlopeInterceptPairD result = LinearFit(logY, X) ;
			result.Slope = Mathematics.Exp(result.Slope) ;
			result.Intercept = Mathematics.Exp(result.Intercept) ;
			return result ;
		}
		internal static SlopeInterceptPairF CurveFit(float[] Y, float[] X)
		{
			float[] logY = new float[Y.Length] ;
			for(int i = 0; i < logY.Length; i ++)
			{
				logY[i] = (float)Mathematics.Log(Y[i]) ;
			}
			SlopeInterceptPairF result = LinearFit(logY, X) ;
			result.Slope = (float)Mathematics.Exp(result.Slope) ;
			result.Intercept = (float)Mathematics.Exp(result.Intercept) ;
			return result ;
		}
		internal static SlopeInterceptPairM CurveFit(Decimal[] Y, Decimal[] X)
		{
			Decimal[] logY = new Decimal[Y.Length] ;
			for(int i = 0; i < logY.Length; i ++)
			{
				logY[i] = Calculus.Log(Y[i]) ;
			}
			SlopeInterceptPairM result = LinearFit(logY, X) ;
			result.Slope = Calculus.Exp(result.Slope) ;
			result.Intercept = Calculus.Exp(result.Intercept) ;
			return result ;
		}
		#endregion
	}
	#region Package Tools
	public class ArrayDimensionsException : Exception
	{
		public ArrayDimensionsException() : base()
		{
		
		}
		public ArrayDimensionsException(string msg) : base(msg)
		{
		
		}
		public ArrayDimensionsException(string msg, Exception inner) : base(msg, inner)
		{
		
		}
	}
	public struct SlopeInterceptPairD
	{
		public double Slope ;
		public double Intercept ;
	}
	public struct SlopeInterceptPairF
	{
		public float Slope ;
		public float Intercept ;
	}
	public struct SlopeInterceptPairM
	{
		public Decimal Slope ;
		public Decimal Intercept ;
	}
	public struct MultipleRegressionResultsD
	{
		public double[] coefficients ;
		public double[] se ;
		public double Seb
		{
			get
			{
				return se[0] ;
			}
			set
			{
				se[0] = value ;
			}
		}
		public double r2 ;
		public double sey ;
		public double F ;
		public int df ;
		public double ssreg ;
		public double ssresid ;
		public MatrixD CovarianceMatrix ;
		public MatrixD CoefficientsMatrix ;
		/// <summary>
		/// Returns the Bi coeffitient
		/// </summary>
		public double this[int i]
		{
			get
			{
				return coefficients[i] ;
			}
			set
			{
				coefficients[i] = value ;
			}
		}
		public double Intercept
		{
			get
			{
				return this[0] ;
			}
			set
			{
				this[0] = value ;
			}
		}
	}
	public struct MultipleRegressionResultsF
	{
		public float[] coefficients ;
		public float[] se ;
		public float Seb
		{
			get
			{
				return se[0] ;
			}
			set
			{
				se[0] = value ;
			}
		}
		public float r2 ;
		public float sey ;
		public float F ;
		public int df ;
		public float ssreg ;
		public float ssresid ;
		public MatrixF CovarianceMatrix ;
		public MatrixF CoefficientsMatrix ;
		/// <summary>
		/// Returns the Bi coeffitient
		/// </summary>
		public float this[int i]
		{
			get
			{
				return coefficients[i] ;
			}
			set
			{
				coefficients[i] = value ;
			}
		}
		public float Intercept
		{
			get
			{
				return this[0] ;
			}
			set
			{
				this[0] = value ;
			}
		}
	}
	public struct MultipleRegressionResultsM
	{
		public Decimal[] coefficients ;
		public Decimal[] se ;
		public Decimal Seb
		{
			get
			{
				return se[0] ;
			}
			set
			{
				se[0] = value ;
			}
		}
		public Decimal r2 ;
		public Decimal sey ;
		public Decimal F ;
		public int df ;
		public Decimal ssreg ;
		public Decimal ssresid ;
		public MatrixM CovarianceMatrix ;
		public MatrixM CoefficientsMatrix ;
		/// <summary>
		/// Returns the Bi coeffitient
		/// </summary>
		public Decimal this[int i]
		{
			get
			{
				return coefficients[i] ;
			}
			set
			{
				coefficients[i] = value ;
			}
		}
		public Decimal Intercept
		{
			get
			{
				return this[0] ;
			}
			set
			{
				this[0] = value ;
			}
		}
	}
	#endregion
}
