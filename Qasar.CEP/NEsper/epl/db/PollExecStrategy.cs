using System;
using System.Collections.Generic;

using com.espertech.esper.events;

namespace com.espertech.esper.epl.db
{

    /// <summary>
    /// Interface for polling data from a data source such as a relational database.
    /// Lifecycle methods are for managing connection resources.
    /// </summary>
    public interface PollExecStrategy
    {
        /// <summary> Start the poll, called before any poll operation.</summary>
        void Start();

        /// <summary> Poll events using the keys provided.</summary>
        /// <param name="lookupValues">is keys for exeuting a query or such
        /// </param>
        /// <returns> a list of events for the keys
        /// </returns>

        IList<EventBean> Poll(Object[] lookupValues);

        /// <summary> Indicate we are done polling and can release resources.</summary>
        void Done();

        /// <summary> Indicate we are no going to use this object again.</summary>
        void Destroy();
    }
}