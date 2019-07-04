using System;

using net.esper.compat;
using net.esper.eql.spec;
using net.esper.pattern.guard;
using net.esper.util;

using org.apache.commons.logging;

namespace net.esper.pattern
{
	/// <summary>
	/// This class represents a guard in the evaluation tree representing an event expressions.
	/// </summary>
	public sealed class EvalGuardNode:EvalNode
	{
	    private readonly PatternGuardSpec patternGuardSpec;
		private GuardFactory guardFactory;
		
		/// <summary>
		/// Gets the guard factory.
		/// </summary>
		
	    public GuardFactory GuardFactory
	    {
	        get { return guardFactory; }
            set { guardFactory = value; }
	    }

        /// <summary>
        /// Gets the guard object specification to use for instantiating the guard factory and guard.
        /// </summary>
        public PatternGuardSpec PatternGuardSpec
	    {
	        get { return patternGuardSpec; }
	    }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvalGuardNode"/> class.
        /// </summary>
        /// <param name="patternGuardSpec">factory for guard construction.</param>
        public EvalGuardNode(PatternGuardSpec patternGuardSpec)
        {
            this.patternGuardSpec = patternGuardSpec;
        }

        /// <summary>
        /// Create the evaluation state node containing the truth value state for each operator in an
        /// event expression.
        /// </summary>
        /// <param name="parentNode">is the parent evaluator node that this node indicates a change in truth value to</param>
        /// <param name="beginState">is the container for events that makes up the Start state</param>
        /// <param name="context">is the handle to services required for evaluation</param>
        /// <param name="stateNodeId">is the new state object's identifier</param>
        /// <returns>
        /// state node containing the truth value state for the operator
        /// </returns>
		public override EvalStateNode NewState(Evaluator parentNode,
											   MatchedEventMap beginState,
											   PatternContext context,
											   Object stateNodeId)
		{
			if (ExecutionPathDebugLog.IsEnabled && log.IsDebugEnabled)
			{
				log.Debug(".newState");
			}
			
			if (ChildNodes.Count != 1)
			{
				throw new IllegalStateException("Expected number of child nodes incorrect, expected 1 child node, found " + ChildNodes.Count);
			}
			
			return context.PatternStateFactory.MakeGuardState(parentNode, this, beginState, context, stateNodeId);
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
		public override String ToString()
		{
			return ("EvalGuardNode guardFactory=" + guardFactory + "  children=" + this.ChildNodes.Count);
		}
		
		private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
}