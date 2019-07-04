///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using Antlr.Runtime.Tree;
using com.espertech.esper.compat;
using com.espertech.esper.epl.generated;
using com.espertech.esper.type;
using com.espertech.esper.util;
using log4net;

namespace com.espertech.esper.epl.parse
{
    /// <summary>Parse AST parameter nodes including constants, arrays, lists. Distinguishes between uniform and non-uniform arrays. </summary>
    public class ASTParameterHelper
    {

        /// <summary>Returns the parse Object for the parameter/constant AST node whose text to parse. </summary>
        /// <param name="parameterNode">AST node to parse</param>
        /// <param name="engineTime">the engine current time</param>
        /// <returns>object value</returns>
        /// <throws>ASTWalkException is thrown to indicate a parse error</throws>
        public static Object MakeParameter(ITree parameterNode, long engineTime)
        {
            return ParseConstant(parameterNode, engineTime);
        }

        private static Object ParseConstant(ITree node, long engineTime)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".parseConstant Node type=" + node.Type + " text=" + node.Text);
            }

            switch(node.Type)
            {
                case EsperEPL2GrammarParser.NUM_INT:
                case EsperEPL2GrammarParser.INT_TYPE:
                case EsperEPL2GrammarParser.LONG_TYPE:
                case EsperEPL2GrammarParser.BOOL_TYPE:
                case EsperEPL2GrammarParser.FLOAT_TYPE:
                case EsperEPL2GrammarParser.DOUBLE_TYPE:
                case EsperEPL2GrammarParser.STRING_TYPE:               return ASTConstantHelper.Parse(node);
                case EsperEPL2GrammarParser.NUMERIC_PARAM_FREQUENCY:   return MakeFrequency(node);
                case EsperEPL2GrammarParser.NUMERIC_PARAM_RANGE:       return MakeRange(node);
                case EsperEPL2GrammarParser.LAST:
                case EsperEPL2GrammarParser.LW:
                case EsperEPL2GrammarParser.WEEKDAY_OPERATOR:
                case EsperEPL2GrammarParser.LAST_OPERATOR:             return MakeCronParameter(node, engineTime);
                case EsperEPL2GrammarParser.STAR:                      return new WildcardParameter();
                case EsperEPL2GrammarParser.NUMERIC_PARAM_LIST:        return MakeList(node, engineTime);
                case EsperEPL2GrammarParser.ARRAY_PARAM_LIST:          return MakeArray(node, engineTime);
                case EsperEPL2GrammarParser.TIME_PERIOD:               return MakeTimePeriod(node, engineTime);
                default:
                    throw new ASTWalkException("Unexpected constant of type " + node.Type + " encountered");
            }
        }

        /// <summary>
        /// Returns a time period from an AST node and taking engine time (year etc) into account.
        /// </summary>
        /// <param name="node">is the AST root node of the time period</param>
        /// <param name="engineTime">current time</param>
        /// <returns>time period</returns>
        protected internal static TimePeriodParameter MakeTimePeriod(ITree node, long engineTime)
        {
            double result = 0;
            for (int i = 0; i < node.ChildCount; i++)
            {
            	ITree child = node.GetChild(i);
                Object numValue = ParseConstant(child.GetChild(0), engineTime);
                double partValue = Convert.ToDouble(numValue);

                switch (child.Type)
                {
                    case EsperEPL2GrammarParser.MILLISECOND_PART :
                        result += partValue / 1000d;
                        break;
                    case EsperEPL2GrammarParser.SECOND_PART :
                        result += partValue;
                        break;
                    case EsperEPL2GrammarParser.MINUTE_PART :
                        result += 60 * partValue;
                        break;
                    case EsperEPL2GrammarParser.HOUR_PART :
                        result += 60 * 60 * partValue;
                        break;
                    case EsperEPL2GrammarParser.DAY_PART :
                        result += 24 * 60 * 60 * partValue;
                        break;
                    default:
                        throw new IllegalStateException("Illegal part of interval encountered, type=" + child.Type + " text=" + child.Text);
                }
            }

            return new TimePeriodParameter(result);
        }

        private static Object MakeList(ITree node, long engineTime)
        {
            ListParameter list = new ListParameter();

            for (int i = 0; i < node.ChildCount; i++)
            {
                ITree child = node.GetChild(i);
                Object parsedChild = ParseConstant(child, engineTime);

                if ( TypeHelper.IsIntegralNumber( parsedChild ) ) {
                    int value = Convert.ToInt32(parsedChild);
                    list.Add(new IntParameter(value));
                }
                else
                {
                    list.Add((NumberSetParameter) parsedChild);
                }
            }

            return list;
        }

        private static Object MakeFrequency(ITree node)
        {
            int frequency = IntValue.ParseString(node.GetChild(0).Text);
            return new FrequencyParameter(frequency);
        }

        private static Object MakeRange(ITree node)
        {
            int low = IntValue.ParseString(node.GetChild(0).Text);
            int high = IntValue.ParseString(node.GetChild(1).Text);
            return new RangeParameter(low, high);
        }

        private static Object MakeCronParameter(ITree node, long engineTime)
        {
           if (node.GetChild(0) == null) {
              return new CronParameter(node.Type, null, engineTime);
           }
           else {
              return new CronParameter(node.Type, node.GetChild(0).Text, engineTime);
           }
        }

        private static Object MakeArray(ITree node, long engineTime)
        {
            // Determine the distinct node types in the AST
            Set<int> nodeTypes = new HashSet<int>();

            for (int i = 0; i < node.ChildCount; i++)
            {
                ITree childNode = node.GetChild(i);
                nodeTypes.Add(childNode.Type);
            }

            if (CollectionHelper.IsEmpty(nodeTypes))
            {
                return new Object[0];
            }
            else if (nodeTypes.Count == 1)
            {
                return MakeUniform(node);
            }
            else
            {
                return MakeNonUniform(node, engineTime);
            }
        }

        private static Object MakeNonUniform(ITree node, long engineTime)
        {
            int count = node.ChildCount;
            Object[] result = new Object[count];

            for (int i = 0; i < node.ChildCount; i++)
            {
                ITree child = node.GetChild(i);
                result[i] = ParseConstant(child, engineTime);
            }

            return result;
        }

        private static Object MakeUniform(ITree node)
        {
            int count = node.ChildCount;
            String[] values = new String[count];

            for (int i = 0; i < node.ChildCount; i++)
            {
                ITree child = node.GetChild(i);
                values[i] = child.Text;
            }

            return ParseStringArray(node.GetChild(0).Type, values);
        }

        private static Object ParseStringArray(int nodeType, String[] nodeValues)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".parseStringArray Node type=" + nodeType + " values=" + CollectionHelper.Render(nodeValues));
            }

            switch(nodeType)
            {
                case EsperEPL2GrammarParser.INT_TYPE:  return IntValue.ParseString(nodeValues);
                case EsperEPL2GrammarParser.LONG_TYPE:  return LongValue.ParseString(nodeValues);
                case EsperEPL2GrammarParser.BOOL_TYPE:  return BoolValue.ParseString(nodeValues);
                case EsperEPL2GrammarParser.FLOAT_TYPE:  return FloatValue.ParseString(nodeValues);
                case EsperEPL2GrammarParser.DOUBLE_TYPE:  return DoubleValue.ParseString(nodeValues);
                case EsperEPL2GrammarParser.STRING_TYPE:  return StringValue.ParseString(nodeValues);
                default:
                    throw new IllegalStateException("Unexpected constant of type " + nodeType + " encountered");
            }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
