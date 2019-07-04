///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.plugin
{
    /// <summary>
    /// Context for use in <see cref="PlugInEventRepresentation"/> to provide information to help
    /// decide whether an event representation can handle the requested event type.
    /// </summary>
    public class PlugInEventTypeHandlerContext
    {
        private readonly Uri eventTypeResolutionURI;
        private readonly Object typeInitializer;
        private readonly String eventTypeAlias;

        /// <summary>Ctor. </summary>
        /// <param name="eventTypeResolutionURI">the URI specified for resolving the event type, may be a child URIof the event representation URI and may carry additional parameters </param>
        /// <param name="typeInitializer">optional configuration for the type, or null if none supplied</param>
        /// <param name="eventTypeAlias">the name of the event</param>
        public PlugInEventTypeHandlerContext(Uri eventTypeResolutionURI, Object typeInitializer, String eventTypeAlias)
        {
            this.eventTypeResolutionURI = eventTypeResolutionURI;
            this.typeInitializer = typeInitializer;
            this.eventTypeAlias = eventTypeAlias;
        }

        /// <summary>Returns the URI specified for resolving the event type, may be a child URI of the event representation URI and may carry additional parameters </summary>
        /// <returns>URI</returns>
        public Uri EventTypeResolutionURI
        {
            get { return eventTypeResolutionURI; }
        }

        /// <summary>Returns optional configuration for the type, or null if none supplied. An String XML document if the configuration was read from an XML file. </summary>
        /// <returns>configuration, or null if none supplied</returns>
        public object TypeInitializer
        {
            get { return typeInitializer; }
        }

        /// <summary>Returns the name assigned to the event type. </summary>
        /// <returns>alias</returns>
        public string EventTypeAlias
        {
            get { return eventTypeAlias; }
        }
    }
}
