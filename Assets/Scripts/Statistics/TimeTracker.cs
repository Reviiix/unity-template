using System;
using System.Collections;
using UnityEngine;

namespace Statistics
{
    /// <summary>
    /// This class tracks game time.
    /// </summary>
    public class TimeTracker
    {
        public const string TimeDisplayPrefix = "TIME: ";
        private const string TimeFormat = @"m\:ss\.ff";
        private bool _trackTime;
        private MonoBehaviour _coRoutineHandler;
        private Coroutine _timeTrackingRoutine;
        private DateTime _gameStartTime = DateTime.Now;
        private DateTime _pauseStartTime = DateTime.Now;
        private DateTime _pauseEndTime = DateTime.Now;
        private TimeSpan _timePaused = TimeSpan.Zero;
        private TimeSpan _currentTime;

        public void StartTimer(MonoBehaviour coRoutineHandler)
        {
            _coRoutineHandler = coRoutineHandler;
            _trackTime = true;
            _gameStartTime = DateTime.Now;
            _timeTrackingRoutine = _coRoutineHandler.StartCoroutine(TrackTime(_gameStartTime));
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
            
            _trackTime = true;
            _gameStartTime += _timePaused;
            _timeTrackingRoutine = _coRoutineHandler.StartCoroutine(TrackTime(_gameStartTime));
        }
        
        public void StopTimer()
        {
            _trackTime = false;
            OnTimerStop();
        }
    
        private IEnumerator TrackTime(DateTime startingTime)
        {
            while (_trackTime)
            {
                _currentTime = TrackTimeFrom(startingTime);
                if (_trackTime == false)
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
                _coRoutineHandler.StopCoroutine(_timeTrackingRoutine);
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
