using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using com.espertech.esper.compat;
using com.espertech.esper.epl.agg;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

using log4net;

namespace com.espertech.esper.epl.expression
{
    /// <summary> Superclass for filter nodes in a filter expression tree. Allow
    /// validation against stream event types and evaluation of events against filter tree.
    /// </summary>

    public abstract class ExprNode
		: ExprValidator
		, ExprEvaluator
		, MetaDefItem
    {
        private readonly IList<ExprNode> childNodes;

        /// <summary>
        /// Returns the type that the node's evaluate method returns an instance of.
        /// </summary>
        /// <value>The type.</value>
        /// <returns> type returned when evaluated
        /// </returns>
        /// <throws>ExprValidationException thrown when validation failed </throws>

        public abstract Type ReturnType
        {
            get;
        }

        /// <summary> Returns the expression node rendered as a string.</summary>
        /// <returns> string rendering of expression
        /// </returns>

        public abstract String ExpressionString
        {
            get;
        }

	    /// <summary>
	    /// Returns true if the expression node's evaluation value doesn't depend on any events data,
	    /// as must be determined at validation time, which is bottom-up and therefore
	    /// reliably allows each node to determine constant value.
	    /// </summary>
	    /// <returns>
	    /// true for constant evaluation value, false for non-constant evaluation value
	    /// </returns>
	    public abstract bool IsConstantResult
		{
			get;
		}

		/// <summary> Returns list of child nodes.</summary>
		/// <returns> list of child nodes
		/// </returns>

		public IList<ExprNode> ChildNodes
		{
			get { return childNodes; }
		}

		/// <summary>
        /// Return true if a expression node semantically equals the current node, or false if not.
        ///
        /// Concrete implementations should compare the type and any additional information
        /// that impact the evaluation of a node.
        /// </summary>
        /// <param name="node">to compare to
        /// </param>
        /// <returns> true if semantically equal, or false if not equals
        /// </returns>
        public abstract bool EqualsNode(ExprNode node);

        /// <summary>
        /// Constructor creates a list of child nodes.
        /// </summary>

        protected ExprNode()
        {
            childNodes = new List<ExprNode>();
        }

        /// <summary>
        /// Validates the expression node subtree that has this
        /// node as root. Some of the nodes of the tree, including the
        /// root, might be replaced in the process.
        /// </summary>
        /// <param name="streamTypeService">serves stream type information</param>
        /// <param name="methodResolutionService">for resolving class names in library method invocations</param>
        /// <param name="viewResourceDelegate">delegates for view resources to expression nodes</param>
        /// <param name="timeProvider">provides engine current time</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <returns>
        /// the root node of the validated subtree, possibly
        /// different than the root node of the unvalidated subtree
        /// </returns>
        /// <throws>ExprValidationException when the validation fails</throws>
        public virtual ExprNode GetValidatedSubtree(StreamTypeService streamTypeService,
                                                    MethodResolutionService methodResolutionService,
                                                    ViewResourceDelegate viewResourceDelegate,
                                                    TimeProvider timeProvider,
                                                    VariableService variableService)
        {
            ExprNode result = this;

            IList<ExprNode> nodes = childNodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i] = nodes[i].GetValidatedSubtree(streamTypeService, methodResolutionService, viewResourceDelegate, timeProvider, variableService);
            }

