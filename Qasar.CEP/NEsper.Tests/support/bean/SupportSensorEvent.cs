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
	public class SupportSensorEvent
	{
	    private int _id;
        private String _type;
        private String _device;
        private double _measurement;
        private double _confidence;

	    public SupportSensorEvent(int id, String type, String device, double measurement, double confidence)
	    {
            this._id = id;
            this._type = type;
            this._device = device;
            this._measurement = measurement;
            this._confidence = confidence;
	    }

	    public int Id
	    {
            get {return _id;}
	    }

	    public String Type
	    {
            get {return _type;}
	    }

	    public String Device
	    {
            get {return _device;}
	    }

	    public double Measurement
	    {
            get {return _measurement;}
	    }

	    public double Confidence
	    {
            get {return _confidence;}
	    }
	}
} // End of namespace
