///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.events;

using log4net;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// Utility for handling properties for the purpose of merging and versioning by revision
    /// event types. </summary>
    public class PropertyUtility
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
        /// <summary>Remove the postfixes for indexed and mapped properties that provide a clue that a property requires a index or map key parameter to return values, changing the array elements. </summary>
        /// <param name="propertyNames">to remove prefix for</param>
        public static void RemovePropNamePostfixes(IList<string> propertyNames)
        {
            for (int i = 0; i < propertyNames.Count; i++)
            {
                String property = propertyNames[i];
                if (property.EndsWith("[]"))
                {
                    property = property.Replace("[]", "");
                }
                if (property.EndsWith("()"))
                {
                    property = property.Replace("()", "");
                }
                propertyNames[i] = property;
            }
        }
        
        /// <summary>Returns a multi-key for an event and key property getters </summary>
        /// <param name="event">to get keys for</param>
        /// <param name="keyPropertyGetters">getters to use</param>
        /// <returns>key</returns>
        public static MultiKeyUntyped GetKeys(EventBean @event, EventPropertyGetter[] keyPropertyGetters)
        {
            Object[] keys = new Object[keyPropertyGetters.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keyPropertyGetters[i].GetValue(@event);
            }
            return new MultiKeyUntyped(keys);
        }
    
        /// <summary>From a list of property groups that include contributing event types, build a map of contributing event types and their type descriptor. </summary>
        /// <param name="groups">property groups</param>
        /// <param name="changesetProperties">properties that change between groups</param>
        /// <param name="keyProperties">key properties</param>
        /// <returns>map of event type and type information</returns>
        public static Map<EventType, RevisionTypeDesc> GetPerType(PropertyGroupDesc[] groups, String[] changesetProperties, String[] keyProperties)
        {
            Map<EventType, RevisionTypeDesc> perType = new HashMap<EventType, RevisionTypeDesc>();
            foreach (PropertyGroupDesc group in groups)
            {
                foreach (EventType type in group.Types.Keys)
                {
                    EventPropertyGetter[] changesetGetters = GetGetters(type, changesetProperties);
                    EventPropertyGetter[] keyGetters = GetGetters(type, keyProperties);
                    RevisionTypeDesc pair = new RevisionTypeDesc(keyGetters, changesetGetters, group);
                    perType.Put(type, pair);
                }
            }
            return perType;
        }
    
        /// <summary>From a list of property groups that include multiple group numbers for each property, make a map of group numbers per property. </summary>
        /// <param name="groups">property groups</param>
        /// <returns>map of property name and group number</returns>
        public static Map<String, int[]> GetGroupsPerProperty(PropertyGroupDesc[] groups)
        {
            Map<String, int[]> groupsNumsPerProp = new HashMap<String, int[]>();
            foreach (PropertyGroupDesc group in groups)
            {
                foreach (String property in group.Properties)
                {
                    int[] value = groupsNumsPerProp.Get(property);
                    if (value == null)
                    {
                        value = new int[1];
                        groupsNumsPerProp.Put(property, value);
                        value[0] = group.GroupNum;
                    }
                    else
                    {
                        int[] copy = new int[value.Length + 1];
                        Array.Copy(value, 0, copy, 0, value.Length);
                        copy[value.Length] = group.GroupNum;
                        Array.Sort(copy);
                        groupsNumsPerProp.Put(property, copy);
                    }                
                }
            }
            return groupsNumsPerProp;
        }
    
        /// <summary>Analyze multiple event types and determine common property sets that form property groups. </summary>
        /// <param name="allProperties">property names to look at</param>
        /// <param name="deltaEventTypes">all types contributing</param>
        /// <param name="aliases">names of properies</param>
        /// <returns>groups</returns>
        public static PropertyGroupDesc[] AnalyzeGroups(String[] allProperties, EventType[] deltaEventTypes, String[] aliases)
        {
            if (deltaEventTypes.Length != aliases.Length)
            {
                throw new ArgumentException("Delta event type number and alias number of elements don't match");
            }
            allProperties = CopyAndSort(allProperties);
    
            Map<MultiKey<String>, PropertyGroupDesc> result = new LinkedHashMap<MultiKey<String>, PropertyGroupDesc>();
            int currentGroupNum = 0;
    
            for (int i = 0; i < deltaEventTypes.Length; i++)
            {
                MultiKey<String> props = GetPropertiesContributed(deltaEventTypes[i], allProperties);
                if (props.Array.Length == 0)
                {
                    log.Warn("Event type alias '" + aliases[i] + "' does not contribute (or override) any properties of the revision event type");
                    continue;
                }
    
                PropertyGroupDesc propertyGroup = result.Get(props);
                Map<EventType, String> typesForGroup;
                if (propertyGroup == null)
                {
                    typesForGroup = new HashMap<EventType, String>();
                    propertyGroup = new PropertyGroupDesc(currentGroupNum++, typesForGroup, props.Array);
                    result.Put(props, propertyGroup);
                }
                else
                {
                    typesForGroup = propertyGroup.Types;
                }
                typesForGroup.Put(deltaEventTypes[i], aliases[i]);
            }

            PropertyGroupDesc[] array = CollectionHelper.ToArray(result.Values);
    
            if (log.IsDebugEnabled)
            {
                log.Debug(".analyzeGroups " + CollectionHelper.Render(array));            
            }
            return array;
        }
    
        private static MultiKey<String> GetPropertiesContributed(EventType deltaEventType, ICollection<string> allPropertiesSorted) {
    
            TreeSet<String> props = new TreeSet<String>();
            foreach (String property in deltaEventType.PropertyNames)
            {
                if ( allPropertiesSorted.Contains(property) ) {
                    props.Add(property);
                }
            }
            return new MultiKey<String>(props.ToArray());
        }
    
        /// <summary>Copy an sort the input array. </summary>
        /// <param name="input">to sort</param>
        /// <returns>sorted copied array</returns>
        protected internal static String[] CopyAndSort(ICollection<string> input)
        {
            String[] result = CollectionHelper.ToArray(input);
            Array.Sort(result);
            return result;
        }
    
        /// <summary>Return getters for property names. </summary>
        /// <param name="eventType">type to get getters from</param>
        /// <param name="propertyNames">names to get</param>
        /// <returns>getters</returns>
        protected internal static EventPropertyGetter[] GetGetters(EventType eventType, String[] propertyNames)
        {
            EventPropertyGetter[] getters = new EventPropertyGetter[propertyNames.Length];
            for (int i = 0; i < getters.Length; i++)
            {
                getters[i] = eventType.GetGetter(propertyNames[i]);
            }
            return getters;
        }
    
        /// <summary>Remove from values all removeValues and build a unique sorted result array. </summary>
        /// <param name="values">to consider</param>
        /// <param name="removeValues">values to remove from values</param>
        /// <returns>sorted unique</returns>
        public static String[] UniqueExclusiveSort(ICollection<string> values, String[] removeValues)
        {
            Set<String> unique = new HashSet<String>();
            foreach (String value in values)
            {
                unique.Add(value);
            }
            foreach (String removeValue in removeValues)
            {
                unique.Remove(removeValue);
            }
            String[] uniqueArr = unique.ToArray();
            Array.Sort(uniqueArr);
            return uniqueArr;
        }
    }
}
