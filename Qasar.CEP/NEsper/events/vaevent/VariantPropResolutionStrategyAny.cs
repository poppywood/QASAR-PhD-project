///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.events;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>A property resolution strategy that allows any type, wherein all properties are Object type. </summary>
    public class VariantPropResolutionStrategyAny : VariantPropResolutionStrategy
    {
        private int currentPropertyNumber;
        private readonly VariantPropertyGetterCache propertyGetterCache;
    
        /// <summary>Ctor. </summary>
        /// <param name="variantSpec">specified the preconfigured types</param>
        public VariantPropResolutionStrategyAny(VariantSpec variantSpec)
        {
            propertyGetterCache = new VariantPropertyGetterCache(variantSpec.EventTypes);
        }
    
        public VariantPropertyDesc ResolveProperty(String propertyName, EventType[] variants)
        {
            // property numbers should start at zero since the serve as array index
            int assignedPropertyNumber = currentPropertyNumber;
            currentPropertyNumber++;
            propertyGetterCache.AddGetters(assignedPropertyNumber, propertyName);

            EventPropertyGetter getter = new ProxyEventPropertyGetter(
                delegate(EventBean eventBean) {
                    VariantEventBean variant = (VariantEventBean) eventBean;
                    EventPropertyGetter _getter =
                        propertyGetterCache.GetGetter(assignedPropertyNumber, variant.UnderlyingEventBean.EventType);
                    if (_getter == null) {
                        return null;
                    }
                    return _getter.GetValue(variant.UnderlyingEventBean);
                },
                delegate(EventBean eventBean) {
                    VariantEventBean variant = (VariantEventBean) eventBean;
                    EventPropertyGetter _getter =
                        propertyGetterCache.GetGetter(assignedPropertyNumber, variant.UnderlyingEventBean.EventType);
                    if (_getter == null) {
                        return false;
                    }
                    return _getter.IsExistsProperty(variant.UnderlyingEventBean);
                });
    
            return new VariantPropertyDesc(typeof(Object), getter, true);
        }
    }
}
