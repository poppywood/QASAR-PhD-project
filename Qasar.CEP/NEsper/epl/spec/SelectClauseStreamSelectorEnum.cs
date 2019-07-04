using System;

using com.espertech.esper.client.soda;

namespace com.espertech.esper.epl.spec
{
    /// <summary>
    /// Enumeration for representing select-clause selection of the remove stream or the insert stream, or both.
    /// </summary>

    public enum SelectClauseStreamSelectorEnum
    {
        /// <summary> Indicates selection of the remove stream only.</summary>
        RSTREAM_ONLY,
        /// <summary> Indicates selection of the insert stream only.</summary>
        ISTREAM_ONLY,
        /// <summary> Indicates selection of both the insert and the remove stream.  </summary>
        RSTREAM_ISTREAM_BOTH
    }

    public class SelectClauseStreamSelectorHelper
    {
        /// <summary>Maps the SODA-selector to the internal representation</summary>
        /// <param name="selector">is the SODA-selector to map</param>
        /// <returns>internal stream selector</returns>
        public static SelectClauseStreamSelectorEnum MapFromSODA(StreamSelector selector)
        {
            switch (selector)
            {
                case StreamSelector.ISTREAM_ONLY:
                    return SelectClauseStreamSelectorEnum.ISTREAM_ONLY;
                case StreamSelector.RSTREAM_ONLY:
                    return SelectClauseStreamSelectorEnum.RSTREAM_ONLY;
                case StreamSelector.RSTREAM_ISTREAM_BOTH:
                    return SelectClauseStreamSelectorEnum.RSTREAM_ISTREAM_BOTH;
                default:
                    throw new ArgumentException("Invalid selector '" + selector + "' encountered");
            }
        }

        /// <summary>Maps the internal stream selector to the SODA-representation</summary>
        /// <param name="selector">is the internal selector to map</param>
        /// <returns>SODA stream selector</returns>
        public static StreamSelector MapFromSODA(SelectClauseStreamSelectorEnum selector)
        {
            switch (selector)
            {
                case SelectClauseStreamSelectorEnum.ISTREAM_ONLY:
                    return StreamSelector.ISTREAM_ONLY;
                case SelectClauseStreamSelectorEnum.RSTREAM_ONLY:
                    return StreamSelector.RSTREAM_ONLY;
                case SelectClauseStreamSelectorEnum.RSTREAM_ISTREAM_BOTH:
                    return StreamSelector.RSTREAM_ISTREAM_BOTH;
                default:
                    throw new ArgumentException("Invalid selector '" + selector + "' encountered");
            }
        }
    }
}
