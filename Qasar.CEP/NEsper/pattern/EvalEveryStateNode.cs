using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.util;

using org.apache.commons.logging;

namespace net.esper.pattern
{
    /// <summary>
    /// This class contains the state of an 'every' operator in the evaluation state tree.
    /// EVERY nodes work as a factory for new state subnodes. When a child node of an EVERY
    /// node calls the evaluateTrue method on the EVERY node, the EVERY node will call newState on its child
    /// node BEFORE it calls evaluateTrue on its parent node. It keeps a reference to the new child in
    /// its list. (BEFORE because the root node could call Quit on child nodes for stopping all
    /// listeners).
    /// </summary>

    sealed class EvalEveryStateSpawnEvaluator : Evaluator
    {
        public bool IsEvaluatedTrue
        {
            get { return isEvaluatedTrue; }
        }

        private bool isEvaluatedTrue;

        public void EvaluateTrue(MatchedEventMap matchEvent, EvalStateNode fromNode, bool isQuitted)
        {
            log.Warn("Event/request processing: Uncontrolled pattern matching of \"every\" operator - infinite loop when using EVERY operator on expression(s) containing a not operator");
            isEvaluatedTrue = true;
        }

        public void EvaluateFalse(EvalStateNode fromNode)
        {
            log.Warn("Event/request processing: Uncontrolled pattern matching of \"every\" operator - infinite loop when using EVERY operator on expression(s) containing a not operator");
            isEvaluatedTrue = true;
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    /// <summary> Contains the state collected by an "every" operator. The state includes handles to any sub-listeners
    /// Started by the operator.
    /// </summary>

    public sealed class EvalEveryStateNode : EvalStateNode, Evaluator
    {
        private readonly IList<EvalStateNode> spawnedNodes;
        private readonly MatchedEventMap beginState;
        private readonly PatternContext context;

	    /// <summary>Constructor.</summary>
	    /// <param name="parentNode">
	    /// is the parent evaluator to call to indicate truth value
	    /// </param>
	    /// <param name="beginState">contains the events that make up prior matches</param>
	    /// <param name="context">contains handles to services required</param>
	    /// <param name="everyNode">is the factory node associated to the state</param>
	    public EvalEveryStateNode(Evaluator parentNode,
	                              EvalEveryNode everyNode,
	                              MatchedEventMap beginState,
	                              PatternContext context)
			: base(everyNode, parentNode, null)
	    {
	        if (ExecutionPathDebugLog.IsEnabled && log.IsDebugEnabled)
	        {
	            log.Debug(".constructor");
	        }

	        this.spawnedNodes = new List<EvalStateNode>();
	        this.beginState = beginState.ShallowCopy();
	        this.context = context;

	        EvalStateNode child = FactoryNode.ChildNodes[0].NewState(this, beginState, context, null);
	        spawnedNodes.Add(child);
	    }

        /// <summary>
        /// Starts the event expression or an instance of it.
        /// Child classes are expected to initialize and Start any event listeners
        /// or schedule any time-based callbacks as needed.
        /// </summary>
        public override void Start()
        {
            if (ExecutionPathDebugLog.IsEnabled && log.IsDebugEnabled)
            {
                log.Debug(".Start Starting single child");
            }

            if (spawnedNodes.Count != 1)
            {
                throw new IllegalStateException("EVERY state node is expected to have single child state node");
            }

            // During the Start of the child we need to use the temporary evaluator to catch any event created during a Start.
            // Events created during the Start would likely come from the "not" operator.
            // Quit the new child again if
            EvalEveryStateSpawnEvaluator spawnEvaluator = new EvalEveryStateSpawnEvaluator();
            EvalStateNode child = spawnedNodes[0];
            child.ParentEvaluator = spawnEvaluator;
            child.Start();

            // If the spawned expression turned true already, just quit it
            if (spawnEvaluator.IsEvaluatedTrue)
            {
                child.Quit();
            }
            else
            {
                child.ParentEvaluator = this;
            }
        }

        /// <summary>
        /// Indicate a change in truth value to false.
        /// </summary>
        /// <param name="fromNode">is the node that indicates the change</param>
        public void EvaluateFalse(EvalStateNode fromNode)
        {
            if (ExecutionPathDebugLog.IsEnabled && log.IsDebugEnabled)
            {
                log.Debug(".evaluateFalse");
            }

            fromNode.Quit();
            spawnedNodes.Remove(fromNode);

            // Spawn all nodes below this EVERY node
            // During the start of a child we need to use the temporary evaluator to catch any event created during a start
            // Such events can be raised when the "not" operator is used.
            EvalNode child = FactoryNode.ChildNodes[0];
            EvalEveryStateSpawnEvaluator spawnEvaluator = new EvalEveryStateSpawnEvaluator();
            EvalStateNode spawned = child.NewState(spawnEvaluator, beginState, context, null);
            spawned.Start();

            // If the whole spawned expression already turned true, quit it again
            if (spawnEvaluator.IsEvaluatedTrue)
            {
                spawned.Quit();
            }
            else
            {
                spawnedNodes.Add(spawned);
                spawned.ParentEvaluator = this;
            }
        }

        /// <summary>
        /// Indicate a change in truth value to true.
        /// </summary>
        /// <param name="matchEvent">is the container for events that caused the change in truth value</param>
        /// <param name="fromNode">is the node that indicates the change</param>
        /// <param name="isQuitted">is an indication of whether the node continues listenening or Stops listening</param>
        public void EvaluateTrue(MatchedEventMap matchEvent, EvalStateNode fromNode, bool isQuitted)
        {
            if (ExecutionPathDebugLog.IsEnabled && log.IsDebugEnabled)
            {
                log.Debug(".evaluateTrue fromNode=" + fromNode + "  isQuitted=" + isQuitted);
            }

            if (isQuitted)
            {
                spawnedNodes.Remove(fromNode);
            }

            // See explanation in EvalFilterStateNode for the type check
            if (fromNode is EvalFilterStateNode)
            {
                // We do not need to newState new listeners here, since the filter state node below this node did not quit
            }
            else
            {
                // Spawn all nodes below this EVERY node
                // During the Start of a child we need to use the temporary evaluator to catch any event created during a Start
                // Such events can be raised when the "not" operator is used.
	            EvalNode child = FactoryNode.ChildNodes[0];
	            EvalEveryStateSpawnEvaluator spawnEvaluator = new EvalEveryStateSpawnEvaluator();
	            EvalStateNode spawned = child.NewState(spawnEvaluator, beginState, context, null);
                spawned.Start();

                // If the whole spawned expression already turned true, quit it again
                if (spawnEvaluator.IsEvaluatedTrue)
                {
                    spawned.Quit();
                }
                else
                {
                    spawnedNodes.Add(spawned);
                    spawned.ParentEvaluator = this;
                }
            }

            // All nodes indicate to their parents that their child node did not quit, therefore a false for isQuitted
            this.ParentEvaluator.EvaluateTrue(matchEvent, this, false);
        }

        /// <summary>
        /// Stops the event expression or an instance of it. Child classes are expected to free resources
        /// and Stop any event listeners or remove any time-based callbacks.
        /// </summary>
        public override void Quit()
        {
            if (ExecutionPathDebugLog.IsEnabled && log.IsDebugEnabled)
            {
                log.Debug(".Quit Quitting EVERY-node all children");
            }

            // Stop all child nodes
            foreach (EvalStateNode child in spawnedNodes)
            {
                child.Quit();
            }
        }

        /// <summary>
        /// Accept a visitor. Child classes are expected to invoke the visit method on the visitor instance
        /// passed in.
        /// </summary>
        /// <param name="visitor">on which the visit method is invoked by each node</param>
        /// <param name="data">any additional data the visitor may need is passed in this parameter</param>
        /// <returns>
        /// any additional data the visitor may need or null
        /// </returns>
        public override Object Accept(EvalStateNodeVisitor visitor, Object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary>
        /// Pass the visitor to all child nodes.
        /// </summary>
        /// <param name="visitor">is the instance to be passed to all child nodes</param>
        /// <param name="data">any additional data the visitor may need is passed in this parameter</param>
        /// <returns>
        /// any additional data the visitor may need or null
        /// </returns>
        public override Object ChildrenAccept(EvalStateNodeVisitor visitor, Object data)
        {
            foreach (EvalStateNode spawnedNode in spawnedNodes)
            {
                spawnedNode.Accept(visitor, data);
            }

            return data;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return "EvalEveryStateNode spawnedChildren=" + spawnedNodes.Count;
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
