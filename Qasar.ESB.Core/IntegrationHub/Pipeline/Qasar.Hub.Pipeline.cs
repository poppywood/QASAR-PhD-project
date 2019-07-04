using System;
using System.Collections.Generic;
using System.Text;

namespace Qasar.ESB.Pipeline
{
    /// <summary>
    /// a collection of filters that makes a pipeline
    /// </summary>
    public class Pipeline : List<Filter.IFilter>
    {
        int _id;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        public Pipeline(int id)
        {
            _id = id;
        }

        /// <summary>
        /// The pipeline Id
        /// </summary>
        public int Id
        {
            get { return _id; }
        }
    }
}
