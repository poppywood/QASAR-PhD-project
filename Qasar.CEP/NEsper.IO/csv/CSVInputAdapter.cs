using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using net.esper.adapter;
using net.esper.client;
using net.esper.compat;
using net.esper.core;
using net.esper.events;
using net.esper.util;

using org.apache.commons.logging;

namespace net.esper.adapter.csv
{
    /// <summary>An event Adapter that uses a CSV file for a source.</summary>

    public class CSVInputAdapter : AbstractCoordinatedAdapter, InputAdapter
    {
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Int32? eventsPerSec;
        private CSVReader reader;
        private EDictionary<String, ObjectFactory<String>> propertyConstructors;
        private String[] propertyOrder;
        private readonly CSVInputAdapterSpec adapterSpec;
        private IDictionary<String, Type> propertyTypes;
        private readonly String eventTypeAlias;
        private long? lastTimestamp = 0;
        private long totalDelay;
        internal bool atEOF = false;
        internal String[] firstRow;

        /// <summary>Ctor.</summary>
        /// <param name="epService">provides the engine runtime and services</param>
        /// <param name="spec">the parameters for this adapter</param>

        public CSVInputAdapter(EPServiceProvider epService, CSVInputAdapterSpec spec)
            : base(epService, spec.IsUsingEngineThread)
        {
            adapterSpec = spec;
            eventTypeAlias = adapterSpec.EventTypeAlias;
            eventsPerSec = spec.EventsPerSec;

            if (epService != null)
            {
                FinishInitialization(epService, spec);
            }
        }

        /// <summary>Ctor.</summary>
        /// <param name="epService">provides the engine runtime and services</param>
        /// <param name="adapterInputSource">the source of the CSV file</param>
        /// <param name="eventTypeAlias">the alias of the Map event to create from the CSV data</param>

        public CSVInputAdapter(EPServiceProvider epService, AdapterInputSource adapterInputSource, String eventTypeAlias)
            : this(epService, new CSVInputAdapterSpec(adapterInputSource, eventTypeAlias))
        {
            
        }

        /// <summary>Ctor for adapters that will be passed to an AdapterCoordinator.</summary>
        /// <param name="adapterSpec">contains parameters that specify the behavior of the input adapter</param>

        public CSVInputAdapter(CSVInputAdapterSpec adapterSpec)
            : this(null, adapterSpec)
        {
        }

        /// <summary>Ctor for adapters that will be passed to an AdapterCoordinator.</summary>
        /// <param name="adapterInputSource">the parameters for this adapter</param>
        /// <param name="eventTypeAlias">the event type alias name that the input adapter generates events for</param>

        public CSVInputAdapter(AdapterInputSource adapterInputSource, String eventTypeAlias)
            : this(null, adapterInputSource, eventTypeAlias)
        {
        }

        public override SendableEvent Read()
        {
            if (stateManager.State == AdapterState.DESTROYED || atEOF)
            {
                return null;
            }

            try
            {
                if (eventsToSend.IsEmpty)
                {
                    return new SendableMapEvent(NewMapEvent(), eventTypeAlias, totalDelay, scheduleSlot);
                }
                else
                {
                    SendableEvent _event = eventsToSend.First;
                    eventsToSend.Remove(_event);
                    return _event;
                }
            }
            catch (EndOfStreamException)
            {
                log.Debug(".read reached end of CSV file");
                atEOF = true;
                if (stateManager.State == AdapterState.STARTED)
                {
                    Stop();
                }
                else
                {
                    Destroy();
                }
                return null;
            }
        }

        public override EPServiceProvider EPService
        {
            set
            {
                base.EPService = value;
                FinishInitialization(value, adapterSpec);
            }
        }

        /// <summary>
        /// Close the CSVReader.
        /// </summary>

        protected override void Close()
        {
            reader.Close();
        }

        /// <summary>
        /// Remove the first member of eventsToSend. If there isanother record in the CSV file, 
        /// insert the event createdfrom it into eventsToSend.
        /// </summary>

        protected override void ReplaceFirstEventToSend()
        {
            eventsToSend.Remove(eventsToSend.First);
            SendableEvent _event = Read();
            if (_event != null)
            {
                eventsToSend.Add(_event);
            }
        }

