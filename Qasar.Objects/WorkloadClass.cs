using System;
using System.Collections;
using System.Text;

namespace Qasar.ObjectLayer
{
    public class WorkloadClass
    {
        private int classid;

	    private String classname;

        private string subAction;

	    public WorkloadClass(int classid, String classname, string subAction) {
		    this.classid = classid;
		    this.classname = classname;
            this.subAction = subAction;
	    }

	    public int getClassid() {
		    return this.classid;
	    }

	    public void setClassid(int classid) {
		    this.classid = classid;
	    }

	    public String getClassname() {
		    return this.classname;
	    }

	    public void setClassname(String classname) {
		    this.classname = classname;
	    }

        public String getSubAction()
        {
            return this.subAction;
        }

        public void setSubAction(String subAction)
        {
            this.subAction = subAction;
        }

    }
    public class WorkloadClassCollection : CollectionBase
    {
        public int Add(WorkloadClass item)
        {
            return List.Add(item);
        }
        public void Insert(int index, WorkloadClass item)
        {
            List.Insert(index, item);
        }
        public void Remove(WorkloadClass item)
        {
            List.Remove(item);
        }
        public bool Contains(WorkloadClass item)
        {
            return List.Contains(item);
        }
        public int IndexOf(WorkloadClass item)
        {
            return List.IndexOf(item);
        }
        public void CopyTo(WorkloadClass[] array, int index)
        {
            List.CopyTo(array, index);
        }
        public WorkloadClass this[int index]
        {
            get { return (WorkloadClass)List[index]; }
            set { List[index] = value; }
        }
    } 
}
