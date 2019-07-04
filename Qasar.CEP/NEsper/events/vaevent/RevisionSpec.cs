///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;

using com.espertech.esper.client;
using com.espertech.esper.events;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// Specification for how to build a revision event type.
    /// <para/>
    /// Compiled from the information provided via configuration, which has already been validated
    /// before building this specification.
    /// </summary>
    public class RevisionSpec
    {
        private readonly PropertyRevision propertyRevision;
        private readonly EventType baseEventType;
        private readonly EventType[] deltaTypes;
        private readonly String[] deltaAliases;
        private readonly String[] keyPropertyNames;
        private readonly String[] changesetPropertyNames;
        private readonly String[] baseEventOnlyPropertyNames;
        private readonly bool deltaTypesAddProperties;
        private readonly bool[] changesetPropertyDeltaContributed;

        /// <summary>Ctor. </summary>
        /// <param name="propertyRevision">strategy to use</param>
        /// <param name="baseEventType">base type</param>
        /// <param name="deltaTypes">delta types</param>
        /// <param name="deltaAliases">aliases of delta types</param>
        /// <param name="keyPropertyNames">names of key properties</param>
        /// <param name="changesetPropertyNames">names of properties that change</param>
        /// <param name="baseEventOnlyPropertyNames">properties only available on the base event</param>
        /// <param name="deltaTypesAddProperties">bool to indicate delta types add additional properties.</param>
        /// <param name="changesetPropertyDeltaContributed">flag for each property indicating whether its contributed only by a delta event</param>
        public RevisionSpec(PropertyRevision propertyRevision,
                            EventType baseEventType,
                            EventType[] deltaTypes,
                            String[] deltaAliases,
                            String[] keyPropertyNames,
                            String[] changesetPropertyNames,
                            String[] baseEventOnlyPropertyNames,
                            bool deltaTypesAddProperties,
                            bool[] changesetPropertyDeltaContributed)
        {
            this.propertyRevision = propertyRevision;
            this.baseEventType = baseEventType;
            this.deltaTypes = deltaTypes;
            this.deltaAliases = deltaAliases;
            this.keyPropertyNames = keyPropertyNames;
            this.changesetPropertyNames = changesetPropertyNames;
            this.baseEventOnlyPropertyNames = baseEventOnlyPropertyNames;
            this.deltaTypesAddProperties = deltaTypesAddProperties;
            this.changesetPropertyDeltaContributed = changesetPropertyDeltaContributed;
        }

        /// <summary>Flag for each changeset property to indicate if only the delta contributes the property. </summary>
        /// <returns>flag per property</returns>
        public bool[] ChangesetPropertyDeltaContributed
        {
            get { return changesetPropertyDeltaContributed; }
        }

        /// <summary>Returns the stratgegy for revisioning. </summary>
        /// <returns>enum</returns>
        public PropertyRevision PropertyRevision
        {
            get { return propertyRevision; }
        }

        /// <summary>Returns the base event type. </summary>
        /// <returns>base type</returns>
        public EventType BaseEventType
        {
            get { return baseEventType; }
        }

        /// <summary>Returns the delta event types. </summary>
        /// <returns>types</returns>
        public EventType[] DeltaTypes
        {
            get { return deltaTypes; }
        }

        /// <summary>Returns aliases for delta events. </summary>
        /// <returns>event type alias names for delta events</returns>
        public string[] DeltaAliases
        {
            get { return deltaAliases; }
        }

        /// <summary>Returns property names for key properties. </summary>
        /// <returns>property names</returns>
        public string[] KeyPropertyNames
        {
            get { return keyPropertyNames; }
        }

        /// <summary>Returns property names of properties that change by deltas </summary>
        /// <returns>prop names</returns>
        public string[] ChangesetPropertyNames
        {
            get { return changesetPropertyNames; }
        }

        /// <summary>Returns the properies only found on the base event. </summary>
        /// <returns>base props</returns>
        public string[] BaseEventOnlyPropertyNames
        {
            get { return baseEventOnlyPropertyNames; }
        }

        /// <summary>Returns true if delta types add properties. </summary>
        /// <returns>flag indicating if delta event types add properties</returns>
        public bool IsDeltaTypesAddProperties
        {
            get { return deltaTypesAddProperties; }
        }
    }
}
