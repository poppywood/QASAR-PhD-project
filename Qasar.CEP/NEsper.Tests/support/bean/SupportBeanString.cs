using System;

namespace net.esper.support.bean
{
    [Serializable]
	public class SupportBeanString
	{
		virtual public String String
		{
			get { return stringValue; }
			set { this.stringValue = value; }
		}

		private String stringValue;
		
		public SupportBeanString(String stringValue)
		{
			this.stringValue = stringValue;
		}
		
		public override String ToString()
		{
			return "SupportBeanString string=" + stringValue;
		}
	}
}