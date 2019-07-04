using System;
using Qasar.ESB.Filter;

namespace Qasar.ESB.Pipeline
{
    /// <summary>
    /// Performs post-processing to get the pipeline output into the desired form
    /// </summary>
    public static class PostProcessor
    {
        /// <summary>
        /// Parse
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEntity Parse(PipeData data)
        {
            IEntity val = null;
                    PolicyRequest req = (PolicyRequest)data.Entity;
                    PolicyResponse resp = new PolicyResponse();
                    resp.OverallResult = data.Success.ToString();
                    resp.PolicyRef = req.PolicyRef;
                    resp.ProductCode = req.ProductCode;
                    resp.Result = data.Request;
                    val = resp;

            return val;
        }
    }
}
