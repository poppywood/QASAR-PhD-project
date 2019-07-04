using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.events.xml
{
    /// <summary>
    /// Optimistic try to resolve the property string into an appropiate xPath,
    /// and use it as getter.
    /// Mapped and Indexed properties supported.
    /// Because no type information is given, all property are resolved to String.
    /// No namespace support.
    /// Cannot access to xml attributes, only elements content.
    /// <para>
    /// If an xsd is present, then use <see cref="com.espertech.esper.events.xml.SchemaXMLEventType"/>
    /// </para>
    /// </summary>
    public class SimpleXMLEventType : BaseXMLEventType
    {
        private readonly Map<String, TypedEventPropertyGetter> propertyGetterCache;
        private readonly String defaultNamespacePrefix;
        private readonly bool isResolvePropertiesAbsolute;
        private readonly XmlNamespaceManager nsManager;

        /// <summary> Ctor.</summary>
        /// <param name="configurationEventTypeXMLDOM">configures the event type
        /// </param>
        public SimpleXMLEventType(ConfigurationEventTypeXMLDOM configurationEventTypeXMLDOM)
            : base(configurationEventTypeXMLDOM)
        {
            isResolvePropertiesAbsolute = configurationEventTypeXMLDOM.IsResolvePropertiesAbsolute;

            // Set of namespace context for XPath expressions
            nsManager = XPathNamespaceContext.Create();
            foreach (KeyValuePair<String, String> entry in configurationEventTypeXMLDOM.NamespacePrefixes)
            {
                nsManager.AddNamespace(entry.Key, entry.Value);
            }

            if (configurationEventTypeXMLDOM.DefaultNamespace != null)
            {
                String defaultNamespace = configurationEventTypeXMLDOM.DefaultNamespace;
                nsManager.AddNamespace(String.Empty, defaultNamespace);

                // determine a default namespace prefix to use to construct XPath expressions from pure property names
                defaultNamespacePrefix = null;
                foreach (KeyValuePair<String, String> entry in configurationEventTypeXMLDOM.NamespacePrefixes)
                {
                    if (Equals(entry.Value, defaultNamespace))
                    {
                        defaultNamespacePrefix = entry.Key;
                        break;
                    }
                }
            }

            base.NamespaceManager = nsManager;
            SetExplicitProperties(configurationEventTypeXMLDOM.XPathProperties.Values);

            propertyGetterCache = new HashMap<String, TypedEventPropertyGetter>();
        }

        internal override Type DoResolvePropertyType(String property)
        {
            return typeof(String);
        }

        internal override EventPropertyGetter DoResolvePropertyGetter(String property)
        {
            TypedEventPropertyGetter getter = propertyGetterCache.Get(property);
            if (getter != null)
            {
                return getter;
            }

            XPathExpression xPathExpression;
            try
            {
                String xPathExpr = SimpleXMLPropertyParser.Parse(property, RootElementName, defaultNamespacePrefix, isResolvePropertiesAbsolute);

                //XPath xpath = getXPathFactory().newXPath();
                //xpath.setNamespaceContext(namespaceContext);

                xPathExpression = XPathExpression.Compile(xPathExpr, nsManager);
            }
            catch (XPathException e)
            {
                throw new EPException("Error constructing XPath expression from property name '" + property + "'", e);
            }

            getter = new XPathPropertyGetter(property, xPathExpression, typeof(string), null) ;
            propertyGetterCache[property] = getter;
            return getter;
        }
    }
}
