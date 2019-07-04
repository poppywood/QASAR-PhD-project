///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace com.espertech.esper.epl.spec
{
    /// <summary>For use in select clauses for specifying a selected stream: select a.* from MyEvent as a, MyOther as b </summary>
    public class SelectClauseStreamRawSpec : SelectClauseElementRaw
    {
        private readonly string streamAliasName;
        private readonly string optionalAsName;
    
        /// <summary>Ctor. </summary>
        /// <param name="streamAliasName">is the stream alias of the stream to select</param>
        /// <param name="optionalAsName">is the column alias</param>
        public SelectClauseStreamRawSpec(string streamAliasName, string optionalAsName)
        {
            this.streamAliasName = streamAliasName;
            this.optionalAsName = optionalAsName;
        }

        /// <summary>Returns the stream alias (e.g. select streamAlias from MyEvent as streamAlias). </summary>
        /// <returns>alias</returns>
        public string StreamAliasName
        {
            get { return streamAliasName; }
        }

        /// <summary>Returns the column alias (e.g. select streamAlias as mycol from MyEvent as streamAlias). </summary>
        /// <returns>alias</returns>
        public string OptionalAsName
        {
            get { return optionalAsName; }
        }
    }
}
