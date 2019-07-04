///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Reflection;

using CGLib;

using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Getter for a dynamic indexed property (syntax field.indexed[0]?),
    /// using vanilla reflection.
    /// </summary>
    public class DynamicIndexedPropertyGetter : DynamicPropertyGetterBase, EventPropertyGetter
    {
        private readonly String propertyName;
        private readonly String getterMethodName;
        private readonly Object[] paramList;
        private readonly int index;

        /// <summary>Ctor.</summary>
        /// <param name="fieldName">property name</param>
        /// <param name="index">index to get the element at</param>
        public DynamicIndexedPropertyGetter(String fieldName, int index)
        {
            this.propertyName = GetPropertyName(fieldName);
            getterMethodName = GetGetterMethodName(fieldName);
            this.paramList = new Object[] { index };
            this.index = index;
        }

        protected override ValueGetter DetermineGetter(Type clazz)
        {
            ValueGetter valueGetter;

            MethodInfo method = clazz.GetMethod(getterMethodName, new Type[] {typeof (int)});
            if (method == null)
            {
                // Did not find a method that took one argument and returned
                // a single item.  Check for a method or property that takes
                // no arguments, but returns an array.

                method = FastClassUtil.GetGetMethodForProperty(clazz, propertyName);
                if (method == null)
                {
                    method = clazz.GetMethod(getterMethodName, new Type[] {});
                }

                if (method == null)
                {
                    return null;
                }

                // This path assumes that the return value from the method is
                // an array, otherwise the index does not make sense.  Keep in
                // mind that the check for a method with the index as a parameter
                // has already failed.

                if (method.ReturnType.IsArray)
                {
                    FastMethod fastMethod = FastClass.CreateMethod(method);
                    valueGetter =
                        delegate(Object underlying)
                            {
                                Array array = (Array) fastMethod.Invoke(underlying);
                                return array.GetValue(index);
                            };
                }
                else
                {
                    return null;
                }
            }
            else
            {
                FastMethod fastMethod = FastClass.CreateMethod(method);
                valueGetter =
                    delegate(Object underlying)
                        {
                            return fastMethod.Invoke(underlying, index);
                        };
            }

            return valueGetter;
        }
    }
} // End of namespace
