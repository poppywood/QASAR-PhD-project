using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.util;

using org.apache.commons.logging;

namespace net.esper.pattern
{
    /// <summary> Superclass of all nodes in an evaluation tree representing an event pattern expression.
    /// Follows the Composite pattern. Child nodes do not carry references to parent nodes, the tree
    /// is unidirectional.
    /// </summary>

    public abstract class EvalNode
    {
        private readonly List<EvalNode> childNodes;
		private EvalNodeNumber nodeNumber;

		/// <summary>
		/// Gets or sets the evaluation node's relative node number in
		/// the evaluation node tree.
		/// </summary>

		public EvalNodeNumber NodeNumber
		{
			get { return nodeNumber; }
			set { nodeNumber = value ; }
		}

        /// <summary> Create the evaluation state node containing the truth value state for each operator in an
        /// event expression.
        /// </summary>
        /// <param name="parentNode">is the parent evaluator node that this node indicates a change in truth value to
        /// </param>
        /// <param name="beginState">is the container for events that makes up the Start state
        /// </param>
        /// <param name="context">is the handle to services required for evaluation
        /// </param>
		/// <param name="stateNodeId">is the new state object's identifier
		/// </param>
        /// <returns> state node containing the truth value state for the operator
        /// </returns>
        public abstract EvalStateNode NewState(Evaluator parentNode,
											   MatchedEventMap beginState,
											   PatternContext context,
											   Object stateNodeId);

        /// <summary> Constructor creates a list of child nodes.</summary>
        internal EvalNode()
        {
            childNodes = new List<EvalNode>();
        }

        /// <summary> Adds a child node.</summary>
        /// <param name="childNode">is the child evaluation tree node to add
        /// </param>
        public void AddChildNode(EvalNode childNode)
        {
            childNodes.Add(childNode);
        }

        /// <summary> Returns list of child nodes.</summary>
        /// <returns> list of child nodes
        /// </returns>
        public List<EvalNode> ChildNodes
        {
            get { return childNodes; }
        }

        /// <summary> Recursively print out all nodes.</summary>
        /// <param name="prefix">is printed out for naming the printed info
        /// </param>
        public void DumpDebug(String prefix)
        {
            if (ExecutionPathDebugLog.IsEnabled && log.IsDebugEnabled)
            {
                log.Debug(".DumpDebug " + prefix + this);
            }
            foreach (EvalNode node in childNodes)
            {
                node.DumpDebug(prefix + "  ");
            }
        }

        public static EvalNodeAnalysisResult RecursiveAnalyzeChildNodes(EvalNode currentNode)
        {
            EvalNodeAnalysisResult evalNodeAnalysisResult = new EvalNodeAnalysisResult();
            RecursiveAnalyzeChildNodes(evalNodeAnalysisResult, currentNode);
            return evalNodeAnalysisResult;
        }

        private static void RecursiveAnalyzeChildNodes(EvalNodeAnalysisResult evalNodeAnalysisResult, EvalNode currentNode)
        {
            if (currentNode is EvalFilterNode)
            {
                evalNodeAnalysisResult.Add((EvalFilterNode) currentNode);
            }
            if (currentNode is EvalGuardNode)
            {
                evalNodeAnalysisResult.Add((EvalGuardNode) currentNode);
            }
            if (currentNode is EvalObserverNode)
            {
                evalNodeAnalysisResult.Add((EvalObserverNode) currentNode);
            }
            foreach (EvalNode node in currentNode.ChildNodes)
            {
                RecursiveAnalyzeChildNodes(evalNodeAnalysisResult, node);
            }
        }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
