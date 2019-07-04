using System;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Converts one object to another through use of a cast.
    /// </summary>
    /// <param name="sourceObj"></param>
    /// <returns></returns>

    public delegate Object CastConverter(Object sourceObj);

    /// <summary>
    /// Provides efficient cast methods for converting from object to
    /// primitive types.  The cast method provided here-in is consistent
    /// with the cast mechanics of C#.  These cast mechanics are not
    /// the same as those provided by the IConvertible interface.
    /// </summary>

    public class CastHelper
    {
        /// <summary>
        /// Gets the cast converter for the specified type.  If none is
        /// found, this method returns null.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static CastConverter GetCastConverter(Type t)
        {
            Type baseT = Nullable.GetUnderlyingType(t);
            if ( baseT != null )
            {
                t = baseT;
            }

            if (t == typeof (Int32)) {
                return PrimitiveCastInt32;
            }
            if (t == typeof (Int64)) {
                return PrimitiveCastInt64;
            }
            if (t == typeof (Int16)) {
                return PrimitiveCastInt16;
            }
            if (t == typeof (SByte)) {
                return PrimitiveCastSByte;
            }
            if (t == typeof (Single)) {
                return PrimitiveCastSingle;
            }
            if (t == typeof (Double)) {
                return PrimitiveCastDouble;
            }
            if (t == typeof (Decimal)) {
                return PrimitiveCastDecimal;
            }
            if (t == typeof (UInt32)) {
                return PrimitiveCastUInt32;
            }
            if (t == typeof (UInt64)) {
                return PrimitiveCastUInt64;
            }
            if (t == typeof (UInt16)) {
                return PrimitiveCastUInt16;
            }
            if (t == typeof (Byte)) {
                return PrimitiveCastByte;
            }

            return delegate(Object sourceObj) {
                       Type sourceObjType = sourceObj.GetType();
                       if (t.IsAssignableFrom(sourceObjType)) {
                           return sourceObj;
                       } else {
                           return null;
                       }
                   };
        }

        /// <summary>
        /// Casts the source object to a Int32.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastInt32(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(Int32))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return ((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64))
            {
                return (Int32)((Int64)sourceObj);
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return (Int32)((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16))
            {
                return (Int32)((Int16)sourceObj);
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return (Int32)((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte))
            {
                return (Int32)((SByte)sourceObj);
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return (Int32)((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single))
            {
                return (Int32)((Single)sourceObj);
            }
            else if (sourceObjType == typeof(Single?))
            {
                return (Int32)((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double))
            {
                return (Int32)((Double)sourceObj);
            }
            else if (sourceObjType == typeof(Double?))
            {
                return (Int32)((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal))
            {
                return (Int32)((Decimal)sourceObj);
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return (Int32)((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32))
            {
                return (Int32)((UInt32)sourceObj);
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return (Int32)((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64))
            {
                return (Int32)((UInt64)sourceObj);
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return (Int32)((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16))
            {
                return (Int32)((UInt16)sourceObj);
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return (Int32)((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte))
            {
                return (Int32)((Byte)sourceObj);
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return (Int32)((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the source object to a Int64.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastInt64(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(Int64))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32))
            {
                return (Int64)((Int32)sourceObj);
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return (Int64)((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return ((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16))
            {
                return (Int64)((Int16)sourceObj);
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return (Int64)((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte))
            {
                return (Int64)((SByte)sourceObj);
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return (Int64)((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single))
            {
                return (Int64)((Single)sourceObj);
            }
            else if (sourceObjType == typeof(Single?))
            {
                return (Int64)((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double))
            {
                return (Int64)((Double)sourceObj);
            }
            else if (sourceObjType == typeof(Double?))
            {
                return (Int64)((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal))
            {
                return (Int64)((Decimal)sourceObj);
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return (Int64)((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32))
            {
                return (Int64)((UInt32)sourceObj);
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return (Int64)((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64))
            {
                return (Int64)((UInt64)sourceObj);
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return (Int64)((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16))
            {
                return (Int64)((UInt16)sourceObj);
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return (Int64)((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte))
            {
                return (Int64)((Byte)sourceObj);
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return (Int64)((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the source object to a Int16.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastInt16(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(Int16))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32))
            {
                return (Int16)((Int32)sourceObj);
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return (Int16)((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64))
            {
                return (Int16)((Int64)sourceObj);
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return (Int16)((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return ((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte))
            {
                return (Int16)((SByte)sourceObj);
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return (Int16)((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single))
            {
                return (Int16)((Single)sourceObj);
            }
            else if (sourceObjType == typeof(Single?))
            {
                return (Int16)((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double))
            {
                return (Int16)((Double)sourceObj);
            }
            else if (sourceObjType == typeof(Double?))
            {
                return (Int16)((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal))
            {
                return (Int16)((Decimal)sourceObj);
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return (Int16)((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32))
            {
                return (Int16)((UInt32)sourceObj);
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return (Int16)((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64))
            {
                return (Int16)((UInt64)sourceObj);
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return (Int16)((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16))
            {
                return (Int16)((UInt16)sourceObj);
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return (Int16)((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte))
            {
                return (Int16)((Byte)sourceObj);
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return (Int16)((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the source object to a SByte.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastSByte(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(SByte))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32))
            {
                return (SByte)((Int32)sourceObj);
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return (SByte)((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64))
            {
                return (SByte)((Int64)sourceObj);
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return (SByte)((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16))
            {
                return (SByte)((Int16)sourceObj);
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return (SByte)((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return ((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single))
            {
                return (SByte)((Single)sourceObj);
            }
            else if (sourceObjType == typeof(Single?))
            {
                return (SByte)((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double))
            {
                return (SByte)((Double)sourceObj);
            }
            else if (sourceObjType == typeof(Double?))
            {
                return (SByte)((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal))
            {
                return (SByte)((Decimal)sourceObj);
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return (SByte)((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32))
            {
                return (SByte)((UInt32)sourceObj);
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return (SByte)((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64))
            {
                return (SByte)((UInt64)sourceObj);
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return (SByte)((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16))
            {
                return (SByte)((UInt16)sourceObj);
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return (SByte)((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte))
            {
                return (SByte)((Byte)sourceObj);
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return (SByte)((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the source object to a Single.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastSingle(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(Single))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32))
            {
                return (Single)((Int32)sourceObj);
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return (Single)((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64))
            {
                return (Single)((Int64)sourceObj);
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return (Single)((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16))
            {
                return (Single)((Int16)sourceObj);
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return (Single)((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte))
            {
                return (Single)((SByte)sourceObj);
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return (Single)((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single?))
            {
                return ((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double))
            {
                return (Single)((Double)sourceObj);
            }
            else if (sourceObjType == typeof(Double?))
            {
                return (Single)((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal))
            {
                return (Single)((Decimal)sourceObj);
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return (Single)((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32))
            {
                return (Single)((UInt32)sourceObj);
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return (Single)((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64))
            {
                return (Single)((UInt64)sourceObj);
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return (Single)((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16))
            {
                return (Single)((UInt16)sourceObj);
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return (Single)((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte))
            {
                return (Single)((Byte)sourceObj);
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return (Single)((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the source object to a Double.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastDouble(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(Double))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32))
            {
                return (Double)((Int32)sourceObj);
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return (Double)((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64))
            {
                return (Double)((Int64)sourceObj);
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return (Double)((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16))
            {
                return (Double)((Int16)sourceObj);
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return (Double)((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte))
            {
                return (Double)((SByte)sourceObj);
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return (Double)((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single))
            {
                return (Double)((Single)sourceObj);
            }
            else if (sourceObjType == typeof(Single?))
            {
                return (Double)((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double?))
            {
                return ((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal))
            {
                return (Double)((Decimal)sourceObj);
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return (Double)((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32))
            {
                return (Double)((UInt32)sourceObj);
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return (Double)((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64))
            {
                return (Double)((UInt64)sourceObj);
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return (Double)((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16))
            {
                return (Double)((UInt16)sourceObj);
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return (Double)((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte))
            {
                return (Double)((Byte)sourceObj);
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return (Double)((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the source object to a Decimal.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastDecimal(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(Decimal))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32))
            {
                return (Decimal)((Int32)sourceObj);
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return (Decimal)((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64))
            {
                return (Decimal)((Int64)sourceObj);
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return (Decimal)((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16))
            {
                return (Decimal)((Int16)sourceObj);
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return (Decimal)((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte))
            {
                return (Decimal)((SByte)sourceObj);
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return (Decimal)((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single))
            {
                return (Decimal)((Single)sourceObj);
            }
            else if (sourceObjType == typeof(Single?))
            {
                return (Decimal)((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double))
            {
                return (Decimal)((Double)sourceObj);
            }
            else if (sourceObjType == typeof(Double?))
            {
                return (Decimal)((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return ((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32))
            {
                return (Decimal)((UInt32)sourceObj);
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return (Decimal)((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64))
            {
                return (Decimal)((UInt64)sourceObj);
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return (Decimal)((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16))
            {
                return (Decimal)((UInt16)sourceObj);
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return (Decimal)((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte))
            {
                return (Decimal)((Byte)sourceObj);
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return (Decimal)((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the source object to a UInt32.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastUInt32(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(UInt32))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32))
            {
                return (UInt32)((Int32)sourceObj);
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return (UInt32)((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64))
            {
                return (UInt32)((Int64)sourceObj);
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return (UInt32)((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16))
            {
                return (UInt32)((Int16)sourceObj);
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return (UInt32)((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte))
            {
                return (UInt32)((SByte)sourceObj);
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return (UInt32)((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single))
            {
                return (UInt32)((Single)sourceObj);
            }
            else if (sourceObjType == typeof(Single?))
            {
                return (UInt32)((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double))
            {
                return (UInt32)((Double)sourceObj);
            }
            else if (sourceObjType == typeof(Double?))
            {
                return (UInt32)((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal))
            {
                return (UInt32)((Decimal)sourceObj);
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return (UInt32)((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return ((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64))
            {
                return (UInt32)((UInt64)sourceObj);
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return (UInt32)((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16))
            {
                return (UInt32)((UInt16)sourceObj);
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return (UInt32)((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte))
            {
                return (UInt32)((Byte)sourceObj);
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return (UInt32)((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the source object to a UInt64.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastUInt64(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(UInt64))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32))
            {
                return (UInt64)((Int32)sourceObj);
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return (UInt64)((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64))
            {
                return (UInt64)((Int64)sourceObj);
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return (UInt64)((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16))
            {
                return (UInt64)((Int16)sourceObj);
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return (UInt64)((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte))
            {
                return (UInt64)((SByte)sourceObj);
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return (UInt64)((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single))
            {
                return (UInt64)((Single)sourceObj);
            }
            else if (sourceObjType == typeof(Single?))
            {
                return (UInt64)((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double))
            {
                return (UInt64)((Double)sourceObj);
            }
            else if (sourceObjType == typeof(Double?))
            {
                return (UInt64)((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal))
            {
                return (UInt64)((Decimal)sourceObj);
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return (UInt64)((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32))
            {
                return (UInt64)((UInt32)sourceObj);
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return (UInt64)((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return ((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16))
            {
                return (UInt64)((UInt16)sourceObj);
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return (UInt64)((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte))
            {
                return (UInt64)((Byte)sourceObj);
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return (UInt64)((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the source object to a UInt16.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastUInt16(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(UInt16))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32))
            {
                return (UInt16)((Int32)sourceObj);
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return (UInt16)((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64))
            {
                return (UInt16)((Int64)sourceObj);
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return (UInt16)((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16))
            {
                return (UInt16)((Int16)sourceObj);
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return (UInt16)((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte))
            {
                return (UInt16)((SByte)sourceObj);
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return (UInt16)((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single))
            {
                return (UInt16)((Single)sourceObj);
            }
            else if (sourceObjType == typeof(Single?))
            {
                return (UInt16)((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double))
            {
                return (UInt16)((Double)sourceObj);
            }
            else if (sourceObjType == typeof(Double?))
            {
                return (UInt16)((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal))
            {
                return (UInt16)((Decimal)sourceObj);
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return (UInt16)((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32))
            {
                return (UInt16)((UInt32)sourceObj);
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return (UInt16)((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64))
            {
                return (UInt16)((UInt64)sourceObj);
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return (UInt16)((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return ((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte))
            {
                return (UInt16)((Byte)sourceObj);
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return (UInt16)((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the source object to a Byte.
        /// </summary>
        /// <param name="sourceObj">The source object.</param>

        public static Object PrimitiveCastByte(Object sourceObj)
        {
            if (sourceObj == null)
            {
                return null;
            }

            Type sourceObjType = sourceObj.GetType();
            if (sourceObjType == typeof(Byte))
            {
                return sourceObj;
            }
            else if (sourceObjType == typeof(Int32))
            {
                return (Byte)((Int32)sourceObj);
            }
            else if (sourceObjType == typeof(Int32?))
            {
                return (Byte)((Int32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int64))
            {
                return (Byte)((Int64)sourceObj);
            }
            else if (sourceObjType == typeof(Int64?))
            {
                return (Byte)((Int64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Int16))
            {
                return (Byte)((Int16)sourceObj);
            }
            else if (sourceObjType == typeof(Int16?))
            {
                return (Byte)((Int16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(SByte))
            {
                return (Byte)((SByte)sourceObj);
            }
            else if (sourceObjType == typeof(SByte?))
            {
                return (Byte)((SByte?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Single))
            {
                return (Byte)((Single)sourceObj);
            }
            else if (sourceObjType == typeof(Single?))
            {
                return (Byte)((Single?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Double))
            {
                return (Byte)((Double)sourceObj);
            }
            else if (sourceObjType == typeof(Double?))
            {
                return (Byte)((Double?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Decimal))
            {
                return (Byte)((Decimal)sourceObj);
            }
            else if (sourceObjType == typeof(Decimal?))
            {
                return (Byte)((Decimal?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt32))
            {
                return (Byte)((UInt32)sourceObj);
            }
            else if (sourceObjType == typeof(UInt32?))
            {
                return (Byte)((UInt32?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt64))
            {
                return (Byte)((UInt64)sourceObj);
            }
            else if (sourceObjType == typeof(UInt64?))
            {
                return (Byte)((UInt64?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(UInt16))
            {
                return (Byte)((UInt16)sourceObj);
            }
            else if (sourceObjType == typeof(UInt16?))
            {
                return (Byte)((UInt16?)sourceObj).Value;
            }
            else if (sourceObjType == typeof(Byte?))
            {
                return ((Byte?)sourceObj).Value;
            }
            else
            {
                return null;
            }
        }
    }
}
