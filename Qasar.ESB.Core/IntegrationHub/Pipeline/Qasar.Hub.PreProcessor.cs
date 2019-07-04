using System;
using Qasar.ESB.Filter;

namespace Qasar.ESB.Pipeline
{
    /// <summary>
    /// Performs pre-processing to get the pipeline input into the desired form
    /// </summary>
    public static class PreProcessor
    {

        public static PipeData Parse(IEntity entity)
        {
            PipeData data = null;

            PolicyRequest req = (PolicyRequest)entity;

            data = new PipeData(
                req.Policy, 
                req.Action.ToString(), 
                req.ProductCode,
                req.Credentials.Source, 
                req.Credentials.User, 
                req.PolicyRef, 
                req.Policy, 
                0,
                entity);

            return data;
        }
    }
}
