///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace com.espertech.esper.core
{
	/// <summary>
	/// Helper methods for use by the statement object model.
	/// </summary>
	public class EPStatementObjectModelHelper
	{
	    /// <summary>Renders a constant as an EPL.</summary>
	    /// <param name="writer">to output to</param>
	    /// <param name="constant">to render</param>
	    public static void RenderEQL(StringWriter writer, Object constant)
	    {
	        if (constant == null)
	        {
	            writer.Write("null");
	            return;
	        }

	        if ((constant is String) ||
	            (constant is Char))
	        {
	            writer.Write('\"');
	            writer.Write(constant.ToString());
	            writer.Write('\"');
	        }
            else if (constant is Double)
            {
                double dvalue = (double) constant;
                double scrubbed = Math.Floor(dvalue);
                if (dvalue == scrubbed)
                {
                    writer.Write("{0:F2}", dvalue);
                } else
                {
                    writer.Write("{0}", dvalue);
                }
            }
            else if (constant is Single)
            {
                double dvalue = (float)constant;
                double scrubbed = Math.Floor(dvalue);
                if (dvalue == scrubbed)
                {
                    writer.Write("{0:F2}", dvalue);
                }
                else
                {
                    writer.Write("{0}", dvalue);
                }

            } 
            else if (constant is Decimal)
            {
                decimal dvalue = (decimal)constant;
                decimal scrubbed = Math.Floor(dvalue);
                if (dvalue == scrubbed)
                {
                    writer.Write("{0:F2}", dvalue);
                }
                else
                {
                    writer.Write("{0}", dvalue);
                }
            }
	        else
	        {
	            writer.Write(constant.ToString());
	        }
	    }
	}
} // End of namespace
