///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using NUnit.Framework;

using net.esper.example.transaction.sim;

namespace net.esper.example.transaction
{
	[TestFixture]
	public class TestTxnSimMain
	{
	    [Test]
	    public void TestTiny()
	    {
	        TxnGenMain main = new TxnGenMain(20, 200);
	        main.Run();
	    }

	    [Test]
	    public void TestSmall()
	    {
	        TxnGenMain main = new TxnGenMain(1000, 3000);
	        main.Run();
	    }
	}
} // End of namespace
