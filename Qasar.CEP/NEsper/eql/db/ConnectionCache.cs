using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using net.esper.collection;
using net.esper.client;
using net.esper.eql.expression;
using net.esper.util;

using Log = org.apache.commons.logging.Log;
using LogFactory = org.apache.commons.logging.LogFactory;

namespace net.esper.eql.db
{
    /// <summary> Base class for a Connection and DbCommand cache.
    /// <para>
    /// Implementations control the lifecycle via lifecycle methods, or
    /// may simple obtain new resources and close new resources every time.
    /// </para>
    /// <para>
    /// This is not a pool - a cache is associated with one client class and that
    /// class is expected to use cache methods in well-defined order of get, done-with and destroy.
    /// </para>
    /// </summary>
    public abstract class ConnectionCache : IDisposable
    {
        private readonly DatabaseConnectionFactory databaseConnectionFactory;
        private readonly String sql;

        /// <summary> Returns a cached or new connection and statement pair.</summary>
        /// <returns> connection and statement pair
        /// </returns>
        public abstract Pair<DbConnection, DbCommand> GetConnection();

        /// <summary> Indicate to return the connection and statement pair after use.</summary>
        /// <param name="pair">is the resources to return
        /// </param>
        public abstract void DoneWith(Pair<DbConnection, DbCommand> pair);

        /// <summary> Destroys cache closing all resources cached, if any.</summary>
        public abstract void Destroy();

        /// <summary> Ctor.</summary>
        /// <param name="databaseConnectionFactory">connection factory</param>
        /// <param name="sql">statement sql</param>

        internal ConnectionCache(DatabaseConnectionFactory databaseConnectionFactory, String sql)
        {
            this.databaseConnectionFactory = databaseConnectionFactory;
            this.sql = sql;
        }

        /// <summary> Close resources.</summary>
        /// <param name="pair">is the resources to close.
        /// </param>
        
        protected static void Close(Pair<DbConnection, DbCommand> pair)
        {
            log.Info(".close Closing statement and connection");
            try
            {
                pair.Second.Dispose();
            }
            catch (DbException ex)
            {
                try
                {
					pair.First.Close();
                }
                catch (DbException)
                {
                }

                throw new EPException("Error closing statement", ex);
            }

            try
            {
                pair.First.Close();
            }
            catch (DbException ex)
            {
                throw new EPException("Error closing statement", ex);
            }
        }

        /// <summary> Make a new pair of resources.</summary>
        /// <returns> pair of resources
        /// </returns>
        
        protected Pair<DbConnection, DbCommand> MakeNew()
        {
            log.Info(".makeNew Obtaining new connection and statement");
            DbConnection connection = null;
            try
            {
                connection = databaseConnectionFactory.Connection;
            }
            catch (DatabaseConfigException ex)
            {
                throw new EPException("Error obtaining connection", ex);
            }

            // Get the parameter model used for this connection
            ParameterModel parameterModel = databaseConnectionFactory.ParameterModel;
            // Get the SQL fragments from the raw SQL
            IList<PlaceholderParser.Fragment> sqlFragments = GetSqlFragments(sql);
            // Create the true SQL that is needed for this connection
            String preparedStatementText = parameterModel.CreateDbCommand(sqlFragments);

            DbCommand preparedStatement = null;
            try
            {
                preparedStatement = connection.CreateCommand() ;
                preparedStatement.CommandText = preparedStatementText;
                preparedStatement.Prepare() ;
                // Create the 'right' parameters
                parameterModel.CreateDbParameters(preparedStatement, sqlFragments);
            }
            catch (DbException ex)
            {
                throw new EPException("Error preparing statement '" + preparedStatementText + "'", ex);
            }

            return new Pair<DbConnection, DbCommand>(connection, preparedStatement);
        }

        /// <summary>
        /// Gets the SQL fragments.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        private static IList<PlaceholderParser.Fragment> GetSqlFragments(string sql)
        {
            List<PlaceholderParser.Fragment> sqlFragments = new List<PlaceholderParser.Fragment>();

            bool inQuote = false;
            bool inEscape = false;
            // Keeps track of the current token
            StringBuilder currToken = new StringBuilder();
            String token = null;

            foreach( char ch in sql )
            {
                switch( ch )
                {
                    case '\\':
                        inEscape = !inEscape; // Addresses the case of double backslash
                        currToken.Append(ch);
                        break;
                    case '"':
                    case '\'':
                        if (!inEscape)
                        {
                            inQuote = !inQuote;
                        }
                        currToken.Append(ch);
                        break;
                    case '?':
                        if (!inEscape && !inQuote)
                        {
                            // Get the current token
                            token = currToken.ToString();
                            // Reset the current token
                            currToken.Length = 0;
                            // Add the current token to the sql fragments if it has any value
                            if (! String.IsNullOrEmpty(token))
                            {
                                sqlFragments.Add(new PlaceholderParser.TextFragment(token));
                            }
                            sqlFragments.Add(new PlaceholderParser.ParameterFragment(null));
                        }
                        break;
                    default:
                        inEscape = false;
                        currToken.Append(ch);
                        break;
                }
            }

            // Get the current token
            token = currToken.ToString();
            // Add the current token to the sql fragments if it has any value
            if (!String.IsNullOrEmpty(token))
            {
                sqlFragments.Add(new PlaceholderParser.TextFragment(token));
            }

            // Return the fragments
            return sqlFragments;
        }


        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        
        public void Dispose()
        {
            Destroy() ;
        }
    }
}
