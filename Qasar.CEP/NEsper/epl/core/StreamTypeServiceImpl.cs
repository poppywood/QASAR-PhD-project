using System;
using System.Collections.Generic;
using com.espertech.esper.collection;
using com.espertech.esper.core;
using com.espertech.esper.epl.parse;
using com.espertech.esper.events;

namespace com.espertech.esper.epl.core
{
	/// <summary>
    /// Implementation that provides stream number and property type information.
    /// </summary>
	public class StreamTypeServiceImpl : StreamTypeService
	{
        /// <summary>
        /// Returns an array of event types for each event stream in the order declared.
        /// </summary>
        /// <value></value>
        /// <returns> event types
        /// </returns>
		virtual public EventType[] EventTypes
		{
			get { return eventTypes; }
		}

        /// <summary>
        /// Returns an array of event stream names in the order declared.
        /// </summary>
        /// <value></value>
        /// <returns> stream names
        /// </returns>
		virtual public String[] StreamNames
		{
			get { return streamNames; }
		}

	    private readonly EventType[] eventTypes;
	    private readonly String[] streamNames;
        private readonly String engineURIQualifier;
        private readonly String[] eventTypeAlias;
	    private readonly bool isStreamZeroUnambigous;
	    private readonly bool requireStreamNames;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="eventTypes">array of event types, one for each stream</param>
        /// <param name="streamNames">array of stream names, one for each stream</param>
        /// <param name="engineURI">engine URI</param>
        /// <param name="eventTypeAlias">alias name of the event type</param>
        public StreamTypeServiceImpl(EventType[] eventTypes, String[] streamNames, String engineURI, String[] eventTypeAlias)
        {
            this.eventTypes = eventTypes;
            this.streamNames = streamNames;
            this.eventTypeAlias = eventTypeAlias;

            if (engineURI == null)
            {
                engineURIQualifier = EPServiceProviderConstants.DEFAULT_ENGINE_URI__QUALIFIER;
            }
            else
            {
                engineURIQualifier = engineURI;
            }

            if (eventTypes.Length != streamNames.Length)
            {
                throw new ArgumentException("Number of entries for event types and stream names differs");
            }
        }

        /// <summary>Ctor.</summary>
        /// <param name="namesAndTypes">is the ordered list of stream names and event types available (stream zero to N)</param>
        /// <param name="isStreamZeroUnambigous">indicates whether when a property is found in stream zero and another stream an exception should bethrown or the stream zero should be assumed</param>
        /// <param name="engineURI">uri of the engine</param>
        /// <param name="requireStreamNames">is true to indicate that stream names are required for any non-zero streams (for subqueries)</param>
        public StreamTypeServiceImpl(ICollection<KeyValuePair<string, Pair<EventType, string>>> namesAndTypes, String engineURI, bool isStreamZeroUnambigous, bool requireStreamNames)
        {
            this.isStreamZeroUnambigous = isStreamZeroUnambigous;
            this.requireStreamNames = requireStreamNames;
            this.engineURIQualifier = engineURI;
            eventTypes = new EventType[namesAndTypes.Count];
            streamNames = new String[namesAndTypes.Count];
            eventTypeAlias = new String[namesAndTypes.Count];
            int count = 0;
            foreach (KeyValuePair<String, Pair<EventType, String>> entry in namesAndTypes) {
                streamNames[count] = entry.Key;
                eventTypes[count] = entry.Value.First;
                eventTypeAlias[count] = entry.Value.Second;
                count++;
            }
        }

