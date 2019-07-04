///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
    /// <summary>
    /// Mirror class to <see cref="SelectExprElementStreamRawSpec"/> but added the stream number for the alias.
    /// </summary>
    public class SelectExprElementStreamCompiledSpec : MetaDefItem
    {
        private readonly String streamAliasName;
        private readonly String optionalAliasName;
        private readonly int streamNumber;
        private readonly bool isTaggedEvent;

        /// <summary>Ctor.</summary>
        /// <param name="streamAliasName">is the stream alias of the stream to select</param>
        /// <param name="optionalAliasName">is the column alias</param>
        /// <param name="streamNumber">is the number of the stream</param>
        /// <param name="isTaggedEvent">
        /// is true to indicate that we are meaning to select a tagged event in a pattern
        /// </param>
        public SelectExprElementStreamCompiledSpec(String streamAliasName, String optionalAliasName, int streamNumber, bool isTaggedEvent)
        {
            this.streamAliasName = streamAliasName;
            this.optionalAliasName = optionalAliasName;
            this.streamNumber = streamNumber;
            this.isTaggedEvent = isTaggedEvent;
        }

        /// <summary>
        /// Returns the stream alias (e.g. select streamAlias from MyEvent as streamAlias).
        /// </summary>
        /// <returns>alias</returns>
        public String StreamAliasName
        {
            get { return streamAliasName; }
        }

        /// <summary>
        /// Returns the column alias (e.g. select streamAlias as mycol from MyEvent as streamAlias).
        /// </summary>
        /// <returns>alias</returns>
        public String OptionalAliasName
        {
            get { return optionalAliasName; }
        }

        /// <summary>Returns the stream number of the stream for the stream alias.</summary>
        /// <returns>stream number</returns>
        public int StreamNumber
        {
            get { return streamNumber; }
        }

        /// <summary>
        /// Returns true to indicate that we are meaning to select a tagged event in a pattern, or false if
        /// selecting an event from a stream.
        /// </summary>
        /// <returns>true for tagged event in pattern, false for stream</returns>
        public bool IsTaggedEvent
        {
            get { return isTaggedEvent; }
        }
    }
} // End of namespace