            try
            {
                Validate(streamTypeService, methodResolutionService, viewResourceDelegate, timeProvider, variableService);
            }
            catch(ExprValidationException e)
            {
                if (this is ExprIdentNode)
                {
                    ExprIdentNode identNode = (ExprIdentNode)this;
                    try
                    {
                        result = ResolveStaticMethodOrField(identNode, streamTypeService, methodResolutionService, e, timeProvider, variableService);
                    }
                    catch (ExprValidationException)
                    {
                        result = ResolveAsStreamName(identNode, streamTypeService, e);
                    }
                }
                else if (this is ExprStaticMethodNode)
                {
                    ExprStaticMethodNode staticMethodNode = (ExprStaticMethodNode) this;
                    result = ResolveInstanceMethod(staticMethodNode, streamTypeService, methodResolutionService, e);
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

        private static ExprNode ResolveInstanceMethod(
            ExprStaticMethodNode staticMethodNode, 
            StreamTypeService streamTypeService, 
            MethodResolutionService methodResolutionService, 
            ExprValidationException existingException)
        {
            String streamName = staticMethodNode.ClassName;

            bool streamFound = false;
            foreach (String name in streamTypeService.StreamNames)
            {
                if (name == streamName)
                {
                    streamFound = true;
                }
            }

            String methodName = staticMethodNode.MethodName;
            ExprStreamInstanceMethodNode exprStream = new ExprStreamInstanceMethodNode(streamName, methodName);
            foreach (ExprNode childNode in staticMethodNode.ChildNodes)
            {
                exprStream.AddChildNode(childNode);
            }

            try
            {
                exprStream.Validate(streamTypeService, methodResolutionService, null, null, null);
            }
            catch (ExprValidationException)
            {
                if (streamFound)
                {
                    throw;
                }
                throw existingException;
            }

            return exprStream;
        }

        private static ExprNode ResolveAsStreamName(
            ExprIdentNode identNode,
            StreamTypeService streamTypeService,
            ExprValidationException existingException)
        {
            ExprStreamUnderlyingNode exprStream = new ExprStreamUnderlyingNode(identNode.UnresolvedPropertyName);

            try
            {
                exprStream.Validate(streamTypeService, null, null, null, null);
            }
            catch (ExprValidationException)
            {
                throw existingException;
            }

            return exprStream;
        }

        /// <summary>
        /// Accept the visitor. The visitor will first visit the parent then visit all child nodes, then their child nodes.
        ///
        /// The visitor can decide to skip child nodes by returning false in isVisit.
        /// </summary>
        /// <param name="visitor">to visit each node and each child node.
        /// </param>
        public virtual void Accept(ExprNodeVisitor visitor)
        {
            if (visitor.IsVisit(this))
            {
                visitor.Visit(this);

                foreach (ExprNode childNode in childNodes)
                {
                    childNode.Accept(visitor);
                }
            }
        }

        /// <summary>
        /// Adds a child node.
        /// </summary>
        /// <param name="childNode">is the child evaluation tree node to add
        /// </param>
        public void AddChildNode(ExprNode childNode)
        {
            if (childNode != null)
            {
                childNodes.Add(childNode);
            }
        }

        /// <summary> Recursively print out all nodes.</summary>
        /// <param name="prefix">is printed out for naming the printed info
        /// </param>
        public void DumpDebug(String prefix)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".DumpDebug " + prefix + this);
            }

