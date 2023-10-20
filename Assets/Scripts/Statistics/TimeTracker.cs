using System;
using System.Collections;
using PureFunctions;
using UnityEngine;

namespace Statistics
{
    /// <summary>
    /// This class tracks game time.
    /// </summary>
    public class TimeTracker
    {
        public bool Active { get; private set; }
        public const string TimeDisplayPrefix = "TIME: ";
        private const string TimeFormat = @"m\:ss\.ff";
        private Coroutine _timeTrackingRoutine;
        private DateTime _gameStartTime = DateTime.Now;
        private DateTime _pauseStartTime = DateTime.Now;
        private DateTime _pauseEndTime = DateTime.Now;
        private TimeSpan _timePaused = TimeSpan.Zero;
        private TimeSpan _currentTime;

        public void StartTimer()
        {
            Active = true;
            _gameStartTime = DateTime.Now;
            _timeTrackingRoutine = Coroutiner.StartCoroutine(TrackTime(_gameStartTime)).Coroutine;
        }
        
        public void PauseTimer()
        {
            _pauseStartTime = DateTime.Now;
            StopTimer();
        }

        public void ResumeTimer()
        {
            _pauseEndTime = DateTime.Now;
            _timePaused = _pauseEndTime - _pauseStartTime;
            
            Active = true;
            _gameStartTime += _timePaused;
            _timeTrackingRoutine = Coroutiner.StartCoroutine(TrackTime(_gameStartTime)).Coroutine;
        }
        
        public void StopTimer()
        {
            Active = false;
            OnTimerStop();
        }
    
        private IEnumerator TrackTime(DateTime startingTime)
        {
            while (Active)
            {
                _currentTime = TrackTimeFrom(startingTime);
                if (Active == false)
                {
                    OnTimerStop();
                    yield return null;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnTimerStop()
        {
            if (_timeTrackingRoutine != null)
            {
                Coroutiner.StopCoroutine(_timeTrackingRoutine);
            }
        }

        private static TimeSpan TrackTimeFrom(DateTime originalTime)
        {
            return(DateTime.Now - originalTime);
        }

        public string ReturnCurrentTimeAsFormattedString()
        {
            return _currentTime.ToString(TimeFormat);
        }
    }
}
