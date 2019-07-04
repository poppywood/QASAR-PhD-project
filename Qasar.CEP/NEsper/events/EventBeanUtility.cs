using System;
using System.Collections.Generic;
using System.IO;

using com.espertech.esper.compat;
using com.espertech.esper.collection;

namespace com.espertech.esper.events
{
    /// <summary>
    /// Method to GetSelectListEvents events in collections to other collections or other event types.
    /// </summary>

    public class EventBeanUtility
    {
        /// <summary>
        /// Flatten the vector of arrays to an array. Return null if an empty vector was passed, else
        /// return an array containing all the events.
        /// </summary>
        /// <param name="eventVector">vector</param>
        /// <returns>array with all events</returns>

        public static UniformPair<T[]> FlattenList<T>(ICollection<UniformPair<T[]>> eventVector)
        {
            int count = eventVector.Count;
            if (count == 0) {
                return null;
            }

            if (count == 1) {
                return CollectionHelper.First(eventVector);
            }

            int totalNew = 0;
            int totalOld = 0;
            foreach (UniformPair<T[]> pair in eventVector) {
                if (pair != null) {
                    T[] _first = pair.First;
                    T[] _second = pair.Second;

                    if (_first != null) {
                        totalNew += _first.Length;
                    }
                    if (_second != null) {
                        totalOld += _second.Length;
                    }
                }
            }

            if ((totalNew + totalOld) == 0) {
                return null;
            }

            T[] resultNew = null;
            if (totalNew > 0) {
                resultNew = new T[totalNew];
            }

            T[] resultOld = null;
            if (totalOld > 0) {
                resultOld = new T[totalOld];
            }

            int destPosNew = 0;
            int destPosOld = 0;
            foreach (UniformPair<T[]> pair in eventVector) {
                if (pair != null) {
                    T[] _first = pair.First;
                    T[] _second = pair.Second;

                    if (_first != null) {
                        Array.Copy(_first, 0, resultNew, destPosNew, _first.Length);
                        destPosNew += _first.Length;
                    }
                    if (_second != null) {
                        Array.Copy(_second, 0, resultOld, destPosOld, _second.Length);
                        destPosOld += _second.Length;
                    }
                }
            }

            return new UniformPair<T[]>(resultNew, resultOld);
        }

        /// <summary>Flatten the vector of arrays to an array. Return null if an empty vector was passed, elsereturn an array containing all the events.</summary>
        /// <param name="eventVector">vector</param>
        /// <returns>array with all events</returns>

        public static T[] Flatten<T>(IList<T[]> eventVector)
        {
            int count = eventVector.Count;

            if (count == 0)
            {
                return null;
            }

            if (count == 1)
            {
                return eventVector[0];
            }

            List<T> tempList = new List<T>();
            foreach (T[] array in eventVector)
            {
                if (array != null)
                {
                    tempList.AddRange(array);
                }
            }

            return
                (tempList.Count > 0) ?
                (tempList.ToArray()) :
                (null);
        }

        /// <summary>
        /// Flatten the vector of arrays to an array. Return null if an empty vector was passed, else
        /// return an array containing all the events.
        /// </summary>
        /// <param name="updateVector">is a list of updates of old and new events</param>
        /// <returns>array with all events</returns>
        public static UniformPair<T[]> FlattenBatchStream<T>(IList<UniformPair<T[]>> updateVector)
        {
            int count = updateVector.Count;
            if (count == 0)
            {
                return new UniformPair<T[]>(null, null);
            }

            if (count == 1)
            {
                return new UniformPair<T[]>(
                    updateVector[0].First,
                    updateVector[0].Second);
            }

            int totalNewEvents = 0;
            int totalOldEvents = 0;
            foreach (UniformPair<T[]> pair in updateVector)
            {
                T[] first = pair.First;
                T[] second = pair.Second;

                if (first != null)
                {
                    totalNewEvents += first.Length;
                }

                if (second != null)
                {
                    totalOldEvents += second.Length;
                }
            }

            if ((totalNewEvents == 0) && (totalOldEvents == 0))
            {
                return new UniformPair<T[]>(null, null);
            }

            T[] newEvents = null;
            T[] oldEvents = null;
            if (totalNewEvents != 0)
            {
                newEvents = new T[totalNewEvents];
            }
            if (totalOldEvents != 0)
            {
                oldEvents = new T[totalOldEvents];
            }

            int destPosNew = 0;
            int destPosOld = 0;
            foreach (UniformPair<T[]> pair in updateVector)
            {
                T[] newData = pair.First;
                T[] oldData = pair.Second;

                if (newData != null)
                {
                    int newDataLen = newData.Length;
                    Array.Copy(newData, 0, newEvents, destPosNew, newDataLen);
                    destPosNew += newDataLen;
                }
                if (oldData != null)
                {
                    int oldDataLen = oldData.Length;
                    Array.Copy(oldData, 0, oldEvents, destPosOld, oldDataLen);
                    destPosOld += oldDataLen;
                }
            }

            return new UniformPair<T[]>(newEvents, oldEvents);
        }

