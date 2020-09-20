using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Statistics
{
    public static class TimeTracker
    {
        public const string TimeDisplayPrefix = "TIME: ";
        private const string TimeFormat = @"m\:ss\.ff";
        private static bool _trackTime;
        private static MonoBehaviour _coRoutineHandler;
        private static TMP_Text _timeText;
        private static Coroutine _timeTracker;
        private static DateTime _gameStartTime;
        private static DateTime _pauseStartTime;
        private static DateTime _pauseEndTime;
        private static TimeSpan _timePaused;
        private static TimeSpan _currentTime;

        public static void Initialise(TMP_Text timeDisplay)
        {
            ResolveDependencies(timeDisplay);
            Reset();
        }

        private static void ResolveDependencies(TMP_Text timeDisplay)
        {
            _timeText = timeDisplay;
        }

        private static void Reset()
        {
            StopTimer();

            _gameStartTime = DateTime.Now;
            _pauseStartTime = DateTime.Now;
            _pauseEndTime = DateTime.Now;
            
            _timePaused = TimeSpan.Zero;
            
            UpdateTimeDisplay(_timeText, DateTime.Now);
        }

        public static void StartTimer(MonoBehaviour coRoutineHandler)
        {
            _coRoutineHandler = coRoutineHandler;
            _trackTime = true;
            _gameStartTime = DateTime.Now;
            _timeTracker = _coRoutineHandler.StartCoroutine(TrackTime(_gameStartTime));
        }
        
        public static void PauseTimer()
        {
            _pauseStartTime = DateTime.Now;
            StopTimer();
        }

        public static void ResumeTimer()
        {
            _pauseEndTime = DateTime.Now;
            _timePaused = _pauseEndTime - _pauseStartTime;
            
            _trackTime = true;
            _gameStartTime += _timePaused;
            _timeTracker = _coRoutineHandler.StartCoroutine(TrackTime(_gameStartTime));
        }
        
        public static void StopTimer()
        {
            _trackTime = false;
            OnTimerStop();
        }
    
        private static IEnumerator TrackTime(DateTime startingTime)
        {
            while (_trackTime)
            {
                if (_trackTime == false)
                {
                    OnTimerStop();
                    yield return null;
                }
                UpdateTimeDisplay(_timeText, startingTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private static void OnTimerStop()
        {
            if (_timeTracker != null)
            {
                _coRoutineHandler.StopCoroutine(_timeTracker);
            }
        }

        private static void UpdateTimeDisplay(TMP_Text display, DateTime startingTime)
        {
            _currentTime = TrackTimeFrom(startingTime);
            display.text = TimeDisplayPrefix + ReturnCurrentTimeAsFormattedString();
        }
    
        private static TimeSpan TrackTimeFrom(DateTime originalTime)
        {
            return(DateTime.Now - originalTime);
        }

        public static string ReturnCurrentTimeAsFormattedString()
        {
            return _currentTime.ToString(TimeFormat);
        }
    }
}