        /// <summary>
        /// Reset all the changeable state of this ReadableAdapter, as if it were just created.
        /// </summary>

        protected override void Reset()
        {
            lastTimestamp = 0;
            totalDelay = 0;
            atEOF = false;
            if (reader.IsResettable)
            {
                reader.Reset();
            }
        }

        private void FinishInitialization(EPServiceProvider epService, CSVInputAdapterSpec spec)
        {
            AssertValidParameters(epService, spec);

            EPServiceProviderSPI spi = (EPServiceProviderSPI)epService;

            scheduleSlot = spi.SchedulingService.AllocateBucket().AllocateSlot();

            reader = new CSVReader(spec.AdapterInputSource);
            reader.Looping = spec.IsLooping;

            String[] firstRow = FirstRow;

            EDictionary<String, Type> givenPropertyTypes = ConstructPropertyTypes(spec.EventTypeAlias, EBaseDictionary<string,Type>.AsEDictionary(spec.PropertyTypes), spi.EventAdapterService);

            propertyOrder =
                spec.PropertyOrder ??
                CSVPropertyOrderHelper.ResolvePropertyOrder(firstRow, givenPropertyTypes);

            reader.IsUsingTitleRow = IsUsingTitleRow(firstRow, propertyOrder);
            if (!IsUsingTitleRow(firstRow, propertyOrder))
            {
                this.firstRow = firstRow;
            }

            propertyTypes = ResolvePropertyTypes(givenPropertyTypes);
            if (givenPropertyTypes == null)
            {
            	spi.EventAdapterService.AddMapType(eventTypeAlias, EBaseDictionary<String,Type>.AsEDictionary(propertyTypes));
            }

            this.propertyConstructors = CreatePropertyConstructors(propertyTypes);
        }

        private EDictionary<String, Object> NewMapEvent()
        {
            String[] row = firstRow ?? reader.GetNextRecord();
            firstRow = null;
            UpdateTotalDelay(row, reader.GetAndClearIsReset());
            return CreateMapFromRow(row);
        }

        private static readonly Type[] parameterTypes = new Type[] { typeof(String) };
        private static readonly Dictionary<Type, ObjectFactory<String>> staticTypeTable;

        static CSVInputAdapter()
        {
            staticTypeTable = new Dictionary<Type, ObjectFactory<string>>();
            staticTypeTable[typeof(bool)] = EasyParser<bool>;
            staticTypeTable[typeof(sbyte)] = EasyParser<sbyte>;
            staticTypeTable[typeof(short)] = EasyParser<short>;
            staticTypeTable[typeof(int)] = EasyParser<int>;
            staticTypeTable[typeof(long)] = EasyParser<long>;
            staticTypeTable[typeof(byte)] = EasyParser<byte>;
            staticTypeTable[typeof(ushort)] = EasyParser<ushort>;
            staticTypeTable[typeof(uint)] = EasyParser<uint>;
            staticTypeTable[typeof(ulong)] = EasyParser<ulong>;
            staticTypeTable[typeof(float)] = EasyParser<float>;
            staticTypeTable[typeof(double)] = EasyParser<double>;
            staticTypeTable[typeof(decimal)] = EasyParser<decimal>;

            staticTypeTable[typeof(bool?)] = EasyParser<bool>;
            staticTypeTable[typeof(sbyte?)] = EasyParser<sbyte>;
            staticTypeTable[typeof(short?)] = EasyParser<short>;
            staticTypeTable[typeof(int?)] = EasyParser<int>;
            staticTypeTable[typeof(long?)] = EasyParser<long>;
            staticTypeTable[typeof(byte?)] = EasyParser<byte>;
            staticTypeTable[typeof(ushort?)] = EasyParser<ushort>;
            staticTypeTable[typeof(uint?)] = EasyParser<uint>;
            staticTypeTable[typeof(ulong?)] = EasyParser<ulong>;
            staticTypeTable[typeof(float?)] = EasyParser<float>;
            staticTypeTable[typeof(double?)] = EasyParser<double>;
            staticTypeTable[typeof(decimal?)] = EasyParser<decimal>;

            staticTypeTable[typeof (string)] = ProxyParser;
        }

