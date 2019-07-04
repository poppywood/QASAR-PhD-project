///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.epl.core;
using com.espertech.esper.epl.join;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.variable;
using com.espertech.esper.epl.view;
using com.espertech.esper.events;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.filter;
using com.espertech.esper.pattern;
using com.espertech.esper.schedule;
using com.espertech.esper.view;

namespace com.espertech.esper.core
{
	/// <summary>
	/// Contains handles to the implementation of the the scheduling service for use in view evaluation.
	/// </summary>
    public sealed class StatementContext
    {
        private readonly String engineURI;
        private readonly String engineInstanceId;
        private readonly String statementId;
        private readonly String statementName;
        private readonly String expression;
        private readonly SchedulingService schedulingService;
        private readonly ScheduleBucket scheduleBucket;
        private readonly EventAdapterService eventAdapterService;
        private readonly EPStatementHandle epStatementHandle;
        private readonly ViewResolutionService viewResolutionService;
        private readonly PatternObjectResolutionService patternResolutionService;
        private readonly StatementExtensionSvcContext statementExtensionSvcContext;
        private readonly StatementStopService statementStopService;
        private readonly MethodResolutionService methodResolutionService;
        private readonly PatternContextFactory patternContextFactory;
        private readonly FilterService filterService;
        private readonly JoinSetComposerFactory joinSetComposerFactory;
        private readonly OutputConditionFactory outputConditionFactory;
        private readonly NamedWindowService namedWindowService;
        private readonly VariableService variableService;
        private readonly StatementResultService statementResultService;
        private readonly IList<Uri> plugInTypeResolutionURIs;
        private readonly ValueAddEventService valueAddEventService;

	    /// <summary>Constructor.</summary>
	    /// <param name="engineURI">is the engine URI</param>
	    /// <param name="engineInstanceId">is the name of the engine instance</param>
	    /// <param name="statementId">is the statement is assigned for the statement for which this context exists</param>
	    /// <param name="statementName">is the statement name</param>
	    /// <param name="expression">is the EPL or pattern expression used</param>
	    /// <param name="schedulingService">implementation for schedule registration</param>
	    /// <param name="scheduleBucket">is for ordering scheduled callbacks within the view statements</param>
	    /// <param name="eventAdapterService">service for generating events and handling event types</param>
	    /// <param name="epStatementHandle">is the statements-own handle for use in registering callbacks with services</param>
	    /// <param name="viewResultionService">is a service for resolving view namespace and name to a view factory</param>
	    /// <param name="statementExtensionSvcContext">provide extension points for custom statement resources</param>
	    /// <param name="statementStopService">for registering a callback invoked when a statement is stopped</param>
	    /// <param name="methodResolutionService">is a service for resolving static methods and aggregation functions</param>
	    /// <param name="patternContextFactory">is the pattern-level services and context information factory</param>
	    /// <param name="filterService">is the filtering service</param>
	    /// <param name="patternResolutionService">is the service that resolves pattern objects for the statement</param>
	    /// <param name="joinSetComposerFactory">is the factory for creating service objects that compose join results</param>
	    /// <param name="outputConditionFactory">is the factory for output condition objects</param>
	    /// <param name="namedWindowService">is holding information about the named windows active in the system</param>
	    /// <param name="variableService">provides access to variable values</param>
	    /// <param name="statementResultService">handles awareness of listeners/subscriptions for a statement customizing output produced</param>
	    /// <param name="plugInTypeResolutionURIs">is URIs for resolving the event name against plug-inn event representations, if any</param>
	    /// <param name="valueAddEventService">service that handles update events</param>
	    public StatementContext(String engineURI,
	                            String engineInstanceId,
	                            String statementId,
	                            String statementName,
	                            String expression,
	                            SchedulingService schedulingService,
	                            ScheduleBucket scheduleBucket,
	                            EventAdapterService eventAdapterService,
	                            EPStatementHandle epStatementHandle,
	                            ViewResolutionService viewResultionService,
	                            PatternObjectResolutionService patternResolutionService,
	                            StatementExtensionSvcContext statementExtensionSvcContext,
	                            StatementStopService statementStopService,
	                            MethodResolutionService methodResolutionService,
	                            PatternContextFactory patternContextFactory,
	                            FilterService filterService,
	                            JoinSetComposerFactory joinSetComposerFactory,
	                            OutputConditionFactory outputConditionFactory,
	                            NamedWindowService namedWindowService,
	                            VariableService variableService,
	                            StatementResultService statementResultService,
	                            IList<Uri> plugInTypeResolutionURIs,
	                            ValueAddEventService valueAddEventService)
        {
            this.engineURI = engineURI;
            this.engineInstanceId = engineInstanceId;
            this.statementId = statementId;
            this.statementName = statementName;
            this.expression = expression;
            this.schedulingService = schedulingService;
            this.eventAdapterService = eventAdapterService;
            this.scheduleBucket = scheduleBucket;
            this.epStatementHandle = epStatementHandle;
            this.viewResolutionService = viewResultionService;
            this.patternResolutionService = patternResolutionService;
            this.statementExtensionSvcContext = statementExtensionSvcContext;
            this.statementStopService = statementStopService;
            this.methodResolutionService = methodResolutionService;
            this.patternContextFactory = patternContextFactory;
            this.filterService = filterService;
            this.joinSetComposerFactory = joinSetComposerFactory;
            this.outputConditionFactory = outputConditionFactory;
            this.namedWindowService = namedWindowService;
            this.variableService = variableService;
            this.statementResultService = statementResultService;
            this.plugInTypeResolutionURIs = plugInTypeResolutionURIs;
            this.valueAddEventService = valueAddEventService;
        }

