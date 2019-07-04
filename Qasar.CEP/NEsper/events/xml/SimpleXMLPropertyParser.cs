using System;
using System.IO;
using System.Text;

using Antlr.Runtime;
using Antlr.Runtime.Tree;

using com.espertech.esper.antlr;
using com.espertech.esper.compat;
using com.espertech.esper.epl.generated;
using com.espertech.esper.events;
using com.espertech.esper.type;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.events.xml
{
	/// <summary> Parses event property names and transforms to XPath expressions. Supports
	/// nested, indexed and mapped event properties.
	/// </summary>
	
    public class SimpleXMLPropertyParser
	{
        /// <summary>
        /// Return the xPath corresponding to the given property.
        /// The propertyName String may be simple, nested, indexed or mapped.
        /// </summary>
        /// <param name="propertyName">is the property name to parse</param>
        /// <param name="rootElementName">is the name of the root element for generating the XPath expression</param>
        /// <param name="defaultNamespacePrefix">The default namespace prefix.</param>
        /// <param name="isResolvePropertiesAbsolute">if set to <c>true</c> [is resolve properties absolute].</param>
        /// <returns>xpath expression</returns>
        /// <throws>  XPathExpressionException </throws>
		
        public static String Parse(String propertyName, String rootElementName, String defaultNamespacePrefix, bool isResolvePropertiesAbsolute)
		{
			ITree ast = Parse(propertyName);
			
			StringBuilder xPathBuf = new StringBuilder();
			xPathBuf.Append('/');
            if (isResolvePropertiesAbsolute)
            {
                if (defaultNamespacePrefix != null)
                {
                    xPathBuf.Append(defaultNamespacePrefix);
                    xPathBuf.Append(':');
                }
                xPathBuf.Append(rootElementName);
            }
			
			if (ast.ChildCount == 1)
			{
                xPathBuf.Append(MakeProperty(ast.GetChild(0), defaultNamespacePrefix)); 
			}
			else
			{
                for (int i = 0; i < ast.ChildCount; i++)
                {
    				ITree child = ast.GetChild(i);
                    xPathBuf.Append(MakeProperty(child, defaultNamespacePrefix));
				}
			}
			
			String xPath = xPathBuf.ToString();

            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
		        log.Debug(".parse For property '" + propertyName + "' the xpath is '" + xPath + "'");
		    }

            return xPath;
		    //return XPathExpression.Compile( xPath );
		}

        private static String MakeProperty(ITree child, String defaultNamespacePrefix)
        {
            String prefix = "";
            if (defaultNamespacePrefix != null)
            {
                prefix = defaultNamespacePrefix + ":";
            }

            switch (child.Type)
            {
                case EsperEPL2GrammarParser.EVENT_PROP_DYNAMIC_SIMPLE:
                case EsperEPL2GrammarParser.EVENT_PROP_SIMPLE:
                    return '/' + prefix + child.GetChild(0).Text;
                case EsperEPL2GrammarParser.EVENT_PROP_DYNAMIC_MAPPED:
                case EsperEPL2GrammarParser.EVENT_PROP_MAPPED:
                    String key = StringValue.ParseString(child.GetChild(1).Text);
                    return '/' + prefix + child.GetChild(0).Text + "[@id='" + key + "']";
                case EsperEPL2GrammarParser.EVENT_PROP_DYNAMIC_INDEXED:
                case EsperEPL2GrammarParser.EVENT_PROP_INDEXED:
                    int index = IntValue.ParseString(child.GetChild(1).Text);
                    return '/' + prefix + child.GetChild(0).Text + "[position() = " + index + ']';
                default:
                    throw new IllegalStateException("Event property AST node not recognized, type=" + child.Type);
            }
        }
		
		/// <summary> Parses a given property name returning an AST.</summary>
		/// <param name="propertyName">to parse
		/// </param>
		/// <returns> AST syntax tree
		/// </returns>

		internal static ITree Parse(String propertyName)
		{
            ICharStream input;
            try
            {
                input = new NoCaseSensitiveStream(propertyName);
            }
            catch (IOException ex)
            {
                throw new PropertyAccessException("IOException parsing property name '" + propertyName + '\'', ex);
            }

            EsperEPL2GrammarLexer lex = new EsperEPL2GrammarLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lex);
            EsperEPL2GrammarParser g = new EsperEPL2GrammarParser(tokens);
            EsperEPL2GrammarParser.startEventPropertyRule_return r;

            try
            {
                r = g.startEventPropertyRule();
            }
            catch (RecognitionException e)
            {
                throw new PropertyAccessException("Failed to parse property name '" + propertyName + '\'', e);
            }

            return (ITree)r.Tree;
        }
		
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
}