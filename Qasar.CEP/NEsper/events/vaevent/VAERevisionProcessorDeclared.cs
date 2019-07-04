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
using com.espertech.esper.core;
using com.espertech.esper.epl.join.table;
using com.espertech.esper.epl.named;
using com.espertech.esper.events;
using com.espertech.esper.view;

using log4net;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// Provides overlay strategy for property group-based versioning.
    /// </summary>
    public class VAERevisionProcessorDeclared : VAERevisionProcessorBase, ValueAddEventProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
        private readonly PropertyGroupDesc[] groups;
        private readonly EventType baseEventType;
        private readonly EventPropertyGetter[] fullKeyGetters;
        private readonly Map<MultiKeyUntyped, RevisionStateDeclared> statePerKey;
    
        /// <summary>Ctor. </summary>
        /// <param name="revisionEventTypeAlias">alias</param>
        /// <param name="spec">specification</param>
        /// <param name="statementStopService">for stop handling</param>
        /// <param name="eventAdapterService">for nested property handling</param>
        public VAERevisionProcessorDeclared(String revisionEventTypeAlias, RevisionSpec spec, StatementStopService statementStopService, EventAdapterService eventAdapterService)
            : base(spec, revisionEventTypeAlias, eventAdapterService)
        {
            
            // on statement stop, remove versions
            statementStopService.AddSubscriber(
                new ProxyStatementStopCallback(
                    delegate {
                        statePerKey.Clear();
                    }));
    
            this.statePerKey = new HashMap<MultiKeyUntyped, RevisionStateDeclared>();
            this.baseEventType = spec.BaseEventType;
            this.fullKeyGetters = PropertyUtility.GetGetters(baseEventType, spec.KeyPropertyNames);
    
            // sort non-key properties, removing keys
            groups = PropertyUtility.AnalyzeGroups(spec.ChangesetPropertyNames, spec.DeltaTypes, spec.DeltaAliases);
    
            Map<String, int[]> propsPerGroup = PropertyUtility.GetGroupsPerProperty(groups);
            Map<String, RevisionPropertyTypeDesc> propertyDesc = new HashMap<String, RevisionPropertyTypeDesc>();
            int count = 0;
    
            foreach (String property in spec.ChangesetPropertyNames)
            {
                EventPropertyGetter fullGetter = spec.BaseEventType.GetGetter(property);
                int propertyNumber = count;
                int[] propGroupsProperty = propsPerGroup.Get(property);
                RevisionGetterParameters paramList = new RevisionGetterParameters(property, propertyNumber, fullGetter, propGroupsProperty);
    
                // if there are no groups (full event property only), then simply use the full event getter
                EventPropertyGetter revisionGetter = new ProxyEventPropertyGetter(
                    delegate(EventBean eventBean) {
                        RevisionEventBeanDeclared riv = (RevisionEventBeanDeclared) eventBean;
                        return riv.GetVersionedValue(paramList);
                    },
                    delegate {
                        return true;
                    });
    
                Type type = spec.BaseEventType.GetPropertyType(property);
                RevisionPropertyTypeDesc propertyTypeDesc = new RevisionPropertyTypeDesc(revisionGetter, paramList, type);
                propertyDesc.Put(property, propertyTypeDesc);
                count++;
            }
    
            foreach (String property in spec.BaseEventOnlyPropertyNames)
            {
                EventPropertyGetter fullGetter = spec.BaseEventType.GetGetter(property);
    
                // if there are no groups (full event property only), then simply use the full event getter
                EventPropertyGetter revisionGetter = new ProxyEventPropertyGetter(
                    delegate(EventBean eventBean) {
                        RevisionEventBeanDeclared riv = (RevisionEventBeanDeclared) eventBean;
                        return fullGetter.GetValue(riv.LastBaseEvent);
                    },
                    delegate {
                        return true;
                    });
    
                Type type = spec.BaseEventType.GetPropertyType(property);
                RevisionPropertyTypeDesc propertyTypeDesc = new RevisionPropertyTypeDesc(revisionGetter, null, type);
                propertyDesc.Put(property, propertyTypeDesc);
                count++;
            }
    
            count = 0;
            foreach (String property in spec.KeyPropertyNames)
            {
                int keyPropertyNumber = count;

                EventPropertyGetter revisionGetter = new ProxyEventPropertyGetter(
                    delegate(EventBean eventBean) {
                        RevisionEventBeanDeclared riv = (RevisionEventBeanDeclared) eventBean;
                        return riv.Key.Keys[keyPropertyNumber];
                    },
                    delegate {
                        return true;
                    });
    
                Type type = spec.BaseEventType.GetPropertyType(property);
                RevisionPropertyTypeDesc propertyTypeDesc = new RevisionPropertyTypeDesc(revisionGetter, null, type);
                propertyDesc.Put(property, propertyTypeDesc);
                count++;
            }
    
            typeDescriptors = PropertyUtility.GetPerType(groups, spec.ChangesetPropertyNames, spec.KeyPropertyNames);
            revisionEventType = new RevisionEventType(propertyDesc, eventAdapterService);
        }
    
        public override EventBean GetValueAddEventBean(EventBean @event)
        {
            return new RevisionEventBeanDeclared(revisionEventType, @event);
        }

        public override void OnUpdate(EventBean[] newData, EventBean[] oldData, NamedWindowRootView namedWindowRootView, NamedWindowIndexRepository indexRepository)
        {
            // If new data is filled, it is not a delete
            RevisionEventBeanDeclared revisionEvent;
            MultiKeyUntyped key;
            if ((newData == null) || (newData.Length == 0))
            {
                // we are removing an event
                revisionEvent = (RevisionEventBeanDeclared) oldData[0];
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
    
            revisionEvent = (RevisionEventBeanDeclared) newData[0];
            EventBean underlyingEvent = revisionEvent.UnderlyingFullOrDelta;
            EventType underyingEventType = underlyingEvent.EventType;
    
            // obtain key values
            key = null;
            RevisionTypeDesc typesDesc = null;
            bool isBaseEventType = false;
            if (underyingEventType == baseEventType)
            {
                key = PropertyUtility.GetKeys(underlyingEvent, fullKeyGetters);
                isBaseEventType = true;
            }
            else
            {
                typesDesc = typeDescriptors.Get(underyingEventType);
    
                // if this type cannot be found, check all supertypes, if any
                if (typesDesc == null)
                {
                    IEnumerable<EventType> superTypes = underyingEventType.DeepSuperTypes;
                    if (superTypes != null)
                    {
                        foreach( EventType superType in superTypes )
                        {
                            if (superType == baseEventType)
                            {
                                key = PropertyUtility.GetKeys(underlyingEvent, fullKeyGetters);
                                isBaseEventType = true;
                                break;
                            }
                            typesDesc = typeDescriptors.Get(superType);
                            if (typesDesc != null)
                            {
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
            RevisionStateDeclared revisionState = statePerKey.Get(key);
    
            // Delta event and no full
            if ((!isBaseEventType) && (revisionState == null))
            {
                return; // Ignore the event, its a delta and we don't currently have a full event for it
            }
    
            // New full event
            if (revisionState == null)
            {
                revisionState = new RevisionStateDeclared(underlyingEvent, null, null);
                statePerKey.Put(key, revisionState);
    
                // prepare revison event
                revisionEvent.LastBaseEvent = underlyingEvent;
                revisionEvent.Key = key;
                revisionEvent.Holders = null;
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
    
            // new version
            long versionNumber = revisionState.IncRevisionNumber();
    
            // Previously-seen full event
            if (isBaseEventType)
            {
                revisionState.Holders = null;
                revisionState.BaseEventUnderlying = underlyingEvent;
            }
            // Delta event to existing full event
            else
            {
                int groupNum = typesDesc.Group.GroupNum;
                RevisionBeanHolder[] holders = revisionState.Holders;
                if (holders == null)    // optimization - the full event sets it to null, deltas all get a new one
                {
                    holders = new RevisionBeanHolder[groups.Length];
                }
                else
                {
                    holders = ArrayCopy(holders);   // preserve the last revisions
                }
    
                // add the new revision for a property group on top
                holders[groupNum] = new RevisionBeanHolder(versionNumber, underlyingEvent, typesDesc.ChangesetPropertyGetters);
                revisionState.Holders = holders;
            }
    
            // prepare revision event
            revisionEvent.LastBaseEvent = revisionState.BaseEventUnderlying;
            revisionEvent.Holders = revisionState.Holders;
            revisionEvent.Key = key;
            revisionEvent.IsLatest = true;
    
            // get prior event
            RevisionEventBeanDeclared lastEvent = revisionState.LastEvent;
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
                    RevisionEventBeanDeclared fullRevision = (RevisionEventBeanDeclared) it.Current;
                    MultiKeyUntyped key = fullRevision.Key;
                    RevisionStateDeclared state = statePerKey.Get(key);
                    list.AddLast(state.LastEvent);
                } while (it.MoveNext());

                return list;
            }
        }

        public override void RemoveOldData(EventBean[] oldData, NamedWindowIndexRepository indexRepository)
        {
            for (int i = 0; i < oldData.Length; i++)
            {
                RevisionEventBeanDeclared @event = (RevisionEventBeanDeclared) oldData[i];
    
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
    
        private static RevisionBeanHolder[] ArrayCopy(RevisionBeanHolder[] array)
        {
            if (array == null)
            {
                return null;
            }
            RevisionBeanHolder[] result = new RevisionBeanHolder[array.Length];
            Array.Copy(array, 0, result, 0, array.Length);
            return result;
        }
    }
}
