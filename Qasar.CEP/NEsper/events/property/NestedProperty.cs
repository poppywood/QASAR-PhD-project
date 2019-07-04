using System;
using System.Collections.Generic;
using System.IO;
using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
	/// <summary> This class represents a nested property, each nesting level made up of a property instance that
	/// can be of type indexed, mapped or simple itself.
	/// <para>
	/// The syntax for nested properties is as follows.
	/// <c>
	/// a.n
	/// a[1].n
	/// a('1').n
	/// </c>
    /// </para>
	/// </summary>

    public class NestedProperty : Property
    {
        private readonly IList<Property> properties;
        private readonly BeanEventTypeFactory beanEventTypeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NestedProperty"/> class.
        /// </summary>
        /// <param name="properties">the list of Property instances representing each nesting level</param>
        /// <param name="beanEventTypeFactory">the cache and factory for event bean types and event wrappers</param>
        public NestedProperty(IList<Property> properties, BeanEventTypeFactory beanEventTypeFactory)
        {
            this.properties = properties;
            this.beanEventTypeFactory = beanEventTypeFactory;
        }

        /// <summary> Returns the list of property instances making up the nesting levels.</summary>
        /// <returns> list of Property instances
        /// </returns>

        public IList<Property> Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Returns value getter for the property of an event of the given event type.
        /// </summary>
        /// <param name="eventType">is the type of event to make a getter for</param>
        /// <returns>fast property value getter for property</returns>
        public virtual EventPropertyGetter GetGetter(BeanEventType eventType)
        {
            List<EventPropertyGetter> getters = new List<EventPropertyGetter>();

            IEnumerator<Property> propertyEnum = properties.GetEnumerator();
            if (propertyEnum.MoveNext())
            {
                bool hasNext;

                do
                {
                    Property property = propertyEnum.Current;
                    EventPropertyGetter getter = property.GetGetter(eventType);
                    if (getter == null)
                    {
                        return null;
                    }

                    hasNext = propertyEnum.MoveNext();
                    if (hasNext)
                    {
                        Type type = property.GetPropertyType(eventType);
                        if (type == null)
                        {
                            // if the property is not valid, return null
                            return null;
                        }

                        if ((typeof(System.Collections.IDictionary).IsAssignableFrom(type)) ||
                            (typeof(IDictionary<string, Object>).IsAssignableFrom(type)) ||
                            (typeof(IDictionary<string, EventBean>).IsAssignableFrom(type)))
                        {
                            return null;
                        }

                        if (type.IsArray)
                        {
                            return null;
                        }

                        //eventType = (BeanEventType)beanEventTypeFactory.CreateBeanType(type.FullName, type);
                        eventType = beanEventTypeFactory.CreateBeanType(type.FullName, type);

                    }

                    getters.Add(getter);
                } while (hasNext);
            }

            return new NestedPropertyGetter(getters, beanEventTypeFactory);
        }

        /// <summary>
        /// Returns the property type.
        /// </summary>
        /// <param name="eventType">is the event type representing the bean</param>
        /// <returns>property type class</returns>
        public virtual Type GetPropertyType(BeanEventType eventType)
        {
            Type result = null;

            IEnumerator<Property> propertyEnum = properties.GetEnumerator();
            if (propertyEnum.MoveNext())
            {
                bool hasNext;

                do
                {
                    Property property = propertyEnum.Current;
                    result = property.GetPropertyType(eventType);

                    if (result == null)
                    {
                        // property not found, return null
                        return null;
                    }

                    hasNext = propertyEnum.MoveNext();
                    if (hasNext)
                    {
                        // Map cannot be used to further nest as the type cannot be determined
                        if ((typeof(System.Collections.IDictionary).IsAssignableFrom(result)) ||
                            (typeof(IDictionary<string, Object>).IsAssignableFrom(result)) ||
                            (typeof(IDictionary<string, EventBean>).IsAssignableFrom(result)))
                        {
                            return null;
                        }

                        if (result.IsArray)
                        {
                            return null;
                        }

                        eventType = beanEventTypeFactory.CreateBeanType(result.FullName, result);
                    }
                } while (hasNext);
            }

            return result;
        }

        /// <summary>
        /// Returns the property type for use with Map event representations.
        /// </summary>
        public Type GetPropertyTypeMap(Map<String, Object> optionalMapPropTypes)
        {
            Map<String, Object> currentDictionary = optionalMapPropTypes;

            int count = 0;
            int last = properties.Count - 1;

            for (int ii = 0; ii < properties.Count; ii++) {
                count++;
                Property property = properties[ii];
                PropertyBase @base = (PropertyBase) property;

                String propertyName = @base.PropertyNameAtomic;

                Object nestedType = null;
                if (currentDictionary != null) {
                    nestedType = currentDictionary.Get(propertyName);
                }

                if (nestedType == null) {
                    if (property is DynamicProperty) {
                        return typeof (Object);
                    } else {
                        return null;
                    }
                }

                if (ii == last) {
                    if (nestedType is Type) {
                        return (Type) nestedType;
                    }
                    if (nestedType is Map<String, Object>) {
                        return typeof (Map<String, Object>);
                    }
                }

                if (nestedType == typeof (Map<String, Object>)) {
                    return typeof (Object);
                }

                if (nestedType is Type) {
                    Type objectType = (Type) nestedType;
                    BeanEventType beanType = beanEventTypeFactory.CreateBeanType(objectType.FullName, objectType);
                    String remainingProps = ToPropertyEPL(properties, count);
                    return beanType.GetPropertyType(remainingProps);
                }

                if (!(nestedType is Map<String, Object>)) {
                    String message = "Nestable map type configuration encountered an unexpected value type of '"
                                     + nestedType.GetType() + " for property '" + propertyName +
                                     "', expected Type, typeof(Map<String,Object>) or Map<String, Object> as value type";
                    throw new PropertyAccessException(message);
                }

                currentDictionary = (Map<String, Object>) nestedType;
            }
            throw new IllegalStateException("Unexpected end of nested property");
        }

        public EventPropertyGetter GetGetterMap(Map<String, Object> optionalMapPropTypes)
        {
            List<EventPropertyGetter> getters = new List<EventPropertyGetter>();
            Map<String, Object> currentDictionary = optionalMapPropTypes;

            int count = 0;
            int last = properties.Count - 1;

            for (int ii = 0; ii < properties.Count; ii++) {
                count++;
                Property property = properties[ii];

                // manufacture a getter for getting the item out of the map
                EventPropertyGetter getter = property.GetGetterMap(currentDictionary);
                if (getter == null) {
                    return null;
                }
                getters.Add(getter);

                PropertyBase @base = (PropertyBase) property;
                String propertyName = @base.PropertyNameAtomic;

                // For the next property if there is one, check how to property type is defined
                if (ii == last) {
                    continue;
                }

                if (currentDictionary != null) {
                    // check the type that this property will return
                    Object propertyReturnType = currentDictionary.Get(propertyName);

                    if (propertyReturnType == null) {
                        currentDictionary = null;
                    }
                    if (propertyReturnType != null) {
                        if (propertyReturnType is Map<String, Object>) {
                            currentDictionary = (Map<String, Object>) propertyReturnType;
                        } else if (propertyReturnType == typeof (Map<String, Object>)) {
                            currentDictionary = null;
                        } else {
                            // treat the return type of the map property as an object
                            Type objectType = (Type) propertyReturnType;
                            BeanEventType beanType =
                                beanEventTypeFactory.CreateBeanType(objectType.FullName, objectType);
                            String remainingProps = ToPropertyEPL(properties, count);
                            getters.Add(beanType.GetGetter(remainingProps));
                            break; // the single Pojo getter handles the rest
                        }
                    }
                }
            }

            return new MapNestedPropertyGetter(getters, beanEventTypeFactory);
        }

	    public void ToPropertyEPL(StringWriter writer)
        {
            String delimiter = "";
            foreach (Property property in properties)
            {
                writer.Write(delimiter);
                property.ToPropertyEPL(writer);
                delimiter = ".";
            }
        }

        private static String ToPropertyEPL(IList<Property> property, int startFromIndex)
        {
            String delimiter = "";
            StringWriter writer = new StringWriter();
            for (int i = startFromIndex; i < property.Count; i++)
            {
                writer.Write(delimiter);
                property[i].ToPropertyEPL(writer);
                delimiter = ".";
            }
            return writer.ToString();
        }
    }
}
