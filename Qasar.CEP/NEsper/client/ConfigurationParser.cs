// ************************************************************************************
// Copyright (C) 2006 Thomas Bernhardt. All rights reserved.                          *
// http://esper.codehaus.org                                                          *
// ---------------------------------------------------------------------------------- *
// The software in this package is published under the terms of the GPL license       *
// a copy of which has been included with this distribution in the license.txt file.  *
// ************************************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using net.esper.compat;

using org.apache.commons.logging;

namespace net.esper.client
{
    /// <summary>
    /// Parser for configuration XML.
    /// </summary>

    public class ConfigurationParser
    {
        /// <summary>
        /// Use the configuration specified in the given input stream.
        /// </summary>
        /// <param name="configuration">is the configuration object to populate</param>
        /// <param name="stream">The stream.</param>
        /// <param name="resourceName">The name to use in warning/error messages</param>
        /// <throws>  net.esper.client.EPException </throws>

        public static void DoConfigure(Configuration configuration, Stream stream, String resourceName)
        {
            XmlDocument document;

            try
            {
                document = new XmlDocument();
                document.Load(stream);
            }
            catch (XmlException ex)
            {
                throw new EPException("Could not parse configuration: " + resourceName, ex);
            }
            catch (IOException ex)
            {
                throw new EPException("Could not read configuration: " + resourceName, ex);
            }
            finally
            {
                try
                {
                    stream.Close();
                }
                catch (IOException ioe)
                {
                    log.Warn("could not close input stream for: " + resourceName, ioe);
                }
            }

            DoConfigure(configuration, document);
        }

        /// <summary> Parse the W3C DOM document.</summary>
        /// <param name="configuration">is the configuration object to populate
        /// </param>
        /// <param name="doc">to parse
        /// </param>
        /// <throws>  net.esper.client.EPException </throws>
		public static void DoConfigure( Configuration configuration, XmlDocument doc )
        {
            XmlElement root = doc.DocumentElement;

            HandleEventTypes(configuration, root);
            HandleAutoImports(configuration, root);
            HandleDatabaseRefs(configuration, root);
			HandlePlugInView(configuration, root);
			HandlePlugInAggregation(configuration, root);
			HandlePlugInPatternObjects(configuration, root);
			HandleAdapterLoaders(configuration, root);
            HandleEngineSettings(configuration, root);
        }

        private static void HandleEventTypes(Configuration configuration, XmlElement parentElement)
        {
            XmlNodeList nodes = parentElement.GetElementsByTagName("event-type");
            foreach( XmlNode node in nodes )
            {
                String name = node.Attributes.GetNamedItem("alias").InnerText;
                XmlNode classNode = node.Attributes.GetNamedItem("class");

                String optionalClassName = null;
                if (classNode != null)
                {
                    optionalClassName = classNode.InnerText;
                    configuration.AddEventTypeAlias(name, optionalClassName);
                }
                HandleSubElement(name, optionalClassName, configuration, node);
            }
        }

        private static void HandleSubElement(String aliasName, String optionalClassName, Configuration configuration, XmlNode parentNode)
        {
            foreach (XmlElement eventTypeElement in CreateElementEnumerable(parentNode.ChildNodes))
            {
                String nodeName = eventTypeElement.Name;
                if (nodeName.Equals("xml-dom"))
                {
                    HandleXMLDOM(aliasName, configuration, eventTypeElement);
                }
                else if (nodeName.Equals("map"))
                {
                    HandleMap(aliasName, configuration, eventTypeElement);
                }
                else if (nodeName.Equals("legacy-type"))
                {
                    HandleLegacy(aliasName, optionalClassName, configuration, eventTypeElement);
                }
            }
        }

        private static void HandleMap(String aliasName, Configuration configuration, XmlElement eventTypeElement)
        {
            Properties propertyTypeNames = new Properties();
            XmlNodeList propertyList = eventTypeElement.GetElementsByTagName("map-property");
            foreach( XmlNode propertyNode in propertyList )
            {
                String name = propertyNode.Attributes.GetNamedItem("name").InnerText;
                String type = propertyNode.Attributes.GetNamedItem("class").InnerText;
                propertyTypeNames[name] = type;
            }
            configuration.AddEventTypeAlias(aliasName, propertyTypeNames);
        }

