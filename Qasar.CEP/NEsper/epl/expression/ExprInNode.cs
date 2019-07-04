///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Represents the in-clause (set check) function in an expression tree.
    /// </summary>

    public class ExprInNode : ExprNode
    {
        private readonly bool isNotIn;

        /// <summary>
        /// Returns the type that the node's evaluate method returns an instance of.
        /// </summary>
        /// <value>The type.</value>
        /// <returns> type returned when evaluated
        /// </returns>
        /// <throws>ExprValidationException thrown when validation failed </throws>
        public override Type ReturnType
        {
            get { return typeof(bool?); }
        }

        /// <summary>Returns true for not-in, false for regular in</summary>
	    /// <returns>
	    /// false for &quot;val in (a,b,c)&quot; or true for &quot;val not in (a,b,c)&quot;
	    /// </returns>
	    public bool IsNotIn
	    {
	        get { return isNotIn; }
	    }

	    public override bool IsConstantResult
	    {
	        get { return false; }
	    }

        /// <summary> Ctor.</summary>
        /// <param name="isNotIn">is true for "not in" and false for "in"
        /// </param>
        public ExprInNode(bool isNotIn)
        {
            this.isNotIn = isNotIn;
        }

        /// <summary>
        /// Returns true if the test collection is wholy contained in the "reference" collection.
        /// </summary>
        /// <param name="referenceCollection"></param>
        /// <param name="testCollection"></param>
        /// <returns></returns>
        private delegate bool InElementDelegate(Object referenceCollection, IEnumerable<Object> testCollection);

        /// <summary>
        /// Gets a contains delegate for the type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns></returns>

        private static InElementDelegate GetInElementDelegate(Type type, Type sourceType)
        {
            InElementDelegate elementDelegate = GetNullDelegate(type);
            if (elementDelegate != null)
                return elementDelegate;
            
            elementDelegate = GetArrayDelegate(type);
            if (elementDelegate != null)
                return elementDelegate;
            
            elementDelegate = GetDictionaryDelegate(type);
            if (elementDelegate != null)
                return elementDelegate;

            // strings are special case collections; we explicitly exclude
            // them from this test to ensure that ethings that return strings
            // do not behave as if it is a collection of characters.
            if (type != typeof(string))
            {
                elementDelegate = GetCollectionDelegate(type);
                if (elementDelegate != null)
                    return elementDelegate;
            }

            elementDelegate = GetNonCollectionDelegate(type, sourceType);

            return elementDelegate;
        }

        private static InElementDelegate GetNullDelegate(Type type)
        {
            if (type == null) {
                return delegate(Object referenceObject, IEnumerable<Object> testCollection) {
                           // Test every item in the test collection
                           foreach (Object value in testCollection) {
                               if (!Equals(value, referenceObject)) {
                                   return false;
                               }
                           }

                           return true;
                       };
            }

            return null;
        }

        /// <summary>
        /// Gets a delegate for non-collection types.
        /// </summary>
        /// <param name="targetType">The type.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns></returns>
        private static InElementDelegate GetNonCollectionDelegate(Type targetType, Type sourceType)
        {
            bool isSourceNumeric = TypeHelper.IsNumeric(sourceType);
            bool isTargetNumeric = TypeHelper.IsNumeric(targetType);
            if ( isSourceNumeric != isTargetNumeric )
                return null;

            // Not sure that I completely agree with this logic, but the way that esper works is that
            // type coercion (not the type caster) is used to coerce from one type to another.  Under
            // that model, non-numerics can not safely be cast to numerics and validation fails.  To
            // ensure consistency (though I will raise the issue), we check that the source and
            // target targetType are both numeric conversions.

            SimpleTypeCaster typeCaster = SimpleTypeCasterFactory.GetCaster(targetType);
            if (typeCaster == null)
                return null;

            return delegate(Object referenceObject, IEnumerable<Object> testCollection)
            {
                // Test every item in the test collection
                foreach (Object value in testCollection)
                {
                    // Cast the object targetType
                    Object tObject = typeCaster(value);
                    // Test the reference object and test object
                    if ( ! Equals(referenceObject, tObject) )
                    {
                        return false;
                    }
                }

                return true;
            };
        }

        /// <summary>
        /// Gets a delegate for an array type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static InElementDelegate GetArrayDelegate(Type type)
        {
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                SimpleTypeCaster typeCaster = SimpleTypeCasterFactory.GetCaster(elementType);

                return delegate(Object referenceCollection, IEnumerable<Object> testCollection) {
                           // Convert the referenceCollection to its target type
                           Array array = (Array) referenceCollection;
                           if (array == null)
                               return false;

                           int length = array.Length;
                           // Test every item in the test collection
                           foreach (Object value in testCollection) {
                               // Cast the object type
                               Object tObject = typeCaster(value);
                               // Find the object in the reference collection
                               bool itemFound = false;
                               for (int ii = 0; ii < length; ii++) {
                                   Object element = array.GetValue(ii);
                                   if (Equals(element, tObject)) {
                                       itemFound = true;
                                       break;
                                   }
                               }

                               if (! itemFound) {
                                   return false;
                               }
                           }

                           return true;
                       };
            }

            return null;
        }

        /// <summary>
        /// Gets a delegate for container types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is collection type] [the specified t]; otherwise, <c>false</c>.
        /// </returns>
        private static InElementDelegate GetCollectionDelegate(Type type)
        {
            // Honestly, type introspection is difficult work.  What we want to know is whether
            // or not we can tell if an object is a "container" and whether we can test for
            // containment.  In order to test for containment, we look for a "Contains" method.

            MethodInfo containsMethod = type.GetMethod("Contains");
            if (containsMethod != null)
            {
                if (containsMethod.ReturnType == typeof(bool))
                {
                    ParameterInfo[] paramInfoList = containsMethod.GetParameters();
                    if (paramInfoList.Length == 1)
                    {
                        ParameterInfo keyParam = paramInfoList[0];
                        SimpleTypeCaster typeCaster = SimpleTypeCasterFactory.GetCaster(keyParam.ParameterType);
                        return delegate(Object referenceCollection, IEnumerable<Object> testCollection)
                                   {
                                       if (referenceCollection == null)
                                           return false;
                                       // Test every item in the test collection
                                       foreach (Object value in testCollection) {
                                           // Cast the object type
                                           Object tObject = typeCaster(value);
                                           if (!((bool) containsMethod.Invoke(referenceCollection, new object[] {tObject}))) {
                                               return false;
                                           }
                                       }
                                       return true;
                                   };
                    }
                }
            }

            return null;
        }

        private static MethodInfo FindMethod( Type type, String methodname )
        {
            MethodInfo method = type.GetMethod(methodname);
            if (method != null) {
                return method;
            }

            Type baseType = type.BaseType;
            if (baseType != null) {
                if ((method = FindMethod(baseType, methodname)) != null) {
                    return method;
                }
            }

            foreach (Type iType in type.GetInterfaces()) {
                method = FindMethod(iType, methodname);
                if (method != null) {
                    return method;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a delegate for container types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is collection type] [the specified t]; otherwise, <c>false</c>.
        /// </returns>
        private static InElementDelegate GetDictionaryDelegate(Type type)
        {
            // Honestly, type introspection is difficult work.  What we want to know is whether
            // or not we can tell if an object is a "container" and whether we can test for
            // containment.  In order to test for containment, we look for a "Contains" method.

            MethodInfo containsMethod = FindMethod(type, "ContainsKey");
            if (containsMethod != null)
            {
                if (containsMethod.ReturnType == typeof(bool))
                {
                    ParameterInfo[] paramInfoList = containsMethod.GetParameters();
                    if (paramInfoList.Length == 1) {
                        ParameterInfo keyParam = paramInfoList[0];
                        SimpleTypeCaster typeCaster = SimpleTypeCasterFactory.GetCaster(keyParam.ParameterType);
                        return delegate(Object referenceCollection, IEnumerable<Object> testCollection)
                                   {
                                       if (referenceCollection == null)
                                           return false;
                                       // Test every item in the test collection
                                       foreach (Object value in testCollection) {
                                           // Cast the object type
                                           Object tObject = typeCaster(value);
                                           if (!(bool) containsMethod.Invoke(referenceCollection, new object[] {tObject})) {
                                               return false;
                                           }
                                       }
                                       return true;
                                   };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the type ident.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        private static String GetTypeIdent( Type t )
        {
            return t != null ? t.FullName : "null";
        }

        private delegate bool TestElementDelegate(IEnumerable<Object> testCollection, EventBean[] eventsPerStream, bool isNewData);

        private readonly List<TestElementDelegate> testElementList = new List<TestElementDelegate>();

        /// <summary>
        /// Validate node.
        /// </summary>
        /// <param name="streamTypeService">serves stream event type info</param>
        /// <param name="methodResolutionService">for resolving class names in library method invocations</param>
        /// <param name="viewResourceDelegate">The view resource delegate.</param>
        /// <param name="timeProvider">provides engine current time</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <throws>ExprValidationException thrown when validation failed </throws>
        public override void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService)
        {
        	IList<ExprNode> children = ChildNodes ;
            if (children.Count < 2) {
                throw new ExprValidationException("The IN operator requires at least 2 child expressions");
            }

            // Get the value expression and the value expression return type
            Type sourceObjectType = children[0].ReturnType;

            // Generate a delegate that can be used to traverse the entire chain of subnodes
            for( int ii = 1 ; ii < children.Count ; ii++ ) {
                ExprNode subExprNode = children[ii];
                Type subExprType = subExprNode.ReturnType;

                // Creating the right InElementDelegate is an art.  We know that the input will be normalized
                // as a collection of objects, so its simply a matter of determining whether the set of objects
                // represented in the referenceCollection are wholy contained within the value that propType
                // will manage.  If propType is some type of a Map, then we want to use a delegate that does
                // a comparison of the referenceObject against the keys of the map.  If the propType is a
                // different type of collection, then we simply want to use a delegate that looks for the
                // referenceObject in the collection.  Last, if none of the above is true, then we do a straight
                // comparison.
                InElementDelegate inElementDelegate = GetInElementDelegate(subExprType, sourceObjectType);
                if (inElementDelegate == null)
                    throw new ExprValidationException(
                        "Unable to find a delegate to for 'in' evaluation for " +
                        " source='" + GetTypeIdent(sourceObjectType) + "' " +
                        " and " +
                        " target='" + GetTypeIdent(subExprType) + "'");

                // Create a wrapper that is now used to extract each node, evaluate the values and
                // then call the inElementDelegate.
                TestElementDelegate testElementDelegate =
                    delegate(IEnumerable<Object> testCollection, EventBean[] eventsPerStream, bool isNewData) {
                        Object referenceCollection = subExprNode.Evaluate(eventsPerStream, isNewData);
                        return inElementDelegate(referenceCollection, testCollection);
                    };
                testElementList.Add(testElementDelegate);
            }
        }

        private static ICollection<Object> MakeObjectCollection(Object value)
        {
            if (value is Array) {
                List<Object> arrayList = new List<Object>();
                foreach (Object o in (Array) value) {
                    arrayList.Add(o);
                }
                return arrayList;
            } else if (value is ICollection<Object>) {
                return (ICollection<Object>) value;
            } else if (value is ICollection) {
                List<Object> arrayList = new List<Object>();
                foreach (Object o in (ICollection) value) {
                    arrayList.Add(o);
                }
                return arrayList;
            } else {
                List<Object> arrayList = new List<Object>();
                arrayList.Add(value);
                return arrayList;
            }
        }

        /// <summary>
        /// Evaluate event tuple and return result.
        /// </summary>
        /// <param name="eventsPerStream">event tuple</param>
        /// <param name="isNewData">is the data new</param>
        /// <returns>
        /// evaluation result, a bool value for OR/AND-type evalution nodes.
        /// </returns>
        public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
        {
            IList<ExprNode> childNodeList = ChildNodes;

            // Evaluate first child which is the base value to compare to
            Object inPropResult = childNodeList[0].Evaluate(eventsPerStream, isNewData);
            ICollection<Object> referenceCollection = MakeObjectCollection(inPropResult);

            foreach( TestElementDelegate testElementDelegate in testElementList )
            {
                if (testElementDelegate.Invoke(referenceCollection, eventsPerStream, isNewData)) {
                    return !isNotIn;
                }
            }

            return isNotIn;
        }

        /// <summary>
        /// Returns true if the objects are equal.
        /// </summary>
        /// <param name="node">The node to compare against.</param>
        /// <returns></returns>
        public override bool EqualsNode(ExprNode node)
        {
            ExprInNode other = node as ExprInNode;
            return other != null
                    ? other.isNotIn == isNotIn
                    : false;
        }

        /// <summary>
        /// Returns the expression node rendered as a string.
        /// </summary>
        /// <value></value>
        /// <returns> string rendering of expression
        /// </returns>
        public override String ExpressionString
        {
            get
            {
                StringBuilder buffer = new StringBuilder();
                String delimiter = "";

                IEnumerator<ExprNode> it = ChildNodes.GetEnumerator();
                it.MoveNext() ;

                buffer.Append(it.Current.ExpressionString);
                if (isNotIn)
                {
                    buffer.Append(" not in (");
                }
                else
                {
                    buffer.Append(" in (");
                }

                while( it.MoveNext() )
                {
                    ExprNode inSetValueExpr = it.Current;
                    buffer.Append(delimiter);
                    buffer.Append(inSetValueExpr.ExpressionString);
                    delimiter = ",";
                }

                buffer.Append(")");
                return buffer.ToString();
            }
        }
    }
}
