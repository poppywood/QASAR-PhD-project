using System;
using System.Collections.Generic;
using System.Text;

namespace com.espertech.esper.compat
{
    public class SetUtil
    {
        /// <summary>
        /// Creates the union of two sets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set1">The set1.</param>
        /// <param name="set2">The set2.</param>
        /// <returns></returns>
        public static Set<T> Union<T>(ICollection<T> set1, ICollection<T> set2)
        {
            Set<T> iset = new HashSet<T>();

            iset.AddAll(set1);
            iset.AddAll(set2);

            return iset;
        }

        /// <summary>
        /// Creates the intersection of two sets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set1">The set1.</param>
        /// <param name="set2">The set2.</param>
        /// <returns></returns>
        public static Set<T> Intersect<T>( ICollection<T> set1, ICollection<T> set2 )
        {
            Set<T> iset = new HashSet<T>();

            if ((set1 != null) && (set2 != null)) {
                // Reverse the sets if set1 is larger than set2
                if (set1.Count > set2.Count) {
                    ICollection<T> temp = set1;
                    set2 = set1;
                    set1 = temp;
                }

                foreach (T item in set2) {
                    if (set1.Contains(item)) {
                        iset.Add(item);
                    }
                }
            }

            return iset;
        }
    }
}
