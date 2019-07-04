using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.epl.expression;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.db.drivers
{
    /// <summary>
    /// Companion to the BaseDbDriver that provides command support in
    /// accordance to ADO.NET and the DbDriverCommand.
    /// </summary>

    public class BaseDbDriverCommand : DbDriverCommand
    {
        private const String SAMPLE_WHERECLAUSE_PLACEHOLDER = "$ESPER-SAMPLE-WHERE";

        /// <summary>
        /// Underlying driver.
        /// </summary>
        private readonly BaseDbDriver driver;

        /// <summary>
        /// Fragments that were used to build the command.
        /// </summary>
        private readonly List<PlaceholderParser.Fragment> fragments;

        /// <summary>
        /// List of input parameters
        /// </summary>
        private readonly List<String> inputParameters;

        /// <summary>
        /// Output parameters; cached upon creation
        /// </summary>
        private Map<String, DBOutputTypeDesc> outputParameters;

        /// <summary>
        /// Command text that needs to be associated with the command.
        /// </summary>
        private readonly String dbCommandText;

        /// <summary>
        /// Column settings
        /// </summary>
        private readonly ColumnSettings metadataSettings;

        /// <summary>
        /// Private lock for connection and command.
        /// </summary>
        private Object allocLock;

        /// <summary>
        /// Connection allocated to this instance
        /// </summary>
        private DbConnection theConnection;

        /// <summary>
        /// Command allocated to this instance
        /// </summary>
        private DbCommand theCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbDriverCommand"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="fragments">The fragments.</param>
        /// <param name="inputParameters">The input parameters.</param>
        /// <param name="dbCommandText">The command text.</param>
        /// <param name="metadataSettings">The metadata settings.</param>
        protected internal BaseDbDriverCommand(
            BaseDbDriver driver, 
            IEnumerable<PlaceholderParser.Fragment> fragments,
            IEnumerable<String> inputParameters,
            String dbCommandText,
            ColumnSettings metadataSettings)
        {
            this.driver = driver;
            this.metadataSettings = metadataSettings;
            this.fragments = new List<PlaceholderParser.Fragment>(fragments);
            this.inputParameters = new List<string>(inputParameters);
            this.dbCommandText = dbCommandText;

            this.allocLock = new Object();
            this.theConnection = null;
            this.theCommand = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbDriverCommand"/> class.
        /// Used for cloning.
        /// </summary>
        protected internal BaseDbDriverCommand()
        {
        }

        /// <summary>
        /// Clones the driver command.
        /// </summary>
        /// <returns></returns>
        public virtual DbDriverCommand Clone()
        {
            BaseDbDriverCommand dbClone = (BaseDbDriverCommand) MemberwiseClone();
            // Create an independent lock
            dbClone.allocLock = new object();
            // Ensure theConnection and theCommand are not copied
            dbClone.theConnection = null;
            dbClone.theCommand = null;
            // Return the clone
            return dbClone;
        }

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (allocLock)
            {
                // Clean up the command
                if (theCommand != null)
                {
                    theCommand.Dispose();
                    theCommand = null;
                }

                // Clean up the connection
                //if (theConnection != null)
                //{
                //    theConnection.Dispose();
                //    theConnection = null;
                //}
            }
        }

        #endregion

        private DbCommand _CreateCommand(DbConnection dbConnection)
        {
            DbCommand myCommand;
            // Create the command
            myCommand = dbConnection.CreateCommand();
            myCommand.CommandType = CommandType.Text;
            myCommand.CommandText = dbCommandText;
            // Bind the parameters
            myCommand.Parameters.Clear();
            foreach (string parameterName in inputParameters)
            {
                DbParameter myParam = myCommand.CreateParameter();
                myParam.ParameterName = parameterName;
                myCommand.Parameters.Add(myParam);
            }

            return myCommand;
        }

        /// <summary>
        /// Ensures that the command is allocated.
        /// </summary>
        protected virtual void AllocateCommand()
        {
            lock( allocLock )
            {
                if (theCommand == null)
                {
                    // Create the connection
                    theConnection = driver.CreateConnectionInternal();
                    // Create the command
                    theCommand = _CreateCommand(theConnection);
                }
            }
        }

        /// <summary>
        /// Gets the actual database command.
        /// </summary>
        /// <value>The command.</value>
        public virtual DbCommand Command
        {
            get
            {
                AllocateCommand();
                return theCommand;
            }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <param name="parseFragements">The parse fragements.</param>
        /// <returns></returns>
        private static SQLParameterDesc GetParameters(IEnumerable<PlaceholderParser.Fragment> parseFragements)
        {
            List<String> eventPropertyParams = new List<String>();
            List<String> builtinParams = new List<String>();
            foreach (PlaceholderParser.Fragment fragment in parseFragements)
            {
                if (fragment.IsParameter)
                {
                    if (fragment.Value == SAMPLE_WHERECLAUSE_PLACEHOLDER)
                    {
                        builtinParams.Add(fragment.Value);
                    }
                    else
                    {
                        eventPropertyParams.Add(fragment.Value);
                    }
                }
            }

            IList<String> paramList = eventPropertyParams;
            IList<String> builtin = eventPropertyParams;
            return new SQLParameterDesc(paramList, builtin);
        }

        /// <summary>
        /// Gets the fragments.
        /// </summary>
        /// <value>The fragments.</value>
        public virtual IEnumerable<PlaceholderParser.Fragment> Fragments
        {
            get { return fragments; }
        }

        /// <summary>
        /// Gets the pseudo text.
        /// </summary>
        /// <value>The pseudo text.</value>
        public virtual String PseudoText
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (PlaceholderParser.Fragment fragment in Fragments)
                {
                    if (fragment.IsParameter)
                    {
                        if (fragment.Value != SAMPLE_WHERECLAUSE_PLACEHOLDER)
                        {
                            builder.Append('?');
                        }
                    }
                    else
                    {
                        builder.Append(fragment.Value);
                    }
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public virtual String CommandText
        {
            get { return dbCommandText; }
        }

        #region DbDriverCommand Members

        /// <summary>
        /// Gets the driver associated with this command.
        /// </summary>
        /// <value></value>
        public virtual DbDriver Driver
        {
            get { return driver; }
        }

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <value>The meta data.</value>
        public virtual QueryMetaData MetaData
        {
            get { return new QueryMetaData(InputParameters, OutputParameters); }
        }

        /// <summary>
        /// Gets the meta data settings associated with this command.
        /// </summary>
        public ColumnSettings MetaDataSettings
        {
            get { return metadataSettings; }
        }

        /// <summary>
        /// Gets a list of parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public virtual SQLParameterDesc ParameterDescription
        {
            get { return GetParameters(fragments); }
        }

        /// <summary>
        /// Gets the input parameters.
        /// </summary>
        /// <value>The input parameters.</value>
        public virtual IList<String> InputParameters
        {
            get { return inputParameters; }
        }

        /// <summary>
        /// Gets the output parameters.
        /// </summary>
        /// <value>The output parameters.</value>
        public virtual Map<String, DBOutputTypeDesc> OutputParameters
        {
            get
            {
                if (outputParameters == null)
                {
                    CreateOutputParameters();
                }

                return outputParameters;
            }

            protected set
            {
                outputParameters = value;
            }
        }

        /// <summary>
        /// Creates and sets the output parameters
        /// </summary>

        protected virtual void CreateOutputParameters()
        {
            try
            {
                if (log.IsInfoEnabled)
                {
                    log.Info(".OutputParameters - dbCommandText = '" + dbCommandText + "'");
                }

                // This embodies the default behavior of the BaseDbDriver and how it
                // handles the analysis of a query and the schema that is associated
                // with it.  If this handling is incorrect, you can (a) subclass and
                // provide your implementation or (b) submit the points you need
                // interceptors to be added so that we can provide you with the
                // right hooks.  ADO.NET can often be difficult to navigate.

                DataTable schemaTable;

                DbConnection dbConnection = driver.CreateConnectionInternal();

                using (DbCommand dbCommand = _CreateCommand(dbConnection))
                {
                    try
                    {
                        using (IDataReader reader = dbCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                        {
                            // Get the schema table
                            schemaTable = reader.GetSchemaTable();
                        }
                    }
                    catch (DbException ex)
                    {
                        String text = "Error in statement '" + dbCommandText +
                                      "', failed to obtain result metadata, consider turning off metadata interrogation via configuration";
                        log.Error(text, ex);
                        throw new ExprValidationException(text + ", please check the statement, reason: " +
                                                          ex.Message);
                    }

                    if (log.IsDebugEnabled)
                    {
                        log.Debug(".OutputParameters value = " + outputParameters);
                    }
                }

                // Analyze the schemaTable
                outputParameters = CompileSchemaTable(schemaTable, metadataSettings);
            }
            catch (DbException ex)
            {
                String text = "Error preparing statement '" + dbCommandText + '\'';
                log.Error(text, ex);
                throw new ExprValidationException(text + ", reason: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the type of the column associated with the row in the
        /// table schema.
        /// </summary>
        /// <param name="schemaDataRow">The schema data row.</param>
        /// <returns></returns>
        protected virtual Type GetColumnType( DataRow schemaDataRow )
        {
            Type columnType = (Type)schemaDataRow["DataType"];
            Int32 columnSize = (Int32)schemaDataRow["ColumnSize"];

            // Some providers (read MySQL) provide booleans as an integer
            // with a size of 1.  We should probably convert these to bool
            // to make client integration easier.
            if ((columnType == typeof(sbyte)) && (columnSize == 1))
            {
                columnType = typeof(bool);
            }

            return columnType;
        }

        /// <summary>
        /// Gets the SQL type of the column associated with the row in the
        /// table schema.
        /// </summary>
        /// <param name="schemaDataRow">The schema data row.</param>
        /// <returns></returns>
        protected virtual String GetColumnSqlType( DataRow schemaDataRow )
        {
            Int32 providerType = (Int32)schemaDataRow["ProviderType"];
            String sqlTypeName = Convert.ToString(providerType);
            return sqlTypeName;
        }

        /// <summary>
        /// Compiles the schema table.
        /// </summary>
        /// <param name="schemaTable">The schema table.</param>
        /// <param name="columnSettings">The column settings.</param>
        /// <returns></returns>
        protected virtual Map<string, DBOutputTypeDesc> CompileSchemaTable(
            DataTable schemaTable,
            ColumnSettings columnSettings)
        {
            Map<String, DBOutputTypeDesc> outputProperties = new HashMap<String, DBOutputTypeDesc>();
            foreach (DataRow dataRow in schemaTable.Rows)
            {
                String columnName = (String)dataRow["ColumnName"];
                Type columnType = GetColumnType(dataRow);
                String sqlTypeName = GetColumnSqlType(dataRow);

                // Address column case management
                ConfigurationDBRef.ColumnChangeCaseEnum caseEnum = columnSettings.ColumnCaseConversionEnum;
                switch (caseEnum)
                {
                    case ConfigurationDBRef.ColumnChangeCaseEnum.LOWERCASE:
                        columnName = columnName.ToLower();
                        break;
                    case ConfigurationDBRef.ColumnChangeCaseEnum.UPPERCASE:
                        columnName = columnName.ToUpper();
                        break;
                }

                // Setup type binding
                DatabaseTypeBinding binding = null;

                // Check the typeBinding; the typeBinding tells us if we are
                // converting the resultant type from the dataType that has been
                // provided to us into a different type.

                //if (columnSettings.SqlTypeBinding != null)
                //{
                //    String typeBinding = columnSettings.SqlTypeBinding.Get(columnType);
                //    if (typeBinding != null)
                //    {
                //        binding = DatabaseTypeEnum.GetEnum(typeBinding).Binding;
                //    }
                //}

                DBOutputTypeDesc outputDesc = new DBOutputTypeDesc(sqlTypeName, columnType, binding);
                outputProperties[columnName] = outputDesc;
            }

            return outputProperties;
        }

        #endregion

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    /// <summary>
    /// Creates database command objects
    /// </summary>
    /// <returns></returns>

    public delegate IDbCommand DbCommandFactory();
}