	    /// <summary>
        /// Returns the offset of the stream and the type of the property for the given property name,
        /// by looking through the types offered and matching up.
        /// <para>
        /// This method considers only a property name and looks at all streams to resolve the property name.
        /// </para>
        /// </summary>
        /// <param name="propertyName">property name in event</param>
        /// <returns>
        /// descriptor with stream number, property type and property name
        /// </returns>
        /// <throws>  DuplicatePropertyException to indicate property was found twice </throws>
        /// <throws>  PropertyNotFoundException to indicate property could not be resolved </throws>
        public virtual PropertyResolutionDescriptor ResolveByPropertyName(String propertyName)
		{
			if (propertyName == null)
			{
				throw new ArgumentException("Null property name");
			}
	        PropertyResolutionDescriptor desc = FindByPropertyName(propertyName);
	        if ((requireStreamNames) && (desc.StreamNum != 0))
	        {
                throw new PropertyNotFoundException("Property named '" + propertyName + "' must be prefixed by a stream name, use the as-clause to name the stream");
	        }
	        return desc;
		}

        /// <summary>
        /// Returns the offset of the stream and the type of the property for the given property name,
        /// by using the specified stream name to resolve the property.
        /// <para>
        /// This method considers and explicit stream name and property name, both parameters are required.
        /// </para>
        /// </summary>
        /// <param name="streamName">name of stream, required</param>
        /// <param name="propertyName">property name in event, , required</param>
        /// <returns>
        /// descriptor with stream number, property type and property name
        /// </returns>
        /// <throws>  PropertyNotFoundException to indicate property could not be resolved </throws>
        /// <throws>  StreamNotFoundException to indicate stream name could not be resolved </throws>
		public virtual PropertyResolutionDescriptor ResolveByStreamAndPropName(String streamName, String propertyName)
		{
			if (streamName == null)
			{
				throw new ArgumentException("Null property name");
			}
			if (propertyName == null)
			{
				throw new ArgumentException("Null property name");
			}
            return FindByStreamAndEngineName(propertyName, streamName);
        }

        /// <summary>
        /// Returns the offset of the stream and the type of the property for the given property name,
        /// by looking through the types offered and matching up.
        /// <para>
        /// This method considers a single property name that may or may not be prefixed by a stream name.
        /// The resolution first attempts to find the property name itself, then attempts
        /// to consider a stream name that may be part of the property name.
        /// </para>
        /// </summary>
        /// <param name="streamAndPropertyName">stream name and property name (e.g. s0.p0) or just a property name (p0)</param>
        /// <returns>
        /// descriptor with stream number, property type and property name
        /// </returns>
        /// <throws>  DuplicatePropertyException to indicate property was found twice </throws>
        /// <throws>  PropertyNotFoundException to indicate property could not be resolved </throws>
		public virtual PropertyResolutionDescriptor ResolveByStreamAndPropName(String streamAndPropertyName)
		{
			if (streamAndPropertyName == null)
			{
				throw new ArgumentException("Null stream and property name");
			}

			PropertyResolutionDescriptor desc;
			try
			{
				// first try to resolve as a property name
				desc = FindByPropertyName(streamAndPropertyName);
			}
			catch (PropertyNotFoundException ex)
			{
				// Attempt to resolve by extracting a stream name
                int index = ASTFilterSpecHelper.UnescapedIndexOfDot(streamAndPropertyName);
				if (index == - 1)
				{
					throw;
				}
				String streamName = streamAndPropertyName.Substring(0, (index) - (0));
				String propertyName = streamAndPropertyName.Substring(index + 1, (streamAndPropertyName.Length) - (index + 1));
				try
				{
					// try to resolve a stream and property name
                    desc = FindByStreamAndEngineName(propertyName, streamName);
                }
				catch (StreamNotFoundException)
				{
                    // Consider the engine URI as a further prefix
                    Pair<String, String> propertyNoEnginePair = GetIsEngineQualified(propertyName, streamName);
                    if (propertyNoEnginePair == null)
                    {
                        throw ex;
                    }
                    try
                    {
                        return FindByStreamNameOnly(propertyNoEnginePair.First, propertyNoEnginePair.Second);
                    }
                    catch (StreamNotFoundException)
                    {
                        throw ex;
                    }
                }
				return desc;
			}

			return desc;
		}

