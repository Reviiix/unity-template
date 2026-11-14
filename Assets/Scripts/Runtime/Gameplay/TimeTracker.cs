using System;
using UnityEngine;

namespace UnityTemplate.Gameplay
{
    /// <summary>Measures stage time on demand from the real-time clock, excluding paused time, without any per-frame work.</summary>
    public class TimeTracker
    {
        private const string TimeFormat = @"m\:ss\.ff";
        private double accumulatedSeconds;
        private double runningSince;

        public bool IsRunning { get; private set; }
        public TimeSpan Elapsed => TimeSpan.FromSeconds(accumulatedSeconds + (IsRunning ? Time.realtimeSinceStartupAsDouble - runningSince : 0));

        /// <summary>Starts timing from zero.</summary>
        public void Start()
        {
            accumulatedSeconds = 0;
            Resume();
        }

        /// <summary>Stops the clock, keeping the time measured so far.</summary>
        public void Pause()
        {
            if (!IsRunning) return;

            accumulatedSeconds += Time.realtimeSinceStartupAsDouble - runningSince;
            IsRunning = false;
        }

        /// <summary>Continues timing after a pause.</summary>
        public void Resume()
        {
            if (IsRunning) return;

            runningSince = Time.realtimeSinceStartupAsDouble;
            IsRunning = true;
        }

        /// <summary>Returns the elapsed time as minutes:seconds.hundredths.</summary>
        public string GetFormattedTime()
        {
            return Elapsed.ToString(TimeFormat);
        }
    }
}
