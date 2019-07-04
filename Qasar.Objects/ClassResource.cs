using System;
using System.Collections;
using System.Xml;
using System.Text;

namespace Qasar.ObjectLayer
{
    public class ClassResource
    {

        private int crid;

	    private int resourceid;

	    private int classid;

        private double spotLoad;

	    public double SpotLoad
	    {
		    get { return spotLoad;}
		    set { spotLoad = value;}
	    }
	


	    public ClassResource(int crid, int resourceid, int classid, double spotLoad) {
		    this.crid = crid;
		    this.resourceid = resourceid;
		    this.classid = classid;
            this.spotLoad = spotLoad;
	    }

	    public int getCrid() {
		    return this.crid;
	    }

	    public void setCrid(int crid) {
		    this.crid = crid;
	    }

	    public int getResourceid() {
		    return this.resourceid;
	    }

	    public void setResourceid(int resourceid) {
		    this.resourceid = resourceid;
	    }

	    public int getClassid() {
		    return this.classid;
	    }

	    public void setClassid(int classid) {
		    this.classid = classid;
	    }

    }

    public class ClassResourceCollection : CollectionBase
    {
        public int Add(ClassResource item)
        {
            return List.Add(item);
        }
        public void Insert(int index, ClassResource item)
        {
            List.Insert(index, item);
        }
        public void Remove(ClassResource item)
        {
            List.Remove(item);
        }
        public bool Contains(ClassResource item)
        {
            return List.Contains(item);
        }
        public int IndexOf(ClassResource item)
        {
            return List.IndexOf(item);
        }
        public void CopyTo(ClassResource[] array, int index)
        {
            List.CopyTo(array, index);
        }
        public ClassResource this[int index]
        {
            get { return (ClassResource)List[index]; }
            set { List[index] = value; }
        }
    } 
}
