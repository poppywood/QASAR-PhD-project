using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;

using net.esper.client;
using net.esper.compat;
using net.esper.collection;
using net.esper.events;

namespace net.esper.eql.db
{
    /// <summary>
    /// Viewable providing historical data from a database.
    /// </summary>

    public class PollExecStrategyDBQuery : PollExecStrategy
    {
        private readonly EventAdapterService eventAdapterService;
        private readonly String preparedStatementText;
        private readonly IDictionary<String, DBOutputTypeDesc> outputTypes;
        private readonly ConnectionCache connectionCache;
        private readonly EventType eventType;

        private Pair<DbConnection, DbCommand> resources;

        /// <summary> Ctor.</summary>
        /// <param name="eventAdapterService">for generating event beans</param>
        /// <param name="eventType">is the event type that this poll generates</param>
        /// <param name="connectionCache">caches Connection and DbCommand</param>
        /// <param name="preparedStatementText">is the SQL to use for polling</param>
        /// <param name="outputTypes">describe columns selected by the SQL</param>
        public PollExecStrategyDBQuery(EventAdapterService eventAdapterService,
                                       EventType eventType,
                                       ConnectionCache connectionCache,
                                       String preparedStatementText,
                                       IDictionary<String, DBOutputTypeDesc> outputTypes)
        {
            this.eventAdapterService = eventAdapterService;
            this.eventType = eventType;
            this.connectionCache = connectionCache;
            this.preparedStatementText = preparedStatementText;
            this.outputTypes = outputTypes;
        }

        /// <summary>
        /// Start the poll, called before any poll operation.
        /// </summary>
        public virtual void Start()
        {
            resources = connectionCache.GetConnection();
        }

        /// <summary>
        /// Indicate we are done polling and can release resources.
        /// </summary>
        public virtual void Done()
        {
            connectionCache.DoneWith(resources);
        }

        /// <summary>
        /// Indicate we are no going to use this object again.
        /// </summary>
        public virtual void Destroy()
        {
            connectionCache.Destroy();
        }

        /// <summary>
        /// Poll events using the keys provided.
        /// </summary>
        /// <param name="lookupValues">is keys for exeuting a query or such</param>
        /// <returns>a list of events for the keys</returns>
        public IList<EventBean> Poll(Object[] lookupValues)
        {
            IList<EventBean> result = null;
            try
            {
                result = Execute(resources.Second, lookupValues);
            }
            catch (EPException)
            {
                connectionCache.DoneWith(resources);
                throw;
            }

            return result;
        }

        private List<DbInfo> dbInfoList = null;

        private IList<EventBean> Execute(DbCommand preparedStatement, Object[] lookupValuePerStream)
        {
        	int dbParamCount = preparedStatement.Parameters.Count;
        	if ( dbParamCount != lookupValuePerStream.Length )
        	{
        	    throw new ArgumentException("Only those parameters that have been prepared may be used here");
        	}

            DbParameter dbParam;

            // set parameters
            for (int i = 0; i < lookupValuePerStream.Length; i++)
            {
                try
                {
                	dbParam = preparedStatement.Parameters[i];
                    dbParam.Value = lookupValuePerStream[i];
                }
                catch (DbException ex)
                {
                    throw new EPException("Error setting parameter " + i, ex);
                }
            }

            // execute
            try
            {
                // generate events for result set
                IList<EventBean> rows = new List<EventBean>();

                using (DbDataReader dataReader = preparedStatement.ExecuteReader())
                {
                    try
                    {
                        if (dataReader.HasRows)
                        {
                            // Determine how many fields we will be receiving
                            int fieldCount = dataReader.FieldCount;
                            // Allocate a buffer to hold the results of the row
                            Object[] rawData = new object[fieldCount];
                            // Convert the names of columns into ordinal indices and prepare
                            // them so that we only have to incur this cost when we first notice
                            // the reader has rows.
                            if ( dbInfoList == null )
                            {
                                dbInfoList = new List<DbInfo>();
                                foreach (KeyValuePair<String, DBOutputTypeDesc> entry in outputTypes)
                                {
                                    DbInfo dbInfo = new DbInfo();
                                    dbInfo.name = entry.Key;
                                    dbInfo.ordinal = dataReader.GetOrdinal(dbInfo.name);
                                    dbInfo.dbOutputTypeDesc = entry.Value;
                                    dbInfoList.Add(dbInfo);
                                }
                            }
                            // Anyone know if the ordinal will always be the same every time
                            // the query is executed; if so, we could certainly cache this
                            // dbInfoList so that we only have to do that once for the lifetime
                            // of the statement.
                            while (dataReader.Read())
                            {
                                DataDictionary row = new DataDictionary();
                                // Get all of the values for the row in one shot
                                dataReader.GetValues(rawData);
                                // Convert the items into raw row objects
                                foreach (DbInfo dbInfo in dbInfoList)
                                {
                                    Object value = rawData[dbInfo.ordinal];
                                    if ( value == DBNull.Value )
                                    {
                                        value = null;
                                    }
                                    else if ( value.GetType() != dbInfo.dbOutputTypeDesc.DataType )
                                    {
                                        value = Convert.ChangeType(value, dbInfo.dbOutputTypeDesc.DataType);
                                    }

                                    row[dbInfo.name] = value;
                                }

                                EventBean eventBeanRow = eventAdapterService.CreateMapFromValues(row, eventType);
                                rows.Add(eventBeanRow);
                            }
                        }
                    }
                    catch (DbException ex)
                    {
                        throw new EPException("Error reading results for statement '" + preparedStatementText + "'", ex);
                    }
                }

                return rows;
            }
            catch (DbException ex)
            {
                throw new EPException("Error executing statement '" + preparedStatementText + "'", ex);
            }
        }

        struct DbInfo
        {
            public string name;
            public int ordinal;
            public DBOutputTypeDesc dbOutputTypeDesc;
        }
    }
}
