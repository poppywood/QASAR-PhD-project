///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

namespace net.esper.view.window
{
    /// <summary>
    /// For indicating that the collection has been updated.
    /// </summary>
    public interface RandomAccessByIndexObserver
    {
        /// <summary>
        /// Callback to indicate an update
        /// </summary>
        /// <param name="randomAccessByIndex">is the collection</param>
        void Updated(RandomAccessByIndex randomAccessByIndex);
    }

    /// <summary>
    /// Delegate used to proxy the RandomAccessByIndexObserver
    /// </summary>
    /// <param name="randomAccessByIndex"></param>

    public delegate void RandomAccessByIndexDelegate(RandomAccessByIndex randomAccessByIndex);

    /// <summary>
    /// Proxies the RandomAccessByIndexObserver with a delegate
    /// </summary>

    public class ProxyRandomAccessByIndexObserver : RandomAccessByIndexObserver
    {
        private readonly RandomAccessByIndexDelegate m_delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyRandomAccessByIndexObserver"/> class.
        /// </summary>
        /// <param name="dg">The dg.</param>
        public ProxyRandomAccessByIndexObserver(RandomAccessByIndexDelegate dg)
        {
            this.m_delegate = dg;
        }

        /// <summary>
        /// Callback to indicate an update
        /// </summary>
        /// <param name="randomAccessByIndex">is the collection</param>
        public void Updated(RandomAccessByIndex randomAccessByIndex)
        {
            m_delegate(randomAccessByIndex);
        }
    }
} // End of namespace
