///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using com.espertech.esper.util;

namespace com.espertech.esper.core
{
	/// <summary>
	/// Class exists once per statement and hold statement resource Lock(s).
	/// <para>
	/// Use by <see cref="EPRuntimeImpl"/> for determining callback-statement affinity and locking of statement
	/// resources.
	/// </para>
	/// </summary>
	public class EPStatementHandle : MetaDefItem
	{
	    private readonly String statementId;
	    private ManagedLock statementLock;
	    private readonly int hashCode;
	    private EPStatementDispatch optionalDispatchable;

        // handles self-join (ie. statement where from-clause lists the same event type or a super-type more then once)
	    // such that the internal dispatching must occur after both matches are processed
	    private bool canSelfJoin;

        private bool hasVariables;
        private InsertIntoLatchFactory insertIntoLatchFactory;

        /**
         * Ctor.
         * @param statementId is the statement id uniquely indentifying the handle
         * @param statementLock is the statement resource lock
         * @param expressionText is the expression
         * @param hasVariables indicator whether the statement uses variables
         */
        public EPStatementHandle(String statementId, ManagedLock statementLock, String expressionText, bool hasVariables)
        {
            this.statementId = statementId;
            this.statementLock = statementLock;
            this.hasVariables = hasVariables;
            hashCode = expressionText.GetHashCode() ^ statementLock.GetHashCode();
        }

	    /// <summary>
	    /// Set the statement's self-join flag to indicate the the statement may join to itself,
	    /// that is a single event may dispatch into multiple streams or patterns for the same statement,
	    /// requiring internal dispatch logic to not shortcut evaluation of all filters for the statement
	    /// within one lock, requiring the callback handle to be sorted.
	    /// </summary>

        public bool CanSelfJoin
	    {
			get { return this.canSelfJoin ; }
			set { this.canSelfJoin = value; }
	    }

	    /// <summary>
	    /// Returns true if the statement potentially self-joins amojng the events it processes.
	    /// </summary>
	    /// <returns>
	    /// true for self-joins possible, false for not possible (most statements)
	    /// </returns>
	    public bool IsCanSelfJoin
	    {
	        get { return canSelfJoin; }
	    }

        /// <summary>Returns statement resource lock.</summary>
	    /// <returns>lock</returns>
	    public ManagedLock StatementLock
	    {
	        get { return statementLock; }
            set { statementLock = value; }
	    }

        /// <summary>
        /// Gets or sets the factory for latches in insert-into guaranteed order of delivery.
        /// </summary>
        /// <value>The insert into latch factory.</value>
	    public InsertIntoLatchFactory InsertIntoLatchFactory
	    {
	        get { return insertIntoLatchFactory; }
	        set { insertIntoLatchFactory = value; }
	    }

        /// <summary>
        /// Gets a value indicating whether this instance has variables.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has variables; otherwise, <c>false</c>.
        /// </value>
        public bool HasVariables
	    {
	        get { return hasVariables; }
	    }

	    /// <summary>
	    /// Provides a callback for use when statement processing for filters and schedules is done,
	    /// for use by join statements that require an explicit indicator that all
	    /// joined streams results have been processed.
	    /// </summary>
	    public EPStatementDispatch OptionalDispatchable
	    {
			get { return this.optionalDispatchable ; }
	        set { this.optionalDispatchable = value; }
	    }

	    /// <summary>
	    /// Invoked by <see cref="com.espertech.esper.client.EPRuntime"/> to indicate that a statements's
	    /// filer and schedule processing is done, and now it's time to process join results.
	    /// </summary>
	    public void InternalDispatch()
	    {
	        if (optionalDispatchable != null)
	        {
	            optionalDispatchable.Execute();
	        }
	    }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(Object obj)
	    {
	        EPStatementHandle other = obj as EPStatementHandle;
			if (other == null)
	        {
	            return false;
	        }

	        if (other.statementId.Equals(this.statementId))
	        {
	            return true;
	        }
	        return false;
	    }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
	    public override int GetHashCode()
	    {
	        return hashCode;
	    }
	}
} // End of namespace
