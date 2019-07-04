using System;
using System.IO;

using NUnit.Framework;

using net.esper.compat;

namespace net.esper.client
{
	[TestFixture]
	public class TestConfiguration
	{
		protected internal const String ESPER_TEST_CONFIG = "regression/esper.test.readconfig.cfg.xml";

		private Configuration config;

		[SetUp]
		public virtual void setUp()
		{
			config = new Configuration();
            config.EngineDefaults.Logging.IsEnableExecutionDebug = true;
        }

		[Test]
		public void testString()
		{
			config.Configure( ESPER_TEST_CONFIG );
			TestConfigurationParser.AssertFileConfig( config );
		}

		[Test]
		public void testURL()
		{
            Uri url = ResourceManager.ResolveResourceURL(ESPER_TEST_CONFIG);
            Assert.IsNotNull(url);

			config.Configure( url );
			TestConfigurationParser.AssertFileConfig( config );
		}

		[Test]
		public void testFile()
		{
            FileInfo file = ResourceManager.ResolveResourceFile(ESPER_TEST_CONFIG);
			config.Configure( file );
			TestConfigurationParser.AssertFileConfig( config );
		}

		[Test]
		public void testAddEventTypeAlias()
		{
			config.AddEventTypeAlias( "AEventType", "BClassName" );

			Assert.AreEqual( 1, config.EventTypeAliases.Count );
			Assert.AreEqual( "BClassName", config.EventTypeAliases.Get( "AEventType" ) );
			assertDefaultConfig();
		}

		private void assertDefaultConfig()
		{
			Assert.AreEqual( 3, config.Imports.Count );
			Assert.AreEqual( "System", config.Imports[0] );
			Assert.AreEqual( "System.Collections", config.Imports[1] );
			Assert.AreEqual( "System.Text", config.Imports[2] );
		}
	}
}
