///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

using Antlr.Runtime;

namespace com.espertech.esper.antlr
{
    /// <summary>
    /// For use with ANTLR to create a case-insensitive token stream.
    /// </summary>
	public class NoCaseSensitiveStream : ANTLRReaderStream
	{
	    /// <summary>Ctor.</summary>
	    /// <param name="s">string to be parsed</param>
	    /// <throws>IOException to indicate IO errors</throws>
	    public NoCaseSensitiveStream(String s) : base(new StringReader(s))
	    {
	    }

		public override int LA(int i) {
			if ( i==0 ) {
				return 0; // undefined
			}
			if ( i<0 ) {
				i++; // e.g., translate LA(-1) to use offset 0
			}
			if ( (p+i-1) >= n ) {
			    return (int) CharStreamConstants.EOF;
	        }
            return Char.ToLower(data[p+i-1]);
	    }
	}
} // End of namespace
