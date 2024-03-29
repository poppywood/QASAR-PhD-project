// ************************************************************************************
// Copyright (C) 2006 Thomas Bernhardt. All rights reserved.                          *
// http://esper.codehaus.org                                                          *
// ---------------------------------------------------------------------------------- *
// The software in this package is published under the terms of the GPL license       *
// a copy of which has been included with this distribution in the license.txt file.  *
// ************************************************************************************

using System;

using net.esper.events;

namespace net.esper.client
{
	/// <summary>
	/// Statement interface that provides methods to start, stop and destroy a statement as well as
	/// get statement information such as statement name, expression text and current state.
	/// <para>
	/// Statements have 3 states: STARTED, STOPPED and DESTROYED.
    /// </para>
	/// <para>
	/// In started state, statements are actively evaluating event streams according to the statement expression. Started
	/// statements can be stopped and destroyed.
    /// </para>
	/// <para>
	/// In stopped state, statements are inactive. Stopped statements can either be started, in which case
	/// they begin to actively evaluate event streams, or destroyed.
    /// </para>
	/// <para>
	/// Destroyed statements have relinguished all statement resources and cannot be started or stopped.
    /// </para>
	/// </summary>

	public interface EPStatement : EPListenable, EPIterable
    {
        /// <summary> Returns the statement name.</summary>
        /// <returns> statement name</returns>
        String Name { get ; }

        /// <summary> Returns the underlying expression text or XML.</summary>
        /// <returns> expression text</returns>
        String Text { get ; }

		/// <summary>Gets the statement's current state</summary>
		EPStatementState State { get ; }

        /// <summary> Start the statement.</summary>
        void Start();

        /// <summary> Stop the statement.</summary>
        void Stop();

		/// <summary>Destroy the statement releasing all statement resources.
		/// <p>A destroyed statement cannot be started again.</p>
		/// </summary>
		void Destroy();
    }
}
