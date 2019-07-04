///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

using CGLib;

using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.util;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Getter for a dynamic mapped property (syntax field.Mapped('key')?), using vanilla reflection.
    /// </summary>
	public class DynamicMappedPropertyGetter : DynamicPropertyGetterBase, EventPropertyGetter
	{
        private readonly String propertyName;
	    private readonly String getterMethodName;
        private readonly String key;

	    /// <summary>Ctor.</summary>
	    /// <param name="fieldName">property name</param>
	    /// <param name="key">mapped access key</param>
	    public DynamicMappedPropertyGetter(String fieldName, String key)
	    {
            this.propertyName = GetPropertyName(fieldName);
	        getterMethodName = GetGetterMethodName(fieldName);
	        this.key = key;
	    }

        /// <summary>
        /// Static method to help with the extraction of values from a
        /// dictionary.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected static Object GetFromDictionary<K,V>( Object dictionary, K key )
        {
            System.Collections.Generic.IDictionary<K, V> gdictionary =
                (System.Collections.Generic.IDictionary<K, V>) dictionary;
            V value;
            return
                gdictionary.TryGetValue(key, out value)
                    ? value
                    : default(V);
        }

        private static readonly MethodInfo _GetFromDictionary = MethodBase
            .GetCurrentMethod()
            .DeclaringType
            .GetMethod("GetFromDictionary", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly Type baseGenericDictionary =
            typeof(System.Collections.Generic.IDictionary<Object, Object>).GetGenericTypeDefinition();

        /// <summary>
        /// Finds the generic dictionary interface.
        /// </summary>
        /// <param name="clazz">The clazz.</param>
        /// <returns></returns>
        protected static Type FindGenericDictionaryInterface(Type clazz)
        {
            foreach (Type iface in clazz.GetInterfaces())
            {
                if (iface.IsGenericType)
                {
                    Type baseIFace = iface.GetGenericTypeDefinition();
                    if (baseIFace == baseGenericDictionary)
                    {
                        Type[] genericParameterTypes = iface.GetGenericArguments();

                        // If the key-type of the dictionary is either a string or an
                        // object, then we can use the method.  The return type is irrelevant
                        // since it will be boxed on the way out and represents an object.

                        if ((genericParameterTypes[0] == typeof(string)) ||
                            (genericParameterTypes[0] == typeof(object)))
                        {
                            return iface;
                        }
                    }
                }
            }

            return null;
        }

        protected override ValueGetter DetermineGetter(Type clazz)
        {
            // What if the item we are being passed is a dictionary?  In
            // that case, the getter should really scan the dictionary to
            // find the entry.  If its not, then we need to look for a
            // method or property on the type that might get us the value
            // we are looking.

            Type iface = FindGenericDictionaryInterface(clazz);
            if ( iface != null )
            {
                Type[] genericParameterTypes = iface.GetGenericArguments();
                MethodInfo extractionMethod = _GetFromDictionary.MakeGenericMethod(
                    genericParameterTypes[0],
                    genericParameterTypes[1]);
                FastMethod fastExtractionMethod = FastClass.CreateMethod(extractionMethod);
                return delegate(Object underlying)
                           {
                               return fastExtractionMethod.InvokeStatic(underlying, key);
                           };
            }

            // Look for a method that matches the getter name of the property
            // and takes a solitary string as an argument.

            MethodInfo method = clazz.GetMethod(getterMethodName, new Type[] {typeof (String)});
            if ( method != null )
            {
                FastMethod fastMethod = FastClass.CreateMethod(method);
                return delegate(Object underlying)
                           {
                               return fastMethod.Invoke(underlying, key);
                           };
            }

            // Did not find a method that took one argument and returned
            // a single item.  Check for a method or property that takes
            // no arguments, but returns a dictionary (one of those we
            // understand).
            method = FastClassUtil.GetGetMethodForProperty(clazz, propertyName);
            if (method == null)
            {
                method = clazz.GetMethod(getterMethodName, new Type[] { });
            }

            // Did we find a method?
            if (method == null)
            {
                return null;
            }

            // Is this a non-generic IDictionary
            Type returnType = method.ReturnType;
            if ( typeof(System.Collections.IDictionary).IsAssignableFrom(returnType))
            {
                FastMethod fastMethod = FastClass.CreateMethod(method);
                return delegate(Object underlying)
                           {
                               System.Collections.IDictionary dictionary =
                                   (System.Collections.IDictionary) fastMethod.Invoke(underlying, key);
                               return dictionary[key];
                           };
            }

            // Is this a generic IDictionary ... these can be much more difficult
            // to deal with.
            iface = FindGenericDictionaryInterface(returnType);
            if ( iface != null )
            {
                Type[] genericParameterTypes = iface.GetGenericArguments();
                MethodInfo extractionMethod = _GetFromDictionary.MakeGenericMethod(
                    genericParameterTypes[0],
                    genericParameterTypes[1]);
                FastMethod fastExtractionMethod = FastClass.CreateMethod(extractionMethod);
                FastMethod fastMethod = FastClass.CreateMethod(method);
                return delegate(Object underlying)
                           {
                               Object dictionary = fastMethod.Invoke(underlying);
                               return fastExtractionMethod.InvokeStatic(dictionary, key);
                           };
            }

            // Could not find any way to represent this return type.
            return null;
        }

        public override bool IsExistsProperty(EventBean eventBean)
        {
            return base.IsExistsProperty(eventBean);
        }
	}
} // End of namespace
