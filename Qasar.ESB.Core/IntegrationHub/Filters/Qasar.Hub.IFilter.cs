using System;
using Qasar.ESB.ConfigEngine;

namespace Qasar.ESB.Filter
{
    /// <summary>
    /// Pipeline Interface
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// This method executes the pipe
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        void Process(PipeData input);
        /// <summary>
        /// Pipe name
        /// </summary>
        string Name {get;}
        /// <summary>
        /// Pipe code for rule-engine lookups
        /// </summary>
        string Code { get;}
    }
}
