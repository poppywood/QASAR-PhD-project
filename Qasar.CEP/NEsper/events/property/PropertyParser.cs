///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.IO;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using com.espertech.esper.antlr;
using com.espertech.esper.compat;
using com.espertech.esper.epl.generated;
using com.espertech.esper.events;
using com.espertech.esper.type;
using com.espertech.esper.util;

using log4net;


namespace com.espertech.esper.events.property
{
    /// <summary>Parser for property names that can be simple, nested, mapped or a combination of these. Uses ANTLR parser to parse. </summary>
    public class PropertyParser
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
        /// <summary>Parse the given property name returning a Property instance for the property. </summary>
        /// <param name="propertyName">is the property name to parse</param>
        /// <param name="beanEventTypeFactory">is the chache and factory for event bean types and event wrappers</param>
        /// <param name="isRootedDynamic">is true to indicate that the property is already rooted in a dynamicproperty and therefore all child properties should be dynamic properties as well </param>
        /// <returns>Property instance for property</returns>
        public static Property Parse(String propertyName, BeanEventTypeFactory beanEventTypeFactory, bool isRootedDynamic)
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

            ITree tree = (ITree)r.Tree;
    
            if ((ExecutionPathDebugLog.isDebugEnabled) && (log.IsDebugEnabled))
            {
                ASTUtil.DumpAST(tree);
            }
    
            if (tree.ChildCount == 1)
            {
                return MakeProperty(tree.GetChild(0), isRootedDynamic);
            }
    
            List<Property> properties = new List<Property>();
            bool isRootedInDynamic = isRootedDynamic;
            for (int i = 0; i < tree.ChildCount; i++)
            {
                ITree child = tree.GetChild(i);
    
                Property property = MakeProperty(child, isRootedInDynamic);
                if (property is DynamicSimpleProperty)
                {
                    isRootedInDynamic = true;
                }
                properties.Add(property);
            }
    
            return new NestedProperty(properties, beanEventTypeFactory);
        }
    
        private static Property MakeProperty(ITree child, bool isRootedInDynamic)
        {
            switch (child.Type) {
                case EsperEPL2GrammarParser.EVENT_PROP_SIMPLE:
                    if (!isRootedInDynamic)
                    {
                        return new SimpleProperty(child.GetChild(0).Text);
                    }
                    else
                    {
                        return new DynamicSimpleProperty(child.GetChild(0).Text);
                    }
                case EsperEPL2GrammarParser.EVENT_PROP_MAPPED:
                    String key = StringValue.ParseString(child.GetChild(1).Text);
                    if (!isRootedInDynamic)
                    {
                        return new MappedProperty(child.GetChild(0).Text, key);
                    }
                    else
                    {
                        return new DynamicMappedProperty(child.GetChild(0).Text, key);
                    }
                case EsperEPL2GrammarParser.EVENT_PROP_INDEXED:
                    int index = IntValue.ParseString(child.GetChild(1).Text);
                    if (!isRootedInDynamic)
                    {
                        return new IndexedProperty(child.GetChild(0).Text, index);
                    }
                    else
                    {
                        return new DynamicIndexedProperty(child.GetChild(0).Text, index);
                    }
                case EsperEPL2GrammarParser.EVENT_PROP_DYNAMIC_SIMPLE:
                    return new DynamicSimpleProperty(child.GetChild(0).Text);
                case EsperEPL2GrammarParser.EVENT_PROP_DYNAMIC_INDEXED:
                    index = IntValue.ParseString(child.GetChild(1).Text);
                    return new DynamicIndexedProperty(child.GetChild(0).Text, index);
                case EsperEPL2GrammarParser.EVENT_PROP_DYNAMIC_MAPPED:
                    key = StringValue.ParseString(child.GetChild(1).Text);
                    return new DynamicMappedProperty(child.GetChild(0).Text, key);
                default:
                    throw new IllegalStateException("Event property AST node not recognized, type=" + child.Type);
            }
        }
    }
}
