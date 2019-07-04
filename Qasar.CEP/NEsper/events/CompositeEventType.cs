using System;
using System.Collections.Generic;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.parse;

namespace com.espertech.esper.events
{
	/// <summary> Event type for events that itself have event properties that are event wrappers.
	/// <para>
	/// For use in pattern expression statements in which multiple events match a pattern. There the
	/// composite event indicates that the whole patterns matched, and indicates the
	/// individual events that caused the pattern as event properties to the event.
    /// </para>
	/// </summary>

    public class CompositeEventType : EventType, TaggedCompositeEventType
	{
        private readonly Map<String, Pair<EventType, String>> taggedEventTypes;

        private readonly string alias;

        /// <summary>
        /// Gets the event type alias.
        /// </summary>
        /// <value>The alias.</value>
        public string Alias
        {
            get { return alias; }
        }

        /// <summary>
        /// Get the class that represents the type of the event type.
        /// Returns a bean event class if the schema represents a bean event type.
        /// Returns Map if the schema represents a collection of values in a Map.
        /// </summary>
        /// <value>The type of the underlying.</value>
        /// <returns> type of the event object
        /// </returns>
		public Type UnderlyingType
		{
			get { return typeof(System.Collections.IDictionary); }
		}

        /// <summary>
        /// Get all valid property names for the event type.
        /// </summary>
        /// <value>The property names.</value>
        /// <returns> A string array containing the property names of this typed event data object.
        /// </returns>
		public ICollection<String> PropertyNames
		{
			get { return taggedEventTypes.Keys; }
		}

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="alias">the event type alias.</param>
        /// <param name="taggedEventTypes">is a map of name tags and event type per tag</param>