        private static void HandleXMLDOM(String aliasName, Configuration configuration, XmlElement xmldomElement)
        {
            String rootElementName = xmldomElement.Attributes.GetNamedItem("root-element-name").InnerText;
            String rootElementNamespace = GetOptionalAttribute(xmldomElement, "root-element-namespace");
            String schemaResource = GetOptionalAttribute(xmldomElement, "schema-resource");
            String defaultNamespace = GetOptionalAttribute(xmldomElement, "default-namespace");

            ConfigurationEventTypeXMLDOM xmlDOMEventTypeDesc = new ConfigurationEventTypeXMLDOM();
            xmlDOMEventTypeDesc.RootElementName = rootElementName;
            xmlDOMEventTypeDesc.SchemaResource = schemaResource;
            xmlDOMEventTypeDesc.RootElementNamespace = rootElementNamespace;
            xmlDOMEventTypeDesc.DefaultNamespace = defaultNamespace;
            configuration.AddEventTypeAlias(aliasName, xmlDOMEventTypeDesc);

            foreach (XmlElement propertyElement in CreateElementEnumerable(xmldomElement.ChildNodes))
            {
                if (propertyElement.Name.Equals("namespace-prefix"))
                {
                    String prefix = propertyElement.Attributes.GetNamedItem("prefix").InnerText;
                    String namespace_ = propertyElement.Attributes.GetNamedItem("namespace").InnerText;
                    xmlDOMEventTypeDesc.AddNamespacePrefix(prefix, namespace_);
                }
                if (propertyElement.Name.Equals("xpath-property"))
                {
                    String propertyName = propertyElement.Attributes.GetNamedItem("property-name").InnerText;
                    String xPath = propertyElement.Attributes.GetNamedItem("xpath").InnerText;
                    String propertyType = propertyElement.Attributes.GetNamedItem("type").InnerText;

                    XPathResultType xpathConstantType;
                    if (propertyType.Equals("NUMBER", StringComparison.InvariantCultureIgnoreCase))
                    {
                        xpathConstantType = XPathResultType.Number;
                    }
                    else if (propertyType.Equals("STRING", StringComparison.InvariantCultureIgnoreCase))
                    {
                        xpathConstantType = XPathResultType.String;
                    }
                    else if (propertyType.Equals("BOOLEAN", StringComparison.InvariantCultureIgnoreCase))
                    {
                        xpathConstantType = XPathResultType.Boolean;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid xpath property type for property '" + propertyName + "' and type '" + propertyType + "'");
                    }
                    
                    xmlDOMEventTypeDesc.AddXPathProperty(propertyName, xPath, xpathConstantType);
                }
            }
        }

        private static void HandleLegacy(String aliasName, String className, Configuration configuration, XmlElement xmldomElement)
        {
            // Class name is required for legacy classes
            if (className == null)
            {
                throw new ConfigurationException("Required class name not supplied for legacy type definition");
            }

            String accessorStyle = xmldomElement.Attributes.GetNamedItem("accessor-style").InnerText;
            String codeGeneration = xmldomElement.Attributes.GetNamedItem("code-generation").InnerText;
            String propertyResolution = xmldomElement.Attributes.GetNamedItem("property-resolution-style").InnerText;

            ConfigurationEventTypeLegacy legacyDesc = new ConfigurationEventTypeLegacy();
            if (accessorStyle != null)
            {
                legacyDesc.AccessorStyle = (ConfigurationEventTypeLegacy.AccessorStyleEnum)Enum.Parse(typeof(ConfigurationEventTypeLegacy.AccessorStyleEnum), accessorStyle, true); 
            }
            if (codeGeneration != null)
            {
                legacyDesc.CodeGeneration = (ConfigurationEventTypeLegacy.CodeGenerationEnum)Enum.Parse(typeof(ConfigurationEventTypeLegacy.CodeGenerationEnum), codeGeneration, true);
            }
            if (propertyResolution != null)
            {
                legacyDesc.PropertyResolutionStyle = (PropertyResolutionStyle)Enum.Parse(typeof(PropertyResolutionStyle), propertyResolution, true);
            }

            configuration.AddEventTypeAlias(aliasName, className, legacyDesc);

            foreach (XmlElement propertyElement in CreateElementEnumerable(xmldomElement.ChildNodes))
            {
                switch (propertyElement.Name)
                {
                    case "method-property":
                        {
                            String name = propertyElement.Attributes.GetNamedItem("name").InnerText;
                            String method = propertyElement.Attributes.GetNamedItem("accessor-method").InnerText;
                            legacyDesc.AddMethodProperty(name, method);
                            break;
                        }
                    case "field-property":
                        {
                            String name = propertyElement.Attributes.GetNamedItem("name").InnerText;
                            String field = propertyElement.Attributes.GetNamedItem("accessor-field").InnerText;
                            legacyDesc.AddFieldProperty(name, field);
                            break;
                        }
                    default:
                        throw new ConfigurationException("Invalid node " + propertyElement.Name +
                                                         " encountered while parsing legacy type definition");
                }
            }
        }

