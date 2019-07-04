using System;

namespace net.esper.support.bean
{
	public class SupportBeanBase
	{
		virtual public String Id
		{
            get { return _id; }
            set { this._id = value; }
		}

		private String _id;
		
		public SupportBeanBase(String id)
		{
            this._id = id;
		}
		
		public override String ToString()
		{
            return "id=" + _id;
		}
	}
}