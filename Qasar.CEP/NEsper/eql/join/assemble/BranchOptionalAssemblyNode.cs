using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.eql.join.rep;
using net.esper.events;
using net.esper.util;

namespace net.esper.eql.join.assemble
{
    /// <summary>
    /// Assembly node for an event stream that is a branch with a single optional child node below it.
    /// </summary>

    public class BranchOptionalAssemblyNode : BaseAssemblyNode
    {
        private IList<Node> resultsForStream;

        // For tracking when we only have a single event for this stream as a result
        private Node singleResultNode;
        private EventBean singleResultEvent;
        private bool haveChildResults;

        // For tracking when we have multiple events for this stream
        private Set<EventBean> completedEvents;

        /// <summary> Ctor.</summary>
        /// <param name="streamNum">is the stream number
        /// </param>
        /// <param name="numStreams">is the number of streams
        /// </param>

        public BranchOptionalAssemblyNode(int streamNum, int numStreams)
            : base(streamNum, numStreams)
        {
        }

        /// <summary>
        /// Provides results to assembly nodes for initialization.
        /// </summary>
        /// <param name="result">is a list of result nodes per stream</param>
        public override void Init(IList<Node>[] result)
        {
            resultsForStream = result[streamNum];
            singleResultNode = null;
            singleResultEvent = null;
            haveChildResults = false;

            if (resultsForStream != null)
            {
                int numNodes = resultsForStream.Count;
                if (numNodes == 1)
                {
                    Node node = resultsForStream[0];
                    Set<EventBean> nodeEvents = node.Events;

                    // If there is a single result event (typical case)
                    if (nodeEvents.Count == 1)
                    {
                        singleResultNode = node;
                        singleResultEvent = nodeEvents.First;
                    }
                }

                if (singleResultNode == null)
                {
                    completedEvents = new HashSet<EventBean>();
                }
            }
        }

        /// <summary>
        /// Process results.
        /// </summary>
        /// <param name="result">is a list of result nodes per stream</param>
        public override void Process(IList<Node>[] result)
        {
            // there cannot be child nodes to compute a cartesian product if this node had no results
            if (resultsForStream == null)
            {
                return;
            }

            // If this node's result set consisted of a single event
            if (singleResultNode != null)
            {
                // If there are no child results, post a row
                if (!haveChildResults)
                {
                    EventBean[] row = new EventBean[numStreams];
                    row[streamNum] = singleResultEvent;
                    parentNode.Result(row, streamNum, singleResultNode.ParentEvent, singleResultNode);
                }

                // if there were child results we are done since they have already been posted to the parent
                return;
            }

            // We have multiple events for this node, generate an event row for each event not yet received from
            // event rows generated by the child node.
            foreach (Node node in resultsForStream)
            {
                Set<EventBean> events = node.Events;
                foreach (EventBean _event in events)
                {
                    if (completedEvents.Contains(_event))
                    {
                        continue;
                    }
                    processEvent(_event, node);
                }
            }
        }

        /// <summary>
        /// Publish a result row.
        /// </summary>
        /// <param name="row">is the result to publish</param>
        /// <param name="fromStreamNum">is the originitor that publishes the row</param>
        /// <param name="myEvent">is optional and is the event that led to the row result</param>
        /// <param name="myNode">is optional and is the result node of the event that led to the row result</param>
        public override void Result(EventBean[] row, int fromStreamNum, EventBean myEvent, Node myNode)
        {
            row[streamNum] = myEvent;
            Node parentResultNode = myNode.Parent;
            parentNode.Result(row, streamNum, myNode.ParentEvent, parentResultNode);

            // record the fact that an event that was generated by a child
            haveChildResults = true;

            // If we had more then on result event for this stream, we need to track all the different events
            // generated by the child node
            if (singleResultNode == null)
            {
                completedEvents.Add(myEvent);
            }
        }

        /// <summary>
        /// Output this node using writer, not outputting child nodes.
        /// </summary>
        /// <param name="indentWriter">to use for output</param>
        public override void Print(IndentWriter indentWriter)
        {
            indentWriter.WriteLine("BranchOptionalAssemblyNode streamNum=" + streamNum);
        }

        /// <summary>
        /// Processes the event.
        /// </summary>
        /// <param name="_event">The _event.</param>
        /// <param name="currentNode">The current node.</param>
        private void processEvent(EventBean _event, Node currentNode)
        {
            EventBean[] row = new EventBean[numStreams];
            row[streamNum] = _event;
            parentNode.Result(row, streamNum, currentNode.ParentEvent, currentNode.Parent);
        }
    }
}
