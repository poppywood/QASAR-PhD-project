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
using com.espertech.esper.core;
using com.espertech.esper.plugin;

using log4net;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Descriptor for URI-based event sender for plug-in event representations.
    /// </summary>
    public class EventSenderURIDesc
    {
        private readonly PlugInEventBeanFactory beanFactory;
        private readonly Uri resolutionURI;
        private readonly Uri representationURI;

        /// <summary>Ctor. </summary>
        /// <param name="beanFactory">factory for events</param>
        /// <param name="resolutionURI">URI use for resolution</param>
        /// <param name="representationURI">URI of event representation</param>
        public EventSenderURIDesc(PlugInEventBeanFactory beanFactory, Uri resolutionURI, Uri representationURI)
        {
            this.beanFactory = beanFactory;
            this.resolutionURI = resolutionURI;
            this.representationURI = representationURI;
        }

        /// <summary>URI used for resolution. </summary>
        /// <returns>resolution URI</returns>
        public Uri ResolutionURI
        {
            get { return resolutionURI; }
        }

        /// <summary>URI of event representation. </summary>
        /// <returns>URI</returns>
        public Uri RepresentationURI
        {
            get { return representationURI; }
        }

        /// <summary>Event wrapper for event objects. </summary>
        /// <returns>factory for events</returns>
        public PlugInEventBeanFactory BeanFactory
        {
            get { return beanFactory; }
        }
    }
}