            foreach (ExprNode node in childNodes)
            {
                node.DumpDebug(prefix + "  ");
            }
        }

        /// <summary> Compare two expression nodes and their children in exact child-node sequence,
        /// returning true if the 2 expression nodes trees are equals, or false if they are not equals.
        ///
        /// Recursive call since it uses this method to compare child nodes in the same exact sequence.
        /// Nodes are compared using the EqualsNode method.
        /// </summary>
        /// <param name="nodeOne">first expression top node of the tree to compare
        /// </param>
        /// <param name="nodeTwo">second expression top node of the tree to compare
        /// </param>
        /// <returns> false if this or all child nodes are not equal, true if equal
        /// </returns>

        public static bool DeepEquals(ExprNode nodeOne, ExprNode nodeTwo)
        {
            if (nodeOne.childNodes.Count != nodeTwo.childNodes.Count)
            {
                return false;
            }
            if (!nodeOne.EqualsNode(nodeTwo))
            {
                return false;
            }
            for (int i = 0; i < nodeOne.childNodes.Count; i++)
            {
                ExprNode childNodeOne = nodeOne.childNodes[i];
                ExprNode childNodeTwo = nodeTwo.childNodes[i];

                if (!ExprNode.DeepEquals(childNodeOne, childNodeTwo))
                {
                    return false;
                }
            }
            return true;
        }

	    // Since static method calls such as "Class.method('a')" and mapped properties "Stream.property('key')"
	    // look the same, however as the validation could not resolve "Stream.property('key')" before calling this method,
	    // this method tries to resolve the mapped property as a static method.
	    // Assumes that this is an ExprIdentNode.
        private ExprNode ResolveStaticMethodOrField(
            ExprIdentNode identNode,
            StreamTypeService streamTypeService, 
            MethodResolutionService methodResolutionService, 
            ExprValidationException propertyException, 
            TimeProvider timeProvider,
            VariableService variableService)
        {
	        // Reconstruct the original string
	        StringBuilder mappedProperty = new StringBuilder(identNode.UnresolvedPropertyName);
	        if(identNode.StreamOrPropertyName != null)
	        {
				mappedProperty.Insert(0, '.');
	            mappedProperty.Insert(0, identNode.StreamOrPropertyName);
	        }

	        // Parse the mapped property format into a class name, method and single string parameter
	        MappedPropertyParseResult parse = ParseMappedProperty(mappedProperty.ToString());
	        if (parse == null)
	        {
                ExprConstantNode constNode = ResolveIdentAsEnumConst(mappedProperty.ToString(), methodResolutionService);
                if (constNode == null)
                {
                    throw propertyException;
                }
                else
                {
                    return constNode;
                }
	        }

	        // If there is a type name, assume a static method is possible
	        if (parse.TypeName != null)
	        {
	            ExprNode result = new ExprStaticMethodNode(parse.TypeName, parse.MethodName);
	            result.AddChildNode(new ExprConstantNode(parse.ArgString));

	            // Validate
	            try
	            {
                    result.Validate(streamTypeService, methodResolutionService, null, timeProvider, variableService);
	            }
	            catch(ExprValidationException)
	            {
	                throw new ExprValidationException("Failed to resolve " + mappedProperty + " as either an event property or as a static method invocation");
	            }

	            return result;
	        }

	        // There is no class name, try an aggregation function
	        try
	        {
	            AggregationSupport aggregation = methodResolutionService.ResolveAggregation(parse.MethodName);
	            ExprNode result = new ExprPlugInAggFunctionNode(false, aggregation, parse.MethodName);
	            result.AddChildNode(new ExprConstantNode(parse.ArgString));

	            // Validate
	            try
	            {
                    result.Validate(streamTypeService, methodResolutionService, null, timeProvider, variableService);
	            }
	            catch (Exception e)
	            {
	                throw new ExprValidationException("Plug-in aggregation function '" + parse.MethodName + "' failed validation: " + e.Message);
	            }

	            return result;
	        }
	        catch (EngineImportUndefinedException)
	        {
	            // Not an aggregation function
	        }
	        catch (EngineImportException e)
	        {
	            throw new IllegalStateException("Error resolving aggregation: " + e.Message, e);
	        }

	        // absolutly cannot be resolved
	        throw propertyException;
	    }

        private ExprConstantNode ResolveIdentAsEnumConst(String constant, MethodResolutionService methodResolutionService)
        {
            int lastDotIndex = constant.LastIndexOf('.');
            if (lastDotIndex == -1)
            {
                return null;
            }
            String className = constant.Substring(0, lastDotIndex);
            String constName = constant.Substring(lastDotIndex + 1);

            Type type;
            try
            {
                type = methodResolutionService.ResolveClass(className);
            }
            catch (EngineImportException)
            {
                return null;
            }

            FieldInfo field = type.GetField(constName);
            if (field == null)
            {
                return null;
            }

            if (field.IsPublic && field.IsStatic)
            {
                try
                {
                    Object value = field.GetValue(null);
                    return new ExprConstantNode(value);
                }
                catch (TargetInvocationException e)
                {
                    throw new ExprValidationException(
                        "Exception accessing field '" + field.Name + "': " + e.Message, e);
                }
            }

            return null;
        }

        /// <summary>
	    /// Parse the mapped property into classname, method and string argument.
	    /// Mind this has been parsed already and is a valid mapped property.
	    /// </summary>
	    /// <param name="property">
	    /// is the string property to be passed as a static method invocation
	    /// </param>
	    /// <returns>descriptor object</returns>
	    public static MappedPropertyParseResult ParseMappedProperty(String property)
	    {
	        // get argument
	        int indexFirstDoubleQuote = property.IndexOf("\"");
	        int indexFirstSingleQuote = property.IndexOf("'");
	        int startArg;
	        if ((indexFirstSingleQuote == -1) && (indexFirstDoubleQuote == -1))
	        {
	            return null;
	        }
	        if ((indexFirstSingleQuote != -1) && (indexFirstDoubleQuote != -1))
	        {
	            if (indexFirstSingleQuote < indexFirstDoubleQuote)
	            {
	                startArg = indexFirstSingleQuote;
	            }
	            else
	            {
	                startArg = indexFirstDoubleQuote;
	            }
	        }
	        else if (indexFirstSingleQuote != -1)
	        {
	            startArg = indexFirstSingleQuote;
	        }
	        else
	        {
	            startArg = indexFirstDoubleQuote;
	        }

	        int indexLastDoubleQuote = property.LastIndexOf("\"");
	        int indexLastSingleQuote = property.LastIndexOf("'");
	        int endArg;
	        if ((indexLastSingleQuote == -1) && (indexLastDoubleQuote == -1))
	        {
	            return null;
	        }
	        if ((indexLastSingleQuote != -1) && (indexLastDoubleQuote != -1))
	        {
	            if (indexLastSingleQuote > indexLastDoubleQuote)
	            {
	                endArg = indexLastSingleQuote;
	            }
	            else
	            {
	                endArg = indexLastDoubleQuote;
	            }
	        }
	        else if (indexLastSingleQuote != -1)
	        {
	            endArg = indexLastSingleQuote;
	        }
	        else
	        {
	            endArg = indexLastDoubleQuote;
	        }
	        String argument = property.Substring(startArg + 1, endArg - startArg - 1);

	        // get method
	        //String[] splitDots = property.ToString().Split("[\\.]");
            String[] splitDots = property.Split('.');
	        if (splitDots.Length == 0)
	        {
	            return null;
	        }

	        String method = splitDots[splitDots.Length - 1];
	        int indexParan = method.IndexOf("(");
	        if (indexParan == -1)
	        {
	            return null;
	        }
	        method = method.Substring(0, indexParan);
	        if (method.Length == 0)
	        {
	            return null;
	        }

	        if (splitDots.Length == 1)
	        {
	            // no type name
	            return new MappedPropertyParseResult(null, method, argument);
	        }

	        // get type
	        StringBuilder type = new StringBuilder();
	        for (int i = 0; i < splitDots.Length - 1; i++)
	        {
	            if (i > 0)
	            {
	                type.Append('.');
	            }
	            type.Append(splitDots[i]);
	        }

	        return new MappedPropertyParseResult(type.ToString(), method, argument);
	    }

	    /// <summary>
	    /// Encapsulates the parse result parsing a mapped property as a type and method name with args.
	    /// </summary>
	    public class MappedPropertyParseResult
	    {
	        private String typeName;
	        private String methodName;
	        private String argString;

	        /// <summary>Returns class name.</summary>
	        /// <returns>name of class</returns>
	        public String TypeName
	        {
	            get { return typeName; }
	        }

	        /// <summary>Returns the method name.</summary>
	        /// <returns>method name</returns>
	        public String MethodName
	        {
	            get { return methodName; }
	        }

	        /// <summary>Returns the method argument.</summary>
	        /// <returns>arg</returns>
	        public String ArgString
	        {
	            get { return argString; }
	        }

	        /// <summary>Returns the parse result of the mapped property.</summary>
	        /// <param name="typeName">is the type name, or null if there isn't one</param>
	        /// <param name="methodName">is the method name</param>
	        /// <param name="argString">is the argument</param>
	        public MappedPropertyParseResult(String typeName, String methodName, String argString)
	        {
	            this.typeName = typeName;
	            this.methodName = methodName;
	            this.argString = argString;
	        }
		}

        /// <summary>
        /// Evaluate event tuple and return result.
        /// </summary>
        /// <param name="eventsPerStream">event tuple</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <returns>
        /// evaluation result, a bool value for OR/AND-type evalution nodes.
        /// </returns>

        public abstract Object Evaluate(EventBean[] eventsPerStream, bool isNewData);

        /// <summary>
        /// Validate node.
        /// </summary>
        /// <param name="streamTypeService">serves stream event type info</param>
        /// <param name="methodResolutionService">for resolving class names in library method invocations</param>
        /// <param name="viewResourceDelegate">delegates for view resources to expression nodes</param>
        /// <param name="timeProvider">provides engine current time</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <throws>ExprValidationException thrown when validation failed </throws>
        public abstract void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService);

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
