using System;

using com.espertech.esper.type;
using com.espertech.esper.util;

namespace com.espertech.esper.schedule
{
	/// <summary>
    /// This class is a slot in a <see cref="ScheduleBucket"/> for sorting schedule service callbacks.
    /// </summary>

    public class ScheduleSlot
		: IComparable<ScheduleSlot>
		, MetaDefItem
	{
		private readonly int bucketNum;
		private readonly int slotNum;

        /// <summary>
        /// Returns the bucket number.
        /// </summary>
        public int BucketNum
        {
            get { return bucketNum; }
        }

        /// <summary>
        /// Returns the slot number.
        /// </summary>
        public int SlotNum
        {
            get { return slotNum; }
        }

        /// <summary> Ctor.</summary>
		/// <param name="bucketNum">is the number of the bucket the slot belongs to
		/// </param>
		/// <param name="slotNum">is the slot number for ordering within the bucket
		/// </param>
		public ScheduleSlot(int bucketNum, int slotNum)
		{
			this.bucketNum = bucketNum;
			this.slotNum = slotNum;
		}

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="scheduleCallbackSlot">The schedule callback slot.</param>
        /// <returns></returns>
		public virtual int CompareTo(ScheduleSlot scheduleCallbackSlot)
		{
			if (this.bucketNum > scheduleCallbackSlot.bucketNum)
			{
				return 1;
			}
			if (this.bucketNum < scheduleCallbackSlot.bucketNum)
			{
				return - 1;
			}
			if (this.slotNum > scheduleCallbackSlot.slotNum)
			{
				return 1;
			}
			if (this.slotNum < scheduleCallbackSlot.slotNum)
			{
				return - 1;
			}
			
			return 0;
		}

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public virtual int CompareTo(Object obj)
		{
            return CompareTo(obj as ScheduleSlot);
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override String ToString()
        {
            return "bucket/slot=" + bucketNum + "/" + slotNum;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ScheduleSlot that = (ScheduleSlot)obj;

            if (bucketNum != that.bucketNum)
            {
                return false;
            }
            if (slotNum != that.slotNum)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            int result;
            result = bucketNum;
            result = 31 * result + slotNum;
            return result;
        }
	}
}