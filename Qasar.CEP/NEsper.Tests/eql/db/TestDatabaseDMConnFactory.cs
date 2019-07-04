using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;

using net.esper.client;
using net.esper.support.eql;

using NUnit.Framework;

using org.apache.commons.logging;

namespace net.esper.eql.db
{
    [TestFixture]
    public class TestDatabaseDMConnFactory
    {
        private DatabaseProviderConnFactory databaseDMConnFactoryOne;
        private DatabaseProviderConnFactory databaseDMConnFactoryTwo;
        private DatabaseProviderConnFactory databaseDMConnFactoryThree;

        [SetUp]
        public virtual void SetUp()
        {
        	NameValueCollection properties = new NameValueCollection() ;

            // driver-manager config 1
            ConfigurationDBRef config = new ConfigurationDBRef();
            // Server=myServerAddress;Database=myDataBase;Uid=myuser2;Pwd=mypassword2;
            properties["Server"] = System.Net.Dns.GetHostName();
            properties["Uid"] = SupportDatabaseService.DBUSER;
            properties["Pwd"] = SupportDatabaseService.DBPWD;
            config.SetDatabaseProviderConnection(SupportDatabaseService.DBPROVIDER1, properties);
            config.ConnectionCatalog = "test";
            config.ConnectionAutoCommit = false; // not supported yet
            config.ConnectionTransactionIsolation = IsolationLevel.Unspecified;
            databaseDMConnFactoryOne = new DatabaseProviderConnFactory((DbProviderFactoryConnection)config.ConnectionFactoryDesc, config.ConnectionSettings);

            // driver-manager config 2
            config = new ConfigurationDBRef();
            config.SetDatabaseProviderConnection(new ConnectionStringSettings(
                "Connection2",
                SupportDatabaseService.DBCONNECTION1_STRING,
                SupportDatabaseService.DBPROVIDER1));
            databaseDMConnFactoryTwo = new DatabaseProviderConnFactory((DbProviderFactoryConnection)config.ConnectionFactoryDesc, config.ConnectionSettings);

            // driver-manager config 3
            config = new ConfigurationDBRef();
            config.SetDatabaseProviderConnection(new ConnectionStringSettings(
                "Connection3",
                SupportDatabaseService.DBCONNECTION2_STRING,
                SupportDatabaseService.DBPROVIDER2));
            databaseDMConnFactoryThree = new DatabaseProviderConnFactory((DbProviderFactoryConnection)config.ConnectionFactoryDesc, config.ConnectionSettings);
        }

        [Test]
        public void testGetConnection()
        {
            DbConnection connection = databaseDMConnFactoryOne.Connection;
            TryAndCloseConnection(connection);

            connection = databaseDMConnFactoryTwo.Connection;
            TryAndCloseConnection(connection);

            connection = databaseDMConnFactoryThree.Connection;
            TryAndCloseConnection(connection);
        }

        private void TryAndCloseConnection(DbConnection connection)
        {
        	DbCommand stmt ;

            stmt = connection.CreateCommand();
            stmt.CommandText = "select 1 from dual";
            stmt.CommandType = CommandType.Text;

            using (DbDataReader result = stmt.ExecuteReader())
            {
                Assert.IsTrue(result.Read());
                Assert.AreEqual(1, result.GetInt32(0));
                result.Close();
            }

            stmt.Dispose();

            connection.Close();
            connection.Dispose();
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
