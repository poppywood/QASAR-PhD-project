using System;
using System.Collections.Generic;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Specification for a stream, consists simply of an optional stream name and a list of views
	/// on to of the stream.
	/// <para>
	/// Implementation classes for views and patterns add additional information defining the
	/// stream of events.
    /// </para>
	/// </summary>

    public interface StreamSpec
    {
        /// <summary>Returns the stream name, or null if undefined.</summary>

        String OptionalStreamName { get ; }

        /// <summary> Returns views definitions onto the stream.</summary>

        IList<ViewSpec> ViewSpecs { get; }

        /// <summary>Returns true to indicate a unidirectional stream in a join, applicable for joins.</summary>
        /// <returns>indicator whether the stream is unidirectional in a join</returns>
        bool IsUnidirectional { get; }
    }
}
