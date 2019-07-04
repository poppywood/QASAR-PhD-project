// ************************************************************************************
// Copyright (C) 2006 Thomas Bernhardt. All rights reserved.                          *
// http://esper.codehaus.org                                                          *
// ---------------------------------------------------------------------------------- *
// The software in this package is published under the terms of the GPL license       *
// a copy of which has been included with this distribution in the license.txt file.  *
// ************************************************************************************

using System;

namespace net.esper.client
{
    public interface EmittedListener
    {
        /// <summary>
        /// Called to indicate an event emitted from an EPRuntime.
        /// </summary>

        void Emitted(Object _event);
    }

	/// <summary>
    /// Called to indicate an event emitted from an EPRuntime.
    /// </summary>
	
    public delegate void EmittedEventHandler( Object _event ) ;

    public class ProxyEmittedListener : EmittedListener
    {
        private EmittedEventHandler @delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyEmittedListener"/> class.
        /// </summary>
        /// <param name="dg">The dg.</param>
        public ProxyEmittedListener( EmittedEventHandler dg )
        {
            @delegate = dg;
        }

        /// <summary>
        /// Called to indicate an event emitted from an EPRuntime.
        /// </summary>

        public void Emitted(Object _event)
        {
            @delegate.Invoke(_event);
        }
    }
}