        /// <summary>
        /// Simplified object construction through type parameters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private static Object ProxyParser(String input)
        {
            return input;
        }

        /// <summary>
        /// Simplified object construction through type parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private static Object EasyParser<T>( String input )
        {
            Object result = Convert.ChangeType(input, typeof (T));
            return result;
        }

        /// <summary>
        /// Retrieves the object factory for a given type. 
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static ObjectFactory<String> ObjectFactoryFor(Type type)
        {
            ObjectFactory<String> factoryObj;

            lock (((ICollection)staticTypeTable).SyncRoot)
            {
                if (!staticTypeTable.TryGetValue(type, out factoryObj))
                {
                    ConstructorInfo constructor = type.GetConstructor(parameterTypes);
                    if ( constructor == null )
                    {
                        throw new EPException("unable to find a usable constructor for " + type.FullName);
                    }

                    factoryObj = new ObjectFactory<string>(
                        delegate(String input)
                            {
                                return constructor.Invoke(new Object[] {input});
                            });

                    staticTypeTable[type] = factoryObj;
                }
            }

            return factoryObj;
        }

        private static EDictionary<String, ObjectFactory<String>> CreatePropertyConstructors(IDictionary<String, Type> propertyTypes)
        {
            EDictionary<String, ObjectFactory<String>> factories = new HashDictionary<String, ObjectFactory<String>>();
            
            foreach( KeyValuePair<String,Type> entry in propertyTypes )
            {
            	String property = entry.Key;
            	Type propertyType = entry.Value;
            	
            	log.Debug(".CreatePropertyConstructors property==" + property + ", type==" + propertyType);

                factories.Put(property, ObjectFactoryFor(propertyType));
            }

            return factories;
        }

        private EDictionary<String, Object> CreateMapFromRow(String[] row)
        {
            EDictionary<String, Object> map = new HashDictionary<String, Object>();

            int count = 0;

            try
            {
                foreach (String property in propertyOrder)
                {
                    // Skip properties that are in the title row but not
                    // part of the map to send
                    if (propertyTypes != null && !propertyTypes.ContainsKey(property))
                    {
                        count++;
                        continue;
                    }

                    String parameter = row[count++];
                    ObjectFactory<String> objectFactory = propertyConstructors.Get(property);
                    Object value = objectFactory.Invoke(parameter);
                    map[property] = value;
                }
            }
            catch (TargetInvocationException e)
            {
                throw new EPException(e);
            }
            return map;
        }

        private static EDictionary<String, Type> ConstructPropertyTypes(
            String eventTypeAlias,
            EDictionary<String, Type> propertyTypesGiven,
            EventAdapterService eventAdapterService)
        {
            EDictionary<String, Type> propertyTypes = new HashDictionary<String, Type>();
            EventType eventType = eventAdapterService.GetEventTypeByAlias(eventTypeAlias);
            if (eventType == null)
            {
                if (propertyTypesGiven != null)
                {
                    eventAdapterService.AddMapType(eventTypeAlias, propertyTypesGiven);
                }
                return propertyTypesGiven;
            }
            if (!eventType.UnderlyingType.Equals(typeof(IDataDictionary)))
            {
                throw new EPException("Alias " + eventTypeAlias + " does not correspond to a map event");
            }
            if (propertyTypesGiven != null && eventType.PropertyNames.Count != propertyTypesGiven.Count)
            {
                throw new EPException("Event type " + eventTypeAlias + " has already been declared with a different number of parameters");
            }
            foreach (String property in eventType.PropertyNames)
            {
                Type type = eventType.GetPropertyType(property);
                if (propertyTypesGiven != null && propertyTypesGiven.Get(property) == null)
                {
                    throw new EPException("Event type " + eventTypeAlias + "has already been declared with different parameters");
                }
                if (propertyTypesGiven != null && !Object.Equals(propertyTypesGiven.Get(property), type))
                {
                    throw new EPException("Event type " + eventTypeAlias + "has already been declared with a different type for property " + property);
                }
                propertyTypes.Put(property, type);
            }
            return propertyTypes;
        }