        /// <summary>Append arrays.</summary>
        /// <param name="source">list of source events</param>
        /// <param name="append">list of events to append</param>
        /// <returns>appended array</returns>

        public static T[] Append<T>(T[] source, T[] append)
        {
            T[] result = new T[source.Length + append.Length];
            Array.Copy(source, 0, result, 0, source.Length);
            Array.Copy(append, 0, result, source.Length, append.Length);
            return result;
        }

        /// <summary>Convert list of events to array, returning null for empty or null lists.</summary>
        /// <param name="eventList">a list of events to convert</param>
        /// <returns>array of events</returns>

        public static T[] ToArray<T>(IList<T> eventList)
        {
            if ((eventList == null) || (eventList.Count == 0))
            {
                return null;
            }

            return CollectionHelper.ToArray(eventList);
        }

        /// <summary>
        /// Returns object array containing property values of given properties, retrieved via EventPropertyGetterinstances.
        /// </summary>
        /// <param name="ev">event to get property values from</param>
        /// <param name="propertyGetters">getters to use for getting property values</param>
        /// <returns>object array with property values</returns>

        public static Object[] GetPropertyArray(EventBean ev, EventPropertyGetter[] propertyGetters)
        {
            Object[] keyValues = new Object[propertyGetters.Length];
            for (int i = 0; i < propertyGetters.Length; i++)
            {
                keyValues[i] = propertyGetters[i].GetValue(ev);
            }
            return keyValues;
        }

        /// <summary>
        /// Returns Multikey instance for given event and getters.
        /// </summary>
        /// <param name="ev">event to get property values from</param>
        /// <param name="propertyGetters">getters for access to properties</param>
        /// <returns>MultiKey with property values</returns>

        public static MultiKeyUntyped GetMultiKey(EventBean ev, EventPropertyGetter[] propertyGetters)
        {
            Object[] keyValues = GetPropertyArray(ev, propertyGetters);
            return new MultiKeyUntyped(keyValues);
        }

        /// <summary>
        /// Format the event and return a string representation.
        /// </summary>
        /// <param name="ev">is the event to format.</param>
        /// <returns>string representation of event</returns>

        public static String PrintEvent(EventBean ev)
        {
            StringWriter writer = new StringWriter();
            PrintEvent(writer, ev);
            return writer.ToString();
        }

        /// <summary>
        /// Prints the event.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="ev">The ev.</param>
        public static void PrintEvent(TextWriter writer, EventBean ev)
        {
            IEnumerable<String> properties = ev.EventType.PropertyNames;
            IEnumerator<String> propEnum = properties.GetEnumerator();

            for (int i = 0; propEnum.MoveNext(); i++)
            {
                String property = propEnum.Current;
                writer.WriteLine("#" + i + "  " + property + " = " + ev[property]);
            }
        }

        /// <summary>
        /// Flattens a list of pairs of join result sets.
        /// </summary>
        /// <param name="joinPostings">is the list</param>
        /// <returns>is the consolidate sets</returns>
        public static UniformPair<Set<MultiKey<EventBean>>> FlattenBatchJoin(IList<UniformPair<Set<MultiKey<EventBean>>>> joinPostings)
        {
            int count = joinPostings.Count;

            if (count == 0)
            {
                return new UniformPair<Set<MultiKey<EventBean>>>(null, null);
            }

            if (count == 1)
            {
                return new UniformPair<Set<MultiKey<EventBean>>>(
                    joinPostings[0].First,
                    joinPostings[0].Second);
            }

            Set<MultiKey<EventBean>> newEvents = new LinkedHashSet<MultiKey<EventBean>>();
            Set<MultiKey<EventBean>> oldEvents = new LinkedHashSet<MultiKey<EventBean>>();

            foreach (UniformPair<Set<MultiKey<EventBean>>> pair in joinPostings)
            {
                Set<MultiKey<EventBean>> newData = pair.First;
                Set<MultiKey<EventBean>> oldData = pair.Second;

                if (newData != null)
                {
                    newEvents.AddAll(newData);
                }
                if (oldData != null)
                {
                    oldEvents.AddAll(oldData);
                }
            }

            return new UniformPair<Set<MultiKey<EventBean>>>(newEvents, oldEvents);
        }
    }
}
