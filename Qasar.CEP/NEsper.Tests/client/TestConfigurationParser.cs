///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Xml.XPath;

using NUnit.Framework;

using net.esper.compat;

namespace net.esper.client
{
	[TestFixture]
	public class TestConfigurationParser
	{
	    private Configuration config;

	    [SetUp]
	    public void SetUp()
	    {
	        config = new Configuration();
	    }

	    [Test]
	    public void testConfigureFromStream()
	    {
            Uri url = ResourceManager.ResolveResourceURL(TestConfiguration.ESPER_TEST_CONFIG);
	        WebRequest request = WebRequest.Create(url);
            using (Stream stream = request.GetResponse().GetResponseStream())
            {
                ConfigurationParser.DoConfigure(config, stream, url.ToString());
                AssertFileConfig(config);
            }
	    }

        [Test]
        public void testEngineDefaults()
        {
            config = new Configuration();

            Assert.IsTrue(config.EngineDefaults.Threading.IsInsertIntoDispatchPreserveOrder);
            Assert.IsTrue(config.EngineDefaults.Threading.IsListenerDispatchPreserveOrder);
            Assert.AreEqual(1000, config.EngineDefaults.Threading.ListenerDispatchTimeout);
            Assert.IsTrue(config.EngineDefaults.Threading.IsInternalTimerEnabled);
            Assert.AreEqual(100, config.EngineDefaults.Threading.InternalTimerMsecResolution);

            // Unit tests are setup for CASE_INSENSITIVE by default; this differs from how
            // the esper unit tests are setup.
            Assert.AreEqual(PropertyResolutionStyle.CASE_INSENSITIVE, config.EngineDefaults.EventMeta.ClassPropertyResolutionStyle);

            Assert.IsTrue(config.EngineDefaults.ViewResources.IsShareViews);

            Assert.IsFalse(config.EngineDefaults.Logging.IsEnableExecutionDebug);
        }

