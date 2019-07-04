using System.Collections;
using System.Threading;

namespace WSQoSAPI
{

    public class NamedItemCollection
    {

        private ArrayList list;
        private Mutex ManilpulateList;
        private ArrayList unsyncronizedList;

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public NamedItemCollection()
        {
            ManilpulateList = new Mutex();
            unsyncronizedList = new ArrayList();
            list = ArrayList.Synchronized(unsyncronizedList);
        }

        public int Add(NamedItem a)
        {
            // trial
            return 0;
        }

        public void Clear()
        {
            list.Clear();
        }

        public int GetIndexByName(string name)
        {
            // trial
            return 0;
        }

        public NamedItem GetItemByName(string name)
        {
            // trial
            return null;
        }

        public NamedItem[] GetItemsByName(string name)
        {
            NamedItem[] namedItemArr1 = new NamedItem[list.Count];
            int i1 = 0;
            for (int i2 = 0; i2 < list.Count; i2++)
            {
                NamedItem namedItem = (NamedItem)list[i2];
                if (namedItem.GetName() == name)
                    namedItemArr1[i1++] = namedItem;
            }
            NamedItem[] namedItemArr2 = new NamedItem[i1];
            for (int i3 = 0; i3 < i1; i3++)
            {
                namedItemArr2[i3] = namedItemArr1[i3];
            }
            return namedItemArr2;
        }

        public void Insert(int index, NamedItem a)
        {
            list.Insert(index, a);
        }

        public NamedItem ItemAt(int index)
        {
            return (NamedItem)list[index];
        }

        public void OverwriteItem(NamedItem oldItem, NamedItem newItem)
        {
            // trial
        }

        public void Remove(string name)
        {
            // trial
        }

        public void Remove(NamedItem toBeRemoved)
        {
            list.Remove(toBeRemoved);
        }

    } // class NamedItemCollection

}

