///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml;

using com.espertech.esper.client;
using com.espertech.esper.core;
using com.espertech.esper.events.xml;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Event sender for XML DOM-backed events.
    /// <para>
    /// Allows sending only event objects of type Node or Document, does check the root name of the
    /// XML document which must match the event type root name as configured. Any other event object
    /// generates an error.
    /// </para>
    /// </summary>
    public class EventSenderXMLDOM
    {
        /// <summary>
        /// Creates the specified runtime event sender.
        /// </summary>
        /// <param name="runtimeEventSender">The runtime event sender.</param>
        /// <param name="baseXMLEventType">Type of the base XML event.</param>
        /// <returns></returns>
        public static EventSender Create(EPRuntimeEventSender runtimeEventSender, BaseXMLEventType baseXMLEventType)
        {
            return delegate(Object node) {
                       XmlNode namedNode;
                       if (node is XmlDocument) {
                           namedNode = ((XmlDocument) node).DocumentElement;
                       } else if (node is XmlElement) {
                           namedNode = (XmlElement) node;
                       } else {
                           throw new EPException("Unexpected event object type '" + node.GetType().FullName +
                                                 "' encountered, please supply a org.w3c.dom.Document or Element node");
                       }

                       String rootElementName = namedNode.LocalName;
                       if (rootElementName == null) {
                           rootElementName = namedNode.Name;
                       }

                       if (rootElementName != baseXMLEventType.RootElementName) {
                           throw new EPException("Unexpected root element name '" + rootElementName +
                                                 "' encountered, expected a root element name of '" +
                                                 baseXMLEventType.RootElementName + "'");
                       }

                       EventBean _event = new XMLEventBean(namedNode, baseXMLEventType);
                       runtimeEventSender.ProcessWrappedEvent(_event);
                   };
        }
    }
}
