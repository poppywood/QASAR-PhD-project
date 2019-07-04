///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

using NUnit.Framework;

using net.esper.eql.core;
using net.esper.util;

namespace net.esper.eql.expression
{
	[TestFixture]
	public class TestExprStaticMethodNode
	{
	    StaticMethodResolver staticMethodResolver;
	    StreamTypeService streamTypeService;
	    MethodResolutionService methodResolutionService;
	    ExprNode intThree;
	    ExprNode intFive;
	    ExprNode shortNine;
	    ExprNode doubleFour;
	    ExprNode doubleEight;
	    ExprNode stringTen;
	    MethodInfo maxInt;
	    MethodInfo maxDouble;

        [SetUp]
	    protected void SetUp()
	    {
	        streamTypeService = null;
	        EngineImportService engineImportService = new EngineImportServiceImpl();
	        engineImportService.AddImport("System");
	        methodResolutionService = new MethodResolutionServiceImpl(engineImportService);
	        staticMethodResolver = new StaticMethodResolver();
	        intThree = new ExprConstantNode(3);
	        intFive = new ExprConstantNode(5);
	        short nine = 9;
	        shortNine = new ExprConstantNode(nine);
	        doubleFour = new ExprConstantNode(4d);
	        doubleEight = new ExprConstantNode(8d);
	        stringTen = new ExprConstantNode("10");
	        maxInt = typeof (Math).GetMethod("Max", new Type[] {typeof (int), typeof (int)});
	        maxDouble = typeof (Math).GetMethod("Max", new Type[] {typeof (double), typeof (double)});
	    }

	    [Test]
	    public void testMaxIntInt()
	    {
            ExprStaticMethodNode root = new ExprStaticMethodNode("Math", "Max");
	        root.AddChildNode(intThree);
	        root.AddChildNode(intFive);
	        Validate(root);

	        Assert.AreEqual(maxInt, root.StaticMethod);
	        int result = Math.Max(3,5);
	        Assert.AreEqual(result, root.Evaluate(null, false));
	    }

	    [Test]
	    public void testIntegerInt()
	    {
            MethodInfo staticMethod = this.GetType().GetMethod("StaticIntMethod", new Type[] { typeof(int) });
	        ExprStaticMethodNode parent = new ExprStaticMethodNode(this.GetType().FullName, "StaticIntMethod");
	        ExprNode child = new ExprStaticMethodNode("Math", "Max");
	        child.AddChildNode(intThree);
	        child.AddChildNode(intFive);
	        parent.AddChildNode(child);
	        Validate(parent);

	        Assert.AreEqual(staticMethod, parent.StaticMethod);
	        int result = Math.Max(3, 5);
	        Assert.AreEqual(result, parent.Evaluate(null, false));
	    }

	    [Test]
	    public void testMaxIntShort()
	    {
	        ExprStaticMethodNode root = new ExprStaticMethodNode("Math", "Max");
	        root.AddChildNode(intThree);
	        root.AddChildNode(shortNine);
	        Validate(root);

	        Assert.AreEqual(maxInt, root.StaticMethod);
	        short nine = 9;
	        int result = Math.Max(3,(int) nine);
	        Assert.AreEqual(result, root.Evaluate(null, false));
	    }

	    [Test]
	    public void testMaxDoubleInt()
	    {
	        ExprStaticMethodNode root = new ExprStaticMethodNode("Math", "Max");
	        root.AddChildNode(doubleEight);
	        root.AddChildNode(intFive);
	        Validate(root);

	        Assert.AreEqual(maxDouble, root.StaticMethod);
	        double? result = Math.Max(8d,5);
	        Assert.AreEqual(result, root.Evaluate(null, false));
	    }

	    [Test]
	    public void testMaxDoubleDouble()
	    {
	        ExprStaticMethodNode root = new ExprStaticMethodNode("Math", "Max");
	        root.AddChildNode(doubleEight);
	        root.AddChildNode(doubleFour);
	        Validate(root);

	        Assert.AreEqual(maxDouble, root.StaticMethod);
	        double? result = Math.Max(8d,4d);
	        Assert.AreEqual(result, root.Evaluate(null, false));
	    }

	    [Test]
	    public void testPowDoubleDouble()
	    {
	        MethodInfo pow = typeof (Math).GetMethod("Pow", new Type[] {typeof (double), typeof (double)});
	        ExprStaticMethodNode root = new ExprStaticMethodNode("Math", "Pow");
	        root.AddChildNode(doubleEight);
	        root.AddChildNode(doubleFour);
	        Validate(root);

	        Assert.AreEqual(pow, root.StaticMethod);
	        double? result = Math.Pow(8d,4d);
	        Assert.AreEqual(result, root.Evaluate(null, false));
	    }

        //[Test]
        //public void testValueOfInt()
        //{
        //    MethodInfo valueOf = typeof(int?).GetMethod("valueOf", typeof(String));
        //    ExprStaticMethodNode root = new ExprStaticMethodNode("Integer", "valueOf");
        //    root.AddChildNode(stringTen);
        //    Validate(root);

        //    Assert.AreEqual(valueOf, root.StaticMethod);
        //    int result = 10;
        //    Assert.AreEqual(result, root.Evaluate(null, false));
        //}

	    private void Validate(ExprNode node)
	    {
	        node.GetValidatedSubtree(streamTypeService, methodResolutionService, null);
	    }

	    public void NonstaticMethod()
	    {
	    }

	    public static void StaticMethod()
	    {
	    }

	    public static int StaticIntMethod(int param)
	    {
	    	return param;
	    }
	}
} // End of namespace