using System.Collections;
using System.Threading;

namespace WSQoSAPI
{

    public class NamedItemCollection
    {

        private System.Collections.ArrayList list;
        private System.Threading.Mutex ManilpulateList;
        private System.Collections.ArrayList unsyncronizedList;

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public NamedItemCollection()
        {
            ManilpulateList = new System.Threading.Mutex();
            unsyncronizedList = new System.Collections.ArrayList();
            list = System.Collections.ArrayList.Synchronized(unsyncronizedList);
        }

        public int Add(WSQoSAPI.NamedItem a)
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

        public WSQoSAPI.NamedItem GetItemByName(string name)
        {
            // trial
            return null;
        }

        public WSQoSAPI.NamedItem[] GetItemsByName(string name)
        {
            WSQoSAPI.NamedItem[] namedItemArr1 = new WSQoSAPI.NamedItem[list.Count];
            int i1 = 0;
            for (int i2 = 0; i2 < list.Count; i2++)
            {
                WSQoSAPI.NamedItem namedItem = (WSQoSAPI.NamedItem)list[i2];
                if (namedItem.GetName() == name)
                    namedItemArr1[i1++] = namedItem;
            }
            WSQoSAPI.NamedItem[] namedItemArr2 = new WSQoSAPI.NamedItem[i1];
            for (int i3 = 0; i3 < i1; i3++)
            {
                namedItemArr2[i3] = namedItemArr1[i3];
            }
            return namedItemArr2;
        }

        public void Insert(int index, WSQoSAPI.NamedItem a)
        {
            list.Insert(index, a);
        }

        public WSQoSAPI.NamedItem ItemAt(int index)
        {
            return (WSQoSAPI.NamedItem)list[index];
        }

        public void OverwriteItem(WSQoSAPI.NamedItem oldItem, WSQoSAPI.NamedItem newItem)
        {
            // trial
        }

        public void Remove(string name)
        {
            // trial
        }

        public void Remove(WSQoSAPI.NamedItem toBeRemoved)
        {
            list.Remove(toBeRemoved);
        }

    } // class NamedItemCollection

}

