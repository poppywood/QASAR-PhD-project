using System;
using System.Diagnostics;
using System.Threading;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Implements a simple spinLock algorithm.  The spinLock will attempt
    /// to exchange a value atomically.  If the exchange can not be done then
    /// the spinLock will enter a loop for a maximum amount of time as
    /// specified.  In the loop it will use a spinWait to allow the CPU to
    /// idle for a few cycles in an attempt to wait for the resource to be
    /// freed up.  If after a number of attempts the resource has not been
    /// freed, the spinLock will give up its quanta using a sleep.  The sleep
    /// will force the thread to yield and if all goes well releases the thread
    /// (which may be on the same processor) to release the critical resource.
    /// There's no reason to use this as a general purpose lock, monitors do
    /// just fine.
    /// </summary>

	public class SpinLock
	{
		private int myLock; // Internal lock object
		
		/// <summary>
		/// Acquires the lock.  If the lock can be acquired immediately
		/// it does so.  In the event that the lock can not be acquired
		/// the lock will use a spin-lock algorithm to acquire the lock.
		/// </summary>
		
        public bool Enter(int timeoutInMillis)
        {
            if (Interlocked.CompareExchange(ref myLock, 1, 0) == 0)
            {
                return true;
            }
            else
            {
		        return EnterMyLockSpin(timeoutInMillis);
            }
        }
		
		/// <summary>
		/// Acquires the lock.  If the lock can be acquired immediately
		/// it does so.  In the event that the lock can not be acquired
		/// the lock will use a spin-lock algorithm to acquire the lock.
		/// </summary>
		
        public void Enter()
        {
            if (Interlocked.CompareExchange(ref myLock, 1, 0) != 0)
            {
                EnterMyLockSpin();
            }
        }

        private void EnterMyLockSpin() 
        {
            for (int i = 0 ;; i++) 
            {
            	if (i < 3 && Environment.ProcessorCount > 1) {
                    Thread.SpinWait(20);    // Wait a few dozen instructions to let another processor release lock. 
            	} else {
                    Thread.Sleep(0);        // Give up my quantum.  
                }

                if (Interlocked.CompareExchange(ref myLock, 1, 0) == 0) {
                    return;
                }
            }
        }
        
        /// <summary>
        /// Enters the lock spin with a timeout.  Returns true if the
        /// lock was acquired within the time allotted.
        /// </summary>
        /// <param name="timeoutInMillis"></param>
        /// <returns></returns>
        
        private bool EnterMyLockSpin(int timeoutInMillis)
        {
        	int endTime = Environment.TickCount + timeoutInMillis;
        	
            for (int i = 0; ; i++)
            {
            	if (Environment.TickCount > endTime) {
            		return false ;
            	} else if (i < 3 && Environment.ProcessorCount > 1) {
                    Thread.SpinWait(20);    // Wait a few dozen instructions to let another processor release lock. 
            	} else {
                    Thread.Sleep(0);        // Give up my quantum.  
            	}
            	
            	if (Interlocked.CompareExchange(ref myLock, 1, 0) == 0) {
                    return true;
            	}
            }
        }
        
		/// <summary>
		/// Releases the lock, allowing waiters to proceed.
		/// </summary>
        
        public void Release()
        {
            Debug.Assert(myLock != 0, "Exiting spin lock that is not held");
		    Interlocked.Decrement(ref myLock);
            //myLock = 0;
        }
	}
}
