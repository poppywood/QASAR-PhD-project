using System;

namespace net.esper.support.bean
{
    [Serializable]
	public class SupportBeanSimple
	{
		virtual public String MyString
		{
            get { return _myString; }
            set { this._myString = value; }
		}

		virtual public int MyInt
		{
            get { return _myInt; }
            set { this._myInt = value; }
		}
        
        private String _myString;
        private int _myInt;
		
		public SupportBeanSimple(String myString, int myInt)
		{
            this._myString = myString;
            this._myInt = myInt;
		}
	}
}
