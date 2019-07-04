using System;
using System.Data;
using System.Data.Common;

using com.espertech.esper.util;

namespace com.espertech.esper.epl.db.drivers
{
    /// <summary>
    /// A generic database driver.
    /// </summary>
    [Serializable]
    public class DbDriverGeneric : BaseDbDriver
    {
        private readonly DbProviderFactory dbProviderFactory;
        private readonly bool isPositional;
        private readonly String paramPrefix;

        /// <summary>
        /// Initializes the <see cref="DbDriverGeneric"/> class.
        /// </summary>
        public DbDriverGeneric()
        {
            dbProviderFactory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
            isPositional = false;
            paramPrefix = "@";
        }

        /// <summary>
        /// Factory method that is used to create instance of a connection.
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            try
            {
                DbConnection dbConnection = dbProviderFactory.CreateConnection();
                dbConnection.ConnectionString = ConnectionString;
                dbConnection.Open();
                return dbConnection;
            }
            catch (DbException ex)
            {
                String detail = "DbException: " + ex.Message + " VendorError: " + ex.ErrorCode;
                throw new DatabaseConfigException(
                    "Error obtaining database connection using connection-string '" + ConnectionString +
                    "' with detail " + detail, ex);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [use position parameters].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use position parameters]; otherwise, <c>false</c>.
        /// </value>
        protected override bool UsePositionalParameters
        {
            get { return isPositional; }
        }

        /// <summary>
        /// Gets the parameter prefix.
        /// </summary>
        /// <value>The param prefix.</value>
        protected override string ParamPrefix
        {
            get { return paramPrefix; }
        }

        /// <summary>
        /// Creates a connection string builder.
        /// </summary>
        /// <returns></returns>
        protected override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return dbProviderFactory.CreateConnectionStringBuilder();
        }
    }
}
