///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace com.espertech.esper.type
{
	/// <summary>
	/// Interface for parameter types that can represent themselves as an EQL syntax.
	/// </summary>
	public interface EQLParameterType
	{
	    /// <summary>Returns the EQL representation of the parameter.</summary>
	    /// <param name="writer">for output to</param>
	    void ToEPL(StringWriter writer);
	}
} // End of namespace
