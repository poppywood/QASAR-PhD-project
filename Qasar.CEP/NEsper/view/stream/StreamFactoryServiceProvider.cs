///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using net.esper.events;

namespace net.esper.view.stream
{
	/// <summary>
	/// Static factory for implementations of the StreamFactoryService interface.
	/// </summary>
	public sealed class StreamFactoryServiceProvider
	{
        /// <summary>Creates an implementation of the StreamFactoryService interface.</summary>
        /// <param name="isReuseViews">
        /// indicator on whether stream and view resources are to be reused between statements
        /// </param>
        /// <returns>implementation</returns>
        public static StreamFactoryService NewService(bool isReuseViews)
        {
            return new StreamFactorySvcImpl(isReuseViews);
        }
	}
} // End of namespace
