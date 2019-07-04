///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.view;

namespace com.espertech.esper.core
{
	/// <summary>
	/// Statement SPI for statements operations for state transitions and internal management.
	/// </summary>
	public interface EPStatementSPI : EPStatement
	{
	    /// <summary>Returns the statement id.</summary>
	    /// <returns>statement id</returns>
	    String StatementId { get; }

        /// <summary>
        /// Gets or sets the current set of listeners for read-only operations.
        /// </summary>
        EPStatementListenerSet ListenerSet { get; set; }

	    /// <summary>Set statement state.</summary>
        void SetCurrentState(EPStatementState currentState, long timeLastStateChange);

	    /// <summary>Sets the parent view.</summary>
	    Viewable ParentView { set ; }
	}
} // End of namespace
