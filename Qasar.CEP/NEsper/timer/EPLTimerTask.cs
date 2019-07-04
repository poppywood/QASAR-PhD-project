///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Timers;

using log4net;

namespace com.espertech.esper.timer
{
    /// <summary>
    /// Timer task to simply invoke the callback when triggered.
    /// </summary>

    sealed class EPLTimerTask
    {
        private readonly TimerCallback timerCallback;
        private bool isCancelled;
        internal bool _enableStats;
        internal long _lastDrift;
        internal long _maxDrift;
        internal long _totalDrift;
        internal long _invocationCount;

        public bool Cancelled
        {
            set { isCancelled = value; }
        }

        public EPLTimerTask(TimerCallback callback)
        {
            this.timerCallback = callback;
            _enableStats = false;
            _lastDrift = 0;
            _maxDrift = 0;
            _totalDrift = 0;
            _invocationCount = 0;
        }

        public void Run(object sender, ElapsedEventArgs e)
        {
            if (!isCancelled)
            {
                if (_enableStats)
                {
                    // If we are called early, then delay will be positive. If we are called late, then the delay will be negative.
                    // NOTE: don't allow _enableStats to be set until future has been set
                    _lastDrift = 0; // no drift detection
                    _totalDrift += _lastDrift;
                    _invocationCount++;
                    if (_lastDrift > _maxDrift)
                        _maxDrift = _lastDrift;
                }

                try
                {
                    timerCallback();
                }
                catch(Exception ex)
                {
                    log.Error("Timer thread caught unhandled exception: " + ex.Message, ex);
                } 
                
            }
        }

        internal void ResetStats()
        {
            _invocationCount = 0;
            _lastDrift = 0;
            _totalDrift = 0;
            _maxDrift = 0;
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}