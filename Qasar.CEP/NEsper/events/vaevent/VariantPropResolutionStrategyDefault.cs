///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.util;


namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// A property resolution strategy that allows only the preconfigured types, wherein
    /// all properties that are common (name and type) to all properties are considered.
    /// </summary>
    public class VariantPropResolutionStrategyDefault : VariantPropResolutionStrategy
    {
        private int currentPropertyNumber;
        private readonly VariantPropertyGetterCache propertyGetterCache;
    
        /// <summary>Ctor. </summary>
        /// <param name="variantSpec">specified the preconfigured types</param>
        public VariantPropResolutionStrategyDefault(VariantSpec variantSpec)
        {
            propertyGetterCache = new VariantPropertyGetterCache(variantSpec.EventTypes);
        }

        private static List<Type> GetTypeList( Type t1 )
        {
            List<Type> tHash = new List<Type>();

            for (; t1 != null; t1 = t1.BaseType ) {
                tHash.Add(t1);
            }

            return tHash;
        }

        private static Type GetCommonBaseClass( Type t1, Type t2 )
        {
            Type gcd = null;
            // Get the type list
            List<Type> t1List = GetTypeList(t1);
            List<Type> t2List = GetTypeList(t2);
            // Reverse the list so that System.Object is now at the head
            t1List.Reverse();
            t2List.Reverse();
            // Find the first case where the list varies
            int maxCount = Math.Min(t1List.Count, t2List.Count);
            for( int ii = 0 ; ii < maxCount ; ii++ ) {
                if ( t1List[ii] == t2List[ii] ) {
                    gcd = t1List[ii];
                } else {
                    return gcd;
                }
            }

            return gcd;
        }

        private static Set<Type> GetCommonInterfaces( Type t1, Type t2 )
        {
            Set<Type> commonInterfaces = SetUtil.Intersect(
                t1.GetInterfaces(),
                t2.GetInterfaces());
            return commonInterfaces;
        }

        public VariantPropertyDesc ResolveProperty(String propertyName, EventType[] variants)
        {
            bool existsInAll = true;
            Type commonType = null;
            bool mustCoerce = false;
            for (int i = 0; i < variants.Length; i++)
            {
                Type type = TypeHelper.GetBoxedType(variants[i].GetPropertyType(propertyName));
                if (type == null)
                {
                    existsInAll = false;
                    continue;
                }
    
                if (commonType == null)
                {
                    Type nType = Nullable.GetUnderlyingType(type);
                    commonType = nType ?? type;
                    continue;
                }

                Type cType = Nullable.GetUnderlyingType(type);
                if ( cType != null ) {
                    type = cType;
                }
    
                // compare types
                if (type.Equals(commonType))
                {
                    continue;
                }
    
                // coercion
                if (TypeHelper.IsNumeric(type))
                {
                    if (TypeHelper.CanCoerce(type, commonType))
                    {
                        mustCoerce = true;
                        continue;
                    }
                    if (TypeHelper.CanCoerce(commonType, type))
                    {
                        mustCoerce = true;
                        commonType = type;
                    }
                }
                else if (commonType == typeof(Object))
                {
                    continue;
                }
                // common interface or base class
                else if (!TypeHelper.IsBuiltinDataType(type))
                {
                    Type commonBaseType = GetCommonBaseClass(commonType, type);
                    if ((commonBaseType != null) && (commonBaseType != typeof(Object))) {
                        commonType = commonBaseType; // commonType isA commonBaseType
                        continue;
                    }

                    // All objects share Object as their base type and this should
                    // only be used in a case of last resort.  Check commonType and
                    // see if type implements commonType.

                    if ( TypeHelper.IsSubclassOrImplementsInterface(type, commonType) ) {
                        // "type" implements "commonType"
                        continue;
                    } else if ( TypeHelper.IsSubclassOrImplementsInterface(commonType, type)) {
                        // "commonType" implements "type"
                        commonType = type;
                        continue;
                    }

                    // Check the interfaces and see if there is any overlap.  Keep in mind that
                    // a single class may implement multiple interfaces which can make this
                    // a very difficult problem to solve.

                    Set<Type> commonInterfaces = GetCommonInterfaces(commonType, type);
                    if ( commonInterfaces.Count != 0 ) {
                        commonType = commonInterfaces.First;
                        continue;
                    }

                    // Last resort is to choose System.Object as the common denominator
                }
    
                commonType = typeof(Object);
            }
    
            if (!existsInAll)
            {
                return null;
            }
    
            if (commonType == null)
            {
                return null;
            }
    
            // property numbers should start at zero since the serve as array index
            int assignedPropertyNumber = currentPropertyNumber;
            currentPropertyNumber++;
            propertyGetterCache.AddGetters(assignedPropertyNumber, propertyName);
    
            EventPropertyGetter getter;
            if (mustCoerce)
            {
                SimpleTypeCaster caster = SimpleTypeCasterFactory.GetCaster(commonType);
                getter = new ProxyEventPropertyGetter(
                    delegate(EventBean eventBean) {
                        VariantEventBean variant = (VariantEventBean) eventBean;
                        getter = propertyGetterCache.GetGetter(assignedPropertyNumber, variant.UnderlyingEventBean.EventType);
                        if (getter == null) {
                            return null;
                        }
                        Object value = getter.GetValue(variant.UnderlyingEventBean);
                        if (value == null) {
                            return value;
                        }
                        return caster.Invoke(value);
                    },
                    delegate(EventBean eventBean) {
                        VariantEventBean variant = (VariantEventBean) eventBean;
                        getter = propertyGetterCache.GetGetter(
                            assignedPropertyNumber, variant.UnderlyingEventBean.EventType);
                        if (getter == null) {
                            return false;
                        }
                        return getter.IsExistsProperty(variant.UnderlyingEventBean);
                    });
            }
            else
            {
                getter = new ProxyEventPropertyGetter(
                    delegate(EventBean eventBean) {
                        VariantEventBean variant = (VariantEventBean) eventBean;
                        getter = propertyGetterCache.GetGetter(assignedPropertyNumber, variant.UnderlyingEventBean.EventType);
                        if (getter == null) {
                            return null;
                        }
                        return getter.GetValue(variant.UnderlyingEventBean);
                    },
                    delegate(EventBean eventBean) {
                        VariantEventBean variant = (VariantEventBean) eventBean;
                        getter = propertyGetterCache.GetGetter(assignedPropertyNumber, variant.UnderlyingEventBean.EventType);
                        if (getter == null) {
                            return false;
                        }
                        return getter.IsExistsProperty(variant.UnderlyingEventBean);
                    });
            }
    
            return new VariantPropertyDesc(commonType, getter, true);
        }
    }
}
