using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// ThreadLocal provides the engine with a way to store information that
    /// is local to the instance and a the thread.  While the CLR provides the
    /// ThreadStatic attribute, it can only be applied to static variables;
    /// some usage patterns in esper (such as statement-specific thread-specific
    /// processing data) require that data be associated by instance and thread.
    /// The CLR provides a solution to this known as LocalDataStoreSlot.  It
    /// has been documented that this method is slower than its ThreadStatic
    /// counterpart, but it allows for instance-based allocation.
    /// <para/>
    /// During recent testing it was determined that the LocalDataStoreSlot was
    /// using an amount of time that seemed a bit excessive.  We took some
    /// snapshots of performance under the profiler.  Using that information we
    /// retooled the class to provide tight and fast access to thread-local
    /// instance-specific data.  The class is pretty tightly wound and takes a
    /// few liberties in understanding how esper uses it.  A ThreadStatic
    /// variable is initialized for the ThreadLocal.  This item is 'thread-local'
    /// and contains an array of 'instance-specific' data.  Indexing is done
    /// when the ThreadLocal item is created.  Under esper this results in roughly
    /// one 'index' per statement.  Changes to this model resulted in good cost
    /// savings in the retrieval and acquisition of local data.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class FastThreadLocal<T> : ThreadLocal<T>
        where T : class
    {
        // Technique    Config      Cycles      time-ms     avg time-us
        // ThreadLocal	Release	    1183734	    6200.5	    5.238085583
        // ThreadLocal	Release	    1224525	    5126.6	    4.186602968
        // ThreadLocal	Release	    1153012	    5935.3	    5.147648073
        // Hashtable	Debug	    1185562	    3848.1	    3.245802413
        // List<T>	    Debug	    996737	    1678	    1.683493238
        // Array	    Debug	    924738	    1032	    1.115991773
        // Array	    Debug	    1179226	    1328.4	    1.126501621
        // Array	    Release	    1224513	    1296.4	    1.058706604

        private static long _typeInstanceId = 0;

        private static readonly Queue<int> _indexReclaim = new Queue<int>();

        private readonly int m_instanceId;

        /// <summary>
        /// Gets the instance id ... if you really must know.
        /// </summary>
        /// <value>The instance id.</value>
        public int InstanceId
        {
            get { return m_instanceId; }
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
        /// Initializes the <see cref="FastThreadLocal&lt;T&gt;"/> class.
        /// </summary>
        static FastThreadLocal()
        {
            _threadDataList = new LinkedList<WeakReference<StaticData>>();
            _threadDataListLock = new FastReaderWriterLock();
        }

        [ThreadStatic]
        private static StaticData _threadData;

        /// <summary>
        /// Factory delegate for construction of data on miss.
        /// </summary>

        private readonly FactoryDelegate<T> m_dataFactory;

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
                int lInstanceId = m_instanceId;
                T[] lThreadData = GetThreadData()._table;
                T value = lThreadData.Length > lInstanceId
                        ? lThreadData[lInstanceId]
                        : default(T);

                return value;
            }

            set
            {
                int lInstanceId = m_instanceId;
                T[] lThreadData = GetThreadData(lInstanceId)._table;
                lThreadData[lInstanceId] = value;
            }
        }

        /// <summary>
        /// Gets the data or creates it if not found.
        /// </summary>
        /// <returns></returns>
        public T GetOrCreate()
        {
            int lInstanceId = m_instanceId;
            T[] lThreadData = GetThreadData(lInstanceId)._table;
            T value;

            if ((value = lThreadData[lInstanceId]) == null)
            {
                lThreadData[lInstanceId] = value = m_dataFactory();
            }

            return value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            int lInstance = m_instanceId;

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
        
        #region ISerializable Members
        #if false
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_dataFactory", m_dataFactory, typeof(FactoryDelegate<T>));
        }
        #endif
        #endregion

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FastThreadLocal&lt;T&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~FastThreadLocal()
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
        /// Initializes a new instance of the <see cref="FastThreadLocal&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public FastThreadLocal( FactoryDelegate<T> factory )
        {
            m_instanceId = AllocateIndex();
            m_dataFactory = factory;
        }

        #if false
        /// <summary>
        /// Initializes a new instance of the <see cref="FastThreadLocal&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="information">The information.</param>
        /// <param name="context">The context.</param>
        public FastThreadLocal(SerializationInfo information, StreamingContext context)
        {
            m_instanceId = AllocateIndex();
            m_dataFactory = (FactoryDelegate<T>) information.GetValue("m_dataFactory", typeof (FactoryDelegate<T>));
        }
        #endif
    }

    /// <summary>
    /// Creates fast thread local objects.
    /// </summary>
    public class FastThreadLocalFactory : ThreadLocalFactory
    {
        #region ThreadLocalFactory Members

        /// <summary>
        /// Create a thread local object of the specified type param.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public ThreadLocal<T> CreateThreadLocal<T>(FactoryDelegate<T> factory) where T : class
        {
            return new FastThreadLocal<T>(factory);
        }

        #endregion
    }
}
