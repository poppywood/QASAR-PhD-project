using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using Qasar.ESB.ConfigEngine;
using Qasar.ESB.Filter;
using Qasar.ESB.Helpers;

namespace Qasar.ESB.Pipeline
{

    /// <summary>
    /// Pipeline design pattern - this is the pipeline builder
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Builder()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public IEntity Process(IEntity request)
        {
            //Pre-Processing takes the input and prepares a pipeData object for Pipeline processing
            PipeData data = null;
            Audit audit = null;
            IEntity returnEntity = null;
            Pipeline pipeline = null;

            try
            {
                // convert the request into a PipeData object.
                data = PreProcessor.Parse(request);

                // start auditing
                audit = new Audit(data);
                
                // Get the filters for the given pipe data
                pipeline = AssemblePipeline(data);

                // get the rules from the database.
                data.Rule = new ConfigEngine.Rule(pipeline.Id);

                // Pass the data through each filter in the pipeline.
                foreach (IFilter filter in pipeline)
                {
                    FilterAudit filterAudit = audit.FilterCreate(filter.Name);
                    filter.Process(data);
                    filterAudit.Log();
                }
            }

            catch (Exception e)
            {
                data.SetFailure();
                data.Request = ExceptionHandler.CreateMessage(e); ;
            }

            finally //check for exceptions and then build a response
            {
                audit.PipelineLog(pipeline.Id);

                //If we have a serious propblem let the HelpDesk know about it.
                if (data.Notification)
                {
                    Notification.Raise(audit.Id, data);
                    Notification.SanitiseError(audit.Id, data);
                }

                //Post-Processing builds a suitable response from the final pipeResult
                returnEntity = PostProcessor.Parse(data); 
            }
            return returnEntity;
        }


        private Pipeline AssemblePipeline(PipeData data)
        {
            int id = 0;
            ConfigEngine.PipelineRule rule = new ConfigEngine.PipelineRule();
            string [] pipearray = rule.PipelineValue(ref id, data.ProductCode, Convert.ToString(data.RequestType), data.SubAction, Convert.ToString(data.Source), data.UserId).Split(Convert.ToChar(","));
            Pipeline pipeline = new Pipeline(id);
            foreach (string pipe in pipearray)
            {
                IFilter filter = (IFilter)Activator.CreateInstance(
                    System.Type.GetType("Qasar.ESB.Filter." + pipe), new object[0]);
                pipeline.Add(filter);
            }
            return pipeline;
        }
    }
}