		private PropertyResolutionDescriptor FindByPropertyName(String propertyName)
		{
			int index = 0;
			int foundIndex = 0;
			int foundCount = 0;
			EventType streamType = null;

			for (int i = 0; i < eventTypes.Length; i++)
			{
				if (eventTypes[i].IsProperty(propertyName))
				{
					streamType = eventTypes[i];
					foundCount++;
					foundIndex = index;

					// If the property could be resolved from stream 0 then we don't need to look further
	                if ((i == 0) && isStreamZeroUnambigous)
	                {
	                    return new PropertyResolutionDescriptor(streamNames[0], eventTypes[0], propertyName, 0, streamType.GetPropertyType(propertyName));
	                }
				}
				index++;
			}

			if (foundCount > 1)
			{
				throw new DuplicatePropertyException("Property named '" + propertyName + "' is ambigous as is valid for more then one stream");
			}

			if (streamType == null)
			{
				throw new PropertyNotFoundException("Property named '" + propertyName + "' is not valid in any stream");
			}

			return new PropertyResolutionDescriptor(streamNames[foundIndex], eventTypes[foundIndex], propertyName, foundIndex, streamType.GetPropertyType(propertyName));
		}

        private PropertyResolutionDescriptor FindByStreamAndEngineName(String propertyName, String streamName)
        {
            PropertyResolutionDescriptor desc;
            try {
                desc = FindByStreamNameOnly(propertyName, streamName);
            } catch (PropertyNotFoundException) {
                Pair<String, String> propertyNoEnginePair = GetIsEngineQualified(propertyName, streamName);
                if (propertyNoEnginePair == null) {
                    throw;
                }
                return FindByStreamNameOnly(propertyNoEnginePair.First, propertyNoEnginePair.Second);
            } catch (StreamNotFoundException) {
                Pair<String, String> propertyNoEnginePair = GetIsEngineQualified(propertyName, streamName);
                if (propertyNoEnginePair == null) {
                    throw;
                }
                return FindByStreamNameOnly(propertyNoEnginePair.First, propertyNoEnginePair.Second);
            }
            return desc;
        }

        private Pair<String, String> GetIsEngineQualified(String propertyName, String streamName)
        {
            // If still not found, test for the stream name to contain the engine URI
            if (streamName != engineURIQualifier) {
                return null;
            }

            int index = ASTFilterSpecHelper.UnescapedIndexOfDot(propertyName);
            if (index == -1) {
                return null;
            }

            String streamNameNoEngine = propertyName.Substring(0, index);
            String propertyNameNoEngine = propertyName.Substring(index + 1);
            return new Pair<String, String>(propertyNameNoEngine, streamNameNoEngine);
        }

	    private PropertyResolutionDescriptor FindByStreamNameOnly(String propertyName, String streamName)
		{
			int index = 0;
			EventType streamType = null;

            // Stream name resultion examples:
            // A)  select A1.price from Event.price as A2  => mismatch stream alias, cannot resolve
            // B)  select Event1.price from Event2.price   => mismatch event type alias, cannot resolve
            // C)  select default.Event2.price from Event2.price   => possible prefix of engine name
            for (int i = 0; i < eventTypes.Length; i++)
			{
				if ((streamNames[i] != null) && (streamNames[i] == streamName))
				{
					streamType = eventTypes[i];
					break;
				}

                // If the stream name is the event type alias, that is also acceptable
                if ((eventTypeAlias[i] != null) && (eventTypeAlias[i] == streamName))
                {
                    streamType = eventTypes[i];
                    break;
                }

                index++;
			}

			if (streamType == null)
			{
				throw new StreamNotFoundException("Stream named " + streamName + " is not defined");
			}

			Type propertyType = streamType.GetPropertyType(propertyName);
			if (propertyType == null)
			{
				throw new PropertyNotFoundException("Property named '" + propertyName + "' is not valid in stream " + streamName);
			}

			return new PropertyResolutionDescriptor(streamName, streamType, propertyName, index, propertyType);
		}
	}
}
