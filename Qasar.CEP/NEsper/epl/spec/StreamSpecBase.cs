///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.epl.spec;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Abstract base specification for a stream, consists simply of an optional stream name and a list of views
	/// on to of the stream.
	/// <para>
	/// Implementation classes for views and patterns add additional information defining the
	/// stream of events.
	/// </para>
	/// </summary>
	public abstract class StreamSpecBase : MetaDefItem
	{
	    private readonly String optionalStreamName;
	    private readonly List<ViewSpec> viewSpecs = new List<ViewSpec>();
        private readonly bool isUnidirectional;

        /// <summary>Ctor.</summary>
        /// <param name="optionalStreamName">stream name, or null if none supplied</param>
        /// <param name="viewSpecs">specifies what view to use to derive data</param>
        /// <param name="isUnidirectional">true to indicate a unidirectional stream in a join, applicable for joins</param>
        public StreamSpecBase(String optionalStreamName, IEnumerable<ViewSpec> viewSpecs, bool isUnidirectional)
        {
            this.optionalStreamName = optionalStreamName;
            this.viewSpecs.AddRange(viewSpecs);
            this.isUnidirectional = isUnidirectional;
        }

	    /// <summary>Default ctor.</summary>
	    public StreamSpecBase()
	    {
	    }

	    /// <summary>Returns the name assigned.</summary>
	    /// <returns>stream name or null if not assigned</returns>
	    public String OptionalStreamName
	    {
            get { return optionalStreamName; }
	    }

	    /// <summary>
	    /// Returns view definitions to use to construct views to derive data on stream.
	    /// </summary>
	    /// <returns>view defs</returns>
	    public IList<ViewSpec> ViewSpecs
	    {
            get { return viewSpecs; }
	    }

        /// <summary>Returns true to indicate a unidirectional stream in a join, applicable for joins.</summary>
        /// <returns>indicator whether the stream is unidirectional in a join</returns>
        public bool IsUnidirectional
	    {
	        get { return isUnidirectional; }
	    }
	}
} // End of namespace
