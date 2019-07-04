using System;

namespace Qasar.Mva
{
	/// <summary>
	/// Based on AS274 "Least-squares routines to supplement those of Gentleman"
	/// by Alan Miller.
	/// A Fortran algorithm available from http://lib.stat.cmu.edu/apstat/274
	/// </summary>
	public class LeastSquares
	{	
		public LeastSquares(){}
		//fits y=ax^2+bx+c
		public void Execute(int n, double [] x, double [] y, 
			ref double a, ref double b, ref double c)
		{
			double P = 0;
			double Q = 0;
			double R = 0;
			double S = 0;
			double T = 0;
			double U = 0;
			double V = 0;
			double W = 0;
			for (int i = 0; i < n; i+=1)
			{
				P = P + x[i];
				Q = Q + Math.Pow(x[i],2);
				R = R + Math.Pow(x[i],3);
				S = S + Math.Pow(x[i],4);
				T = T + y[i];
				U = U + x[i]*y[i];
				V = V + Math.Pow(x[i],2)*y[i];
			}
			W=n*Q*S+2*P*Q*R-Math.Pow(Q,3)-Math.Pow(P,2)*S-n*Math.Pow(R,2);
			a=(n*Q*V+P*R*T+P*Q*U-Math.Pow(Q,2)*T-Math.Pow(P,2)*V-n*R*U)/W;
			b=(n*S*U+P*Q*V+Q*R*T-Math.Pow(Q,2)*U-P*S*T-n*R*V)/W;
			c=(Q*S*T+Q*R*U+P*R*V-Math.Pow(Q,2)*V-P*S*U-Math.Pow(R,2)*T)/W;
		}
	}
}
