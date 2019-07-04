using System;
using System.Configuration;
using System.Data.Common;

using net.esper.client;
using net.esper.compat;
using net.esper.eql.db;
using net.esper.support.schedule;

namespace net.esper.support.eql
{
	public class SupportDatabaseService
	{
        public const String DBUSER = "nesper";
        public const String DBPWD = "password";

        static SupportDatabaseService()
        {
            DB1_SETTINGS = ConfigurationManager.ConnectionStrings["db1"];
            DB2_SETTINGS = ConfigurationManager.ConnectionStrings["db2"];

            DBCONNECTION1_STRING = DB1_SETTINGS.ConnectionString;
            DBPROVIDER1 = DB1_SETTINGS.ProviderName;

            DBCONNECTION2_STRING = DB2_SETTINGS.ConnectionString;
            DBPROVIDER2 = DB2_SETTINGS.ProviderName;

            DB1_PARAMETER_STYLE = ParameterStyle.Named;
            DB2_PARAMETER_STYLE = ParameterStyle.Positional;

            DB1_PARAMETER_PREFIX = "?";
            DB2_PARAMETER_PREFIX = "?";
        }

	    public static readonly ConnectionStringSettings DB1_SETTINGS;
	    public static readonly ConnectionStringSettings DB2_SETTINGS;

	    public static readonly String DBCONNECTION1_STRING;
	    public static readonly String DBCONNECTION2_STRING;

	    public static readonly String DBPROVIDER1;
	    public static readonly String DBPROVIDER2;

        public static readonly String DB1_PARAMETER_PREFIX;
        public static readonly String DB2_PARAMETER_PREFIX;

        public static readonly ParameterStyle DB1_PARAMETER_STYLE;
        public static readonly ParameterStyle DB2_PARAMETER_STYLE;

        public const String DBNAME_FULL = "mydb";
        public const String DBNAME_PART = "mydb2";

        private static void ExecuteNonQuery( DbConnection dbConnection, String command )
        {
        	DbCommand dbCommand = dbConnection.CreateCommand() ;
        	dbCommand.CommandText = command ;
        	dbCommand.ExecuteNonQuery() ;
        }
        
        public static ConfigurationDBRef MakeConfigurationDB1()
        {
            ConfigurationDBRef config;

            config = new ConfigurationDBRef();
            config.SetDatabaseProviderConnection(DB1_SETTINGS);
            config.ParameterStyle = DB1_PARAMETER_STYLE;
            config.ParameterPrefix = DB1_PARAMETER_PREFIX;

            return config;
        }

        public static ConfigurationDBRef MakeConfigurationDB2()
        {
            ConfigurationDBRef config;

            config = new ConfigurationDBRef();
            config.SetDatabaseProviderConnection(DB2_SETTINGS);
            config.ParameterStyle = DB2_PARAMETER_STYLE;
            config.ParameterPrefix = DB2_PARAMETER_PREFIX;

            return config;
        }

	    public static DatabaseConfigServiceImpl MakeService()
		{
			EDictionary<String, ConfigurationDBRef> configs = new HashDictionary<String, ConfigurationDBRef>();

			ConfigurationDBRef config;

	        config = MakeConfigurationDB1();
		    configs.Put(DBNAME_FULL, config);

	        config = MakeConfigurationDB2();
            configs.Put(DBNAME_PART, config);

			return new DatabaseConfigServiceImpl( configs, new SupportSchedulingServiceImpl(), null );
		}
	}
}
