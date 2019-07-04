using System;
using System.Text;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.parse;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Represents an stream property identifier in a filter expressiun tree.
    /// </summary>

    public class ExprIdentNode : ExprNode
    {
        // select myprop from...        is a simple property, no stream supplied
        // select s0.myprop from...     is a simple property with a stream supplied, or a nested property (cannot tell until resolved)
        // select indexed[1] from ...   is a indexed property

        private readonly String unresolvedPropertyName;
        private readonly String streamOrPropertyName;

        private String resolvedStreamName;
        private String resolvedPropertyName;
        private EventPropertyGetter propertyGetter;
        private int streamNum = -1;
        private Type propertyType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unresolvedPropertyName">the event property name in unresolved form, ie. unvalidated against streams</param>

        public ExprIdentNode(String unresolvedPropertyName)
        {
            if (unresolvedPropertyName == null)
            {
                throw new ArgumentException("Property name is null");
            }
            this.unresolvedPropertyName = unresolvedPropertyName;
            this.streamOrPropertyName = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unresolvedPropertyName">the event property name in unresolved form, ie. unvalidated against streams</param>
        /// <param name="streamOrPropertyName">the stream name, or if not a valid stream name a possible nested property name in one of the streams.</param>

        public ExprIdentNode(String unresolvedPropertyName, String streamOrPropertyName)
        {
            if (unresolvedPropertyName == null)
            {
                throw new ArgumentException("Property name is null");
            }
            if (streamOrPropertyName == null)
            {
                throw new ArgumentException("Stream (or property name) name is null");
            }
            this.unresolvedPropertyName = unresolvedPropertyName;
            this.streamOrPropertyName = streamOrPropertyName;
        }

        /// <summary>For unit testing, returns unresolved property name.</summary>
        /// <returns>property name</returns>

        public String UnresolvedPropertyName
        {
            get { return unresolvedPropertyName; }
        }

        /// <summary>For unit testing, returns stream or property name candidate.</summary>
        /// <returns>stream name, or property name of a nested property of one of the streams</returns>

        public String StreamOrPropertyName
        {
            get { return streamOrPropertyName; }
        }

        /// <summary>
        /// Returns the unresolved property name in it's complete form, including
        /// the stream name if there is one.
        /// </summary>
        /// <returns>property name</returns>
        public string FullUnresolvedName
        {
            get
            {
                if (streamOrPropertyName == null) {
                    return unresolvedPropertyName;
                } else {
                    return streamOrPropertyName + "." + unresolvedPropertyName;
                }
            }
        }

        /// <summary>
        /// Validate node.
        /// </summary>
        /// <param name="streamTypeService">serves stream event type info</param>
        /// <param name="methodResolutionService">for resolving class names in library method invocations</param>
        /// <param name="viewResourceDelegate">The view resource delegate.</param>
        /// <param name="timeProvider">provides engine current time</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <throws>ExprValidationException thrown when validation failed </throws>
        public override void Validate(StreamTypeService streamTypeService,
                                      MethodResolutionService methodResolutionService,
                                      ViewResourceDelegate viewResourceDelegate,
                                      TimeProvider timeProvider,
                                      VariableService variableService)
        {
            Pair<PropertyResolutionDescriptor, String> propertyInfoPair = GetTypeFromStream(streamTypeService, unresolvedPropertyName, streamOrPropertyName);
            resolvedStreamName = propertyInfoPair.Second;
            streamNum = propertyInfoPair.First.StreamNum;
            propertyType = TypeHelper.GetBoxedType(propertyInfoPair.First.PropertyType);
            resolvedPropertyName = propertyInfoPair.First.PropertyName;
            propertyGetter = propertyInfoPair.First.StreamEventType.GetGetter(resolvedPropertyName);
        }

        /// <summary>
        /// Returns the type that the node's evaluate method returns an instance of.
        /// </summary>
        /// <value>The type.</value>
        /// <returns> type returned when evaluated
        /// </returns>
        /// <throws>ExprValidationException thrown when validation failed </throws>
        public override Type ReturnType
        {
            get
            {
                if (resolvedPropertyName == null)
                {
                    throw new IllegalStateException("Identifier node has not been validated");
                }
                return propertyType;
            }
        }

        /// <summary>
        /// Returns true if the expression node's evaluation value doesn't depend on any events data,
        /// as must be determined at validation time, which is bottom-up and therefore
        /// reliably allows each node to determine constant value.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// true for constant evaluation value, false for non-constant evaluation value
        /// </returns>
		public override bool IsConstantResult
	    {
	        get { return false; }
	    }

        /// <summary>
        /// Gets the stream id supplying the property value
        /// </summary>

        public int StreamId
        {
            get
            {
                if (streamNum == -1)
                {
                    throw new IllegalStateException("Identifier node has not been validated");
                }
                return streamNum;
            }
        }

        /// <summary>
        /// Returns stream name as resolved by lookup of property in streams.
        /// </summary>

        public String ResolvedStreamName
        {
            get
            {
                if (resolvedStreamName == null)
                {
                    throw new IllegalStateException("Identifier node has not been validated");
                }
                return resolvedStreamName;
            }
        }

        /// <summary>
        /// Return property name as resolved by lookup in streams.
        /// </summary>

        public String ResolvedPropertyName
        {
            get
            {
                if (resolvedPropertyName == null)
                {
                    throw new IllegalStateException("Identifier node has not been validated");
                }
                return resolvedPropertyName;
            }
        }

        /// <summary>
        /// Determine stream id and property type given an unresolved property name and a
        /// stream name that may also be part of the property name.
        /// <para>
        /// For example: select s0.p1 from...    p1 is the property name, s0 the stream name, however this could also be a nested property
        /// </para>
        /// </summary>
        /// <param name="streamTypeService">service for type infos</param>
        /// <param name="unresolvedPropertyName">property name</param>
        /// <param name="streamOrPropertyName">stream name, this can also be the first part of the property name</param>
        /// <returns>pair of stream number and property type</returns>
        /// <throws>ExprValidationException if no such property exists</throws>

        protected static Pair<PropertyResolutionDescriptor, String> GetTypeFromStream(StreamTypeService streamTypeService, String unresolvedPropertyName, String streamOrPropertyName)
        {
            PropertyResolutionDescriptor propertyInfo = null;

            // no stream/property name supplied
            if (streamOrPropertyName == null)
            {
                try
                {
                    propertyInfo = streamTypeService.ResolveByPropertyName(unresolvedPropertyName);
                }
                catch (StreamTypesException ex)
                {
                    throw new ExprValidationException(ex.Message);
                }
                catch (PropertyAccessException ex)
                {
                    throw new ExprValidationException(ex.Message);
                }

                // resolves without a stream name, return descriptor and null stream name
                return new Pair<PropertyResolutionDescriptor, String>(propertyInfo, propertyInfo.StreamName);
            }

            // try to resolve the property name and stream name as it is (ie. stream name as a stream name)
            try
            {
                propertyInfo = streamTypeService.ResolveByStreamAndPropName(streamOrPropertyName, unresolvedPropertyName);
                // resolves with a stream name, return descriptor and stream name
                return new Pair<PropertyResolutionDescriptor, String>(propertyInfo, streamOrPropertyName);
            }
            catch (StreamTypesException)
            {
                // unhandled
            }

            // try to resolve the property name to a nested property 's0.p0'
            String propertyNameCandidate = streamOrPropertyName + "." + unresolvedPropertyName;
            try
            {
                propertyInfo = streamTypeService.ResolveByPropertyName(propertyNameCandidate);
                // resolves without a stream name, return null for stream name
                return new Pair<PropertyResolutionDescriptor, String>(propertyInfo, null);
            }
            catch (StreamTypesException)
            {
                // unhandled
            }

            // fail to resolve
            throw new ExprValidationException("Failed to resolve property '" + propertyNameCandidate + "' to a stream or nested property in a stream");
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return "unresolvedPropertyName=" + unresolvedPropertyName +
                    " streamOrPropertyName=" + streamOrPropertyName +
                    " resolvedPropertyName=" + resolvedPropertyName +
                    " propertyInfo.pos=" + streamNum +
                    " propertyInfo.type=" + propertyType;
        }

        /// <summary>
        /// Evaluate event tuple and return result.
        /// </summary>
        /// <param name="eventsPerStream">event tuple</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <returns>
        /// evaluation result, a bool value for OR/AND-type evalution nodes.
        /// </returns>
        public override Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
        {
            EventBean ev = eventsPerStream[streamNum];
            if (ev == null)
            {
                return null;
            }
            return propertyGetter.GetValue(ev);
        }

        /// <summary>
        /// Returns true if the property exists, or false if not.
        /// </summary>
        /// <param name="eventsPerStream">each stream's events</param>
        /// <param name="isNewData">if the stream represents insert or remove stream</param>
        /// <returns>true if the property exists, false if not</returns>

        public virtual bool EvaluatePropertyExists(EventBean[] eventsPerStream, bool isNewData)
        {
            EventBean @event = eventsPerStream[streamNum];
            if (@event == null)
            {
                return false;
            }
            return propertyGetter.IsExistsProperty(@event);
        }

        /// <summary>
        /// Returns the expression node rendered as a string.
        /// </summary>
        /// <value></value>
        /// <returns> string rendering of expression
        /// </returns>
        public override String ExpressionString
        {
            get
            {
                StringBuilder buffer = new StringBuilder();
                if (streamOrPropertyName != null) {
                    buffer.Append(ASTFilterSpecHelper.UnescapeDot(streamOrPropertyName));
                    buffer.Append('.');
                }

                buffer.Append(ASTFilterSpecHelper.UnescapeDot(unresolvedPropertyName));
                return buffer.ToString();
            }
        }

        /// <summary>
        /// Return true if a expression node semantically equals the current node, or false if not.
        /// Concrete implementations should compare the type and any additional information
        /// that impact the evaluation of a node.
        /// </summary>
        /// <param name="node">to compare to</param>
        /// <returns>
        /// true if semantically equal, or false if not equals
        /// </returns>
        public override bool EqualsNode(ExprNode node)
        {
            if (!(node is ExprIdentNode))
            {
                return false;
            }

            ExprIdentNode other = (ExprIdentNode)node;

            if (this.streamNum == -1)
            {
                throw new IllegalStateException("ExprIdentNode has not been validated");
            }

            if ((other.streamNum != this.streamNum) ||
                (!(other.resolvedPropertyName.Equals(this.resolvedPropertyName))))
            {
                return false;
            }


            return true;
        }
    }
}