using System;
using System.Collections.Generic;

using net.esper.eql;
using net.esper.eql.core;
using net.esper.eql.join;
using net.esper.eql.spec;

using net.esper.collection;
using net.esper.compat;
using net.esper.events;
using net.esper.view;

using Log = org.apache.commons.logging.Log;
using LogFactory = org.apache.commons.logging.LogFactory;

namespace net.esper.eql.view
{
	/// <summary>
	/// Base output processing view that has the responsibility to serve up event type and
	/// statement iterator.
	/// &lt;p&gt;
	/// Implementation classes may enforce an output rate stabilizing or limiting policy.
	/// </summary>
	public abstract class OutputProcessView
		: ViewSupport
		, JoinSetIndicator
		, IEnumerable<EventBean>
	{
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    /// <summary>
	    /// Processes the parent views result set generating events for pushing out to child view.
	    /// </summary>
	    protected readonly ResultSetProcessor resultSetProcessor;

        private bool isJoin;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputProcessView"/> class.
        /// </summary>
        /// <param name="resultSetProcessor">processes the results posted by parent view or joins.</param>
        /// <param name="isJoin">Is true for join statements.</param>
        protected OutputProcessView(ResultSetProcessor resultSetProcessor, bool isJoin)
        {
            this.resultSetProcessor = resultSetProcessor;
            this.isJoin = isJoin;
        }

	    public override EventType EventType
	    {
			get
			{
		    	if(resultSetProcessor != null)
		    	{
		            EventType eventType = resultSetProcessor.ResultEventType;
		            if (eventType != null)
		            {
		                return eventType;
		            }
		            return parent.EventType;
		    	}
		    	else
		    	{
		    		return parent.EventType;
		    	}
		    }
		}

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
	    public override IEnumerator<EventBean> GetEnumerator()
	    {
            if (isJoin)
            {
                throw new UnsupportedOperationException("Joins statements do not allow iteration");
            }
            if (resultSetProcessor != null)
            {
                return resultSetProcessor.GetEnumerator(parent);
            }
            else
            {
                return parent.GetEnumerator();
            }
	    }

		abstract public void Process(Set<MultiKey<EventBean>> newEvents, Set<MultiKey<EventBean>> oldEvents);
    }
}
