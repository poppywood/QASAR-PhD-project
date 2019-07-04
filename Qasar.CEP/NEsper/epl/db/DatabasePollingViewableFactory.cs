using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Antlr.Runtime;

using com.espertech.esper.antlr;
using com.espertech.esper.client;
using com.espertech.esper.core;
using com.espertech.esper.compat;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.generated;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.util;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.epl.db
{
    /// <summary>
    /// Factory for a view onto historical data via SQL statement.
    /// </summary>

    public class DatabasePollingViewableFactory
    {
        private const String SAMPLE_WHERECLAUSE_PLACEHOLDER = "$ESPER-SAMPLE-WHERE";

        /// <summary> Creates the viewable for polling via database SQL query.</summary>
        /// <param name="streamNumber">is the stream number of the view</param>
        /// <param name="databaseStreamSpec">provides the SQL statement, database name and additional info</param>
        /// <param name="databaseConfigService">for getting database connection and settings</param>
        /// <param name="eventAdapterService">for generating event beans from database information</param>
        /// <param name="epStatementHandle">the statements-own handle for use in registering callbacks with services</param>
        /// <returns>viewable providing poll functionality</returns>
        /// <exception cref="ExprValidationException">the validation failed</exception>
        public static HistoricalEventViewable CreateDBStatementView(
            int streamNumber,
            DBStatementStreamSpec databaseStreamSpec,
            DatabaseConfigService databaseConfigService,
            EventAdapterService eventAdapterService,
            EPStatementHandle epStatementHandle)
        {
            // Parse the SQL for placeholders and text fragments
            IList<PlaceholderParser.Fragment> sqlFragments = GetSqlFragments(databaseStreamSpec);
            IList<String> invocationInputParameters = new List<string>();
            foreach( PlaceholderParser.Fragment fragment in sqlFragments )
            {
                if ((fragment.IsParameter) && (fragment.Value != SAMPLE_WHERECLAUSE_PLACEHOLDER))
                {
                    invocationInputParameters.Add(fragment.Value);
                }
            }

            // Get the database information
            String databaseName = databaseStreamSpec.DatabaseName;
            DbDriver dbDriver = GetDatabaseConnectionFactory(databaseConfigService, databaseName).Driver;
            DbDriverCommand dbCommand = dbDriver.CreateCommand(sqlFragments, GetMetaDataSettings(databaseConfigService, databaseName));

            if (log.IsDebugEnabled)
            {
                log.Debug(".CreateDBStatementView dbCommand=" + dbCommand);
            }

            QueryMetaData queryMetaData = GetQueryMetaData(
                databaseStreamSpec,
                databaseConfigService,
                dbCommand);

            // Construct an event type from SQL query result metadata
            EventType eventType = CreateEventType(queryMetaData, eventAdapterService);

            // Get a proper connection and data cache
            ConnectionCache connectionCache;
            DataCache dataCache;
            try
            {
                connectionCache = databaseConfigService.GetConnectionCache(databaseName, dbCommand.PseudoText);
                dataCache = databaseConfigService.GetDataCache(databaseName, epStatementHandle);
            }
            catch (DatabaseConfigException e)
            {
                String text = "Error obtaining cache configuration";
                log.Error(text, e);
                throw new ExprValidationException(text + ", reason: " + e.Message);
            }

            PollExecStrategyDBQuery dbPollStrategy = new PollExecStrategyDBQuery(
                eventAdapterService,
                eventType,
                connectionCache,
                dbCommand.CommandText,
                queryMetaData.OutputParameters);

            return new DatabasePollingViewable(
                streamNumber,
                invocationInputParameters,
                dbPollStrategy,
                dataCache,
                eventType);
        }

        /// <summary>
        /// Gets the meta data settings from the database configuration service for the specified
        /// database name.
        /// </summary>
        /// <param name="databaseConfigService"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>

        private static ColumnSettings GetMetaDataSettings(DatabaseConfigService databaseConfigService,
                                                          String databaseName)
        {
            try
            {
                return databaseConfigService.GetQuerySetting(databaseName);
            }
            catch (DatabaseConfigException ex)
            {
                String text = "Error connecting to database '" + databaseName + '\'';
                log.Error(text, ex);
                throw new ExprValidationException(text + ", reason: " + ex.Message);
            }
        }

        /// <summary>
        /// Creates an event type from the query meta data.
        /// </summary>
        /// <param name="queryMetaData">The query meta data.</param>
        /// <param name="eventAdapterService">The event adapter service.</param>
        /// <returns></returns>
        private static EventType CreateEventType(QueryMetaData queryMetaData, EventAdapterService eventAdapterService)
        {
            Map<String, Object> eventTypeFields = new HashMap<String, Object>();
            foreach (KeyValuePair<String, DBOutputTypeDesc> entry in queryMetaData.OutputParameters)
            {
                String name = entry.Key;
                DBOutputTypeDesc dbOutputDesc = entry.Value;

                Type clazz;
                if (dbOutputDesc.OptionalBinding != null)
                {
                    clazz = dbOutputDesc.OptionalBinding.DataType;
                }
                else
                {
                    clazz = dbOutputDesc.DataType;
                }

                eventTypeFields[name] = clazz;
            }

            return eventAdapterService.CreateAnonymousMapType(eventTypeFields);
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
        /// Gets the query meta data.
        /// </summary>
        /// <param name="databaseStreamSpec">The database stream spec.</param>
        /// <param name="databaseConfigService">The database config service.</param>
        /// <param name="dbCommand">The database command.</param>
        /// <returns></returns>
        private static QueryMetaData GetQueryMetaData(
            DBStatementStreamSpec databaseStreamSpec,
            DatabaseConfigService databaseConfigService,
            DbDriverCommand dbCommand)
        {
            // Get a database connection
            String databaseName = databaseStreamSpec.DatabaseName;
            //DatabaseConnectionFactory databaseConnectionFactory = GetDatabaseConnectionFactory(databaseConfigService, databaseName);
            ColumnSettings metadataSetting = dbCommand.MetaDataSettings;

            QueryMetaData queryMetaData;
            try
            {
                // On default setting, if we detect Oracle in the connection then don't query metadata from prepared statement
                ConfigurationDBRef.MetadataOriginEnum metaOriginPolicy = metadataSetting.MetadataRetrievalEnum;
                if (metaOriginPolicy == ConfigurationDBRef.MetadataOriginEnum.DEFAULT)
                {
                    // Ask the driver how it interprets the default meta origin policy; the
                    // esper code has a specific hook for Oracle.  We have moved this into
                    // the driver to avoid specifically coding behavior to a driver.
                    metaOriginPolicy = dbCommand.Driver.DefaultMetaOriginPolicy;
                }

                switch( metaOriginPolicy )
                {
                    case ConfigurationDBRef.MetadataOriginEnum.METADATA:
                    case ConfigurationDBRef.MetadataOriginEnum.DEFAULT:
                        queryMetaData = dbCommand.MetaData;
                        break;
                    case ConfigurationDBRef.MetadataOriginEnum.SAMPLE:
                        {
                            SQLParameterDesc parameterDesc = dbCommand.ParameterDescription;

                            String sampleSQL;
                            if (databaseStreamSpec.MetadataSQL != null)
                            {
                                sampleSQL = databaseStreamSpec.MetadataSQL;
                                if (log.IsInfoEnabled)
                                {
                                    log.Info(".GetQueryMetaData Using provided sample SQL '" + sampleSQL + "'");
                                }
                            }
                            else
                            {
                                // Create the sample SQL by replacing placeholders with null and
                                // SAMPLE_WHERECLAUSE_PLACEHOLDER with a "where 1=0" clause
                                sampleSQL = CreateSamplePlaceholderStatement(dbCommand.Fragments);

                                if (log.IsInfoEnabled)
                                {
                                    log.Info(".GetQueryMetaData Using un-lexed sample SQL '" + sampleSQL + "'");
                                }

                                // If there is no SAMPLE_WHERECLAUSE_PLACEHOLDER, lexical analyse the SQL
                                // adding a "where 1=0" clause.
                                if (parameterDesc.BuiltinIdentifiers.Count != 1)
                                {
                                    sampleSQL = LexSampleSQL(sampleSQL);
                                    if (log.IsInfoEnabled)
                                    {
                                        log.Info(".GetQueryMetaData Using lexed sample SQL '" + sampleSQL + "'");
                                    }
                                }
                            }

                            // finally get the metadata by firing the sample SQL
                            queryMetaData = GetExampleQueryMetaData(dbCommand.Driver, sampleSQL, metadataSetting);
                        }
                        break;
                    default:
                        throw new ArgumentException("MetaOriginPolicy contained an unhandled value: #" + metaOriginPolicy);
                }
            }
            catch (DatabaseConfigException ex)
            {
                String text = "Error connecting to database '" + databaseName + '\'';
                log.Error(text, ex);
                throw new ExprValidationException(text + ", reason: " + ex.Message);
            }

            return queryMetaData;
        }

        /// <summary>
        /// Lexes the sample SQL and inserts a "where 1=0" where-clause.
        /// </summary>
        /// <param name="querySQL">to inspect using lexer</param>
        /// <returns>sample SQL with where-clause inserted</returns>
        /// <exception cref="ExprValidationException">indicates a lexer problem</exception>
        public static String LexSampleSQL(String querySQL)
        {
            ICharStream input;
            try
            {
                input = new NoCaseSensitiveStream(querySQL);
            }
            catch (IOException ex)
            {
                throw new ExprValidationException("IOException lexing query SQL '" + querySQL + '\'', ex);
            }

            int whereIndex = -1;
            int groupbyIndex = -1;
            int havingIndex = -1;
            int orderByIndex = -1;
            List<int> unionIndexes = new List<int>();

            EsperEPL2GrammarLexer lex = new EsperEPL2GrammarLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lex);
            IList tokenList = tokens.GetTokens();

            for (int i = 0; i < tokenList.Count; i++)
            {
                IToken token = (IToken)tokenList[i];
                if ((token == null) || token.Text == null)
                {
                    break;
                }
                String text = token.Text.ToLower().Trim();
                switch (text) {
                    case "where":
                        whereIndex = token.CharPositionInLine + 1;
                        break;
                    case "group":
                        groupbyIndex = token.CharPositionInLine + 1;
                        break;
                    case "having":
                        havingIndex = token.CharPositionInLine + 1;
                        break;
                    case "order":
                        orderByIndex = token.CharPositionInLine + 1;
                        break;
                    case "union":
                        unionIndexes.Add(token.CharPositionInLine + 1);
                        break;
                }
            }

            // If we have a union, break string into subselects and process each
            if (unionIndexes.Count != 0)
            {
                String fragment;
                String lexedFragment;
                StringBuilder changedSQL = new StringBuilder();
                int lastIndex = 0;
                for (int i = 0; i < unionIndexes.Count; i++)
                {
                    int index = unionIndexes[i];
                    if (i > 0)
                    {
                        fragment = querySQL.Substring(lastIndex + 5, index - 6 - lastIndex);
                    }
                    else
                    {
                        fragment = querySQL.Substring(lastIndex, index - 1 - lastIndex);
                    }

                    lexedFragment = LexSampleSQL(fragment);

                    if (i > 0)
                    {
                        changedSQL.Append("union ");
                    }
                    changedSQL.Append(lexedFragment);
                    lastIndex = index - 1;
                }

                // last part after last union
                fragment = querySQL.Substring(lastIndex + 5);
                lexedFragment = LexSampleSQL(fragment);
                changedSQL.Append("union ");
                changedSQL.Append(lexedFragment);

                return changedSQL.ToString();
            }

            // Found a where clause, simplest cases
            if (whereIndex != -1)
            {
                StringBuilder changedSQL = new StringBuilder();
                String prefix = querySQL.Substring(0, whereIndex + 5);
                String suffix = querySQL.Substring(whereIndex + 5);
                changedSQL.Append(prefix);
                changedSQL.Append("1=0 and ");
                changedSQL.Append(suffix);
                return changedSQL.ToString();
            }

            // No where clause, find group-by
            int insertIndex;
            if (groupbyIndex != -1)
            {
                insertIndex = groupbyIndex;
            }
            else if (havingIndex != -1)
            {
                insertIndex = havingIndex;
            }
            else if (orderByIndex != -1)
            {
                insertIndex = orderByIndex;
            }
            else
            {
                StringBuilder changedSQL = new StringBuilder();
                changedSQL.Append(querySQL);
                changedSQL.Append(" where 1=0 ");
                return changedSQL.ToString();
            }

            try
            {
                StringBuilder changedSQL = new StringBuilder();
                String prefix = querySQL.Substring(0, insertIndex - 1);
                changedSQL.Append(prefix);
                changedSQL.Append("where 1=0 ");
                String suffix = querySQL.Substring(insertIndex - 1);
                changedSQL.Append(suffix);
                return changedSQL.ToString();
            }
            catch (Exception ex)
            {
                String text =
                    "Error constructing sample SQL to retrieve metadata for JDBC-drivers that don't support metadata, consider using the " +
                    SAMPLE_WHERECLAUSE_PLACEHOLDER + " placeholder or providing a sample SQL";
                log.Error(text, ex);
                throw new ExprValidationException(text, ex);
            }
        }

        /// <summary>
        /// Gets the example query meta data.
        /// </summary>
        /// <param name="dbDriver">The driver.</param>
        /// <param name="sampleSQL">The sample SQL.</param>
        /// <param name="metadataSetting">The metadata setting.</param>
        /// <returns></returns>
        private static QueryMetaData GetExampleQueryMetaData(
            DbDriver dbDriver,
            String sampleSQL,
            ColumnSettings metadataSetting)
        {
            IList<PlaceholderParser.Fragment> sampleSQLFragments = PlaceholderParser.ParsePlaceholder(sampleSQL);
            using (DbDriverCommand dbCommand = dbDriver.CreateCommand(sampleSQLFragments, metadataSetting))
            {
                return dbCommand.MetaData;
            }
        }

        /// <summary>
        /// Creates the sample placeholder statement.
        /// </summary>
        /// <param name="parseFragements">The parse fragements.</param>
        /// <returns></returns>
        private static String CreateSamplePlaceholderStatement(IEnumerable<PlaceholderParser.Fragment> parseFragements)
        {
            StringBuilder buffer = new StringBuilder();
            foreach (PlaceholderParser.Fragment fragment in parseFragements)
            {
                if (!fragment.IsParameter)
                {
                    buffer.Append(fragment.Value);
                }
                else
                {
                    if (fragment.Value.Equals(SAMPLE_WHERECLAUSE_PLACEHOLDER))
                    {
                        buffer.Append(" where 1=0 ");
                        break;
                    }
                    else
                    {
                        buffer.Append("null");
                    }
                }
            }
            return buffer.ToString();
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
