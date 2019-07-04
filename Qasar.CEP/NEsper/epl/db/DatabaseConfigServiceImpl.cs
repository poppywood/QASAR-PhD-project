using System;
using System.Collections.Generic;

using com.espertech.esper.core;
using com.espertech.esper.compat;
using com.espertech.esper.client;
using com.espertech.esper.schedule;

namespace com.espertech.esper.epl.db
{
	/// <summary> Implementation provides database instance services such as connection factory and
	/// connection settings.
	/// </summary>
	
	public class DatabaseConfigServiceImpl : DatabaseConfigService
	{
        private readonly Map<String, ConfigurationDBRef> mapDatabaseRef;
		private readonly IDictionary<String, DatabaseConnectionFactory> connectionFactories;
		private readonly SchedulingService schedulingService;
		private readonly ScheduleBucket scheduleBucket;
		
		/// <summary> Ctor.</summary>
		/// <param name="mapDatabaseRef">is a map of database name and database configuration entries
		/// </param>
		/// <param name="schedulingService">is for scheduling callbacks for a cache
		/// </param>
		/// <param name="scheduleBucket">is a system bucket for all scheduling callbacks for caches
		/// </param>
        public DatabaseConfigServiceImpl(Map<String, ConfigurationDBRef> mapDatabaseRef, SchedulingService schedulingService, ScheduleBucket scheduleBucket)
        {
            this.mapDatabaseRef = mapDatabaseRef;
            this.connectionFactories = new Dictionary<String, DatabaseConnectionFactory>();
            this.schedulingService = schedulingService;
            this.scheduleBucket = scheduleBucket;
        }

        /// <summary>
        /// Returns true to indicate a setting to retain connections between lookups.
        /// </summary>
        /// <param name="databaseName">is the name of the database</param>
        /// <param name="preparedStatementText">is the sql text</param>
        /// <returns>
        /// a cache implementation to cache connection and prepared statements
        /// </returns>
        /// <throws>  DatabaseConfigException is thrown to indicate database configuration errors </throws>
		public virtual ConnectionCache GetConnectionCache(String databaseName, String preparedStatementText)
		{
            ConfigurationDBRef config;
            if ( ! mapDatabaseRef.TryGetValue(databaseName, out config ) )
			{
				throw new DatabaseConfigException("Cannot locate configuration information for database '" + databaseName + "'");
			}
			
			DatabaseConnectionFactory connectionFactory = GetConnectionFactory(databaseName);
			
			if ( config.ConnectionLifecycle == ConnectionLifecycleEnum.RETAIN)
			{
				return new ConnectionCacheImpl(connectionFactory, preparedStatementText);
			}
			else
			{
				return new ConnectionNoCacheImpl(connectionFactory, preparedStatementText);
			}
		}

        /// <summary>
        /// Returns a connection factory for a configured database.
        /// </summary>
        /// <param name="databaseName">is the name of the database</param>
        /// <returns>
        /// is a connection factory to use to get connections to the database
        /// </returns>
        /// <throws>  DatabaseConfigException is thrown to indicate database configuration errors </throws>
		public virtual DatabaseConnectionFactory GetConnectionFactory(String databaseName)
		{
			// check if we already have a reference
            DatabaseConnectionFactory factory;
            if ( connectionFactories.TryGetValue(databaseName, out factory ) )
			{
				return factory;
			}

            ConfigurationDBRef config;
            if ( ! mapDatabaseRef.TryGetValue( databaseName, out config ) )
			{
				throw new DatabaseConfigException("Cannot locate configuration information for database '" + databaseName + "'");
			}
			
			ConnectionSettings settings = config.ConnectionSettings;
			if (config.ConnectionFactoryDesc is DbDriverFactoryConnection)
			{
                DbDriverFactoryConnection dbConfig = (DbDriverFactoryConnection)config.ConnectionFactoryDesc;
				factory = new DatabaseDriverConnFactory(dbConfig, settings);
			}
			else
			{
                throw new NotSupportedException();
			}
			
			connectionFactories[databaseName] = factory;
			
			return factory;
		}

        /// <summary>
        /// Returns a new cache implementation for this database.
        /// </summary>
		/// <param name="databaseName">the name of the database to return a new cache implementation for for</param>
		/// <param name="epStatementHandle">the statements-own handle for use in registering callbacks with services</param>
        /// <returns>cache implementation</returns>
        /// <throws>  DatabaseConfigException is thrown to indicate database configuration errors </throws>
		public virtual DataCache GetDataCache(String databaseName, EPStatementHandle epStatementHandle)
		{
            ConfigurationDBRef config = null;
            if ( ! mapDatabaseRef.TryGetValue(databaseName, out config ) )
			{
				throw new DatabaseConfigException("Cannot locate configuration information for database '" + databaseName + "'");
			}

            ConfigurationDataCache dataCacheDesc = config.DataCacheDesc;
            return DataCacheFactory.GetDataCache(dataCacheDesc, epStatementHandle, schedulingService, scheduleBucket);
		}

        public ColumnSettings GetQuerySetting(String databaseName)
        {
            ConfigurationDBRef config = mapDatabaseRef.Get(databaseName);
            if (config == null)
            {
                throw new DatabaseConfigException("Cannot locate configuration information for database '" + databaseName + '\'');
            }
            return new ColumnSettings(
                config.MetadataRetrievalEnum,
                config.ColumnChangeCase,
                config.DataTypeMapping);
        }
	}
}
