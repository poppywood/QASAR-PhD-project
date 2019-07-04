using System;

namespace Qasar.ESB.Filter
{
    /// <summary>
    /// Use this template by copying and pasting it into your own filter class.
    /// </summary>
    public class MyFilter: FilterBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Name { get { return "MyFilter"; } }
        /// <summary>
        /// 
        /// </summary>
        public override string Code { get { return "ABC"; } }
        /// <summary>
        /// 
        /// </summary>
        public MyFilter()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessData(PipeData data)
        {
            //Get some rules
            string someRule = data.Rule.Value(this.Code, "DEF", data.ProductCode, Convert.ToString(data.RequestType), data.SubAction, Convert.ToString(data.Source), data.UserId, data);
            //do something to d.request
            data.Request = "something";//some result of what we just did
        }
    }
}