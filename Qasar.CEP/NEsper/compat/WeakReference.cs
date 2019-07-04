using System;
using System.Collections.Generic;
using System.Text;

namespace com.espertech.esper.compat
{
	public class WeakReference<T> : WeakReference where T : class
	{
        private readonly int hashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
		public WeakReference( T target )
			: base( target, false )
		{
		    hashCode = target.GetHashCode();
		}

        /// <summary>
        /// Gets a value indicating whether this instance is dead.
        /// </summary>
        /// <value><c>true</c> if this instance is dead; otherwise, <c>false</c>.</value>
	    public bool IsDead
	    {
            get { return !IsAlive; }
	    }

        /// <summary>
        /// Gets or sets the object (the target) referenced by the current <see cref="T:System.WeakReference"></see> object.
        /// </summary>
        /// <value></value>
        /// <returns>null if the object referenced by the current <see cref="T:System.WeakReference"></see> object has been garbage collected; otherwise, a reference to the object referenced by the current <see cref="T:System.WeakReference"></see> object.</returns>
        /// <exception cref="T:System.InvalidOperationException">The reference to the target object is invalid. This can occur if the current <see cref="T:System.WeakReference"></see> object has been finalized.</exception>
		public new T Target
		{
			get { return (T) base.Target; }
		}

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return hashCode;
        }
	}
}
