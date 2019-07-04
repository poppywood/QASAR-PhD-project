///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.view;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Entry holding lookup resource references for use by <see cref="SubSelectStreamCollection"/>.
    /// </summary>
    public class SubSelectHolder
    {
        private readonly int streamNumber;
        private readonly Viewable viewable;
        private readonly ViewFactoryChain viewFactoryChain;

        /// <summary>Ctor.</summary>
        /// <param name="streamNumber">is the lookup stream number</param>
        /// <param name="viewable">is the root viewable</param>
        /// <param name="viewFactoryChain">is the view chain</param>
        public SubSelectHolder(int streamNumber, Viewable viewable, ViewFactoryChain viewFactoryChain)
        {
            this.streamNumber = streamNumber;
            this.viewable = viewable;
            this.viewFactoryChain = viewFactoryChain;
        }

        /// <summary>Returns lookup stream number.</summary>
        /// <returns>stream num</returns>
        public int StreamNumber
        {
            get { return streamNumber; }
        }

        /// <summary>Returns the lookup child viewable.</summary>
        /// <returns>child-most viewable</returns>
        public Viewable Viewable
        {
            get { return viewable; }
        }

        /// <summary>Returns the lookup view factory chain</summary>
        /// <returns>view factory chain</returns>
        public ViewFactoryChain ViewFactoryChain
        {
            get { return viewFactoryChain; }
        }
    }
}
