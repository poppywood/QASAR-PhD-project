///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using net.esper.support.client;

using NUnit.Framework;

using net.esper.client;
using net.esper.eql.parse;
using net.esper.support.bean;

using org.apache.commons.logging;

namespace net.esper.regression.pattern
{
	[TestFixture]
	public class TestInvalidPattern
	{
	    private EPServiceProvider epService;

	    private readonly String EVENT_NUM = typeof(SupportBean_N).FullName;
	    private readonly String EVENT_COMPLEX = typeof(SupportBeanComplexProps).FullName;
	    private readonly String EVENT_ALLTYPES = typeof(SupportBean).FullName;

	    [SetUp]
	    public void SetUp()
	    {
	        EPServiceProviderManager.PurgeAllProviders();
	        epService = EPServiceProviderManager.GetDefaultProvider(SupportConfigFactory.Configuration);
	    }

        [Test]
	    public void testNullStatementName()
	    {
	        EPStatement stmt = epService.EPAdministrator.CreateEQL("select * from " + typeof(SupportBean).FullName, null);
	    }

	    [Test]
	    public void testSyntaxException()
	    {
	        String exceptionText = GetSyntaxExceptionPattern(EVENT_NUM + "(doublePrimitive='ss'");
	        Assert.AreEqual("end of input when expecting \"a closing parenthesis ')'\" near line 1, column 58 [net.esper.support.bean.SupportBean_N(doublePrimitive='ss']", exceptionText);
	    }

	    [Test]
	    public void testStatementException()
	    {
	        String exceptionText = null;

            exceptionText = GetStatementExceptionPattern(typeof(SupportBean).FullName + " -> timer:at(2,3,4,4,4)");
            Assert.AreEqual("Invalid parameter for pattern observer: Invalid combination between days of week and days of month fields for timer:at [net.esper.support.bean.SupportBean -> timer:at(2,3,4,4,4)]", exceptionText);

            exceptionText = GetStatementExceptionPattern(EVENT_ALLTYPES + " -> timer:within()");
            Assert.AreEqual("Failed to resolve pattern object: Pattern guard function 'within' cannot be used as a pattern observer [net.esper.support.bean.SupportBean -> timer:within()]", exceptionText);

            exceptionText = GetStatementExceptionPattern(EVENT_ALLTYPES + " where timer:interval(100)");
            Assert.AreEqual("Failed to resolve pattern object: Pattern observer function 'interval' cannot be used as a pattern guard [net.esper.support.bean.SupportBean where timer:interval(100)]", exceptionText);

            exceptionText = GetStatementExceptionPattern(EVENT_ALLTYPES + " -> timer:interval()");
            Assert.AreEqual("Invalid parameter for pattern observer: Timer-interval observer requires a single numeric or time period parameter [net.esper.support.bean.SupportBean -> timer:interval()]", exceptionText);

            exceptionText = GetStatementExceptionPattern(EVENT_ALLTYPES + " where timer:within()");
            Assert.AreEqual("Invalid parameter for pattern guard: Timer-within guard requires a single numeric or time period parameter [net.esper.support.bean.SupportBean where timer:within()]", exceptionText);

	        // class not found
	        exceptionText = GetStatementExceptionPattern("dummypkg.Dummy()");
	        Assert.AreEqual("Failed to resolve event type: Failed to load class dummypkg.Dummy [dummypkg.Dummy()]", exceptionText);

	        // simple property not found
	        exceptionText = GetStatementExceptionPattern(EVENT_NUM + "(dummy=1)");
	        Assert.AreEqual("Property named 'dummy' is not valid in any stream [net.esper.support.bean.SupportBean_N(dummy=1)]", exceptionText);

	        // nested property not found
	        exceptionText = GetStatementExceptionPattern(EVENT_NUM + "(dummy.nested=1)");
	        Assert.AreEqual("Failed to resolve property 'dummy.nested' to a stream or nested property in a stream [net.esper.support.bean.SupportBean_N(dummy.nested=1)]", exceptionText);

	        // property wrong type
	        exceptionText = GetStatementExceptionPattern(EVENT_NUM + "(intPrimitive='s')");
	        Assert.AreEqual("Implicit conversion from datatype '" + typeof(int?).FullName + "' to 'System.String' is not allowed [net.esper.support.bean.SupportBean_N(intPrimitive='s')]", exceptionText);

	        // property not a primitive type
	        exceptionText = GetStatementExceptionPattern(EVENT_COMPLEX + "(nested=1)");
            Assert.AreEqual("Implicit conversion from datatype '" + typeof(SupportBeanComplexProps.SupportBeanSpecialGetterNested).FullName + "' to '" + typeof(int?).FullName + "' is not allowed [net.esper.support.bean.SupportBeanComplexProps(nested=1)]", exceptionText);

	        // no tag matches prior use
	        exceptionText = GetStatementExceptionPattern(EVENT_NUM + "(doublePrimitive=x.abc)");
	        Assert.AreEqual("Failed to resolve property 'x.abc' to a stream or nested property in a stream [net.esper.support.bean.SupportBean_N(doublePrimitive=x.abc)]", exceptionText);

	        // range not valid on string
	        exceptionText = GetStatementExceptionPattern(EVENT_ALLTYPES + "(string in [1:2])");
	        Assert.AreEqual("Implicit conversion from datatype 'System.String' to numeric is not allowed [net.esper.support.bean.SupportBean(string in [1:2])]", exceptionText);

	        // range does not allow string params
	        exceptionText = GetStatementExceptionPattern(EVENT_ALLTYPES + "(doubleBoxed in ['a':2])");
	        Assert.AreEqual("Implicit conversion from datatype 'System.String' to numeric is not allowed [net.esper.support.bean.SupportBean(doubleBoxed in ['a':2])]", exceptionText);

            // invalid observer arg
            exceptionText = GetStatementExceptionPattern("timer:at(9l)");
            Assert.AreEqual("Invalid parameter for pattern observer: Invalid number of parameters for timer:at [timer:at(9l)]", exceptionText);

            // invalid guard arg
            exceptionText = GetStatementExceptionPattern(EVENT_ALLTYPES + " where timer:within('s')");
            Assert.AreEqual("Invalid parameter for pattern guard: Timer-within guard requires a single numeric or time period parameter [net.esper.support.bean.SupportBean where timer:within('s')]", exceptionText);

	        // use-result property is wrong type
	        exceptionText = GetStatementExceptionPattern("x=" + EVENT_ALLTYPES + " -> " + EVENT_ALLTYPES + "(doublePrimitive=x.boolBoxed)");
            Assert.AreEqual("Implicit conversion from datatype '" + typeof(double?).FullName + "' to '" + typeof(bool?).FullName + "' is not allowed [x=net.esper.support.bean.SupportBean -> net.esper.support.bean.SupportBean(doublePrimitive=x.boolBoxed)]", exceptionText);
	    }

