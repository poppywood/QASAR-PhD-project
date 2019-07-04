using System;
using System.Collections;
using System.Text;

namespace Qasar.ObjectLayer
{
    public class Resource
    {

        private int resourceid;

        private String resource;

        public Resource(int resourceid, String resource)
        {
            this.resourceid = resourceid;
            this.resource = resource;
        }

        public int getResourceid()
        {
            return this.resourceid;
        }

        public void setResourceid(int resourceid)
        {
            this.resourceid = resourceid;
        }

        public String getResource()
        {
            return this.resource;
        }

        public void setResource(String resource)
        {
            this.resource = resource;
        }
    }

    public class ResourceCollection : CollectionBase
    {
        public int Add(Resource item)
        {
            return List.Add(item);
        }
        public void Insert(int index, Resource item)
        {
            List.Insert(index, item);
        }
        public void Remove(Resource item)
        {
            List.Remove(item);
        }
        public bool Contains(Resource item)
        {
            return List.Contains(item);
        }
        public int IndexOf(Resource item)
        {
            return List.IndexOf(item);
        }
        public void CopyTo(Resource[] array, int index)
        {
            List.CopyTo(array, index);
        }
        public Resource this[int index]
        {
            get { return (Resource)List[index]; }
            set { List[index] = value; }
        }
    }
}
