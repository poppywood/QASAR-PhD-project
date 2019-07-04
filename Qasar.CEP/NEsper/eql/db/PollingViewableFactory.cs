using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;

using net.esper.client;
using net.esper.core;
using net.esper.compat;
using net.esper.eql.expression;
using net.esper.eql.spec;
using net.esper.events;
using net.esper.util;
using net.esper.view;

using org.apache.commons.logging;

namespace net.esper.eql.db
{
    /// <summary>
    /// Factory for a view onto historical data via SQL statement.
    /// </summary>

    public class PollingViewableFactory
    {
        /// <summary> Creates the viewable for polling via database SQL query.</summary>
        /// <param name="streamNumber">is the stream number of the view</param>
        /// <param name="databaseStreamSpec">provides the SQL statement, database name and additional info</param>
        /// <param name="databaseConfigService">for getting database connection and settings</param>
        /// <param name="eventAdapterService">for generating event beans from database information</param>
		/// <param name="epStatementHandle">the statements-own handle for use in registering callbacks with services</param>
        /// <returns>viewable providing poll functionality</returns>
        /// <throws>ExprValidationException if the validation failed </throws>

        public static HistoricalEventViewable CreateDBStatementView(
            int streamNumber,
            DBStatementStreamSpec databaseStreamSpec,
            DatabaseConfigService databaseConfigService,
            EventAdapterService eventAdapterService,
			EPStatementHandle epStatementHandle)
        {
            // Parse the SQL for placeholders and text fragments
            IList<PlaceholderParser.Fragment> sqlFragments = GetSqlFragments(databaseStreamSpec);

            // Get the database information
            String databaseName = databaseStreamSpec.DatabaseName;
            DatabaseConnectionFactory databaseConnectionFactory =
                GetDatabaseConnectionFactory(databaseConfigService, databaseName);
            ParameterModel parameterModel = databaseConnectionFactory.ParameterModel;

            // Assemble a DbCommand and parameter list
            String preparedStatementText = parameterModel.CreateDbCommand(sqlFragments);
            String pseudoStatementText = parameterModel.CreatePseudoCommand(sqlFragments);
            List<String> parameters = GetParameters(sqlFragments);
            List<string> inputParameters = parameters; // GetInputParameters(preparedStatementText, prepared);

            if (log.IsDebugEnabled)
            {
                log.Debug(
                    ".createDBEventStream preparedStatementText=" + preparedStatementText +
                    " parameters=" + CollectionHelper.Render(parameters));
            }

            // Get a database connection

            try
            {
                using (DbConnection connection = databaseConnectionFactory.Connection)
                {
                    try
                    {
                        using (DbCommand prepared = connection.CreateCommand())
                        {
                            prepared.CommandText = preparedStatementText;

                            parameterModel.CreateDbParameters(prepared, sqlFragments);

                            using (DbDataReader reader = prepared.ExecuteReader(CommandBehavior.SchemaOnly))
                            {
                                IDictionary<string, DBOutputTypeDesc> outputProperties = GetOutputProperties(preparedStatementText, reader);

                                if (log.IsDebugEnabled)
                                {
                                    log.Debug(
                                        ".createDBEventStream" +
                                        " in=" + CollectionHelper.Render(inputParameters) +
                                        " out=" + CollectionHelper.Render(outputProperties));
                                }

                                // Create event type
                                // Construct an event type from SQL query result metadata
                                EDictionary<String, Type> eventTypeFields = new HashDictionary<String, Type>();
                                foreach (KeyValuePair<string, DBOutputTypeDesc> entry in outputProperties)
                                {
                                    String name = entry.Key;
                                    DBOutputTypeDesc dbOutputDesc = entry.Value;
                                    Type clazz = TypeHelper.GetBoxedType(dbOutputDesc.DataType);
                                    eventTypeFields[name] = clazz;
                                }

                                EventType eventType = eventAdapterService.CreateAnonymousMapType(eventTypeFields);

                                // Get a proper connection and data cache
                                ConnectionCache connectionCache = null;
                                DataCache dataCache = null;
                                try
                                {
                                    connectionCache = databaseConfigService.GetConnectionCache(databaseName, pseudoStatementText);
                                    dataCache = databaseConfigService.GetDataCache(databaseName, epStatementHandle);
                                }
                                catch (DatabaseConfigException e)
                                {
                                    String text = "Error obtaining cache configuration";
                                    log.Error(text, e);
                                    throw new ExprValidationException(text + ", reason: " + e.Message);
                                }

                                PollExecStrategyDBQuery dbPollStrategy = new PollExecStrategyDBQuery(eventAdapterService, eventType, connectionCache, preparedStatementText, outputProperties);

                                return new PollingViewable(streamNumber, inputParameters, dbPollStrategy, dataCache, eventType);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        String text = "Error executing statement '" + preparedStatementText + "'";
                        log.Error(text, ex);
                        throw new ExprValidationException(text + ", reason: " + ex.Message);
                    }
                }
            }
            catch (ExprValidationException)
            {
                throw; // inner exception
            }
            catch (Exception ex)
            {
                String text = "Error connecting to database '" + databaseName + "'";
                log.Error(text, ex);
                throw new ExprValidationException(text + ", reason: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the SQL fragments.
        /// </summary>
        /// <param name="databaseStreamSpec">The database stream spec.</param>
        /// <returns></returns>
        private static IList<PlaceholderParser.Fragment> GetSqlFragments(DBStatementStreamSpec databaseStreamSpec)
        {
            IList<PlaceholderParser.Fragment> sqlFragments;
            try
            {
                sqlFragments = PlaceholderParser.ParsePlaceholder(databaseStreamSpec.SqlWithSubsParams);
            }
            catch (PlaceholderParseException ex)
            {
                String text = "Error parsing SQL";
                throw new ExprValidationException(text + ", reason: " + ex.Message);
            }
            return sqlFragments;
        }

        /// <summary>
        /// Gets the output properties.
        /// </summary>
        /// <param name="preparedStatementText">The prepared statement text.</param>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private static IDictionary<string, DBOutputTypeDesc> GetOutputProperties(string preparedStatementText, DbDataReader reader)
        {
            IDictionary<String, DBOutputTypeDesc> outputProperties = new Dictionary<String, DBOutputTypeDesc>();
            try
            {
                DataTable dataTable = reader.GetSchemaTable();
                foreach( DataRow dataRow in dataTable.Rows )
                {
                    Type columnType = (Type) dataRow["DataType"];
                    Int32 columnSize = (Int32) dataRow["ColumnSize"];
                    String columnName = (String)dataRow["ColumnName"];
                    Int32 providerType = (Int32)dataRow["ProviderType"];
                    String sqlTypeName = Convert.ToString(providerType);

                    // Some providers (read MySQL) provide booleans as an integer
                    // with a size of 1.  We should probably convert these to bool
                    // to make clien integration easier.
                    if ((columnType == typeof(sbyte)) && (columnSize == 1))
                    {
                        columnType = typeof(bool);
                    }

                    outputProperties[columnName] = new DBOutputTypeDesc(sqlTypeName, columnType);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                String text = "Error in statement '" + preparedStatementText + "', failed to obtain result metadata";
                log.Error(text, ex);
                throw new ExprValidationException(text + ", please check the statement, reason: " + ex.Message);
            }
            return outputProperties;
        }

        /// <summary>
        /// Gets the input parameters.
        /// </summary>
        /// <param name="preparedStatementText">The prepared statement text.</param>
        /// <param name="prepared">The prepared.</param>
        /// <returns></returns>
        private static IList<string> GetInputParameters(string preparedStatementText, DbCommand prepared)
        {
            IList<String> inputParameters = new List<String>();
            try
            {
                DbParameterCollection parameterMetaData = prepared.Parameters;
                foreach (DbParameter parameter in parameterMetaData)
                {
                    inputParameters.Add(parameter.ParameterName);
                }
            }
            catch (Exception ex)
            {
                String text = "Error obtaining parameter metadata from prepared statement '" + preparedStatementText + "'";
                log.Error(text, ex);
                throw new ExprValidationException(text + ", please check the statement, reason: " + ex.Message);
            }
            return inputParameters;
        }

        /// <summary>
        /// Gets the database connection factory.
        /// </summary>
        /// <param name="databaseConfigService">The database config service.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <returns></returns>
        private static DatabaseConnectionFactory GetDatabaseConnectionFactory(DatabaseConfigService databaseConfigService, string databaseName)
        {
            DatabaseConnectionFactory databaseConnectionFactory;
            try
            {
                databaseConnectionFactory = databaseConfigService.GetConnectionFactory(databaseName);
            }
            catch (DatabaseConfigException ex)
            {
                String text = "Error connecting to database '" + databaseName + "'";
                log.Error(text, ex);
                throw new ExprValidationException(text + ", reason: " + ex.Message);
            }
            return databaseConnectionFactory;
        }

        private static List<String> GetParameters(IEnumerable<PlaceholderParser.Fragment> parseFragements)
        {
            List<String> eventPropertyParams = new List<String>();
            foreach (PlaceholderParser.Fragment fragment in parseFragements)
            {
                if (fragment.IsParameter)
                {
                	eventPropertyParams.Add(fragment.Value) ;
                }
            }

            return eventPropertyParams;
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