        private static void HandleAutoImports(Configuration configuration, XmlElement parentNode)
        {
            XmlNodeList importNodes = parentNode.GetElementsByTagName("auto-import");
            for (int i = 0; i < importNodes.Count; i++)
            {
                String name = importNodes.Item(i).Attributes.GetNamedItem("import-name").InnerText;
                configuration.AddImport(name);
            }
        }

        private static void HandleDatabaseRefs(Configuration configuration, XmlElement parentNode)
        {
            XmlNodeList dbRefNodes = parentNode.GetElementsByTagName("database-reference");
            foreach (XmlNode dbRefNode in dbRefNodes)
            {
                String name = dbRefNode.Attributes.GetNamedItem("name").InnerText;
                ConfigurationDBRef configDBRef = new ConfigurationDBRef();
                configuration.AddDatabaseReference(name, configDBRef);

                foreach (XmlElement subElement in CreateElementEnumerable(dbRefNode.ChildNodes))
                {
                    switch (subElement.Name)
                    {
                        case "provider-connection":
                            {
                                ConnectionStringSettings settings = new ConnectionStringSettings();
                                settings.Name = subElement.Attributes.GetNamedItem("provider").InnerText;
                                settings.ProviderName = subElement.Attributes.GetNamedItem("provider").InnerText;
                                settings.ConnectionString =
                                    subElement.Attributes.GetNamedItem("connection-string").InnerText;
                                configDBRef.SetDatabaseProviderConnection(settings);
                                break;
                            }
                        case "connection-lifecycle":
                            {
                                String value = subElement.Attributes.GetNamedItem("value").InnerText;
                                configDBRef.ConnectionLifecycle =
                                    (ConnectionLifecycleEnum) Enum.Parse(typeof (ConnectionLifecycleEnum), value, true);
                                break;
                            }
                        case "connection-settings":
                            if (subElement.Attributes.GetNamedItem("auto-commit") != null)
                            {
                                String autoCommit = subElement.Attributes.GetNamedItem("auto-commit").InnerText;
                                configDBRef.ConnectionAutoCommit = Boolean.Parse(autoCommit);
                            }
                            if (subElement.Attributes.GetNamedItem("transaction-isolation") != null)
                            {
                                String transactionIsolation =
                                    subElement.Attributes.GetNamedItem("transaction-isolation").InnerText;
                                configDBRef.ConnectionTransactionIsolation =
                                    (IsolationLevel) Enum.Parse(typeof (IsolationLevel), transactionIsolation, true);
                            }
                            if (subElement.Attributes.GetNamedItem("catalog") != null)
                            {
                                String catalog = subElement.Attributes.GetNamedItem("catalog").InnerText;
                                configDBRef.ConnectionCatalog = catalog;
                            }
                            break;
                        case "parameters":
                            if (subElement.Attributes.GetNamedItem("prefix") != null)
                            {
                                String prefix = subElement.Attributes.GetNamedItem("prefix").InnerText;
                                configDBRef.ParameterPrefix = prefix;
                            }
                            if (subElement.Attributes.GetNamedItem("style") != null)
                            {
                                String style = subElement.Attributes.GetNamedItem("style").InnerText;
                                configDBRef.ParameterStyle =
                                    (ParameterStyle) Enum.Parse(typeof (ParameterStyle), style, true);
                            }
                            break;
                        case "expiry-time-cache":
                            {
                                String maxAge = subElement.Attributes.GetNamedItem("max-age-seconds").InnerText;
                                String purgeInterval =
                                    subElement.Attributes.GetNamedItem("purge-interval-seconds").InnerText;
                                configDBRef.SetExpiryTimeCache(Double.Parse(maxAge), Double.Parse(purgeInterval));
                                break;
                            }
                        case "lru-cache":
                            {
                                String size = subElement.Attributes.GetNamedItem("size").InnerText;
                                configDBRef.LRUCache = Int32.Parse(size);
                                break;
                            }
                    }
                }
            }
        }

