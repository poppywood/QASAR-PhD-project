using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using com.espertech.esper.collection;
using com.espertech.esper.client;
using com.espertech.esper.epl.expression;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.db
{
    /// <summary>
    /// Base class for a Connection and DbCommand cache.
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
        private readonly IList<PlaceholderParser.Fragment> sqlFragments;

        /// <summary> Returns a cached or new connection and statement pair.</summary>
        /// <returns> connection and statement pair
        /// </returns>
        public abstract Pair<DbDriver, DbDriverCommand> GetConnection();

        /// <summary> Indicate to return the connection and statement pair after use.</summary>
        /// <param name="pair">is the resources to return
        /// </param>
        public abstract void DoneWith(Pair<DbDriver, DbDriverCommand> pair);

        /// <summary> Destroys cache closing all resources cached, if any.</summary>
        public abstract void Destroy();

        /// <summary> Ctor.</summary>
        /// <param name="databaseConnectionFactory">connection factory</param>
        /// <param name="sql">statement sql</param>

        internal ConnectionCache(DatabaseConnectionFactory databaseConnectionFactory, String sql)
        {
            this.databaseConnectionFactory = databaseConnectionFactory;
            this.sql = sql;
            this.sqlFragments = GetSqlFragments(sql);
        }

        /// <summary> Close resources.</summary>
        /// <param name="pair">is the resources to close.
        /// </param>

        protected static void Close(Pair<DbDriver, DbDriverCommand> pair)
        {
            log.Info(".Close Closing statement and connection");
            try
            {
                pair.Second.Dispose();
            }
            catch (DbException ex)
            {
                throw new EPException("Error closing statement", ex);
            }
        }

        /// <summary> Make a new pair of resources.</summary>
        /// <returns> pair of resources
        /// </returns>
        
        protected Pair<DbDriver, DbDriverCommand> MakeNew()
        {
            log.Info(".MakeNew Obtaining new connection and statement");

            try
            {
                // Get the driver
                DbDriver dbDriver = databaseConnectionFactory.Driver;
                // Get the command
                DbDriverCommand dbCommand = dbDriver.CreateCommand(sqlFragments, null);

                return new Pair<DbDriver, DbDriverCommand>(dbDriver, dbCommand);
            }
            catch (DatabaseConfigException ex)
            {
                throw new EPException("Error obtaining connection", ex);
            }

        }

        /// <summary>
        /// Gets the SQL fragments.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public static IList<PlaceholderParser.Fragment> GetSqlFragments(string sql)
        {
            List<PlaceholderParser.Fragment> sqlFragments = new List<PlaceholderParser.Fragment>();

            bool inQuote = false;
            bool inEscape = false;
            // Keeps track of the current token
            StringBuilder currToken = new StringBuilder();
            String token;

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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        
        public void Dispose()
        {
            Destroy() ;
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
