///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.epl.core;
using com.espertech.esper.epl.join;
using com.espertech.esper.epl.spec;
using com.espertech.esper.pattern;
using com.espertech.esper.schedule;
using com.espertech.esper.util;
using com.espertech.esper.view;

namespace com.espertech.esper.core
{
	/// <summary>
	/// Default implementation for making a statement-specific context class.
	/// </summary>
    public class StatementContextFactoryDefault : StatementContextFactory
    {
        private readonly PluggableObjectCollection viewClasses;
        private readonly PluggableObjectCollection patternObjectClasses;

        /// <summary>Ctor.</summary>
        /// <param name="viewPlugIns">is the view plug-in object descriptions</param>
        /// <param name="plugInPatternObj">is the pattern plug-in object descriptions</param>
        public StatementContextFactoryDefault(PluggableObjectCollection viewPlugIns, PluggableObjectCollection plugInPatternObj)
        {
            viewClasses = new PluggableObjectCollection();
            viewClasses.AddObjects(viewPlugIns);
            viewClasses.AddObjects(ViewEnumHelper.BuiltinViews);

            patternObjectClasses = new PluggableObjectCollection();
            patternObjectClasses.AddObjects(plugInPatternObj);
            patternObjectClasses.AddObjects(PatternObjectHelper.BuiltinPatternObjects);
        }

        public StatementContext MakeContext(String statementId,
                                        String statementName,
                                        String expression,
                                        bool hasVariables,
                                        EPServicesContext engineServices,
                                        Map<String, Object> optAdditionalContext,
                                        OnTriggerDesc optOnTriggerDesc,
                                        CreateWindowDesc optCreateWindowDesc)
        {
            // Allocate the statement's schedule bucket which stays constant over it's lifetime.
            // The bucket allows callbacks for the same time to be ordered (within and across statements) and thus deterministic.
            ScheduleBucket scheduleBucket = engineServices.SchedulingService.AllocateBucket();

            // Create a lock for the statement
            ManagedLock statementResourceLock;

            // For on-delete statements, use the create-named-window statement lock
            if ((optOnTriggerDesc != null) && (optOnTriggerDesc is OnTriggerWindowDesc))
            {
                String windowName = ((OnTriggerWindowDesc)optOnTriggerDesc).WindowName;
                statementResourceLock = engineServices.NamedWindowService.GetNamedWindowLock(windowName);
                if (statementResourceLock == null)
                {
                    throw new EPStatementException("Named window '" + windowName + "' has not been declared", expression);
                }
            }
            // For creating a named window, save the lock for use with on-delete statements
            else
                if (optCreateWindowDesc != null)
                {
                    statementResourceLock =
                        engineServices.NamedWindowService.GetNamedWindowLock(optCreateWindowDesc.WindowName);
                    if (statementResourceLock == null)
                    {
                        statementResourceLock = engineServices.StatementLockFactory.GetStatementLock(statementName, expression);
                        engineServices.NamedWindowService.AddNamedWindowLock(optCreateWindowDesc.WindowName,
                                                                             statementResourceLock);
                    }
                }
                else
                {
                    statementResourceLock = engineServices.StatementLockFactory.GetStatementLock(statementName, expression);
                }

            EPStatementHandle epStatementHandle =
                new EPStatementHandle(statementId, statementResourceLock, expression, hasVariables);

            MethodResolutionService methodResolutionService =
                new MethodResolutionServiceImpl(engineServices.EngineImportService);

            PatternContextFactory patternContextFactory = new PatternContextFactoryDefault();

            ViewResolutionService viewResolutionService = new ViewResolutionServiceImpl(viewClasses);
            PatternObjectResolutionService patternResolutionService =
                new PatternObjectResolutionServiceImpl(patternObjectClasses);

            // Create statement context
            return new StatementContext(engineServices.EngineURI,
                                        engineServices.EngineInstanceId,
                                        statementId,
                                        statementName,
                                        expression,
                                        engineServices.SchedulingService,
                                        scheduleBucket,
                                        engineServices.EventAdapterService,
                                        epStatementHandle,
                                        viewResolutionService,
                                        patternResolutionService,
                                        null, // no statement extension context
                                        new StatementStopServiceImpl(),
                                        methodResolutionService,
                                        patternContextFactory,
                                        engineServices.FilterService,
                                        new JoinSetComposerFactoryImpl(),
                                        engineServices.OutputConditionFactory,
                                        engineServices.NamedWindowService,
                                        engineServices.VariableService,
                                        new StatementResultServiceImpl(engineServices.StatementLifecycleSvc),
                                        engineServices.EngineSettingsService.PlugInEventTypeResolutionURIs,
                                        engineServices.ValueAddEventService);
        }
    }
} // End of namespace