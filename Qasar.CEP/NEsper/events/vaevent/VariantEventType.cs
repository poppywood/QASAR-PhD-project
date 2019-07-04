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

namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// Event type for variant event streams.
    /// <para/>
    /// Caches properties after having resolved a property via a resolution strategy.
    /// </summary>
    public class VariantEventType : EventType
    {
        private readonly EventType[] variants;
        private readonly VariantPropResolutionStrategy propertyResStrategy;
        private readonly Map<String, VariantPropertyDesc> propertyDesc;
        private readonly String[] propertyNames;
    
        /// <summary>Ctor. </summary>
        /// <param name="variantSpec">the variant specification</param>
        /// <param name="propertyResStrategy">stragegy for resolving properties</param>
        public VariantEventType(VariantSpec variantSpec, VariantPropResolutionStrategy propertyResStrategy)
        {
            this.variants = variantSpec.EventTypes;
            this.propertyResStrategy = propertyResStrategy;
            propertyDesc = new HashMap<String, VariantPropertyDesc>();
    
            // for each of the properties in each type, attempt to load the property to build a property list
            foreach (EventType type in variants)
            {
                IList<String> properties = new List<String>(type.PropertyNames);
                properties = PropertyUtility.CopyAndSort(properties);
                PropertyUtility.RemovePropNamePostfixes(properties);
                foreach (String property in properties)
                {
                    if (!propertyDesc.ContainsKey(property))
                    {
                        FindProperty(property);
                    }
                }
            }
            propertyNames = CollectionHelper.ToArray(propertyDesc.Keys);
        }
    
        public Type GetPropertyType(String property)
        {
            VariantPropertyDesc entry = propertyDesc.Get(property);
            if (entry != null)
            {
                return entry.PropertyType;
            }
            entry = FindProperty(property);
            if (entry != null)
            {
                return entry.PropertyType;
            }
            return null;
        }

        public Type UnderlyingType
        {
            get { return typeof (Object); }
        }

        public EventPropertyGetter GetGetter(String property)
        {
            VariantPropertyDesc entry = propertyDesc.Get(property);
            if (entry != null)
            {
                return entry.Getter;
            }
            entry = FindProperty(property);
            if (entry != null)
            {
                return entry.Getter;
            }
            return null;
        }

        public ICollection<string> PropertyNames
        {
            get { return propertyNames; }
        }

        public bool IsProperty(String property)
        {
            VariantPropertyDesc entry = propertyDesc.Get(property);
            if (entry != null)
            {
                return entry.IsProperty;
            }
            entry = FindProperty(property);
            if (entry != null)
            {
                return entry.IsProperty;
            }
            return false;
        }

        public IEnumerable<EventType> SuperTypes
        {
            get { return null; }
        }

        public IEnumerable<EventType> DeepSuperTypes
        {
            get { return null; }
        }

        private VariantPropertyDesc FindProperty(String propertyName)
        {
            VariantPropertyDesc desc = propertyResStrategy.ResolveProperty(propertyName, variants);
            if (desc != null)
            {
                propertyDesc.Put(propertyName, desc);
            }
            return desc;
        }
    }
}
