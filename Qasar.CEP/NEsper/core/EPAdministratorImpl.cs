using System;
using System.Collections.Generic;

using Antlr.Runtime.Tree;

using com.espertech.esper.antlr;
using com.espertech.esper.client;
using com.espertech.esper.client.soda;
using com.espertech.esper.compat;
using com.espertech.esper.epl.parse;
using com.espertech.esper.epl.generated;
using com.espertech.esper.epl.spec;

using log4net;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Implementation for the admin interface.
    /// </summary>

    public class EPAdministratorImpl : EPAdministrator
    {
        private static readonly ParseRuleSelector patternParseRule;
        private static readonly ParseRuleSelector eplParseRule;
        private static readonly WalkRuleSelector patternWalkRule;
        private static readonly WalkRuleSelector eplWalkRule;

        private EPServicesContext services;
        private ConfigurationOperations configurationOperations;
        private readonly SelectClauseStreamSelectorEnum defaultStreamSelector;

        /// <summary> Constructor - takes the services context as argument.</summary>
        /// <param name="services">references to services</param>
        /// <param name="configurationOperations">runtime configuration operations</param>
        /// <param name="defaultStreamSelector">the configuration for which insert or remove streams (or both) to produce</param>
        public EPAdministratorImpl(EPServicesContext services,
                                   ConfigurationOperations configurationOperations,
                                   SelectClauseStreamSelectorEnum defaultStreamSelector)
        {
            this.services = services;
            this.configurationOperations = configurationOperations;
            this.defaultStreamSelector = defaultStreamSelector;
        }

        /// <summary>
        /// Create and starts an event pattern statement for the expressing string passed.
        /// <p>The engine assigns a unique name to the statement.</p>
        /// </summary>
        /// <param name="onExpression">must follow the documented syntax for pattern statements</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>  EPException when the expression was not valid </throws>
        public EPStatement CreatePattern(String onExpression)
        {
            return CreatePatternStmt(onExpression, null);
        }

        /// <summary>
        /// Create and starts an EPL statement.
        /// <p>The engine assigns a unique name to the statement.  The returned statement is in started state.</p>
        /// </summary>
        /// <param name="eplStatement">is the query language statement</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>  EPException when the expression was not valid </throws>
        public EPStatement CreateEPL(String eplStatement)
        {
            return CreateEPLStmt(eplStatement, null);
        }

        /// <summary>
        /// Creates the pattern.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="statementName">Name of the statement.</param>
        /// <returns></returns>
        public EPStatement CreatePattern(String expression, String statementName)
        {
            return CreatePatternStmt(expression, statementName);
        }

        /// <summary>
        /// Create and starts an EPL statement.
        /// <para>
        /// The statement name is optimally a unique name. If a statement of the same name
        /// has already been created, the engine assigns a postfix to create a unique statement name.
        /// </para>
        /// </summary>
        /// <param name="eplStatement">is the query language statement</param>
        /// <param name="statementName">is the name to assign to the statement for use in manageing the statement</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement CreateEPL(String eplStatement, String statementName)
        {
            return CreateEPLStmt(eplStatement, statementName);
        }

        /// <summary>
        /// Creates the pattern.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="statementName">Name of the statement.</param>
        /// <returns></returns>

        private EPStatement CreatePatternStmt(String expression, String statementName)
        {
            StatementSpecRaw rawPattern = CompilePattern(expression);
            return services.StatementLifecycleSvc.CreateAndStart(rawPattern, expression, true, statementName);
        }

        /// <summary>
        /// For round-trip testing of all statements, of a statement to SODA and creation from SODA,
        /// use below lines:
        /// <pre>
        ///     String pattern = "select * from pattern[" + expression + "]";
        ///     EPStatementObjectModel model = CompileEPL(pattern);
        ///     return Create(model, statementName);
        /// </pre>
        /// </summary>
        private EPStatement CreateEPLStmt(String eplStatement, String statementName)
        {
            StatementSpecRaw statementSpec = CompileEPL(eplStatement, statementName, services, defaultStreamSelector);
            EPStatement statement =
                services.StatementLifecycleSvc.CreateAndStart(statementSpec, eplStatement, false, statementName);

            log.Debug(".createEPLStmt Statement created and started");
            return statement;
        }

        /// <summary>For round-trip testing of all statements, of a statement to SODA and creation from SODA, use below lines:EPStatementObjectModel model = compile(eplStatement);return create(model, statementName);</summary>
        public EPStatement Create(EPStatementObjectModel sodaStatement)
        {
            return Create(sodaStatement, null);
        }

        public EPStatement Create(EPStatementObjectModel sodaStatement, String statementName)
        {
            // Specifies the statement
            StatementSpecRaw statementSpec = StatementSpecMapper.Map(sodaStatement, services.EngineImportService, services.VariableService);
            String eplStatement = sodaStatement.ToEPL();

            EPStatement statement =
                services.StatementLifecycleSvc.CreateAndStart(statementSpec, eplStatement, false, statementName);

            log.Debug(".createEPLStmt Statement created and started");
            return statement;
        }

        public EPPreparedStatement PrepareEPL(String eplExpression)
        {
            // compile to specification
            StatementSpecRaw statementSpec = CompileEPL(eplExpression, null, services, defaultStreamSelector);

            // map to object model thus finding all substitution parameters and their indexes
            StatementSpecUnMapResult unmapped = StatementSpecMapper.Unmap(statementSpec);

            // the prepared statement is the object model plus a list of substitution parameters
            // map to specification will refuse any substitution parameters that are unfilled
            return new EPPreparedStatementImpl(unmapped.ObjectModel, unmapped.IndexedParams);
        }

        public EPPreparedStatement PreparePattern(String patternExpression)
        {
            StatementSpecRaw rawPattern = CompilePattern(patternExpression);

            // map to object model thus finding all substitution parameters and their indexes
            StatementSpecUnMapResult unmapped = StatementSpecMapper.Unmap(rawPattern);

            // the prepared statement is the object model plus a list of substitution parameters
            // map to specification will refuse any substitution parameters that are unfilled
            return new EPPreparedStatementImpl(unmapped.ObjectModel, unmapped.IndexedParams);
        }

        public EPStatement Create(EPPreparedStatement prepared, String statementName)
        {
            EPPreparedStatementImpl impl = (EPPreparedStatementImpl)prepared;

            StatementSpecRaw statementSpec = StatementSpecMapper.Map(impl.Model, services.EngineImportService, services.VariableService);
            String eplStatement = impl.Model.ToEPL();

            return services.StatementLifecycleSvc.CreateAndStart(statementSpec, eplStatement, false, statementName);
        }

        public EPStatement Create(EPPreparedStatement prepared)
        {
            return Create(prepared, null);
        }

        public EPStatementObjectModel CompileEPL(String eplStatement)
        {
            StatementSpecRaw statementSpec = CompileEPL(eplStatement, null, services, defaultStreamSelector);
            StatementSpecUnMapResult unmapped = StatementSpecMapper.Unmap(statementSpec);
            if (unmapped.IndexedParams.Count != 0)
            {
                throw new EPException(
                    "Invalid use of substitution parameters marked by '?' in statement, use the prepare method to prepare statements with substitution parameters");
            }
            return unmapped.ObjectModel;
        }

        /// <summary>
        /// Returns the statement by the given statement name. Returns null if a statement of that name has not
        /// been created, or if the statement by that name has been destroyed.
        /// </summary>
        /// <param name="name">is the statement name to return the statement for</param>
        /// <returns>
        /// statement for the given name, or null if no such started or stopped statement exists
        /// </returns>
        public EPStatement GetStatement(String name)
        {
            return services.StatementLifecycleSvc.GetStatementByName(name);
        }

        /// <summary>
        /// Returns the statement names of all started and stopped statements.
        /// <para>
        /// This excludes the name of destroyed statements.
        /// </para>
        /// </summary>
        /// <value></value>
        /// <returns>statement names</returns>
        public IList<String> StatementNames
        {
            get { return services.StatementLifecycleSvc.StatementNames; }
        }

        /// <summary>
        /// Starts all statements that are in stopped state. Statements in started state
        /// are not affected by this method.
        /// </summary>
        /// <throws>EPException when an error occured starting statements.</throws>
        public void StartAllStatements()
        {
            services.StatementLifecycleSvc.StartAllStatements();
        }

        /// <summary>
        /// Stops all statements that are in started state. Statements in stopped state are not affected by this method.
        /// </summary>
        /// <throws>EPException when an error occured stopping statements</throws>
        public void StopAllStatements()
        {
            services.StatementLifecycleSvc.StopAllStatements();
        }

        /// <summary>
        /// Stops and destroys all statements.
        /// </summary>
        /// <throws>EPException when an error occured stopping or destroying statements</throws>
        public void DestroyAllStatements()
        {
            services.StatementLifecycleSvc.DestroyAllStatements();
        }

        /// <summary>
        /// Returns configuration operations for runtime engine configuration.
        /// </summary>
        /// <value></value>
        /// <returns>runtime engine configuration operations</returns>
        public ConfigurationOperations Configuration
        {
            get { return configurationOperations; }
        }

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        public void Destroy()
        {
            services = null;
            configurationOperations = null;
        }

        /// <summary>Compile the EPL.</summary>
        /// <param name="eplStatement">expression to compile</param>
        /// <param name="statementName">is the name of the statement</param>
        /// <param name="services">is the context</param>
        /// <param name="defaultStreamSelector">the configuration for which insert or remove streams (or both) to produce</param>
        /// <returns>statement specification</returns>
        internal static StatementSpecRaw CompileEPL(String eplStatement,
                                                    String statementName,
                                                    EPServicesContext services,
                                                    SelectClauseStreamSelectorEnum defaultStreamSelector)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(".createEPLStmt statementName=" + statementName + " eplStatement=" + eplStatement);
            }

            ITree ast = ParseHelper.Parse(eplStatement, eplParseRule);
            CommonTreeNodeStream nodes = new CommonTreeNodeStream(ast);

            EPLTreeWalker walker =
                new EPLTreeWalker(nodes,
                                  services.EngineImportService,
                                  services.VariableService,
                                  services.SchedulingService.Time,
                                  defaultStreamSelector);

            try
            {
                ParseHelper.Walk(ast, walker, eplWalkRule, eplStatement);
            }
            catch (ASTWalkException ex)
            {
                log.Error(".CreateEPL Error validating expression", ex);
                throw new EPStatementException(ex.Message, eplStatement);
            }
            catch (EPStatementSyntaxException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(".CreateEPL Error validating expression", ex);
                throw new EPStatementException(ex.Message, eplStatement);
            }

            if (log.IsDebugEnabled)
            {
                ASTUtil.DumpAST(ast);
            }

            // Specifies the statement
            return walker.GetStatementSpec();
        }

        private StatementSpecRaw CompilePattern(String expression)
        {
            // Parse and walk
            ITree ast = ParseHelper.Parse(expression, patternParseRule);
            CommonTreeNodeStream nodes = new CommonTreeNodeStream(ast);
            EPLTreeWalker walker =
                new EPLTreeWalker(nodes,
                                  services.EngineImportService,
                                  services.VariableService,
                                  services.SchedulingService.Time,
                                  defaultStreamSelector);


            try
            {
                ParseHelper.Walk(ast, walker, patternWalkRule, expression);
            }
            catch (ASTWalkException ex)
            {
                log.Debug(".createPattern Error validating expression", ex);
                throw new EPStatementException(ex.Message, expression);
            }
            catch (EPStatementSyntaxException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Debug(".createPattern Error validating expression", ex);
                throw new EPStatementException(ex.Message, expression);
            }

            if (log.IsDebugEnabled)
            {
                ASTUtil.DumpAST(ast);
            }

            if (walker.StatementSpec.StreamSpecs.Count > 1)
            {
                throw new IllegalStateException("Unexpected multiple stream specifications encountered");
            }

            // Get pattern specification
            PatternStreamSpecRaw patternStreamSpec = (PatternStreamSpecRaw)walker.StatementSpec.StreamSpecs[0];

            // Create statement spec, set pattern stream, set wildcard select
            StatementSpecRaw statementSpec = new StatementSpecRaw(SelectClauseStreamSelectorEnum.ISTREAM_ONLY);
            statementSpec.StreamSpecs.Add(patternStreamSpec);
            statementSpec.SelectClauseSpec.SelectExprList.Clear();
            statementSpec.SelectClauseSpec.SelectExprList.Add(new SelectClauseElementWildcard());
            
            return statementSpec;
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes the <see cref="EPAdministratorImpl"/> class.
        /// </summary>
        static EPAdministratorImpl()
        {
            patternParseRule = delegate(EsperEPL2GrammarParser parser) {
                                   EsperEPL2GrammarParser.startPatternExpressionRule_return r =
                                       parser.startPatternExpressionRule();
                                   return (ITree) r.Tree;
                               };
            patternWalkRule = delegate(EPLTreeWalker walker) {
                                  walker.startPatternExpressionRule();
                              };
            eplParseRule = delegate(EsperEPL2GrammarParser parser) {
                               EsperEPL2GrammarParser.startEPLExpressionRule_return r = parser.startEPLExpressionRule();
                               return (ITree) r.Tree;
                           };
            eplWalkRule = delegate(EPLTreeWalker walker) {
                              walker.startEPLExpressionRule();
                          };
        }
    }
}
