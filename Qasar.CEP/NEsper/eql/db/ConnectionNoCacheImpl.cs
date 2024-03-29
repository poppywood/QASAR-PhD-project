using System;
using System.Data.Common;

using net.esper.collection;

namespace net.esper.eql.db
{
	
	/// <summary>
    /// Implementation of a connection cache that simply doesn't cache but gets
	/// a new connection and statement every request, and closes these every time
	/// a client indicates done.
	/// </summary>
    
    public class ConnectionNoCacheImpl : ConnectionCache
    {
        /// <summary> Ctor.</summary>
        /// <param name="databaseConnectionFactory">is the connection factory
        /// </param>
        /// <param name="sql">is the statement sql
        /// </param>
        public ConnectionNoCacheImpl(DatabaseConnectionFactory databaseConnectionFactory, String sql)
            : base(databaseConnectionFactory, sql)
        {
        }

        /// <summary>
        /// Returns a cached or new connection and statement pair.
        /// </summary>
        /// <returns>connection and statement pair</returns>
        public override Pair<DbConnection, DbCommand> GetConnection()
        {
            return MakeNew();
        }

        /// <summary>
        /// Indicate to return the connection and statement pair after use.
        /// </summary>
        /// <param name="pair">is the resources to return</param>
        public override void DoneWith(Pair<DbConnection, DbCommand> pair)
        {
            Close(pair);
        }

        /// <summary>
        /// Destroys cache closing all resources cached, if any.
        /// </summary>
        public override void Destroy()
        {
            // no resources held
        }
    }
}
