///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.epl.variable
{
    /// <summary>
    /// Exception indicating a problem in a variable declaration.
    /// </summary>
	public class VariableDeclarationException : Exception
	{
	    /// <summary>Ctor.</summary>
	    /// <param name="msg">the exception message.</param>
	    public VariableDeclarationException(String msg)
            : base(msg)
	    {
	    }
	}
} // End of namespace
