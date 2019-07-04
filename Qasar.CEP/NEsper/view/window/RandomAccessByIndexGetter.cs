///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace net.esper.view.window
{
	/// <summary>Getter that provides an index at runtime.</summary>
    public class RandomAccessByIndexGetter : RandomAccessByIndexObserver
	{
	    private RandomAccessByIndex randomAccessByIndex;

	    /// <summary>Returns the index for access.</summary>
	    /// <returns>index</returns>
	    public RandomAccessByIndex Accessor
	    {
	        get { return randomAccessByIndex; }
	    }

        /// <summary>
        /// Callback to indicate an update
        /// </summary>
        /// <param name="randomAccessByIndex">is the collection</param>
        public void Updated(RandomAccessByIndex randomAccessByIndex)
        {
            this.randomAccessByIndex = randomAccessByIndex;
        }
	}
} // End of namespace
