///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.events;
using com.espertech.esper.view;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// Service associating handling vaue-added event types, such a revision event types and
    /// variant stream event types.
    /// <para/>
    /// Associates named windows and revision event types.
    /// </summary>
    public interface ValueAddEventService
    {
        /// <summary>
        /// Called at initialization time, verifies configurations provided.
        /// </summary>
        /// <param name="revisionTypes">is the revision types to add</param>
        /// <param name="variantStreams">is the variant streams to add</param>
        /// <param name="eventAdapterService">for obtaining event type information for each alias</param>
        void Init(Map<String, ConfigurationRevisionEventType> revisionTypes,
                  Map<String, ConfigurationVariantStream> variantStreams,
                  EventAdapterService eventAdapterService);

        /// <summary>
        /// Adds a new revision event types.
        /// </summary>
        /// <param name="alias">to add</param>
        /// <param name="config">the revision event type configuration</param>
        /// <param name="eventAdapterService">for obtaining event type information for each alias</param>
        void AddRevisionEventType(String alias,
                                  ConfigurationRevisionEventType config,
                                  EventAdapterService eventAdapterService);

        /// <summary>
        /// Adds a new variant stream.
        /// </summary>
        /// <param name="variantEventTypeAlias">the alias of the type</param>
        /// <param name="variantStreamConfig">the configs</param>
        /// <param name="eventAdapterService">for handling nested events</param>
        /// <throws>ConfigurationException if the configuration is invalid</throws>
        void AddVariantStream(String variantEventTypeAlias,
                              ConfigurationVariantStream variantStreamConfig,
                              EventAdapterService eventAdapterService);

        /// <summary>
        /// Upon named window creation, and during resolution of type specified as part of a
        /// named window create statement, returns looks up the revision event type alias provided and return the revision event type if found, or null if not found.
        /// </summary>
        /// <param name="alias">to look up</param>
        /// <returns>null if not found, of event type</returns>
        EventType GetValueAddUnderlyingType(String alias);

        /// <summary>
        /// Upon named window creation, create a unique revision event type that this window processes.
        /// </summary>
        /// <param name="namedWindowName">name of window</param>
        /// <param name="alias">alias to use</param>
        /// <param name="statementStopService">for handling stops</param>
        /// <param name="eventAdapterService">for event type info</param>
        /// <returns>revision event type</returns>
        EventType CreateRevisionType(String namedWindowName,
                                     String alias,
                                     StatementStopService statementStopService,
                                     EventAdapterService eventAdapterService);

        /// <summary>
        /// Upon named window creation, check if the alias used is a revision event type alias.
        /// </summary>
        /// <param name="alias">to check</param>
        /// <returns>
        /// true if revision event type, false if not
        /// </returns>
        bool IsRevisionTypeAlias(String alias);

        /// <summary>
        /// Gets a value-added event processor.
        /// </summary>
        /// <param name="alias">of the value-add events</param>
        /// <returns>processor</returns>
        ValueAddEventProcessor GetValueAddProcessor(String alias);
    }
}
