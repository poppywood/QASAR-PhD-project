///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.join;
using com.espertech.esper.epl.named;
using com.espertech.esper.epl.spec;
using com.espertech.esper.events;
using com.espertech.esper.view;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Starts and provides the stop method for EPL statements.
    /// </summary>
    public class EPPreparedExecuteMethod
    {
        private readonly StatementSpecCompiled statementSpec;    
        private readonly ResultSetProcessor resultSetProcessor;
        private readonly NamedWindowProcessor[] processors;
        private readonly JoinSetComposer joinComposer;
    
        /// <summary>Ctor. </summary>
        /// <param name="statementSpec">is a container for the definition of all statement constructs thatmay have been used in the statement, i.e. if defines the select clauses, insert into, outer joins etc. </param>
        /// <param name="services">is the service instances for dependency injection</param>
        /// <param name="statementContext">is statement-level information and statement services</param>
        /// <throws>ExprValidationException if the preparation failed</throws>
        public EPPreparedExecuteMethod(StatementSpecCompiled statementSpec,
                                    EPServicesContext services,
                                    StatementContext statementContext)
        {
            this.statementSpec = statementSpec;
    
            ValidateExecuteQuery();
    
            int numStreams = statementSpec.StreamSpecs.Count;
            EventType[] typesPerStream = new EventType[numStreams];
            String[] namesPerStream = new String[numStreams];
            bool[] namedWindow = new bool[numStreams];
            for (int ii = 0; ii < numStreams; ii++)
                namedWindow[ii] = true;

            processors = new NamedWindowProcessor[numStreams];
    
            for (int i = 0; i < numStreams; i++)
            {
                StreamSpecCompiled streamSpec = statementSpec.StreamSpecs[i];
                NamedWindowConsumerStreamSpec namedSpec = (NamedWindowConsumerStreamSpec) streamSpec;
    
                String streamName = namedSpec.WindowName;
                if (namedSpec.OptionalStreamName != null)
                {
                    streamName = namedSpec.OptionalStreamName;
                }
                namesPerStream[i] = streamName;
    
                processors[i] = services.NamedWindowService.GetProcessor(namedSpec.WindowName);
                typesPerStream[i] = processors[i].TailView.EventType;
            }

            StreamTypeService typeService =
                new StreamTypeServiceImpl(typesPerStream, namesPerStream, services.EngineURI, namesPerStream);
            EPStatementStartMethod.ValidateNodes(statementSpec, statementContext, typeService, null);
    
            resultSetProcessor = ResultSetProcessorFactory.GetProcessor(statementSpec, statementContext, typeService, null);
    
            if (numStreams > 1)
            {
                Viewable[] viewablePerStream = new Viewable[numStreams];
                for (int i = 0; i < numStreams; i++)
                {
                    viewablePerStream[i] = processors[i].TailView;
                }
                joinComposer =
                    statementContext.JoinSetComposerFactory.MakeComposer(statementSpec.OuterJoinDescList,
                                                                         statementSpec.FilterRootNode,
                                                                         typesPerStream,
                                                                         namesPerStream,
                                                                         viewablePerStream,
                                                                         SelectClauseStreamSelectorEnum.ISTREAM_ONLY,
                                                                         new bool[numStreams],
                                                                         new bool[numStreams],
                                                                         namedWindow);
            }
            else
            {
                joinComposer = null;
            }
        }

        /// <summary>Returns the event type of the prepared statement. </summary>
        /// <returns>event type</returns>
        public EventType EventType
        {
            get { return resultSetProcessor.ResultEventType; }
        }

        /// <summary>Executes the prepared query. </summary>
        /// <returns>query results</returns>
        public EPPreparedQueryResult Execute()
        {
            int numStreams = processors.Length;
    
            ICollection<EventBean>[] snapshots = new ICollection<EventBean>[numStreams];
            for (int i = 0; i < numStreams; i++)
            {
                StreamSpecCompiled streamSpec = statementSpec.StreamSpecs[i];
                NamedWindowConsumerStreamSpec namedSpec = (NamedWindowConsumerStreamSpec) streamSpec;
                snapshots[i] = processors[i].TailView.Snapshot();
    
                if (namedSpec.FilterExpressions.Count != 0)
                {
                    snapshots[i] = GetFiltered(snapshots[i], namedSpec.FilterExpressions);
                }
            }
    
            UniformPair<EventBean[]> results;
            if (numStreams == 1)
            {
                if (statementSpec.FilterRootNode != null) {
                    snapshots[0] = GetFiltered(snapshots[0], new ExprNode[] {statementSpec.FilterRootNode});
                }
                EventBean[] rows = CollectionHelper.ToArray(snapshots[0]);
                results = resultSetProcessor.ProcessViewResult(rows, null, true);
            }
            else
            {
                EventBean[][] oldDataPerStream = new EventBean[numStreams][];
                EventBean[][] newDataPerStream = new EventBean[numStreams][];
                Viewable[] viewablePerStream = new Viewable[numStreams];
                for (int i = 0; i < numStreams; i++)
                {
                    newDataPerStream[i] = CollectionHelper.ToArray(snapshots[i]);
                    viewablePerStream[i] = processors[i].TailView;
                }
                UniformPair<Set<MultiKey<EventBean>>> result = joinComposer.Join(newDataPerStream, oldDataPerStream);
                results = resultSetProcessor.ProcessJoinResult(result.First, null, true);
            }
    
            return new EPPreparedQueryResult(resultSetProcessor.ResultEventType, results.First);
        }
    
        private void ValidateExecuteQuery()
        {
            if (statementSpec.SubSelectExpressions.Count > 0)
            {
                throw new ExprValidationException("Subqueries are not a supported feature of on-demand queries");
            }
            for (int i = 0; i < statementSpec.StreamSpecs.Count; i++)
            {
                if (!(statementSpec.StreamSpecs[i] is NamedWindowConsumerStreamSpec))
                {
                    throw new ExprValidationException("On-demand queries require named windows and do not allow event streams or patterns");
                }
                if (statementSpec.StreamSpecs[i].ViewSpecs.Count != 0)
                {
                    throw new ExprValidationException("Views are not a supported feature of on-demand queries");
                }
            }
            if (statementSpec.OutputLimitSpec != null)
            {
                throw new ExprValidationException("Output rate limiting is not a supported feature of on-demand queries");
            }
            if (statementSpec.InsertIntoDesc != null)
            {
                throw new ExprValidationException("Insert-into is not a supported feature of on-demand queries");
            }
        }
    
        private static List<EventBean> GetFiltered(IEnumerable<EventBean> snapshot, IEnumerable<ExprNode> filterExpressions)
        {
            EventBean[] eventsPerStream = new EventBean[1];
            List<EventBean> filteredSnapshot = new List<EventBean>();
            foreach (EventBean row in snapshot)
            {
                bool pass = true;
                eventsPerStream[0] = row;
                foreach (ExprNode filter in filterExpressions)
                {
                    bool? result = (bool?) filter.Evaluate(eventsPerStream, true);
                    if (result != null)
                    {
                        if (!result.Value)
                        {
                            pass = false;
                            break;
                        }
                    }
                }
    
                if (pass)
                {
                    filteredSnapshot.Add(row);
                }
            }
    
            return filteredSnapshot;
        }
    }
}
