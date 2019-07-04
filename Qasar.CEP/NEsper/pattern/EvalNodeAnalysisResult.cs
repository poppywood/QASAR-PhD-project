///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace net.esper.pattern
{
    /// <summary>
    /// Result of analysis of pattern expression node tree.
    /// </summary>
    public class EvalNodeAnalysisResult
    {
        private readonly List<EvalFilterNode> filterNodes = new List<EvalFilterNode>();
        private readonly List<EvalGuardNode> guardNodes = new List<EvalGuardNode>();
        private readonly List<EvalObserverNode> observerNodes = new List<EvalObserverNode>();

        /// <summary>Adds a filter node.</summary>
        /// <param name="filterNode">filter node to add</param>
        public void Add(EvalFilterNode filterNode)
        {
            filterNodes.Add(filterNode);
        }
        /// <summary>Adds a guard node.</summary>
        /// <param name="guardNode">node to add</param>
        public void Add(EvalGuardNode guardNode)
        {
            guardNodes.Add(guardNode);
        }
        /// <summary>Adds an observer node.</summary>
        /// <param name="observerNode">node to add</param>
        public void Add(EvalObserverNode observerNode)
        {
            observerNodes.Add(observerNode);
        }

        /// <summary>Returns filter nodes.</summary>
        /// <returns>filter nodes</returns>
        public IList<EvalFilterNode> FilterNodes
        {
            get { return filterNodes; }
        }

        /// <summary>Returns guard nodes.</summary>
        /// <returns>guard nodes</returns>
        public IList<EvalGuardNode> GuardNodes
        {
            get { return guardNodes; }
        }

        /// <summary>Returns observer nodes.</summary>
        /// <returns>observer nodes</returns>
        public IList<EvalObserverNode> ObserverNodes
        {
            get { return observerNodes; }
        }
    }
} // End of namespace
