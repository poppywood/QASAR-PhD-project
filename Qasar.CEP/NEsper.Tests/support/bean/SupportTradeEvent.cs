///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace net.esper.support.bean
{
	public class SupportTradeEvent
	{
	    private int _id;
        private String _userId;
        private String _ccypair;
        private String _direction;

	    public SupportTradeEvent(int id, String userId, String ccypair, String direction)
	    {
            this._id = id;
            this._userId = userId;
            this._ccypair = ccypair;
            this._direction = direction;
	    }

	    public String UserId
	    {
            get {return _userId;}
	    }

	    public String Ccypair
	    {
            get {return _ccypair;}
	    }

	    public String Direction
	    {
            get {return _direction;}
	    }

	    public String ToString()
	    {
            return "id=" + _id +
                   " userId=" + _userId +
                   " ccypair=" + _ccypair +
                   " direction=" + _direction;
	    }
	}
} // End of namespace
