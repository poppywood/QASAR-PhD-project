using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.epl.parse;
using com.espertech.esper.events.property;
using com.espertech.esper.util;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Implementation of the <see cref="EventType" /> interface for handling plain Maps containing name value pairs.
    /// </summary>

    public class MapEventType : EventType
    {
        /// <summary>
        /// Gets the event type alias.
        /// </summary>

        virtual public String Alias
        {
            get { return typeName; }
        }

        /// <summary>Returns the name-type map of map properties, each value in the mapcan be a Class or a Map&lt;String, Object&gt; (for nested maps).</summary>
        /// <returns>is the property name and types</returns>

        public virtual Map<string, object> Types
        {
            get { return this.nestableTypes; }
        }

        /// <summary>
        /// Get the class that represents the type of the event type.
        /// Returns a bean event class if the schema represents a bean event type.
        /// Returns Map if the schema represents a collection of values in a Map.
        /// </summary>
        /// <value>The type of the underlying.</value>
        /// <returns> type of the event object
        /// </returns>
        virtual public Type UnderlyingType
        {
            get { return typeof(DataMap); }
        }

        /// <summary>
        /// Get all valid property names for the event type.
        /// </summary>
        /// <value>The property names.</value>
        /// <returns> A string array containing the property names of this typed event data object.
        /// </returns>
        virtual public ICollection<String> PropertyNames
        {
            get { return propertyNames; }
        }

        private readonly String typeName;
        private readonly String[] propertyNames; // Cache an array of property names so not to construct one frequently
        private readonly int hashCode;
        private readonly EventAdapterService eventAdapterService;

        // Simple (not-nested) properties are stored here
        private readonly Map<String, Type> simplePropertyTypes;     // Mapping of property name (simple-only) and type
        private readonly Map<String, EventPropertyGetter> propertyGetters;   // Mapping of property name and getters
        // Nestable definition of Map contents is here
        private readonly Map<String, Object> nestableTypes;  // Deep definition of the map-type, containing nested maps and objects

        /// <summary>Constructor takes a type name, map of property names and types.</summary>
        /// <param name="typeName">
        /// is the event type name used to distinquish map types that have the same property types,
        /// empty string for anonymous maps, or for insert-into statements generating map events
        /// the stream name
        /// </param>
        /// <param name="propertyTypes">is pairs of property name and type</param>
        /// <param name="eventAdapterService">
        /// is required for access to objects properties within map values
        /// </param>

        public MapEventType(String typeName,
                            Map<String, Type> propertyTypes,
                            EventAdapterService eventAdapterService)
        {
            this.typeName = typeName;
            this.eventAdapterService = eventAdapterService;

            // copy the property names and types (simple-properties only)
            this.nestableTypes = new HashMap<String, Object>();
            this.simplePropertyTypes = new HashMap<String, Type>();
            foreach (KeyValuePair<String, Type> entry in propertyTypes)
            {
                this.nestableTypes[entry.Key] = entry.Value;
                this.simplePropertyTypes[entry.Key] = TypeHelper.GetBoxedType(entry.Value);
            }

            hashCode = typeName.GetHashCode();
            propertyNames = new String[simplePropertyTypes.Count];
            propertyGetters = new HashMap<String, EventPropertyGetter>();

            // Initialize getters and names array
            int index = 0;
            foreach (KeyValuePair<String, Type> entry in simplePropertyTypes)
            {
                String name = entry.Key;
                hashCode = hashCode ^ name.GetHashCode();

                EventPropertyGetter getter = new MapEventPropertyGetter(name);
                propertyGetters.Put(name, getter);
                propertyNames[index++] = name;
            }
        }

        /// <summary>Constructor takes a type name, map of property names and types, foruse with nestable Map events.</summary>
        /// <param name="typeName">is the event type name used to distinquish map types that have the same property types,empty string for anonymous maps, or for insert-into statements generating map eventsthe stream name</param>
        /// <param name="propertyTypes">is pairs of property name and type</param>
        /// <param name="eventAdapterService">is required for access to objects properties within map values</param>
        public MapEventType(String typeName,
                            EventAdapterService eventAdapterService,
                            IDictionary<String, Object> propertyTypes)
        {
            this.typeName = typeName;
            this.eventAdapterService = eventAdapterService;

            this.simplePropertyTypes = new HashMap<String, Type>();
            List<String> propertyNameList = new List<String>();
            this.nestableTypes = new HashMap<String, Object>();
            this.nestableTypes.PutAll(propertyTypes);

            hashCode = typeName.GetHashCode();
            propertyGetters = new HashMap<String, EventPropertyGetter>();

            // Initialize getters and names array: at this time we do not care about nested types,
            // these are handled at the time someone is asking for them
            foreach (KeyValuePair<String, Object> entry in propertyTypes)
            {
                String name = entry.Key;
                hashCode = hashCode ^ name.GetHashCode();

                EventPropertyGetter getter;
                if (entry.Value is Type)
                {
                    simplePropertyTypes[name] = TypeHelper.GetBoxedType((Type) entry.Value);
                    propertyNameList.Add(name);
                    getter = new MapEventPropertyGetter(name);
                    propertyGetters[name] = getter;
                    continue;
                }
                
                // A null-type is also allowed
                if (entry.Value == null)
                {
                    simplePropertyTypes[name] = null;
                    propertyNameList.Add(name);
                    getter = new MapEventPropertyGetter(name);
                    propertyGetters[name] = getter;
                    continue;
                }

                if (entry.Value is DataMap) {
                    // Add Map itself as a property
                    simplePropertyTypes[name] = typeof (DataMap);
                    propertyNameList.Add(name);
                    propertyGetters[name] = new MapEventPropertyGetter(name);
                    continue;
                }

                if (!(entry.Value is EventType))
                {
                    GenerateExceptionNestedProp(name, entry.Value);
                }

                // Add EventType itself as a property
                EventType eventType = (EventType)entry.Value;
                simplePropertyTypes[name] = eventType.UnderlyingType;
                propertyNameList.Add(name);
                propertyGetters[name] = new MapEventBeanPropertyGetter(name);
            }

            propertyNames = propertyNameList.ToArray();
        }

        /// <summary>
        /// An EventProperty designed to extract the named property from a DataMap.
        /// This method was originally implemented as an anonymous innerclass in Java.
        /// </summary>

        internal class SimpleEventPropertyGetter : EventPropertyGetter
        {
            private readonly string name;

            internal SimpleEventPropertyGetter(string name)
            {
                this.name = name;
            }

            /// <summary>
            /// Return the value for the property in the event object specified when the instance was obtained.
            /// Useful for fast access to event properties. Throws a PropertyAccessException if the getter instance
            /// doesn't match the EventType it was obtained from, and to indicate other property access problems.
            /// </summary>
            /// <param name="eventBean">is the event to get the value of a property from</param>
            /// <returns>value of property in event</returns>
            /// <throws>  PropertyAccessException to indicate that property access failed </throws>
            public object GetValue(EventBean eventBean)
            {
                Object underlying = eventBean.Underlying;
                if (underlying is DataMap)
                {
                    Object value;
                    ((DataMap)underlying).TryGetValue(name, out value);
                    return value;
                }
                else if (underlying is System.Collections.IDictionary)
                {
                    Object value = ((System.Collections.IDictionary)underlying)[name];
                    return value;
                }
                else
                {
                    throw new PropertyAccessException(
                        "Mismatched property getter to event bean type, " +
                        "the underlying data object is not of type IDictionary");
                }
            }

            /// <summary>
            /// Returns true if the property exists, or false if the type does not have such a property.
            /// <para>
            /// Useful for dynamic properties of the syntax "property?" and the dynamic nested/indexed/mapped versions.
            /// Dynamic nested properties follow the syntax "property?.nested" which is equivalent to "property?.nested?".
            /// If any of the properties in the path of a dynamic nested property return null, the dynamic nested property
            /// does not exists and the method returns false.
            /// </para>
            /// 	<para>
            /// For non-dynamic properties, this method always returns true since a getter would not be available
            /// unless
            /// </para>
            /// </summary>
            /// <param name="eventBean">the event to check if the dynamic property exists</param>
            /// <returns>
            /// indictor whether the property exists, always true for non-dynamic (default) properties
            /// </returns>
            public bool IsExistsProperty(EventBean eventBean)
            {
                return true; // Property exists as the property is not dynamic (unchecked)
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public Object GetValue(String propertyName, Map<String, Object> values)
        {
            if (simplePropertyTypes.Get(ASTFilterSpecHelper.UnescapeDot(propertyName)) != null)
            {
                return values.Get(ASTFilterSpecHelper.UnescapeDot(propertyName));
            }

            // see if this is a nested property
            int index = ASTFilterSpecHelper.UnescapedIndexOfDot(propertyName);
            if (index == -1)
            {
                return null;
            }

            // Take apart the nested property into a map key and a nested value class property name
            String propertyMap = ASTFilterSpecHelper.UnescapeDot(propertyName.Substring(0, index));
            String propertyNested = propertyName.Substring(index + 1);

            Type result = simplePropertyTypes.Get(propertyMap);
            if (result == null)
            {
                return null;
            }

            // ask the nested class to resolve the property
            EventType nestedType = eventAdapterService.AddBeanType(result.Name, result);
            EventPropertyGetter nestedGetter = nestedType.GetGetter(propertyNested);
            if (nestedGetter == null)
            {
                return null;
            }

            // Wrap object
            Object value;
            values.TryGetValue(propertyMap, out value);
            if (value == null)
            {
                return null;
            }
            EventBean _event = eventAdapterService.AdapterForBean(value);
            return nestedGetter.GetValue(_event);
        }

        /// <summary>
        /// Gets the type of property associated with the property name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>

        public Type GetPropertyType(String propertyName)
        {
            Type result = simplePropertyTypes.Get(ASTFilterSpecHelper.UnescapeDot(propertyName));
            if (result != null)
            {
                return result;
            }

            // see if this is a nested property
            int index = ASTFilterSpecHelper.UnescapedIndexOfDot(propertyName); 
            if (index == -1)
            {
                // dynamic simple property
                if (propertyName.EndsWith("?"))
                {
                    return typeof(Object);
                }
                return null;
            }

            // Map event types allow 2 types of properties inside:
            //   - a property that is an object is interrogated via bean property getters and BeanEventType
            //   - a property that is a Map itself is interrogated via map property getters
            // The property getters therefore act on

            // Take apart the nested property into a map key and a nested value class property name
            String propertyMap = ASTFilterSpecHelper.UnescapeDot(propertyName.Substring(0, index));
            String propertyNested = propertyName.Substring(index + 1, propertyName.Length - index - 1);
            bool isRootedDynamic = false;

            // If the property is dynamic, remove the ? since the property type is defined without
            if (propertyMap.EndsWith("?"))
            {
                propertyMap = propertyMap.Substring(0, propertyMap.Length - 1);
                isRootedDynamic = true;
            }

            Object nestedType = nestableTypes.Get(propertyMap);
            if (nestedType == null)
            {
                return null;
            }

            // If there is a map value in the map, return the Object value if this is a dynamic property
            if (nestedType == typeof(DataMap))
            {
                Property prop = PropertyParser.Parse(propertyNested, eventAdapterService.BeanEventTypeFactory, isRootedDynamic);
                return prop.GetPropertyTypeMap(null);   // we don't have a definition of the nested props
            }
            else if (nestedType is DataMap)
            {
                Property prop = PropertyParser.Parse(propertyNested, eventAdapterService.BeanEventTypeFactory, isRootedDynamic);
                DataMap nestedTypes = (DataMap)nestedType;
                return prop.GetPropertyTypeMap(nestedTypes);
            }
            else if (nestedType is Type)
            {
                Type simpleClass = (Type)nestedType;
                EventType nestedEventType = eventAdapterService.AddBeanType(simpleClass.Name, simpleClass);
                return nestedEventType.GetPropertyType(propertyNested);
            }
            else if (nestedType is EventType)
            {
                EventType innerType = (EventType) nestedType;
                return innerType.GetPropertyType(propertyNested);
            }
            else
            {
                String message =
                    "Nestable map type configuration encountered an unexpected value type of '" + nestedType.GetType() +
                    " for property '" + propertyName + "', expected Class, Map.class or Map<String, Object> as value type";
                throw new PropertyAccessException(message);
            }
        }

        /// <summary>
        /// Gets the getter for the property name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>

        public virtual EventPropertyGetter GetGetter(String propertyName)
        {
            String unescapePropName = ASTFilterSpecHelper.UnescapeDot(propertyName);
            EventPropertyGetter getter = propertyGetters.Get(unescapePropName);
            if (getter != null)
            {
                return getter;
            }

            // see if this is a nested property
            int index = ASTFilterSpecHelper.UnescapedIndexOfDot(propertyName);
            if (index == -1)
            {
                // dynamic property for maps is allowed
                Property prop = PropertyParser.Parse(propertyName, eventAdapterService.BeanEventTypeFactory, false);
                if (prop is DynamicProperty)
                {
                    return prop.GetGetterMap(null);
                }
                return null;
            }

            // Take apart the nested property into a map key and a nested value class property name
            String propertyMap = ASTFilterSpecHelper.UnescapeDot(propertyName.Substring(0, index));
            String propertyNested = propertyName.Substring(index + 1);
            bool isRootedDynamic = false;

            // If the property is dynamic, remove the ? since the property type is defined without
            if (propertyMap.EndsWith("?"))
            {
                propertyMap = propertyMap.Substring(0, propertyMap.Length - 1);
                isRootedDynamic = true;
            }

            Object nestedType = nestableTypes.Get(propertyMap);
            if (nestedType == null)
            {
                return null;
            }

            // The map contains another map, we resolve the property dynamically
            if (nestedType == typeof(DataMap))
            {
                Property prop = PropertyParser.Parse(propertyNested, eventAdapterService.BeanEventTypeFactory, isRootedDynamic);
                EventPropertyGetter getterNestedMap = prop.GetGetterMap(null);
                if (getterNestedMap == null)
                {
                    return null;
                }
                return new MapPropertyGetter(propertyMap, getterNestedMap);
            }
            else if (nestedType is DataMap)
            {
                Property prop = PropertyParser.Parse(propertyNested, eventAdapterService.BeanEventTypeFactory, isRootedDynamic);
                DataMap nestedTypes = (DataMap)nestedType;
                EventPropertyGetter getterNestedMap = prop.GetGetterMap(nestedTypes);
                if (getterNestedMap == null)
                {
                    return null;
                }
                return new MapPropertyGetter(propertyMap, getterNestedMap);
            }
            else if (nestedType is Type)
            {
                // ask the nested class to resolve the property
                Type simpleClass = (Type)nestedType;
                EventType nestedEventType = eventAdapterService.AddBeanType(simpleClass.Name, simpleClass);
                EventPropertyGetter nestedGetter = nestedEventType.GetGetter(propertyNested);
                if (nestedGetter == null)
                {
                    return null;
                }

                // construct getter for nested property
                getter = new MapObjectEntryPropertyGetter(propertyMap, nestedGetter, eventAdapterService);
                return getter;
            }
            else if (nestedType is EventType)
            {
                // ask the nested class to resolve the property
                EventType innerType = (EventType) nestedType;
                EventPropertyGetter nestedGetter = innerType.GetGetter(propertyNested);
                if (nestedGetter == null)
                {
                    return null;
                }

                // construct getter for nested property
                getter = new MapEventBeanEntryPropertyGetter(propertyMap, nestedGetter);

                return getter;
            }
            else
            {
                String message =
                    "Nestable map type configuration encountered an unexpected value type of '" + nestedType.GetType() +
                    " for property '" + propertyName + "', expected Type, typeof(IDataMap) or IDictionary<String, Object> as value type";
                throw new PropertyAccessException(message);
            }
        }

        /// <summary>
        /// Returns true if the specified property name maps to a property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>

        public virtual bool IsProperty(String propertyName)
        {
            Type propertyType = GetPropertyType(propertyName);
            if (propertyType == null)
            {
                // Could be a native null type, such as "insert into A select null as field..."
                if (simplePropertyTypes.ContainsKey(ASTFilterSpecHelper.UnescapeDot(propertyName)))
                {
                    return true;
                }
            }
            return propertyType != null;
        }

        /// <summary>
        /// Returns an array of event types that are super to this event type, from which this event type
        /// inherited event properties.  For object instances underlying the event this method returns the
        /// event types for all superclasses extended by the object and all interfaces implemented by the
        /// object.
        /// </summary>
        /// <value></value>
        /// <returns>an array of event types</returns>
        public virtual IEnumerable<EventType> SuperTypes
        {
            get { return null; }
        }

        /// <summary>
        /// Returns enumerable over all super types to event type, going up the hierarchy and including
        /// all interfaces (and their extended interfaces) and superclasses as EventType instances.
        /// </summary>
        /// <value></value>
        public virtual IEnumerable<EventType> DeepSuperTypes
        {
            get { return EventTypeArray.Empty; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return "MapEventType " +
                "typeName=" + typeName +
                "propertyNames=" + CollectionHelper.Render(propertyNames);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }

            MapEventType other = obj as MapEventType;
            if (other == null)
            {
                return false;
            }

            // Should have the same type name
            if (other.typeName != this.typeName)
            {
                return false;
            }

            return IsDeepEqualsProperties(other.nestableTypes, this.nestableTypes);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return hashCode;
        }

        /// <summary>Compares two sets of properties and determines if they are the same, allowing forboxed/unboxed types.</summary>
        /// <param name="setOne">is the first set of properties</param>
        /// <param name="setTwo">is the second set of properties</param>
        /// <returns>true if the property set is equivalent, false if not</returns>
        public static bool IsEqualsProperties(Map<String, Type> setOne, Map<String, Type> setTwo)
        {
            // Should have the same number of properties
            if (setOne.Count != setTwo.Count)
            {
                return false;
            }

            // Compare property by property
            foreach (KeyValuePair<String, Type> entry in setOne)
            {
                Type otherClass = setTwo.Get(entry.Key);
                Type thisClass = entry.Value;
                if (((otherClass == null) && (thisClass != null)) ||
                     (otherClass != null) && (thisClass == null))
                {
                    return false;
                }
                if (otherClass == null)
                {
                    continue;
                }
                Type boxedOther = TypeHelper.GetBoxedType(otherClass);
                Type boxedThis = TypeHelper.GetBoxedType(thisClass);
                if (!Equals(boxedOther, boxedThis))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Compares two sets of properties and determines if they are the same, allowing forboxed/unboxed types, and nested map types.</summary>
        /// <param name="setOne">is the first set of properties</param>
        /// <param name="setTwo">is the second set of properties</param>
        /// <returns>true if the property set is equivalent, false if not</returns>
        public static bool IsDeepEqualsProperties(Map<String, Object> setOne, Map<String, Object> setTwo)
        {
            // Should have the same number of properties
            if (setOne.Count !=
                setTwo.Count)
            {
                return false;
            }

            // Compare property by property
            foreach (KeyValuePair<String, Object> entry in setOne)
            {
                Object setTwoType = setTwo.Get(entry.Key);
                Object setOneType = entry.Value;
                if (((setTwoType == null) && (setOneType != null)) ||
                    (setTwoType != null) && (setOneType == null))
                {
                    return false;
                }
                if (setTwoType == null)
                {
                    continue;
                }

                if ((setTwoType is Type) &&
                    (setOneType is Type))
                {
                    Type boxedOther = TypeHelper.GetBoxedType((Type)setTwoType);
                    Type boxedThis = TypeHelper.GetBoxedType((Type)setOneType);
                    if (!boxedOther.Equals(boxedThis))
                    {
                        return false;
                    }
                }
                else if ((setTwoType is DataMap) &&
                         (setOneType is DataMap))
                {
                    bool isDeepEqual = IsDeepEqualsProperties((DataMap) setOneType, (DataMap) setTwoType);
                    if (!isDeepEqual)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static void GenerateExceptionNestedProp(String name, Object value)
        {
            String clazzName = (value == null) ? "null" : value.GetType().Name;
            throw new EPException(
                "Nestable map type configuration encountered an unexpected property type of '" + clazzName + 
                "' for property '" + name + "', expected System.Type or DataMap definition");
        }
    }
}
