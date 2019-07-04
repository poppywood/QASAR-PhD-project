using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Qasar.GA
{
    public class ChromosomeIndex
    {

        private int chromosomeIndex;

        private int classType; //classType 0 is the resource delimiter

        public ChromosomeIndex(int ChromosomeIndex, int ClassType)
        {
            this.chromosomeIndex = ChromosomeIndex;
            this.classType = ClassType;
        }

        public int getChromosomeIndex()
        {
            return this.chromosomeIndex;
        }

        public void setChromosomeIndex(int ChromosomeIndex)
        {
            this.chromosomeIndex = ChromosomeIndex;
        }

        public int getClassType()
        {
            return this.classType;
        }

        public void setClassType(int ClassType)
        {
            this.classType = ClassType;
        }
    }

    public class ChromosomeIndexCollection : CollectionBase
    {
        public int Add(ChromosomeIndex item)
        {
            return List.Add(item);
        }
        public void Insert(int index, ChromosomeIndex item)
        {
            List.Insert(index, item);
        }
        public void Remove(ChromosomeIndex item)
        {
            List.Remove(item);
        }
        public bool Contains(ChromosomeIndex item)
        {
            return List.Contains(item);
        }
        public int IndexOf(ChromosomeIndex item)
        {
            return List.IndexOf(item);
        }
        public void CopyTo(ChromosomeIndex[] array, int index)
        {
            List.CopyTo(array, index);
        }
        public ChromosomeIndex this[int index]
        {
            get { return (ChromosomeIndex)List[index]; }
            set { List[index] = value; }
        }
    }
}