        public CompositeEventType(String alias, Map<String, Pair<EventType, String>> taggedEventTypes)
        {
            this.taggedEventTypes = taggedEventTypes;
            this.alias = alias;
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
		public virtual Type GetPropertyType(String propertyName)
		{
            Pair<EventType, String> result = taggedEventTypes.Get(propertyName);
            if (result != null)
            {
                return result.First.UnderlyingType;
            }

            // see if this is a nested property
            int index = ASTFilterSpecHelper.UnescapedIndexOfDot(propertyName);
            if (index == -1)
            {
                return null;
            }
			
			// Take apart the nested property into a map key and a nested value class property name
			String propertyMap = propertyName.Substring(0, index);
            String propertyNested = propertyName.Substring(index + 1, propertyName.Length - index - 1);
            result = taggedEventTypes.Get(propertyMap);
            if (result == null)
			{
				return null;
			}
			
			// ask the nested class to resolve the property
            return result.First.GetPropertyType(propertyNested);
		}

        /// <summary>
        /// Gets the getter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
		public virtual EventPropertyGetter GetGetter(String propertyName)
		{
			// see if this is a nested property
            int index = ASTFilterSpecHelper.UnescapedIndexOfDot(propertyName);
            if (index == -1)
            {
                Pair<EventType, String> result = taggedEventTypes.Get(propertyName);
                if (result == null)
				{
					return null;
				}
				
				// Not a nested property, return tag's underlying value
				String tag = propertyName;
				return new TagEventPropertyGetter( tag ) ;
			}
			
			// Take apart the nested property into a map key and a nested value class property name
			String propertyMap = propertyName.Substring(0, index);
			String propertyNested = propertyName.Substring(index + 1, propertyName.Length - index - 1);

            Pair<EventType, String> result2 = taggedEventTypes.Get(propertyMap);
            if (result2 == null)
			{
				return null;
			}
			
            EventPropertyGetter getterNested = result2.First.GetGetter(propertyNested);
			if (getterNested == null)
			{
				return null;
			}
			
			return new NestedEventPropertyGetter( propertyMap, getterNested ) ;
		}

        /// <summary>
        /// Determines whether the specified property name is property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property name is property; otherwise, <c>false</c>.
        /// </returns>
		public virtual bool IsProperty(String propertyName)
		{
			Type propertyType = GetPropertyType(propertyName);
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
		public IEnumerable<EventType> DeepSuperTypes
		{
            get { return EventTypeArray.Empty ; }
		}

        public override bool Equals(Object obj)
        {
            if (!(obj is CompositeEventType))
            {
                return false;
            }

            CompositeEventType other = (CompositeEventType) obj;
            // Composite event types are always anonymous therefore not checking alias name

            if (!(other.taggedEventTypes.Count == taggedEventTypes.Count))
            {
                return false;
            }

            foreach (KeyValuePair<String, Pair<EventType, String>> entry in taggedEventTypes)
            {
                EventType composed = entry.Value.First;
                Pair<EventType, String> otherComposed = other.taggedEventTypes.Get(entry.Key);

                EventType otherComposedType = otherComposed.First;
                if (!Equals(composed, otherComposedType))
                {
                    return false;
                }
            }

            return true;
        }

	    public override int GetHashCode()
        {
            return alias.GetHashCode();
        }

        public Map<String, Pair<EventType, String>> TaggedEventTypes
        {
            get { return taggedEventTypes; }
        }
		
		/// <summary>
		/// An EventPropertyGetter that is based upon a named tag.
		/// </summary>
		
		internal class TagEventPropertyGetter : EventPropertyGetter
		{
            private readonly string tag;

            /// <summary>
            /// Initializes a new instance of the <see cref="TagEventPropertyGetter"/> class.
            /// </summary>
            /// <param name="tag">The tag.</param>
			public TagEventPropertyGetter(string tag)
			{
				this.tag = tag;
			}

            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <param name="obj">The obj.</param>
            /// <returns></returns>
			public Object GetValue( EventBean obj )
			{
                EventBean wrapper;

                // Underlying just be a type of dictionary.  We don't care if its generic
                // or the old style dictionary, but it must cast to one.

                Object underlying = obj.Underlying;
                if ( underlying is IDictionary<string,EventBean> )
                {
                    IDictionary<string, EventBean> tempDict = (IDictionary<string, EventBean>) underlying;
                    tempDict.TryGetValue(tag, out wrapper);
                }
                else if (underlying is System.Collections.IDictionary)
                {
                    wrapper = ((System.Collections.IDictionary)underlying)[tag] as EventBean;
                }
                else
                {
					throw new PropertyAccessException(
						"Mismatched property getter to event bean type, " +
						"the underlying data object is not of type IDictionary");
				}

				// If the map does not contain the key, this is allowed and represented as null
				if (wrapper != null)
				{
					return wrapper.Underlying;
				}
	
				return null;
			}

            public bool IsExistsProperty(EventBean eventBean)
            {
                return true; // Property exists as the property is not dynamic (unchecked)
            }
		}

		/// <summary>
		/// An EventPropertyGetter that is based upon a named tag
		/// and a nester EventPropertyGetter.
		/// </summary>
		
		internal class NestedEventPropertyGetter : EventPropertyGetter
		{
			private readonly string tag ;
            private readonly EventPropertyGetter nestedGetter;

            /// <summary>
            /// Initializes a new instance of the <see cref="NestedEventPropertyGetter"/> class.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="nestedGetter">The nested getter.</param>
			public NestedEventPropertyGetter(string tag, EventPropertyGetter nestedGetter)
			{
				this.tag = tag;
				this.nestedGetter = nestedGetter;
			}

            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <param name="obj">The obj.</param>
            /// <returns></returns>
			public Object GetValue( EventBean obj )
			{
                EventBean wrapper;

                // Underlying just be a type of dictionary.  We don't care if its generic
                // or the old style dictionary, but it must cast to one.

                Object underlying = obj.Underlying;
                if (underlying is IDictionary<string, EventBean>)
                {
                    IDictionary<string, EventBean> tempDict = (IDictionary<string, EventBean>)underlying;
                    tempDict.TryGetValue(tag, out wrapper);
                }
                else if (underlying is System.Collections.IDictionary)
                {
                    wrapper = ((System.Collections.IDictionary)underlying)[tag] as EventBean;
                }
                else
                {
                    throw new PropertyAccessException(
                        "Mismatched property getter to event bean type, " +
                        "the underlying data object is not of type IDictionary");
                }

				// If the map does not contain the key, this is allowed and represented as null
				if (wrapper !=  null)
				{
					return nestedGetter.GetValue(wrapper);
				}
	
				return null;
			}

            public bool IsExistsProperty(EventBean eventBean)
            {
                return true; // Property exists as the property is not dynamic (unchecked)
            }
		}
	}
}