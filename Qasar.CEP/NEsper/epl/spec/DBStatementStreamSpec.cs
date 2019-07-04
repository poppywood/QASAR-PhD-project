using System;
using System.Collections.Generic;
using com.espertech.esper.compat;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.spec;
using com.espertech.esper.epl.variable;
using com.espertech.esper.events;
using com.espertech.esper.epl.core;
using com.espertech.esper.events.vaevent;
using com.espertech.esper.pattern;
using com.espertech.esper.schedule;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Specification object for historical data poll via database SQL statement.
	/// </summary>
	public class DBStatementStreamSpec
		: StreamSpecBase
		, StreamSpecRaw
		, StreamSpecCompiled
		, MetaDefItem
	{
		/// <summary> Returns the database name.</summary>
		/// <returns> name of database.
		/// </returns>
		
        public String DatabaseName
		{
			get { return databaseName; }
		}

		/// <summary> Returns the SQL with substitution parameters.</summary>
		/// <returns> SQL with parameters embedded as ${stream.param}
		/// </returns>
		
        public String SqlWithSubsParams
		{
			get { return sqlWithSubsParams; }
		}

        /// <summary>
        /// Returns the optional sample metadata SQL
        /// </summary>
        /// <returns>null if not supplied, or SQL to fire to retrieve metadata</returns>

        public String MetadataSQL
        {
            get { return metadataSQL; }
        }

        private readonly String databaseName;
        private readonly String sqlWithSubsParams;
        private readonly String metadataSQL;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="optionalStreamName">is a stream name optionally given to stream</param>
        /// <param name="viewSpecs">is a list of views onto the stream</param>
        /// <param name="databaseName">is the database name to poll</param>
        /// <param name="sqlWithSubsParams">is the SQL with placeholder parameters</param>
        /// <param name="metadataSQL">The metadata SQL.</param>
	    public DBStatementStreamSpec(String optionalStreamName,
	                                 IEnumerable<ViewSpec> viewSpecs,
	                                 String databaseName,
	                                 String sqlWithSubsParams,
	                                 String metadataSQL)
            : base(optionalStreamName, viewSpecs, false)
        {
            this.databaseName = databaseName;
            this.sqlWithSubsParams = sqlWithSubsParams;
            this.metadataSQL = metadataSQL;
        }

	    public StreamSpecCompiled Compile(EventAdapterService eventAdapterService,
                                          MethodResolutionService methodResolutionService,
                                          PatternObjectResolutionService patternObjectResolutionService,
                                          TimeProvider timeProvider)
        {
            return this;
        }


        /// <summary>
        /// Compiles a raw stream specification consisting event type information and filter expressionsto an validated, optimized form for use with filter service
        /// </summary>
        /// <param name="eventAdapterService">supplies type information</param>
        /// <param name="methodResolutionService">for resolving imports</param>
        /// <param name="patternObjectResolutionService">for resolving pattern objects</param>
        /// <param name="timeProvider">provides engine current time</param>
        /// <param name="namedWindowService">is the service managing named windows</param>
        /// <param name="valueAddEventService">The value add event service.</param>
        /// <param name="variableService">provides variable values</param>
        /// <param name="engineURI">The engine URI.</param>
        /// <param name="plugInTypeResolutionURIs">The plug in type resolution UR is.</param>
        /// <returns>compiled stream</returns>
        /// <throws>ExprValidationException to indicate validation errors</throws>
	    public StreamSpecCompiled Compile(EventAdapterService eventAdapterService, MethodResolutionService methodResolutionService, PatternObjectResolutionService patternObjectResolutionService, TimeProvider timeProvider, NamedWindowService namedWindowService, ValueAddEventService valueAddEventService, VariableService variableService, string engineURI, IList<Uri> plugInTypeResolutionURIs)
        {
            return this;
        }
	}
}