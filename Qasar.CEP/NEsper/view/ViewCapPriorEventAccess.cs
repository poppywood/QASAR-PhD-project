///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using net.esper.core;
using net.esper.client;
using net.esper.view.internals;

namespace net.esper.view
{
	/// <summary>
	/// Describes that we need access to prior events (result events published by views),
	/// for use by the &quot;prior&quot; expression function.
	/// </summary>
    public class ViewCapPriorEventAccess : ViewCapability
    {
        private readonly int? indexConstant;

        /// <summary>Ctor.</summary>
        /// <param name="indexConstant">
        /// is the index of the prior event, with zero being the current event.
        /// </param>
        public ViewCapPriorEventAccess(int? indexConstant)
        {
            this.indexConstant = indexConstant;
        }

        /// <summary>Index or the prior event we are asking for.</summary>
        /// <returns>prior event index constant</returns>
        public int? IndexConstant
        {
            get { return indexConstant; }
        }

        public bool Inspect(int streamNumber, IList<ViewFactory> viewFactories, StatementContext statementContext)
        {
            bool unboundStream = viewFactories.Count == 0;

            // Find the prior event view to see if it has already been added
            foreach (ViewFactory viewFactory in viewFactories)
            {
                if (viewFactory is PriorEventViewFactory)
                {
                    return true;
                }
            }

            try
            {
                String _namespace = ViewEnum.PRIOR_EVENT_VIEW.Namespace;
                String name = ViewEnum.PRIOR_EVENT_VIEW.Name;
                ViewFactory factory = statementContext.ViewResolutionService.Create(_namespace, name);
                viewFactories.Add(factory);

                ViewFactoryContext context = new ViewFactoryContext(statementContext, streamNumber, viewFactories.Count + 1, _namespace, name);
                factory.SetViewParameters(context, new Object[] { unboundStream });
            }
            catch (ViewProcessingException ex)
            {
                String text = "Exception creating prior event view factory";
                throw new EPException(text, ex);
            }
            catch (ViewParameterException ex)
            {
                String text = "Exception creating prior event view factory";
                throw new EPException(text, ex);
            }

            return true;
        }
    }
} // End of namespace
