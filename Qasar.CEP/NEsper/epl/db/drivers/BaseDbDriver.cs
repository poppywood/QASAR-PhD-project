using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.epl.db;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.db.drivers
{
    /// <summary>
    /// An abstract base driver that provides some of the functionality
    /// that is common to all ADO.NET based drivers, but leaves the specifics
    /// of the database to the driver implementation.  ADO.NET leaves some
    /// wholes in its implementation and advises that for maximum performance
    /// that you use driver specific semantics.  This code exists to allow
    /// developers to integrate their own database models.
    /// </summary>
    [Serializable]
    abstract public class BaseDbDriver : DbDriver
    {
        private const String SAMPLE_WHERECLAUSE_PLACEHOLDER = "$ESPER-SAMPLE-WHERE";

        /// <summary>
        /// Connection name
        /// </summary>
        private String name;

        /// <summary>
        /// Connection properties
        /// </summary>
        private Properties connectionProperties;

        /// <summary>
        /// Connection string
        /// </summary>
        private String connectionString;

        /// <summary>
        /// Creates a connection string builder.
        /// </summary>
        /// <returns></returns>
        protected abstract DbConnectionStringBuilder CreateConnectionStringBuilder();

        /// <summary>
        /// Gets the default meta origin policy.
        /// </summary>
        /// <value>The default meta origin policy.</value>
        public virtual ConfigurationDBRef.MetadataOriginEnum DefaultMetaOriginPolicy
        {
            get { return ConfigurationDBRef.MetadataOriginEnum.DEFAULT; }
        }

        /// <summary>
        /// Gets the parameter prefix.
        /// </summary>
        /// <value>The param prefix.</value>
        abstract protected String ParamPrefix { get; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public virtual String ConnectionString
        {
            get { return connectionString; }
            protected set { connectionString = value;  }
        }

        /// <summary>
        /// Gets a value indicating whether [use position parameters].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use position parameters]; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool UsePositionalParameters
        {
            get { return false;  }
        }

        #region "PositionalToTextConversion"

        /// <summary>
        /// Converts a positional parameter into text that can be embedded
        /// into the command text.
        /// </summary>
        /// <param name="parameterIndex"></param>
        /// <returns></returns>

        protected delegate String PositionalToTextConverter(int parameterIndex);

        /// <summary>
        /// Gets the text for the parameter at the given index.
        /// </summary>
        /// <param name="parameterIndex">Index of the parameter.</param>
        /// <returns></returns>
        protected String PositionalToNamedTextConverter(int parameterIndex)
        {
            return String.Format("{0}arg{1}", ParamPrefix, parameterIndex);
        }

        /// <summary>
        /// Gets the text for the parameter at the given index.
        /// </summary>
        /// <param name="parameterIndex">Index of the parameter.</param>
        /// <returns></returns>
        protected String PositionalToPositionalTextConverter(int parameterIndex)
        {
            return ParamPrefix;
        }

        /// <summary>
        /// Gets the positional to text converter.
        /// </summary>
        protected PositionalToTextConverter ParamToTextConverter
        {
            get
            {
                if (UsePositionalParameters)
                {
                    return PositionalToPositionalTextConverter;
                }
                else
                {
                    return PositionalToNamedTextConverter;
                }
            }
        }
        #endregion

        /// <summary>
        /// Weak reference to the database connection.  Allows a thread to
        /// reuse an existing connection rather than opening a new one as
        /// opening a new connection can be considerably expensive with some
        /// drivers.  The reference is weak which means that after it is no
        /// longer is use, the weak reference will go out of scope.  To
        /// prevent the database connection from going out of scope prematurely
        /// we keep around a strong reference that is swept on a regular
        /// interval.
        /// </summary>
      
        private readonly FastThreadStore<WeakReference<DbConnection>> wdbConnection = 
            new FastThreadStore<WeakReference<DbConnection>>();

        /// <summary>
        /// Collects connections across threads and stores them in a strongly
        /// referenced table.  The table allows us to reuse connections to
        /// database that are continually accessed on the same thread.
        /// </summary>

        [NonSerialized]
        private static readonly Dictionary<DbConnection, long> sdbConnectionTable = new Dictionary<DbConnection, long>();

        /// <summary>
        /// Periodically removes unused connections from the sdbConnectionTable
        /// and allows them to be reclaimed.
        /// </summary>

        [NonSerialized]
        private static Timer releaseTimer = null;

        /// <summary>
        /// Releases the connections.
        /// </summary>
        /// <param name="userObject">The user object.</param>
        private static void ReleaseConnections(Object userObject)
        {
            long touchPoint = Environment.TickCount - 15000;

            lock (((ICollection) sdbConnectionTable).SyncRoot)
            {
                IList<DbConnection> termList = new List<DbConnection>();
                foreach( KeyValuePair<DbConnection, long> entry in sdbConnectionTable )
                {
                    if ( entry.Value < touchPoint )
                    {
                        termList.Add(entry.Key);
                    }
                }

                // Remove strong references to any databases that have not
                // been active since the touchPoint (expiry time).

                foreach( DbConnection termPoint in termList )
                {
                    sdbConnectionTable.Remove(termPoint);
                }
            }
        }


        /// <summary>
        /// Factory method that is used to create instance of a connection.
        /// </summary>
        /// <returns></returns>

        public abstract DbConnection CreateConnection();

        /// <summary>
        /// Creates a connection using the internal mechanism.  Avoids having
        /// to make CreateConnection protected internal.  Its primary use is
        /// by the BaseDbDriverCommand.
        /// </summary>
        /// <returns></returns>

        protected internal DbConnection CreateConnectionInternal()
        {
            DbConnection _dbConnection;
            WeakReference<DbConnection> dbConnection = wdbConnection.Value;
            if ((dbConnection == null) ||
                (dbConnection.IsDead) ||
                (dbConnection.Target == null))
            {
                _dbConnection = CreateConnection();
                dbConnection = new WeakReference<DbConnection>(_dbConnection);
                ApplyConnectionOptions(_dbConnection);
                wdbConnection.Value = dbConnection;
            } 
            else
            {
                _dbConnection = dbConnection.Target;
            }

            // Enter the time of the last activity on the database
            // connection... i.e. touch the connection.
            lock (((ICollection)sdbConnectionTable).SyncRoot)
            {
                sdbConnectionTable[_dbConnection] = Environment.TickCount;
                if (releaseTimer == null)
                {
                    releaseTimer = new Timer(ReleaseConnections, null, 0L, 5000L);
                }
            }

            return _dbConnection;
        }

        /// <summary>
        /// Sets the transaction isolation.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        protected virtual void SetTransactionIsolation( DbConnection connection, IsolationLevel? isolationLevel )
        {
			try
			{
				if (isolationLevel != null)
				{
                    // Begin a transaction to provide the proper isolation.  Need to ensure
                    // that the transaction is properly committed upon completion since we
                    // do not have auto-commit handled.
                    connection.BeginTransaction(isolationLevel.Value);
				}
			}
			catch (DbException ex)
			{
				throw new DatabaseConfigException(
                    "Error setting transaction isolation level to " + isolationLevel +
                    " on connection with detail " + GetDetail(ex), ex);
			}
        }

        /// <summary>
        /// Sets the catalog.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="catalog">The catalog.</param>
        protected virtual void SetCatalog( DbConnection connection, String catalog )
        {
            try
            {
                if (catalog != null)
                {
                    connection.ChangeDatabase(catalog);
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseConfigException(
                    "Error setting catalog to '" + catalog +
                    "' on connection with detail " + GetDetail(ex), ex);
            } 
        }

        /// <summary>
        /// Sets the automatic commits.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="useAutoCommit">The use auto commit.</param>
        protected virtual void SetAutoCommit( DbConnection connection, bool? useAutoCommit )
        {
            try
            {
                if (useAutoCommit ?? false)
                {
                    throw new NotSupportedException("AutoCommit semantics not yet supported in this version");
                }
            }
            catch (DbException ex)
            {
                throw new DatabaseConfigException(
                    "Error setting auto-commit to " + useAutoCommit +
                    " on connection with detail " + GetDetail(ex), ex);
            }
        }

        /// <summary> Method to set connection-level configuration settings.</summary>
		/// <param name="connection">is the connection to set on
		/// </param>
		/// <param name="connectionSettings">are the settings to apply
		/// </param>
		/// <throws>  DatabaseConfigException is thrown if an DbException is thrown </throws>
        protected virtual void ApplyConnectionOptions(DbConnection connection, ConnectionSettings connectionSettings)
		{
            SetTransactionIsolation(connection, connectionSettings.TransactionIsolation);
            SetCatalog(connection, connectionSettings.Catalog);
            SetAutoCommit(connection, connectionSettings.AutoCommit);
		}

        /// <summary>
        /// Sets the connection options using the default connection options.
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected virtual void ApplyConnectionOptions(DbConnection connection)
        {
        }

        /// <summary>
        /// Gets the detail.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static String GetDetail(DbException ex)
        {
            return
                "DbException: " + ex.Message +
                " VendorError: " + ex.ErrorCode;
        }

        #region DbDriver Members

        /// <summary>
        /// Creates a database driver command from a collection of fragments.
        /// </summary>
        /// <param name="sqlFragments">The SQL fragments.</param>
        /// <param name="metadataSettings">The metadata settings.</param>
        /// <returns></returns>
        public virtual DbDriverCommand CreateCommand(IEnumerable<PlaceholderParser.Fragment> sqlFragments,
                                                     ColumnSettings metadataSettings)
        {
            // How do we convert from positional to underlying
            PositionalToTextConverter paramConverter = ParamToTextConverter;
            // List of parameters
            List<string> parameters = new List<string>();
            // Command text builder
            StringBuilder buffer = new StringBuilder();
            // Counter for parameters
            int parameterCount = 0;
            foreach (PlaceholderParser.Fragment fragment in sqlFragments)
            {
                if (!fragment.IsParameter)
                {
                    buffer.Append(fragment.Value);
                }
                else if (fragment.Value == SAMPLE_WHERECLAUSE_PLACEHOLDER)
                {
                    continue;
                }
                else
                {
                    String parameter = paramConverter.Invoke(parameterCount++);
                    // Add the parameter to the parameter list
                    parameters.Add(parameter);
                    // Add the parameter to the command text
                    buffer.Append(parameter);
                }
            }

            String dbCommandText = buffer.ToString();
            return new BaseDbDriverCommand(
                this,
                sqlFragments,
                parameters,
                dbCommandText,
                metadataSettings);
        }

        /// <summary>
        /// Gets or sets the properties for the driver.
        /// </summary>
        /// <value>The properties.</value>
        public virtual Properties Properties
        {
            get { return connectionProperties; }
            set
            {
                connectionProperties = value;

                // Look for the term "connectionString" in the properties.  If it is not specified
                // then use the other items in the properties to drive a connection string builder.

                if (!connectionProperties.TryGetValue("connectionString", out connectionString) &&
                    !connectionProperties.TryGetValue("connection-string", out connectionString))
                {
                    // Create the connection string; to do so, we require a connection
                    // string builder.  These are native to every connection class and
                    // ADO.NET providers will provide them to you natively.  We require
                    // that the implementation class provide us with one of these.
                    DbConnectionStringBuilder builder = CreateConnectionStringBuilder();

                    foreach (KeyValuePair<String, String> entry in connectionProperties)
                    {
                        String ekey = entry.Key;
                        String evalue = entry.Value;
                        if (String.Equals(ekey, "name", StringComparison.CurrentCultureIgnoreCase))
                        {
                            name = evalue;
                        }
                        else
                        {
                            builder.Add(ekey, evalue);
                        }
                    }

                    connectionString = builder.ConnectionString;
                }
            }
        }

        /// <summary>
        /// Connection name
        /// </summary>
        public virtual string Name
        {
            get { return name; }
        }

        #endregion
    }
}
