///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using net.esper.core;

namespace net.esper.view
{
	/// <summary>
	/// Interface for use by expression nodes to indicate view resource requirements
	/// allowing inspection and modification of view factories.
	/// </summary>
	public interface ViewCapability
	{
        /// <summary>
        /// Inspect view factories returning false to indicate that view factories do not meet
        /// view resource requirements, or true to indicate view capability and view factories can be compatible.
        /// </summary>
        /// <param name="streamNumber">is the number of the stream</param>
        /// <param name="viewFactories">is a list of view factories that originate the final views</param>
        /// <param name="statementContext">is the statement-level services</param>
        /// <returns>
        /// true to indicate inspection success, or false to indicate inspection failure
        /// </returns>
        bool Inspect(int streamNumber, IList<ViewFactory> viewFactories, StatementContext statementContext);
	}
} // End of namespace
