///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using net.esper.compat;

using NUnit.Framework;

namespace net.esper.adapter.csv
{
    [TestFixture]
	public class TestPropertyOrderHelper
	{
		private EDictionary<String, Type> propertyTypes;

        [SetUp]
		protected void SetUp()
		{
			propertyTypes = new LinkedDictionary<String, Type>();
            propertyTypes.Put("myInt", typeof (int));
			propertyTypes.Put("myDouble", typeof(double));
			propertyTypes.Put("myString", typeof(string));
		}

        [Test]
		public void TestResolveTitleRow()
		{
			// Use first row
			String[] firstRow = new String[] { "myDouble", "myInt", "myString" };
			Assert.AreEqual(firstRow, CSVPropertyOrderHelper.ResolvePropertyOrder(firstRow, propertyTypes));
		}
	}
} // End of namespace
