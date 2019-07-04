using System;

using com.espertech.esper.compat;
using com.espertech.esper.collection;

namespace com.espertech.esper.type
{
    /// <summary>
    /// Enumeration for the type of arithmatic to use.
    /// </summary>

    [Serializable]
    public class MathArithTypeEnum
    {
        /// <summary>
        /// Plus.
        /// </summary>
        public static readonly MathArithTypeEnum ADD = new MathArithTypeEnum("+");

        /// <summary>
        /// Minus
        /// </summary>

        public static readonly MathArithTypeEnum SUBTRACT = new MathArithTypeEnum("-");

        /// <summary>
        /// Divide
        /// </summary>

        public static readonly MathArithTypeEnum DIVIDE = new MathArithTypeEnum("/");

        /// <summary>
        /// Multiply.
        /// </summary>

        public static readonly MathArithTypeEnum MULTIPLY = new MathArithTypeEnum("*");

        /// <summary>
        /// Modulo.
        /// </summary>

        public static readonly MathArithTypeEnum MODULO = new MathArithTypeEnum("%");

        /// <summary>
        /// Public enumeration of pseudo-enumerated type values.
        /// </summary>

        public static readonly MathArithTypeEnum[] Values = {
            ADD,
            SUBTRACT,
            DIVIDE,
            MULTIPLY,
            MODULO
        };


        private static Map<MultiKey<Object>, Computer> computers;

