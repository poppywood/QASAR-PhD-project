using System.Collections.Generic;

using com.espertech.esper.compat;
using com.espertech.esper.epl.join.rep;
using com.espertech.esper.events;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.join.assemble
{
    /// <summary>
    /// Assembly node for an event stream that is a root with a two or more child nodes below it.
    /// </summary>

    public class RootCartProdAssemblyNode : BaseAssemblyNode
    {
        private readonly int[] childStreamIndex; // maintain mapping of stream number to index in array
        private readonly IList<EventBean[]>[] rowsPerStream;
        private bool allSubStreamsOptional;

        // maintain for each child the list of stream number descending that child
        private int[][] subStreamsNumsPerChild;
        private int[][] combinedSubStreams; // for any cartesian product past 2 streams
        private bool haveChildResults;

        /// <summary> Ctor.</summary>
        /// <param name="streamNum">is the stream number
        /// </param>
        /// <param name="numStreams">is the number of streams
        /// </param>
        /// <param name="allSubStreamsOptional">true if all substreams are optional and none are required
        /// </param>
        public RootCartProdAssemblyNode(int streamNum, int numStreams, bool allSubStreamsOptional)
            : base(streamNum, numStreams)
        {
            this.allSubStreamsOptional = allSubStreamsOptional;
            this.childStreamIndex = new int[numStreams];
            this.rowsPerStream = new IList<EventBean[]>[numStreams];
        }

        /// <summary>
        /// Add a child node.
        /// </summary>
        /// <param name="childNode">to add</param>
        public override void AddChild(BaseAssemblyNode childNode)
        {
            childStreamIndex[childNode.StreamNum] = childNodes.Count;
            base.AddChild(childNode);
        }

        /// <summary>
        /// Provides results to assembly nodes for initialization.
        /// </summary>
        /// <param name="result">is a list of result nodes per stream</param>
        public override void Init(IList<Node>[] result)
        {
            if (subStreamsNumsPerChild == null)
            {
                if (childNodes.Count < 2)
                {
                    throw new IllegalStateException("Expecting at least 2 child nodes");
                }
                subStreamsNumsPerChild = new int[childNodes.Count][];
                for (int i = 0; i < childNodes.Count; i++)
                {
                    subStreamsNumsPerChild[i] = childNodes[i].Substreams;
                }

                combinedSubStreams = ComputeCombined(subStreamsNumsPerChild);
            }

            haveChildResults = false;
            for (int i = 0; i < rowsPerStream.Length; i++)
            {
                rowsPerStream[i] = null;
            }
        }

        /// <summary>
        /// Process results.
        /// </summary>
        /// <param name="result">is a list of result nodes per stream</param>
        public override void Process(IList<Node>[] result)
        {
            // If no child has posted any rows, generate row and done
            if ((!haveChildResults) && (allSubStreamsOptional))
            {
                // post an empty row
                EventBean[] row = new EventBean[numStreams];
                parentNode.Result(row, streamNum, null, null);
                return;
            }

            // Compute the cartesian product
            postCartesian(rowsPerStream);
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
            haveChildResults = true;

            // fill event in
            row[streamNum] = myEvent;
            int childStreamArrIndex = childStreamIndex[fromStreamNum];

            // keep a reference to the row to build a cartesian product on the call to process
            IList<EventBean[]> rows = rowsPerStream[childStreamArrIndex];
            if (rows == null)
            {
                rows = new List<EventBean[]>();
                rowsPerStream[childStreamArrIndex] = rows;
            }
            rows.Add(row);
        }

        /// <summary>
        /// Output this node using writer, not outputting child nodes.
        /// </summary>
        /// <param name="indentWriter">to use for output</param>
        public override void Print(IndentWriter indentWriter)
        {
            indentWriter.WriteLine("RootCartProdAssemblyNode streamNum=" + streamNum);
        }

        private void postCartesian(IList<EventBean[]>[] rowsPerStream)
        {
            IList<EventBean[]> result = new List<EventBean[]>();
            CartesianUtil.ComputeCartesian(
            		rowsPerStream[0], subStreamsNumsPerChild[0],
            		rowsPerStream[1], subStreamsNumsPerChild[1],
            		result);

            if (rowsPerStream.Length > 2)
            {
                for (int i = 0; i < subStreamsNumsPerChild.Length - 2; i++)
                {
                    IList<EventBean[]> product = new List<EventBean[]>();
                    CartesianUtil.ComputeCartesian(result, combinedSubStreams[i], rowsPerStream[i + 2], subStreamsNumsPerChild[i + 2], product);
                    result = product;
                }
            }

            foreach (EventBean[] row in result)
            {
                parentNode.Result(row, streamNum, null, null);
            }
        }

        /// <summary> Compute an array of supersets of sub stream numbers per stream, for at least 3 or more streams.</summary>
        /// <param name="subStreamsPerChild">is for each stream number a list of direct child sub streams
        /// </param>
        /// <returns> an array in with length (subStreamsPerChild.Length - 2) in which
        /// array[0] contains the streams for subStreamsPerChild[0] and subStreamsPerChild[1] combined, and
        /// array[1] contains the streams for subStreamsPerChild[0], subStreamsPerChild[1] and subStreamsPerChild[2] combined
        /// </returns>
        public static int[][] ComputeCombined(int[][] subStreamsPerChild)
        {
            if (subStreamsPerChild.Length < 3)
            {
                return null;
            }

            // Add all substreams of (1 + 2)  up into = Sum3
            // Then add all substreams of (Sum3 + 3) => Sum4
            // Results in an array of size (subStreamsPerChild.Length - 2) containing Sum3, Sum4 etc

            int[][] result = new int[subStreamsPerChild.Length - 2][];

            result[0] = AddSubstreams(subStreamsPerChild[0], subStreamsPerChild[1]);
            for (int i = 0; i < subStreamsPerChild.Length - 3; i++)
            {
                result[i + 1] = AddSubstreams(result[i], subStreamsPerChild[i + 2]);
            }

            return result;
        }

        private static int[] AddSubstreams(int[] arrayOne, int[] arrayTwo)
        {
            int[] result = new int[arrayOne.Length + arrayTwo.Length];

            int count = 0;
            for (int i = 0; i < arrayOne.Length; i++)
            {
                result[count] = arrayOne[i];
                count++;
            }

            for (int i = 0; i < arrayTwo.Length; i++)
            {
                result[count] = arrayTwo[i];
                count++;
            }
            return result;
        }
    }
}
