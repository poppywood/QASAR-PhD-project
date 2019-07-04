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
using net.esper.compat;
using net.esper.eql.core;
using net.esper.events;
using net.esper.view;

namespace net.esper.view.std
{
	/// <summary>Factory for {@link MergeView} instances.</summary>
	public class MergeViewFactory : ViewFactory
	{
	    private String[] fieldNames;
	    private EventType eventType;

	    public void SetViewParameters(ViewFactoryContext viewFactoryContext, IList<Object> viewParameters)
	    {
	        fieldNames = GroupByViewFactory.GetFieldNameParams(viewParameters, "Group-by-merge");
	    }

	    public void Attach(EventType parentEventType, StatementContext statementContext, ViewFactory optionalParentFactory, IList<ViewFactory> parentViewFactories)
	    {
	        // Find the group by view matching the merge view
	        ViewFactory groupByViewFactory = null;
	        foreach (ViewFactory parentView in parentViewFactories)
	        {
	            if (!(parentView is GroupByViewFactory))
	            {
	                continue;
	            }
	            GroupByViewFactory candidateGroupByView = (GroupByViewFactory) parentView;
	            if (CollectionHelper.AreEqual(candidateGroupByView.GroupFieldNames, this.fieldNames))
	            {
	                groupByViewFactory = candidateGroupByView;
	            }
	        }

	        if (groupByViewFactory == null)
	        {
	            throw new ViewAttachException("Group by view for this merge view could not be found among parent views");
	        }

	        // determine types of fields
            Type[] fieldTypes = new Type[fieldNames.Length];
	        for (int i = 0; i < fieldTypes.Length; i++)
	        {
	            fieldTypes[i] = groupByViewFactory.EventType.GetPropertyType(fieldNames[i]);
	        }

	        // Determine the final event type that the merge view generates
	        // This event type is ultimatly generated by AddPropertyValueView which is added to each view branch for each
	        // group key.

	        // If the parent event type contains the merge fields, we use the same event type
	        bool parentContainsMergeKeys = true;
	        for (int i = 0; i < fieldNames.Length; i++)
	        {
	            if (!(parentEventType.IsProperty(fieldNames[i])))
	            {
	                parentContainsMergeKeys = false;
	            }
	        }

	        // If the parent view contains the fields to group by, the event type after merging stays the same
	        if (parentContainsMergeKeys)
	        {
	            eventType = parentEventType;
	        }
	        else
	        // If the parent event type does not contain the fields, such as when a statistics views is
	        // grouped which simply provides a map of calculated values,
	        // then we need to add in the merge field as an event property thus changing event types.
	        {
	            eventType = statementContext.EventAdapterService.CreateAddToEventType(
	                    parentEventType, fieldNames, fieldTypes);
	        }
	    }

	    public bool CanProvideCapability(ViewCapability viewCapability)
	    {
	        return false;
	    }

	    public void SetProvideCapability(ViewCapability viewCapability, ViewResourceCallback resourceCallback)
	    {
	        throw new UnsupportedOperationException("View capability " + viewCapability.GetType().FullName + " not supported");
	    }

	    public View MakeView(StatementContext statementContext)
	    {
	        return new MergeView(statementContext, fieldNames, eventType);
	    }

	    public EventType EventType
	    {
	    	get { return eventType; }
	    }

	    public bool CanReuse(View view)
	    {
	        if (!(view is MergeView))
	        {
	            return false;
	        }

	        MergeView myView = (MergeView) view;
            if (!CollectionHelper.AreEqual(myView.GroupFieldNames, fieldNames))
	        {
	            return false;
	        }
	        return true;
	    }
	}
} // End of namespace