        static MathArithTypeEnum()
        {
            computers = new HashMap<MultiKey<Object>, Computer>();
            computers.Add(new MultiKey<Object>(new Object[] { typeof(decimal?), ADD }), AddDecimal);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(double?), ADD }), AddDouble);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(float?), ADD }), AddSingle);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(long?), ADD }), AddInt64);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(int?), ADD }), AddInt32);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(ulong?), ADD }), AddUInt64);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(uint?), ADD }), AddUInt32);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(decimal?), SUBTRACT }), SubtractDecimal);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(double?), SUBTRACT }), SubtractDouble);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(float?), SUBTRACT }), SubtractSingle);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(long?), SUBTRACT }), SubtractInt64);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(int?), SUBTRACT }), SubtractInt32);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(ulong?), SUBTRACT }), SubtractUInt64);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(uint?), SUBTRACT }), SubtractUInt32);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(decimal?), DIVIDE }), DivideDecimal);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(double?), DIVIDE }), DivideDouble);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(float?), DIVIDE }), DivideSingle);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(long?), DIVIDE }), DivideInt64);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(int?), DIVIDE }), DivideInt32);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(ulong?), DIVIDE }), DivideUInt64);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(uint?), DIVIDE }), DivideUInt32);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(decimal?), MULTIPLY }), MultiplyDecimal);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(double?), MULTIPLY }), MultiplyDouble);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(float?), MULTIPLY }), MultiplySingle);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(long?), MULTIPLY }), MultiplyInt64);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(int?), MULTIPLY }), MultiplyInt32);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(ulong?), MULTIPLY }), MultiplyUInt64);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(uint?), MULTIPLY }), MultiplyUInt32);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(decimal), MODULO }), ModuloDecimal);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(double?), MODULO }), ModuloDouble);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(float?), MODULO }), ModuloSingle);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(long?), MODULO }), ModuloInt64);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(int?), MODULO }), ModuloInt32);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(ulong?), MODULO }), ModuloUInt64);
            computers.Add(new MultiKey<Object>(new Object[] { typeof(uint?), MODULO }), ModuloUInt32);
        }

        /// <summary>
        /// Interface for number cruncher.
        /// </summary>

        public delegate Object Computer(Object d1, Object d2);

        private readonly String expressionText;

        private MathArithTypeEnum(String expressionText)
        {
            this.expressionText = expressionText;
        }

        /// <summary>
        /// Returns number cruncher for the target coercion type.
        /// </summary>
        /// <param name="coercedType">target type</param>
        /// <returns>number cruncher</returns>

        public Computer GetComputer(Type coercedType)
        {
            if ((coercedType != typeof(decimal?)) &&
                (coercedType != typeof(double?)) &&
                (coercedType != typeof(float?)) &&
                (coercedType != typeof(long?)) &&
                (coercedType != typeof(int?)))
            {
                throw new ArgumentException("Expected base numeric type for computation result but got type " + coercedType);
            }

            MultiKey<Object> key = new MultiKey<Object>(new Object[] { coercedType, this });
            Computer computer = computers.Get(key);

            if (computer == null)
            {
                throw new ArgumentException("Could not determine process or type " + this + " type " + coercedType);
            }
            return computer;
        }

        /// <summary>
        /// Adds decimals.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object AddDecimal(Object d1, Object d2)
        {
            return Convert.ToDecimal(d1) + Convert.ToDecimal(d2);
        }

        /// <summary>
        /// Adds doubles.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object AddDouble(Object d1, Object d2)
        {
            return Convert.ToDouble(d1) + Convert.ToDouble(d2);
        }

        /// <summary>
        /// Adds singles.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object AddSingle(Object d1, Object d2)
        {
            return Convert.ToSingle(d1) + Convert.ToSingle(d2);
        }

        /// <summary>
        /// Adds int64s.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object AddInt64(Object d1, Object d2)
        {
            return Convert.ToInt64(d1) + Convert.ToInt64(d2);
        }

        /// <summary>
        /// Adds int32s.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object AddInt32(Object d1, Object d2)
        {
            return Convert.ToInt32(d1) + Convert.ToInt32(d2);
        }

        /// <summary>
        /// Adds unsigned int64s.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object AddUInt64(Object d1, Object d2)
        {
            return Convert.ToUInt64(d1) + Convert.ToUInt64(d2);
        }

        /// <summary>
        /// Adds unsigned 32-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object AddUInt32(Object d1, Object d2)
        {
            return Convert.ToUInt32(d1) + Convert.ToUInt32(d2);
        }

        /// <summary>
        /// Subtracts decimals.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object SubtractDecimal(Object d1, Object d2)
        {
            return Convert.ToDecimal(d1) - Convert.ToDecimal(d2);
        }

        /// <summary>
        /// Subtracts doubles.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object SubtractDouble(Object d1, Object d2)
        {
            return Convert.ToDouble(d1) - Convert.ToDouble(d2);
        }

        /// <summary>
        /// Subtracts singles.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object SubtractSingle(Object d1, Object d2)
        {
            return Convert.ToSingle(d1) - Convert.ToSingle(d2);
        }

        /// <summary>
        /// Subtracts 64-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object SubtractInt64(Object d1, Object d2)
        {
            return Convert.ToInt64(d1) - Convert.ToInt64(d2);
        }

        /// <summary>
        /// Subtracts 32-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object SubtractInt32(Object d1, Object d2)
        {
            return Convert.ToInt32(d1) - Convert.ToInt32(d2);
        }

        /// <summary>
        /// Subtracts unsigned 64-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object SubtractUInt64(Object d1, Object d2)
        {
            return Convert.ToUInt64(d1) - Convert.ToUInt64(d2);
        }

        /// <summary>
        /// Subtracts unsigned 32-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object SubtractUInt32(Object d1, Object d2)
        {
            return Convert.ToUInt32(d1) - Convert.ToUInt32(d2);
        }

        /// <summary>
        /// Divides decimals.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object DivideDecimal(Object d1, Object d2)
        {
            return Convert.ToDecimal(d1) / Convert.ToDecimal(d2);
        }

        /// <summary>
        /// Divides doubles.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object DivideDouble(Object d1, Object d2)
        {
            return Convert.ToDouble(d1) / Convert.ToDouble(d2);
        }

        /// <summary>
        /// Divides singles.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object DivideSingle(Object d1, Object d2)
        {
            return Convert.ToSingle(d1) / Convert.ToSingle(d2);
        }

        /// <summary>
        /// Divides 64-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object DivideInt64(Object d1, Object d2)
        {
            return Convert.ToInt64(d1) / Convert.ToInt64(d2);
        }

        /// <summary>
        /// Divides 32-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object DivideInt32(Object d1, Object d2)
        {
            return Convert.ToInt32(d1) / Convert.ToInt32(d2);
        }

        /// <summary>
        /// Divides unsigned 64-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object DivideUInt64(Object d1, Object d2)
        {
            return Convert.ToUInt64(d1) / Convert.ToUInt64(d2);
        }

        /// <summary>
        /// Divides unsigned 32-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object DivideUInt32(Object d1, Object d2)
        {
            return Convert.ToUInt32(d1) / Convert.ToUInt32(d2);
        }

        /// <summary>
        /// Multiplies decimals.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object MultiplyDecimal(Object d1, Object d2)
        {
            return Convert.ToDecimal(d1) * Convert.ToDecimal(d2);
        }

        /// <summary>
        /// Multiplies doubles.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object MultiplyDouble(Object d1, Object d2)
        {
            return Convert.ToDouble(d1) * Convert.ToDouble(d2);
        }

        /// <summary>
        /// Multiplies singles.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object MultiplySingle(Object d1, Object d2)
        {
            return Convert.ToSingle(d1) * Convert.ToSingle(d2);
        }

        /// <summary>
        /// Multiplies 64-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object MultiplyInt64(Object d1, Object d2)
        {
            return Convert.ToInt64(d1) * Convert.ToInt64(d2);
        }

        /// <summary>
        /// Multiplies 32-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object MultiplyInt32(Object d1, Object d2)
        {
            return Convert.ToInt32(d1) * Convert.ToInt32(d2);
        }

        /// <summary>
        /// Multiplies unsigned 64-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object MultiplyUInt64(Object d1, Object d2)
        {
            return Convert.ToUInt64(d1) * Convert.ToUInt64(d2);
        }

        /// <summary>
        /// Multiplies unsigned 32-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object MultiplyUInt32(Object d1, Object d2)
        {
            return Convert.ToUInt32(d1) * Convert.ToUInt32(d2);
        }

        /// <summary>
        /// Moduloes decimals.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object ModuloDecimal(Object d1, Object d2)
        {
            return Convert.ToDecimal(d1) % Convert.ToDecimal(d2);
        }

        /// <summary>
        /// Moduloes doubles.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object ModuloDouble(Object d1, Object d2)
        {
            return Convert.ToDouble(d1) % Convert.ToDouble(d2);
        }

        /// <summary>
        /// Moduloes singles.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object ModuloSingle(Object d1, Object d2)
        {
            return Convert.ToSingle(d1) % Convert.ToSingle(d2);
        }

        /// <summary>
        /// Moduloes 64-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object ModuloInt64(Object d1, Object d2)
        {
            return Convert.ToInt64(d1) % Convert.ToInt64(d2);
        }

        /// <summary>
        /// Moduloes 32-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object ModuloInt32(Object d1, Object d2)
        {
            return Convert.ToInt32(d1) % Convert.ToInt32(d2);
        }

        /// <summary>
        /// Moduloes unsigned 64-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object ModuloUInt64(Object d1, Object d2)
        {
            return Convert.ToUInt64(d1) % Convert.ToUInt64(d2);
        }

        /// <summary>
        /// Moduloes unsigned 32-bit ints.
        /// </summary>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <returns></returns>
        public static Object ModuloUInt32(Object d1, Object d2)
        {
            return Convert.ToUInt32(d1) % Convert.ToUInt32(d2);
        }

        /// <summary>Returns string representation of enum.</summary>
        /// <returns>text for enum</returns>

        public String ExpressionText
        {
            get { return expressionText; }
        }

        /// <summary>
        /// Returns the math operator for the string.
        /// </summary>
        /// <param name="operator">operator to parse</param>
        /// <returns>math enum</returns>
        public static MathArithTypeEnum ParseOperator(String @operator)
        {
            MathArithTypeEnum[] enumArray = MathArithTypeEnum.Values;
            for (int i = 0; i < enumArray.Length; i++)
            {
                MathArithTypeEnum val = enumArray[i];
                if (val.ExpressionText == @operator)
                {
                    return val;
                }
            }
            throw new ArgumentException("Unknown operator '" + @operator + "'");
        }
    }
}
