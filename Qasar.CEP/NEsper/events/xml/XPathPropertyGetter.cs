using System;
using System.Xml;
using System.Xml.XPath;

using com.espertech.esper.events;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.events.xml
{
	/// <summary>Getter for properties of DOM xml events.</summary>
	/// <author>pablo</author>

    public class XPathPropertyGetter : TypedEventPropertyGetter
	{
        private readonly XPathExpression expression;
        private readonly String property;
        private readonly Type resultType;
        private readonly Type boxedResultType;
        private readonly SimpleTypeParser simpleTypeParser;
        private readonly Type optionalCastToType;

        /// <summary>
        /// Returns type of event property.
        /// </summary>
        /// <value></value>
        /// <returns> class of the objects returned by this getter
        /// </returns>
		public Type ResultClass
		{
            get { return optionalCastToType ?? boxedResultType; }
		}

        /// <summary>Ctor.</summary>
        /// <param name="propertyName">is the name of the event property for which this getter gets values</param>
        /// <param name="xPathExpression">is a compile XPath expression</param>
        /// <param name="resultType">is the resulting type</param>
        /// <param name="optionalCastToType">if non-null then the return value of the xpath expression is cast to this value</param>
        public XPathPropertyGetter(String propertyName, XPathExpression xPathExpression, Type resultType, Type optionalCastToType)
		{
		    this.expression = xPathExpression;
		    this.property = propertyName;
		    this.resultType = resultType;
		    this.boxedResultType = TypeHelper.GetBoxedType(resultType);

		    if (optionalCastToType != null) {
		        simpleTypeParser = SimpleTypeParserFactory.GetParser(optionalCastToType);
		    } else {
		        simpleTypeParser = null;
		    }
		    this.optionalCastToType = optionalCastToType;
		}

		/// <summary>
		/// Gets the property from the specified event bean.
		/// </summary>
		/// <value></value>

        public virtual Object GetValue(EventBean eventBean)
		{
		    Object und = eventBean.Underlying;
		    if (und == null) {
		        throw new PropertyAccessException(
		            "Unexpected null underlying event encountered, expecting org.w3c.dom.Node instance as underlying");
		    }

		    XmlNode node = und as XmlNode;
		    if (node == null) {
		        throw new PropertyAccessException("Unexpected underlying event of type '" + und.GetType() +
		                                          "' encountered, expecting org.w3c.dom.Node as underlying");
		    }

		    try {
		        XPathNavigator navigator = node.CreateNavigator();
		        Object result = navigator.Evaluate(expression);
		        if (result == null) {
		            return null;
		        }

		        // The result of the expression (Boolean, number, string, or node set).
		        // This maps to Boolean, Double, String, or XPathNodeIterator objects
		        // respectively.

		        switch (expression.ReturnType) {
		            case XPathResultType.Boolean:
		            case XPathResultType.Number:
		            case XPathResultType.String:
		            case XPathResultType.Any:
		                result = Convert.ChangeType(result, resultType);
		                break;
		            case XPathResultType.NodeSet:
		                do {
		                    XPathNodeIterator iterator = (XPathNodeIterator) result;
		                    if (iterator.MoveNext()) {
		                        XPathNavigator current = iterator.Current;
		                        if (resultType == typeof (XmlNode)) {
		                            result = current.UnderlyingObject;
		                        } else {
		                            result = Convert.ChangeType(iterator.Current.Value, resultType);
		                        }
		                    } else {
		                        result = String.Empty;
		                    }
		                } while (false);
		                break;
		        }

		        if (optionalCastToType == null) {
		            return result;
		        }

		        // string results get parsed
		        if (result is String) {
		            try {
		                return simpleTypeParser.Invoke((String) result);
		            } catch {
		                log.Warn("Error parsing XPath property named '" + property + "' expression result '" + result +
		                         " as type " + optionalCastToType.Name);
		                return null;
		            }
		        }

		        // coercion
		        if (result is Double) {
		            try {
		                return TypeHelper.CoerceBoxed(result, optionalCastToType);
		            } catch {
		                log.Warn("Error coercing XPath property named '" + property + "' expression result '" + result +
		                         " as type " + optionalCastToType.Name);
		                return null;
		            }
		        }

		        // check boolean type
		        if (result is Boolean) {
		            if (optionalCastToType != typeof (bool)) {
		                log.Warn("Error coercing XPath property named '" + property + "' expression result '" + result +
		                         " as type " + optionalCastToType.Name);
		                return null;
		            }
		        }

		        log.Warn("Error processing XPath property named '" + property + "' expression result '" + result +
		                 ", not a known type");
		        return null;
		    } catch (XPathException e) {
		        throw new PropertyAccessException("Error getting property " + property, e);
		    }
		}

	    /// <summary>
        /// Returns true if the property exists, or false if the type does not have such a property.
        /// <para>
        /// Useful for dynamic properties of the syntax "property?" and the dynamic nested/indexed/mapped versions.
        /// Dynamic nested properties follow the syntax "property?.nested" which is equivalent to "property?.nested?".
        /// If any of the properties in the path of a dynamic nested property return null, the dynamic nested property
        /// does not exists and the method returns false.
        /// </para>
        /// 	<para>
        /// For non-dynamic properties, this method always returns true since a getter would not be available
        /// unless
        /// </para>
        /// </summary>
        /// <param name="eventBean">the event to check if the dynamic property exists</param>
        /// <returns>
        /// indictor whether the property exists, always true for non-dynamic (default) properties
        /// </returns>
        public bool IsExistsProperty(EventBean eventBean)
        {
            return true; // Property exists as the property is not dynamic (unchecked)
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