        private static void HandlePlugInView(Configuration configuration, XmlElement parentElement)
	    {
	        XmlNodeList nodes = parentElement.GetElementsByTagName("plugin-view");
            foreach (XmlNode node in nodes)
            {
                String _namespace = node.Attributes.GetNamedItem("namespace").InnerText;
                String name = node.Attributes.GetNamedItem("name").InnerText;
	            String factoryClassName = node.Attributes.GetNamedItem("factory-class").InnerText;
	            configuration.AddPlugInView(_namespace, name, factoryClassName);
	        }
	    }

	    private static void HandlePlugInAggregation(Configuration configuration, XmlElement parentElement)
	    {
	        XmlNodeList nodes = parentElement.GetElementsByTagName("plugin-aggregation-function");
            foreach( XmlNode node in nodes )
	        {
	            String name = node.Attributes.GetNamedItem("name").InnerText;
	            String functionClassName = node.Attributes.GetNamedItem("function-class").InnerText;
	            configuration.AddPlugInAggregationFunction(name, functionClassName);
	        }
	    }

	    private static void HandlePlugInPatternObjects(Configuration configuration, XmlElement parentElement)
	    {
	        XmlNodeList nodes = parentElement.GetElementsByTagName("plugin-pattern-guard");
            foreach( XmlNode node in nodes )
	        {
	            String _namespace = node.Attributes.GetNamedItem("namespace").InnerText;
	            String name = node.Attributes.GetNamedItem("name").InnerText;
	            String factoryClassName = node.Attributes.GetNamedItem("factory-class").InnerText;
	            configuration.AddPlugInPatternGuard(_namespace, name, factoryClassName);
	        }

	        nodes = parentElement.GetElementsByTagName("plugin-pattern-observer");
            foreach( XmlNode node in nodes )
	        {
	            String _namespace = node.Attributes.GetNamedItem("namespace").InnerText;
	            String name = node.Attributes.GetNamedItem("name").InnerText;
	            String factoryClassName = node.Attributes.GetNamedItem("factory-class").InnerText;
	            configuration.AddPlugInPatternObserver(_namespace, name, factoryClassName);
	        }
	    }

	    private static void HandleAdapterLoaders(Configuration configuration, XmlElement parentElement)
	    {
	        XmlNodeList nodes = parentElement.GetElementsByTagName("adapter-loader");
	        foreach(XmlNode node in nodes)
	        {
	            String loaderName = node.Attributes.GetNamedItem("name").InnerText;
	            String className = node.Attributes.GetNamedItem("class-name").InnerText;
	            Properties properties = new Properties();

                foreach( XmlElement subElement in CreateElementEnumerable(node.ChildNodes))
	            {
	                if (subElement.Name == "init-arg")
	                {
	                    String name = subElement.Attributes.GetNamedItem("name").InnerText;
	                    String value = subElement.Attributes.GetNamedItem("value").InnerText;
	                    properties[name] = value;
	                }
	            }
	            configuration.AddAdapterLoader(loaderName, className, properties);
	        }
	    }

        private static void HandleEngineSettings(Configuration configuration, XmlElement parentElement)
        {
            XmlNodeList nodes = parentElement.GetElementsByTagName("engine-settings");
            foreach( XmlElement element in nodes )
            {
                foreach( XmlElement subElement in CreateElementEnumerable( element.ChildNodes ) )
                {
                    if (subElement.Name == "defaults" )
                    {
                        HandleEngineSettingsDefaults(configuration, subElement);
                    }
                }
            }
        }

        private static void HandleEngineSettingsDefaults(Configuration configuration, XmlElement parentElement)
        {
            foreach (XmlElement subElement in CreateElementEnumerable(parentElement.ChildNodes))
            {
                switch( subElement.Name )
                {
                    case "threading":
                        HandleDefaultsThreading(configuration, subElement);
                        break;
                    case "event-meta":
                        HandleDefaultsEventMeta(configuration, subElement);
                        break;
                    case "view-resources":
                        HandleDefaultsViewResources(configuration, subElement);
                        break;
                    case "logging":
                        HandleDefaultsLogging(configuration, subElement);
                        break;
                }
            }
        }

