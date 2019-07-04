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
    /// A thread-safe cache for property getters per event type.
    /// <para/>
    /// Since most often getters are used in a row for the same type, keeps a row of last
    /// used getters for fast lookup based on type.
    /// </summary>
    public class VariantPropertyGetterCache
    {
        private volatile EventType[] knownTypes;
        private volatile VariantPropertyGetterRow lastUsedGetters;
        private readonly List<String> properties;
        private Map<EventType, VariantPropertyGetterRow> allGetters;
    
        /// <summary>Ctor. </summary>
        /// <param name="knownTypes">types known at cache construction type, may be an empty list for the ANY type variance.</param>
        public VariantPropertyGetterCache(EventType[] knownTypes)
        {
            this.knownTypes = knownTypes;
            allGetters = new HashMap<EventType, VariantPropertyGetterRow>();
            properties = new List<String>();
        }
    
        /// <summary>Adds the getters for a property that is identified by a property number which indexes into array of getters per type. </summary>
        /// <param name="assignedPropertyNumber">number of property</param>
        /// <param name="propertyName">to add</param>
        public void AddGetters(int assignedPropertyNumber, String propertyName)
        {
            foreach (EventType type in knownTypes)
            {
                EventPropertyGetter getter = type.GetGetter(propertyName);
    
                VariantPropertyGetterRow row = allGetters.Get(type);
                if (row == null)
                {
                    row = new VariantPropertyGetterRow(type, new EventPropertyGetter[assignedPropertyNumber + 1]);
                    allGetters.Put(type, row);
                }
                row.AddGetter(assignedPropertyNumber, getter);
            }
            properties.Add(propertyName);
        }
    
        /// <summary>Fast lookup of a getter for a property and type. </summary>
        /// <param name="assignedPropertyNumber">number of property to use as index</param>
        /// <param name="eventType">type of underlying event</param>
        /// <returns>getter</returns>
        public EventPropertyGetter GetGetter(int assignedPropertyNumber, EventType eventType)
        {
            VariantPropertyGetterRow lastGetters = lastUsedGetters;
            if ((lastGetters != null) && (lastGetters.EventType == eventType))
            {
                return lastGetters.GetterPerProp[assignedPropertyNumber];
            }
    
            VariantPropertyGetterRow row = allGetters.Get(eventType);
    
            // newly seen type (Using ANY type variance or as a subtype of an existing variance type)
            // synchronized add, if added twice then that is ok too
            if (row == null)
            {
                lock(this)
                {
                    row = allGetters.Get(eventType);
                    if (row == null)
                    {
                        row = AddType(eventType);
                    }
                }            
            }
    
            EventPropertyGetter getter = row.GetterPerProp[assignedPropertyNumber];
            lastUsedGetters = row;
            return getter;
        }
    
        private VariantPropertyGetterRow AddType(EventType eventType)
        {
            EventType[] newKnownTypes = (EventType[]) ResizeArray(knownTypes, knownTypes.Length + 1);
            newKnownTypes[newKnownTypes.Length - 1] = eventType;
    
            // create getters
            EventPropertyGetter[] getters = new EventPropertyGetter[properties.Count];
            for (int i = 0; i < properties.Count; i++)
            {
                getters[i] = eventType.GetGetter(properties[i]);
            }
    
            VariantPropertyGetterRow row = new VariantPropertyGetterRow(eventType, getters);
    
            Map<EventType, VariantPropertyGetterRow> newAllGetters = new HashMap<EventType, VariantPropertyGetterRow>();
            newAllGetters.PutAll(allGetters);
            newAllGetters.Put(eventType, row);
    
            // overlay volatiles
            knownTypes = newKnownTypes;
            allGetters = newAllGetters;
            
            return row;
        }
    
        private static Array ResizeArray(Array oldArray, int newSize)
        {
            Type elementType = oldArray.GetType().GetElementType();
            Array newArray = Array.CreateInstance(elementType, newSize);
            int oldSize = oldArray.Length;
            
            Array.Copy(
                oldArray,
                newArray,
                Math.Min(oldSize, newSize));

            return newArray;
        }
    
        private class VariantPropertyGetterRow
        {
            private readonly EventType eventType;
            private EventPropertyGetter[] getterPerProp;
    
            public VariantPropertyGetterRow(EventType eventType, EventPropertyGetter[] getterPerProp)
            {
                this.eventType = eventType;
                this.getterPerProp = getterPerProp;
            }

            public EventType EventType
            {
                get { return eventType; }
            }

            public EventPropertyGetter[] GetterPerProp
            {
                get { return getterPerProp; }
                set { this.getterPerProp = value; }
            }

            public void AddGetter(int assignedPropertyNumber, EventPropertyGetter getter)
            {
                if (assignedPropertyNumber > (getterPerProp.Length - 1))
                {
                    getterPerProp = (EventPropertyGetter[]) ResizeArray(getterPerProp, getterPerProp.Length + 10);
                }
                getterPerProp[assignedPropertyNumber] = getter;
            }
        }
    }
}
