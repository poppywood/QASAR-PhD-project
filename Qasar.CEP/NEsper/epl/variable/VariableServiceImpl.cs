///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using com.espertech.esper.client;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.core;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.variable
{
    /// <summary>
    /// Variables service for reading and writing variables, and for setting a version number for the current thread to
    /// consider variables for.
    /// <para>
    /// Consider a statement as follows: select * from MyEvent as A where A.val > var1 and A.val2 > var1 and A.val3 > var2
    /// </para>
    /// <para>
    /// Upon statement execution we need to guarantee that the same atomic value for all variables is applied for all
    /// variable reads (by expressions typically) within the statement.
    /// </para>
    /// <para>
    /// Designed to support:
    /// <ol>
    /// <li>lock-less read of the current and prior version, locked reads for older versions</li>
    /// <li>atomicity by keeping multiple versions for each variable and a threadlocal that receives the current version each call</li>
    /// <li>one write lock for all variables (required to coordinate with single global version number),
    /// however writes are very fast (entry to collection plus increment an int) and therefore blocking should not be an issue</li>
    /// </ol>
    /// </para>
    /// <para>
    /// As an alternative to a version-based design, a read-lock for the variable space could also be used, with the following
    /// disadvantages: The write lock may just not be granted unless fair locks are used which are more expensive; And
    /// a read-lock is more expensive to acquire for multiple CPUs; A thread-local is still need to deal with
    /// &quot;set var1=3, var2=var1+1&quot; assignments where the new uncommitted value must be visible in the local evaluation.
    /// </para>
    /// <para>
    /// Every new write to a variable creates a new version. Thus when reading variables, readers can ignore newer versions
    /// and a read lock is not required in most circumstances.
    /// </para>
    /// <para>
    /// This algorithm works as follows:
    /// </para>
    /// <para>
    /// A thread processing an event into the engine via SendEvent() calls the &quot;setLocalVersion&quot; method once
    /// before processing a statement that has variables.
    /// This places into a threadlocal variable the current version number, say version 570.
    /// </para>
    /// <para>
    /// A statement that reads a variable has an <see cref="com.espertech.esper.epl.expression.ExprVariableNode"/> that has a <see cref="com.espertech.esper.epl.variable.VariableReader"/> handle
    /// obtained during validation (example).
    /// </para>
    /// <para>
    /// The <see cref="com.espertech.esper.epl.variable.VariableReader"/> takes the version from the threadlocal (570) and compares the version number with the
    /// version numbers held for the variable.
    /// If the current version is same or lower (520, as old or older) then the threadlocal version,
    /// then use the current value.
    /// If the current version is higher (571, newer) then the threadlocal version, then go to the prior value.
    /// Use the prior value until a version is found that as old or older then the threadlocal version.
    /// </para>
    /// <para>
    /// If no version can be found that is old enough, output a warning and return the newest version.
    /// This should not happen, unless a thread is executing for very long within a single statement such that
    /// lifetime-old-version time speriod passed before the thread asks for variable values.
    /// </para>
    /// <para>
    /// As version numbers are counted up they may reach a boundary. Any write transaction after the boundary
    /// is reached performs a roll-over. In a roll-over, all variables version lists are
    /// newly created and any existing threads that read versions go against a (old) high-collection,
    /// while new threads reading the reset version go against a new low-collection.
    /// </para>
    /// <para>
    /// The class also allows an optional state handler to be plugged in to handle persistence for variable state.
    /// The state handler gets invoked when a variable changes value, and when a variable gets created
    /// to obtain the current value from persistence, if any.
    /// </para>
    /// </summary>
	public class VariableServiceImpl : VariableService
	{
	    /// <summary>
	    /// Sets the boundary above which a reader considers the high-version list of variable values.
	    /// For use in roll-over when the current version number overflows the ROLLOVER_WRITER_BOUNDARY.
	    /// </summary>
	    public const int ROLLOVER_READER_BOUNDARY = Int32.MaxValue - 100000;

	    /// <summary>
	    /// Sets the boundary above which a write transaction rolls over all variable's
	    /// version lists.
	    /// </summary>
	    public const int ROLLOVER_WRITER_BOUNDARY = ROLLOVER_READER_BOUNDARY + 10000;

	    /// <summary>
	    /// Applicable for each variable if more then the number of versions accumulated, check
	    /// timestamps to determine if a version can be expired.
	    /// </summary>
	    public const int HIGH_WATERMARK_VERSIONS = 50;

	    // Keep the variable list
	    private readonly Map<String, VariableReader> variables;

	    // Each variable has an index number, a current version and a list of values
	    private readonly List<VersionedValueList<Object>> variableVersions;

	    // Each variable may have a single callback to invoke when the variable changes
	    private readonly List<VariableChangeCallback> changeCallbacks;

	    // Write lock taken on write of any variable; and on read of older versions
	    private readonly FastReaderWriterLock readWriteLock;

	    // Thread-local for the visible version per thread
        private readonly VariableVersionThreadLocal versionThreadLocal;

	    // Number of milliseconds that old versions of a variable are allowed to live
	    private readonly long millisecondLifetimeOldVersions;
	    private readonly TimeProvider timeProvider;
	    private readonly VariableStateHandler optionalStateHandler;

        [NonSerialized]
	    private int currentVersionNumber;
	    private int currentVariableNumber;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="millisecondLifetimeOldVersions">number of milliseconds a version may hang around before expiry</param>
        /// <param name="timeProvider">provides the current time</param>
        /// <param name="optionalStateHandler">a optional plug-in that may store variable state and retrieve state upon creation</param>
	    public VariableServiceImpl(long millisecondLifetimeOldVersions,
                                   TimeProvider timeProvider, 
                                   VariableStateHandler optionalStateHandler)
	        : this(0, millisecondLifetimeOldVersions, timeProvider, optionalStateHandler)
	    {
	    }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="startVersion">the first version number to start from</param>
        /// <param name="millisecondLifetimeOldVersions">number of milliseconds a version may hang around before expiry</param>
        /// <param name="timeProvider">provides the current time</param>
        /// <param name="optionalStateHandler">a optional plug-in that may store variable state and retrieve state upon creation</param>
	    public VariableServiceImpl(int startVersion,
                                   long millisecondLifetimeOldVersions, 
                                   TimeProvider timeProvider, 
                                   VariableStateHandler optionalStateHandler)
	    {
            this.versionThreadLocal = new VariableVersionThreadLocal();
	        this.millisecondLifetimeOldVersions = millisecondLifetimeOldVersions;
	        this.timeProvider = timeProvider;
	        this.optionalStateHandler = optionalStateHandler;
	        this.variables = new HashMap<String, VariableReader>();
	        this.variableVersions = new List<VersionedValueList<Object>>();
	        this.readWriteLock = new FastReaderWriterLock();
	        this.changeCallbacks = new List<VariableChangeCallback>();
	        currentVersionNumber = startVersion;
	    }

	    public void SetLocalVersion()
	    {
	        versionThreadLocal.CurrentThread.Version = currentVersionNumber;
	    }

	    public void RegisterCallback(int variableNumber, VariableChangeCallback variableChangeCallback)
	    {
	        changeCallbacks[variableNumber] = variableChangeCallback;
	    }

	    public void CreateNewVariable(String variableName, Type type, Object value, StatementExtensionSvcContext extensionServicesContext)
	    {
            lock (this)
            {
                // check type
                Type variableType = TypeHelper.GetBoxedType(type);

                if (!TypeHelper.IsBuiltinDataType(variableType))
                {
                    throw new VariableTypeException("Invalid variable type for variable '" + variableName
                                                    + "' as type '" + variableType.FullName +
                                                    "', only primitive, boxed or String types are allowed");
                }

                // check coercion
                Object coercedValue = value;

                // allow string assignments to non-string variables
                if ((coercedValue != null) && (coercedValue is String))
                {
                    try
                    {
                        coercedValue = TypeHelper.Parse(type, (String) coercedValue);
                    }
                    catch (Exception ex)
                    {
                        throw new VariableTypeException("Variable '" + variableName
                                                        + "' of declared type '" + variableType.FullName +
                                                        "' cannot be initialized by value '" + coercedValue + "': " +
                                                        ex.GetType().FullName + ": " + ex.Message);
                    }
                }

                // At this point coercedValue is probably the value that was passed
                // into us.  We want to make sure the coercedValue is compatible with
                // the variable type.  The VariableType is probably boxed, while the
                // coercedValue may no be.  First thing to do is to normalize the
                // conversation.

                if (coercedValue != null)
                {
                    Type coercedValueType = TypeHelper.GetBoxedType(coercedValue.GetType());
                    if (coercedValueType != variableType)
                    {
                        // if the declared type is not numeric or the init value is not numeric, fail
                        if ((!TypeHelper.IsNumeric(variableType)) ||
                            (!TypeHelper.IsNumericValue(coercedValue)))
                        {
                            throw new VariableTypeException("Variable '" + variableName
                                                            + "' of declared type '" + variableType.FullName +
                                                            "' cannot be initialized by a value of type '" +
                                                            TypeHelper.GetBoxedType(coercedValue.GetType()).FullName + "'");
                        }

                        if (!TypeHelper.CanCoerce(coercedValue.GetType(), variableType))
                        {
                            throw new VariableTypeException("Variable '" + variableName
                                                            + "' of declared type '" + variableType.FullName +
                                                            "' cannot be initialized by a value of type '" +
                                                            TypeHelper.GetBoxedType(coercedValue.GetType()).FullName + "'");
                        }

                        // coerce
                        coercedValue = TypeHelper.CoerceBoxed(coercedValue, variableType);
                    }
                }

                // check if it exists
                VariableReader reader = variables.Get(variableName);
                if (reader != null)
                {
                    throw new VariableExistsException("Variable by name '" + variableName + "' has already been created");
                }

                long timestamp = timeProvider.Time;

                // Check current state - see if the variable exists in the state handler
                if (optionalStateHandler != null)
                {
                    Pair<Boolean, Object> priorValue =
                        optionalStateHandler.GetHasState(variableName, currentVariableNumber, variableType,
                                                         extensionServicesContext);
                    if (priorValue.First)
                    {
                        coercedValue = priorValue.Second;
                    }
                }

                // create new holder for versions
                VersionedValueList<Object> valuePerVersion =
                    new VersionedValueList<Object>(variableName, currentVersionNumber, coercedValue, timestamp,
                                                   millisecondLifetimeOldVersions, readWriteLock.ReadLock,
                                                   HIGH_WATERMARK_VERSIONS, false);

                // add entries matching in index the variable number
                variableVersions.Add(valuePerVersion);
                changeCallbacks.Add(null);

                // create reader
                reader =
                    new VariableReader(versionThreadLocal, variableType, variableName, currentVariableNumber,
                                       valuePerVersion);
                variables.Put(variableName, reader);

                currentVariableNumber++;
            }
	    }

	    public VariableReader GetReader(String variableName)
	    {
	        return variables.Get(variableName);
	    }

	    public void Write(int variableNumber, Object newValue)
	    {
	        VariableVersionThreadEntry entry = versionThreadLocal.CurrentThread;
	        if (entry.Uncommitted == null)
	        {
	            entry.Uncommitted = new HashMap<int, Object>();
	        }
	        entry.Uncommitted.Put(variableNumber, newValue);
	    }

	    public FastReaderWriterLock ReadWriteLock
	    {
	        get { return readWriteLock; }
	    }

	    public void Commit()
	    {
	        VariableVersionThreadEntry entry = versionThreadLocal.CurrentThread;
	        if (entry.Uncommitted == null)
	        {
	            return;
	        }

	        // get new version for adding the new values (1 or many new values)
	        int newVersion = currentVersionNumber + 1;

	        if (currentVersionNumber == ROLLOVER_READER_BOUNDARY)
	        {
	            // Roll over to new collections;
	            // This honors existing threads that will now use the "high" collection in the reader for high version requests
	            // and low collection (new and updated) for low version requests
	            RollOver();
	            newVersion = 2;
	        }
	        long timestamp = timeProvider.Time;

	        // apply all uncommitted changes
	        foreach (KeyValuePair<int, Object> uncommittedEntry in entry.Uncommitted)
	        {
	            VersionedValueList<Object> versions = variableVersions[uncommittedEntry.Key];

	            // add new value as a new version
	            Object oldValue = versions.AddValue(newVersion, uncommittedEntry.Value, timestamp);

	            // make a callback that the value changed
	            VariableChangeCallback callback = changeCallbacks[uncommittedEntry.Key];
	            if (callback != null)
	            {
	                callback.Update(uncommittedEntry.Value, oldValue);
	            }

	            // Check current state - see if the variable exists in the state handler
	            if (optionalStateHandler != null)
	            {
	                String name = versions.Name;
	                optionalStateHandler.SetState(name, uncommittedEntry.Key, uncommittedEntry.Value);
	            }
	        }

	        // this makes the new values visible to other threads (not this thread unless set-version called again)
	        currentVersionNumber = newVersion;
	        entry.Uncommitted = null;    // clean out uncommitted variables
	    }

	    public void Rollback()
	    {
	        VariableVersionThreadEntry entry = versionThreadLocal.CurrentThread;
	        entry.Uncommitted = null;
	    }

	    /// <summary>Rollover includes creating a new</summary>
	    private void RollOver()
	    {
            foreach (KeyValuePair<String, VariableReader> entry in variables)
	        {
	            int variableNum = entry.Value.VariableNumber;
	            String name = entry.Key;
	            long timestamp = timeProvider.Time;

	            // Construct a new collection, forgetting the history
	            VersionedValueList<Object> versionsOld = variableVersions[variableNum];
	            Object currentValue = versionsOld.CurrentAndPriorValue.CurrentVersion.Value;
	            VersionedValueList<Object> versionsNew = new VersionedValueList<Object>(
                    name, 1, currentValue, timestamp,
                    millisecondLifetimeOldVersions, 
                    readWriteLock.ReadLock,
                    HIGH_WATERMARK_VERSIONS, 
                    false);

	            // Tell the reader to use the high collection for old requests
	            entry.Value.VersionsHigh = versionsOld;
	            entry.Value.VersionsLow = versionsNew;

	            // Save new collection instead
	            variableVersions[variableNum] = versionsNew;
	        }
	    }

        public void CheckAndWrite(int variableNumber, Object newValue)
        {
            if (newValue == null) {
                Write(variableNumber, newValue);
                return;
            }

            Type valueType = newValue.GetType();
            String variableName = variableVersions[variableNumber].Name;
            Type variableType = variables.Get(variableName).VariableType;

            if (Equals(valueType,variableType)) {
                Write(variableNumber, newValue);
                return;
            }

            if ((!TypeHelper.IsNumeric(variableType)) ||
                (!TypeHelper.IsNumeric(valueType)))
            {
                throw new VariableValueException("Variable '" + variableName
                                                 + "' of declared type '" + variableType.FullName +
                                                 "' cannot be assigned a value of type '" + valueType.FullName + "'");
            }

            // determine if the expression type can be assigned
            if (!(TypeHelper.CanCoerce(valueType, variableType)))
            {
                throw new VariableValueException("Variable '" + variableName
                                                 + "' of declared type '" + variableType.FullName +
                                                 "' cannot be assigned a value of type '" + valueType.FullName + "'");
            }

            Object valueCoerced = TypeHelper.CoerceBoxed(newValue, variableType);
            Write(variableNumber, valueCoerced);
        }

        public override String ToString()
	    {
	        StringWriter writer = new StringWriter();
            foreach (KeyValuePair<String, VariableReader> entry in variables)
	        {
	            int variableNum = entry.Value.VariableNumber;
	            VersionedValueList<Object> list = variableVersions[variableNum];
	            writer.Write("Variable '" + entry.Key + "' : " + list.ToString() + "\n");
	        }
	        return writer.ToString();
	    }

        public Map<String, VariableReader> Variables
        {
            get
            {
                Map<String, VariableReader> variables = new HashMap<String, VariableReader>();
                variables.PutAll(this.variables);
                return variables;
            }
        }
	}
} // End of namespace
