using System;
using System.Collections;
using System.Text;

namespace Qasar.ObjectLayer
{
    public class ResourceMeasurement
    {
        private int measurementid;

	    private int crid;

	    private double load;

	    private double average;


        public ResourceMeasurement(int measurementid, int crid, double load,
                double average)
        {
		    this.measurementid = measurementid;
		    this.crid = crid;
		    this.load = load;
		    this.average = average;
	    }


	    public int getMeasurementid() {
		    return this.measurementid;
	    }

	    public void setMeasurementid(int measurementid) {
		    this.measurementid = measurementid;
	    }

	    public int getCrid() {
		    return this.crid;
	    }

	    public void setCrid(int crid) {
		    this.crid = crid;
	    }

        public double getLoad()
        {
		    return this.load;
	    }

        public void setLoad(double load)
        {
		    this.load = load;
	    }

        public double getAverage()
        {
		    return this.average;
	    }

        public void setAverage(double average)
        {
		    this.average = average;
	    }



        }

        public class ResourceMeasurementCollection : CollectionBase
        {
        public int Add(ResourceMeasurement item)
        {
            return List.Add(item);
        }
        public void Insert(int index, ResourceMeasurement item)
        {
            List.Insert(index, item);
        }
        public void Remove(ResourceMeasurement item)
        {
            List.Remove(item);
        }
        public bool Contains(ResourceMeasurement item)
        {
            return List.Contains(item);
        }
        public int IndexOf(ResourceMeasurement item)
        {
            return List.IndexOf(item);
        }
        public void CopyTo(ResourceMeasurement[] array, int index)
        {
            List.CopyTo(array, index);
        }
        public ResourceMeasurement this[int index]
        {
            get { return (ResourceMeasurement)List[index]; }
            set { List[index] = value; }
        }
    } 
}
