using System;
using System.Collections.Generic;
using System.Threading;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// FastThreadStore is a variation of the FastThreadLocal, but it lacks a factory
    /// for object creation.  While there are plenty of cases where this makes sense,
    /// we actually did this to work around an issue in .NET 3.5 SP1.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    [Serializable]
    public class FastThreadStore<T>
        where T : class
    {
        private static long _typeInstanceId = 0;

        private static readonly Queue<int> _indexReclaim = new Queue<int>();

        [NonSerialized]
        private int m_instanceId = 0;

        /// <summary>
        /// Gets the instance id ... if you really must know.
        /// </summary>
        /// <value>The instance id.</value>
        public int InstanceId
        {
            get
            {
                int temp;
                while(( temp = Interlocked.CompareExchange(ref m_instanceId, -1, 0) ) <= 0) {
                    if ( temp == 0 ) {
                        Interlocked.Exchange(ref m_instanceId, temp = AllocateIndex());
                        return temp;
                    } else {
                        Thread.Sleep(0);
                    }
                }

                return temp;
            }
        }

        internal class StaticData
        {
            internal T[] _table;
            internal int _count;
            internal T   _last;
            internal int _lastIndx;

            internal StaticData()
            {
                _table = new T[100];
                _count = _table.Length;
                _last = default(T);
                _lastIndx = -1;
            }
        }

        /// <summary>
        /// List of weak reference data.  This list is allocated when the
        /// class is instantiated and keeps track of data that is allocated
        /// regardless of thread.  Minimal locks should be used to ensure
        /// that normal ThreadLocal activity is not placed in the crossfire
        /// of this structure.
        /// </summary>
        private static readonly LinkedList<WeakReference<StaticData>> _threadDataList;

        /// <summary>
        /// Lock for the _threadDataList
        /// </summary>
        private static readonly FastReaderWriterLock _threadDataListLock;

        /// <summary>
        /// Initializes the <see cref="FastThreadStore&lt;T&gt;"/> class.
        /// </summary>
        static FastThreadStore()
        {
            _threadDataList = new LinkedList<WeakReference<StaticData>>();
            _threadDataListLock = new FastReaderWriterLock();
        }

        [ThreadStatic]
        private static StaticData _threadData;

        private static StaticData GetThreadData()
        {
            StaticData lThreadData = _threadData;
            if (lThreadData == null)
            {
                _threadData = lThreadData = new StaticData();
                using (_threadDataListLock.WriteLock.Acquire())
                {
                    _threadDataList.AddLast(new WeakReference<StaticData>(_threadData));
                }
            }

            return lThreadData; //._table;
        }

        private static StaticData GetThreadData(int index)
        {
            StaticData lThreadData = _threadData;
            if (lThreadData == null)
            {
                _threadData = lThreadData = new StaticData();
                using (_threadDataListLock.WriteLock.Acquire())
                {
                    _threadDataList.AddLast(new WeakReference<StaticData>(_threadData));
                }
            }

            T[] lTable = lThreadData._table;
            if (index >= lThreadData._count)
            {
                Rebalance(lThreadData, index, lTable);
            }

            return lThreadData;
        }

        private static void Rebalance(StaticData lThreadData, int index, T[] lTable)
        {
            T[] tempTable = new T[index + 100 - index%100];
            Array.Copy(lTable, tempTable, lTable.Length);
            lThreadData._table = lTable = tempTable;
            lThreadData._count = lTable.Length;
            //return lTable;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get
            {
                int lInstanceId = InstanceId;
                T[] lThreadData = GetThreadData()._table;
                T value = lThreadData.Length > lInstanceId
                        ? lThreadData[lInstanceId]
                        : default(T);

                return value;
            }

            set
            {
                int lInstanceId = InstanceId;
                T[] lThreadData = GetThreadData(lInstanceId)._table;
                lThreadData[lInstanceId] = value;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            int lInstance = InstanceId;

            using (_threadDataListLock.ReadLock.Acquire())
            {
                LinkedList<WeakReference<StaticData>>.Enumerator threadDataEnum =
                    _threadDataList.GetEnumerator();
                while (threadDataEnum.MoveNext())
                {
                    WeakReference<StaticData> threadDataRef = threadDataEnum.Current;
                    if (threadDataRef.IsAlive)
                    {
                        StaticData threadData = threadDataRef.Target;
                        if (threadData != null)
                        {
                            if (threadData._count > lInstance)
                            {
                                threadData._table[lInstance] = null;
                            }

                            continue;
                        }
                    }

                    // Anything making it to this point indicates that the thread
                    // has probably terminated and we are still keeping it's static
                    // data weakly referenced in the threadDataList.  We can safely
                    // remove it, but it needs to be done with a writerLock.
                }
            }

            lock (_indexReclaim)
            {
                _indexReclaim.Enqueue(lInstance);
            }
        }
        
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FastThreadStore&lt;T&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~FastThreadStore()
        {
            Dispose();
        }

        /// <summary>
        /// Allocates a usable index.  This method looks in the indexReclaim
        /// first to determine if there is a slot that has been released.  If so,
        /// it is reclaimed.  If no space is available, a new index is allocated.
        /// This can lead to growth of the static data table.
        /// </summary>
        /// <returns></returns>
        private static int AllocateIndex()
        {
            if (_indexReclaim.Count != 0)
            {
                lock (_indexReclaim)
                {
                    if (_indexReclaim.Count != 0)
                    {
                        return _indexReclaim.Dequeue() ;
                    }
                }
            }

            return (int) Interlocked.Increment(ref _typeInstanceId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastThreadStore&lt;T&gt;"/> class.
        /// </summary>
        public FastThreadStore()
        {
            m_instanceId = AllocateIndex();
        }
    }
}
