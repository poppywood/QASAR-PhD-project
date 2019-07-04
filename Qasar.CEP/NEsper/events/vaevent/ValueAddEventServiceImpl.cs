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
using com.espertech.esper.events;
using com.espertech.esper.view;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// Service for handling revision event types.
    /// <para/>
    /// Each named window instance gets a dedicated revision processor.
    /// </summary>
    public class ValueAddEventServiceImpl : ValueAddEventService
    {
        private readonly Map<String, RevisionSpec> specificationsByRevisionAlias;
        private readonly Map<String, ValueAddEventProcessor> processorsByNamedWindow;
        private readonly Map<String, ValueAddEventProcessor> variantProcessors;

        /// <summary>
        /// Ctor.
        /// </summary>
        public ValueAddEventServiceImpl()
        {
            this.specificationsByRevisionAlias = new HashMap<String, RevisionSpec>();
            this.processorsByNamedWindow = new HashMap<String, ValueAddEventProcessor>();
            variantProcessors = new HashMap<String, ValueAddEventProcessor>();
        }

        /// <summary>
        /// Inits the specified config revision.
        /// </summary>
        /// <param name="configRevision">The config revision.</param>
        /// <param name="configVariant">The config variant.</param>
        /// <param name="eventAdapterService">The event adapter service.</param>
        public void Init(Map<String, ConfigurationRevisionEventType> configRevision,
                         Map<String, ConfigurationVariantStream> configVariant,
                         EventAdapterService eventAdapterService)
        {
            foreach (KeyValuePair<String, ConfigurationRevisionEventType> entry in configRevision)
            {
                AddRevisionEventType(entry.Key, entry.Value, eventAdapterService);
            }
            foreach (KeyValuePair<String, ConfigurationVariantStream> entry in configVariant)
            {
                AddVariantStream(entry.Key, entry.Value, eventAdapterService);
            }
        }

        /// <summary>
        /// Adds the type of the revision event.
        /// </summary>
        /// <param name="revisionEventTypeAlias">The revision event type alias.</param>
        /// <param name="config">The config.</param>
        /// <param name="eventAdapterService">The event adapter service.</param>
        public void AddRevisionEventType(String revisionEventTypeAlias,
                                         ConfigurationRevisionEventType config,
                                         EventAdapterService eventAdapterService)
        {
            if ((config.AliasBaseEventTypes == null) || (config.AliasBaseEventTypes.Count == 0))
            {
                throw new ConfigurationException("Required base event type alias is not set in the configuration for revision event type '" + revisionEventTypeAlias + "'");
            }
    
            if (config.AliasBaseEventTypes.Count > 1)
            {
                throw new ConfigurationException("Only one base event type alias may be added to revision event type '" + revisionEventTypeAlias + "', multiple base types are not yet supported");
            }
    
            // get base types
            String baseEventTypeAlias = CollectionHelper.First(config.AliasBaseEventTypes);
            EventType baseEventType = eventAdapterService.GetEventTypeByAlias(baseEventTypeAlias);
            if (baseEventType == null)
            {
                throw new ConfigurationException("Could not locate event type for alias '" + baseEventTypeAlias + "' in the configuration for revision event type '" + revisionEventTypeAlias + "'");
            }
    
            // get alias types
            EventType[] deltaTypes = new EventType[config.AliasDeltaEventTypes.Count];
            String[] deltaAliases = new String[config.AliasDeltaEventTypes.Count];
            int count = 0;
            foreach (String deltaAlias in config.AliasDeltaEventTypes)
            {
                EventType deltaEventType = eventAdapterService.GetEventTypeByAlias(deltaAlias);
                if (deltaEventType == null)
                {
                    throw new ConfigurationException("Could not locate event type for alias '" + deltaAlias + "' in the configuration for revision event type '" + revisionEventTypeAlias + "'");
                }
                deltaTypes[count] = deltaEventType;
                deltaAliases[count] = deltaAlias;
                count++;
            }
    
            // the key properties must be set
            if ((config.KeyPropertyNames == null) || (config.KeyPropertyNames.Count == 0))
            {
                throw new ConfigurationException("Required key properties are not set in the configuration for revision event type '" + revisionEventTypeAlias + "'");
            }
    
            // make sure the key properties exist the base type and all delta types
            CheckKeysExist(baseEventType, baseEventTypeAlias, config.KeyPropertyNames, revisionEventTypeAlias);
            for (int i = 0; i < deltaTypes.Length; i++)
            {
                CheckKeysExist(deltaTypes[i], deltaAliases[i], config.KeyPropertyNames, revisionEventTypeAlias);
            }
    
            // key property names shared between base and delta must have the same type
            String[] keyPropertyNames = PropertyUtility.CopyAndSort(config.KeyPropertyNames);
            foreach (String key in keyPropertyNames)
            {
                Type typeProperty = baseEventType.GetPropertyType(key);
                foreach (EventType dtype in deltaTypes)
                {
                    Type dtypeProperty = dtype.GetPropertyType(key);
                    if ((dtypeProperty != null) && (typeProperty != dtypeProperty))
                    {
                        throw new ConfigurationException("Key property named '" + key + "' does not have the same type for base and delta types of revision event type '" + revisionEventTypeAlias + "'");
                    }
                }
            }
    
            RevisionSpec specification;
            // In the "declared" type the change set properties consist of only :
            //   (base event type properties) minus (key properties) minus (properties only on base event type)
            if (config.PropertyRevision == PropertyRevision.OVERLAY_DECLARED)
            {
                // determine non-key properties: those overridden by any delta, and those simply only present on the base event type
                String[] nonkeyPropertyNames = PropertyUtility.UniqueExclusiveSort(
                    baseEventType.PropertyNames, keyPropertyNames);
                Set<String> baseEventOnlyProperties = new HashSet<String>();
                Set<String> changesetPropertyNames = new HashSet<String>();
                foreach (String nonKey in nonkeyPropertyNames)
                {
                    bool overriddenProperty = false;
                    foreach (EventType type in deltaTypes)
                    {
                        if (type.IsProperty(nonKey))
                        {
                            changesetPropertyNames.Add(nonKey);
                            overriddenProperty = true;
                            break;
                        }
                    }
                    if (!overriddenProperty)
                    {
                        baseEventOnlyProperties.Add(nonKey);
                    }
                }
    
                String[] changesetProperties = changesetPropertyNames.ToArray();
                String[] baseEventOnlyPropertyNames = baseEventOnlyProperties.ToArray();
                PropertyUtility.RemovePropNamePostfixes(changesetProperties);
                PropertyUtility.RemovePropNamePostfixes(baseEventOnlyPropertyNames);
    
                // verify that all changeset properties match event type
                foreach (String changesetProperty in changesetProperties)
                {
                    Type typeProperty = baseEventType.GetPropertyType(changesetProperty);
                    foreach (EventType dtype in deltaTypes)
                    {
                        Type dtypeProperty = dtype.GetPropertyType(changesetProperty);
                        if ((dtypeProperty != null) && (typeProperty != dtypeProperty))
                        {
                            throw new ConfigurationException("Property named '" + changesetProperty + "' does not have the same type for base and delta types of revision event type '" + revisionEventTypeAlias + "'");
                        }
                    }
                }
    
                specification = new RevisionSpec(config.PropertyRevision, baseEventType, deltaTypes, deltaAliases, keyPropertyNames, changesetProperties, baseEventOnlyPropertyNames, false, null);
            }
            else
            {
                // In the "exists" type the change set properties consist of all properties: base event properties plus delta types properties
                Set<String> allProperties = new HashSet<String>();
                allProperties.AddAll(baseEventType.PropertyNames);
                foreach (EventType deltaType in deltaTypes)
                {
                    allProperties.AddAll(deltaType.PropertyNames);
                }
    
                String[] allPropertiesArr = allProperties.ToArray();
                String[] changesetProperties = PropertyUtility.UniqueExclusiveSort(allPropertiesArr, keyPropertyNames);
                PropertyUtility.RemovePropNamePostfixes(allPropertiesArr);
                PropertyUtility.RemovePropNamePostfixes(changesetProperties);
    
                // All properties must have the same type, if a property exists for any given type
                bool hasContributedByDelta = false;
                bool[] contributedByDelta = new bool[changesetProperties.Length];
                count = 0;
                foreach (String property in changesetProperties)
                {
                    String mproperty = property;
                    if (mproperty.EndsWith("[]"))
                    {
                        mproperty = property.Replace("[]", "");
                    }
                    if (mproperty.EndsWith("()"))
                    {
                        mproperty = property.Replace("()", "");
                    }

                    Type basePropertyType = baseEventType.GetPropertyType(mproperty);
                    Type typeTemp = null;
                    if (basePropertyType != null)
                    {
                        typeTemp = basePropertyType;
                    }
                    else
                    {
                        hasContributedByDelta = true;
                        contributedByDelta[count] = true;
                    }
                    foreach (EventType dtype in deltaTypes)
                    {
                        Type dtypeProperty = dtype.GetPropertyType(mproperty);
                        if (dtypeProperty != null)
                        {
                            if ((typeTemp != null) && (dtypeProperty != typeTemp))
                            {
                                throw new ConfigurationException("Property named '" + mproperty + "' does not have the same type for base and delta types of revision event type '" + revisionEventTypeAlias + "'");
                            }
    
                        }
                        typeTemp = dtypeProperty;
                    }
                    count++;
                }
    
                // Compile changeset
                specification = new RevisionSpec(config.PropertyRevision, baseEventType, deltaTypes, deltaAliases, keyPropertyNames, changesetProperties, new String[0], hasContributedByDelta, contributedByDelta);
            }
            specificationsByRevisionAlias.Put(revisionEventTypeAlias, specification);
        }
    
        public void AddVariantStream(String variantStreamname, ConfigurationVariantStream variantStreamConfig, EventAdapterService eventAdapterService)
        {
            if (variantStreamConfig.TypeVariance == TypeVariance.PREDEFINED)
            {
                if (CollectionHelper.IsEmpty(variantStreamConfig.VariantTypeAliases))
                {
                    throw new ConfigurationException("Invalid variant stream configuration, no event type alias has been added and default type variance requires at least one type, for name '" + variantStreamname + "'");
                }
            }
    
            Set<EventType> types = new LinkedHashSet<EventType>();
            foreach (String alias in variantStreamConfig.VariantTypeAliases)
            {
                EventType type = eventAdapterService.GetEventTypeByAlias(alias);
                if (type == null)
                {
                    throw new ConfigurationException("Event type by name '" + alias + "' could not be found for use in variant stream configuration by name '" + variantStreamname + "'");
                }
                types.Add(type);
            }
    
            EventType[] eventTypes = types.ToArray();
            VariantSpec variantSpec = new VariantSpec(variantStreamname, eventTypes, variantStreamConfig.TypeVariance);
            VAEVariantProcessor processor = new VAEVariantProcessor(variantSpec);
    
            eventAdapterService.AddTypeByAlias(variantStreamname, processor.ValueAddEventType);
            variantProcessors.Put(variantStreamname, processor);
        }
    
        public EventType CreateRevisionType(String namedWindowName, String alias, StatementStopService statementStopService, EventAdapterService eventAdapterService)
        {
            RevisionSpec spec = specificationsByRevisionAlias.Get(alias);
            ValueAddEventProcessor processor;
            if (spec.PropertyRevision == PropertyRevision.OVERLAY_DECLARED)
            {
                processor = new VAERevisionProcessorDeclared(alias, spec, statementStopService, eventAdapterService);
            }
            else
            {
                processor = new VAERevisionProcessorMerge(alias, spec, statementStopService, eventAdapterService);
            }
    
            processorsByNamedWindow.Put(namedWindowName, processor);
            return processor.ValueAddEventType;
        }
    
        public ValueAddEventProcessor GetValueAddProcessor(String alias)
        {
            ValueAddEventProcessor proc = processorsByNamedWindow.Get(alias);
            if (proc != null)
            {
                return proc;
            }
            return variantProcessors.Get(alias);
        }
    
        public EventType GetValueAddUnderlyingType(String alias)
        {
            RevisionSpec spec = specificationsByRevisionAlias.Get(alias);
            return spec.BaseEventType;
        }
        
        public bool IsRevisionTypeAlias(String revisionTypeAlias)
        {
            return specificationsByRevisionAlias.ContainsKey(revisionTypeAlias);
        }

        private static void CheckKeysExist(EventType baseEventType,
                                           String alias,
                                           IEnumerable<string> keyProperties,
                                           String revisionEventTypeAlias)
        {
            foreach (String keyProperty in keyProperties)
            {
                bool exists = baseEventType.IsProperty(keyProperty);
                if (!exists)
                {
                    throw new ConfigurationException("Key property '" + keyProperty + "' as defined in the configuration for revision event type '" + revisionEventTypeAlias + "' does not exists in event type '" + alias + "'");
                }
            }
        }
    }
}
