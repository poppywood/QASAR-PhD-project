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
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.epl.named;
using com.espertech.esper.events;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// Provides a set of merge-strategies for merging individual properties 
    /// (rather then overlaying groups). 
    /// </summary>
    public class VAERevisionProcessorMerge
        : VAERevisionProcessorBase
        , ValueAddEventProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
        private readonly RevisionTypeDesc infoFullType;
        private readonly Map<MultiKeyUntyped, RevisionStateMerge> statePerKey;
        private readonly UpdateStrategy updateStrategy;
    
        /// <summary>Ctor. </summary>
        /// <param name="revisionEventTypeAlias">alias</param>
        /// <param name="spec">specification</param>
        /// <param name="statementStopService">for stop handling</param>
        /// <param name="eventAdapterService">for nested property handling</param>
        public VAERevisionProcessorMerge(String revisionEventTypeAlias, RevisionSpec spec, StatementStopService statementStopService, EventAdapterService eventAdapterService)
            : base(spec, revisionEventTypeAlias, eventAdapterService)
        {
    
            // on statement stop, remove versions
            statementStopService.AddSubscriber(
                new ProxyStatementStopCallback(
                    delegate {
                        statePerKey.Clear();
                    }));
    
            this.statePerKey = new HashMap<MultiKeyUntyped, RevisionStateMerge>();
    
            // For all changeset properties, add type descriptors (property number, getter etc)
            Map<String, RevisionPropertyTypeDesc> propertyDesc = new HashMap<String, RevisionPropertyTypeDesc>();
            int count = 0;
    
            foreach (String property in spec.ChangesetPropertyNames)
            {
                EventPropertyGetter fullGetter = spec.BaseEventType.GetGetter(property);
                int propertyNumber = count;
                RevisionGetterParameters paramList = new RevisionGetterParameters(property, propertyNumber, fullGetter, null);
    
                // if there are no groups (full event property only), then simply use the full event getter
                EventPropertyGetter revisionGetter = new ProxyEventPropertyGetter(
                    delegate(EventBean eventBean) {
                        RevisionEventBeanMerge riv = (RevisionEventBeanMerge) eventBean;
                        return riv.GetVersionedValue(paramList);
                    },
                    delegate {
                        return true;
                    });
    
                Type type = spec.BaseEventType.GetPropertyType(property);
                if (type == null)
                {
                    foreach (EventType deltaType in spec.DeltaTypes)
                    {
                        Type dtype = deltaType.GetPropertyType(property);
                        if (dtype != null)
                        {
                            type = dtype;
                            break;
                        }
                    }
                }
                RevisionPropertyTypeDesc propertyTypeDesc = new RevisionPropertyTypeDesc(revisionGetter, paramList, type);
                propertyDesc.Put(property, propertyTypeDesc);
                count++;
            }
    
            count = 0;
            foreach (String property in spec.KeyPropertyNames)
            {
                int keyPropertyNumber = count;

                EventPropertyGetter revisionGetter = new ProxyEventPropertyGetter(
                    delegate(EventBean eventBean) {
                        RevisionEventBeanMerge riv = (RevisionEventBeanMerge) eventBean;
                        return riv.Key.Keys[keyPropertyNumber];
                    },
                    delegate {
                        return true;
                    });
    
                Type type = spec.BaseEventType.GetPropertyType(property);
                if (type == null)
                {
                    foreach (EventType deltaType in spec.DeltaTypes)
                    {
                        Type dtype = deltaType.GetPropertyType(property);
                        if (dtype != null)
                        {
                            type = dtype;
                            break;
                        }
                    }
                }
                RevisionPropertyTypeDesc propertyTypeDesc = new RevisionPropertyTypeDesc(revisionGetter, null, type);
                propertyDesc.Put(property, propertyTypeDesc);
                count++;
            }
    
            // compile for each event type a list of getters and indexes within the overlay
            foreach (EventType deltaType in spec.DeltaTypes)
            {
                RevisionTypeDesc typeDesc = MakeTypeDesc(deltaType, spec.PropertyRevision);
                typeDescriptors.Put(deltaType, typeDesc);
            }
            infoFullType = MakeTypeDesc(spec.BaseEventType, spec.PropertyRevision);
    
            // how to handle updates to a full event
            if (spec.PropertyRevision == PropertyRevision.MERGE_DECLARED)
            {
                updateStrategy = new UpdateStrategyDeclared(spec);
            }
            else if (spec.PropertyRevision == PropertyRevision.MERGE_NON_NULL)
            {
                updateStrategy = new UpdateStrategyNonNull(spec);
            }         
            else if (spec.PropertyRevision == PropertyRevision.MERGE_EXISTS)
            {
                updateStrategy = new UpdateStrategyExists(spec);
            }
            else
            {
                throw new ArgumentException("Unknown revision type '" + spec.PropertyRevision + "'");
            }
    
            revisionEventType = new RevisionEventType(propertyDesc, eventAdapterService);
        }
    
        public override EventBean GetValueAddEventBean(EventBean @event)
        {
            return new RevisionEventBeanMerge(revisionEventType, @event);
        }

        public override void OnUpdate(EventBean[] newData,
                                      EventBean[] oldData,
                                      NamedWindowRootView namedWindowRootView,
                                      NamedWindowIndexRepository indexRepository)
        {
            // If new data is filled, it is not a delete
            RevisionEventBeanMerge revisionEvent;
            MultiKeyUntyped key;
            if ((newData == null) || (newData.Length == 0))
            {
                // we are removing an event
                revisionEvent = (RevisionEventBeanMerge) oldData[0];
                key = revisionEvent.Key;
                statePerKey.Remove(key);
    
                // Insert into indexes for fast deletion, if there are any
                foreach (EventTable table in indexRepository.Tables)
                {
                    table.Remove(oldData);
                }
    
                // make as not the latest event since its due for removal
                revisionEvent.IsLatest = false;
    
                namedWindowRootView.UpdateChildren(null, oldData);
                return;
            }
    
            revisionEvent = (RevisionEventBeanMerge) newData[0];
            EventBean underlyingEvent = revisionEvent.UnderlyingFullOrDelta;
            EventType underyingEventType = underlyingEvent.EventType;
    
            // obtain key values
            key = null;
            RevisionTypeDesc typesDesc;
            bool isBaseEventType = false;
            if (underyingEventType == revisionSpec.BaseEventType)
            {
                typesDesc = infoFullType;
                key = PropertyUtility.GetKeys(underlyingEvent, infoFullType.KeyPropertyGetters);
                isBaseEventType = true;
            }
            else
            {
                typesDesc = typeDescriptors.Get(underyingEventType);
    
                // if this type cannot be found, check all supertypes, if any
                if (typesDesc == null)
                {
                    IEnumerable<EventType> superTypes = underyingEventType.DeepSuperTypes;
                    if (superTypes != null) {
                        foreach( EventType superType in superTypes ) {
                            if (superType == revisionSpec.BaseEventType) {
                                typesDesc = infoFullType;
                                key = PropertyUtility.GetKeys(underlyingEvent, infoFullType.KeyPropertyGetters);
                                isBaseEventType = true;
                                break;
                            }
                            typesDesc = typeDescriptors.Get(superType);
                            if (typesDesc != null) {
                                typeDescriptors.Put(underyingEventType, typesDesc);
                                key = PropertyUtility.GetKeys(underlyingEvent, typesDesc.KeyPropertyGetters);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    key = PropertyUtility.GetKeys(underlyingEvent, typesDesc.KeyPropertyGetters);
                }
    
                if (key == null)
                {
                    log.Warn("Ignoring event of event type '" + underyingEventType + "' for revision processing type '" + revisionEventTypeAlias);
                    return;
                }
            }
    
            // get the state for this key value
            RevisionStateMerge revisionState = statePerKey.Get(key);
    
            // Delta event and no full
            if ((!isBaseEventType) && (revisionState == null))
            {
                return; // Ignore the event, its a delta and we don't currently have a full event for it
            }
    
            // New full event
            if (revisionState == null)
            {
                revisionState = new RevisionStateMerge(underlyingEvent, null, null);
                statePerKey.Put(key, revisionState);
    
                // prepare revison event
                revisionEvent.LastBaseEvent = underlyingEvent;
                revisionEvent.Key = key;
                revisionEvent.Overlay = null;
                revisionEvent.IsLatest = true;
    
                // Insert into indexes for fast deletion, if there are any
                foreach (EventTable table in indexRepository.Tables)
                {
                    table.Add(newData);
                }
    
                // post to data window
                revisionState.LastEvent = revisionEvent;
                namedWindowRootView.UpdateChildren(new EventBean[] {revisionEvent}, null);
                return;
            }
    
            // handle update, changing revision state and event as required
            updateStrategy.HandleUpdate(isBaseEventType, revisionState, revisionEvent, typesDesc);
    
            // prepare revision event
            revisionEvent.LastBaseEvent = revisionState.BaseEventUnderlying;
            revisionEvent.Overlay = revisionState.Overlays;
            revisionEvent.Key = key;
            revisionEvent.IsLatest = true;
    
            // get prior event
            RevisionEventBeanMerge lastEvent = revisionState.LastEvent;
            lastEvent.IsLatest = false;
    
            // data to post
            EventBean[] newDataPost = new EventBean[]{revisionEvent};
            EventBean[] oldDataPost = new EventBean[]{lastEvent};
    
            // update indexes
            foreach (EventTable table in indexRepository.Tables)
            {
                table.Remove(oldDataPost);
                table.Add(newDataPost);
            }
    
            // keep reference to last event
            revisionState.LastEvent = revisionEvent;
    
            namedWindowRootView.UpdateChildren(newDataPost, oldDataPost);
        }
    
        private static readonly EventBean[] _emptyList = new EventBean[] {};

        public override ICollection<EventBean> GetSnapshot(EPStatementHandle createWindowStmtHandle, Viewable parent)
        {
            using( createWindowStmtHandle.StatementLock.AcquireLock(null) ) {
                IEnumerator<EventBean> it = parent.GetEnumerator();
                if (!it.MoveNext()) {
                    return _emptyList;
                }

                LinkedList<EventBean> list = new LinkedList<EventBean>();
                do {
                    RevisionEventBeanMerge fullRevision = (RevisionEventBeanMerge) it.Current;
                    MultiKeyUntyped key = fullRevision.Key;
                    RevisionStateMerge state = statePerKey.Get(key);
                    list.AddLast(state.LastEvent);
                } while (it.MoveNext());

                return list;
            }
        }
    
        public override void RemoveOldData(EventBean[] oldData, NamedWindowIndexRepository indexRepository)
        {
            foreach (EventBean anOldData in oldData)
            {
                RevisionEventBeanMerge @event = (RevisionEventBeanMerge) anOldData;
    
                // If the remove event is the latest event, remove from all caches
                if (@event.IsLatest)
                {
                    MultiKeyUntyped key = @event.Key;
                    statePerKey.Remove(key);
    
                    foreach (EventTable table in indexRepository.Tables)
                    {
                        table.Remove(oldData);
                    }
                }
            }
        }
    
        private RevisionTypeDesc MakeTypeDesc(EventType eventType, PropertyRevision propertyRevision)
        {
            EventPropertyGetter[] keyPropertyGetters = PropertyUtility.GetGetters(eventType, revisionSpec.KeyPropertyNames);
    
            int len = revisionSpec.ChangesetPropertyNames.Length;
            List<EventPropertyGetter> listOfGetters = new List<EventPropertyGetter>();
            List<int> listOfIndexes = new List<int>();
    
            for (int i = 0; i < len; i++)
            {
                String propertyName = revisionSpec.ChangesetPropertyNames[i];
                EventPropertyGetter getter = null;
    
                if (propertyRevision != PropertyRevision.MERGE_EXISTS)
                {
                    getter = eventType.GetGetter(revisionSpec.ChangesetPropertyNames[i]);
                }
                else
                {
                    // only declared properties may be used a dynamic properties to avoid confusion of properties suddenly appearing
                    foreach (String propertyNamesDeclared in eventType.PropertyNames)
                    {
                        if (propertyNamesDeclared.Equals(propertyName))
                        {
                            // use dynamic properties
                            getter = eventType.GetGetter(revisionSpec.ChangesetPropertyNames[i] + "?");
                            break;
                        }
                    }
                }
                
                if (getter != null)
                {
                    listOfGetters.Add(getter);
                    listOfIndexes.Add(i);
                }
            }
    
            EventPropertyGetter[] changesetPropertyGetters = listOfGetters.ToArray();
            int[] changesetPropertyIndex = new int[listOfIndexes.Count];
            for (int i = 0; i < listOfIndexes.Count; i++)
            {
                changesetPropertyIndex[i] = listOfIndexes[i];
            }
    
            return new RevisionTypeDesc(keyPropertyGetters, changesetPropertyGetters, changesetPropertyIndex);
        }    
    }
}
