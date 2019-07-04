using System;
using System.Collections.Generic;
using System.Text;

using Qasar.ESB.Helpers;

namespace Qasar.ESB.Filter
{
    /// <summary>
    /// base class for all filters
    /// </summary>
    public abstract class FilterBase : IFilter
    {
        /// <summary>
        /// The name of the filter
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// The code of the filter
        /// </summary>
        public abstract string Code { get; }

        /// <summary>
        /// All sub classes of FilterBase must implement ProcessData. This is the function
        /// that executes the work of the filter.
        /// </summary>
        /// <param name="data"></param>
        protected abstract void ProcessData(PipeData data);

        /// <summary>
        /// The Pipeline Builder calls process to execute the filter.
        /// </summary>
        /// <param name="data"></param>
        public void Process(PipeData data)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (data.Rule == null) throw new ArgumentNullException("data.Rule");
            if (data.Failed && SkipOnFailure) return;

            try
            {
                ProcessData(data);
            }
            catch (Exception e)
            {
                data.Request = ExceptionHandler.CreateMessage(e);
                data.SetNotificationFailure();
            }
        }

        /// <summary>
        /// override this property to return false if your filter must always process
        /// even if a filter earlier in the chain has failed.
        /// </summary>
        /// <returns></returns>
        protected virtual bool SkipOnFailure
        {
            get { return true; }
        }
    }
}