        private static void HandleDefaultsThreading(Configuration configuration, XmlElement parentElement)
        {
            foreach (XmlElement subElement in CreateElementEnumerable(parentElement.ChildNodes))
            {
                switch (subElement.Name)
                {
                    case "listener-dispatch":
                        {
                            String preserveOrderText =
                                subElement.Attributes.GetNamedItem("preserve-order").InnerText;
                            Boolean preserveOrder = Boolean.Parse(preserveOrderText);
                            String timeoutMSecText =
                                subElement.Attributes.GetNamedItem("timeout-msec").InnerText;
                            Int64 timeoutMSec = Int64.Parse(timeoutMSecText);
                            configuration.EngineDefaults.Threading.IsListenerDispatchPreserveOrder = preserveOrder;
                            configuration.EngineDefaults.Threading.ListenerDispatchTimeout = timeoutMSec;
                            break;
                        }
                    case "insert-into-dispatch":
                        {
                            String preserveOrderText =
                                subElement.Attributes.GetNamedItem("preserve-order").InnerText;
                            Boolean preserveOrder = Boolean.Parse(preserveOrderText);
                            configuration.EngineDefaults.Threading.IsInsertIntoDispatchPreserveOrder = preserveOrder;
                            break;
                        }
                    case "internal-timer":
                        {
                            String enabledText = subElement.Attributes.GetNamedItem("enabled").InnerText;
                            Boolean enabled = Boolean.Parse(enabledText);
                            String msecResolutionText =
                                subElement.Attributes.GetNamedItem("msec-resolution").InnerText;
                            Int64 msecResolution = Int64.Parse(msecResolutionText);
                            configuration.EngineDefaults.Threading.IsInternalTimerEnabled = enabled;
                            configuration.EngineDefaults.Threading.InternalTimerMsecResolution = msecResolution;
                            break;
                        }
                }
            }
        }

        private static void HandleDefaultsViewResources(Configuration configuration, XmlElement parentElement)
        {
            foreach( XmlElement subElement in CreateElementEnumerable( parentElement.ChildNodes ) )
            {
                if (subElement.Name == "share-views")
                {
                    String valueText = subElement.Attributes.GetNamedItem("enabled").InnerText;
                    Boolean value = Boolean.Parse(valueText);
                    configuration.EngineDefaults.ViewResources.IsShareViews = value;
                }
            }
        }

        private static void HandleDefaultsLogging(Configuration configuration, XmlElement parentElement)
        {
            foreach (XmlElement subElement in CreateElementEnumerable(parentElement.ChildNodes))
            {
                if (subElement.Name =="execution-path")
                {
                    String valueText = subElement.Attributes.GetNamedItem("enabled").InnerText;
                    Boolean value = Boolean.Parse(valueText);
                    configuration.EngineDefaults.Logging.IsEnableExecutionDebug = value;
                }
            }
        }

        private static void HandleDefaultsEventMeta(Configuration configuration, XmlElement parentElement)
        {
            foreach (XmlElement subElement in CreateElementEnumerable(parentElement.ChildNodes))
            {
                if (subElement.Name == "class-property-resolution")
                {
                    String styleText = subElement.Attributes.GetNamedItem("style").InnerText;
                    PropertyResolutionStyle value =
                        (PropertyResolutionStyle) Enum.Parse(typeof (PropertyResolutionStyle), styleText, true);
                    configuration.EngineDefaults.EventMeta.ClassPropertyResolutionStyle = value;
                }
            }
        }


        private static Properties HandleProperties(XmlElement element, String propElementName)
        {
            Properties properties = new Properties();
            foreach( XmlElement subElement in CreateElementEnumerable(element.ChildNodes))
            {
                if (subElement.Name.Equals(propElementName))
                {
                    String name = subElement.Attributes.GetNamedItem("name").InnerText;
                    String value = subElement.Attributes.GetNamedItem("value").InnerText;
                    properties[name] = value;
                }
            }
            return properties;
        }

		/// <summary>
		/// Returns an input stream from an application resource in the classpath.
		/// </summary>
		/// <param name="resource">to get input stream for</param>
		/// <returns>input stream for resource</returns>

		public static Stream GetResourceAsStream( String resource )
        {
            String stripped = resource.StartsWith("/") ? resource.Substring(1) : resource;

            Stream stream = ResourceManager.GetResourceAsStream( resource ) ;
            if ( stream == null )
            {
            	stream = ResourceManager.GetResourceAsStream( stripped ) ;
            }
            if (stream == null)
            {
                throw new EPException(resource + " not found");
            }
            return stream;
        }

        private static String GetOptionalAttribute(XmlNode node, String key)
        {
            XmlNode valueNode = node.Attributes.GetNamedItem(key);
            if (valueNode != null)
            {
                return valueNode.InnerText;
            }
            return null;
        }
        
        private static IEnumerable<XmlElement> CreateElementEnumerable( XmlNodeList nodeList )
        {
        	foreach( XmlNode node in nodeList )
            {
                if (node is XmlElement)
                {
                    yield return node as XmlElement ;
                }
            }
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
