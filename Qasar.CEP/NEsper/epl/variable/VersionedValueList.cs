///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

using com.espertech.esper.compat;
using com.espertech.esper.util;
using log4net;

namespace com.espertech.esper.epl.variable
{
    /// <summary>
    /// A self-cleaning list of versioned-values.
    /// <para/>
    /// The current and prior version are held for lock-less read access in a transient variable.
    /// <para/>
    /// The list relies on transient as well as a read-lock to guard against concurrent modification. However a read lock is only
    /// taken when a list of old versions must be updated.
    /// <para/>
    /// When a high watermark is reached, the list on write access removes old versions up to the
    /// number of milliseconds compared to current write timestamp.
    /// <para/>
    /// If an older version is requested then held by the list, the list can either throw an exception
    /// or return the current value.
    /// </summary>
	public class VersionedValueList<T> 
        where T : class
	{
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    // Variables name and read lock; read lock used when older version then the prior version is requested
	    private readonly String name;
	    private readonly ILockable readLock;
	    private readonly int highWatermark;    // used for removing older versions
	    private readonly bool errorWhenNotFound;
	    private readonly long millisecondLifetimeOldVersions;

	    // Hold the current and prior version for no-lock reading
        [NonSerialized]
	    private CurrentValue<T> currentAndPriorValue;

	    // Holds the older versions
	    private readonly List<VersionedValue<T>> olderVersions;

	    /// <summary>Ctor.</summary>
	    /// <param name="name">variable name</param>
	    /// <param name="initialVersion">first version number</param>
	    /// <param name="initialValue">first value</param>
	    /// <param name="timestamp">timestamp of first version</param>
	    /// <param name="millisecondLifetimeOldVersions">
	    /// number of milliseconds after which older versions get expired and removed
	    /// </param>
	    /// <param name="readLock">for coordinating update to old versions</param>
	    /// <param name="highWatermark">
	    /// when the number of old versions reached high watermark, the list inspects size on every write
	    /// </param>
	    /// <param name="errorWhenNotFound">
	    /// true if an exception should be throw if the requested version cannot be found,
	    /// or false if the engine should log a warning
	    /// </param>
        public VersionedValueList(String name, int initialVersion, T initialValue, long timestamp, long millisecondLifetimeOldVersions, ILockable readLock, int highWatermark, bool errorWhenNotFound)
	    {
	        this.name = name;
	        this.readLock = readLock;
	        this.highWatermark = highWatermark;
	        this.olderVersions = new List<VersionedValue<T>>();
	        this.errorWhenNotFound = errorWhenNotFound;
	        this.millisecondLifetimeOldVersions = millisecondLifetimeOldVersions;

	        currentAndPriorValue = new CurrentValue<T>(new VersionedValue<T>(initialVersion, initialValue, timestamp),
	                                                   new VersionedValue<T>(-1, null, timestamp));
	    }

	    /// <summary>Returns the name of the value stored.</summary>
	    /// <returns>value name</returns>
	    public String Name
	    {
            get { return name; }
	    }

        /// <summary>
        /// Retrieve a value for the given version or older then then given version.
        /// <para/>
        /// The implementaton only locks the read lock if an older version the the prior version is requested.
        /// </summary>
        /// <param name="versionAndOlder">the version we are looking for</param>
        /// <returns>
        /// value for the version or the next older version, ignoring newer versions
        /// </returns>
	    public T GetVersion(int versionAndOlder)
	    {
            if (ExecutionPathDebugLog.isDebugEnabled && log.IsDebugEnabled)
	        {
	            log.Debug(".GetVersion Thread " + Thread.CurrentThread.ManagedThreadId + " for '" + name + "' retrieving version " + versionAndOlder + " or older");
	        }

	        T resultValue = null;
	        CurrentValue<T> current = currentAndPriorValue;

	        if (current.CurrentVersion.Version <= versionAndOlder)
	        {
	            resultValue = current.CurrentVersion.Value;
	        }
	        else if ((current.PriorVersion.Version != -1) &&
	            (current.PriorVersion.Version <= versionAndOlder))
	        {
	            resultValue = current.PriorVersion.Value;
	        }
	        else
	        {
                using (readLock.Acquire())
                {
                    current = currentAndPriorValue;

                    if (current.CurrentVersion.Version <= versionAndOlder)
                    {
                        resultValue = current.CurrentVersion.Value;
                    }
                    else if ((current.PriorVersion.Version != -1) &&
                             (current.PriorVersion.Version <= versionAndOlder))
                    {
                        resultValue = current.PriorVersion.Value;
                    }
                    else
                    {
                        bool found = false;
                        for (int i = olderVersions.Count - 1; i >= 0; i--)
                        {
                            VersionedValue<T> entry = olderVersions[i];
                            if (entry.Version <= versionAndOlder)
                            {
                                resultValue = entry.Value;
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            int currentVersion = current.CurrentVersion.Version;
                            int priorVersion = current.PriorVersion.Version;
                            int? oldestVersion = null;
                            if (olderVersions.Count > 0)
                            {
                                oldestVersion = olderVersions[0].Version;
                            }

                            T oldestValue = (olderVersions.Count > 0) ? olderVersions[0].Value : null;

                            String text = "Variables value for version '" + versionAndOlder +
                                          "' and older could not be found" +
                                          " (currentVersion=" + currentVersion + " priorVersion=" + priorVersion +
                                          " oldestVersion=" + oldestVersion + " numOldVersions=" + olderVersions.Count +
                                          " oldestValue=" + oldestValue + ")";
                            if (errorWhenNotFound)
                            {
                                throw new IllegalStateException(text);
                            }
                            log.Warn(text);
                            return current.CurrentVersion.Value;
                        }
                    }
                }
	        }

	        if (ExecutionPathDebugLog.isDebugEnabled && log.IsDebugEnabled)
	        {
	            log.Debug(".getVersion Thread " + Thread.CurrentThread.ManagedThreadId +  " for '" + name + " version " + versionAndOlder + " or older result is " + resultValue);
	        }

	        return resultValue;
	    }

	    /// <summary>
	    /// Add a value and version to the list, returning the prior value of the variable.
	    /// </summary>
	    /// <param name="version">for the value to add</param>
	    /// <param name="value">to add</param>
	    /// <param name="timestamp">the time associated with the version</param>
	    /// <returns>prior value</returns>
	    public Object AddValue(int version, T value, long timestamp)
	    {
            if (ExecutionPathDebugLog.isDebugEnabled && log.IsDebugEnabled)
	        {
	            log.Debug(".addValue Thread " + Thread.CurrentThread.ManagedThreadId + " for '" + name + "' adding version " + version + " at value " + value);
	        }

	        // push to prior if not already used
	        if (currentAndPriorValue.PriorVersion.Version == -1)
	        {
	            currentAndPriorValue = new CurrentValue<T>(new VersionedValue<T>(version, value, timestamp),
	              currentAndPriorValue.CurrentVersion);
	            return currentAndPriorValue.PriorVersion.Value;
	        }

	        // add to list
	        VersionedValue<T> priorVersion = currentAndPriorValue.PriorVersion;
	        olderVersions.Add(priorVersion);

	        // check watermarks
	        if (olderVersions.Count >= highWatermark)
	        {
	            long expireBefore = timestamp - millisecondLifetimeOldVersions;
	            while(olderVersions.Count > 0)
	            {
	                VersionedValue<T> oldestVersion = olderVersions[0];
	                if (oldestVersion.Timestamp <= expireBefore)
	                {
	                    olderVersions.RemoveAt(0);
	                }
	                else
	                {
	                    break;
	                }
	            }
	        }

	        currentAndPriorValue = new CurrentValue<T>(new VersionedValue<T>(version, value, timestamp),
	                                                   currentAndPriorValue.CurrentVersion);
	        return currentAndPriorValue.PriorVersion.Value;
	    }

	    /// <summary>Returns the current and prior version.</summary>
	    /// <returns>value</returns>
	    public CurrentValue<T> CurrentAndPriorValue
	    {
            get { return currentAndPriorValue; }
	    }

	    /// <summary>Returns the list of old versions, for testing purposes.</summary>
	    /// <returns>list of versions older then current and prior version</returns>
        public List<VersionedValue<T>> OlderVersions
	    {
            get { return olderVersions; }
	    }

	    public override String ToString()
	    {
	        StringBuilder buffer = new StringBuilder();
	        buffer.Append("Variable '").Append(name).Append("' ");
	        buffer.Append(" current=").Append(currentAndPriorValue.CurrentVersion.ToString());
	        buffer.Append(" prior=").Append(currentAndPriorValue.CurrentVersion.ToString());

	        int count = 0;
	        foreach (VersionedValue<T> old in olderVersions)
	        {
	            buffer.Append(" Old(").Append(count).Append(")=").Append(old.ToString()).Append("\n");
	            count++;
	        }
	        return buffer.ToString();
	    }
	}
} // End of namespace
