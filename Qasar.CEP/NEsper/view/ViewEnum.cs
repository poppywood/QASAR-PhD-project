using System;

using net.esper.view.internals;
using net.esper.view.std;
using net.esper.view.ext;
using net.esper.view.window;
using net.esper.view.stat;

namespace net.esper.view
{
    /// <summary>
    /// Enum for all build-in views.
    /// </summary>

    public class ViewEnum
    {
        /// <summary>Length window.</summary>
        public static readonly ViewEnum LENGTH = new ViewEnum("win", "length", typeof(LengthWindowViewFactory), null);

        /// <summary>Length batch window.</summary>
        public static readonly ViewEnum LENGTH_BATCH = new ViewEnum("win", "length_batch", typeof(LengthBatchViewFactory), null);

        /// <summary>Time window.</summary>
        public static readonly ViewEnum TIME_WINDOW = new ViewEnum("win", "time", typeof(TimeWindowViewFactory), null);

        /// <summary>Time batch.</summary>
        public static readonly ViewEnum TIME_BATCH = new ViewEnum("win", "time_batch", typeof(TimeBatchViewFactory), null);

        /// <summary>Externally timed window.</summary>
        public static readonly ViewEnum EXT_TIMED_WINDOW = new ViewEnum("win", "ext_timed", typeof(ExternallyTimedWindowViewFactory), null);

        /// <summary>Size view.</summary>
        public static readonly ViewEnum SIZE = new ViewEnum("std", "size", typeof(SizeViewFactory), null);

        /// <summary>Last event.</summary>
        public static readonly ViewEnum LAST_EVENT = new ViewEnum("std", "lastevent", typeof(LastElementViewFactory), null);

        /// <summary>Unique.</summary>
        public static readonly ViewEnum UNIQUE_BY_PROPERTY = new ViewEnum("std", "unique", typeof(UniqueByPropertyViewFactory), null);

        /// <summary>Group-by merge.</summary>
        public static readonly ViewEnum GROUP_MERGE = new ViewEnum("std", "merge", typeof(MergeViewFactory), null);

        /// <summary>Group-by.</summary>
        public static readonly ViewEnum GROUP_PROPERTY = new ViewEnum("std", "groupby", typeof(GroupByViewFactory), GROUP_MERGE);

        /// <summary>Univariate statistics.</summary>
        public static readonly ViewEnum UNIVARIATE_STATISTICS = new ViewEnum("stat", "uni", typeof(UnivariateStatisticsViewFactory), null);

        /// <summary>Weighted avg.</summary>
        public static readonly ViewEnum WEIGHTED_AVERAGE = new ViewEnum("stat", "weighted_avg", typeof(WeightedAverageViewFactory), null);

        /// <summary>Correlation.</summary>
        public static readonly ViewEnum CORRELATION = new ViewEnum("stat", "correl", typeof(CorrelationViewFactory), null);

        /// <summary>Linest.</summary>
        public static readonly ViewEnum REGRESSION_LINEST = new ViewEnum("stat", "linest", typeof(RegressionLinestViewFactory), null);

        /// <summary>Cubes.</summary>
        public static readonly ViewEnum MULTIDIM_VIEW = new ViewEnum("stat", "cube", typeof(MultiDimStatsViewFactory), null);

        /// <summary>Sorted window.</summary>
        public static readonly ViewEnum SORT_WINDOW = new ViewEnum("ext", "sort", typeof(SortWindowViewFactory), null);

        /// <summary>Prior event view.</summary>
        public static readonly ViewEnum PRIOR_EVENT_VIEW = new ViewEnum("int", "prioreventinternal", typeof(PriorEventViewFactory), null);

        /// <summary>
        /// All of the "values" in the pseudo-enum ViewEnum.
        /// </summary>
        public static readonly ViewEnum[] Values = new ViewEnum[]
            {
                LENGTH,
                LENGTH_BATCH,
                TIME_WINDOW,
                TIME_BATCH,
                EXT_TIMED_WINDOW,
                SIZE,
                LAST_EVENT,
                UNIQUE_BY_PROPERTY,
                GROUP_MERGE,
                GROUP_PROPERTY,
                UNIVARIATE_STATISTICS,
                WEIGHTED_AVERAGE,
                CORRELATION,
                REGRESSION_LINEST,
                MULTIDIM_VIEW,
                SORT_WINDOW,
                PRIOR_EVENT_VIEW
            };

        private readonly String nspace;
        private readonly String name;
        private readonly Type factoryType;
        private readonly ViewEnum mergeView;

        ViewEnum(String nspace, String name, Type factoryType, ViewEnum mergeView)
        {
            this.nspace = nspace;
            this.name = name;
            this.factoryType = factoryType;
            this.mergeView = mergeView;
        }

        /// <summary> Returns namespace that the object belongs to.</summary>
        /// <returns> namespace
        /// </returns>

        public String Namespace
        {
            get { return nspace; }
        }

        /// <summary> Returns name of the view that can be used to reference the view in a view expression.</summary>
        /// <returns> short name of view
        /// </returns>

        public String Name
        {
            get { return name; }
        }

        /// <summary> Gets the view's factory class.</summary>
        /// <returns> view's factory class
        /// </returns>

        public Type FactoryType
        {
            get { return factoryType; }
        }

        /// <summary> Returns the enumeration value of the view for merging the data generated by another view.</summary>
        /// <returns> view enum for the merge view
        /// </returns>

        public ViewEnum MergeView
        {
            get { return mergeView; }
        }

        /// <summary>
        /// Returns the view enumeration value given the name of the view.
        /// </summary>
        /// <param name="nspace">The nspace.</param>
        /// <param name="name">is the short name of the view as used in view expressions</param>
        /// <returns>
        /// view enumeration value, or null if no such view name is among the enumerated values
        /// </returns>

        public static ViewEnum ForName(String nspace, String name)
        {
            foreach (ViewEnum viewEnum in ViewEnum.Values)
            {
                if ((viewEnum.Namespace.Equals(nspace)) && (viewEnum.Name.Equals(name)))
                {
                    return viewEnum;
                }
            }

            return null;
        }
    }
}