        /// <summary>Returns the statement id.</summary>
        /// <returns>statement id</returns>
        public String StatementId
        {
            get { return statementId; }
        }

        /// <summary>Returns the statement name</summary>
        /// <returns>statement name</returns>
        public String StatementName
        {
            get { return statementName; }
        }

        /// <summary>Returns service to use for schedule evaluation.</summary>
        /// <returns>schedule evaluation service implemetation</returns>
        public SchedulingService SchedulingService
        {
            get { return schedulingService; }
        }

        /// <summary>Returns service for generating events and handling event types.</summary>
        /// <returns>event adapter service</returns>
        public EventAdapterService EventAdapterService
        {
            get { return eventAdapterService; }
        }

        /// <summary>
        /// Returns the schedule bucket for ordering schedule callbacks within this pattern.
        /// </summary>
        /// <returns>schedule bucket</returns>
        public ScheduleBucket ScheduleBucket
        {
            get { return scheduleBucket; }
        }

        /// <summary>Returns the statement's resource locks.</summary>
        /// <returns>statement resource lock/handle</returns>
        public EPStatementHandle EpStatementHandle
        {
            get { return epStatementHandle; }
        }

        /// <summary>Returns view resolution svc.</summary>
        /// <returns>view resolution</returns>
        public ViewResolutionService ViewResolutionService
        {
            get { return viewResolutionService; }
        }

        /// <summary>Returns extension context for statements.</summary>
        /// <returns>context</returns>
        public StatementExtensionSvcContext ExtensionServicesContext
        {
            get { return statementExtensionSvcContext; }
        }

        /// <summary>Returns statement stop subscription taker.</summary>
        /// <returns>stop service</returns>
        public StatementStopService StatementStopService
        {
            get { return statementStopService; }
        }

        /// <summary>
        /// Returns service to look up static and aggregation methods or functions.
        /// </summary>
        /// <returns>method resolution</returns>
        public MethodResolutionService MethodResolutionService
        {
            get { return methodResolutionService; }
        }

        /// <summary>Returns the pattern context factory for the statement.</summary>
        /// <returns>pattern context factory</returns>
        public PatternContextFactory PatternContextFactory
        {
            get { return patternContextFactory; }
        }

        /// <summary>Returns the statement expression text</summary>
        /// <returns>expression text</returns>
        public String Expression
        {
            get { return expression; }
        }

        /// <summary>Returns the engine URI.</summary>
        /// <returns>engine URI</returns>
        public String EngineURI
        {
            get { return engineURI; }
        }

        /// <summary>Returns the engine instance id.</summary>
        /// <returns>instance id</returns>
        public String EngineInstanceId
        {
            get { return engineInstanceId; }
        }

        /// <summary>Returns the filter service.</summary>
        /// <returns>filter service</returns>
        public FilterService FilterService
        {
            get { return filterService; }
        }

        /// <summary>
        /// Gets the join set composer factory.
        /// </summary>
        /// <value>The join set composer factory.</value>
        public JoinSetComposerFactory JoinSetComposerFactory
        {
            get { return joinSetComposerFactory; }
        }

        /// <summary>
        /// Gets the output condition factory.
        /// </summary>
        /// <value>The output condition factory.</value>
        public OutputConditionFactory OutputConditionFactory
        {
            get { return outputConditionFactory; }
        }

        /// <summary>
        /// Gets the pattern resolution service.
        /// </summary>
        /// <value>The pattern resolution service.</value>
        public PatternObjectResolutionService PatternResolutionService
        {
            get { return patternResolutionService; }
        }

        /// <summary>Returns the named window management service.</summary>
        /// <returns>service for managing named windows</returns>
        public NamedWindowService NamedWindowService
        {
            get { return namedWindowService; }
        }

        /// <summary>Returns variable service.</summary>
        /// <returns>variable service</returns>
        public VariableService VariableService
        {
            get { return variableService; }
        }

        /// <summary>
        /// Returns the service that handles awareness of listeners/subscriptions for a statement
        /// customizing output produced
        /// </summary>
        /// <returns>statement result svc</returns>
        public StatementResultService StatementResultService
        {
            get { return statementResultService; }
        }

        /// <summary>Returns the URIs for resolving the event name against plug-inn event representations, if any</summary>
        /// <returns>URIs</returns>
	    public IList<Uri> PlugInTypeResolutionURIs
	    {
	        get { return plugInTypeResolutionURIs; }
	    }

	    /// <summary>Returns the update event service.</summary>
	    /// <returns>revision service</returns>
	    public ValueAddEventService ValueAddEventService
	    {
	        get { return valueAddEventService; }
	    }

	    /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return " stmtId=" + statementId +
                   " stmtName=" + statementName;
        }
    }
} // End of namespace