	    internal static void AssertFileConfig(Configuration config)
	    {
	        // assert alias for class
	        Assert.AreEqual(3, config.EventTypeAliases.Count);
	        Assert.AreEqual("com.mycompany.myapp.MySampleEventOne", config.EventTypeAliases.Get("MySampleEventOne"));
            Assert.AreEqual("com.mycompany.myapp.MySampleEventTwo", config.EventTypeAliases.Get("MySampleEventTwo"));
            Assert.AreEqual("com.mycompany.package.MyLegacyTypeEvent", config.EventTypeAliases.Get("MyLegacyTypeEvent"));

	        // assert auto imports
	        Assert.AreEqual(2, config.Imports.Count);
	        Assert.AreEqual("com.mycompany.myapp.*", config.Imports[0]);
	        Assert.AreEqual("com.mycompany.myapp.ClassOne", config.Imports[1]);

	        // assert XML DOM - no schema
	        Assert.AreEqual(2, config.EventTypesXMLDOM.Count);
            ConfigurationEventTypeXMLDOM noSchemaDesc = config.EventTypesXMLDOM.Get("MyNoSchemaXMLEventAlias");
	        Assert.AreEqual("MyNoSchemaEvent", noSchemaDesc.RootElementName);
            Assert.AreEqual("/myevent/element1", noSchemaDesc.XPathProperties.Get("element1").XPath);
	        Assert.AreEqual(XPathResultType.Number, noSchemaDesc.XPathProperties.Get("element1").ResultType);

	        // assert XML DOM - with schema
            ConfigurationEventTypeXMLDOM schemaDesc = config.EventTypesXMLDOM.Get("MySchemaXMLEventAlias");
	        Assert.AreEqual("MySchemaEvent", schemaDesc.RootElementName);
	        Assert.AreEqual("MySchemaXMLEvent.xsd", schemaDesc.SchemaResource);
	        Assert.AreEqual("samples:schemas:simpleSchema", schemaDesc.RootElementNamespace);
	        Assert.AreEqual("default-name-space", schemaDesc.DefaultNamespace);
	        Assert.AreEqual("/myevent/element1", schemaDesc.XPathProperties.Get("element1").XPath);
            Assert.AreEqual(XPathResultType.Number, schemaDesc.XPathProperties.Get("element1").ResultType);
	        Assert.AreEqual(1, schemaDesc.NamespacePrefixes.Count);
	        Assert.AreEqual("samples:schemas:simpleSchema", schemaDesc.NamespacePrefixes.Get("ss"));

	        // assert mapped events
	        Assert.AreEqual(1, config.EventTypesMapEvents.Count);
	        Assert.IsTrue(config.EventTypesMapEvents.ContainsKey("MyMapEvent"));
	        Properties expectedProps = new Properties();
	        expectedProps.Put("myInt", "int");
	        expectedProps.Put("myString", "string");
            Assert.AreEqual(expectedProps, config.EventTypesMapEvents.Get("MyMapEvent"));

	        // assert legacy type declaration
	        Assert.AreEqual(1, config.EventTypesLegacy.Count);
	        ConfigurationEventTypeLegacy legacy = config.EventTypesLegacy.Get("MyLegacyTypeEvent");
	        Assert.AreEqual(ConfigurationEventTypeLegacy.CodeGenerationEnum.ENABLED, legacy.CodeGeneration);
	        Assert.AreEqual(ConfigurationEventTypeLegacy.AccessorStyleEnum.PUBLIC, legacy.AccessorStyle);
	        Assert.AreEqual(1, legacy.FieldProperties.Count);
	        Assert.AreEqual("myFieldName", legacy.FieldProperties[0].AccessorFieldName);
            Assert.AreEqual("myfieldprop", legacy.FieldProperties[0].Name);
	        Assert.AreEqual(1, legacy.MethodProperties.Count);
	        Assert.AreEqual("myAccessorMethod", legacy.MethodProperties[0].AccessorMethodName);
	        Assert.AreEqual("mymethodprop", legacy.MethodProperties[0].Name);
            Assert.AreEqual(PropertyResolutionStyle.CASE_INSENSITIVE, legacy.PropertyResolutionStyle);

	        // assert database reference - data source config
            Assert.AreEqual(2, config.DatabaseReferences.Count);
            ConfigurationDBRef configDBRef = config.DatabaseReferences.Get("mydb1");
            DbProviderFactoryConnection dsDef = (DbProviderFactoryConnection)configDBRef.ConnectionFactoryDesc;
            Assert.AreEqual("MySql.Data.MySqlClient", dsDef.Settings.ProviderName);
            Assert.AreEqual("Server=localhost;Database=test;Uid=nesper;Pwd=password", dsDef.Settings.ConnectionString);
            Assert.AreEqual(ConnectionLifecycleEnum.POOLED, configDBRef.ConnectionLifecycle);
            Assert.IsNull(configDBRef.ConnectionSettings.AutoCommit);
            Assert.IsNull(configDBRef.ConnectionSettings.Catalog);
            Assert.IsNull(configDBRef.ConnectionSettings.TransactionIsolation);
            LRUCacheDesc lruCache = (LRUCacheDesc) configDBRef.DataCacheDesc;
            Assert.AreEqual(10, lruCache.Size);

	        // assert database reference - driver manager config
            configDBRef = config.DatabaseReferences.Get("mydb2");
            DbProviderFactoryConnection dmDef = (DbProviderFactoryConnection) configDBRef.ConnectionFactoryDesc;
            Assert.AreEqual("System.Data.Odbc", dmDef.Settings.ProviderName);
            Assert.AreEqual("Driver={MySQL ODBC 3.51 Driver};Server=localhost;Database=test;User=nesper;Password=password;Option=3", dmDef.Settings.ConnectionString);
            Assert.AreEqual(ConnectionLifecycleEnum.RETAIN, configDBRef.ConnectionLifecycle);
            Assert.AreEqual(false, configDBRef.ConnectionSettings.AutoCommit);
            Assert.AreEqual("test", configDBRef.ConnectionSettings.Catalog);
            Assert.AreEqual(IsolationLevel.ReadCommitted, configDBRef.ConnectionSettings.TransactionIsolation);
            ExpiryTimeCacheDesc expCache = (ExpiryTimeCacheDesc) configDBRef.DataCacheDesc;
            Assert.AreEqual(60.5, expCache.MaxAgeSeconds);
            Assert.AreEqual(120.1, expCache.PurgeIntervalSeconds);

	        // assert custom view implementations
	        IList<ConfigurationPlugInView> configViews = config.PlugInViews;
	        Assert.AreEqual(2, configViews.Count);
	        for (int i = 0; i < configViews.Count; i++)
	        {
	            ConfigurationPlugInView entry = configViews[i];
	            Assert.AreEqual("ext" + i, entry.Namespace);
                Assert.AreEqual("myview" + i, entry.Name);
	            Assert.AreEqual("com.mycompany.MyViewFactory" + i, entry.FactoryClassName);
	        }

	        // assert adapter loaders parsed
	        IList<ConfigurationAdapterLoader> adapters = config.AdapterLoaders;
	        Assert.AreEqual(2, adapters.Count);
	        ConfigurationAdapterLoader adapterOne = adapters[0];
	        Assert.AreEqual("Loader1", adapterOne.LoaderName);
	        Assert.AreEqual("net.esper.support.adapter.SupportLoaderOne", adapterOne.TypeName);
	        Assert.AreEqual(2, adapterOne.ConfigProperties.Count);
	        Assert.AreEqual("val1", adapterOne.ConfigProperties.Get("name1"));
	        Assert.AreEqual("val2", adapterOne.ConfigProperties.Get("name2"));

	        ConfigurationAdapterLoader adapterTwo = adapters[1];
	        Assert.AreEqual("Loader2", adapterTwo.LoaderName);
	        Assert.AreEqual("net.esper.support.adapter.SupportLoaderTwo", adapterTwo.TypeName);
	        Assert.AreEqual(0, adapterTwo.ConfigProperties.Count);

	        // assert plug-in aggregation function loaded
	        Assert.AreEqual(2, config.PlugInAggregationFunctions.Count);
	        ConfigurationPlugInAggregationFunction pluginAgg = config.PlugInAggregationFunctions[0];
	        Assert.AreEqual("com.mycompany.MyMatrixAggregationMethod0", pluginAgg.FunctionClassName);
            Assert.AreEqual("func1", pluginAgg.Name);
	        pluginAgg = config.PlugInAggregationFunctions[1];
	        Assert.AreEqual("com.mycompany.MyMatrixAggregationMethod1", pluginAgg.FunctionClassName);
            Assert.AreEqual("func2", pluginAgg.Name);

	        // assert plug-in guard objects loaded
	        Assert.AreEqual(4, config.PlugInPatternObjects.Count);
	        ConfigurationPlugInPatternObject pluginPattern = config.PlugInPatternObjects[0];
	        Assert.AreEqual("com.mycompany.MyGuardFactory0", pluginPattern.FactoryClassName);
	        Assert.AreEqual("ext0", pluginPattern.Namespace);
            Assert.AreEqual("guard1", pluginPattern.Name);
	        Assert.AreEqual(ConfigurationPlugInPatternObject.PatternObjectTypeEnum.GUARD, pluginPattern.PatternObjectType);
	        pluginPattern = config.PlugInPatternObjects[1];
	        Assert.AreEqual("com.mycompany.MyGuardFactory1", pluginPattern.FactoryClassName);
	        Assert.AreEqual("ext1", pluginPattern.Namespace);
            Assert.AreEqual("guard2", pluginPattern.Name);
	        Assert.AreEqual(ConfigurationPlugInPatternObject.PatternObjectTypeEnum.GUARD, pluginPattern.PatternObjectType);
	        pluginPattern = config.PlugInPatternObjects[2];
	        Assert.AreEqual("com.mycompany.MyObserverFactory0", pluginPattern.FactoryClassName);
	        Assert.AreEqual("ext0", pluginPattern.Namespace);
            Assert.AreEqual("observer1", pluginPattern.Name);
	        Assert.AreEqual(ConfigurationPlugInPatternObject.PatternObjectTypeEnum.OBSERVER, pluginPattern.PatternObjectType);
	        pluginPattern = config.PlugInPatternObjects[3];
	        Assert.AreEqual("com.mycompany.MyObserverFactory1", pluginPattern.FactoryClassName);
	        Assert.AreEqual("ext1", pluginPattern.Namespace);
            Assert.AreEqual("observer2", pluginPattern.Name);
	        Assert.AreEqual(ConfigurationPlugInPatternObject.PatternObjectTypeEnum.OBSERVER, pluginPattern.PatternObjectType);

            // assert engine defaults
            Assert.IsFalse(config.EngineDefaults.Threading.IsInsertIntoDispatchPreserveOrder);
            Assert.IsFalse(config.EngineDefaults.Threading.IsListenerDispatchPreserveOrder);
            Assert.AreEqual(2000, config.EngineDefaults.Threading.ListenerDispatchTimeout);
            Assert.IsFalse(config.EngineDefaults.Threading.IsInternalTimerEnabled);
            Assert.AreEqual(1234567, config.EngineDefaults.Threading.InternalTimerMsecResolution);
            Assert.IsFalse(config.EngineDefaults.ViewResources.IsShareViews);
            Assert.AreEqual(PropertyResolutionStyle.DISTINCT_CASE_INSENSITIVE, config.EngineDefaults.EventMeta.ClassPropertyResolutionStyle);
            Assert.IsTrue(config.EngineDefaults.Logging.IsEnableExecutionDebug);
        }
	}
} // End of namespace
