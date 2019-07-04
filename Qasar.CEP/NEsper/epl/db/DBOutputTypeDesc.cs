using System;

using com.espertech.esper.util;

namespace com.espertech.esper.epl.db
{
	/// <summary>
    /// Descriptor for SQL output columns.
    /// </summary>
	
    public class DBOutputTypeDesc
	{
		/// <summary> Returns the SQL type of the output column.</summary>
		/// <returns> sql type
		/// </returns>
		virtual public string SqlType
		{
			get { return sqlType; }
		}

		/// <summary> Returns the type that getObject on the output column produces.</summary>
		/// <returns> type from statement metadata
		/// </returns>
		
        virtual public Type DataType
		{
			get { return dataType; }
		}

        /// <summary>
        /// Gets the optional mapping from output column type to built-in.
        /// </summary>
        virtual public DatabaseTypeBinding OptionalBinding
        {
            get { return optionalBinding; }
        }
		
        private readonly string sqlType;
		private readonly Type dataType ;
        private readonly DatabaseTypeBinding optionalBinding;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="sqlType">the type of the column</param>
        /// <param name="dataType">the type reflecting column type</param>
        /// <param name="optionalBinding">the optional mapping from output column type to built-in</param>
		
        public DBOutputTypeDesc(string sqlType, Type dataType, DatabaseTypeBinding optionalBinding)
		{
			this.sqlType = sqlType;
            this.dataType = dataType;
            this.optionalBinding = optionalBinding;
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override String ToString()
		{
			return "sqlType=" + sqlType + " dataType=" + dataType;
		}
	}
}