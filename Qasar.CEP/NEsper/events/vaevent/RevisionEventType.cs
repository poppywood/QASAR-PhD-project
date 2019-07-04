///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.epl.parse;
using com.espertech.esper.events;
using com.espertech.esper.events.property;


namespace com.espertech.esper.events.vaevent
{
    /// <summary>Event type of revision events. </summary>
    public class RevisionEventType : EventType
    {
        private readonly HashSet<String> propertyNames;
        private readonly Map<String, RevisionPropertyTypeDesc> propertyDesc;
        private readonly EventAdapterService eventAdapterService;
    
        /// <summary>Ctor. </summary>
        /// <param name="propertyDesc">describes each properties type</param>
        /// <param name="eventAdapterService">for nested property handling</param>
        public RevisionEventType(Map<String, RevisionPropertyTypeDesc> propertyDesc, EventAdapterService eventAdapterService)
        {
            this.eventAdapterService = eventAdapterService;
            this.propertyDesc = propertyDesc;
            this.propertyNames = new HashSet<String>(propertyDesc.Keys);
        }

        public EventPropertyGetter GetGetter(String propertyName)
        {
            RevisionPropertyTypeDesc desc = propertyDesc.Get(propertyName);
            if (desc != null) {
                return desc.RevisionGetter;
            }

            // dynamic property names note allowed
            if (propertyName.IndexOf('?') != -1) {
                return null;
            }

            // see if this is a nested property
            int index = ASTFilterSpecHelper.UnescapedIndexOfDot(propertyName);
            if (index == -1)
            {
                Property prop = PropertyParser.Parse(propertyName, eventAdapterService.BeanEventTypeFactory, false);
                if (prop is SimpleProperty)
                {
                    // there is no such property since it wasn't found earlier
                    return null;
                }
                String atomic = null;
                if (prop is IndexedProperty)
                {
                    IndexedProperty indexedprop = (IndexedProperty) prop;
                    atomic = indexedprop.PropertyNameAtomic;
                }
                if (prop is MappedProperty)
                {
                    MappedProperty indexedprop = (MappedProperty) prop;
                    atomic = indexedprop.PropertyNameAtomic;
                }
                desc = propertyDesc.Get(atomic);
                if (desc == null)
                {
                    return null;
                }

                Type nestedClass = desc.PropertyType as Type;
                if ( nestedClass == null ) {
                    return null;
                }

                BeanEventType complexProperty = (BeanEventType) eventAdapterService.AddBeanType(nestedClass.Name, nestedClass);
                return prop.GetGetter(complexProperty);
            }
    
            // Map event types allow 2 types of properties inside:
            //   - a property that is an Object is interrogated via property getters and BeanEventType
            //   - a property that is a Map itself is interrogated via map property getters
    
            // Take apart the nested property into a map key and a nested value class property name
            String propertyMap = ASTFilterSpecHelper.UnescapeDot(propertyName.Substring(0, index));
            String propertyNested = propertyName.Substring(index + 1);
    
            desc = propertyDesc.Get(propertyMap);
            if (desc == null)
            {
                return null;  // prefix not a known property
            }
    
            // only nested classes supported for revision event types since deep property information not currently exposed by EventType
            Type simpleClass = desc.PropertyType as Type;
            if (simpleClass != null)
            {
                // ask the nested class to resolve the property
                EventType nestedEventType = eventAdapterService.AddBeanType(simpleClass.FullName, simpleClass);
                EventPropertyGetter nestedGetter = nestedEventType.GetGetter(propertyNested);
                if (nestedGetter == null)
                {
                    return null;
                }
    
                // construct getter for nested property
                return new RevisionNestedPropertyGetter(desc.RevisionGetter, nestedGetter, eventAdapterService);
            }
            else
            {
                return null;
            }
        }
    
        public Type GetPropertyType(String propertyName)
        {
            RevisionPropertyTypeDesc desc = propertyDesc.Get(propertyName);
            if (desc != null)
            {
                return desc.PropertyType as Type;
            }
    
            // dynamic property names note allowed
            if (propertyName.IndexOf('?') != -1)
            {
                return null;
            }
    
            // see if this is a nested property
            int index = ASTFilterSpecHelper.UnescapedIndexOfDot(propertyName);
            if (index == -1)
            {
                return null;
            }
    
            // Map event types allow 2 types of properties inside:
            //   - a property that is an object is interrogated via property getters and BeanEventType
            //   - a property that is a Map itself is interrogated via map property getters
    
            // Take apart the nested property into a map key and a nested value class property name
            String propertyMap = ASTFilterSpecHelper.UnescapeDot(propertyName.Substring(0, index));
            String propertyNested = propertyName.Substring(index + 1);
    
            desc = propertyDesc.Get(propertyMap);
            if (desc == null)
            {
                return null;  // prefix not a known property
            }
            else if (desc.PropertyType is Type)
            {
                Type simpleClass = (Type)desc.PropertyType;
                EventType nestedEventType = eventAdapterService.AddBeanType(simpleClass.FullName, simpleClass);
                return nestedEventType.GetPropertyType(propertyNested);
            }
            else
            {
                return null;
            }
        }

        public Type UnderlyingType
        {
            get { return typeof (RevisionEventType); }
        }

        public ICollection<string> PropertyNames
        {
            get { return propertyNames; }
        }

        public bool IsProperty(String property)
        {
            return GetPropertyType(property) != null;
        }

        public IEnumerable<EventType> SuperTypes
        {
            get { return null; }
        }

        public IEnumerable<EventType> DeepSuperTypes
        {
            get { return null; }
        }
    }
}
