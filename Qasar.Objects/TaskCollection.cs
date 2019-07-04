using System;
using System.Collections;
using System.Text;

namespace Qasar.ObjectLayer
{
    public class Task
    {
        private int resourceid;

        private int classId;

        private double executionTime;

        private int jobCount;

        public double getJobCount()
        {
            return this.jobCount;
        }

        public void setJobCount(int JobCount)
        {
            this.jobCount = JobCount;
        }


        public Task(int resourceid, int classId, double ExecutionTime, int JobCount)
        {
            this.resourceid = resourceid;
            this.classId = classId;
            this.executionTime = ExecutionTime;
            this.jobCount = JobCount;
        }

        public double getExecutionTime()
        {
            return this.executionTime;
        }

        public void setExecutionTime(double ExecutionTime)
        {
            this.executionTime = ExecutionTime;
        }

        public int getResourceid()
        {
            return this.resourceid;
        }

        public void setResourceid(int resourceid)
        {
            this.resourceid = resourceid;
        }

        public int getclassId()
        {
            return this.classId;
        }

        public void setsubAction(int classId)
        {
            this.classId = classId;
        }
    }

    public class TaskCollection : CollectionBase
    {
        public int Add(Task item)
        {
            return List.Add(item);
        }
        public void Insert(int index, Task item)
        {
            List.Insert(index, item);
        }
        public void Remove(Task item)
        {
            List.Remove(item);
        }
        public bool Contains(Task item)
        {
            return List.Contains(item);
        }
        public int IndexOf(Task item)
        {
            return List.IndexOf(item);
        }
        public void CopyTo(Task[] array, int index)
        {
            List.CopyTo(array, index);
        }
        public Task this[int index]
        {
            get { return (Task)List[index]; }
            set { List[index] = value; }
        }
    }
}