	    [Test]
	    public void testUseResult()
	    {
	        String _event = typeof(SupportBean_N).FullName;

	        TryValid("na=" + _event + " -> nb=" + _event + "(doublePrimitive = na.doublePrimitive)");
	        TryInvalid("xx=" + _event + " -> nb=" + _event + "(doublePrimitive = na.doublePrimitive)");
	        TryInvalid("na=" + _event + " -> nb=" + _event + "(doublePrimitive = xx.doublePrimitive)");
	        TryInvalid("na=" + _event + " -> nb=" + _event + "(doublePrimitive = na.xx)");
	        TryInvalid("xx=" + _event + " -> nb=" + _event + "(xx = na.doublePrimitive)");
	        TryInvalid("na=" + _event + " -> nb=" + _event + "(xx = na.xx)");
	        TryValid("na=" + _event + " -> nb=" + _event + "(doublePrimitive = na.doublePrimitive, intBoxed=na.intBoxed)");
	        TryValid("na=" + _event + "() -> nb=" + _event + "(doublePrimitive in (na.doublePrimitive:na.doubleBoxed))");
	        TryValid("na=" + _event + "() -> nb=" + _event + "(doublePrimitive in [na.doublePrimitive:na.doubleBoxed])");
	        TryValid("na=" + _event + "() -> nb=" + _event + "(doublePrimitive in [na.intBoxed:na.intPrimitive])");
	        TryInvalid("na=" + _event + "() -> nb=" + _event + "(doublePrimitive in [na.intBoxed:na.xx])");
	        TryInvalid("na=" + _event + "() -> nb=" + _event + "(doublePrimitive in [na.intBoxed:na.boolBoxed])");
	        TryInvalid("na=" + _event + "() -> nb=" + _event + "(doublePrimitive in [na.xx:na.intPrimitive])");
	        TryInvalid("na=" + _event + "() -> nb=" + _event + "(doublePrimitive in [na.boolBoxed:na.intPrimitive])");
	    }

	    private void TryInvalid(String eqlInvalidPattern)
	    {
	        try
	        {
	            epService.EPAdministrator.CreatePattern(eqlInvalidPattern);
	            Assert.Fail();
	        }
	        catch (EPException ex)
	        {
	            // Expected exception
	        }
	    }

	    private String GetSyntaxExceptionPattern(String expression)
	    {
	        String exceptionText = null;
	        try
	        {
	            epService.EPAdministrator.CreatePattern(expression);
	            Assert.Fail();
	        }
	        catch (EPStatementSyntaxException ex)
	        {
	            exceptionText = ex.Message;
	            log.Debug(".getSyntaxExceptionPattern pattern=" + expression, ex);
	            // Expected exception
	        }

	        return exceptionText;
	    }

	    private String GetStatementExceptionPattern(String expression)
	    {
	        return GetStatementExceptionPattern(expression, false);
	    }

	    private String GetStatementExceptionPattern(String expression, bool isLogException)
	    {
	        String exceptionText = null;
	        try
	        {
	            epService.EPAdministrator.CreatePattern(expression);
	            Assert.Fail();
	        }
	        catch (EPStatementSyntaxException es)
	        {
	            throw;
	        }
	        catch (EPStatementException ex)
	        {
	            // Expected exception
	            exceptionText = ex.Message;
	            if (isLogException)
	            {
	                log.Debug(".getSyntaxExceptionPattern pattern=" + expression, ex);
	            }
	        }

	        return exceptionText;
	    }

	    private void TryValid(String eqlInvalidPattern)
	    {
	        epService.EPAdministrator.CreatePattern(eqlInvalidPattern);
	    }

        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	}
} // End of namespace
