///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using com.espertech.esper.compat;

namespace com.espertech.esper.epl.spec
{
    /// <summary>
    /// Mirror class to <see cref="SelectClauseStreamRawSpec"/> but added the stream number for the alias.
    /// </summary>
    public class SelectClauseStreamCompiledSpec : SelectClauseElementCompiled
    {
        private readonly String streamAliasName;
        private readonly String optionalAliasName;
        private int streamNumber = -1;
        private bool isTaggedEvent = false;
        private bool isProperty = false;
        private Type propertyType;

        /// <summary>Ctor. </summary>
        /// <param name="streamAliasName">is the stream alias of the stream to select</param>
        /// <param name="optionalAliasName">is the column alias</param>
        public SelectClauseStreamCompiledSpec(String streamAliasName, String optionalAliasName)
        {
            this.streamAliasName = streamAliasName;
            this.optionalAliasName = optionalAliasName;
        }

        /// <summary>Returns the stream alias (e.g. select streamAlias from MyEvent as streamAlias). </summary>
        /// <returns>alias</returns>
        public String StreamAliasName
        {
            get { return streamAliasName; }
        }

        /// <summary>Returns the column alias (e.g. select streamAlias as mycol from MyEvent as streamAlias). </summary>
        /// <returns>alias</returns>
        public String OptionalAliasName
        {
            get { return optionalAliasName; }
        }

        /// <summary>Gets or sets the stream number of the stream for the stream alias. </summary>
        /// <returns>stream number</returns>
        public int StreamNumber
        {
            get
            {
                if (streamNumber == -1)
                {
                    throw new IllegalStateException("Not initialized for stream number and tagged event");
                }
                return streamNumber;
            }

            set { this.streamNumber = value; }
        }

        /// <summary>Returns true to indicate that we are meaning to select a tagged event in a pattern, or false if selecting an event from a stream. </summary>
        /// <returns>true for tagged event in pattern, false for stream</returns>
        public bool IsTaggedEvent
        {
            get
            {
                if (streamNumber == -1)
                {
                    throw new IllegalStateException("Not initialized for stream number and tagged event");
                }
                return isTaggedEvent;
            }

            set { isTaggedEvent = value; }
        }

        /// <summary>True if selecting from a property, false if not</summary>
        /// <returns>indicator whether property or not</returns>
        public bool IsProperty
        {
            get { return isProperty; }
            set { isProperty = value; }

        }

        /// <summary>Gets or sets property type.</summary>
        /// <returns>property type</returns>
        public Type PropertyType
        {
            get { return propertyType; }
            set { propertyType = value; }
        }
    }
}
