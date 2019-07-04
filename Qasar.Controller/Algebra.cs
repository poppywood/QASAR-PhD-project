using System;
using System.Text;
using System.Globalization;
using Mathematics = System.Math ;

namespace Qasar.Controller
{
	/// <summary>
	/// Summary description for Algebra.
	/// </summary>
	public class MatrixD
	{
		#region Protected Fields
		protected double[,] coefficients ;
		protected int n ; // defines an n rows matrix
		protected int m ; // defines an m cols matrix
		#endregion

		#region Contruction
		public MatrixD(double[,] coefficients)
		{
			if (coefficients == null)
				throw new ArgumentNullException() ;
			if (coefficients.GetLength(0) == 0 || coefficients.GetLength(1) == 0)
				throw new ArgumentException() ;
			this.coefficients = coefficients ;
			this.n = coefficients.GetLength(0) ;
			this.m = coefficients.GetLength(1) ;
		}
		public MatrixD(int n, int m) :
			this(new double[n, m])
		{
		
		}
		#endregion

		#region Static Fields and Initialization
		static readonly NumberFormatInfo DefaultFloatFormat = new NumberFormatInfo() ;
		static MatrixD()
		{
			DefaultFloatFormat.NumberDecimalDigits = 2 ;
			DefaultFloatFormat.PositiveSign = "+" ;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the 1 base refered element in matrix.
		/// </summary>
		public double this[int rowIndex, int colIndex]
		{
			get
			{
				return coefficients[rowIndex - 1, colIndex - 1] ;
			}
			set
			{
				coefficients[rowIndex - 1, colIndex - 1] = value ;
			}
		}
		/// <summary>
		/// Gets the Row count.
		/// </summary>
		public int RowCount
		{
			get
			{
				return n ;
			}
		}
		/// <summary>
		/// Gets the Columns count.
		/// </summary>
		public int ColCount
		{
			get
			{
				return m ;
			}
		}		
		#endregion
	
		#region Object Overrides and new ToString definitions
		public override string ToString()
		{
			return ToString(DefaultFloatFormat) ;
		}
		public virtual string ToString(NumberFormatInfo numberFormat)
		{
			if (m == 0 || n == 0)
				return "" ;
			StringBuilder result = new StringBuilder("") ;
			for(int i = 0; i < n; i++)
			{
				result.Append("|") ;
				for(int j = 0; j < m; j++)
				{
					result.Append(coefficients[i, j].ToString("N", numberFormat) + "\t") ;
				}
				result.Append("|") ;
				result.Append("\r\n") ;
			}
			result.Remove(result.Length - 2, 1) ;
			return result.ToString() ;
		}
		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is MatrixD))
				return false ;
			MatrixD matrix = (MatrixD)obj ;
			return n == matrix.n && m == matrix.m &&
				coefficients.Equals(matrix.coefficients) ;
		}
		public override int GetHashCode()
		{
			return n.GetHashCode() + m.GetHashCode() +
				coefficients.GetHashCode() ;
		}

		#endregion

		#region Public Methods/Basic Matrix Operations
		/// <summary>
		/// Returns an exact copy of current matrix.
		/// </summary>
		/// <remarks>This method does an array copy, a whole new memory space will be 
		/// used for this method's result. Be carefull.</remarks>
		public MatrixD CarbonCopy()
		{
			double[,] cc = new double[n, m] ;
			Array.Copy(coefficients, 0, cc, 0, coefficients.Length) ;
			return new MatrixD(cc) ;
		}
		/// <summary>
		/// Multiplies current matrix by an scalar.
		/// </summary>
		/// <param name="value">Scalara value to multiplicate by</param>
		public void Multiply(double value)
		{
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					coefficients[i, j] *= value ;
		}
		/// <summary>
		/// Adds to matrices and places the result on current matrix.
		/// </summary>
		/// <param name="matrix">The other matrix to add</param>
		public void Add(MatrixD matrix)
		{
			if (matrix.n != n || matrix.m != m)
				throw new ArgumentException("Can't add") ;
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					coefficients[i, j] += matrix[i, j] ;
		}
		/// <summary>
		/// Substracts parameter matrix from current matrix.
		/// </summary>
		/// <param name="matrix">matrix to substract from current</param>
		public void Sub(MatrixD matrix)
		{
			if (matrix.n != n || matrix.m != m)
				throw new ArgumentException("Can't add") ;
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					coefficients[i, j] -= matrix[i, j] ;			
		}
		/// <summary>
		/// Pre-Multiplies parameter by this and places result on current matrix.
		/// </summary>
		/// <param name="matrix">Matrix to multiply by this</param>
		public void PreMultiply(MatrixD matrix)
		{
			MatrixD result = new MatrixD(matrix.n, m) ;
			Multiply(matrix, this, result) ;
			this.coefficients = result.coefficients ;
			this.n = result.n ;
		}
		/// <summary>
		/// Pre-Multiplies this by parameterand places result on current matrix.
		/// </summary>
		/// <param name="matrix">Matrix to multiply by this</param>
		public void PostMultiply(MatrixD matrix)
		{
			MatrixD result = new MatrixD(n, matrix.m) ;
			Multiply(this, matrix, result) ;
			this.coefficients = result.coefficients ;
			this.m = result.m ;
		}
		/// <summary>
		/// Calculates the determinant of current matrix. Modifies current matrix to it's factorized form PLU.
		/// </summary>
		/// <remarks>This is a high cost method.</remarks>
		public double Determinant()
		{
			if (n != m)
				throw new InvalidOperationException("Cannot calculate determinant of a non squared matrix") ;
			int[] Pivot = new int[n] ;
			double det = FactorizePivot(coefficients, Pivot) ;
			for(int i = 0; i < n; i++)
				det *= coefficients[i, i] ;
			return det ;
		}
		/// <summary>
		/// Calculates this matrix transposed.
		/// </summary>
		/// <returns>This matrix transposed</returns>
		/// <remarks>This method doesn't modify current matrix.</remarks>
		public MatrixD Transposed()
		{
			double[,] result = new double[m, n] ;
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					result[j, i] = coefficients[i, j] ;
			return new MatrixD(result) ;
		}
		/// <summary>
		/// Factorizes current matrix and returns it's inverse.
		/// </summary>
		/// <remarks>As in Determinant, after using this method current matrix will be in it's
		/// factorized (PLU) form</remarks>
		public MatrixD Inverse()
		{
			if (n != m)
				throw new InvalidOperationException("Matrix should be NxN") ;
			MatrixD inv = new MatrixD(n, n) ;
			Inverse(coefficients, inv.coefficients, new int[n]) ;
			return inv ;
		}
		#endregion

		#region Static Methods. Most of these where translated from Fortran
		/// <summary>
		/// Returns the order n Identity Matrix.
		/// </summary>
		/// <param name="n">order of the matrix to be created.</param>
		public static MatrixD Identity(int n)
		{
			MatrixD result = new MatrixD(n, n) ;
			for(int i = 0; i < n; i++)
				result.coefficients[i, i] = 1 ;
			return result ;
		}
		/// <summary>
		/// Multiplies m1 by m2 and places result on result.
		/// </summary>
		public static void Multiply(MatrixD m1, MatrixD m2, MatrixD result)
		{
			if (m1.m != m2.n || result.n != m1.n || result.m != m2.m)
				throw new ArgumentException("Can't multiply") ;
			double[,] rcoef = result.coefficients,
				m1coef = m1.coefficients,
				m2coef = m2.coefficients ;
			for(int i = 0; i < m1.n; i++)
				for(int j = 0; j < m2.m; j++)
					for(int e = 0; e < m1.m; e++)
						rcoef[i, j] += m1coef[i, e] * m2coef[e, j] ;
		}
		/// <summary>
		/// Calculates the Triangular Factorization of a Matrix using Choleski Factorization. Use this factorization
		/// when matrix is a symmetric positive definite matrix.
		/// References:
		/// 1) Elementary Numerical Analisys, An Algorithmic Aproach; 3rd Edition.
		/// </summary>
		/// <param name="W">An squared array representing the work matrix, also the resulting</param>
		/// <param name="Pivot">An n-vetor that will output the permutations aplied to rows in W</param>
		/// <returns>the sign of determinant, if 0 then W matrix is singular</returns>
		/// <remarks>This is not implemented in this version</remarks>
		public static int FactorizeCholeski(double[,] W, int[] Pivot)
		{
			return -1 ;
		}
		/// <summary>
		/// Calculates the Triangular Factorization of a Matrix using Scaled Partial Pivot.
		/// Original matrix A will be factorized to PLU where P is a permutations vector, L is
		/// a lower Triangular Matrix and U an upper Triangular Matrix.
		/// References:
		/// 1) Elementary Numerical Analisys, An Algorithmic Aproach; 3rd Edition.
		/// </summary>
		/// <param name="W">An squared array representing the work matrix, also the resulting</param>
		/// <param name="Pivot">An n-vetor that will output the permutations aplied to rows in W</param>
		/// <returns>the sign of determinant, if 0 then W matrix is singular</returns>
		public static int FactorizePivot(double[,] W, int[] Pivot)
		{
			#region Parameter Check & Locals Init
			if (W.GetLength(0) != W.GetLength(1) ||
				Pivot.Length != W.GetLength(0))
				throw new ArgumentException() ;
			int Size = W.GetLength(0) ;
			int N = Size - 1 ;
			int i, j, k ;
			int Flag = 1 ;
			double[] D = new double[Size] ;
			#endregion

			#region Fortran Algorithm
			#region Initialize Pivot, D
			for(i = 0; i <= N; i++)
			{
				Pivot[i] = i ;
				double RowMax = .0 ;
				for(j = 0; j <= N; j++)
				{
					RowMax = Mathematics.Max(Mathematics.Abs(W[i, j]), RowMax) ;
					if (RowMax == .0)
					{
						Flag = 0 ;
						RowMax = 1.0 ;
					}
				}
				D[i] = RowMax ;
			}
			#endregion
			if (Size <= 1)
				return 0 ;
			#region Factorization
			for(k = 0; k <= N - 1; k++)
			{
				#region Determine Pivot Row, the Row Istar
				double ColMax = Mathematics.Abs(W[k, k]) / D[k] ;
				int Istar = k ;
				for(i = k + 1; i <= N; i++)
				{
					double Awikod = Mathematics.Abs(W[i,k]) / D[k] ;
					if (Awikod > ColMax)
					{
						ColMax = Awikod ;
						Istar = i ;
					}
				}
				if (ColMax == .0)
					Flag = 0 ;
				else
				{
					if (Istar > k)
					{
						#region Make k the pivot Row by interchanging it with the chosen row Istar
						Flag = -Flag ;
						i = Pivot[Istar] ;
						Pivot[Istar] = Pivot[k] ;
						Pivot[k] = i ;
						double temp = D[Istar] ;
						D[Istar] = D[k] ;
						D[k] = temp ;
						for(j = 0; j <= N; j++)
						{
							temp = W[Istar, j] ;
							W[Istar, j] = W[k, j] ;
							W[k, j] = temp ;
						}
						#endregion
					}
					#region Eliminate X(k) From Rows k+1, .., N
					for(i = k + 1; i <= N; i++)
					{
						double ratio = W[i, k] /= W[k, k] ;
						for(j = k + 1; j <= N; j++)
						{
							W[i, j] -= ratio * W[k, j] ;
						}
					}
					#endregion
				}
				#endregion
			}
			#endregion
			if (W[N, N] == 0)
				Flag = 0 ;
			#endregion

			#region Return
			return Flag ;
			#endregion
		}
		/// <summary>
		/// Solves an equation system using Forward and Back Substitution Algorithm. Work Matrix (W) must be
		/// factorized.
		/// </summary>
		/// <param name="W">Factorized squared array representing the work Matrix</param>
		/// <param name="Pivot">An n-vector representing the permutations done when factorizing W</param>
		/// <param name="B">Independent terms vector</param>
		/// <param name="X">Result</param>
		public static void Subst(double[,] W, int[] Pivot, double[] B, double[] X)
		{
			#region Parameter Check and Initilization
			if (W.GetLength(0) != W.GetLength(1))
				throw new ArgumentException("Matrix must be NxN") ;
			int N = W.GetLength(0) - 1;
			int i, j ;
			double sum ;
			#endregion

			#region Fortran algorithm
			if (N <= 1)
			{
				X[0] = B[0] / W[0, 0] ;
				return ;
			}
			int Ip = Pivot[0] ;
			X[0] = B[Ip] ;
			for(i = 1; i <= N; i++)
			{
				sum = .0 ;
				for(j = 0; j <= i - 1; j++)
					sum += W[i, j] * X[j] ;
				Ip = Pivot[i] ;
				X[i] = B[Ip] - sum ;
			}
			X[N] = X[N] / W[N, N] ;
			for(i = N - 1; i >= 0; i--)
			{
				sum = .0 ;
				for(j = i + 1; j <= N; j++)
				{
					sum += W[i, j] * X[j] ;
				}
				X[i] = (X[i] - sum) / W[i, i] ;
			}
			#endregion
		}
		/// <summary>
		/// Inverses a matrix using W as the work matrix.
		/// </summary>
		/// <param name="W"></param>
		/// <param name="InvW"></param>
		/// <remarks>Original data in W will be modified, can be recoverd but it's complicated.</remarks>
		public static void Inverse(double[,] W, double[,] InvW, int[] Pivot)
		{
			#region Initialization
			int N = W.GetLength(0) ;
			//			int[] Pivot = new int[N] ;
			double[] B = new double[N] ;
			double[] X = new double[N] ;
			#endregion

			#region Fortran Algorithm
			int Flag = FactorizePivot(W, Pivot) ;
			if (Flag == 0)
				throw new ArgumentException("Matrix is singular, cannot calculate inverse") ;
			int IBeg = 0 ;
			for(int j = 0; j < N; j++)
			{
				B[j] = 1.0 ;
				Subst(W, Pivot, B, X) ;
				B[j] = .0 ;
				for(int k = 0; k < N; k++)
					InvW[k, IBeg] = X[k] ;
				IBeg ++ ;
			}
			#endregion
		}
		/// <summary>
		/// Solves a linear equaltion system of the form AX=B, using the Gauss method.
		/// </summary>
		/// <param name="A">Coefficients Matrix</param>
		/// <param name="B">Independet Terms Matrix</param>
		/// <returns>Values for x that satifies AX=B</returns>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters are not 
		/// correct. B shoul be an N vector and A an NxN Matrix</exception>
		/// <remarks>This method is obsolete. </remarks>
		public static void Solve(double[,] A, double[] B, double[] X)
		{
			#region Local Initialization and Parameter Check
			int N = B.Length ;
			if (A.GetLength(0) != A.GetLength(1) ||
				N != A.GetLength(0))
				throw new ArrayDimensionsException() ;
			double[,] WorkMatrix = new double[N, N] ;
			Array.Copy(A, 0, WorkMatrix, 0, A.Length) ;
			A = WorkMatrix ;
			#endregion

			#region Triangulation
			for(int i = 0; i < N - 1; i++)
			{
				double a = A[i, i] ;
				for(int j = i + 1; j < N; j++)
				{
					double b = A[j, i] ;
					double pivot = - b / a ;
					A[j, i] = 0 ;
					for(int jj = i + 1; jj < N; jj++)
						A[j, jj] += pivot * A[i, jj] ;
					B[j] += pivot * B[i] ;
				}
			}
			#endregion

			#region Create Result
			//			X[N - 1] = B[N - 1] / A[N - 1, N - 1] ;
			for(int i = N - 1; i >= 0; i--)
			{
				double sum = B[i] ;
				for(int j = i + 1; j < N; j ++)
					sum -= X[j] * A[i, j] ;
				X[i] = sum / A[i, i] ;
			}
			#endregion
		}
		#endregion

		#region Operators
		public static MatrixD operator+(MatrixD m1, MatrixD m2)
		{
			MatrixD result = m1.CarbonCopy() ;
			result.Add(m2) ;
			return result ;
		}
		public static MatrixD operator-(MatrixD m1, MatrixD m2)
		{
			MatrixD result = m1.CarbonCopy() ;
			result.Sub(m2) ;
			return result ;
		}
		public static MatrixD operator*(MatrixD m1, MatrixD m2)
		{
			MatrixD result = new MatrixD(m1.RowCount, m2.ColCount) ;
			MatrixD.Multiply(m1, m2, result) ;
			return result ;
		}
		public static MatrixD operator*(MatrixD m, double a)
		{
			MatrixD result = m.CarbonCopy() ;
			result.Multiply(a) ;
			return result ;
		}
		public static MatrixD operator*(double a, MatrixD m)
		{
			MatrixD result = m.CarbonCopy() ;
			result.Multiply(a) ;
			return result ;
		}
		/// <summary>
		/// Transpose operand matrix.
		/// </summary>
		public static MatrixD operator!(MatrixD operand)
		{
			return operand.Transposed() ;
		}
		/// <summary>
		/// Returns operand.Inverse(). This one modifies operand to it's factorized(PLU) form.
		/// </summary>
		public static MatrixD operator~(MatrixD operand)
		{
			return operand.Inverse() ;
		}
		#endregion
	}
	public class MatrixF
	{
		#region Protected Fields
		protected float[,] coefficients ;
		protected int n ; // defines an n rows matrix
		protected int m ; // defines an m cols matrix
		#endregion

		#region Contruction
		public MatrixF(float[,] coefficients)
		{
			if (coefficients == null)
				throw new ArgumentNullException() ;
			if (coefficients.GetLength(0) == 0 || coefficients.GetLength(1) == 0)
				throw new ArgumentException() ;
			this.coefficients = coefficients ;
			this.n = coefficients.GetLength(0) ;
			this.m = coefficients.GetLength(1) ;
		}
		public MatrixF(int n, int m) :
			this(new float[n, m])
		{
		
		}
		#endregion

		#region Static Fields and Initialization
		static readonly NumberFormatInfo DefaultFloatFormat = new NumberFormatInfo() ;
		static MatrixF()
		{
			DefaultFloatFormat.NumberDecimalDigits = 2 ;
			DefaultFloatFormat.PositiveSign = "+" ;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the 1 base refered element in matrix.
		/// </summary>
		public float this[int rowIndex, int colIndex]
		{
			get
			{
				return coefficients[rowIndex - 1, colIndex - 1] ;
			}
			set
			{
				coefficients[rowIndex - 1, colIndex - 1] = value ;
			}
		}
		/// <summary>
		/// Gets the Row count.
		/// </summary>
		public int RowCount
		{
			get
			{
				return n ;
			}
		}
		/// <summary>
		/// Gets the Columns count.
		/// </summary>
		public int ColCount
		{
			get
			{
				return m ;
			}
		}		
		#endregion
	
		#region Object Overrides and new ToString definitions
		public override string ToString()
		{
			return ToString(DefaultFloatFormat) ;
		}
		public virtual string ToString(NumberFormatInfo numberFormat)
		{
			if (m == 0 || n == 0)
				return "" ;
			StringBuilder result = new StringBuilder("") ;
			for(int i = 0; i < n; i++)
			{
				result.Append("|") ;
				for(int j = 0; j < m; j++)
				{
					result.Append(coefficients[i, j].ToString("N", numberFormat) + "\t") ;
				}
				result.Append("|") ;
				result.Append("\r\n") ;
			}
			result.Remove(result.Length - 2, 1) ;
			return result.ToString() ;
		}
		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is MatrixF))
				return false ;
			MatrixF matrix = (MatrixF)obj ;
			return n == matrix.n && m == matrix.m &&
				coefficients.Equals(matrix.coefficients) ;
		}
		public override int GetHashCode()
		{
			return n.GetHashCode() + m.GetHashCode() +
				coefficients.GetHashCode() ;
		}

		#endregion

		#region Public Methods/Basic Matrix Operations
		/// <summary>
		/// Returns an exact copy of current matrix.
		/// </summary>
		/// <remarks>This method does an array copy, a whole new memory space will be 
		/// used for this method's result. Be carefull.</remarks>
		public MatrixF CarbonCopy()
		{
			float[,] cc = new float[n, m] ;
			Array.Copy(coefficients, 0, cc, 0, coefficients.Length) ;
			return new MatrixF(cc) ;
		}
		/// <summary>
		/// Multiplies current matrix by an scalar.
		/// </summary>
		/// <param name="value">Scalara value to multiplicate by</param>
		public void Multiply(float value)
		{
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					coefficients[i, j] *= value ;
		}
		/// <summary>
		/// Adds to matrices and places the result on current matrix.
		/// </summary>
		/// <param name="matrix">The other matrix to add</param>
		public void Add(MatrixF matrix)
		{
			if (matrix.n != n || matrix.m != m)
				throw new ArgumentException("Can't add") ;
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					coefficients[i, j] += matrix[i, j] ;
		}
		/// <summary>
		/// Substracts parameter matrix from current matrix.
		/// </summary>
		/// <param name="matrix">matrix to substract from current</param>
		public void Sub(MatrixF matrix)
		{
			if (matrix.n != n || matrix.m != m)
				throw new ArgumentException("Can't add") ;
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					coefficients[i, j] -= matrix[i, j] ;			
		}
		/// <summary>
		/// Pre-Multiplies parameter by this and places result on current matrix.
		/// </summary>
		/// <param name="matrix">Matrix to multiply by this</param>
		public void PreMultiply(MatrixF matrix)
		{
			MatrixF result = new MatrixF(matrix.n, m) ;
			Multiply(matrix, this, result) ;
			this.coefficients = result.coefficients ;
			this.n = result.n ;
		}
		/// <summary>
		/// Pre-Multiplies this by parameterand places result on current matrix.
		/// </summary>
		/// <param name="matrix">Matrix to multiply by this</param>
		public void PostMultiply(MatrixF matrix)
		{
			MatrixF result = new MatrixF(n, matrix.m) ;
			Multiply(this, matrix, result) ;
			this.coefficients = result.coefficients ;
			this.m = result.m ;
		}
		/// <summary>
		/// Calculates the determinant of current matrix. Modifies current matrix to it's factorized form PLU.
		/// </summary>
		/// <remarks>This is a high cost method.</remarks>
		public float Determinant()
		{
			if (n != m)
				throw new InvalidOperationException("Cannot calculate determinant of a non squared matrix") ;
			int[] Pivot = new int[n] ;
			float det = FactorizePivot(coefficients, Pivot) ;
			for(int i = 0; i < n; i++)
				det *= coefficients[i, i] ;
			return det ;
		}
		/// <summary>
		/// Calculates this matrix transposed.
		/// </summary>
		/// <returns>This matrix transposed</returns>
		/// <remarks>This method doesn't modify current matrix.</remarks>
		public MatrixF Transposed()
		{
			float[,] result = new float[m, n] ;
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					result[j, i] = coefficients[i, j] ;
			return new MatrixF(result) ;
		}
		/// <summary>
		/// Factorizes current matrix and returns it's inverse.
		/// </summary>
		/// <remarks>As in Determinant, after using this method current matrix will be in it's
		/// factorized (PLU) form</remarks>
		public MatrixF Inverse()
		{
			if (n != m)
				throw new InvalidOperationException("Matrix should be NxN") ;
			MatrixF inv = new MatrixF(n, n) ;
			Inverse(coefficients, inv.coefficients, new int[n]) ;
			return inv ;
		}
		#endregion

		#region Static Methods. Most of these where translated from Fortran
		/// <summary>
		/// Returns the order n Identity Matrix.
		/// </summary>
		/// <param name="n">order of the matrix to be created.</param>
		public static MatrixF Identity(int n)
		{
			MatrixF result = new MatrixF(n, n) ;
			for(int i = 0; i < n; i++)
				result.coefficients[i, i] = 1 ;
			return result ;
		}
		/// <summary>
		/// Multiplies m1 by m2 and places result on result.
		/// </summary>
		public static void Multiply(MatrixF m1, MatrixF m2, MatrixF result)
		{
			if (m1.m != m2.n || result.n != m1.n || result.m != m2.m)
				throw new ArgumentException("Can't multiply") ;
			float[,] rcoef = result.coefficients,
				m1coef = m1.coefficients,
				m2coef = m2.coefficients ;
			for(int i = 0; i < m1.n; i++)
				for(int j = 0; j < m2.m; j++)
					for(int e = 0; e < m1.m; e++)
						rcoef[i, j] += m1coef[i, e] * m2coef[e, j] ;
		}
		/// <summary>
		/// Calculates the Triangular Factorization of a Matrix using Choleski Factorization. Use this factorization
		/// when matrix is a symmetric positive definite matrix.
		/// References:
		/// 1) Elementary Numerical Analisys, An Algorithmic Aproach; 3rd Edition.
		/// </summary>
		/// <param name="W">An squared array representing the work matrix, also the resulting</param>
		/// <param name="Pivot">An n-vetor that will output the permutations aplied to rows in W</param>
		/// <returns>the sign of determinant, if 0 then W matrix is singular</returns>
		/// <remarks>This is not implemented in this version</remarks>
		public static int FactorizeCholeski(float[,] W, int[] Pivot)
		{
			return -1 ;
		}
		/// <summary>
		/// Calculates the Triangular Factorization of a Matrix using Scaled Partial Pivot.
		/// Original matrix A will be factorized to PLU where P is a permutations vector, L is
		/// a lower Triangular Matrix and U an upper Triangular Matrix.
		/// References:
		/// 1) Elementary Numerical Analisys, An Algorithmic Aproach; 3rd Edition.
		/// </summary>
		/// <param name="W">An squared array representing the work matrix, also the resulting</param>
		/// <param name="Pivot">An n-vetor that will output the permutations aplied to rows in W</param>
		/// <returns>the sign of determinant, if 0 then W matrix is singular</returns>
		public static int FactorizePivot(float[,] W, int[] Pivot)
		{
			#region Parameter Check & Locals Init
			if (W.GetLength(0) != W.GetLength(1) ||
				Pivot.Length != W.GetLength(0))
				throw new ArgumentException() ;
			int Size = W.GetLength(0) ;
			int N = Size - 1 ;
			int i, j, k ;
			int Flag = 1 ;
			float[] D = new float[Size] ;
			#endregion

			#region Fortran Algorithm
			#region Initialize Pivot, D
			for(i = 0; i <= N; i++)
			{
				Pivot[i] = i ;
				float RowMax = 0F ;
				for(j = 0; j <= N; j++)
				{
					RowMax = Mathematics.Max(Mathematics.Abs(W[i, j]), RowMax) ;
					if (RowMax == 0F)
					{
						Flag = 0 ;
						RowMax = 1F ;
					}
				}
				D[i] = RowMax ;
			}
			#endregion
			if (Size <= 1)
				return 0 ;
			#region Factorization
			for(k = 0; k <= N - 1; k++)
			{
				#region Determine Pivot Row, the Row Istar
				float ColMax = Mathematics.Abs(W[k, k]) / D[k] ;
				int Istar = k ;
				for(i = k + 1; i <= N; i++)
				{
					float Awikod = Mathematics.Abs(W[i,k]) / D[k] ;
					if (Awikod > ColMax)
					{
						ColMax = Awikod ;
						Istar = i ;
					}
				}
				if (ColMax == 0F)
					Flag = 0 ;
				else
				{
					if (Istar > k)
					{
						#region Make k the pivot Row by interchanging it with the chosen row Istar
						Flag = -Flag ;
						i = Pivot[Istar] ;
						Pivot[Istar] = Pivot[k] ;
						Pivot[k] = i ;
						float temp = D[Istar] ;
						D[Istar] = D[k] ;
						D[k] = temp ;
						for(j = 0; j <= N; j++)
						{
							temp = W[Istar, j] ;
							W[Istar, j] = W[k, j] ;
							W[k, j] = temp ;
						}
						#endregion
					}
					#region Eliminate X(k) From Rows k+1, .., N
					for(i = k + 1; i <= N; i++)
					{
						float ratio = W[i, k] /= W[k, k] ;
						for(j = k + 1; j <= N; j++)
						{
							W[i, j] -= ratio * W[k, j] ;
						}
					}
					#endregion
				}
				#endregion
			}
			#endregion
			if (W[N, N] == 0)
				Flag = 0 ;
			#endregion

			#region Return
			return Flag ;
			#endregion
		}
		/// <summary>
		/// Solves an equation system using Forward and Back Substitution Algorithm. Work Matrix (W) must be
		/// factorized.
		/// </summary>
		/// <param name="W">Factorized squared array representing the work Matrix</param>
		/// <param name="Pivot">An n-vector representing the permutations done when factorizing W</param>
		/// <param name="B">Independent terms vector</param>
		/// <param name="X">Result</param>
		public static void Subst(float[,] W, int[] Pivot, float[] B, float[] X)
		{
			#region Parameter Check and Initilization
			if (W.GetLength(0) != W.GetLength(1))
				throw new ArgumentException("Matrix must be NxN") ;
			int N = W.GetLength(0) - 1;
			int i, j ;
			float sum ;
			#endregion

			#region Fortran algorithm
			if (N <= 1)
			{
				X[0] = B[0] / W[0, 0] ;
				return ;
			}
			int Ip = Pivot[0] ;
			X[0] = B[Ip] ;
			for(i = 1; i <= N; i++)
			{
				sum = 0F ;
				for(j = 0; j <= i - 1; j++)
					sum += W[i, j] * X[j] ;
				Ip = Pivot[i] ;
				X[i] = B[Ip] - sum ;
			}
			X[N] = X[N] / W[N, N] ;
			for(i = N - 1; i >= 0; i--)
			{
				sum = 0F ;
				for(j = i + 1; j <= N; j++)
				{
					sum += W[i, j] * X[j] ;
				}
				X[i] = (X[i] - sum) / W[i, i] ;
			}
			#endregion
		}
		/// <summary>
		/// Inverses a matrix using W as the work matrix.
		/// </summary>
		/// <param name="W"></param>
		/// <param name="InvW"></param>
		/// <remarks>Original data in W will be modified, can be recoverd but it's complicated.</remarks>
		public static void Inverse(float[,] W, float[,] InvW, int[] Pivot)
		{
			#region Initialization
			int N = W.GetLength(0) ;
			//			int[] Pivot = new int[N] ;
			float[] B = new float[N] ;
			float[] X = new float[N] ;
			#endregion

			#region Fortran Algorithm
			int Flag = FactorizePivot(W, Pivot) ;
			if (Flag == 0)
				throw new ArgumentException("Matrix is singular, cannot calculate inverse") ;
			int IBeg = 0 ;
			for(int j = 0; j < N; j++)
			{
				B[j] = 1F ;
				Subst(W, Pivot, B, X) ;
				B[j] = 0F ;
				for(int k = 0; k < N; k++)
					InvW[k, IBeg] = X[k] ;
				IBeg ++ ;
			}
			#endregion
		}
		/// <summary>
		/// Solves a linear equaltion system of the form AX=B, using the Gauss method.
		/// </summary>
		/// <param name="A">Coefficients Matrix</param>
		/// <param name="B">Independet Terms Matrix</param>
		/// <returns>Values for x that satifies AX=B</returns>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters are not 
		/// correct. B shoul be an N vector and A an NxN Matrix</exception>
		/// <remarks>This method is obsolete. </remarks>
		public static void Solve(float[,] A, float[] B, float[] X)
		{
			#region Local Initialization and Parameter Check
			int N = B.Length ;
			if (A.GetLength(0) != A.GetLength(1) ||
				N != A.GetLength(0))
				throw new ArrayDimensionsException() ;
			float[,] WorkMatrix = new float[N, N] ;
			Array.Copy(A, 0, WorkMatrix, 0, A.Length) ;
			A = WorkMatrix ;
			#endregion

			#region Triangulation
			for(int i = 0; i < N - 1; i++)
			{
				float a = A[i, i] ;
				for(int j = i + 1; j < N; j++)
				{
					float b = A[j, i] ;
					float pivot = - b / a ;
					A[j, i] = 0 ;
					for(int jj = i + 1; jj < N; jj++)
						A[j, jj] += pivot * A[i, jj] ;
					B[j] += pivot * B[i] ;
				}
			}
			#endregion

			#region Create Result
			//			X[N - 1] = B[N - 1] / A[N - 1, N - 1] ;
			for(int i = N - 1; i >= 0; i--)
			{
				float sum = B[i] ;
				for(int j = i + 1; j < N; j ++)
					sum -= X[j] * A[i, j] ;
				X[i] = sum / A[i, i] ;
			}
			#endregion
		}
		#endregion

		#region Operators
		public static MatrixF operator+(MatrixF m1, MatrixF m2)
		{
			MatrixF result = m1.CarbonCopy() ;
			result.Add(m2) ;
			return result ;
		}
		public static MatrixF operator-(MatrixF m1, MatrixF m2)
		{
			MatrixF result = m1.CarbonCopy() ;
			result.Sub(m2) ;
			return result ;
		}
		public static MatrixF operator*(MatrixF m1, MatrixF m2)
		{
			MatrixF result = new MatrixF(m1.RowCount, m2.ColCount) ;
			MatrixF.Multiply(m1, m2, result) ;
			return result ;
		}
		public static MatrixF operator*(MatrixF m, float a)
		{
			MatrixF result = m.CarbonCopy() ;
			result.Multiply(a) ;
			return result ;
		}
		public static MatrixF operator*(float a, MatrixF m)
		{
			MatrixF result = m.CarbonCopy() ;
			result.Multiply(a) ;
			return result ;
		}
		/// <summary>
		/// Transpose operand matrix.
		/// </summary>
		public static MatrixF operator!(MatrixF operand)
		{
			return operand.Transposed() ;
		}
		/// <summary>
		/// Returns operand.Inverse(). This one modifies operand to it's factorized(PLU) form.
		/// </summary>
		public static MatrixF operator~(MatrixF operand)
		{
			return operand.Inverse() ;
		}
		#endregion
	}
	public class MatrixM
	{
		#region Protected Fields
		protected Decimal[,] coefficients ;
		protected int n ; // defines an n rows matrix
		protected int m ; // defines an m cols matrix
		#endregion

		#region Contruction
		public MatrixM(Decimal[,] coefficients)
		{
			if (coefficients == null)
				throw new ArgumentNullException() ;
			if (coefficients.GetLength(0) == 0 || coefficients.GetLength(1) == 0)
				throw new ArgumentException() ;
			this.coefficients = coefficients ;
			this.n = coefficients.GetLength(0) ;
			this.m = coefficients.GetLength(1) ;
		}
		public MatrixM(int n, int m) :
			this(new Decimal[n, m])
		{
		
		}
		#endregion

		#region Static Fields and Initialization
		static readonly NumberFormatInfo DefaultFloatFormat = new NumberFormatInfo() ;
		static MatrixM()
		{
			DefaultFloatFormat.NumberDecimalDigits = 2 ;
			DefaultFloatFormat.PositiveSign = "+" ;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the 1 base refered element in matrix.
		/// </summary>
		public Decimal this[int rowIndex, int colIndex]
		{
			get
			{
				return coefficients[rowIndex - 1, colIndex - 1] ;
			}
			set
			{
				coefficients[rowIndex - 1, colIndex - 1] = value ;
			}
		}
		/// <summary>
		/// Gets the Row count.
		/// </summary>
		public int RowCount
		{
			get
			{
				return n ;
			}
		}
		/// <summary>
		/// Gets the Columns count.
		/// </summary>
		public int ColCount
		{
			get
			{
				return m ;
			}
		}		
		#endregion
	
		#region Object Overrides and new ToString definitions
		public override string ToString()
		{
			return ToString(DefaultFloatFormat) ;
		}
		public virtual string ToString(NumberFormatInfo numberFormat)
		{
			if (m == 0 || n == 0)
				return "" ;
			StringBuilder result = new StringBuilder("") ;
			for(int i = 0; i < n; i++)
			{
				result.Append("|") ;
				for(int j = 0; j < m; j++)
				{
					result.Append(coefficients[i, j].ToString("N", numberFormat) + "\t") ;
				}
				result.Append("|") ;
				result.Append("\r\n") ;
			}
			result.Remove(result.Length - 2, 1) ;
			return result.ToString() ;
		}
		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is MatrixM))
				return false ;
			MatrixM matrix = (MatrixM)obj ;
			return n == matrix.n && m == matrix.m &&
				coefficients.Equals(matrix.coefficients) ;
		}
		public override int GetHashCode()
		{
			return n.GetHashCode() + m.GetHashCode() +
				coefficients.GetHashCode() ;
		}

		#endregion

		#region Public Methods/Basic Matrix Operations
		/// <summary>
		/// Returns an exact copy of current matrix.
		/// </summary>
		/// <remarks>This method does an array copy, a whole new memory space will be 
		/// used for this method's result. Be carefull.</remarks>
		public MatrixM CarbonCopy()
		{
			Decimal[,] cc = new Decimal[n, m] ;
			Array.Copy(coefficients, 0, cc, 0, coefficients.Length) ;
			return new MatrixM(cc) ;
		}
		/// <summary>
		/// Multiplies current matrix by an scalar.
		/// </summary>
		/// <param name="value">Scalara value to multiplicate by</param>
		public void Multiply(Decimal value)
		{
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					coefficients[i, j] *= value ;
		}
		/// <summary>
		/// Adds to matrices and places the result on current matrix.
		/// </summary>
		/// <param name="matrix">The other matrix to add</param>
		public void Add(MatrixM matrix)
		{
			if (matrix.n != n || matrix.m != m)
				throw new ArgumentException("Can't add") ;
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					coefficients[i, j] += matrix[i, j] ;
		}
		/// <summary>
		/// Substracts parameter matrix from current matrix.
		/// </summary>
		/// <param name="matrix">matrix to substract from current</param>
		public void Sub(MatrixM matrix)
		{
			if (matrix.n != n || matrix.m != m)
				throw new ArgumentException("Can't add") ;
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					coefficients[i, j] -= matrix[i, j] ;			
		}
		/// <summary>
		/// Pre-Multiplies parameter by this and places result on current matrix.
		/// </summary>
		/// <param name="matrix">Matrix to multiply by this</param>
		public void PreMultiply(MatrixM matrix)
		{
			MatrixM result = new MatrixM(matrix.n, m) ;
			Multiply(matrix, this, result) ;
			this.coefficients = result.coefficients ;
			this.n = result.n ;
		}
		/// <summary>
		/// Pre-Multiplies this by parameterand places result on current matrix.
		/// </summary>
		/// <param name="matrix">Matrix to multiply by this</param>
		public void PostMultiply(MatrixM matrix)
		{
			MatrixM result = new MatrixM(n, matrix.m) ;
			Multiply(this, matrix, result) ;
			this.coefficients = result.coefficients ;
			this.m = result.m ;
		}
		/// <summary>
		/// Calculates the determinant of current matrix. Modifies current matrix to it's factorized form PLU.
		/// </summary>
		/// <remarks>This is a high cost method.</remarks>
		public Decimal Determinant()
		{
			if (n != m)
				throw new InvalidOperationException("Cannot calculate determinant of a non squared matrix") ;
			int[] Pivot = new int[n] ;
			Decimal det = FactorizePivot(coefficients, Pivot) ;
			for(int i = 0; i < n; i++)
				det *= coefficients[i, i] ;
			return det ;
		}
		/// <summary>
		/// Calculates this matrix transposed.
		/// </summary>
		/// <returns>This matrix transposed</returns>
		/// <remarks>This method doesn't modify current matrix.</remarks>
		public MatrixM Transposed()
		{
			Decimal[,] result = new Decimal[m, n] ;
			for(int i = 0; i < n; i++)
				for(int j = 0; j < m; j++)
					result[j, i] = coefficients[i, j] ;
			return new MatrixM(result) ;
		}
		/// <summary>
		/// Factorizes current matrix and returns it's inverse.
		/// </summary>
		/// <remarks>As in Determinant, after using this method current matrix will be in it's
		/// factorized (PLU) form</remarks>
		public MatrixM Inverse()
		{
			if (n != m)
				throw new InvalidOperationException("Matrix should be NxN") ;
			MatrixM inv = new MatrixM(n, n) ;
			Inverse(coefficients, inv.coefficients, new int[n]) ;
			return inv ;
		}
		#endregion

		#region Static Methods. Most of these where translated from Fortran
		/// <summary>
		/// Returns the order n Identity Matrix.
		/// </summary>
		/// <param name="n">order of the matrix to be created.</param>
		public static MatrixM Identity(int n)
		{
			MatrixM result = new MatrixM(n, n) ;
			for(int i = 0; i < n; i++)
				result.coefficients[i, i] = 1 ;
			return result ;
		}
		/// <summary>
		/// Multiplies m1 by m2 and places result on result.
		/// </summary>
		public static void Multiply(MatrixM m1, MatrixM m2, MatrixM result)
		{
			if (m1.m != m2.n || result.n != m1.n || result.m != m2.m)
				throw new ArgumentException("Can't multiply") ;
			Decimal[,] rcoef = result.coefficients,
				m1coef = m1.coefficients,
				m2coef = m2.coefficients ;
			for(int i = 0; i < m1.n; i++)
				for(int j = 0; j < m2.m; j++)
					for(int e = 0; e < m1.m; e++)
						rcoef[i, j] += m1coef[i, e] * m2coef[e, j] ;
		}
		/// <summary>
		/// Calculates the Triangular Factorization of a Matrix using Choleski Factorization. Use this factorization
		/// when matrix is a symmetric positive definite matrix.
		/// References:
		/// 1) Elementary Numerical Analisys, An Algorithmic Aproach; 3rd Edition.
		/// </summary>
		/// <param name="W">An squared array representing the work matrix, also the resulting</param>
		/// <param name="Pivot">An n-vetor that will output the permutations aplied to rows in W</param>
		/// <returns>the sign of determinant, if 0 then W matrix is singular</returns>
		/// <remarks>This is not implemented in this version</remarks>
		public static int FactorizeCholeski(Decimal[,] W, int[] Pivot)
		{
			return -1 ;
		}
		/// <summary>
		/// Calculates the Triangular Factorization of a Matrix using Scaled Partial Pivot.
		/// Original matrix A will be factorized to PLU where P is a permutations vector, L is
		/// a lower Triangular Matrix and U an upper Triangular Matrix.
		/// References:
		/// 1) Elementary Numerical Analisys, An Algorithmic Aproach; 3rd Edition.
		/// </summary>
		/// <param name="W">An squared array representing the work matrix, also the resulting</param>
		/// <param name="Pivot">An n-vetor that will output the permutations aplied to rows in W</param>
		/// <returns>the sign of determinant, if 0 then W matrix is singular</returns>
		public static int FactorizePivot(Decimal[,] W, int[] Pivot)
		{
			#region Parameter Check & Locals Init
			if (W.GetLength(0) != W.GetLength(1) ||
				Pivot.Length != W.GetLength(0))
				throw new ArgumentException() ;
			int Size = W.GetLength(0) ;
			int N = Size - 1 ;
			int i, j, k ;
			int Flag = 1 ;
			Decimal[] D = new Decimal[Size] ;
			#endregion

			#region Fortran Algorithm
			#region Initialize Pivot, D
			for(i = 0; i <= N; i++)
			{
				Pivot[i] = i ;
				Decimal RowMax = .0M ;
				for(j = 0; j <= N; j++)
				{
					RowMax = Mathematics.Max(Mathematics.Abs(W[i, j]), RowMax) ;
					if (RowMax == .0M)
					{
						Flag = 0 ;
						RowMax = 1.0M ;
					}
				}
				D[i] = RowMax ;
			}
			#endregion
			if (Size <= 1)
				return 0 ;
			#region Factorization
			for(k = 0; k <= N - 1; k++)
			{
				#region Determine Pivot Row, the Row Istar
				Decimal ColMax = Mathematics.Abs(W[k, k]) / D[k] ;
				int Istar = k ;
				for(i = k + 1; i <= N; i++)
				{
					Decimal Awikod = Mathematics.Abs(W[i,k]) / D[k] ;
					if (Awikod > ColMax)
					{
						ColMax = Awikod ;
						Istar = i ;
					}
				}
				if (ColMax == .0M)
					Flag = 0 ;
				else
				{
					if (Istar > k)
					{
						#region Make k the pivot Row by interchanging it with the chosen row Istar
						Flag = -Flag ;
						i = Pivot[Istar] ;
						Pivot[Istar] = Pivot[k] ;
						Pivot[k] = i ;
						Decimal temp = D[Istar] ;
						D[Istar] = D[k] ;
						D[k] = temp ;
						for(j = 0; j <= N; j++)
						{
							temp = W[Istar, j] ;
							W[Istar, j] = W[k, j] ;
							W[k, j] = temp ;
						}
						#endregion
					}
					#region Eliminate X(k) From Rows k+1, .., N
					for(i = k + 1; i <= N; i++)
					{
						Decimal ratio = W[i, k] /= W[k, k] ;
						for(j = k + 1; j <= N; j++)
						{
							W[i, j] -= ratio * W[k, j] ;
						}
					}
					#endregion
				}
				#endregion
			}
			#endregion
			if (W[N, N] == 0)
				Flag = 0 ;
			#endregion

			#region Return
			return Flag ;
			#endregion
		}
		/// <summary>
		/// Solves an equation system using Forward and Back Substitution Algorithm. Work Matrix (W) must be
		/// factorized.
		/// </summary>
		/// <param name="W">Factorized squared array representing the work Matrix</param>
		/// <param name="Pivot">An n-vector representing the permutations done when factorizing W</param>
		/// <param name="B">Independent terms vector</param>
		/// <param name="X">Result</param>
		public static void Subst(Decimal[,] W, int[] Pivot, Decimal[] B, Decimal[] X)
		{
			#region Parameter Check and Initilization
			if (W.GetLength(0) != W.GetLength(1))
				throw new ArgumentException("Matrix must be NxN") ;
			int N = W.GetLength(0) - 1;
			int i, j ;
			Decimal sum ;
			#endregion

			#region Fortran algorithm
			if (N <= 1)
			{
				X[0] = B[0] / W[0, 0] ;
				return ;
			}
			int Ip = Pivot[0] ;
			X[0] = B[Ip] ;
			for(i = 1; i <= N; i++)
			{
				sum = .0M ;
				for(j = 0; j <= i - 1; j++)
					sum += W[i, j] * X[j] ;
				Ip = Pivot[i] ;
				X[i] = B[Ip] - sum ;
			}
			X[N] = X[N] / W[N, N] ;
			for(i = N - 1; i >= 0; i--)
			{
				sum = .0M ;
				for(j = i + 1; j <= N; j++)
				{
					sum += W[i, j] * X[j] ;
				}
				X[i] = (X[i] - sum) / W[i, i] ;
			}
			#endregion
		}
		/// <summary>
		/// Inverses a matrix using W as the work matrix.
		/// </summary>
		/// <param name="W"></param>
		/// <param name="InvW"></param>
		/// <remarks>Original data in W will be modified, can be recoverd but it's complicated.</remarks>
		public static void Inverse(Decimal[,] W, Decimal[,] InvW, int[] Pivot)
		{
			#region Initialization
			int N = W.GetLength(0) ;
			//			int[] Pivot = new int[N] ;
			Decimal[] B = new Decimal[N] ;
			Decimal[] X = new Decimal[N] ;
			#endregion

			#region Fortran Algorithm
			int Flag = FactorizePivot(W, Pivot) ;
			if (Flag == 0)
				throw new ArgumentException("Matrix is singular, cannot calculate inverse") ;
			int IBeg = 0 ;
			for(int j = 0; j < N; j++)
			{
				B[j] = 1.0M ;
				Subst(W, Pivot, B, X) ;
				B[j] = .0M ;
				for(int k = 0; k < N; k++)
					InvW[k, IBeg] = X[k] ;
				IBeg ++ ;
			}
			#endregion
		}
		/// <summary>
		/// Solves a linear equaltion system of the form AX=B, using the Gauss method.
		/// </summary>
		/// <param name="A">Coefficients Matrix</param>
		/// <param name="B">Independet Terms Matrix</param>
		/// <returns>Values for x that satifies AX=B</returns>
		/// <exception cref="ScaredFingers.Math.ArrayDimensionsException">Thrown if dimensions of parameters are not 
		/// correct. B shoul be an N vector and A an NxN Matrix</exception>
		/// <remarks>This method is obsolete. </remarks>
		public static void Solve(Decimal[,] A, Decimal[] B, Decimal[] X)
		{
			#region Local Initialization and Parameter Check
			int N = B.Length ;
			if (A.GetLength(0) != A.GetLength(1) ||
				N != A.GetLength(0))
				throw new ArrayDimensionsException() ;
			Decimal[,] WorkMatrix = new Decimal[N, N] ;
			Array.Copy(A, 0, WorkMatrix, 0, A.Length) ;
			A = WorkMatrix ;
			#endregion

			#region Triangulation
			for(int i = 0; i < N - 1; i++)
			{
				Decimal a = A[i, i] ;
				for(int j = i + 1; j < N; j++)
				{
					Decimal b = A[j, i] ;
					Decimal pivot = - b / a ;
					A[j, i] = 0 ;
					for(int jj = i + 1; jj < N; jj++)
						A[j, jj] += pivot * A[i, jj] ;
					B[j] += pivot * B[i] ;
				}
			}
			#endregion

			#region Create Result
			//			X[N - 1] = B[N - 1] / A[N - 1, N - 1] ;
			for(int i = N - 1; i >= 0; i--)
			{
				Decimal sum = B[i] ;
				for(int j = i + 1; j < N; j ++)
					sum -= X[j] * A[i, j] ;
				X[i] = sum / A[i, i] ;
			}
			#endregion
		}
		#endregion

		#region Operators
		public static MatrixM operator+(MatrixM m1, MatrixM m2)
		{
			MatrixM result = m1.CarbonCopy() ;
			result.Add(m2) ;
			return result ;
		}
		public static MatrixM operator-(MatrixM m1, MatrixM m2)
		{
			MatrixM result = m1.CarbonCopy() ;
			result.Sub(m2) ;
			return result ;
		}
		public static MatrixM operator*(MatrixM m1, MatrixM m2)
		{
			MatrixM result = new MatrixM(m1.RowCount, m2.ColCount) ;
			MatrixM.Multiply(m1, m2, result) ;
			return result ;
		}
		public static MatrixM operator*(MatrixM m, Decimal a)
		{
			MatrixM result = m.CarbonCopy() ;
			result.Multiply(a) ;
			return result ;
		}
		public static MatrixM operator*(Decimal a, MatrixM m)
		{
			MatrixM result = m.CarbonCopy() ;
			result.Multiply(a) ;
			return result ;
		}
		/// <summary>
		/// Transpose operand matrix.
		/// </summary>
		public static MatrixM operator!(MatrixM operand)
		{
			return operand.Transposed() ;
		}
		/// <summary>
		/// Returns operand.Inverse(). This one modifies operand to it's factorized(PLU) form.
		/// </summary>
		public static MatrixM operator~(MatrixM operand)
		{
			return operand.Inverse() ;
		}
		#endregion
	}
}