        private void UpdateTotalDelay(String[] row, bool isFirstRow)
        {
            if (eventsPerSec != null)
            {
                int msecPerEvent = 1000 / eventsPerSec.Value;
                totalDelay += msecPerEvent;
            }
            else if (adapterSpec.TimestampColumn != null)
            {
                long? timestamp = ResolveTimestamp(row);
                if (timestamp == null)
                {
                    throw new EPException("Couldn't resolve the timestamp for record " + CollectionHelper.Render(row));
                }
                else if (timestamp < 0)
                {
                    throw new EPException("Encountered negative timestamp for CSV record : " + CollectionHelper.Render(row));
                }
                else
                {
                    long? timestampDifference = 0;
                    if (timestamp < lastTimestamp)
                    {
                        if (!isFirstRow)
                        {
                            throw new EPException("Subsequent timestamp " + timestamp + " is smaller than previous timestamp " + lastTimestamp);
                        }
                        else
                        {
                            timestampDifference = timestamp;
                        }
                    }
                    else
                    {
                        timestampDifference = timestamp - lastTimestamp;
                    }
                    lastTimestamp = timestamp;
                    totalDelay += timestampDifference.Value;
                }
            }
        }

        private Int64? ResolveTimestamp(String[] row)
        {
            int timestampIndex = GetTimestampIndex(row);
            if (timestampIndex != -1)
            {
                return Int64.Parse(row[timestampIndex]);
            }
            else
            {
                return null;
            }
        }

        private int GetTimestampIndex(String[] row)
        {
            if (adapterSpec.TimestampColumn != null)
            {
                for (int i = 0; i < propertyOrder.Length; i++)
                {
                    if (Object.Equals(propertyOrder[i],adapterSpec.TimestampColumn))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private IDictionary<String, Type> ResolvePropertyTypes(IDictionary<String, Type> propertyTypes)
        {
            if (propertyTypes != null)
            {
                return propertyTypes;
            }

            IDictionary<String, Type> result = new HashDictionary<String, Type>();
            foreach (String property in propertyOrder)
            {
                result[property] = typeof(string);
            }
            return result;
        }

        private bool IsUsingTitleRow(String[] firstRow, String[] propertyOrder)
        {
            if (firstRow == null)
            {
                return false;
            }
            Set<String> firstRowSet = new TreeSet<String>(firstRow);
            Set<String> propertyOrderSet = new TreeSet<String>(propertyOrder);
            return Object.Equals(firstRowSet, propertyOrderSet);
        }

        /// <summary>
        /// Gets the first row.
        /// </summary>
        /// <value>The first row.</value>
        
        private String[] FirstRow
        {
            get
            {
                String[] firstRow;
                try
                {
                	firstRow = reader.GetNextRecord();
                }
                catch (EndOfStreamException)
                {
                    atEOF = true;
                    firstRow = null;
                }
                return firstRow;
            }
        }

        private void AssertValidEventsPerSec(Int32? eventsPerSec)
        {
            if (eventsPerSec != null)
            {
                if (eventsPerSec < 1 || eventsPerSec > 1000)
                {
                    throw new ArgumentException("Illegal value of eventsPerSec:" + eventsPerSec);
                }
            }
        }

        private void AssertValidParameters(EPServiceProvider epService, CSVInputAdapterSpec adapterSpec)
        {
            if (!(epService is EPServiceProviderSPI))
            {
                throw new ArgumentException("Invalid type of EPServiceProvider");
            }

            if (adapterSpec.EventTypeAlias == null)
            {
                throw new ArgumentException("eventTypeAlias cannot be null");
            }

            if (adapterSpec.AdapterInputSource == null)
            {
                throw new ArgumentException("adapterInputSource cannot be null");
            }

            AssertValidEventsPerSec(adapterSpec.EventsPerSec);

            if (adapterSpec.IsLooping && !adapterSpec.AdapterInputSource.IsResettable)
            {
                throw new EPException("Cannot loop on a non-resettable input source");
            }
        }
    }
}
