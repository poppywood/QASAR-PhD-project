using System;
using System.Data.Common;
using System.Configuration;

using com.espertech.esper.client;

using com.espertech.esper.epl.db.drivers;

namespace com.espertech.esper.epl.db
{
	/// <summary>
	/// Database connection factory using DbProviderFactory to obtain connections.
	/// </summary>

    public class DatabaseDriverConnFactory : DatabaseConnectionFactory
    {
        private readonly DbDriver dbDriver;
        private readonly ConnectionSettings connectionSettings;

        /// <summary>
        /// Gets the database driver.
        /// </summary>
        /// <value></value>
        public virtual DbDriver Driver
        {
            get { return dbDriver; }
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="dbConfig">is the database provider configuration</param>
        /// <param name="connectionSettings">are connection-level settings</param>
        /// <throws>  DatabaseConfigException thrown if the driver class cannot be loaded </throws>

        public DatabaseDriverConnFactory(DbDriverFactoryConnection dbConfig, ConnectionSettings connectionSettings)
        {
            this.dbDriver = dbConfig.Driver;
            this.connectionSettings = connectionSettings;
        }
    }
}
